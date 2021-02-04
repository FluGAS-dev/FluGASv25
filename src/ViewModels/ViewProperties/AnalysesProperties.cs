using FluGASv25.Proc.Flow.Properties;
using FluGASv25.Proc.Flow;
using FluGASv25.Proc.Process;
using FluGASv25.Utils;
using FluGASv25.ViewModels.Base;
using Livet.Commands;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FluGASv25.Proc.Several;

namespace FluGASv25.ViewModels
{
    partial class MainWindowViewModel 
    {
        public const string buttonAnalysis = "  Analysis ";
        public const string buttonCancel = "  Cancel ";
        public const string buttonCreateExcel = "  Create Excel ";
        public const string buttonExcute = "  Execute ";

        // private const string buttonBack = "  Back ";

        private void SelectFile()
        {
            var fastqs = SelectFileDialog(   // windows api dialog.
                                   "Select FastqFiles",
                                    true,
                                    false);
            if (fastqs.Any())
                CreateSelectDataList(fastqs);
        }

        private void SelectFolder()
        {
            var folders = SelectFileDialog(   // windows api dialog.
                                   "Select Folders",
                                    true,
                                    true);
            if (folders.Any())
                CreateSelectDataList(folders);
        }

        private void SelectClearFile()
        {
            SelectDataList.Clear();

        }

        // Folder File Select
        private void CreateSelectDataList(IEnumerable<string> files)
        {
            if (this._selectDataList == null) this._selectDataList = new ObservableCollection<string>();
            var newData = new List<string>();
            var cautionData = new List<string>();
            foreach (var file in files)
            {
                if (!WfComponent.Utils.FileUtils.IsOneByteString(file)) 
                    cautionData.Add(file);
                else
                    newData.Add(file);
            }

            // 同じFolder/Fileがある？
            if (newData.Any())
            {
                foreach(var dat in newData.Distinct())
                {
                    if (!_selectDataList.Contains(dat))
                        _selectDataList.Add(dat);
                }
            }

            // 全角が入って居るディレクトリとかを指定した場合はダイアログだす。
            if (cautionData.Any())
                ShowErrorDialog(
                    "include 2byte chareactor file/folder." + Environment.NewLine + string.Join(Environment.NewLine, cautionData),
                    "invarid data error.");

            SelectedDir = Directory.GetParent(files.First()).FullName;
            RaisePropertyChanged(nameof(SelectDataList));

            // 2020.01.20 混合は認めない。
            InputFolderFileCheck(SelectDataList);
        }

        protected bool InputFolderFileCheck(IEnumerable<string> vs)
        {
            bool isMixed = VariousUtils.IsFolderFast5Fastq(vs);
            
            if (isMixed)
            {
                MessageBox.Show("Both Fast5 and Fastq were chosen.",
                                                "FluGAS not analysis condition,",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                mainLog.Report("selecet data is mixed fastq-file/fast5-data, FluGAS not start analysis. ");
            }
            return isMixed; 
        }



        // Select Data ListView Drag & Drop
        ListenerCommand<IEnumerable<Uri>> m_addItemsCommand;
        // ICommandを公開する
        public ICommand AddItemsCommand
        {
            get
            {
                if (m_addItemsCommand == null)
                    m_addItemsCommand = new ListenerCommand<IEnumerable<Uri>>(AddItems);

                return m_addItemsCommand;
            }
        }

        private void AddItems(IEnumerable<Uri> urilist)
        {
            //var urilist = (IEnumerable<Uri>)arg;
            var list = urilist.Select(s => s.LocalPath).ToList();
            CreateSelectDataList(list);
            // IsDirty = true;
        }

        private void SequencerChange()
        {
            if (_isMinion || _isMiseq) IsParameterEdit = true;
            SelectParameters = GetParamterList();
            RaisePropertyChanged(nameof(SelectedParameter));
        }

        private List<string> minionPatameters;
        private List<string> miseqPatameters;
        private string selectedMinionParameter;
        private string selectedMiseqParameter;
        private List<string> GetParamterList()
        {
            if (_isMinion) { // minion parameter
                minionPatameters = Dao.MinionParameterDao.GetParameters()
                                                .OrderByDescending(s => s.CreateDate)
                                                .Select(s => s.Name)
                                                .Distinct()
                                                .ToList();
                if (string.IsNullOrEmpty(_selectedParameter) || !minionPatameters.Contains(_selectedParameter))
                    SelectedParameter = ConstantValues.DefaultMinionParameterName;
                else
                    SelectedParameter = selectedMinionParameter;

                return minionPatameters;
            }

            if (_isMiseq)
            { // miseq parameter
                miseqPatameters = Dao.MiseqParameterDao.GetParameters()
                                                .OrderByDescending(s => s.CreateDate)
                                                .Select(s => s.Name)
                                                .Distinct()
                                                .ToList();
                if (string.IsNullOrEmpty(_selectedParameter) || !miseqPatameters.Contains(_selectedParameter))
                    SelectedParameter = ConstantValues.DefaultMiseqParameterName;
                else
                    SelectedParameter = selectedMiseqParameter;

                return miseqPatameters;
            }

            return new List<string>();

        }

        // Execute.
        protected void AnalysisExecute()
        {
            System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

            switch(this._analysisButton)
            {
                case buttonAnalysis:
                    Anarysis();
                    return;

                case buttonCancel:
                    ProcessCancel();
                    return;

                default:
                    ShowErrorDialog("Fatal error !!");
                    System.Windows.Application.Current.Shutdown();
                    return;
            }

        }

        private IFlow flow = null;
        private void Anarysis() {

            if (!IsAnalysisOK()) return;   // 必要なものがない。

            // defalt parameter name save.
            if (_isMinion) Properties.Settings.Default.minion_param = _selectedParameter;
            if (_isMiseq) Properties.Settings.Default.miseq_param = _selectedParameter;
            Properties.Settings.Default.Save();

            var analysisProperties = string.Empty;
            AnalysisViewParameters viewParameters = null;

            // minion ?
            if (_isMinion) {

                if (! isFast5Analysis()) return;

                // 2020.01.20 混合は認めない。
                if (InputFolderFileCheck(SelectDataList)) return;

                // var sampleList = new ObservableCollection<Barcode2Name>();

                IEnumerable<Barcode2Name> barcodeList = null ;
                if (_selectDataList.Where(s => Directory.Exists(s) || s.EndsWith(".fast5")).Any())
                {
                    
                    bool isBarcodeComit = false;
                    using (var barcodeView = new BarcodeManagementViewModel(this._selectDataList, this._isOneSample, this._isBarcode))
                    {
                        Messenger.Raise(new TransitionMessage(barcodeView, "BarcodeManagementCommand"));
                        barcodeList = barcodeView.BarcodeList;  // user input sample-names
                        analysisProperties = barcodeView.SelectedConfig;
                        isBarcodeComit = barcodeView.IsCommand;
                    };

                    // barcode view で、ボタン押下以外でCloseした。
                    if (!isBarcodeComit) return;

                }
                viewParameters = GetFlowProperties(barcodeList, analysisProperties);

                if (viewParameters != null)
                    flow = new MinionFlow(viewParameters, mainLog);
            }

            if (_isMiseq)
            {   // Miseq flow
                viewParameters = GetFlowProperties();
                if (viewParameters != null)
                    flow = new MiseqFlow(viewParameters, mainLog);
            }

            // Select folder cancel.
            if (viewParameters == null) return;

            // flow properties が 正常　= ユーザが save Dir を指定した。
            if (flow != null)
            {
                AnalysisButton = buttonCancel; 
                // タスクマネージャ起動
                System.Diagnostics.Process.Start(
                                                new System.Diagnostics.ProcessStartInfo("taskmgr") 
                                                    { UseShellExecute = true } ); 

                _ = ProcessAsync(flow);
            }
        }

        // 各Process の 非同期処理
        private string ProcessResultMessage;
        protected async Task<string> ProcessAsync(IFlow flow)
        {
            // Log puts
            mainLog.Report(ConstantValues.MainLogClear);  // 処理の開始時に下部に表示されているログをクリア
            mainLog.Report("FluGAS version : " + Properties.Settings.Default.version);
            mainLog.Report("----- start analysis. -----");
            mainLog.Report(WfComponent.Utils.FileUtils.LogDateString());

            // 非同期
            ProcessResultMessage = await flow.CallFlowAsync().ConfigureAwait(true);
            mainLog.Report(ProcessResultMessage);
            ProcessEnd(ProcessResultMessage);

            return ProcessResultMessage;
        }

        protected void ProcessCancel()
        {
            mainLog.Report("analysis cancel call ! ");
            this.flow.CancelFlow();
            
            ProcessEnd( ConstantValues.CanceledMessage );
        }


        // 通常は此処に戻る。
        internal void ProcessEnd(string resValue)
        {
            // 作業終了
            mainLog.Report(ConstantValues.EndAnalysis);

            var message = string.Empty;
            var logfile = WfComponent.Utils.FileUtils.GetUniqDateLogFile(ConstantValues.EndAnalysisLog);
            WfComponent.Utils.FileUtils.WriteFileFromString(logfile, LogMessage, ref message);
            if (! string.IsNullOrEmpty(message))
                mainLog.Report("error report , file write error. " + message);


            // 出力ディレクトリ先にも。
            if (Directory.Exists(userOutDir))
            {
                var outlog = Path.Combine(userOutDir,
                                    Path.GetFileName(logfile));
                WfComponent.Utils.FileUtils.WriteFileFromString(outlog, LogMessage, ref message);
            }
            if(! string.IsNullOrEmpty(message))
                mainLog.Report("error report , file write error. " + message);

            AnalysisButton = buttonAnalysis;
            SetDetailClear();
            ResultsTabOnload();

            // 終了ダイアログ。
            MessageBox.Show("Processing finished" + Environment.NewLine + resValue, 
                            flow.GetType().ToString(),
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            // ダイアログ出す前に削除されていたらException
            if(Directory.Exists(userOutDir))
                System.Diagnostics.Process.Start(
                                new System.Diagnostics.ProcessStartInfo(userOutDir)
                                { UseShellExecute = true });
        }

        protected string userOutDir;
        protected AnalysisViewParameters GetFlowProperties(IEnumerable<Barcode2Name> barcodeList = null, string analysisProperties = null)
        {
            var outdir = SelectFileDialog("save folder", false, true);
            if(outdir != null && outdir.Any()) {

                userOutDir = outdir.First();
                // パラメータチェック済み。
                return new AnalysisViewParameters()
                {
                    ParamName = this._selectedParameter,
                    OutDir = userOutDir,
                    SelectedFolderFiles = _selectDataList,
                    IsMinion = _isMinion,
                    IsMiseq = _isMiseq,
                    IsUseBarcode = _isBarcode,
                    IsOneRun = _isOneSample,
                    IsFluA = _isFluA,
                    IsFluB = _isFluB,
                    DoSegment = GetDoSegment(),
                    GuppyConfigName = analysisProperties, // 注意 miseq の時は null
                    BarcodeList = barcodeList,
                };
            }
            // save folder で Cancel したとか。
            return null;
        }

        // View 画面で選択している対象セグメント
        private IEnumerable<string> GetDoSegment()
        {
            var doSegment = new List<string>();
            if (IsHA)  doSegment.Add(FluTypeSeparate.ha.ToUpper());
            if (IsMP) doSegment.Add(FluTypeSeparate.mp.ToUpper());
            if (IsNA) doSegment.Add(FluTypeSeparate.na.ToUpper());
            if (IsNP) doSegment.Add(FluTypeSeparate.np.ToUpper());
            if (IsNS) doSegment.Add(FluTypeSeparate.ns.ToUpper());
            if (IsPA) doSegment.Add(FluTypeSeparate.pa.ToUpper());
            if (IsPB1) doSegment.Add(FluTypeSeparate.pb1.ToUpper());
            if (IsPB2) doSegment.Add(FluTypeSeparate.pb2.ToUpper());

            if (!doSegment.Any())
                return CommonFlow.SEGMENTS;

            return doSegment;
        }

        // Parameter 画面をだす
        private void EditParameter()
        {
            System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);
            if (this._isMiseq)
            {
                using (var paramView = new MiseqParameterViewModel(this._selectedParameter))
                {
                    Messenger.Raise(new TransitionMessage(paramView, "MiseqParameterEditCommand"));
                    selectedMiseqParameter = string.IsNullOrWhiteSpace(paramView.CurrentParameterName) ?
                                                            ConstantValues.DefaultMiseqParameterName :
                                                            paramView.Name;
                    // if (miseqPatameters != null)
                     //   miseqPatameters.Clear();
                };

            } 
            else
            {  // default .MinionParameter
                using (var paramView = new MinionParameterViewModel(this._selectedParameter))
                {
                    Messenger.Raise(new TransitionMessage(paramView, "MinionParameterEditCommand"));
                    selectedMinionParameter = string.IsNullOrWhiteSpace(paramView.CurrentParameterName) ?
                                                            ConstantValues.DefaultMinionParameterName :
                                                            paramView.Name;
                    // if (minionPatameters != null)
                    //    minionPatameters.Clear();
                };
            }
            SelectParameters = GetParamterList();
        }

        // DataList フォルダー・ファイルの削除
        public string SelectDataItem { get; set; }
        private void RemoveDataItem()
        {
            System.Diagnostics.Debug.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (string.IsNullOrEmpty(this.SelectDataItem)) return;
            if (this._selectDataList.Contains(SelectDataItem)) {
                
                var res = this._selectDataList.Remove(SelectDataItem);
                System.Diagnostics.Debug.WriteLine("remove dat : " + this.SelectDataItem + "  res :" + res);
            }

            RaisePropertyChanged(nameof(SelectDataList));
        }


        private readonly string selectDataListPropertyName = "SelectDataList";
        private readonly string isFluAPropertyName = "IsFluA";
        private readonly string isFluBPropertyName = "IsFluB";
        protected bool IsAnalysisOK()
        {
            bool isAnalysis = true;
            if (_isFluA || _isFluB)
            {
                RemoveError(isFluAPropertyName);
                RemoveError(isFluBPropertyName);
            }
            else
            {
                AddError(isFluAPropertyName, "no check.");
                AddError(isFluBPropertyName, "no check.");
                isAnalysis = false;
            }

            if (_selectDataList == null || !_selectDataList.Any())
            {
                AddError(selectDataListPropertyName, "no data.");
                isAnalysis = false;
            }
            else
            {
                RemoveError(selectDataListPropertyName);
            }

            if (! _isMinion && ! _isMiseq && !isAnalysis)
            {
                ShowErrorDialog("no select sequenceer");
            }


            return isAnalysis;
        }

        private bool _isMiseq;
        public bool IsMiseq
        {
            get => _isMiseq;
            set
            {
                if (RaisePropertyChangedIfSet(ref _isMiseq, value) && _isMiseq)
                {
                    this.IsMinion = false;
                    SequencerChange();
                }
            }
        }

        private bool _isMinion;
        public bool IsMinion
        {
            get => _isMinion;
            set
            {
                if (RaisePropertyChangedIfSet(ref _isMinion, value) && _isMinion)
                {
                    this.IsMiseq = false;
                    SequencerChange();
                }
            }
        }

        private bool _isMinionEnabled;
        public bool IsMinionEnabled
        {
            get => _isMinionEnabled;
            set { RaisePropertyChangedIfSet(ref _isMinionEnabled, value); }
        }

        private bool _isOneSample = true;
        public bool IsOneSample
        {
            get => _isOneSample;
            set { RaisePropertyChangedIfSet(ref _isOneSample, value); }

        }

        private bool _isBarcode = true;
        public bool IsBarcode
        {
            get => _isBarcode;
            set { RaisePropertyChangedIfSet(ref _isBarcode, value); }

        }

        private bool _isParameterEdit;
        public bool IsParameterEdit
        {
            get => _isParameterEdit;
            set { RaisePropertyChangedIfSet(ref _isParameterEdit, value); }
        }

        private string _selectedParameter;
        public string SelectedParameter
        {
            get
            {
                if (_isMinion)
                {
                    if (minionPatameters != null && minionPatameters.Contains(selectedMinionParameter))
                    {
                        _selectedParameter = selectedMinionParameter;
                    }
                    else
                    {
                        _selectedParameter = ConstantValues.DefaultMinionParameterName;
                        selectedMinionParameter = ConstantValues.DefaultMinionParameterName;
                    }
                }

                if (_isMiseq)
                {
                    if (miseqPatameters != null && miseqPatameters.Contains(selectedMiseqParameter))
                    {
                        _selectedParameter = selectedMiseqParameter;
                    }
                    else
                    {
                        _selectedParameter = ConstantValues.DefaultMiseqParameterName;
                        selectedMiseqParameter = ConstantValues.DefaultMiseqParameterName;
                    }
                }
                return _selectedParameter;
            }
            set 
            {
                if (RaisePropertyChangedIfSet(ref _selectedParameter, value)) { 
                    if (_isMinion) selectedMinionParameter = _selectedParameter;
                    if (_isMiseq) selectedMiseqParameter = _selectedParameter;
                }
            }
        }

        private List<string> _selectParameters;
        public List<string> SelectParameters
        {
            get => _selectParameters;
            set
            {
                if (RaisePropertyChangedIfSet(ref _selectParameters, value))
                    RaisePropertyChanged(nameof(SelectedParameter));
            }
        }

        private bool _isFluA;
        public bool IsFluA
        {
            get => _isFluA;
            set { RaisePropertyChangedIfSet(ref _isFluA, value); }
        }

        private bool _isFluB;
        public bool IsFluB
        {
            get => _isFluB;
            set { RaisePropertyChangedIfSet(ref _isFluB, value); }
        }

        private bool _isHA;
        public bool IsHA
        {
            get => _isHA;
            set { RaisePropertyChangedIfSet(ref _isHA, value); }
        }
        private bool _isMP;
        public bool IsMP
        {
            get => _isMP;
            set { RaisePropertyChangedIfSet(ref _isMP, value); }
        }
        private bool _isNA;
        public bool IsNA
        {
            get => _isNA;
            set { RaisePropertyChangedIfSet(ref _isNA, value); }
        }
        private bool _isNP;
        public bool IsNP
        {
            get => _isNP;
            set { RaisePropertyChangedIfSet(ref _isNP, value); }
        }
        private bool _isNS;
        public bool IsNS
        {
            get => _isNS;
            set { RaisePropertyChangedIfSet(ref _isNS, value); }
        }
        private bool _isPA;
        public bool IsPA
        {
            get => _isPA;
            set { RaisePropertyChangedIfSet(ref _isPA, value); }
        }
        private bool _isPB1;
        public bool IsPB1
        {
            get => _isPB1;
            set { RaisePropertyChangedIfSet(ref _isPB1, value); }
        }
        private bool _isPB2;
        public bool IsPB2
        {
            get => _isPB2;
            set { RaisePropertyChangedIfSet(ref _isPB2, value); }
        }


        private ObservableCollection<string> _selectDataList;
        public ObservableCollection<string> SelectDataList
        {
            get
            {
                if (_selectDataList == null) return new ObservableCollection<string>() { string.Empty };
                return _selectDataList;
            }
        }

        private string _executeButton = buttonExcute;
        public string ExecuteButton
        {
            get => _executeButton;
            set { RaisePropertyChangedIfSet(ref _executeButton, value); }
        }

        private string _createExcelButton = buttonCreateExcel;
        public string CreateExcelButton
        {
            get => _createExcelButton;
            set { RaisePropertyChangedIfSet(ref _createExcelButton, value); }
        }

        protected string _analysisButton = buttonAnalysis;
        public string AnalysisButton
        {
            get => _analysisButton;
            set
            {
                if (RaisePropertyChangedIfSet(ref _analysisButton, value))
                {
                    switch (value)
                    {
                        case buttonAnalysis:
                            CreateExcelButton = buttonCreateExcel;
                            ExecuteButton = buttonExcute;
                            break;

                        case buttonCancel:
                            CreateExcelButton = buttonCancel;
                            ExecuteButton = buttonCancel;
                            break;
                    }
                }
            }
        }

    }
}
