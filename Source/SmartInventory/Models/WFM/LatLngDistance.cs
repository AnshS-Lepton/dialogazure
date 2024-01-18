using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WFM
{
   public static class LatLngDistance
    {
        static double toRadians(double angleIn10thofaDegree)
        {
            return (angleIn10thofaDegree * Math.PI) / 180;
        }
        public static double distance(double lat1,
                                double lat2,
                                double lon1,
                                double lon2)
        {
            lon1 = toRadians(lon1);
            lon2 = toRadians(lon2);
            lat1 = toRadians(lat1);
            lat2 = toRadians(lat2);

            double dlon = lon2 - lon1;
            double dlat = lat2 - lat1;
            double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);
            double c = 2 * Math.Asin(Math.Sqrt(a));
            double r = 6371;
            return (c * r);
        }
    }
}
