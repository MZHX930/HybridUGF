using System.IO;
using System.Security.Cryptography;
using System;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 配置表加密解密工具
    /// </summary>
    public static class DataTableEncryptUtility
    {
        private static byte[] Key = new byte[32] { 20, 112, 118, 231, 185, 252, 25, 22, 10, 214, 124, 12, 160, 84, 221, 225, 114, 92, 65, 32, 121, 7, 53, 234, 17, 175, 164, 189, 153, 170, 174, 110 };// 32 bytes for AES-256
        private static byte[] IV = new byte[16] { 88, 22, 139, 249, 198, 81, 60, 252, 23, 191, 76, 129, 81, 79, 184, 27 }; // 16 bytes
        public static string Encrypt(string plainText)
        {
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}