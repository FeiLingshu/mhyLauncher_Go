using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MHYLAUNCHER_GO.Functions
{
    public static class WQL
    {
        private static ManagementScope WMIBASE = null;

        private static ManagementObjectSearcher WMIOBJECT = null;

        public static void Connect()
        {
            try
            {
                ConnectionOptions options = new ConnectionOptions
                {
                    Timeout = TimeSpan.FromSeconds(1)
                };
                WMIBASE = new ManagementScope(@"\\.\root\cimv2", options);
                WMIBASE.Connect();
                WMIOBJECT = new ManagementObjectSearcher(WMIBASE, new ObjectQuery());
            }
            catch (Exception) { }
        }

        private static bool Verify()
        {
            if (WMIBASE == null || WMIOBJECT == null)
            {
                Task.Run(() =>
                {
                    MessageBox.Show(
                        "无法连接到WMI服务，将自动回退至兼容实现。",
                        "WMI组件提示...",
                        MessageBoxButton.OK, MessageBoxImage.Warning,
                        MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                });
                return false;
            }
            return true;
        }

        public static bool Test(IEnumerable<string> fullpaths)
        {
            if (!Verify()) return false;
            var paths = fullpaths.ToList();
            if (fullpaths.Count() == 0)
            {
                return true;
            }
            var conditions = fullpaths.Select(p =>
                $"ExecutablePath = '{p.Replace("\\", "\\\\").Replace("'", "''")}'"
            );
            var whereClause = string.Join(" OR ", conditions);
            var query = $"SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE {whereClause}";
            const int SafeThreshold = 8192;
            if (Encoding.Unicode.GetByteCount(query) > SafeThreshold)
            {
                return false;
            }
            return true;
        }

        public static Dictionary<string, List<uint>> GetPidsByNamesCim(IEnumerable<string> fullpaths)
        {
            var result = new Dictionary<string, List<uint>>(fullpaths.Count());
            var conditions = fullpaths.Select(p =>
                $"ExecutablePath = '{p.Replace("\\", "\\\\").Replace("'", "''")}'"
            );
            WMIOBJECT.Query = new ObjectQuery($"SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE {string.Join(" OR ", conditions)}");
            using (var objs = WMIOBJECT.Get())
            {
                foreach (ManagementObject obj in objs.Cast<ManagementObject>())
                {
                    var path = obj["ExecutablePath"]?.GetString() ?? string.Empty;
                    var pid = obj["ProcessId"]?.Getuint() ?? 0U;
                    if (!result.ContainsKey(path) && pid != 0U)
                    {
                        result[path] = new List<uint>();
                    }
                    result[path].Add(pid);
                }
            }
            foreach (var p in fullpaths)
            {
                if (!result.ContainsKey(p))
                {
                    result[p] = new List<uint>();
                }
            }
            return result;
        }

        public static string GetString(this object value)
        {
            if (value is string value_string)
            {
                return value_string;
            }
            else
            {
                return string.Empty;
            }
        }

        public static uint Getuint(this object value)
        {
            if (value is uint value_uint)
            {
                return value_uint;
            }
            else
            {
                return 0U;
            }
        }
    }
}
