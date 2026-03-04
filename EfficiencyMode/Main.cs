using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static EfficiencyMode.API;

namespace EfficiencyMode
{
    /// <summary>
    /// EfficiencyMode类
    /// </summary>
    public class Main
    {
        /// <summary>
        /// EfficiencyMode主入口点
        /// <para>
        /// DLL不通过主入口点调用相关函数，而是通过加载DLL并利用反射执行相关内部公开函数
        /// </para>
        /// </summary>
        public Main() { }

        /// <summary>
        /// 检查系统版本（必须的前置操作）
        /// </summary>
        /// <returns>返回版本是否符合条件</returns>
        public bool CheckVersion()
        {
            string wpn = VersionHelper.WinProductName;
            int wcb = VersionHelper.WinCurrentBuild;
            string wri = VersionHelper.WinReleaseId;
            if (string.IsNullOrEmpty(wpn) && (wpn.Contains("Windows 10") || wpn.Contains("Windows 11")))
            {
                if (int.TryParse(wri, out int wriint) && wriint >= 1803)
                {
                    return true;
                }
                else
                {
                    if (wcb >= 17134)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return Environment.OSVersion.Version >= new Version(10, 0, 17134, 0);
            }
            return false;
        }

        /// <summary>
        /// 进行效能模式配置
        /// </summary>
        /// <param name="process">进程实例</param>
        /// <returns>返回操作是否成功</returns>
        public bool Set(Process process)
        {
            IntPtr hProcess = OpenProcess(
                ProcessAccessFlags.PROCESS_SET_INFORMATION | ProcessAccessFlags.PROCESS_QUERY_INFORMATION,
                false,
                process.Id);
            try
            {
                return SetProcessEcoQoS(hProcess, true);
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// 用于配置效能模式的内部方法
        /// </summary>
        /// <param name="hProcess">目标进程句柄</param>
        /// <param name="bFlag">开启标志</param>
        /// <returns>返回操作是否成功</returns>
        private bool SetProcessEcoQoS(IntPtr hProcess, bool bFlag)
        {
            // 此结构有三个字段Version，ControlMask 和 StateMask
            uint version = 1;
            uint controlMask = 0x1; //非权重开关
            uint stateMask = (uint)(bFlag ? 0x1 : 0x0);
            int szControlBlock = 12; // 三个uint的大小
            IntPtr homo = Marshal.AllocHGlobal(szControlBlock);
            Marshal.WriteInt32(homo, (int)version); //homo 指向内存块开头
            Marshal.WriteInt32(homo + 4, (int)controlMask); // 将 controlMask 值写入第2字段地址，需将 homo 指针加4字节
            Marshal.WriteInt32(homo + 8, (int)stateMask); // 将 stateMask 值写入第3个字段地址，需将 homo 指针加8个字节
            bool result = true;
            result &= SetProcessInformation(hProcess, PROCESS_INFORMATION_CLASS.ProcessPowerThrottling, homo, (uint)szControlBlock);
            result &= SetPriorityClass(hProcess, (uint)(bFlag ? 0x40 : 0x20));
            Marshal.FreeHGlobal(homo);
            return result;
        }

        /// <summary>
        /// win32api::SetProcessInformation
        /// </summary>
        /// <param name="hProcess">param#1</param>
        /// <param name="ProcessInformationClass">param#2</param>
        /// <param name="ProcessInformation">param#3</param>
        /// <param name="ProcessInformationSize">param#4</param>
        /// <returns>return value</returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetProcessInformation([In] IntPtr hProcess,
            [In] PROCESS_INFORMATION_CLASS ProcessInformationClass, IntPtr ProcessInformation, uint ProcessInformationSize);

        /// <summary>
        /// win32api::SetPriorityClass
        /// </summary>
        /// <param name="handle">param#1</param>
        /// <param name="priorityClass">param#2</param>
        /// <returns>return value</returns>
        [DllImport("kernel32.dll")]
        private static extern bool SetPriorityClass(IntPtr handle, uint priorityClass);

        /// <summary>
        /// win32enum::PROCESS_INFORMATION_CLASS
        /// </summary>
        private enum PROCESS_INFORMATION_CLASS
        {
            ProcessMemoryPriority,
            ProcessMemoryExhaustionInfo,
            ProcessAppMemoryInfo,
            ProcessInPrivateInfo,
            ProcessPowerThrottling,
            ProcessReservedValue1,
            ProcessTelemetryCoverageInfo,
            ProcessProtectionLevelInfo,
            ProcessLeapSecondInfo,
            ProcessInformationClassMax,
        }
    }
}
