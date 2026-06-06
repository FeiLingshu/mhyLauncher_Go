using System;
using System.IO;
using System.Text;
using BuildinMD5 = System.Security.Cryptography.MD5;

namespace MHYLAUNCHER_GO.Functions.IO
{
    /// <summary>
    /// 用于实现计算文件MD5校验值的静态类
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// 计算指定文件的MD5校验值
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <returns>返回指定文件的MD5校验值</returns>
        public static string Get(string filepath)
        {
            using (BuildinMD5 md5 = BuildinMD5.Create())
            {
                using (FileStream fileStream = new FileStream(
                    filepath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    options: FileOptions.SequentialScan))
                {
                    byte[] hash = md5.ComputeHash(fileStream);
                    StringBuilder sb = new StringBuilder(hash.Length * 2);
                    foreach (byte b in hash)
                    {
                        sb.Append(b.ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
        }

        /// <summary>
        /// 附加方法(仅当校验不通过时调用)：覆盖资源文件
        /// </summary>
        /// <param name="path">目标文件路径</param>
        [Obsolete("程序已不再内置特定资源文件，该方法不再提供使用", true)]
        public static void Set(string path)
        {
            byte[] videofile = null;
            //byte[] videofile = MHYLAUNCHER_GO.Properties.Resources.video;
            using (FileStream filestream = File.Create(path))
            {
                filestream.Position = 0;
                filestream.Write(videofile, 0, videofile.Length);
                filestream.Close();
            }
        }
    }
}
