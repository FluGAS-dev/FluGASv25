using FluGASv25.Utils;
using FluGASv25.ViewModels.Base;
using FluGASv25.Views;
using Livet.Commands;
using Livet.Messaging;
using System;
using System.Windows;

namespace FluGASv25.ViewModels
{
    public partial class MainWindowViewModel : ViewModelNotifyDataError
    {
        // 共通
        public ViewModelCommand SelectFolderCommand { private set; get; }
        public ViewModelCommand SelectFileCommand { private set; get; }
        public ViewModelCommand SelectFileClearCommand { private set; get; }
        public ViewModelCommand SelectUserFastaCommand { private set; get; }

        // Analyses
        public ViewModelCommand EditParameterCommand { private set; get; }
        public ViewModelCommand AnarysisExecuteCommand { set; get; }
        public ViewModelCommand SelectDataCommand { private set; get; }


        // Results
        public ViewModelCommand OpenInformationCommand { set; get; }
        public ViewModelCommand SampleEditCommand { set; get; }
        public ViewModelCommand SampleDeleteCommand { set; get; }
        public ViewModelCommand ChangeViewNameCommand { set; get; }

        // LivetCallMethodAction へ 移行した
        // public ViewModelCommand AlignmentViewTypeACommand { private set; get; }
        // public ViewModelCommand AlignmentViewTypeBCommand { private set; get; }
        public ViewModelCommand CreateExcelCommand { set; get; }

        // Reference
        public ViewModelCommand SelectAhaFastaCommand { private set; get; }
        public ViewModelCommand SelectAnaFastaCommand { private set; get; }
        public ViewModelCommand SelectYamagataFastaCommand { private set; get; }
        public ViewModelCommand SelectVictoriaFastaCommand { private set; get; }

        public ViewModelCommand ReferenceUpdateCommand { private set; get; }
        public ViewModelCommand DicisionReferenceUpdateCommand { set; get; }
        public ViewModelCommand SetDefaultReferenceParameterCommand { set; get; }

        protected bool isLicenseActivate;
        protected void OpenInformation()
        {
            System.Diagnostics.Debug.WriteLine("call information");
            using (var infoView = new InformationViewModel())
            {
                Messenger.Raise(new TransitionMessage(infoView, "InformationCommand"));
                this.isLicenseActivate = infoView.IsLicenceActivate;  // license が正常に認識されれば true

                System.Diagnostics.Debug.WriteLine("retuen license is " + infoView.IsLicenceActivate); // open URL or PDF
                // ライセンス認証が通って居ればそのまま。
                if (this.isLicenseActivate) return;

                // ライセンス認証通ってなければ再検査。
                if (EnvInfo.IsLicenceInvalid())// license invalid
                {
                    MessageBox.Show("License error. " + Environment.NewLine +
                                            "License is invalid!");
                    System.Threading.Thread.Sleep(1800);
                    Application.Current.Shutdown();
                }
            };

            /*
            if (ShowConfirmDialog("your address : " + Utils.EnvInfo.firstAddress + Environment.NewLine +
                                            "contact us? (open web page)", "information") ) {

                Utils.Approbate.OpenUrl("https://www.w-fusion.co.jp/J/contactus.php");
                System.Diagnostics.Debug.WriteLine("information click."); // open URL or PDF
            }
            */
        }


    }
}
