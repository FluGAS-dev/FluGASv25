using FluGASv25.Models.Properties;
using FluGASv25.Utils;
using FluGASv25.ViewModels.Base;
using Livet;
using Livet.Messaging.Windows;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FluGASv25.ViewModels
{
    public abstract class CommonParameterProperties : ViewModelNotifyDataError
    {

        public string CurrentParameterName;

        // Parameter commons 
        private string _name;
        private bool _isAnalysisTop3;
        private bool _isFirstReference;

        // Blast Consensus
        private string _cnsMinCoverage;
        private string _variantMinFrequency;
        private string _cnsHomoFrequency;
        private string _cnsInsDelFrequency;
        private bool _isLessThanN1st;
        private bool _isLessThanNone1st;
        private bool _isLessThanN2nd;
        private bool _isLessThanNone2nd;


        private string _cnsIncludeNRatio;
        private string _blastEvalueCutoff;
        private string _blastResultCutoff;
        private ObservableCollection<string> _selectBlastElm 
            = new ObservableCollection<string>
            {
                Proc.Several.Blast.SelectEvalueElm,
                Proc.Several.Blast.SelectIdenticalElm,
                Proc.Several.Blast.SelectLengthElm,
                Proc.Several.Blast.SelectScoreElm
            };
        private string _referenceSelectBlastElement;
        private string _referenceSelectBlastEvalue;
        private bool _isReferenceSelectBlastEvalue;
        private string _referenceSelectBlastIdentical;
        private bool _isReferenceSelectBlastIdentical;
        private string _referenceSelectBlastScore;
        private bool _isReferenceSelectBlastScore;
        private string _referenceSelectBlastLength;
        private bool _isReferenceSelectBlastLength;


        public const string NameAudit = "Name";
        public string Name
        {
            get { return _name; }
            set { RaisePropertyChangedIfSet(ref _name, value); }
        }

        public bool IsAnalysisTop3
        {
            get { return _isAnalysisTop3; }
            set { RaisePropertyChangedIfSet(ref _isAnalysisTop3, value); }
        }

        public bool IsFirstReference
        {
            get { return _isFirstReference; }
            set { RaisePropertyChangedIfSet(ref _isFirstReference, value); }
        }

        public const string CnsMinCoverageAudit = "CnsMinCoverage";
        public string CnsMinCoverage
        {
            get { return _cnsMinCoverage; }
            set { RaisePropertyChangedIfSet(ref _cnsMinCoverage, value); }
        }

        public const string CnsVariantMinFrequencyAudit = "CnsVariantMinFrequency";
        public string CnsVariantMinFrequency
        {
            get { return _variantMinFrequency; }
            set { RaisePropertyChangedIfSet(ref _variantMinFrequency, value); }
        }

        public const string CnsHomoFrequencyAudit = "CnsHomoFrequency";
        public string CnsHomoFrequency
        {
            get { return _cnsHomoFrequency; }
            set { RaisePropertyChangedIfSet(ref _cnsHomoFrequency, value); }
        }

        public const string CnsInsDelFrequencyAudit = "CnsInsDelFrequency";
        public string CnsInsDelFrequency
        {
            get { return _cnsInsDelFrequency; }
            set { RaisePropertyChangedIfSet(ref _cnsInsDelFrequency, value); }
        }

        public bool IsLessThanN1st
        {
            get { return _isLessThanN1st; }
            set
            {
                if (RaisePropertyChangedIfSet(ref _isLessThanN1st, value))
                    IsLessThanNone1st = !_isLessThanN1st;
            }
        }

        public bool IsLessThanNone1st
        {
            get { return _isLessThanNone1st; }
            set
            {
                if (RaisePropertyChangedIfSet(ref _isLessThanNone1st, value))
                    IsLessThanN1st = !_isLessThanNone1st;
            }
        }

        public bool IsLessThanN2nd
        {
            get { return _isLessThanN2nd; }
            set
            {
                if (RaisePropertyChangedIfSet(ref _isLessThanN2nd, value))
                    IsLessThanNone2nd = !_isLessThanN2nd;
            }
        }

        public bool IsLessThanNone2nd
        {
            get { return _isLessThanNone2nd; }
            set
            {
                if (RaisePropertyChangedIfSet(ref _isLessThanNone2nd, value))
                    IsLessThanN2nd = !_isLessThanNone2nd;
            }
        }


        public const string CnsIncludeNRatioAudit = "CnsIncludeNRatio";
        public string CnsIncludeNRatio
        {
            get { return _cnsIncludeNRatio; }
            set { RaisePropertyChangedIfSet(ref _cnsIncludeNRatio, value); }
        }

        public const string BlastEvalueCutoffAudit = "BlastEvalueCutoff";
        public string BlastEvalueCutoff
        {
            get { return _blastEvalueCutoff; }
            set { RaisePropertyChangedIfSet(ref _blastEvalueCutoff, value); }
        }

        public const string BlastResultCutoffAudit = "BlastResultCutoff";
        public string BlastResultCutoff
        {
            get { return _blastResultCutoff; }
            set { RaisePropertyChangedIfSet(ref _blastResultCutoff, value); }
        }

        public ObservableCollection<string> SelectBlastElm
        {
            get { return _selectBlastElm; }
            // set は init で行うだけ。
        }

        public string ReferenceSelectBlastElement
        {
            get { return _referenceSelectBlastElement; }
            set { RaisePropertyChangedIfSet(ref _referenceSelectBlastElement, value); }
        }

        public const string ReferenceSelectBlastEvalueAudit = "ReferenceSelectBlastEvalue";
        public string ReferenceSelectBlastEvalue
        {
            get { return _referenceSelectBlastEvalue; }
            set { RaisePropertyChangedIfSet(ref _referenceSelectBlastEvalue, value); }
        }
        public bool IsReferenceSelectBlastEvalue
        {
            get { return _isReferenceSelectBlastEvalue; }
            set { RaisePropertyChangedIfSet(ref _isReferenceSelectBlastEvalue, value); }
        }

        public const string ReferenceSelectBlastIdenticalAudit = "ReferenceSelectBlastIdentical";
        public string ReferenceSelectBlastIdentical
        {
            get { return _referenceSelectBlastIdentical; }
            set { RaisePropertyChangedIfSet(ref _referenceSelectBlastIdentical, value); }
        }
        public bool IsReferenceSelectBlastIdentical
        {
            get { return _isReferenceSelectBlastIdentical; }
            set { RaisePropertyChangedIfSet(ref _isReferenceSelectBlastIdentical, value); }
        }

        public const string ReferenceSelectBlastScoreAudit = "ReferenceSelectBlastScore";
        public string ReferenceSelectBlastScore
        {
            get { return _referenceSelectBlastScore; }
            set { RaisePropertyChangedIfSet(ref _referenceSelectBlastScore, value); }
        }
        public bool IsReferenceSelectBlastScore
        {
            get { return _isReferenceSelectBlastScore; }
            set { RaisePropertyChangedIfSet(ref _isReferenceSelectBlastScore, value); }
        }

        public const string ReferenceSelectBlastLengthAudit = "ReferenceSelectBlastLength";
        public string ReferenceSelectBlastLength
        {
            get { return _referenceSelectBlastLength; }
            set { RaisePropertyChangedIfSet(ref _referenceSelectBlastLength, value); }
        }

        public bool IsReferenceSelectBlastLength
        {
            get { return _isReferenceSelectBlastLength; }
            set
            {
                if (RaisePropertyChangedIfSet(ref _isReferenceSelectBlastLength, value))
                {
                    this._isReferenceSelectBlastEvalue = false;
                    this._isReferenceSelectBlastIdentical = false;
                    this._isReferenceSelectBlastScore = false;
                }
            }
        }

        protected void CheckCommonParameterCommon()
        {
            CheckCommonParameter(NameAudit, Name);
            CheckCommonParameter(CnsMinCoverageAudit, CnsMinCoverage);
            CheckCommonParameter(CnsVariantMinFrequencyAudit, CnsVariantMinFrequency);
            CheckCommonParameter(CnsHomoFrequencyAudit, CnsHomoFrequency);
            CheckCommonParameter(CnsIncludeNRatioAudit, CnsIncludeNRatio);
            CheckCommonParameter(BlastEvalueCutoffAudit, BlastEvalueCutoff);
            CheckCommonParameter(BlastResultCutoffAudit, BlastResultCutoff);
            CheckCommonParameter(ReferenceSelectBlastEvalueAudit, ReferenceSelectBlastEvalue);
            CheckCommonParameter(ReferenceSelectBlastIdenticalAudit, ReferenceSelectBlastIdentical);
            CheckCommonParameter(ReferenceSelectBlastScoreAudit, ReferenceSelectBlastScore);
            CheckCommonParameter(ReferenceSelectBlastLengthAudit, ReferenceSelectBlastLength);

            // 2020.05.15 追加
            float.TryParse(this._cnsInsDelFrequency, out float insDelFreq);
            if (insDelFreq < 0.51f)
                AddError(CnsInsDelFrequencyAudit, "Ins/Del Min Frequency is not set below 0.51");
            else
                RemoveError(CnsInsDelFrequencyAudit);

        }

        protected void CheckCommonParameter(string propertyName, object value)
        {
            // 各パラメータの整合性
            switch (propertyName)
            {
                case CnsMinCoverageAudit:
                case CnsIncludeNRatioAudit:
                case BlastResultCutoffAudit:
                case ReferenceSelectBlastScoreAudit:
                case ReferenceSelectBlastLengthAudit:
                    NumericValidateProperty(propertyName, value);
                    break;

                case CnsVariantMinFrequencyAudit:
                case CnsHomoFrequencyAudit:
                case BlastEvalueCutoffAudit:
                case ReferenceSelectBlastEvalueAudit:
                case ReferenceSelectBlastIdenticalAudit:
                    FloatValidateProperty(propertyName, value);
                    break;

                default:
                    StringValidateProperty(propertyName, value);
                    break;
            }
        }

        // Databaseのパラメータから、設定画面へ
        // protected Parameters dbParameter;
        protected void SetParameter(Parameters p) // Parameters : Minion/Miseq
        {

            PropertyInfo[] myInfos = this.GetType().GetProperties();
            PropertyInfo[] dbInfos = p.GetType().GetProperties();
            foreach (PropertyInfo dbParam in dbInfos)
            {
                if (dbParam.GetValue(p, null) != null)
                {
                    // Property name
                    var dbPropertyName = dbParam.Name;

                    // Dbと同じProperty名があればセットする。
                    foreach (PropertyInfo myInfo in myInfos)
                    {
                        // var myProtertyName = myInfo.Name;
                        if (dbPropertyName == myInfo.Name)
                        {
                            myInfo.SetValue(this, dbParam.GetValue(p));
                            System.Diagnostics.Debug.WriteLine("##  " + dbPropertyName + " : " + dbParam.GetValue(p));
                        }
                    }
                }
            }

            // Blast Top-Hit Select Element default is Evalue
            if (string.IsNullOrEmpty(p.ReferenceSelectBlastElement))
                this.ReferenceSelectBlastElement = Proc.Several.Blast.SelectEvalueElm;

            // less min-length 
            if (!this._isLessThanNone1st && !this._isLessThanN1st)
                IsLessThanN1st = true;

            if (!this._isLessThanNone2nd && !this._isLessThanN2nd)
                IsLessThanNone2nd = true;


            if (p.Name.Equals(ConstantValues.DefaultMinionParameterName, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals(ConstantValues.DefaultMiseqParameterName, StringComparison.OrdinalIgnoreCase))
                Name = string.Empty;

        }

        // viewパラメータからDatabse へ
        public Parameters SetDbParameter()
        {
            // this.dbParameter = dbParameter?? new Parameters();
            var dbParameter =this.GetType().Name.StartsWith("Minion", StringComparison.OrdinalIgnoreCase) ?
                                  (Parameters) new MinionParameters():
                                  (Parameters) new MiseqParameters();

            
            PropertyInfo[] myInfos = this.GetType().GetProperties();
            PropertyInfo[] dbInfos = dbParameter.GetType().GetProperties();
            foreach (PropertyInfo myParam in myInfos)
            {
                if (myParam.GetValue(this, null) != null)
                {
                    // Property name
                    var myPropertyName = myParam.Name;

                    // Dbと同じProperty名があればセットする。
                    foreach (PropertyInfo dbInfo in dbInfos)
                    {
                        if (myPropertyName == dbInfo.Name)
                        {
                            dbInfo.SetValue(dbParameter, myParam.GetValue(this));
                            System.Diagnostics.Debug.WriteLine("## dao set : " + dbInfo.Name + " = " + myParam.GetValue(this));
                        }
                    }
                }
            }

            // IDは新規
            // dbParameter.Id = null;
            // dbParameter.CreateDate = null;
            return dbParameter;
        }


        // cancel-commit = View Close 
        protected void ParameterViewClose()
        {
            DispatcherHelper.UIDispatcher.BeginInvoke((Action)(() =>
            {
                Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
            }));
        }


    }
}
