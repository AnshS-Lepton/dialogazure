using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
    public class DAGetRouteAssetDetails: Repository<RouteAssetDeatils>
    {
      
           public RouteAssetDeatils getRouteDetails(string latitude, string longitude)
        {
            try
            {
                return repo.ExecuteProcedure<RouteAssetDeatils>("fn_get_route_asset_details_json", new { lat = latitude, lng = longitude }, true).FirstOrDefault();
            }
            catch { throw; }
        }
    }
}
