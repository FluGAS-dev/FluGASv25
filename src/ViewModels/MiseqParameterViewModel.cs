using FluGASv25.Dao;
using FluGASv25.Utils;
using Livet;
using Livet.Commands;
using Livet.Messaging.Windows;
using System;
using System.Linq;

namespace FluGASv25.ViewModels
{
    public class MiseqParameterViewModel : CommonParameterProperties
    {

        public ViewModelCommand MiseqParameterCommitCommand { protected set; get; }
        public ViewModelCommand MiseqParameterCancelCommand { protected set; get; }

        // constractor
        public MiseqParameterViewModel()
        {
            System.Diagnostics.Debug.WriteLine(nameof(this.ToString) );
            CommandInit();
        }

        public MiseqParameterViewModel(string selectedParameterName = null)
        {
            this.CurrentParameterName = selectedParameterName;
            CommandInit();
        }

        protected void CommandInit()
        {
            MiseqParameterCommitCommand = new ViewModelCommand(MiseqParameterCommit);
            MiseqParameterCancelCommand = new ViewModelCommand(ParameterViewClose);
        }

        
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        public void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("Initialize");

            // default minion parameter
            var parameterName = string.IsNullOrWhiteSpace(CurrentParameterName) ?
                                 ConstantValues.DefaultMinionParameterName :
                                 CurrentParameterName;

            var dbParams = MiseqParameterDao.GetParameters()
                                                                .Where(s => s.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                                                                .OrderByDescending(s => s.CreateDate);
            var dbParam = dbParams.Any() ?
                                        dbParams.First() :
                                        MiseqParameterDao.GetParameters().First();


            SetParameter(dbParam);
        }

        // setting parameter save.
        private void MiseqParameterCommit()
        {
            // Parameter chck....
            CheckCommonParameterCommon();
            CheckParameter();

            if (this.HasErrors) {
                ShowErrorDialog("Some parameters are not set. ", "not set patameter error.");
                return; // error.
            }

            var dbparameter = SetDbParameter();
            var insertId = MiseqParameterDao.InsertPatameter(dbparameter);
            if(insertId > 0)
                this.CurrentParameterName = Name; // Patameter name
            else
                ShowErrorDialog("Miseq paramer not save... ", "database error.");


            
            DispatcherHelper.UIDispatcher.BeginInvoke((Action)(() =>
            {
                Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
            }));
        }

        // QC-Parameters
        private bool _isFastQC;
        public bool IsFastQC
        {
            get { return _isFastQC; }
            set { RaisePropertyChangedIfSet(ref _isFastQC, value); }
        }

        private string _fastQCMinPhredScore;
        public string FastQCMinPhredScore
        {
            get { return _fastQCMinPhredScore; }
            set { RaisePropertyChangedIfSet(ref _fastQCMinPhredScore, value); }
        }

        private string _fastQCWindowSize;
        public string FastQCWindowSize
        {
            get { return _fastQCWindowSize; }
            set { RaisePropertyChangedIfSet(ref _fastQCWindowSize, value); }
        }

        private string _fastQCMinLength;
        public string FastQCMinLength
        {
            get { return _fastQCMinLength; }
            set { RaisePropertyChangedIfSet(ref _fastQCMinLength, value); }
        }

        // bowtie2 mapping.
        private bool isBowtie2;
        private string _bowtie2Mp;
        private string _bowtie2Np;
        private string _bowtie2Rdg;
        private string _bowtie2Rfg;
        private string _bowtie2ScoreMin;
        private string _bowtie2Gbar;
        private string _bowtie2Nceil;
        private string _bowtie2D;
        private string _bowtie2R;
        private string _bowtie2N;
        private string _bowtie2L;
        private string _bowtie2I;

        public bool IsBowtie2
        {
            get { return isBowtie2; }
            set { RaisePropertyChangedIfSet(ref isBowtie2, value); }
        }

        public const string Bowtie2MpAudit = "Bowtie2Mp";
        public string Bowtie2Mp
        {
            get { return _bowtie2Mp; }
            set { RaisePropertyChangedIfSet(ref _bowtie2Mp, value); }
        }

        public const string Bowtie2NpAudit = "Bowtie2Np";
        public string Bowtie2Np
        {
            get { return _bowtie2Np; }
            set { RaisePropertyChangedIfSet(ref _bowtie2Np, value); }
        }

        public const string Bowtie2RdgAudit = "Bowtie2Rdg";
        public string Bowtie2Rdg
        {
            get { return _bowtie2Rdg; }
            set { RaisePropertyChangedIfSet(ref _bowtie2Rdg, value); }
        }

        public const string Bowtie2RfgAudit = "Bowtie2Rfg";
        public string Bowtie2Rfg
        {
            get { return _bowtie2Rfg; }
            set { RaisePropertyChangedIfSet(ref _bowtie2Rfg, value); }
        }

        public const string Bowtie2ScoreMinAudit = "Bowtie2ScoreMin";
        public string Bowtie2ScoreMin
        {
            get { return _bowtie2ScoreMin; }
            set { RaisePropertyChangedIfSet(ref _bowtie2ScoreMin, value); }
        }

        public const string Bowtie2GbarAudit = "Bowtie2Gbar";
        public string Bowtie2Gbar
        {
            get { return _bowtie2Gbar; }
            set { RaisePropertyChangedIfSet(ref _bowtie2Gbar, value); }
        }

        public const string Bowtie2NceilAudit = "Bowtie2Nceil";
        public string Bowtie2Nceil
        {
            get { return _bowtie2Nceil; }
            set { RaisePropertyChangedIfSet(ref _bowtie2Nceil, value); }
        }

        public const string Bowtie2DAudit = "Bowtie2D";
        public string Bowtie2D
        {
            get { return _bowtie2D; }
            set { RaisePropertyChangedIfSet(ref _bowtie2D, value); }
        }

        public const string Bowtie2RAudit = "Bowtie2R";
        public string Bowtie2R
        {
            get { return _bowtie2R; }
            set { RaisePropertyChangedIfSet(ref _bowtie2R, value); }
        }

        public const string Bowtie2NAudit = "Bowtie2N";
        public string Bowtie2N
        {
            get { return _bowtie2N; }
            set { RaisePropertyChangedIfSet(ref _bowtie2N, value); }
        }

        public const string Bowtie2LAudit = "Bowtie2L";
        public string Bowtie2L
        {
            get { return _bowtie2L; }
            set { RaisePropertyChangedIfSet(ref _bowtie2L, value); }
        }

        public const string Bowtie2IAudit = "Bowtie2I";
        public string Bowtie2I
        {
            get { return _bowtie2I; }
            set { RaisePropertyChangedIfSet(ref _bowtie2I, value); }
        }

        private bool _isSampling;
        public bool IsSampling
        {
            get { return _isSampling; }
            set { RaisePropertyChangedIfSet(ref _isSampling, value); }
        }

        private string _mappingSeqCount;
        public string MappingSeqCount
        {
            get { return _mappingSeqCount; }
            set { RaisePropertyChangedIfSet(ref _mappingSeqCount, value); }
        }

        protected void CheckParameter()
        {
            NumericValidateProperty(nameof(FastQCMinPhredScore), FastQCMinPhredScore);
            NumericValidateProperty(nameof(FastQCWindowSize), FastQCWindowSize);
            NumericValidateProperty(nameof(FastQCMinLength), FastQCMinLength);

            NumericValidateProperty(nameof(MappingSeqCount), MappingSeqCount);

            // そのほかは可変で・・・放っておく。

        }


    }
}
