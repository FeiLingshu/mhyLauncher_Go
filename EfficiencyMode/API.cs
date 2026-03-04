using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyMode
{
    /// <summary>
    /// 提供win32交互的静态类
    /// </summary>
    public static class API
    {
        /// <summary>
        /// win32enum::ProcessAccessFlags
        /// </summary>
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            PROCESS_SET_INFORMATION = 0x00000200,
            PROCESS_QUERY_INFORMATION = 0x00000400
        }

        /// <summary>
        /// win32api::OpenProcess
        /// </summary>
        /// <param name="processAccess">param#1</param>
        /// <param name="bInheritHandle">param#2</param>
        /// <param name="processId">param#3</param>
        /// <returns>returns</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            int processId);

        /// <summary>
        /// win32api::SetProcessAffinityMask
        /// </summary>
        /// <param name="hProcess">param#1</param>
        /// <param name="dwProcessAffinityMask">param#2</param>
        /// <returns>returns</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern ulong SetProcessAffinityMask(IntPtr hProcess, ulong dwProcessAffinityMask);

        /// <summary>
        /// win32api::CloseHandle
        /// </summary>
        /// <param name="hObject">param#1</param>
        /// <returns>returns</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
