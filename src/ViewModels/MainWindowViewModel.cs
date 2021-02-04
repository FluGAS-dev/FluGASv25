using FluGASv25.Utils;
using FluGASv25.ViewModels.Base;
using Livet.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FluGASv25.ViewModels
{
    public partial class MainWindowViewModel : ViewModelNotifyDataError
    {
        // constractor
        public MainWindowViewModel()
        {
            this.OpenInformationCommand = new ViewModelCommand(OpenInformation);

            this.AnarysisExecuteCommand = new ViewModelCommand(AnalysisExecute);
            this.EditParameterCommand   = new ViewModelCommand(EditParameter);
            this.SelectDataCommand = new ViewModelCommand(RemoveDataItem);

            this.SampleEditCommand = new ViewModelCommand(SampleEdit);
            this.SampleDeleteCommand = new ViewModelCommand(SampleDelete);
            this.ChangeViewNameCommand = new ViewModelCommand(ViewNameUpdate);
            this.CreateExcelCommand = new ViewModelCommand(CreateExcel);

            this.SelectFolderCommand    = new ViewModelCommand(SelectFolder);
            this.SelectFileCommand      = new ViewModelCommand(SelectFile);
            this.SelectFileClearCommand = new ViewModelCommand(SelectClearFile);
            this.SelectUserFastaCommand = new ViewModelCommand(SelectUserFasta);

            this.SelectYamagataFastaCommand = new ViewModelCommand(SelectYamagataFasta);
            this.SelectVictoriaFastaCommand = new ViewModelCommand(SelectVictoriaFasta);

            this.SetDefaultReferenceParameterCommand = new ViewModelCommand(SetReferenceDefaultParameters);
            this.ReferenceUpdateCommand = new ViewModelCommand(ReferenceUpdate);
            this.DicisionReferenceUpdateCommand = new ViewModelCommand(DicisionReferenceUpdate);
            // log view append.
            this.mainLog = new Progress<string>(OnLogAppend);
        }

        public string Title
            => "FluGASv2 -Influenza Genome Assembly and subtyping-";

        protected bool isApplied = false;
        public void InitializeActivated()
        {
            System.Diagnostics.Debug.WriteLine("Initialize main view InitializeActivated.");
            if (isApplied) return;

            isApplied = true;  // 一回だけActiveWindowで通したい苦肉
            System.Diagnostics.Debug.WriteLine("InitializeActivated .");


            if (LicenseSetting()) { 
                Application.Current.Shutdown();
                return;
            }

            // guppy path config.
            GuppySetting();
        }

        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).
        public void Initialize() // ContentRendered.
        {
            System.Diagnostics.Debug.WriteLine("Initialize main view Initialize.");

            if (Properties.Settings.Default.clearsetting)
            {
                System.Diagnostics.Debug.WriteLine("Clear mode ,");
                Properties.Settings.Default.ont_path = string.Empty;
                Properties.Settings.Default.select_dir = string.Empty;
                Properties.Settings.Default.save_dir = string.Empty;
                Properties.Settings.Default.work_dir = string.Empty;
                Properties.Settings.Default.imm_dir = string.Empty;
                Properties.Settings.Default.version = string.Empty;
                Properties.Settings.Default.Save();
            }

            // version text
            var message = string.Empty;
            var vf = System.IO.Path.Combine(
                           System.AppDomain.CurrentDomain.BaseDirectory,
                           "FluGAS.version");

            if (File.Exists(vf)) File.Delete(vf);
            WfComponent.Utils.FileUtils.WriteFileFromString
                (vf,Properties.Settings.Default.version,ref message);

            // Log out directory clear .
            var logsDir = System.IO.Path.Combine(
                           System.AppDomain.CurrentDomain.BaseDirectory,
                           WfComponent.Utils.ConstantValues.CurrentLogDir);
            if (Directory.Exists(logsDir)) { 
                var logsDI = Directory.GetFiles(logsDir, "*.log");
                foreach (var logf in logsDI) { 
                    if (Path.GetFileName(logf).StartsWith(ConstantValues.EndAnalysisLog)) continue;
                    if (File.Exists(logf)) File.Delete(logf);
                }
            }
            else
            {
                Directory.CreateDirectory(logsDir);
            }

            this.IsMinionEnabled = true;
            this.IsFluA = true;
            this.IsFluB = true;
            this.IsHA = true;
            this.IsMP = true;
            this.IsNA = true;
            this.IsNP = true;
            this.IsNS = true;
            this.IsPA = true;
            this.IsPB1 = true;
            this.IsPB2 = true;
            this.IsOneSample = true;
            if (string.IsNullOrEmpty(Properties.Settings.Default.minion_param))
                this.selectedMinionParameter = Utils.ConstantValues.DefaultMinionParameterName;
            else
                selectedMinionParameter = Properties.Settings.Default.minion_param;

            if (string.IsNullOrEmpty(Properties.Settings.Default.miseq_param))
                selectedMiseqParameter = Utils.ConstantValues.DefaultMiseqParameterName;
            else
                selectedMiseqParameter = Properties.Settings.Default.miseq_param;

            // Create Reference tab 前回セットしたもの
            SetReferenceLastPatrameters();
            ResultsTabOnload();
        }

        protected IProgress<string> mainLog;
        private string _logMessage;
        public string LogMessage
        {
            get => _logMessage;
            set { RaisePropertyChangedIfSet(ref _logMessage, value); }
        }

        private void OnLogAppend(string log)
        {
            if (string.IsNullOrEmpty(log)) return;
            if (log.Equals(ConstantValues.MainLogClear))
            {
                var message = string.Empty;
                var logfile = WfComponent.Utils.FileUtils.GetUniqDateLogFile();
                WfComponent.Utils.FileUtils.WriteFileFromString(logfile, LogMessage, ref message);
                if (!string.IsNullOrEmpty(message))
                    System.Diagnostics.Debug.WriteLine("Logfile init error, " + message);


                LogMessage = string.Empty;
                return;
            }

            log = log.EndsWith(Environment.NewLine) ?
                    log :
                    log + Environment.NewLine;
            LogMessage += log;
        }


        private bool LicenseSetting() 
        {
            // Livet messenger 経由は起動直後に別スレッドになる様子・・・
            if (EnvInfo.IsLicenceInvalid())
            {
                // Display message box
                if (MessageBox.Show(
                                "License is invalid. " + Environment.NewLine + "Do you open up the setting window of the license?",
                                "License error.",   // caption
                                MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                {

                    OpenInformation();
                    if (this.isLicenseActivate) return false;
                }

                System.Threading.Thread.Sleep(1500);
                System.Windows.Application.Current.Shutdown();
                return true;   // license invalid.
            }
            // License ok.
            return false;
        }

        protected bool isFast5Analysis()
        {  /// MinION の場合、Fast5 を含まない場合がある。
           /// Fast5を含まないとき Guppy は不要。
            var fast5s = _selectDataList.Where(s => Directory.Exists(s) ||
                                                                        s.EndsWith(".fast5"));
            if (!fast5s.Any())
            {
                mainLog.Report("no fast5s, fastq only!  set barcoder off.");
                this.IsBarcode = false;
                return true;
            }


            if (string.IsNullOrEmpty(Properties.Settings.Default.ont_path) || !Directory.Exists(Properties.Settings.Default.ont_path))
            {
                var issetting = MessageBox.Show("no set Guppy Path," + Environment.NewLine + " Do you set guppy path?",
                                                                 "FluGAS ONT-analysis setting,",
                                                                 MessageBoxButton.YesNo,
                                                                 MessageBoxImage.Error);

                if (issetting == MessageBoxResult.Yes)
                {
                    EditParameter(); // Paramete画面に行ってからAnalysis画面へ戻る
                    return false;
                }
                // fast5 あるのにGuppy 設定しない。
                mainLog.Report("Guppy is necessary to perform ont row-data(fast5). ");
                return false;
            }
            return true; // Guppy アリマス。
        }


        // 起動初回に
        private void GuppySetting(bool isAnalysis = false)
        {
            System.Threading.Thread.Sleep(1500);
            // Guppy setteing?
            if ((string.IsNullOrEmpty(Properties.Settings.Default.ont_path) || !Directory.Exists(Properties.Settings.Default.ont_path)) &&
                !Utils.ConstantValues.NeverAskAgain.Equals(Properties.Settings.Default.ont_path, StringComparison.Ordinal) )
            {

                var basecallerPath = @"C:\Program Files\OxfordNanopore\ont-guppy-cpu\bin\guppy_basecaller.exe";
                var barcoderPath = @"C:\Program Files\OxfordNanopore\ont-guppy-cpu\bin\guppy_barcoder.exe";
                if (File.Exists(basecallerPath) && File.Exists(barcoderPath)) 
                {
                    var issetting = MessageBox.Show("no set Guppy Path, but found a Guppy " + Environment.NewLine +
                                                                    Path.GetDirectoryName(basecallerPath) + Environment.NewLine +
                                                                    Environment.NewLine +
                                                                    " is use this Guppy program?" ,
                                                                     "FluGAS Initialize setting,",
                                                                     MessageBoxButton.YesNo,
                                                                     MessageBoxImage.Warning);
                    if (issetting == MessageBoxResult.Yes)
                    {
                        this.IsMinion = true;   // 一時
                        Properties.Settings.Default.ont_path = @"C:\Program Files\OxfordNanopore\ont-guppy-cpu\bin";
                        Properties.Settings.Default.Save();
                    }

                }
                else
                {
                    // var issetting = ShowConfirmDialog("no set Guppy Path, \nDo you set guppy path?");
                    var issetting = MessageBox.Show("no set Guppy Path, but " + Environment.NewLine + " Do you set guppy path?",
                                                                     "FluGAS Initialize setting,",
                                                                     MessageBoxButton.YesNo,
                                                                     MessageBoxImage.Warning);
                    if (issetting == MessageBoxResult.Yes)
                    {
                        this.IsMinion = true;   // 一時
                        EditParameter();
                    }
                    else
                    {
                        // 聞かないようにする LSIとか
                        // var isnonsetting = ShowConfirmDialog("never do this question?");
                        var isnonsetting = MessageBox.Show("never do this question?",
                                                                               "FluGAS Initialize setting, miseq only setting?",
                                                                               MessageBoxButton.YesNo,
                                                                               MessageBoxImage.Question);


                        if (isnonsetting == MessageBoxResult.Yes)
                        {
                            Properties.Settings.Default.ont_path = Utils.ConstantValues.NeverAskAgain;
                            Properties.Settings.Default.Save();
                            this.IsMiseq = true;
                            this.IsMinionEnabled = false;
                        }
                    }

                }
            }
        }

        private async void SleepAsync()
        {
            await Task.Delay(5 * 1000).ConfigureAwait(true);
        }
    }
}
