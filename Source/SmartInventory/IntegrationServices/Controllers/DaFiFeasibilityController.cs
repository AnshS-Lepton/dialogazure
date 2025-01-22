using BusinessLogics.DaFiFeasibilityAPI;
using IntegrationServices.Settings;
using Models;
using Models.DaFiFeasibilityAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility;

namespace IntegrationServices.Controllers
{
    public class DaFiFeasibilityController : ApiController
    {

        [HttpGet]
        [Route("darkFiberFeasibility")]
        public HttpResponseMessage getDarkFiberFeasibility(string request_id, string a_lat_lng, string z_lat_lng, string fiber_cores, string a_buffer = "500", string z_buffer = "500")
        {


            try
            {
                double dbl_a_buffer, dbl_z_buffer;
                int val_fiber_cores;

                if (!isLatLngValid(a_lat_lng) || !isLatLngValid(z_lat_lng))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid lat_lng!" });
                }

                if (!isbufferValid(a_buffer, out dbl_a_buffer) || !isbufferValid(z_buffer, out dbl_z_buffer))
                {
                    var buffervalue = ApplicationSettings.feasibility_buffer_limit;
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Invalid buffer Value. The value must be non-negative and less than {buffervalue}." });
                }

                if (!isfiberValid(fiber_cores, out val_fiber_cores))
                {
                    var fibervalue = ApplicationSettings.fiber_core_limit; // Get the fiber_core_limit

                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Invalid fiber_cores Value. The value must be non-negative and less than {fibervalue}." });

                }
                if (string.IsNullOrEmpty(request_id))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "request_id cannot be blank!" });
                }

                var apiKey = ApplicationSettings.Map_Key_Backend;

                var chkRoutes = BLDarkFiberFeasibility.Instance.CheckReservedRoute(request_id);
                if (chkRoutes != null)
                {
                    if (chkRoutes.status == true)
                        return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = chkRoutes.message });
                }

                var routes = BLDarkFiberFeasibility.Instance.GetExistingFiberRoutes(request_id, a_lat_lng, z_lat_lng, val_fiber_cores, apiKey, dbl_a_buffer, dbl_z_buffer);
                string feasibilityStatus = string.Empty;

                List<RouteDto> routesDto = new List<RouteDto>();

                if (routes.Count == 0)
                {
                    feasibilityStatus = "Not Feasible";
                    routes = BLDarkFiberFeasibility.Instance.GetNewFiberRoutes(request_id, a_lat_lng, z_lat_lng, val_fiber_cores, apiKey, dbl_a_buffer, dbl_z_buffer);

                    if (routes != null && routes.Count > 0)
                    {
                        // save the new route
                        BLDarkFiberFeasibility.Instance.SaveDarkFiber(request_id, "New", routes[0].route_id, routes[0].geojson_new_built, routes[0].total_new_length);
                    }
                    else
                    {
                        return this.Request.CreateResponse(HttpStatusCode.OK, new { status = "NO_ROUTE", message = "No route found." });
                    }
                }
                else
                {
                    feasibilityStatus = "Feasible";
                }

                routesDto = routes.Select(r => new RouteDto
                {
                    route_id = r.route_id,
                    route_type = r.route_type,
                    cable_segments = r.cable_segments,
                    geojson_existing_built = r.geojson_existing_built,
                    geojson_new_built = r.geojson_new_built,
                    total_existing_length = r.total_existing_length,
                    total_new_length = r.total_new_length,
                    start_entity = r.start_entity,
                    end_entity = r.end_entity
                }).ToList();

                var darkFiberFeasibilityResponse = new DaFiFeasibilityResponse
                {
                    request_id = request_id,
                    feasibility_result = feasibilityStatus,
                    routes = routesDto
                };

                return this.Request.CreateResponse(HttpStatusCode.OK, darkFiberFeasibilityResponse);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getDarkFiberFeasibility()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }

        [HttpGet]
        [Route("feasibilityKML")]
        public HttpResponseMessage getfeasibilityKML(string feasibility_request_id = "", string route_id = "")
        {

            try
            {
                if (string.IsNullOrEmpty(feasibility_request_id) && string.IsNullOrEmpty(route_id))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid Request!" });
                }
                var kmlContent = BLDarkFiberFeasibility.Instance.GetDarkFiberKML(feasibility_request_id, route_id);
                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(kmlContent);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileBytes)
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.google-earth.kml+xml");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{feasibility_request_id}_routes.kml"
                };

                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getfeasibilityKML()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }

        [HttpPost]
        [Route("reserveFeasibilityRoute")]
        public HttpResponseMessage reserveFeasibilityRoute(ReserveFeasibility obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => x.Key)
                              .ToList();

                    DbMessageForDaFiFeasibility ret = new DbMessageForDaFiFeasibility();
                    ret.status = "BadRequest";
                    ret.message = errors.ToString();
                    foreach (var err in errors)
                    {
                        ret.message += err + " ";
                    }

                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = ret.status, message = ret.message });


                }
                else
                {
                    var objRet = BLDarkFiberFeasibility.Instance.ReserveFeasibilityRoute(obj);
                    //return this.Request.CreateResponse(HttpStatusCode.OK, objRet);
                    return this.Request.CreateResponse(commonUtil.getHttpsStatus(objRet.status), new { status = objRet.status, message = objRet.message });
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("reserveFeasibilityRoute()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }
        [HttpPost]
        [Route("releaseFeasibilityRoute")]
        public HttpResponseMessage releaseFeasibilityRoute(ReleaseFeasibility obj)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    var errors = ModelState.Select(x => x.Key)
                //             .ToList();

                //    DbMessageForDaFiFeasibility ret = new DbMessageForDaFiFeasibility();
                //    ret.status = "BadRequest";
                //    ret.message = errors.ToString();
                //    foreach (var err in errors)
                //    {
                //        ret.message += err + "  ";
                //    }

                //    return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = ret.status, message = ret.message });

                //}
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => x.Key.Replace("obj.", "")).ToList();

                    var ret = new DbMessageForDaFiFeasibility
                    {
                        status = "BadRequest",
                        message = errors.Count == 1 ? $"Please provide a {errors[0]} .it cannot be null." : $"Please provide a {string.Join(" and ", errors)} .it cannot be null."
                    };
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = ret.status, message = ret.message });
                }

                else
                {
                    var objRet = BLDarkFiberFeasibility.Instance.ReleaseFeasibilityRoute(obj);
                    //return this.Request.CreateResponse(HttpStatusCode.OK, objRet);
                    return this.Request.CreateResponse(commonUtil.getHttpsStatus(objRet.status), new { status = objRet.status, message = objRet.message });
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("releaseFeasibilityRoute()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }

        private bool isLatLngValid(string lat_lng)
        {
            double latitude, longitude;

            try
            {
                string[] aparts = lat_lng.Split(',');
                if (aparts.Length == 2)
                {
                    latitude = double.Parse(aparts[0]);
                    longitude = double.Parse(aparts[1]);

                    return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;

                }
                return false;
            }

            catch { return false; }
        }


        private bool isbufferValid(string strBuffer, out double dblBuffer)
        {
            var buffervalue = ApplicationSettings.feasibility_buffer_limit;

            try
            {
                dblBuffer = double.Parse(strBuffer);
                return dblBuffer > 0 && dblBuffer <= Convert.ToInt32(buffervalue);
            }
            catch
            {
                dblBuffer = 0;
                return false;
            }

        }
        private bool isfiberValid(string fibernuber, out int val_fiber_cores)
        {

            var fibervalue = ApplicationSettings.fiber_core_limit;

            try
            {
                val_fiber_cores = int.Parse(fibernuber);
                return val_fiber_cores > 0 && val_fiber_cores <= Convert.ToInt32(fibervalue);
            }
            catch
            {
                val_fiber_cores = 0;
                return false;
            }
        }

    }
}