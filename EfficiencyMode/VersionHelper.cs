using Microsoft.Win32;
using System;

namespace EfficiencyMode
{
    public static class VersionHelper
    {
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public static string WinProductName
        {
            get
            {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", out var productName))
                {
                    return (string)productName;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 操作系统主要版本号
        /// </summary>
        public static uint WinMajorVersion
        {
            get
            {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out var major))
                {
                    return (uint)major;
                }

                if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out var version))
                    return 0;

                var versionParts = ((string)version).Split('.');
                if (versionParts.Length != 2) return 0;
                return uint.TryParse(versionParts[0], out var majorAsUInt) ? majorAsUInt : 0;
            }
        }

        /// <summary>
        /// 操作系统次要版本号
        /// </summary>
        public static uint WinMinorVersion
        {
            get
            {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber", out var minor))
                {
                    return (uint)minor;
                }

                if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out var version))
                    return 0;

                var versionParts = ((string)version).Split('.');
                if (versionParts.Length != 2) return 0;
                return uint.TryParse(versionParts[1], out var minorAsUInt) ? minorAsUInt : 0;
            }
        }

        /// <summary>
        /// 判断是否为Server系统
        /// </summary>
        public static uint IsServer
        {
            get
            {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "InstallationType",
                        out var installationType))
                {
                    return (uint)(installationType.Equals("Client") ? 0 : 1);
                }

                return 0;
            }
        }

        /// <summary>
        /// 操作系统更新补丁编号
        /// </summary>
        public static string WinDisplayVersion
        {
            get
            {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DisplayVersion", out var displayVersion))
                {
                    return (string)displayVersion;
                }
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 操作系统内部版本号
        /// </summary>
        public static int WinCurrentBuild
        {
            get
            {
                if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuild", out var currentBuild))
                {
                    if (Int32.TryParse(currentBuild, out int currentBuild_int))
                    {
                        return currentBuild_int;
                    }
                    else
                    {
                        return Environment.OSVersion.Version.Build;
                    };
                }
                else if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", out var currentBuildNumber))
                {
                    if (Int32.TryParse(currentBuildNumber, out int currentBuildNumber_int))
                    {
                        return currentBuildNumber_int;
                    }
                    else
                    {
                        return Environment.OSVersion.Version.Build;
                    };
                }
                return Environment.OSVersion.Version.Build;
            }
        }

        /// <summary>
        /// 获取注册表值
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        private static bool TryGetRegistryKey(string path, string key, out dynamic value)
        {
            value = null;
            try
            {
                using (var rk = Registry.LocalMachine.OpenSubKey(path))
                {
                    if (rk == null) return false;
                    value = rk.GetValue(key);
                    return value != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
