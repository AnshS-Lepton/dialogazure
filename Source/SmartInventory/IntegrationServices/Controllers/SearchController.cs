using BusinessLogics;
using IntegrationServices.Settings;
using Models;
using Models.API;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using Utility;

namespace IntegrationServices.Controllers
{
    [Authorize]
    [RoutePrefix("api/entity")]
    public class SearchController : ApiController
    {
        [HttpPost]
        [Route("GetAvailableDevices")]
        public ApiResponse<dynamic> GetAvailableDevices(DeviceSearch obj)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                var objResponse = BLDeviceSearch.Instance.GetNearbyDevices(obj);
                response.status = objResponse.status.ToString();
                response.error_message = Convert.ToString(objResponse.error_message);
                response.results = objResponse.results;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAvailableDevices()", "SearchController", "", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        [Route("UpdatePortStatus")]
        public ApiResponse<dynamic> UpdatePortStatus(PortStatusUpdateInfo obj)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                if (obj.system_id == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Port system id is required.";
                    return response;
                }
                if (string.IsNullOrEmpty(obj.port_status))
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Port status id is required.";
                    return response;
                }
                obj.user_id = ApplicationSettings.getUser("user_id");
                obj.source_ref_type = ApplicationSettings.getUser("source_ref_type");
                var objResponse = BLDeviceSearch.Instance.UpdatePortStatus(obj);
                response.status = objResponse.status.ToString();
                response.error_message = Convert.ToString(objResponse.error_message);
                response.results = objResponse.results;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdatePortStatus()", "SearchController", "", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        [HttpPost]
        [Route("GetFaultTrace")]
        public ApiResponse<dynamic> GetFaultTrace(FiberCutTracingInfo obj)
        {
            var response = new ApiResponse<dynamic>();
            try
            {

                if (obj.core_port_no == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Port number is required. (parameter name: core_port_no)";
                    return response;
                }
                if (obj.entity_type == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Entity type is required. (parameter name: entity_type)";
                    return response;
                }
                if (obj.cut_distance == null)
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "Cable cut distance is required. (parameter name: cut_distance)";
                    return response;
                }

                var objResponse = BLDeviceSearch.Instance.GetFiberCutDistance(obj);
                response.status = objResponse.status.ToString();
                response.error_message = Convert.ToString(objResponse.error_message);
                response.results = objResponse.results;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetFaultTrace()", "SearchController", "", ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpGet]
        public IHttpActionResult GetCablesByFiberLinkIds(string linkIds)
        {
            try
            {
                string cableDetails = new BLSite().GetCablesByFiberLinkIds(linkIds).FirstOrDefault();

                var cableDetailsObject = JsonConvert.DeserializeObject(cableDetails);

                return Json(new
                {
                    cableDetails = cableDetailsObject
                });
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("IntegrationServices.GetCableListByLinkIds()", "Search", ex);
                return Json(new { status = ResponseStatus.ERROR.ToString(), result = "" });
            }
        }
    }
}