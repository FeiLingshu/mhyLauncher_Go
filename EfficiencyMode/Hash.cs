using System.IO;
using System.Security.Cryptography;

namespace EfficiencyMode
{
    /// <summary>
    /// SHA512校验和计算类
    /// </summary>
    public class Hash
    {
        /// <summary>
        /// 获取文件的SHA512校验和
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns>返回文件的SHA512校验和（类型：byte[]）</returns>
        public byte[] SHA512HASH(string file)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    byte[] hashBytes = sha512Hash.ComputeHash(fileStream);
                    return hashBytes;
                }
            }
        }
    }
}
