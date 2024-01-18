using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Models
{
    public class DateTimeHelper
    {
        public static DateTime Now
        {
            get
            {   
                string timeZone = System.Configuration.ConfigurationManager.AppSettings["TimeZone"];
                if (!string.IsNullOrEmpty(timeZone))
                {
                    // RETURN  DATETIME BASED ON CONFIGURED TIMEZONE..
                    return TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
                }
                else
                {
                    // RETURN  DATETIME BASED ON SERVER TIMEZONE..
                    return DateTime.Now;
                }
            }
        }

        public static string DateTimeFormate(string date)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                result = Convert.ToDateTime(date).ToString("dd-MMM-yyyy", new CultureInfo("en-US"));
            }
            return result;
        }
        public static string DateTimeFormatWithTime(string date)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(date))
            {
                result = Convert.ToDateTime(date).ToString("dd'-'MMM'-'yyyy hh:mm tt", new CultureInfo("en-US"));
            }
            return result;
        }
    }

}
