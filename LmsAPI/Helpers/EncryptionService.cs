using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LMSAPI.Helpers
{
    public static class EncryptionHelper
    {
        //private static readonly string Key = "12345678901234567890123456789012"; // 32 bytes = AES-256
        //private static readonly string IV = "1234567890123456";                   // 16 bytes = AES block size

        private static readonly string Key = "InfoplusInfoplusInfoplusInfoplus"; // 32 bytes = AES-256
        private static readonly string IV = "InfoplusInfoplus";                   // 16 bytes = AES block size
        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }


}
