using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resources
{
    public class Helper
    {
        public static string MultilingualMessageConvert(string message)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(message))
            {
                var Messages = message.Trim().Split('[', ']');
                foreach (var res in Messages)
                {
                    if (!string.IsNullOrEmpty(res))
                    {
                        var propertyDetail = typeof(Resources).GetProperty(res);
                        if (propertyDetail == null)
                        {
                            result += string.IsNullOrEmpty(result) ? res : " " + res;
                        }
                        else
                        {
                            result += string.IsNullOrEmpty(result) ? propertyDetail.GetValue(propertyDetail.Name) : " " + propertyDetail.GetValue(propertyDetail.Name);
                        }
                    }
                }
            }
            return result;
        }
        public static void SetApplicationLanguge(string Languge)
        {
            if (!string.IsNullOrEmpty(Languge))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Languge);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Languge);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            }
           
        }
    }
}
