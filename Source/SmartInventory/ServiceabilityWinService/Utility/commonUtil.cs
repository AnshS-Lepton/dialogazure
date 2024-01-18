using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace ServiceabilityWinService.Utility
{
    public static class commonUtil
    {
        public static string getTimeStamp()
        {
            return DateTimeHelper.Now.Year.ToString() + DateTimeHelper.Now.Month.ToString() + DateTimeHelper.Now.Hour.ToString() + DateTimeHelper.Now.Minute.ToString() + DateTimeHelper.Now.Second.ToString() + DateTimeHelper.Now.Millisecond.ToString();
        }
    }
    public static class ListExtensions
    {
        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 5)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}
