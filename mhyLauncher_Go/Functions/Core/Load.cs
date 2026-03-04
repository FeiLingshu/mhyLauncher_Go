using System;
using System.Diagnostics;
using System.Reflection;

namespace MHYLAUNCHER_GO.Functions.Core
{
    /// <summary>
    /// 用于缓存静态反射实例的静态类
    /// <para>
    /// 使用静态类缓存相关数据，确保DLL在程序集中始终存在，以便后续调用相关代码<br/>
    /// 该类的内部字段、属性、方法不提供注释
    /// </para>
    /// </summary>
    public static class Load
    {
        private static bool state = false;
        private static Assembly assembly = null;
        private static Type type_hash = null;
        private static object obj_hash = null;
        private static MethodInfo func_SHA512HASH = null;
        private static Type type_main = null;
        private static object obj_main = null;
        private static MethodInfo func_CheckVersion = null;
        private static MethodInfo func_Set = null;
        private static Type type_code = null;
        private static MethodInfo func_GetCoreMap = null;
        private static MethodInfo func_SetProcess = null;
        public static void Cache(string DLL)
        {
            if (!state)
            {
                try
                {
                    assembly = Assembly.LoadFile(DLL);
                    type_hash = assembly.GetType("EfficiencyMode.Hash");
                    obj_hash = Activator.CreateInstance(type_hash);
                    func_SHA512HASH = type_hash.GetMethod("SHA512HASH");
                    type_main = assembly.GetType("EfficiencyMode.Main");
                    obj_main = Activator.CreateInstance(type_main);
                    func_CheckVersion = type_main.GetMethod("CheckVersion");
                    func_Set = type_main.GetMethod("Set");
                    type_code = assembly.GetType("EfficiencyMode.Core");
                    func_GetCoreMap = type_code.GetMethod("GetCoreMap", BindingFlags.Static | BindingFlags.Public);
                    func_SetProcess = type_code.GetMethod("SetProcess", BindingFlags.Static | BindingFlags.Public);
                    state = true;
                }
                catch (Exception)
                {
                    Clear();
                    throw;
                }
            }
        }
        public static void Clear()
        {
            state = false;
            assembly = null;
            type_hash = null;
            obj_hash = null;
            func_SHA512HASH = null;
            type_main = null;
            obj_main = null;
            func_CheckVersion = null;
            func_Set = null;
            type_code = null;
            func_GetCoreMap = null;
            func_SetProcess = null;
        }
        public static byte[] SHA512HASH(string file)
        {
            if (state)
            {
                return (byte[])func_SHA512HASH.Invoke(obj_hash, new object[1] { file });
            }
            return new byte[0];
        }
        public static bool CheckVersion()
        {
            if (state)
            {
                return (bool)func_CheckVersion.Invoke(obj_main, null);
            }
            return false;
        }
        public static bool Set(Process process)
        {
            if (state)
            {
                return (bool)func_Set.Invoke(obj_main, new object[1] { process });
            }
            return false;
        }
        public static bool CoreType = false;
        public static bool GetCoreMap()
        {
            if (state)
            {
                return (bool)func_GetCoreMap.Invoke(null, null);
            }
            return false;
        }
        public static bool SetProcess(int PID, bool PoE)
        {
            if (state)
            {
                return (bool)func_SetProcess.Invoke(null, new object[2]
                    {
                        PID, 
                        PoE
                    });
            }
            return false;
        }
        public static bool SetProcess_Pro(Process Proc, ProcessPriorityClass Priority, bool PoE)
        {
            Proc.PriorityClass = Priority;
            return SetProcess(Proc.Id, PoE);
        }
    }
}
