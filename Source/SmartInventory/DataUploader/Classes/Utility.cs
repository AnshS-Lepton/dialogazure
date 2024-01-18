using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace DataUploader
{
    public static class Utility
    {
        internal static Dictionary<string, string> GetBulkUploadColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    DbColName = p.Element("DbColName").Value.ToLower().Trim().Replace(" ", "_"),
                    TemplateColName = p.Element("TemplateColName").Value.ToLower().Trim().Replace(" ", "_")
                })
                .ToDictionary(t => t.DbColName, t => t.TemplateColName);
        }
        internal static string validateTemplateColumn(Dictionary<string, string> dicColumnMapping, DataTable dt)
        {
            string[] arrColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.Trim().ToLower().Replace(" ", "_")).ToArray();
            foreach (var pair in dicColumnMapping)
            {
                // if column not found in template and return error..
                if (!arrColumns.Contains(pair.Value.ToLower()))
                    return "Selected file does not contain '" + pair.Value + "' column!";
            }
            return "";
        }
        internal static string Wrap(string str, int maxLength, string prefix)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (maxLength <= 0) return prefix + str;

            var lines = new List<string>();

            // breaking the string into lines makes it easier to process.
            foreach (string line in str.Split("\n".ToCharArray()))
            {
                var remainingLine = line.Trim();
                do
                {
                    var newLine = GetLine(remainingLine, maxLength - prefix.Length);
                    lines.Add(newLine);
                    remainingLine = remainingLine.Substring(newLine.Length).Trim();
                    // Keep iterating as int as we've got words remaining 
                    // in the line.
                } while (remainingLine.Length > 0);
            }

            return string.Join(Environment.NewLine + prefix, lines.ToArray());
        }
        private static string GetLine(string str, int maxLength)
        {
            // The string is less than the max length so just return it.
            if (str.Length <= maxLength) return str;

            // Search backwords in the string for a whitespace char
            // starting with the char one after the maximum length
            // (if the next char is a whitespace, the last word fits).
            for (int i = maxLength; i >= 0; i--)
            {
                if (char.IsWhiteSpace(str[i]))
                    return str.Substring(0, i).TrimEnd();
            }

            // No whitespace chars, just break the word at the maxlength.
            return str.Substring(0, maxLength);
        }
      
    }
}
