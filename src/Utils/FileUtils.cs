using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace FluGASv25.Utils
{
    public static class FileUtils 
    {

        public static string GetBarcodeName(string baseName, int barcodeNo)
        {
            if(string.IsNullOrEmpty(baseName))
                return ConstantValues.barcode + barcodeNo.ToString("00");

            return "[" + baseName + "]-" + ConstantValues.barcode + barcodeNo.ToString("00");
        }

        public static string GetBarcodeName(string baseName, string barcode)
        {
            // barcode = barcode01 とか
            if (string.IsNullOrEmpty(baseName))
                return barcode;

            return "[" + baseName + "]-" +  barcode;
        }

    }

}
