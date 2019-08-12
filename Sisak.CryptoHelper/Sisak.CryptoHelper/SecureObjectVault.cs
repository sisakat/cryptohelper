using Sisak.CryptoHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sisak.CryptoHelper
{
    public class SecureObjectVault<T> : SecureVault, ISecureVault<T>
    {
        public SecureObjectVault() : base() { }

        public void AddObject(string name, T obj, string password)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            base.AddObject(name, json, password);
        }

        T ISecureVault<T>.GetObject(string name, string password)
        {
            string json = base.GetObject(name, password);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
