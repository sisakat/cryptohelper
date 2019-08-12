using Sisak.CryptoHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Sisak.CryptoHelper
{
    /// <summary>
    /// AES Cryptography helper class
    /// 
    /// Parts taken from "C# AES 256 bits Encryption Library with Salt"
    /// by adriancs from CodeProject
    /// (Further information see:
    ///     https://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt
    /// )
    /// </summary>
    public class AESCryptography : ICryptographicHelper
    {
        /// <summary>
        /// Salt
        /// </summary>
        public byte[] Salt { get; set; } = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        /// <summary>
        /// Key Size
        /// </summary>
        public int KeySize { get; set; } = 256;

        /// <summary>
        /// Key Block-Size
        /// </summary>
        public int BlockSize { get; set; } = 128;

        /// <summary>
        /// Key Iterations
        /// </summary>
        public int Iterations { get; set; } = 1000;

        /// <summary>
        /// Creates a random salt with given length. Can be used to set the Salt property.
        /// </summary>
        /// <param name="length">Length of the resulting salt</param>
        /// <returns>Salt</returns>
        public byte[] CreateRandomSalt(int length)
        {
            Random rand = new Random();
            byte[] salt = new byte[length];
            rand.NextBytes(salt);
            return salt;
        }

        /// <summary>
        /// Decrypts given bytes using given password and the Salt property.
        /// </summary>
        /// <param name="bytesToDecrypt"></param>
        /// <param name="passwordBytes"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] bytesToDecrypt, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            byte[] saltBytes = Salt;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, Iterations);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToDecrypt, 0, bytesToDecrypt.Length);
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        /// <summary>
        /// Decrypts given string using given password and Salt property.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string DecryptText(string input, string password)
        {
            byte[] bytesToDecrypt = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = Decrypt(bytesToDecrypt, passwordBytes);
            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

        /// <summary>
        /// Encrypts given bytes using given password and Salt property.
        /// </summary>
        /// <param name="bytesToEncrypt"></param>
        /// <param name="passwordBytes"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] bytesToEncrypt, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = Salt;

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, Iterations);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        /// <summary>
        /// Encrypts given string with password and Salt property.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string EncryptText(string input, string password)
        {
            byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = Encrypt(bytesToEncrypt, passwordBytes);
            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }
    }
}
