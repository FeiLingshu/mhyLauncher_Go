using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace MHYLAUNCHER_GO.Functions
{
    public static class WQL
    {
        public static Dictionary<string, List<uint>> GetPidsByNamesCim(IEnumerable<string> fullpaths)
        {
            var result = new Dictionary<string, List<uint>>(fullpaths.Count());
            var conditions = fullpaths.Select(p =>
                $"ExecutablePath = '{p.Replace("\\", "\\\\").Replace("'", "''")}'"
            );
            var query = $"SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE {string.Join(" OR ", conditions)}";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
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

        public static bool Test(IEnumerable<string> fullpaths)
        {
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
            if (query.Length > SafeThreshold)
            {
                return false;
            }
            return true;
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
