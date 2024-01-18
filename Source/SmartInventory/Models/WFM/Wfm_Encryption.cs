using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Configuration;

namespace Models.WFM
{
    public class WfmEncryption
    {
        public static string m_strPassPhrase =Convert.ToString(ConfigurationManager.AppSettings["m_strPassPhrase"]);
        public static string iswfmencryption = Convert.ToString(ConfigurationManager.AppSettings["iswfmencryption"]);
        public static string Encrypt(string value)
        {
            int key = 0;
            try
            {
                //var globalSettings = new BLGlobalSetting().GetGlobalSettings("Mobile").Where(x => x.key == "iswfmencryption");
                //if (globalSettings != null)
                //{
                //key = Convert.ToInt32(globalSettings.FirstOrDefault().value);
                //}
                key = Convert.ToInt32(iswfmencryption);
                if (key > 0)
                {
                    //var m_strPassPhrase = "MAKV2SPBNI99212";
                    var p_strSaltValue = "XXXXXXXXXXXXXXXXX";
                    var m_strPasswordIterations = 2;
                    var m_strInitVector = "ZZZZZZZZZZZZZZZZ";
                    var plainText = value;
                    var blockSize = 32;
                    var saltValueBytes = Encoding.ASCII.GetBytes(p_strSaltValue);
                    var password = new Rfc2898DeriveBytes(m_strPassPhrase, saltValueBytes, m_strPasswordIterations);
                    var keyBytes = password.GetBytes(blockSize);

                    var symmetricKey = new RijndaelManaged();

                    var initVectorBytes = Encoding.ASCII.GetBytes(m_strInitVector);
                    var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                    var memoryStream = new System.IO.MemoryStream();
                    var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                    var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    var cipherTextBytes = memoryStream.ToArray();
                    memoryStream.Close();
                    cryptoStream.Close();

                    var cipherText = Convert.ToBase64String(cipherTextBytes);

                    return cipherText;

                }
                else
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string Decrypt(string value)
        {
            int key = 0;
            try
            {

                //var globalSettings = new BLGlobalSetting().GetGlobalSettings("Mobile").Where(x => x.key == "iswfmencryption");
                //if (globalSettings != null)
                //{
                //    key = Convert.ToInt32(globalSettings.FirstOrDefault().value);
                //}
                key = Convert.ToInt32(iswfmencryption);
                if (key > 0 && !string.IsNullOrEmpty(value))
                {
                    //var m_strPassPhrase = "MAKV2SPBNI99212";
                    var p_strSaltValue = "XXXXXXXXXXXXXXXXX";
                    var m_strPasswordIterations = 2;
                    var m_strInitVector = "ZZZZZZZZZZZZZZZZ";
                    var plainText = value;
                    var blockSize = 32;
                    var saltValueBytes = Encoding.ASCII.GetBytes(p_strSaltValue);
                    var password = new Rfc2898DeriveBytes(m_strPassPhrase, saltValueBytes, m_strPasswordIterations);
                    var keyBytes = password.GetBytes(blockSize);

                    var symmetricKey = new RijndaelManaged();

                    var initVectorBytes = Encoding.ASCII.GetBytes(m_strInitVector);
                    var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                    var memoryStream = new System.IO.MemoryStream();
                    var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

                    byte[] plainTextBytes = Convert.FromBase64String(plainText);

                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    var cipherTextBytes = memoryStream.ToArray();
                    memoryStream.Close();
                    cryptoStream.Close();

                    var cipherText = Encoding.UTF8.GetString(cipherTextBytes);
                    return cipherText;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }
   
}
