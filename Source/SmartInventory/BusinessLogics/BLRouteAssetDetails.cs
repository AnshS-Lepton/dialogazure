using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;

namespace BusinessLogics
{
    public class BLRouteAssetDetails
    {
        DAGetRouteAssetDetails objDAgetRoute = new DAGetRouteAssetDetails();
        public RouteAssetDeatils getRouteDetails(string latitude, string longitude)
        {
            return objDAgetRoute.getRouteDetails(latitude, longitude);

        }
    }
}
