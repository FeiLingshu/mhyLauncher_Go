using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace mhyLauncher_Go
{
    /// <summary>
    /// 实现INI配置文件互操作的帮助类
    /// </summary>
    internal static class INI
    {
        /// <summary>
        /// 实现读取INI配置文件的WindowsAPI
        /// </summary>
        /// <param name="section">要检索的节</param>
        /// <param name="key">要检索的键</param>
        /// <param name="defaultValue">检索结果的默认值</param>
        /// <param name="retVal">用于承载返回结果的变量</param>
        /// <param name="size">用于承载返回结果的变量的最大容量</param>
        /// <param name="filePath">INI文件路径</param>
        /// <returns>返回检索结果的字节大小</returns>
        [DllImport("kernel32.dll")]
        private static extern long GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 实现写入INI配置文件的WindowsAPI
        /// </summary>
        /// <param name="section">要检索的节</param>
        /// <param name="key">要检索的键</param>
        /// <param name="value">要写入指定位置的值</param>
        /// <param name="filePath">INI文件路径</param>
        /// <returns>指示操作是否成功，非零表示成功，零表示失败</returns>
        [DllImport("kernel32.dll")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        /// <summary>
        /// 指示目标#1是否需要跳过检查
        /// </summary>
        internal static bool Skip_YuanShen = false;

        /// <summary>
        /// 指示目标#2是否需要跳过检查
        /// </summary>
        internal static bool Skip_StarRail = false;

        /// <summary>
        /// 目标#1的路径信息
        /// </summary>
        internal static string YuanShenPath = string.Empty;

        /// <summary>
        /// 目标#2的路径信息
        /// </summary>
        internal static string StarRailPath = string.Empty;

        /// <summary>
        /// 自动检查INI文件中所有目标的值是否符合规则
        /// </summary>
        internal static void Check()
        {
            if (!File.Exists("./mhyLauncher_Go.ini")) // 判断INI文件是否存在
            {
                Creat();
            }
            else
            {
                StringBuilder s = new StringBuilder(short.MaxValue);
                GetPrivateProfileString("GamePaths", "YuanShen", "null", s, s.MaxCapacity, "./mhyLauncher_Go.ini");
                string path1 = s.ToString();
                s.Clear();
                GetPrivateProfileString("GamePaths", "StarRail", "null", s, s.MaxCapacity, "./mhyLauncher_Go.ini");
                string path2 = s.ToString();
                if (path1 == "null" || path2 == "null") // 判断INI文件结构#1是否完整
                {
                    YuanShenPath = string.Empty;
                    StarRailPath = string.Empty;
                    File.Delete("./mhyLauncher_Go.ini");
                    Creat();
                    new Input().ShowDialog();
                    return;
                }
                StringBuilder b = new StringBuilder(short.MaxValue);
                GetPrivateProfileString("Skip", "YuanShen", "null", b, b.MaxCapacity, "./mhyLauncher_Go.ini");
                string bool1 = b.ToString();
                b.Clear();
                GetPrivateProfileString("Skip", "StarRail", "null", b, b.MaxCapacity, "./mhyLauncher_Go.ini");
                string bool2 = b.ToString();
                bool convert1 = bool.TryParse(bool1, out bool bool1bool);
                bool convert2 = bool.TryParse(bool2, out bool bool2bool);
                if (bool1 == "null" || bool2 == "null" || !convert1 || !convert2) // 判断INI文件结构#2是否完整
                {
                    Skip_YuanShen = false;
                    Skip_StarRail = false;
                    File.Delete("./mhyLauncher_Go.ini");
                    Creat();
                    new Input().ShowDialog();
                    return;
                }
                else
                {
                    if (bool1bool) Skip_YuanShen = true;
                    if (bool2bool) Skip_StarRail = true;
                }
                if (Skip_YuanShen || !(path1.EndsWith("YuanShen.exe") && File.Exists(path1))) // 读取目标#1的值
                {
                    YuanShenPath = string.Empty;
                    Write("GamePaths", "YuanShen", string.Empty);
                }
                else
                {
                    YuanShenPath = path1;
                }
                if (Skip_StarRail || !(path2.EndsWith("StarRail.exe") && File.Exists(path2))) // 读取目标#2的值
                {
                    StarRailPath = string.Empty;
                    Write("GamePaths", "StarRail", string.Empty);
                }
                else
                {
                    StarRailPath = path2;
                }
            }
            if ((!Skip_YuanShen && string.IsNullOrEmpty(YuanShenPath)) || (!Skip_StarRail && string.IsNullOrEmpty(StarRailPath))) // 判断目标的值是否符合规则
            {
                new Input().ShowDialog();
            }
        }

        /// <summary>
        /// 根据需要自动生成全新的INI配置文件
        /// </summary>
        private static void Creat()
        {
            FileStream fs = File.Create("./mhyLauncher_Go.ini");
            fs.Close();
            fs.Dispose();
            Write("GamePaths", "YuanShen", string.Empty);
            Write("GamePaths", "StarRail", string.Empty);
            Write("Skip", "YuanShen", "false");
            Write("Skip", "StarRail", "false");
        }

        /// <summary>
        /// 实现向INI文件写入数据的简易接口
        /// <para>接口函数未做任何额外适配处理，仅包含写入数据的最基础功能</para>
        /// </summary>
        /// <param name="section">目标节</param>
        /// <param name="key">目标键</param>
        /// <param name="value">目标值</param>
        internal static void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, "./mhyLauncher_Go.ini");
        }
    }
}
