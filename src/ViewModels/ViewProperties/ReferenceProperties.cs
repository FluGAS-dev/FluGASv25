using FluGASv25.Dao;
using FluGASv25.Models.Properties;
using FluGASv25.Proc.Flow;
using FluGASv25.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FluGASv25.ViewModels
{
    public partial class MainWindowViewModel 
    {
        public string DownloadUrl { get => Properties.Settings.Default.ncbi_reference_url; }

        private string _userFasta;
        public string UserFasta
        {
            get { return _userFasta; }
            set { RaisePropertyChangedIfSet(ref _userFasta, value); }
        }

        private string _yamagataFasta;
        public string YamagataFasta
        {
            get { return _yamagataFasta; }
            set { RaisePropertyChangedIfSet(ref _yamagataFasta, value); }
        }

        private string _victoriaFasta;
        public string VictoriaFasta
        {
            get { return _victoriaFasta; }
            set { RaisePropertyChangedIfSet(ref _victoriaFasta, value); }
        }

        private bool _isRererenceExec = true;
        public bool IsRererenceExec
        {
            get { return _isRererenceExec; }
            set { RaisePropertyChangedIfSet(ref _isRererenceExec, value); }
        }


        private string[] suffixs = new string[]
        {
            ConstantValues.minlength,
            ConstantValues.maxlength,
            ConstantValues.nuc5ds,
            ConstantValues.nuc3ds,
            ConstantValues.includeN,
            ConstantValues.mappingcdhit,
            ConstantValues.blastcdhit,
            ConstantValues.minCDS,
            ConstantValues.maxCDS,

        };

        private void ReferenceUpdate()
        {
            switch (this._analysisButton)
            {
                case buttonAnalysis:
                    ReferenceUpdateCall();
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

        // Update.
        private void ReferenceUpdateCall()
        {
            // parameter check.
            var s = GetErrors(nameof(APB2maxCDS));
            if (HasErrors)  return;

            mainLog.Report("Reference Update call.");

            var referenceDbId = SaveReference();
            if (referenceDbId < 1) {
                mainLog.Report("db error! not insert reference table.");
                return;
            }
            this.flow = new ReferenceUpdateFlow(
                new Proc.Flow.Properties.ReferenceUpdateParameters() { 
                    CurrentInsertId = referenceDbId,
                    UserFastaPath = _userFasta
                    }, mainLog);

            AnalysisButton = buttonCancel;  // cancel 
            _ = ProcessAsync(flow);

        }

        private void SetReferenceDefaultParameters() 
            => SetReferenceParameters(ReferenceDao.GetDefalutParameter());

        private void SetReferenceLastPatrameters()
            => SetReferenceParameters(ReferenceDao.GetLastParameter());

        private void SetReferenceParameters(Reference parameters)
        {
            // var parameters = Dao.ReferenceDao.GetDefalutParameter();
            foreach (var type in CommonFlow.VIRUS_TYPES)
            {
                foreach (var segment in CommonFlow.SEGMENTS)
                {
                    foreach (var suffix in suffixs) { 
                        var typeSegment = type.Last() + segment + suffix;  // AHAminLength

                        var dbValue = parameters.
                                                        GetType().
                                                        GetProperty(typeSegment). // 
                                                        GetValue(parameters);

                        // DB-clumn と View-Param が異なるとエラー
                        var viewField = "_" + type.ToLower().Last() + segment + suffix;  // _aHAminLength
                        var objType = this.GetType();
                        if(objType  != null ){
                            var targetField = objType.GetField(viewField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                            if (targetField != null)
                                this.GetType().GetField(viewField, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, dbValue);
                            RaisePropertyChanged(typeSegment);
                        }
                        // this.GetType().GetProperty(typeSegment).SetValue(this, dbValue);

                    }
                }
            }
        }

        // ReferenceView LivetCallMethodAction 5'start default nuc AGC
        public void SetNuc5Ds()
            => SetDefaultParameter(ConstantValues.nuc5ds, string.Empty, ConstantValues.nuc5start);
        public void SetNuc3Ds()
            => SetDefaultParameter(ConstantValues.nuc3ds, string.Empty,  ConstantValues.nuc3end);
        public void SetIncN()
            => SetDefaultParameter(ConstantValues.includeN, 0, ConstantValues.NnucDefaultIntValue);
        public void SetMapCdHit()
            => SetDefaultParameter(ConstantValues.mappingcdhit, string.Empty, ConstantValues.MappingCdHitDefaultValue.ToString());
        public void SetBlastCdHit()
            => SetDefaultParameter(ConstantValues.blastcdhit, string.Empty, ConstantValues.BlastCdHitDefaultValue.ToString());


        // ReferenceView LivetCallMethodAction
        private void SetDefaultParameter(string suffix, object emptyValue, object defaultValue)
        {
            var currentValues = new List<object>();
            foreach (var type in CommonFlow.VIRUS_TYPES)
            {
                foreach (var segment in CommonFlow.SEGMENTS)
                {
                    var typeSegment = type.Last() + segment + suffix;  // AHAminLength
                    currentValues.Add(this.GetType().GetProperty(typeSegment).GetValue(this));

                }
            }
            var viewValues = currentValues.Distinct();

            if (viewValues.Count() > 1)
            { // 2種類以上パラメータが入力されているなら空白へ
                SetDefaultParameterImpl(suffix, emptyValue);
                return;
            }

            if (viewValues.First() == null || viewValues.First() == emptyValue)
            {
                SetDefaultParameterImpl(suffix, defaultValue);
                return;
            }

            SetDefaultParameterImpl(suffix, emptyValue);
        }

        private void SetDefaultParameterImpl(string suffix, object value)
        {
            foreach (var type in CommonFlow.VIRUS_TYPES)
            {
                foreach (var segment in CommonFlow.SEGMENTS)
                {
                    var typeSegment = type.Last() + segment + suffix;  // AHAminLength
                    this.GetType().GetProperty(typeSegment).SetValue(this, value);
                }
            }
        }


        // Execute -> 画面情報からデータベース.Reference テーブルへ
        private long SaveReference()
        {
            var dbReference = ReferenceDao.GetDefalutParameter();
            foreach (var type in CommonFlow.VIRUS_TYPES)
            {
                foreach (var segment in CommonFlow.SEGMENTS)
                {
                    foreach (var suffix in suffixs)
                    {
                        var typeSegment = type.Last() + segment + suffix;  // AHAminLength
                        switch (suffix)
                        {
                            case ConstantValues.includeN:
                                SetView2Db(typeSegment, ViewFields.num, ref dbReference);
                                break;
                            case ConstantValues.nuc5ds:
                            case ConstantValues.nuc3ds:
                                SetView2Db(typeSegment, ViewFields.nuc, ref dbReference);
                                break;
                            case ConstantValues.mappingcdhit:
                            case ConstantValues.blastcdhit:
                                SetView2Db(typeSegment, ViewFields.dec, ref dbReference);
                                break;
                            case ConstantValues.minlength:
                            case ConstantValues.maxlength:
                            case ConstantValues.minCDS:
                            case ConstantValues.maxCDS:
                                SetView2Db(typeSegment, ViewFields.digit, ref dbReference);
                                break;
                        }
                    }
                }
            }

            // Object 作成した
            var insId = ReferenceDao.InsertReferenceTable(dbReference);
            mainLog.Report("Reference table save, " + insId);
            return insId;
        }

        private void SetView2Db( string typeSegment, ViewFields field, ref Reference dbReference)
        {
            // Viewに入力してある文字列
            var viewValue = this.GetType().GetProperty(typeSegment).GetValue(this);
            switch (field)
            {
                case ViewFields.dec:
                    Double cdhit;
                    if (!Double.TryParse((string)viewValue, out cdhit))
                        cdhit = ConstantValues.MappingCdHitDefaultValue;
                    this.GetType().GetProperty(typeSegment).SetValue(this, cdhit.ToString());
                    dbReference.GetType().GetProperty(typeSegment).SetValue(dbReference, viewValue);
                    break;

                case ViewFields.nuc:
                    if (viewValue != null && ! string.IsNullOrEmpty((string)viewValue))
                    {
                        var nuc = (string)viewValue;
                        var nucOnly = string.Join("", nuc.ToUpper().ToArray().Where(s => s == 'C' || s == 'G' || s == 'A' || s == 'T'));
                        this.GetType().GetProperty(typeSegment).SetValue(this, nucOnly);
                        dbReference.GetType().GetProperty(typeSegment).SetValue(dbReference, nucOnly);
                    }
                    break;

                case ViewFields.num:
                    int len;  // 変換出来たらセット、できなかったらデフォルト値へ
                    var length = viewValue.ToString();
                    if ( int.TryParse(length, out len)) { 
                        dbReference.GetType().GetProperty(typeSegment).SetValue(dbReference, viewValue);
                    }
                    else
                    {
                        var defaultInt = dbReference.GetType().GetProperty(typeSegment).GetValue(dbReference);
                        this.GetType().GetProperty(typeSegment).SetValue(this, defaultInt.ToString());
                    }
                    break;

                case ViewFields.digit:
                    // view=text  field=int 既に変換確認はしている
                    var fieldval = GetPropertyField(typeSegment);
                    viewValue = this.GetType()
                                .GetField(fieldval, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                                .GetValue(this);
                    dbReference.GetType().GetProperty(typeSegment).SetValue(dbReference, viewValue);
                    break;
            }

        }


        // View の Int を string で受け取り、格納はInt にするため　苦肉の策
        private void SetDigitText(string property, string field, string viewValue)
        {
            int intval = 0;

            if (int.TryParse(viewValue, out intval))
            { // int 変換出来なければ 0 にする
                this.GetType().GetField(field, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                        .SetValue(this, intval);
                RaisePropertyChanged(property);
                return;
            }

            ShowErrorDialog("Please input a numerical value.", "input value error.");
        }

        private void DicisionReferenceUpdate()
        {
            switch (this._analysisButton)
            {
                case buttonAnalysis:
                    DicisionReferenceUpdateCall();
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


        private void DicisionReferenceUpdateCall()
        {
            // 型判定用 BALST-database update
            mainLog.Report("Dicision Reference Update call.");
            

            this.flow = new DecisionReferenceUpdateFlow(
                new Proc.Flow.Properties.ReferenceUpdateParameters()
                {
                    victoriaFastaPath = this._victoriaFasta,
                    yamagataFastaPath = this._yamagataFasta
                }, mainLog);

            AnalysisButton = buttonCancel;  // cancel 
            _ = ProcessAsync(flow);
        }

        // Select user reference.
        private void SelectUserFasta()
        {
            var userFasta = SelectFileDialog(
                                        "select your reference fasta file,",
                                        false,
                                        false);
            if (userFasta.Any() && File.Exists(userFasta.First()))
                UserFasta = userFasta.First();
        }

        // Select user reference.
        private void SelectYamagataFasta()
        {
            var userFasta = SelectFileDialog(
                                        "select your reference typeB yamagata fasta file,",
                                        false,
                                        false);
            if (File.Exists(userFasta.First()))
                YamagataFasta = userFasta.First();
        }

        // Select user reference.
        private void SelectVictoriaFasta()
        {
            var userFasta = SelectFileDialog(
                                        "select your reference typeB victoria fasta file,",
                                        false,
                                        false);
            if (File.Exists(userFasta.First()))
                VictoriaFasta = userFasta.First();
        }


        private string GetPropertyField(string propertyName)
        {
            return  "_" + propertyName.ToLower()[0] +
                                    propertyName.Substring(1);
        }


        private void PropertyChange()
        {

        }

        // フィールドに対する措置
        private enum ViewFields
        {
            nuc,
            dec,
            num,
            digit
        }
    }


}
