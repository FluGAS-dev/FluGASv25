using FluGASv25.Dao;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FluGASv25.Utils
{
    public static class Approbate
    {

        public const string LicenceFileName = "license_010000.lic";
        private const int SALT_LENGTH = 16;
        private const int AUTH_KEY_LENGTH = 8;
        private const int IV_LENGTH = 16;
        private const int ITERATION_COUNT = 32768;

        public static string DefaultLicenceFilePath =>
                                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        "data",
                                         LicenceFileName);
        public static bool IsNonApprobate(string LicenceFilePath)
        {
            if (string.IsNullOrEmpty(LicenceFilePath))
                LicenceFilePath = DefaultLicenceFilePath;

            // File read 所定の場所に無ければ起動しない
            // if (!File.Exists(LicenceFilePath)) return true;
            if (!File.Exists(LicenceFilePath)) return IsValidDate();

            var errorMessage = string.Empty;
            var liclines = WfComponent.Utils.FileUtils.ReadFile(LicenceFilePath, ref errorMessage);
            if (!string.IsNullOrEmpty(errorMessage)) return true;

            var address = string.Empty;
            var lickey = string.Empty;
            foreach (var line in liclines)
            {
                var keyval = line.Split(':');
                if (keyval.First().StartsWith("Email"))
                    address = keyval.Last().Trim();

                if (keyval.First().StartsWith("License"))
                    lickey = keyval.Last().Trim();
            }
            // 指定のキーが
            if (string.IsNullOrEmpty(address) ||
                string.IsNullOrEmpty(lickey)) return true;


            var licence = GetLicaddress(address, lickey);
            var properties = GetProperties(licence);

            // date address is accept.
            if (IsInvalidDate(properties.endDate) && IsComprise(properties.address))
                return false;

            return true;
        }

        private static string GetLicaddress(string address, string lickey)
        {
            var dataBytes = Convert.FromBase64String(lickey);
            var pass = address.ToCharArray();

            int byteOffset = 0;
            int keyLength = 8 * dataBytes[byteOffset]; // 128?

            /* Retrieves salt */
            byteOffset = 1;
            byte[] salt = (new ArraySegment<byte>(dataBytes, byteOffset, SALT_LENGTH)).ToArray();

            byteOffset += SALT_LENGTH;
            byte[] authKey = (new ArraySegment<byte>(dataBytes, byteOffset, AUTH_KEY_LENGTH)).ToArray();

            byteOffset += AUTH_KEY_LENGTH;
            byte[] iv = (new ArraySegment<byte>(dataBytes, byteOffset, IV_LENGTH)).ToArray();

            byteOffset += IV_LENGTH;
            byte[] inms = (new ArraySegment<byte>(dataBytes, byteOffset, 48)).ToArray();


            authKey = DecodeKey(keyLength, address, salt);

            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = authKey;
            aes.IV = iv;

            var plainText = string.Empty;
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                var decBytes = decrypt.TransformFinalBlock(inms, 0, inms.Length);
                plainText = Encoding.ASCII.GetString(decBytes);
            }
            return plainText;
        }

        private static byte[] DecodeKey(int keyLength, string password, byte[] salt)
        {
            var encoding = new ASCIIEncoding();
            var kf = new Rfc2898DeriveBytes(password, salt, ITERATION_COUNT, HashAlgorithmName.SHA1);
            byte[] encodedKey = kf.GetBytes(192);

            var byteOffset = 0;
            var secretKey = (new ArraySegment<byte>(encodedKey, byteOffset, AUTH_KEY_LENGTH)).ToArray();

            byteOffset += AUTH_KEY_LENGTH;
            var encryptionKey = (new ArraySegment<byte>(encodedKey, byteOffset, IV_LENGTH)).ToArray();

            // return string.Empty;
            return encryptionKey;
        }

        private static string Decode(string address, string lickey)
        {
            try
            {
                var keyiv = string.Empty;
                while (keyiv.Count() < 48)
                {
                    keyiv += Gatkey(address);
                }
                var key = keyiv.Substring(0, 32);
                var iv = keyiv.Substring(32, 16);
                var plainText = string.Empty;

                var csp = new AesCryptoServiceProvider();
                csp.BlockSize = 128;
                csp.KeySize = 256;
                csp.Mode = CipherMode.CBC;
                csp.Padding = PaddingMode.PKCS7;
                // csp.Padding = PaddingMode.Zeros;
                // csp.Padding = PaddingMode.None;
                // csp.Padding = PaddingMode.ANSIX923;
                // csp.Padding = PaddingMode.ISO10126;

                csp.IV = Encoding.UTF8.GetBytes(iv);
                csp.Key = Encoding.UTF8.GetBytes(key);

                var s = Convert.FromBase64String(lickey);

                using (var inms = new MemoryStream(Convert.FromBase64String(lickey)))
                using (var decryptor = csp.CreateDecryptor())
                using (var cs = new CryptoStream(inms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    plainText = reader.ReadToEnd();
                }
                return plainText;
            }
            catch (Exception e)
            {
                Utils.VariousUtils.WriteError("app.err", e.Message);
                return string.Empty;
            }
        }

        public static string Encode(string str)
        {
            var key = "00000000000000000000000000000000";
            var iv = "0000000000000000";
            byte[] encrypted;

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(str);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        private static string Gatkey(string str)
        {
            var res = string.Empty;
            using (var sha = SHA256CryptoServiceProvider.Create())
            {
                var val = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
                foreach (var s in val)
                {
                    res = res + string.Format("{0:X2}", s);
                }
            }
            return res.ToLower();
        }


        private static ApprobatePropertirs GetProperties(string word)
        {
            if (string.IsNullOrEmpty(word))
                return null;

            var words = word.Split('_');
            if (words.Count() < 3)
                return null;

            var properties = new ApprobatePropertirs
            {
                header = words[0],
                address = words[1],
                endDate = words[2]
            };

            return properties;
        }

        private static bool IsComprise(string address)
        {
            var comprise = false;
            address = address.Replace("-", string.Empty).ToLower();
            if (GetAddress().StartsWith(address)) return true;

            var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in nics)
            {
                var localaddress = nic.GetPhysicalAddress().ToString().ToLower();
                if (localaddress.Equals(address))
                    comprise = true;
            }
            return comprise;
        }

        private static bool IsInvalidDate(string licenseDate)
        {
            if (string.IsNullOrEmpty(licenseDate) ||
                licenseDate.Count() < 8)
                return false;

            var licymd = licenseDate.Split('-');

            int.TryParse(licymd.First(), out int licyyyy);
            int.TryParse(licymd[1], out int licmm);
            int.TryParse(licymd.Last(), out int licdd);

            var licdate = new DateTime(licyyyy, licmm, licdd - 2);
            return (licdate > DateTime.Now);
        }

        private static string GetAddress()
        {

            RijndaelManaged aes = new RijndaelManaged();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] keyArr = Convert.FromBase64String(new string('x', 44));
            byte[] KeyArrBytes32Value = new byte[32];
            Array.Copy(keyArr, KeyArrBytes32Value, 32);
            byte[] ivArr = { 1, 2, 3, 4, 5, 6, 6, 5, 4, 3, 2, 1, 7, 7, 7, 7 };
            byte[] IVBytes16Value = new byte[16];
            Array.Copy(ivArr, IVBytes16Value, 16);

            aes.Key = KeyArrBytes32Value;
            aes.IV = IVBytes16Value;

            ICryptoTransform decrypto = aes.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64CharArray("b2VxfyFzqESXQ3Et55QuGg==".ToCharArray(), 0, 24);
            byte[] decryptedData = decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return ASCIIEncoding.ASCII.GetString(decryptedData);
        }


        // Processing when there is no file
        private static bool IsValidDate()
        {
            // demo license term?
            var s = MinionParameterDao.GetParameters();
            if (!s.Any()) return false;

            var initDate = s.First().CreateDate;
            if (initDate == null) return false;

            var validDbDate = initDate.Value.AddDays(15); // 2weeks
            var validThisData = Properties.Settings.Default.init_date.AddMonths(12);
            var currentDate = DateTime.Now;

            if (validDbDate < currentDate || validThisData < currentDate) {
                return true;   // デモ期間切れた。
            }
            else {
                Properties.Settings.Default.demo_mode = "  demo-license:" + validDbDate;
                Properties.Settings.Default.Save();
                return false;
            }
        }

        // 指定のURLを標準ブラウザで開く
        public static void OpenUrl(string targetUrl)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = targetUrl,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        public static void OpenApp(string targetCmd)
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {targetCmd}") );

            /**
            ProcessStartInfo info = new ProcessStartInfo();
            // URLに関連づけられたアプリケーションを探す
            RegistryKey rkey = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command");
            var val = rkey.GetValue("").ToString();

            if (val.StartsWith("\""))
            {
                int n = val.IndexOf("\"", 1);
                info.FileName = val.Substring(1, n - 1);
                info.Arguments = val.Substring(n + 1);
            }
            else
            {
                string[] a = val.Split(new char[] { ' ' });
                info.FileName = a[0];
                info.Arguments = val.Substring(a[0].Length + 1);
            }
            
            info.WorkingDirectory = Path.GetDirectoryName(info.FileName);
            info.Arguments = targetCmd;
            Process.Start(info);
    */
        }

        private class ApprobatePropertirs
        {
            public string header;
            public string address;
            public string endDate;
        }


    }

}
