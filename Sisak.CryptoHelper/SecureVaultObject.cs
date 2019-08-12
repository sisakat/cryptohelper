using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sisak.CryptoHelper
{
    public class SecureVaultObject<T>
    {
        public DateTime Created { get; set; }
        public Dictionary<String, T> Objects { get; set; }
    }
}
