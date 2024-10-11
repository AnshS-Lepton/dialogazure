using RSAImplementation.Utility;
using System.Security.Cryptography;
using System.Text;

namespace RSAImplementation;

public static class RSAOperation
{
    public static void GenerateRSAKeys()
    {
        using (RSA rsa = RSA.Create(2048))
        {
            // Export and store public key
            var publicKey = rsa.ExportSubjectPublicKeyInfo();
            SaveEncryptedFile("_publicKey.pem", publicKey);

            // Export and encrypt private key
            var privateKey = rsa.ExportRSAPrivateKey();
            string encryptionKey = RSAConstants.RSAEncryptionKey;

            var encryptedPrivateKey = EncryptPrivateKey(privateKey, encryptionKey);
            SaveEncryptedFile("_privateKey.enc", encryptedPrivateKey);
        }
    }

    static byte[] EncryptPrivateKey(byte[] privateKey, string encryptionKey)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(encryptionKey); // Example key, use a secure key management solution
            aes.IV = new byte[16]; //aes.IV Gets or sets the initialization vector (SymmetricAlgorithm.IV) for the symmetric algorithm.
            using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                return PerformCryptography(privateKey, encryptor);
            }
        }
    }

    public static byte[] DecryptPrivateKey(byte[] encryptedPrivateKey, string encryptionKey)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(encryptionKey);
            aes.IV = new byte[16]; // Should use the IV that was used during encryption
            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                return PerformCryptography(encryptedPrivateKey, decryptor);
            }
        }
    }
    static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }
    public static string GetPropertyValue<T>(string fieldName)
    {
        var propertyInfo = typeof(RSAConstants).GetField(fieldName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(null)?.ToString();
        }

        return null;
    }

    public static void SaveEncryptedFile(string fileName, byte[] encryptedData)
    {
        // Get the project directory path
        string currentDirectory = GetProjectDirectory();

        // Create RSAKeys and RSAKeysBackup directories if they don't exist
        string rsaKeysDirectory = Path.Combine(currentDirectory, "RSAKeys");
        
        if (!Directory.Exists(rsaKeysDirectory))
        {
            Directory.CreateDirectory(rsaKeysDirectory);
        }

        string backupDirectory = Path.Combine(currentDirectory, "RSAKeysBackup");
        if (!Directory.Exists(backupDirectory))
        {
            Directory.CreateDirectory(backupDirectory);
        }

        // Path to the file in RSAKeys directory
        string filePath = Path.Combine(rsaKeysDirectory, fileName);

        // Check if the file already exists in the RSAKeys directory
        //if (File.Exists(filePath))
        if (false)          //After discussion stopped backup of old rsa keys
        {
            // Create a backup filename with the current date and time
            string backupFileName = Path.GetFileNameWithoutExtension(filePath) + "_" +
                                    DateTime.Now.ToString("yyyyMMdd_HHmmss") +
                                    Path.GetExtension(filePath);

            // Move the existing file to the RSAKeysBackup directory
            string backupFilePath = Path.Combine(backupDirectory, backupFileName);
            File.Move(filePath, backupFilePath);
        }

        // Save the new encrypted file in the RSAKeys directory
        File.WriteAllBytes(filePath, encryptedData);
    }

    public static string GetProjectDirectory()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        
        // Traverse upwards until we find the .csproj file
        //while (!Directory.GetFiles(currentDirectory, "*.csproj").Any())
        //{
        //    DirectoryInfo parentDirectory = Directory.GetParent(currentDirectory);
        //    if (parentDirectory == null)
        //    {
        //        throw new Exception("Could not locate the project directory.");
        //    }

        //    currentDirectory = parentDirectory.FullName;
        //}

        return currentDirectory;
    }
    public static string Encrypt(string clearText)
    {
        string EncryptionKey = RSAConstants.CryptoEncryptionKey;
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    public static string Decrypt(string cipherText)
    {
        string EncryptionKey = RSAConstants.CryptoEncryptionKey;
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }
    public static string GetSpecialCharacterSet(SpecialCharacterSet set)
    {
        switch (set)
        {
            case SpecialCharacterSet.SpChar1:
                return "!@#";
            case SpecialCharacterSet.SpChar2:
                return "(*&";
            case SpecialCharacterSet.SpChar3:
                return "$%^";
            case SpecialCharacterSet.SpChar4:
                return "&*(";
            case SpecialCharacterSet.SpChar5:
                return "^%$";
            case SpecialCharacterSet.SpChar6:
                return "#@!";
            default:
                return string.Empty;
        }
    }
}
