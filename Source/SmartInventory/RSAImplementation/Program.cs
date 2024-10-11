using RSAImplementation;
using RSAImplementation.Utility;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;


class Program
{
    static void Main()
    {
        string expiryDate = string.Empty;

        // Loop until a valid date is entered
        do
        {
            Console.WriteLine("Please enter the expiry date of the license in the DDMMYYYY format: ");
            expiryDate = Console.ReadLine();

            if (!ValidateDateString(expiryDate))
            {
                Console.WriteLine($"Entered date {expiryDate} is not valid.");
            }

        } while (!ValidateDateString(expiryDate));
        string encodedExpDate = EncodedLicenseData(expiryDate);

        //// Generate RSA keys using the provided encryption key
        Console.WriteLine("Do you want to generate a new pair of public and private keys? (yes/no) default is (yes):");
        string response = Console.ReadLine()?.Trim().ToLower() == "" ? "yes" : "no";
        
        if (response == "yes")
        {
            RSAOperation.GenerateRSAKeys();
            Console.WriteLine("RSA keys generated successfully.");
        }
        else
        {
            Console.WriteLine("RSA key generation skipped.");
        }

        string projectDirectory = RSAOperation.GetProjectDirectory();
        string rsaPublicKeysPath = Path.Combine(projectDirectory, "RSAKeys", "_publicKey.pem");
        string rsaPrivateKeysPath = Path.Combine(projectDirectory, "RSAKeys", "_privateKey.enc");
        if (File.Exists(rsaPublicKeysPath) && File.Exists(rsaPrivateKeysPath))
        {
            string encryptedData = EncryptData(encodedExpDate);

            string decryptedData = DecryptData();

            string extractedData = GetProductExpiryDate(encodedExpDate);
        }
        else
        {
            Console.WriteLine("RSA key files not found. Please generate the keys first.");
        }
        
        Console.ReadKey();
    }

    static string EncryptData(string encodedExpDate)
    {
        // Load the public key
        string projectDirectory = RSAOperation.GetProjectDirectory();
        string rsaKeysPath = Path.Combine(projectDirectory, "RSAKeys", "_publicKey.pem");
        byte[] publicKeyBytes = File.ReadAllBytes(rsaKeysPath);
        byte[] encryptedKey;
        string base64str = string.Empty;
        Console.WriteLine("RSA keys and Encrypted License key generated and saved at: \n {0}", projectDirectory);
        using (RSA rsa = RSA.Create())
        {
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

            byte[] nameBytes = Encoding.UTF8.GetBytes(encodedExpDate);
            encryptedKey = rsa.Encrypt(nameBytes, RSAEncryptionPadding.OaepSHA1);

            // Save the encrypted name to a file
            base64str = Convert.ToBase64String(encryptedKey);
            //Console.WriteLine($"Encrypted Value: {base64str}");
            RSAOperation.SaveEncryptedFile("_rsaEncryptedKey.dat", encryptedKey);

            string txtFilePath = Path.Combine(projectDirectory, "RSAKeys", "_rsaBase64OfEncryptedKey.txt");
            File.WriteAllText(txtFilePath, base64str);
            // Later, convert the Base64 string back to a byte array
            //byte[] byteArrayFromBase64 = Convert.FromBase64String(base64str);

            //Verify the conversion by comparing the original and converted byte arrays
            //bool areEqual = encryptedKey.SequenceEqual(byteArrayFromBase64);
            //Console.WriteLine($"Are the original and converted byte arrays equal? {areEqual}");
        }
        return base64str;
    }
    static string DecryptData()
    {
        string projectDirectory = RSAOperation.GetProjectDirectory();
        string getDataFromFile = Path.Combine(projectDirectory, "RSAKeys", "_rsaBase64OfEncryptedKey.txt");
        string base64str = File.ReadAllText(getDataFromFile);
        byte[] byteArrayFromBase64 = Convert.FromBase64String(base64str);
        // Load and decrypt the private key
        string rsaKeysPath = Path.Combine(projectDirectory, "RSAKeys", "_privateKey.enc");
        byte[] encryptedPrivateKeyBytes = File.ReadAllBytes(rsaKeysPath);
        string encryptionKey = RSAConstants.RSAEncryptionKey; //"F77F0312D228C4019FDAB954AAAAAAAA"; 
        // Ensure the key is 32 bytes (for AES-256)
        //encryptionKey = encryptionKey.PadRight(32, '0'); // Padding to 32 characters (256 bits)
        byte[] privateKeyBytes = RSAOperation.DecryptPrivateKey(encryptedPrivateKeyBytes, encryptionKey);
        string decryptedName = string.Empty;
        using (RSA rsa = RSA.Create())
        {
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            // Load and decrypt the encrypted name
            //byte[] encryptedName = File.ReadAllBytes("encryptedName.dat");
            byte[] decryptedNameBytes = rsa.Decrypt(byteArrayFromBase64, RSAEncryptionPadding.OaepSHA1);
            decryptedName = Encoding.UTF8.GetString(decryptedNameBytes);

            //Console.WriteLine($"Decrypted Value: {decryptedName}");
        }
        return decryptedName;
    }

    static string EncodedLicenseData(string expiryDate)
    {
        StringBuilder stringBuilder = new StringBuilder();

        string encKeyConst = RSAConstants.EncryptionKeyConstant + "ABC";       //Get the encryption key constant 
        byte[] byteArray = Encoding.UTF8.GetBytes(encKeyConst);         //Convert string to byte array
        stringBuilder.Append(Convert.ToBase64String(byteArray));        //Append the encoded encryption key constant to string builder

        stringBuilder.Append(GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(SpecialCharacterSet.SpChar1)));                //append special characters to string builder

        GetDateComponents(expiryDate, DtEnum.M, ref stringBuilder);           //append encrypted Month to string builder

        stringBuilder.Append(GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(SpecialCharacterSet.SpChar2)));                  //append special characters to string builder

        encKeyConst = RSAConstants.EncryptionKeyConstant + "DEF";       //Get the encryption key constant 
        byteArray = Encoding.UTF8.GetBytes(encKeyConst);
        stringBuilder.Append(Convert.ToBase64String(byteArray));        //Append the encoded encryption key constant to string builder

        stringBuilder.Append(GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(SpecialCharacterSet.SpChar3)));                 //append special characters to string builder

        GetDateComponents(expiryDate, DtEnum.Y, ref stringBuilder);           //append encrypted Month to string builder

        stringBuilder.Append(GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(SpecialCharacterSet.SpChar4)));                 //append special characters to string builder

        encKeyConst = RSAConstants.EncryptionKeyConstant + "GHI";       //Get the encryption key constant 
        byteArray = Encoding.UTF8.GetBytes(encKeyConst);
        stringBuilder.Append(Convert.ToBase64String(byteArray));        //Append the encoded encryption key constant to string builder

        stringBuilder.Append(GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(SpecialCharacterSet.SpChar5)));                 //append special characters to string builder

        GetDateComponents(expiryDate, DtEnum.D, ref stringBuilder);           //append encrypted Day to string builder

        stringBuilder.Append(GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(SpecialCharacterSet.SpChar6)));                 //append special characters to string builder

        encKeyConst = RSAConstants.EncryptionKeyConstant + "JKL";       //Get the encryption key constant 
        byteArray = Encoding.UTF8.GetBytes(encKeyConst);
        stringBuilder.Append(Convert.ToBase64String(byteArray));        //Append the encoded encryption key constant to string builder

        return stringBuilder.ToString();
    }
    private static string GetASCIIEncodedData(string st)
    {
        StringBuilder stringBuilder = new StringBuilder();
        // Append ASCII values of each character in the st string
        foreach (char c in st)
        {
            int asciiValue = (int)c;                //Get ASCII value of character
            stringBuilder.Append(asciiValue);       //Append the ASCII value
        }
        return stringBuilder.ToString();
    }
    private static void GetDateComponents(string dateStr, DtEnum dtComponent, ref StringBuilder stringBuilder)
    {
        // Initialize strings for day, month, and year
        string day = dateStr.Substring(0, 2);   // "26"
        string month = dateStr.Substring(2, 2); // "11"
        string year = dateStr.Substring(4, 4);  // "2024"

        // Convert each character of the part of the date to the corresponding AlphaEnum value
        string convertedPart = string.Empty;
        switch (dtComponent)
        {
            case DtEnum.D: // Day
                convertedPart = ConvertToAlpha(day);
                break;
            case DtEnum.M: // Month
                convertedPart = ConvertToAlpha(month);
                break;
            case DtEnum.Y: // Year
                convertedPart = ConvertToAlpha(year);
                // Ensure non-null value, reverse and append the result to the StringBuilder
                convertedPart = new string(convertedPart.Reverse().ToArray()); // Reverse the string properly
                break;
        }
        string encString = RSAOperation.Encrypt(convertedPart);   //Encrypted converted to alphabet part of the date
        // Append the result to the StringBuilder
        stringBuilder.Append(encString);
    }
    private static string ConvertToAlpha(string numericPart)
    {
        if (string.IsNullOrEmpty(numericPart)) return string.Empty; // Ensure non-null return
        StringBuilder alphaBuilder = new StringBuilder();
        foreach (char digit in numericPart)
        {
            if (Enum.TryParse<AlphaEnum>(Enum.GetName(typeof(AlphaEnum), int.Parse(digit.ToString())), out AlphaEnum alphaEnumValue))
            {
                alphaBuilder.Append(alphaEnumValue.ToString());
            }
        }
        return alphaBuilder.ToString();
    }
    private static string ConvertToNumeric(string alphaPart)
    {
        if (string.IsNullOrEmpty(alphaPart)) return string.Empty; // Ensure non-null return
        StringBuilder numericBuilder = new StringBuilder();

        // Iterate through each character in the alphaPart
        foreach (char alphaChar in alphaPart)
        {
            // Try to parse the character to the AlphaEnum
            if (Enum.TryParse<AlphaEnum>(alphaChar.ToString(), out AlphaEnum alphaEnumValue))
            {
                // Get the numeric value of the enum and append it
                int numericValue = (int)alphaEnumValue;
                numericBuilder.Append(numericValue.ToString());
            }
        }

        return numericBuilder.ToString();
    }

    static string GetProductExpiryDate(string data)
    {
        //data = "SI|3.0|SEFWRUZVTg==336435FDTcefqFuWrReQ2V3mS3ew==356433SEFWRUZVTg==3637940a6C6ZXxOULB42KAKxHOdQ==943736SEFWRUZVTg==384240OAmv5XxJtysspAGckbW7vg==404238SEFWRUZVTg==|VI";

        string extractedData1 = DecodeData(SpecialCharacterSet.SpChar1, SpecialCharacterSet.SpChar2, data);
        string extractedData2 = DecodeData(SpecialCharacterSet.SpChar3, SpecialCharacterSet.SpChar4, data);
        string extractedData3 = DecodeData(SpecialCharacterSet.SpChar5, SpecialCharacterSet.SpChar6, data);

        string decryptedData = RSAOperation.Decrypt(extractedData2);            //This is required for Year Part only
        string reversedData = new string(decryptedData.Reverse().ToArray()); // Reverse the string properly
        string expDate = ConvertToNumeric(RSAOperation.Decrypt(extractedData3)) + ConvertToNumeric(RSAOperation.Decrypt(extractedData1)) + ConvertToNumeric(reversedData);
        return expDate;
    }
    static string DecodeData(SpecialCharacterSet stCharSet, SpecialCharacterSet edCharSet, string data)
    {
        string stIndexData = GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(stCharSet));
        string edIndexData = GetASCIIEncodedData(RSAOperation.GetSpecialCharacterSet(edCharSet));
        // Step 1: Find the index of the first substring "336435"
        int stIndex = data.IndexOf(stIndexData);

        // Step 2: Find the index of the second substring "356433"
        int edIndex = data.IndexOf(edIndexData);
        // Step 3: Extract the data between the two substrings
        if (stIndex != -1 && edIndex != -1 && edIndex > stIndex)
        {
            stIndex += stIndexData.ToString().Length;

            string result = data.Substring(stIndex, edIndex - stIndex);
            return result;
        }

        return string.Empty; // Return empty if the substrings are not found or invalid positions
    }
    public static bool ValidateDateString(string dateInput)
    {
        // Check if the input has exactly 8 characters
        if (string.IsNullOrEmpty(dateInput) || dateInput.Length != 8)
        {
            return false;
        }

        // Define the expected date format (ddMMyyyy)
        string dateFormat = "ddMMyyyy";

        // Try to parse the string to a DateTime object
        DateTime parsedDate;
        bool isValid = DateTime.TryParseExact(dateInput, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

        // Ensure the parsed date is a valid date in the future
        if (isValid && parsedDate > DateTime.Now)
        {
            return true;  // Date is valid
        }

        return false; // Invalid date or date not in the future
    }
}
