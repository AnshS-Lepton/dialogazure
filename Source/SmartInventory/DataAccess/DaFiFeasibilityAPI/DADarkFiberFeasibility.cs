using DataAccess.DBHelpers;
using Models;
using Models.DaFiFeasibilityAPI;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.DaFiFeasibilityAPI
{
    public class DADarkFiberFeasibility : Repository<object>
    {
        DADarkFiberFeasibility()
        {

        }
        private static DADarkFiberFeasibility objDADarkFiberFeasibility = null;
        private static readonly object lockObject = new object();
        public static DADarkFiberFeasibility Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDADarkFiberFeasibility == null)
                    {
                        objDADarkFiberFeasibility = new DADarkFiberFeasibility();
                    }
                }
                return objDADarkFiberFeasibility;
            }
        }

        public List<Route> GetExistingFiberRoutes(string request_id, string source, string destination, int fiber_cores, double a_buffer, double z_buffer)
        {
            try
            {
                var resp =  repo.ExecuteProcedure<Route>("fn_sf_get_existing_fiber_route",
                                                              new { p_request_id = request_id, p_source = source, p_destination = destination, p_start_buffer = a_buffer, p_end_buffer = z_buffer, p_fiber_cores = fiber_cores }, true);
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.WriteDebugLog("GetExistingFiberRoutes-1");
                return resp;
            }
            catch {
                throw; 
            }
        }

        public DbMessage SaveDarkFiber(string request_id, string route_type, string route_id, string geojson_new_built, double total_new_length)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_sf_save_dark_fiber_routes", new { p_request_id = request_id, p_route_type = route_type, p_route_id = route_id, p_geojson_new_built = geojson_new_built, p_total_new_length = total_new_length }).FirstOrDefault();
                return response;
            }
            catch
            {
                throw;
            }
        }
        public DbMessageForDaFiFeasibility ReserveFeasibilityRoute(ReserveFeasibility obj)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessageForDaFiFeasibility>("fn_api_reserve_feasibility_route", new { p_feasibility_request_id = obj.feasibility_request_id, p_route_id = obj.route_id, p_customer_id = obj.customer_id, p_customer_name = obj.customer_name }).FirstOrDefault();
                return response;
            }
            catch
            {
                throw;
            }
        }
        public DbMessageForDaFiFeasibility ReleaseFeasibilityRoute(ReleaseFeasibility obj)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessageForDaFiFeasibility>("fn_api_release_feasibility_route", new { p_feasibility_request_id = obj.feasibility_request_id, p_route_id = obj.route_id }).FirstOrDefault();
                return response;
            }
            catch
            {
                throw;
            }
        }

        public List<FeasibileRouteKML> GetDarkFiberKML(string request_id,string route_id)
        {
            try
            {
                var resp = repo.ExecuteProcedure<FeasibileRouteKML>("fn_sf_get_darkFiber_kml", new { p_request_id = request_id, p_route_id= route_id }, true);

                return resp;
            }
            catch { throw; }
        }
        public DbMessage CheckReservedRoute(string request_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_check_sf_reserved_route", new { p_request_id = request_id }, false).FirstOrDefault();
            }
            catch { throw; }
        }
    }
}
