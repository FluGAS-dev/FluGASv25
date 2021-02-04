using FluGASv25.Dao;
using FluGASv25.Utils;
using Livet;
using Livet.Commands;
using Livet.Messaging.Windows;
using System;
using System.IO;
using System.Linq;

namespace FluGASv25.ViewModels
{
    public class MinionParameterViewModel : CommonParameterProperties
    {

        public ViewModelCommand MinionParameterCommitCommand { private set; get; }
        public ViewModelCommand MinionParameterCancelCommand { private set; get; }
        public ViewModelCommand GuppySelectCommand { private set; get; }

        // constractor
        public MinionParameterViewModel()
        {
            CommandInit();
        }

        public MinionParameterViewModel(string selectedParameterName = null )
        {
            this.CurrentParameterName = selectedParameterName;
            CommandInit();
        }

        protected void CommandInit()
        {
            MinionParameterCommitCommand = new ViewModelCommand(MinionParameterCommit);
            MinionParameterCancelCommand = new ViewModelCommand(ParameterViewClose);
            GuppySelectCommand = new ViewModelCommand(SetGuppyPath);

        }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        public void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("MinionParameterViewModel Initialize");

            // default minion parameter
            var parameterName = string.IsNullOrWhiteSpace(this.CurrentParameterName) ?
                                 ConstantValues.DefaultMinionParameterName :
                                 CurrentParameterName;

            var dbParam = MinionParameterDao.GetParameters()
                                                                        .Where(s => s.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                                                                        .OrderByDescending(s => s.CreateDate)
                                                                        .First();
            SetParameter(dbParam);

            // ont-path
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ont_path))
                this.GuppyBinDirectory = Properties.Settings.Default.ont_path;
        }

        // setting parameter save.
        private void MinionParameterCommit()
        {
            // Parameter chck....
            CheckCommonParameterCommon();
            CheckParameter();

            if (this.HasErrors)
            {
                ShowErrorDialog("Some parameters are not set. ", "not set patameter error.");
                return; // error.
            }

            // チェックが通っているのでGuppy-path を保存。
            Properties.Settings.Default.ont_path = _guppyBinDirectory;
            Properties.Settings.Default.Save();

            var dbparameter = SetDbParameter();
            var insertId = MinionParameterDao.InsertPatameter(dbparameter);
            if (insertId > 0) { 
                this.CurrentParameterName = Name; // Patameter name
                System.Diagnostics.Debug.WriteLine("MinionParameter db-commit ok.");

            }
            else { 
                ShowErrorDialog("MinION paramer not save... ", "database error.");
                System.Diagnostics.Debug.WriteLine("MinionParameter db-commit NG!");
            }

            DispatcherHelper.UIDispatcher.BeginInvoke((Action)(() =>
            {
                Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
            }));
        }

        private void SetGuppyPath()
        {
            var binDir = SelectFileDialog("Select ont-Guppy bin folder.", false, true);
            if (binDir.Any())
            {
                // var reg = new Regex("^guppy_basecaller*exe$");
                var di = new DirectoryInfo(binDir.First());
                var files = di.EnumerateFiles("*basecaller.exe", SearchOption.AllDirectories);

                if (files.Any())
                {
                    RemoveError(nameof(GuppyBinDirectory));
                    GuppyBinDirectory = binDir.First();
                }
                else
                {
                    this.GuppyBinDirectory = string.Empty;

                    AddError(nameof(GuppyBinDirectory), "not found guppy-basecaller.");
                    ShowErrorDialog("not found guppy-basecaller");
                }
            }
        }

        protected void CheckParameter()
        {
            StringValidateProperty(nameof(GuppyBinDirectory), GuppyBinDirectory);
            if (Directory.Exists(_guppyBinDirectory))
                RemoveError(nameof(GuppyBinDirectory));
            else
                AddError(nameof(GuppyBinDirectory), "not found guppy-basecaller.");


            if (_isGuppyAlign)
                NumericValidateProperty(nameof(GuppyMinCover), GuppyMinCover);

            if (_isMinimap2)
            {
                NumericValidateProperty(nameof(Minimap2KmerSize), Minimap2KmerSize);
                NumericValidateProperty(nameof(Minimap2MinWindowSize), Minimap2MinWindowSize);
                NumericValidateProperty(nameof(Minimap2MatchingScore), Minimap2MatchingScore);
                NumericValidateProperty(nameof(Minimap2MismachPenalty), Minimap2MismachPenalty);
                FloatValidateProperty(nameof(Minimap2GapOpenPenalty), Minimap2GapOpenPenalty);
                FloatValidateProperty(nameof(Minimap2GapExtentionPenalty), Minimap2GapExtentionPenalty);
            }
            // そのほかは可変で・・・放っておく。

        }
        // Name -> CommonParameterProperties
        private string _guppyBinDirectory;
        public string GuppyBinDirectory
        {
            get { return _guppyBinDirectory; }
            set { RaisePropertyChangedIfSet(ref _guppyBinDirectory, value); }
        }

        private bool _isGuppyExist;
        public bool IsGuppyExist
        {
            get { return _isGuppyExist; }
            set { RaisePropertyChangedIfSet(ref _isGuppyExist, value); }
        }

        private bool _isGuppyAlign;
        public bool IsGuppyAlign
        {
            get { return _isGuppyAlign; }
            set { RaisePropertyChangedIfSet(ref _isGuppyAlign, value); }
        }

        private string _guppyMinCover;
        public string GuppyMinCover
        {
            get { return _guppyMinCover; }
            set { RaisePropertyChangedIfSet(ref _guppyMinCover, value); }
        }

        private bool _isMinimap2;
        public bool IsMinimap2
        {
            get { return _isMinimap2; }
            set { RaisePropertyChangedIfSet(ref _isMinimap2, value); }
        }

        private string _minimap2KmerSize;
        public string Minimap2KmerSize
        {
            get { return _minimap2KmerSize; }
            set { RaisePropertyChangedIfSet(ref _minimap2KmerSize, value); }
        }

        private string _minimap2MinWindowSize;
        public string Minimap2MinWindowSize
        {
            get { return _minimap2MinWindowSize; }
            set { RaisePropertyChangedIfSet(ref _minimap2MinWindowSize, value); }
        }

        private string _minimap2MatchingScore;
        public string Minimap2MatchingScore
        {
            get { return _minimap2MatchingScore; }
            set { RaisePropertyChangedIfSet(ref _minimap2MatchingScore, value); }
        }

        private string _minimap2MismachPenalty;
        public string Minimap2MismachPenalty
        {
            get { return _minimap2MismachPenalty; }
            set { RaisePropertyChangedIfSet(ref _minimap2MismachPenalty, value); }
        }

        private string _minimap2GapOpenPenalty;
        public string Minimap2GapOpenPenalty
        {
            get { return _minimap2GapOpenPenalty; }
            set { RaisePropertyChangedIfSet(ref _minimap2GapOpenPenalty, value); }
        }

        private string _minimap2GapExtentionPenalty;
        public string Minimap2GapExtentionPenalty
        {
            get { return _minimap2GapExtentionPenalty; }
            set { RaisePropertyChangedIfSet(ref _minimap2GapExtentionPenalty, value); }
        }

    }
}
