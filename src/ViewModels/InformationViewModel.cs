using FluGASv25.Utils;
using System.IO;
using System.Linq;

namespace FluGASv25.ViewModels
{
    public class InformationViewModel : DialogViewModel
    {
        public InformationViewModel()
        {
            // CommandInit();
            System.Diagnostics.Debug.WriteLine("information ViewModel consractor.");
        }

        public void Initialize() // ContentRendered.
        {
            System.Diagnostics.Debug.WriteLine("information ViewModel Initialize");

        }

        public void CallOpenManual()
        {
            System.Diagnostics.Debug.WriteLine("call open manual.");
            var pdf = Path.Combine(
                                    System.AppDomain.CurrentDomain.BaseDirectory,
                                    "data",
                                    "FluGASv2.pdf");
            if (File.Exists(pdf))
                Utils.Approbate.OpenApp(pdf);
            else
                Utils.Approbate.OpenUrl("https://www.w-fusion.co.jp/J/productlist/Flugasn.html");
        }

        public void CallOpenContact()
        {
            System.Diagnostics.Debug.WriteLine("call open url.");
            Utils.Approbate.OpenUrl("https://www.w-fusion.co.jp/J/contactus.php");
        }


        private bool isActivate = false;
        public bool IsLicenceActivate => isActivate;
        public void CallSelectLicense()
        {
            System.Diagnostics.Debug.WriteLine("call open lisence.");
            var licFile = SelectFileDialog(   // windows api dialog.
                       "Select your license file",
                        false,
                        false,
                        Approbate.LicenceFileName);

            if (licFile.Any() && File.Exists(licFile.First()))
                this.LicenseFile = licFile.First();


            // File null (close window) =>  IsActivateLicence = false;
        }

        public void CallAcceptLicense()
        {
            if (File.Exists(this._licenseFile))
            {
                var message = string.Empty;
                WfComponent.Utils.FileUtils.FileCopy(
                                            _licenseFile,   // user select file pass
                                            Approbate.DefaultLicenceFilePath,
                                            ref message);

                System.Threading.Thread.Sleep(1000);
                if (string.IsNullOrEmpty(message))
                {  
                    // copy success.
                    if (EnvInfo.IsLicenceInvalid())// license invalid
                    {
                        ViewClose(); // 閉じる
                        return; // IsActivateLicence = false;
                    }
                    else
                    {   // 
                        ShowInfoDialog("license is activated.", "license file accept.");
                        Properties.Settings.Default.demo_mode = string.Empty;
                        Properties.Settings.Default.Save();
                        isActivate = true;
                    }
                }
                else
                {
                    ShowErrorDialog("license file copy error");
                }
            }
            // File null (close window) =>  IsActivateLicence = false;
            ViewClose(); // 閉じる
        }

        private string _nicAddress = "your address :  " + Utils.EnvInfo.firstAddress;
        public string NicAddress
        {
            get { return _nicAddress; }
            set { RaisePropertyChangedIfSet(ref _nicAddress, value); }
        }


        private string _licenseFile;
        public string LicenseFile
        {
            get { return _licenseFile; }
            set { RaisePropertyChangedIfSet(ref _licenseFile, value); }
        }

    }
}
