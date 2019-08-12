using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sisak.CryptoHelper.Interfaces
{
    public interface ICryptographicHelper
    {
        byte[] Encrypt(byte[] bytesToEncrypt, byte[] passwordBytes);
        byte[] Decrypt(byte[] bytesToDecrypt, byte[] passwordBytes);

        string EncryptText(string input, string password);
        string DecryptText(string input, string password);
    }
}
