using System;
using System.IO;
using System.Security.Cryptography;

namespace EfficiencyMode
{
    public class Hash
    {
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
