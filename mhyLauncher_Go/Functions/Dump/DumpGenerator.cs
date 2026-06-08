using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace MHYLAUNCHER_GO.Functions.Dump
{
    /// <summary>
    /// 用于生成Dump文件的公开类
    /// </summary>
    public static class DumpGenerator
    {
        /// <summary>
        /// Win32API::MiniDumpWriteDump
        /// </summary>
        /// <param name="hProcess">参数#1</param>
        /// <param name="ProcessId">参数#2</param>
        /// <param name="hFile">参数#3</param>
        /// <param name="DumpType">参数#4</param>
        /// <param name="ExceptionParam">参数#5</param>
        /// <param name="UserStreamParam">参数#6</param>
        /// <param name="CallbackParam">参数#7</param>
        /// <returns>返回值(bool)</returns>
        [DllImport("dbghelp.dll")]
        private static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint ProcessId,
            IntPtr hFile,
            MINIDUMP_TYPE DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam);

        /// <summary>
        /// Win32Enum::MINIDUMP_TYPE
        /// </summary>
        [Flags]
        private enum MINIDUMP_TYPE : uint
        {
            /// <summary>
            /// 枚举#1
            /// </summary>
            MiniDumpNormal = 0x00000000,
            /// <summary>
            /// 枚举#2
            /// </summary>
            MiniDumpWithDataSegs = 0x00000001,
            /// <summary>
            /// 枚举#3
            /// </summary>
            MiniDumpWithFullMemory = 0x00000002,
            /// <summary>
            /// 枚举#4
            /// </summary>
            MiniDumpWithHandleData = 0x00000004,
            /// <summary>
            /// 枚举#5
            /// </summary>
            MiniDumpFilterMemory = 0x00000008,
            /// <summary>
            /// 枚举#6
            /// </summary>
            MiniDumpScanMemory = 0x00000010,
            /// <summary>
            /// 枚举#7
            /// </summary>
            MiniDumpWithUnloadedModules = 0x00000020,
            /// <summary>
            /// 枚举#8
            /// </summary>
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            /// <summary>
            /// 枚举#9
            /// </summary>
            MiniDumpFilterModulePaths = 0x00000080,
            /// <summary>
            /// 枚举#10
            /// </summary>
            MiniDumpWithProcessThreadData = 0x00000100,
            /// <summary>
            /// 枚举#11
            /// </summary>
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            /// <summary>
            /// 枚举#12
            /// </summary>
            MiniDumpWithoutOptionalData = 0x00000400,
            /// <summary>
            /// 枚举#13
            /// </summary>
            MiniDumpWithFullMemoryInfo = 0x00000800,
            /// <summary>
            /// 枚举#14
            /// </summary>
            MiniDumpWithThreadInfo = 0x00001000,
            /// <summary>
            /// 枚举#15
            /// </summary>
            MiniDumpWithCodeSegs = 0x00002000,
            /// <summary>
            /// 枚举#16
            /// </summary>
            MiniDumpWithoutAuxiliaryState = 0x00004000,
            /// <summary>
            /// 枚举#17
            /// </summary>
            MiniDumpWithFullAuxiliaryState = 0x00008000,
            /// <summary>
            /// 枚举#18
            /// </summary>
            MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
            /// <summary>
            /// 枚举#19
            /// </summary>
            MiniDumpIgnoreInaccessibleMemory = 0x00020000,
            /// <summary>
            /// 枚举#20
            /// </summary>
            MiniDumpWithTokenInformation = 0x00040000,
            /// <summary>
            /// 枚举#21
            /// </summary>
            MiniDumpWithModuleHeaders = 0x00080000,
            /// <summary>
            /// 枚举#22
            /// </summary>
            MiniDumpFilterTriage = 0x00100000,
            /// <summary>
            /// 枚举#23
            /// </summary>
            MiniDumpValidTypeFlags = 0x001fffff
        }

        /// <summary>
        /// 在指定路径生成Dump文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void GenerateDump(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    IntPtr processHandle = process.Handle;
                    uint processId = (uint)process.Id;
                    MINIDUMP_TYPE dumpType = MINIDUMP_TYPE.MiniDumpWithFullMemory |
                         MINIDUMP_TYPE.MiniDumpWithHandleData |
                         MINIDUMP_TYPE.MiniDumpWithThreadInfo;
                    MiniDumpWriteDump(
                        processHandle,
                        processId,
                        fs.SafeFileHandle.DangerousGetHandle(),
                        dumpType,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
            }
        }
    }
}
