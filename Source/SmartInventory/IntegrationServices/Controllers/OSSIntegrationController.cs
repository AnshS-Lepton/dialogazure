using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using BusinessLogics;
using IntegrationServices.Settings;
using Lepton.Entities;
using Models;

using Utility;

namespace IntegrationServices.Controllers
{
    [Authorize]
    [RoutePrefix("OSSIntegration/v2.0")]
    [Filters.CustomActionForXml]

    public class OSSIntegrationController : ApiController
    {
        #region GIS_OSS Integration

        [HttpGet]
        [Route("GetEntityLocation")]
        public ApiResponse<dynamic> GetEntityLocation(string requestID, string entity_type, string entity_network_id)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                if (!string.IsNullOrEmpty(entity_type) && !string.IsNullOrEmpty(entity_network_id))
                {
                    var res = new BLServiceability().GetEntityLocation(entity_type, entity_network_id);
                    if (res != null)
                    {
                        response.results = res;
                        response.status = ((int)HttpStatusCode.OK).ToString();
                    }
                    else
                    {
                        response.status = ((int)HttpStatusCode.NotFound).ToString();
                        response.error_message = "Record not found.";
                    }
                }
                else
                {
                    response.status = ((int)HttpStatusCode.BadRequest).ToString();
                    response.error_message = "Data Inputs Are Not Valid.";
                }
               
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityLocation()", "OSSIntegrationController", "", ex);
                response.status = ((int)HttpStatusCode.InternalServerError).ToString(); 
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpGet]
        [Route("GetIntermediateEntities")]
        public ApiResponse<dynamic> GetIntermediateEntities(string requestID, string source_entity_type, string source_id, string destination_entity_type, string destination_id, string port)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                if (!string.IsNullOrEmpty(source_entity_type)&& !string.IsNullOrEmpty(source_id) && !string.IsNullOrEmpty(destination_entity_type) && !string.IsNullOrEmpty(destination_id) && !string.IsNullOrEmpty(port))
                {
                    var res = new BLServiceability().GetIntermediateEntities(source_entity_type, source_id, destination_entity_type, destination_id, port);
                    if (res != null && res.IntermediateEntities!=null)
                    {
                        response.results = res;
                        response.status = ((int)HttpStatusCode.OK).ToString();
                    }
                    else
                    {
                        response.status = ((int)HttpStatusCode.NotFound).ToString();
                        response.error_message = "Record not found.";
                    }
                }
                else
                {
                    response.status = ((int)HttpStatusCode.BadRequest).ToString();
                    response.error_message = "Data Inputs Are Not Valid.";
                }
                
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetIntermediateEntities()", "OSSIntegrationController", "", ex);
                response.status = ((int)HttpStatusCode.InternalServerError).ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        [Route("updateAlarmStatus")]
        
        public ApiResponse<dynamic> UpdateAlarmStatusetails(UpdateAlarmStatusetails objUpdateAlarmStatusetails)
        {
            var response = new ApiResponse<dynamic>();
            if (!string.IsNullOrEmpty(objUpdateAlarmStatusetails.reference_id))
            {
                foreach (var obj in objUpdateAlarmStatusetails.Impacted_entities)
                {
                    if (!string.IsNullOrEmpty(obj.entity_id) && !string.IsNullOrEmpty(obj.entity_type)&& !string.IsNullOrEmpty(obj.port_number))
                    {
                        try
                        {
                            var res = new BLServiceability().UpdateAlarmStatusetails(obj);
                            if (res!=null)
                            {
                                
                                response.status = ((int)HttpStatusCode.OK).ToString();
                                response.error_message = res.error_message;
                                response.results = res.results;
                            }
                            else
                            {
                                response.status = ((int)HttpStatusCode.BadRequest).ToString();
                                response.error_message = "Data Inputs Are Not Valid.";
                            }
                            

                        }
                        catch (Exception ex)
                        {
                            ErrorLogHelper logHelper = new ErrorLogHelper();
                            logHelper.ApiLogWriter("UpdateAlarmStatusetails()", "OSSIntegrationController", "", ex);
                            response.status = ((int)HttpStatusCode.InternalServerError).ToString();
                            response.error_message = "Error While Processing  Request.";  
                        }
                    }
                }
                



            }
            else
            {
                response.status = ((int)HttpStatusCode.BadRequest).ToString();
                response.error_message = "Data Inputs Are Not Valid.";
            }
            return response;
        }


        #endregion
    }
}
