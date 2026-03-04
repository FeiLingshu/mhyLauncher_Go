using System;
using System.Management;
using static EfficiencyMode.API;

namespace EfficiencyMode
{
    /// <summary>
    /// 用于实现性能配置的静态类
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// 指示数据状态的内部字段
        /// </summary>
        private static bool CoreState = false;

        /// <summary>
        /// P核心Map
        /// </summary>
        private static uint PCoreMap = 0U;

        /// <summary>
        /// E核心Map
        /// </summary>
        private static uint ECoreMap = 0U;

        /// <summary>
        /// 获取核心参数并生成Map（写入缓存）
        /// </summary>
        /// <returns>返回操作是否成功</returns>
        public static bool GetCoreMap()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
                int corecount = 0;
                foreach (ManagementBaseObject obj in searcher.Get())
                {
                    object cores = obj["NumberOfCores"];
                    if (cores == null)
                    {
                        continue;
                    }
                    bool success = int.TryParse(cores.ToString(), out int coreCount);
                    if (success)
                    {
                        corecount += coreCount;
                    }
                }
                int processorcount = Environment.ProcessorCount;
                if (processorcount > 64)
                {
                    CoreState = false;
                    return false;
                }
                else
                {
                    bool reportcheck = processorcount >= corecount && processorcount <= corecount * 2;
                    if (!reportcheck || corecount == processorcount || corecount * 2 == processorcount)
                    {
                        CoreState = false;
                        return false;
                    }
                    else
                    {
                        PCoreMap = 0U;
                        for (int i = 0; i < (processorcount - corecount) * 2; i++)
                        {
                            PCoreMap |= (1U << i);
                        }
                        ECoreMap = 0U;
                        for (int i = (processorcount - corecount) * 2; i < processorcount; i++)
                        {
                            ECoreMap |= (1U << i);
                        }
                        CoreState = true;
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                CoreState = false;
                return false;
            }
        }

        /// <summary>
        /// 设置指定进程的CPU核心亲和性
        /// </summary>
        /// <param name="PID">目标进程ID</param>
        /// <param name="PoE">指示绑定到P核还是E核（true=P, false=E）</param>
        /// <returns>返回操作是否成功</returns>
        public static bool SetProcess(int PID, bool PoE)
        {
            if (CoreState)
            {
                IntPtr hProcess = OpenProcess(
                    ProcessAccessFlags.PROCESS_SET_INFORMATION | ProcessAccessFlags.PROCESS_QUERY_INFORMATION,
                    false,
                    PID);
                try
                {
                    if (hProcess == IntPtr.Zero)
                    {
                        return false;
                    }
                    if (SetProcessAffinityMask(hProcess, PoE ? PCoreMap : ECoreMap) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                finally
                {
                    CloseHandle(hProcess);
                }
            }
            else
            {
                return false;
            }
        }
    }
}
