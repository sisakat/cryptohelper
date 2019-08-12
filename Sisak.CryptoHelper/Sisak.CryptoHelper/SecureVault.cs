using Sisak.CryptoHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sisak.CryptoHelper
{
    public class SecureVault : ISecureVault<String>
    {
        public string Location { get; set; }

        /// <summary>
        /// FileName used for saving the Vault to disk.
        /// </summary>
        public string FileName { get; set; }

        public string FullFileName
        {
            get
            {
                return System.IO.Path.Combine(Location, FileName);
            }
        }

        /// <summary>
        /// Encryption to use
        /// </summary>
        public ICryptographicHelper CryptoHelper { get; set; }

        private Dictionary<String, String> _vaultObjects;

        /// <summary>
        /// Creates new secure Vault using given objects.
        /// </summary>
        /// <param name="vaultObjects">Objects that can be represented in JSON format.</param>
        private SecureVault(Dictionary<String, String> vaultObjects)
        {
            _vaultObjects = vaultObjects ?? new Dictionary<String, String>();
        }

        public SecureVault() : this(null) { }

        /// <summary>
        /// Clears the vault
        /// </summary>
        public void RemoveAllObjects()
        {
            _vaultObjects.Clear();
        }

        private void ValidateFileName()
        {
            if (FileName == null || Location == null)
            {
                throw new ArgumentException("FileName and/or Location was null");
            }
        }

        public String GetVaultAsJson()
        {
            var obj = new SecureVaultObject<String>
            {
                Created = DateTime.Now,
                Objects = _vaultObjects
            };

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            return jsonString;
        }

        /// <summary>
        /// Saves the vault to disk using given FileName.
        /// </summary>
        public void Save()
        {
            ValidateFileName();

            if (!System.IO.Directory.Exists(Location))
                System.IO.Directory.CreateDirectory(Location);
            if (System.IO.File.Exists(FullFileName))
                System.IO.File.Delete(FullFileName);

            System.IO.File.WriteAllText(FullFileName, GetVaultAsJson());
        }

        /// <summary>
        /// Loads the vault from disk using given FileName;
        /// </summary>
        public void Load()
        {
            ValidateFileName();

            if (!System.IO.File.Exists(FullFileName))
                throw new ArgumentException($"File '{FullFileName}' not found");

            string contents = System.IO.File.ReadAllText(FullFileName);
            SecureVaultObject<String> svo = 
                Newtonsoft.Json.JsonConvert.DeserializeObject<SecureVaultObject<String>>(contents);
            _vaultObjects = svo.Objects;
        }

        public void Refresh()
        {
            this.RemoveAllObjects();
            this.Load();
        }

        public String GetObject(string name, string password)
        {
            if (!_vaultObjects.ContainsKey(name))
            {
                return default(String);
            }

            string encryptedContent = _vaultObjects[name];
            string decryptedContent = CryptoHelper.DecryptText(encryptedContent, password);
            return decryptedContent;
        }

        public void AddObject(string name, String obj, string password)
        {
            if (_vaultObjects.ContainsKey(name))
            {
                throw new ArgumentException("Key already exists");
            }

            string encryptedContent = CryptoHelper.EncryptText(obj, password);
            _vaultObjects.Add(name, encryptedContent);
        }

        public void RemoveObject(string name)
        {
            if (_vaultObjects.ContainsKey(name))
            {
                _vaultObjects.Remove(name);
            }
        }
    }
}
