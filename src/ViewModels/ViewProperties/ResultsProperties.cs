using FluGASv25.Models.Properties;
using FluGASv25.Proc.Flow;
using FluGASv25.Proc.Several;
using FluGASv25.Utils;
using FluGASv25.ViewModels.Base;
using FluGASv25.ViewModels.ViewProperties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace FluGASv25.ViewModels
{
    public partial class MainWindowViewModel : ViewModelNotifyDataError
    {

        public string HALabel1 => FluTypeSeparate.ha;
        public string MPLabel1 => FluTypeSeparate.mp;
        public string NALabel1 => FluTypeSeparate.na;
        public string NPLabel1 => FluTypeSeparate.np;
        public string NSLabel1 => FluTypeSeparate.ns;
        public string PALabel1 => FluTypeSeparate.pa;
        public string PB1Label1 => FluTypeSeparate.pb1;
        public string PB2Label1 => FluTypeSeparate.pb2;

        protected int sampleId = int.MinValue;

        private const string detailTabRatio = "ratio";
        private const string detailTabAverage = "avg";
        private const string detailTabSequence = "seq";
        private const string detailTabIncludeN = "inc";
        private const string detailTabCds = "cds";
        private const string detailTabAlign = "align";
        private const char nucleotideN = 'N';
        private string[] dbDelimiter = new string[] { ConstantValues.DbDelimiter };

        // detail tabs
        public IEnumerable<ResultsTabProperties> SampleTabs { get; set; }
        public int TabIndex { get; set; } = 0;

        private Sample _selectSample;
        public Sample SelectSample
        {
            get { return _selectSample; }
            set
            {
                if (value == null) return;
                if (RaisePropertyChangedIfSet(ref _selectSample, value))
                    SetSampleDetail(value);
            }
        }

        // 2020.10.25 DetailTab変更に伴い修正。
        private void SetSampleDetail(Sample s)
        {
            // Listで選択されたサンプル情報を更新
            this.sampleId = s.ID;
            SampleName = s.NAME;
            ExecDate = s.DATE;
            CallType = s.TYPE + " (" + s.SUBTYPE + ")";
            ViewName = s.VIEW_NAME;

            if (s.TYPE == Proc.Flow.CommonFlow.TYPE_A_STR)
                SubTypes = "ha:(" + s.A_HA_CALL + ") na:(" + s.A_NA_CALL + ")";
            else
                SubTypes = string.Empty; // // 2018.12.18 Bの時に詳細は不要。

            var detailTabs = new List<ResultsTabProperties>(); // 6-tab 分
                                                               // include-N 必ず nuc-sequence をセットした後に行う。  配列の真ん中にNが含まれてるかを表示 2019.09.24

            // divid の順番大事。　viewとdbの対応
            var dividPair = new Dictionary<string, string>()
            {
                { detailTabRatio ,      ConstantValues.DbPropertyPrefixCoverRatio },
                { detailTabAverage ,  ConstantValues.DbPropertyPrefixCoverAvarage },
                { detailTabCds ,         ConstantValues.DbPropertyPrefixCdsRegion },
                { detailTabSequence , string.Empty}
            };
            foreach (var type in CommonFlow.VIRUS_TYPES)
                foreach (var idx in Enumerable.Range(1, 3))
                {
                    var tabDetail = new ResultsTabProperties(mainLog) { 
                                                    TabName =  type + idx,
                                                    SelectedSampleId = this.sampleId
                    }; // tab 1個分

                    foreach (var divid in dividPair) 
                        foreach (var segment in CommonFlow.SEGMENTS)
                        {
                            if (string.IsNullOrEmpty(divid.Value))
                                SetDetailTabSequence(s, type.Last(), segment, idx - 1, ref tabDetail); // consensus sequence ....
                            else
                                SetDetailTab(s, type.Last(), segment, divid, idx -1, ref tabDetail);  // ratio ave ....
                        }
                    detailTabs.Add(tabDetail);
                }
            this.SampleTabs = detailTabs;
            RaisePropertyChanged(nameof(SampleTabs));
            this.TabIndex = 0;
            RaisePropertyChanged(nameof(TabIndex));
        }

        private void SetDetailTab(Sample s, char type, string segment, KeyValuePair<string,string> divid, int splitIdx, ref ResultsTabProperties tabProperty)
        {
            // Sample Property ; COVER_RATIO_A_HA とか
            var dbPropartyName = divid.Value + type + "_" + segment; // COVER_RATIO_A_HA とか
            var dbValueObj =  s.GetType().GetProperty(dbPropartyName).GetValue(s);
            var dbValue = dbValueObj == null ? string.Empty : dbValueObj.ToString();

            if (string.IsNullOrEmpty(dbValue) ||
                dbValue.Split(ConstantValues.DbDelimiter).Length <= splitIdx ||
                string.IsNullOrEmpty(dbValue.Split(ConstantValues.DbDelimiter).ElementAt(splitIdx)) ||
                dbValue.Split(ConstantValues.DbDelimiter).ElementAt(splitIdx)  == "0" )
            {  // DB の中に 値を持っていない。                
                switch (divid.Key)
                {
                    case detailTabRatio:
                    case detailTabAverage:
                        dbValue = ConstantValues.DbDefaultFloatValue;
                        break;
                    case detailTabSequence:
                    case detailTabIncludeN:
                        dbValue = string.Empty;
                        break; 
                    case detailTabCds:
                        dbValue = ConstantValues.ViewDelimiter;
                        break; 
                }
                // default value set.
                tabProperty.GetType().GetProperty(segment + divid.Key)
                            .SetValue(tabProperty, dbValue);
                return;
            }

            // db に正常値。
            tabProperty.GetType().GetProperty(segment + divid.Key)
                            .SetValue(tabProperty, 
                                           dbValue.Split(ConstantValues.DbDelimiter).ElementAt(splitIdx));
        }

        private void SetDetailTabSequence(Sample s, char type, string segment,  int splitIdx, ref ResultsTabProperties tabProperty)
        {
            var dbValueObj = s.GetType()   
                                        .GetProperty(type + "_" + segment) // A_HA に コンセンサス配列が入って居る
                                        .GetValue(s);
            var dbValue = dbValueObj == null ? string.Empty : dbValueObj.ToString();
            var seqs = dbValue.Split(ConstantValues.DbDelimiter);

            var detailseq = seqs.Length < 3 ? string.Empty :  seqs.ElementAt(splitIdx).ToUpper();
            tabProperty.GetType()
                                .GetProperty(segment + detailTabSequence) // HAseq
                                .SetValue(tabProperty, detailseq);

            // inc-N
            var nuc = detailseq.TrimStart(nucleotideN).TrimEnd(nucleotideN);
            var incN = nuc.Contains(nucleotideN) ? nucleotideN.ToString() : string.Empty;

            // var nucChars = "[^AGCTN]";
            var altNucsMath = Regex.Matches(nuc, @"[^AGCTN]"); // IUPAC のときはR 表示？

            incN += altNucsMath.Any() ?  "R" : string.Empty;
            tabProperty.GetType()
                                .GetProperty(segment + detailTabIncludeN) // HAinc
                                .SetValue(tabProperty, incN);

            // tablet 起動時のパラメータ
            tabProperty.GetType()
                    .GetProperty(segment + detailTabAlign) // HAalign
                    .SetValue(tabProperty,
                                    type + ConstantValues.ViewDelimiter + segment + ConstantValues.ViewDelimiter + (splitIdx + 1).ToString());// A-HA-1
        }

        // 現在表示している値のクリア。
        private void SetDetailClear()
        {
            this.sampleId = int.MinValue;
            this.SampleName = string.Empty;
            this.CallType = string.Empty;
            this.SubTypes = string.Empty;
            this.ExecDate = string.Empty;
            this.ViewName = string.Empty;
            // var ditaiTabsA  = Enumerable.Range(1, 3)
            //                         .Select(i => new ResultsTabProperties { TabName = "InfluA" + i });
            // var ditaiTabsB = Enumerable.Range(1, 3)
            //                         .Select(i => new ResultsTabProperties { TabName = "InfluB" + i });
            // this.SampleTabs = ditaiTabsA.Except(ditaiTabsB);
            this.SampleTabs = new List<ResultsTabProperties>();
            RaisePropertyChanged(nameof(SampleTabs));
        }

        protected void SampleEdit()
        {
            var hideSampleList = SampleList.Where(s => !s.IsSelected).ToList();
            this.SampleList = hideSampleList;
            //this.SampleList.Remove(selectSample);

            SetDetailClear();
            // FooterMessage = "Select sample : " + this.SampleList.Count();
        }

        protected void SampleDelete()
        {
            string messageBoxText = "Are you sure you want to delete selected data?";
            string caption = "Delete sample";

            // Display message box
            if (ShowConfirmDialog(messageBoxText, caption))
            {

                var deleteSampleList = SampleList.Where(s => s.IsSelected)
                                                                 .Select(s => (long)s.ID)
                                                                 .ToArray();
                // Console.WriteLine("delete sample");

                // リスト更新(DBから更新すると、Hideのものが復活するのでViewの中で完結させる)
                var res = Dao.SampleDao.DeleteSample(deleteSampleList);
                SampleEdit();
                return;
            }
            // Console.WriteLine("Delete Cancel");
        }

        private void CreateExcel()
        {
            var cnt = this.SampleList.Where(s => s.IsSelected).Count();
            if (cnt < 1)  // リストのサンプルが選択されていないー
            {
                ShowErrorDialog("Please select sample.", "no selected data");
                ResultsTabOnload();
                return;
            }

            var s = this.SampleList.Where(s => s.IsSelected).ToList();

            // 保存場所の選択？
            var saveDir = string.IsNullOrWhiteSpace(Properties.Settings.Default.save_dir) ?
                        // System.IO.Path.Combine(@"C:\Users", Environment.UserName, "Documents") :
                        System.Environment.GetFolderPath(Environment.SpecialFolder.Personal):
                        Properties.Settings.Default.save_dir;
            var exlName = this._selectSample.VIEW_NAME + ".xlsx";

            string resMessage = string.Empty;
            string saveExcel = string.Empty;
            try {
                using (var dialog = new CommonOpenFileDialog("Excel save"))
                {
                    // フォルダ選択モード。
                    dialog.IsFolderPicker = false;
                    dialog.Multiselect = false;
                    dialog.InitialDirectory = SelectedDir;
                    dialog.DefaultExtension = ".xlsx";
                    dialog.DefaultFileName = exlName;

                    var ret = dialog.ShowDialog();
                    if (ret == CommonFileDialogResult.Ok) {
                        saveExcel = dialog.FileName;
                        var samples = this._sampleList.Where(s => s.IsSelected).ToArray();
                        resMessage = Proc.Several.GisaidExcel.CreateGsaidExcel(saveExcel, samples);
                    }
                };
                mainLog.Report("created excel,  " + resMessage);

            } catch (Exception e) {
                mainLog.Report("create Excel error,  "+ saveExcel + Environment.NewLine +  e.Message);
            }
        }
  

        // GSAID Excel用に名前を付ける DB.Sample.name update
        private void ViewNameUpdate()
        {
            if (this._selectSample == null) return;
            
            var updateSample = new Sample()
            {
                ID = this._selectSample.ID,
                PRAM_ID = this._selectSample.PRAM_ID,
                VIEW_NAME = this._viewName
            };

            var resId = Dao.SampleDao.UpdateSample(updateSample);
            ResultsTabOnload();
            SetDetailClear();
        }

        public void ResultsTabRequestCommand()
            => ResultsTabOnload();

        private void ResultsTabOnload()
        {
            // Viewの呼び出し直前。
            this.ToDatePick = DateTime.Now.Date;
            this.FromDatePick = DateTime.Now.Date;

            // 実行済みのサンプル情報
            var samples = Dao.SampleDao.GetSamples();
            this.SampleList = samples.ToList();

            this.FooterMessage = string.Empty;
        }

        private string _footerMessage;
        public string FooterMessage
        {
            get { return _footerMessage; }
            set { RaisePropertyChangedIfSet(ref _footerMessage, value); }
        }

        public void CallCurrentDate(string dat)
        {
            // LivetCallMethodAction
            System.Diagnostics.Debug.WriteLine("call current date." + dat);
        }

    }
}
