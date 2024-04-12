using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BookShop.Data
{


    public static class EncryptionService
    {
        private static readonly byte[] key = Encoding.UTF8.GetBytes("1234567890123456"); 
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("1234567890123456"); 


        public static string EncryptString(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                aes.Key = key;

                var iv = aes.IV;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    var encrypted = ms.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }

        public static string DecryptString(string cipherText)
        {
            var buffer = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(buffer))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }

}
