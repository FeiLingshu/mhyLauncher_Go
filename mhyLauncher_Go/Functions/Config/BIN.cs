using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MHYLAUNCHER_GO.Functions.Config
{
    /// <summary>
    /// 实现保存配置文件的帮助类
    /// </summary>
    public static class BIN
    {
        /// <summary>
        /// 程序运行模式枚举
        /// </summary>
        public enum MODE : byte
        {
            PHONE = 0,
            PC = 1,
            CUSTOM = 2
        }

        /// <summary>
        /// 保存配置参数的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DATA
        {
            /// <summary>
            /// 程序运行模式
            /// </summary>
            public MODE MODE;
            /// <summary>
            /// 配置参数1
            /// </summary>
            public int SET_1;
            /// <summary>
            /// 配置参数2
            /// </summary>
            public int SET_2;
            /// <summary>
            /// 路径数据的字节长度
            /// </summary>
            public int PLENGTH;
            /// <summary>
            /// 路径数据的UTF8编码字节
            /// </summary>
            public byte[] PATH;

            /// <summary>
            /// 初始化配置实例
            /// </summary>
            /// <param name="MODE">运行模式</param>
            /// <param name="SET">配置参数</param>
            /// <param name="PATH">程序路径</param>
            public DATA(MODE MODE, (int, int) SET, string[] PATH)
            {
                this.MODE = MODE;
                this.SET_1 = SET.Item1;
                this.SET_2 = SET.Item2;
                this.PATH = new UTF8Encoding(false).GetBytes(string.Join("\n", PATH));
                this.PLENGTH = this.PATH.Length;
            }

            /// <summary>
            /// 编码配置数据
            /// </summary>
            /// <returns>返回配置数据的字节码</returns>
            public byte[] Serialize()
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms, new UTF8Encoding(false)))
                    {
                        bw.Write((byte)MODE);
                        bw.Write(SET_1);
                        bw.Write(SET_2);
                        bw.Write(PLENGTH);
                        bw.Write(PATH);
                        return ms.ToArray();
                    }
                }
            }

            /// <summary>
            /// 解码配置数据
            /// </summary>
            /// <param name="data">配置数据的字节码</param>
            /// <returns>返回原始配置数据实例</returns>
            public static DATA Deserialize(byte[] data)
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (BinaryReader br = new BinaryReader(ms, new UTF8Encoding(false)))
                    {
                        DATA result = new DATA
                        {
                            MODE = (MODE)br.ReadByte(),
                            SET_1 = br.ReadInt32(),
                            SET_2 = br.ReadInt32(),
                            PLENGTH = br.ReadInt32()
                        };
                        result.PATH = br.ReadBytes(result.PLENGTH);
                        return result;
                    }
                }
            }

            /// <summary>
            /// 初始化默认配置信息
            /// </summary>
            /// <returns>返回默认配置信息实例</returns>
            public static DATA Empty()
            {
                return new DATA(MODE.PHONE, (640, 480), new string[0]);
            }

            /// <summary>
            /// 重写==运算符
            /// </summary>
            /// <param name="left">结构体实例1</param>
            /// <param name="right">结构体实例2</param>
            /// <returns>返回两个结构体是否相等</returns>
            public static bool operator ==(DATA left, DATA right)
            {
                return left.MODE == right.MODE
                    && left.SET_1 == right.SET_1
                    && left.SET_2 == right.SET_2
                    && left.PLENGTH == right.PLENGTH
                    && left.PATH == right.PATH;
            }

            /// <summary>
            /// 重写!=运算符
            /// </summary>
            /// <param name="left">结构体实例1</param>
            /// <param name="right">结构体实例2</param>
            /// <returns>返回两个结构体是否不等</returns>
            public static bool operator !=(DATA left, DATA right)
            {
                return !(left == right);
            }

            /// <summary>
            /// 重写Equals对象方法
            /// </summary>
            /// <param name="obj">比较源</param>
            /// <returns>返回两个对象是否相同</returns>
            public override bool Equals(object obj)
            {
                return obj is DATA other && Equals(other);
            }

            /// <summary>
            /// 实现IEquatable<T>接口
            /// </summary>
            /// <param name="other">比较对象</param>
            /// <returns>返回两个对象是否相等</returns>
            public bool Equals(DATA other)
            {
                return MODE == other.MODE
                    && SET_1 == other.SET_1
                    && SET_2 == other.SET_2
                    && PLENGTH == other.PLENGTH
                    && PATH == other.PATH;
            }

            /// <summary>
            /// 重写GetHashCode方法
            /// </summary>
            /// <returns>返回对象（在内置编码器编码后）的字节数组的哈希值</returns>
            public override int GetHashCode()
            {
                return this.Serialize().GetHashCode();
            }
        }

        /// <summary>
        /// 用于存储配置文件路径的全局字段
        /// </summary>
        public static string FILE = string.Empty;

        /// <summary>
        /// 用于存储配置信息实例的全局字段
        /// </summary>
        public static DATA BINDATA = DATA.Empty();

        /// <summary>
        /// 用于缓存路径信息的全局字段
        /// </summary>
        public static string[] PATHS = null;

        /// <summary>
        /// 检查配置文件合法性
        /// </summary>
        /// <returns>返回检查结果</returns>
        public static bool Check()
        {
            using (Process CurrentProcess = Process.GetCurrentProcess())
            {
                FILE =
                    $"{Path.GetDirectoryName(CurrentProcess.MainModule.FileName)}\\" +
                    $"{Path.GetFileNameWithoutExtension(CurrentProcess.MainModule.FileName)}.bin";
            }
            if (File.Exists(FILE))
            {
                DATA data;
                List<string> paths;
                try
                {
                    data = Read();
                    paths = new UTF8Encoding(false).GetString(data.PATH).Split('\n').ToList();
                }
                catch (Exception)
                {
                    Write(DATA.Empty());
                    return false;
                }
                for (int i = 0; i < paths.Count; i++)
                {
                    if (!File.Exists(paths[i]))
                    {
                        paths.RemoveAt(i);
                        i--;
                    }
                }
                if (paths.Count > 0)
                {
                    return true;
                }
                else
                {
                    Write(DATA.Empty());
                    return false;
                }
            }
            else
            {
                // File.Create(FILE);
                Write(DATA.Empty());
                return false;
            }
        }

        /// <summary>
        /// 读取配置信息中的路径部分
        /// </summary>
        /// <returns>返回是否成功获取配置信息</returns>
        public static bool GetPaths()
        {
            DATA data;
            List<string> paths;
            try
            {
                data = Read();
                paths = new UTF8Encoding(false).GetString(data.PATH).Split('\n').ToList();
            }
            catch (Exception)
            {
                Write(DATA.Empty());
                PATHS = null;
                return false;
            }
            for (int i = 0; i < paths.Count; i++)
            {
                if (!File.Exists(paths[i]))
                {
                    paths.RemoveAt(i);
                    i--;
                }
            }
            if (paths.Count > 0)
            {
                PATHS = paths.ToArray();
                return true;
            }
            else
            {
                Write(DATA.Empty());
                PATHS = null;
                return false;
            }
        }

        /// <summary>
        /// 将相关信息写入配置信息
        /// </summary>
        /// <param name="mode">运行模式</param>
        /// <param name="size">配置参数</param>
        /// <param name="paths">程序路径</param>
        /// <returns>返回写入是否成功</returns>
        public static bool SetPaths(MODE mode, (int, int) size ,List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                if (!File.Exists(paths[i]))
                {
                    paths.RemoveAt(i);
                    i--;
                }
            }
            if (paths.Count > 0)
            {
                Write(new DATA(mode, size, paths.ToArray()));
                return true;
            }
            else
            {
                Write(DATA.Empty());
                return false;
            }
        }

        /// <summary>
        /// 用于从文件读取配置信息的内部方法
        /// </summary>
        /// <returns>返回配置信息实例</returns>
        private static DATA Read()
        {
            if (string.IsNullOrEmpty(FILE))
            {
                return DATA.Empty();
            }
            BINDATA = DATA.Deserialize(File.ReadAllBytes(FILE));
            return BINDATA;
        }

        /// <summary>
        /// 用于向文件写入配置信息的内部方法
        /// </summary>
        /// <param name="data">要写入的配置信息实例</param>
        private static void Write(DATA data)
        {
            if (string.IsNullOrEmpty(FILE))
            {
                return;
            }
            if (BINDATA != data)
            {
                BINDATA = data;
                File.WriteAllBytes(FILE, data.Serialize());
            }
        }
    }
}
