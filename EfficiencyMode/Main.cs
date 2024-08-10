using System;
using System.Runtime.InteropServices;

namespace EfficiencyMode
{
    public class Main
    {
        public Main() { }

        public bool CheckVersion()
        {
            string wpn = VersionHelper.WinProductName;
            int wcb = VersionHelper.WinCurrentBuild;
            string[] wdv = VersionHelper.WinDisplayVersion.Split('H');
            if (wpn.Contains("Windows 10 Pro"))
            {
                if (wcb >= 22000)
                {
                    if (wdv.Length == 2)
                    {
                        int[] wdvints = new int[2];
                        if (Int32.TryParse(wdv[0], out wdvints[0]) && Int32.TryParse(wdv[1], out wdvints[1]))
                        {
                            if (wdvints[0] > 22 || (wdvints[0] == 22 && wdvints[1] >= 2))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool Set(IntPtr phWnd)
        {
            return SetProcessEcoQoS(phWnd, true);
        }

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

        [DllImport("kernel32.dll")]
        private static extern bool SetProcessInformation([In] IntPtr hProcess,
            [In] PROCESS_INFORMATION_CLASS ProcessInformationClass, IntPtr ProcessInformation, uint ProcessInformationSize);

        [DllImport("kernel32.dll")]
        private static extern bool SetPriorityClass(IntPtr handle, uint priorityClass);

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
