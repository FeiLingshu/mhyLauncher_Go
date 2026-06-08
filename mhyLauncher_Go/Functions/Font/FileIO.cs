using System.Diagnostics;
using System.IO;
using static MHYLAUNCHER_GO.Functions.BugFix;

namespace MHYLAUNCHER_GO.Functions.Font
{
    public static class FileIO
    {
        public static ProcessStartInfo DEL = new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            Arguments = $"/c rd /s /q ",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            StandardOutputEncoding = utf8,
            StandardErrorEncoding = utf8
        };

        public static ProcessStartInfo SH = new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            Arguments = $"/c attrib +s +h ",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = false,
            StandardOutputEncoding = utf8,
            StandardErrorEncoding = utf8
        };

        public static int DeleteDirectory(this string dir)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                throw new FileNotFoundException($"目录 ({dir}) 不存在。");
            }
            DEL.Arguments = $"/c rd /s /q \"{dir}\"";
            using (var process = Process.Start(DEL))
            {
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        public static int SetAttrib(this string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                throw new FileNotFoundException($"文件 ({file}) 不存在。");
            }
            SH.Arguments = $"/c attrib +s +h \"{file}\"";
            using (var process = Process.Start(SH))
            {
                process.WaitForExit();
                return process.ExitCode;
            }
        }
    }
}
