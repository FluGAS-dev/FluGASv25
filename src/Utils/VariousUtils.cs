using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FluGASv25.Utils
{
    public static class VariousUtils
    {
        public static void MoveProperties(ref object fromObj, ref object toObj)
        {
            var f = fromObj.GetType().GetProperties().Select(s => s.Name).ToArray();
            var t = toObj.GetType().GetProperties().Select(s => s.Name).ToArray();

            foreach (var fp in f)
            {
                if (t.Contains(fp))
                    toObj.GetType().GetProperty(fp.ToString()).SetValue(toObj,
                        fromObj.GetType().GetProperty(fp.ToString()).GetValue(fromObj));
            }
        }

        public static string DirectoryBackup(string dirPath, string backupDate, bool isNewCreate = true)
        {
            var bkDir = dirPath + "-" + backupDate;

            // 既存リファレンスのバックアップ
            if (System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.Move(dirPath, bkDir);
                if(isNewCreate)
                    System.IO.Directory.CreateDirectory(dirPath);
            }

            // バックアップ後のディレクトリ名を返す。
            return bkDir;
        }


        public static string DirectoryRollback(string dirPath, string backupDate)
        {
            var bkDir = dirPath + "-" + backupDate;
            var badDir = dirPath + "-" + backupDate + "-falt";

            if (System.IO.Directory.Exists(bkDir))
            {
                // 新規に作成したと思われるディレクトリを保存する。
                if (System.IO.Directory.Exists(dirPath)) System.IO.Directory.Move(dirPath, badDir);
                System.IO.Directory.Move(bkDir, dirPath);
            }
            else
            {
                return "not found backuped directory. " + bkDir;
            }

            return badDir;
        }

        public static bool IsFolderFast5Fastq(IEnumerable<string> vs)
        {
            var isFolder = IsFloderInclude(vs);
            var isFast5 = IsFast5Include(vs);
            var isFastq = IsFastqInclude(vs);

            if ((isFolder || isFast5) && isFastq)
                return true;  // mixed data

            return false; // uniq data type
        }

        public static bool IsFloderInclude(IEnumerable<string> vs)
        {
            if (vs == null) return false;
            foreach(var s in vs)
                if (Directory.Exists(s))
                    return true;

            return false;
        }

        public static bool IsFast5Include(IEnumerable<string> vs)
        {
            if (vs == null) return false;
            var list = vs.Where(s => s.EndsWith(".fast5"));

            if (list.Any()) return true;
            return false;
        }

        public static bool IsFastqInclude(IEnumerable<string> vs)
        {
            if (vs == null) return false;
            var list = vs.Where(s => s.EndsWith(".fastq") || s.EndsWith(".gz"));

            if (list.Any()) return true;
            return false;
        }

        public static void WriteError(string logName, string errString)
        {
            var outDir = System.IO.Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                WfComponent.Utils.ConstantValues.CurrentLogDir);
            if (!System.IO.Directory.Exists(outDir)) System.IO.Directory.CreateDirectory(outDir);

            try { 
                System.IO.File.WriteAllText(System.IO.Path.Combine(outDir, logName), errString );
            }
            catch { 
                WfComponent.Utils.FileUtils.WriteUniqDateLog(errString);
            }
        }

        public static bool IsFastqLines(IEnumerable<string> lines)
        {
            var cnt = 0;
            foreach (var line in lines)
            {
                switch (cnt % 4)
                {
                    case 0:
                        if (!line.StartsWith("@")) return false;
                        break;
                    case 1:
                        var charcnt = line.Length;
                        var agct = line.ToLower().Replace("a", string.Empty).Replace("g", string.Empty).Replace("c", string.Empty).Replace("t", string.Empty);
                        if (agct.Length / charcnt > 0.2) return false;
                        break;
                }
                cnt += 1;
            }
            return true;
        }

        public static string ShortNameString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            
            var ss = str.Split('_', '-', '.');
            if(ss.Length > 3)
            {
                var res = new List<string>();
                foreach(var s in ss)
                {
                    if (int.TryParse(s, out int sint)) continue;  // date?
                    if (s.Contains("sequencing", StringComparison.OrdinalIgnoreCase) ||
                        s.Contains("read", StringComparison.OrdinalIgnoreCase) ||
                        s.Contains("run", StringComparison.OrdinalIgnoreCase) ||
                        s.Contains("ch", StringComparison.OrdinalIgnoreCase) ||
                        s.Contains("ont", StringComparison.OrdinalIgnoreCase)) continue;

                    res.Add(s);
                }
                return string.Join("_", res);
            }

            // 目安がないので、適当に30文字で
            if (str.Length > 30)
                return str.Substring(0, 30);

            // そのまま返す
            return str;
        }

        public static string Path2String(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var s = path.Split( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.PathSeparator, Path.VolumeSeparatorChar, '.');
            return string.Join("_", s);
        }


        // dotnet core は Process.start で Program を指定しないとWin32Exception になる?
        public static string StartExcel(string excelFilePath)
        {
            if (!File.Exists(excelFilePath)) return "not found file, " + excelFilePath ;

            // 関連付けられているアプリ
            var shell = EnvInfo.FindAssociatedCommand(excelFilePath);
            if (string.IsNullOrEmpty(shell)) 
                return "connected program was not found." + Path.GetExtension(excelFilePath);

            var pg = shell.Split(" ").First();

            return ExecCommandLeave(pg, excelFilePath);  // 何も無ければ string.empty;
        }

        // Windowsコマンド　投げっぱなし版 (IGV起動とかに使う)
        public static string ExecCommandLeave(string command, string arguments)
        {
            var message = string.Empty; // init.

            // tmp directory
            var workdir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var startInfo = new ProcessStartInfo(command, arguments)
            {
                ErrorDialog = true,
                WorkingDirectory = workdir,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                using (var process = Process.Start(startInfo))
                {
                    // process.BeginOutputReadLine();
                    // process.BeginErrorReadLine();
                    // process.CancelOutputRead();
                    // process.CancelErrorRead();
                }
            }
            catch (Exception e)
            {
                message += "-----   Exception Message\n" + e.Message + "\n";
                message += "-----   Exception StackTrace\n" + e.StackTrace + "\n";
            }

            return message;   // 何も無ければ string.empty
        }

    }
}
