using BusinessLogics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;


namespace CodeIN
{

    public class CodeOperation
    {
        public static void Main(string[] args)
        { }
        public static bool GetDataFromCO(out int remDays)
        {
            var pLData = new BLGlobalSetting().GetMobileAppVersionByKey("product_license_web");

            string encdDtStr = MiscHelper.DecryptData(pLData.value);
            if (string.IsNullOrEmpty(encdDtStr))
            {
                remDays = 0;
                return true;
            }
            TimeSpan difference;
            bool isWithinThreshold = true;
            CodeOperation.ValidateCO(encdDtStr, out difference);

            double thresholdDays = pLData is null ? 0 : pLData.max_value;

            if (difference.Days > thresholdDays)
                isWithinThreshold = false;

            remDays = difference.Days;
            return isWithinThreshold;
        }
        public static string GetProductExpiryDate(string data)
        {
            //data = "SI|3.0|SEFWRUZVTg==336435FDTcefqFuWrReQ2V3mS3ew==356433SEFWRUZVTg==3637940a6C6ZXxOULB42KAKxHOdQ==943736SEFWRUZVTg==384240OAmv5XxJtysspAGckbW7vg==404238SEFWRUZVTg==|VI";

            string extractedData1 = DecodeData(SpecialCharacterSet.SpChar1, SpecialCharacterSet.SpChar2, data);
            string extractedData2 = DecodeData(SpecialCharacterSet.SpChar3, SpecialCharacterSet.SpChar4, data);
            string extractedData3 = DecodeData(SpecialCharacterSet.SpChar5, SpecialCharacterSet.SpChar6, data);

            string decryptedData = MiscHelper.Decrypt(extractedData2);
            string reversedData = new string(decryptedData.Reverse().ToArray()); // Reverse the string properly
            string expDate = ConvertToNumeric(MiscHelper.Decrypt(extractedData3)) + ConvertToNumeric(MiscHelper.Decrypt(extractedData1)) + ConvertToNumeric(reversedData);
            return expDate;
        }
        static string DecodeData(SpecialCharacterSet stCharSet, SpecialCharacterSet edCharSet, string data)
        {
            string stIndexData = GetASCIIEncodedData(GetSpecialCharacterSet(stCharSet));
            string edIndexData = GetASCIIEncodedData(GetSpecialCharacterSet(edCharSet));
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
        public static void ValidateCO(string givenEncdExpDate, out TimeSpan difference)
        {
            string dt = CodeOperation.GetProductExpiryDate(givenEncdExpDate);
            // Parse the given date
            DateTime givenEDate = DateTime.ParseExact(dt, "ddMMyyyy", null);           //line to be deleted
            difference = givenEDate.Date - DateTime.Now.Date;
            string rsaKeysPath = string.Empty;
            if (difference.Days <= 0 && MiscHelper.GetLKeyPath(out rsaKeysPath))
            {
                // Delete the file
                File.Delete(rsaKeysPath);
            }
        }
    }

}