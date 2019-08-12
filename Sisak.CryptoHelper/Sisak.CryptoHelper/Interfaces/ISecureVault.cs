using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sisak.CryptoHelper.Interfaces
{
    public interface ISecureVault<T>
    {
        string Location { get; set; }
        string FileName { get; set; }
        ICryptographicHelper CryptoHelper { get; set; }

        T GetObject(string name, string password);
        void AddObject(string name, T obj, string password);
        void RemoveObject(string name);
        void RemoveAllObjects();
        void Save();
        void Load();
        void Refresh();
    }
}
