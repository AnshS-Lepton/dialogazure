using BusinessLogics;
using Lepton.Entities;
using Models;
using System;
using System.Net;
using System.Web.Http;
using Utility;

namespace IntegrationServices.Controllers
{
    [Authorize]
    [RoutePrefix("api/v1")]
    [Filters.CustomActionForXml]

    public class OSSIntegrationController : ApiController
    {
        
        #region GIS_OSS Integration

        [HttpGet]
        [Route("entityLocation")]
        public IHttpActionResult GetEntityLocation(string entity_type, string entity_network_id)
        {
            try
            {
                if (!string.IsNullOrEmpty(entity_type) && !string.IsNullOrEmpty(entity_network_id))
                {
                    var res = new BLServiceability().GetEntityLocation(entity_type, entity_network_id);
                    if (res != null)
                    {

                        var responses = new EntityLocationDetails();
                        responses = res;
                        return Json(responses);
                    }
                    else
                    {
                        var errorResponse = new ErrorResponse
                        {
                            code = (int)HttpStatusCode.NotFound,
                            message = "Data not found"
                        };

                        return Content(HttpStatusCode.NotFound, errorResponse);
                    }
                }
                else
                {
                    var errorResponse = new ErrorResponse
                    {
                        code = (int)HttpStatusCode.BadRequest,
                        message = "Data Inputs Are Not Valid."
                    };
                    return Content(HttpStatusCode.BadRequest, errorResponse);
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetEntityLocation()", "OSSIntegrationController", "", ex);
                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.InternalServerError,
                    message = "Error While Processing  Request."
                };
                return Content(HttpStatusCode.InternalServerError, errorResponse);

            }
            
        }

        [HttpGet]
        [Route("intermediateEntities")]
        public IHttpActionResult GetIntermediateEntities(string source_entity_type, string source_id, string destination_entity_type, string destination_id, string port)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                if (!string.IsNullOrEmpty(source_entity_type) && !string.IsNullOrEmpty(source_id) && !string.IsNullOrEmpty(destination_entity_type) && !string.IsNullOrEmpty(destination_id) && !string.IsNullOrEmpty(port))
                {
                    var res = new BLServiceability().GetIntermediateEntities(source_entity_type, source_id, destination_entity_type, destination_id, port);
                    if (res != null && res.intermediate_entities != null && res.intermediate_entities.Count >0)
                    {
                        var responses = new IntermediateEntitiesDetails();
                        responses = res;
                        return Json(responses);
                    }
                    else
                    {
                        var errorResponse = new ErrorResponse
                        {
                            code = (int)HttpStatusCode.NotFound,
                            message = "Data not found"
                        };
                        return Content(HttpStatusCode.NotFound, errorResponse);
                    }
                }
                else
                {
                    var errorResponse = new ErrorResponse
                    {
                        code = (int)HttpStatusCode.BadRequest,
                        message = "Data Inputs Are Not Valid."
                    };
                    return Content(HttpStatusCode.BadRequest, errorResponse);
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetIntermediateEntities()", "OSSIntegrationController", "", ex);
                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.InternalServerError,
                    message = "Error While Processing  Request."
                };
                return Content(HttpStatusCode.InternalServerError, errorResponse);
            } 
        }

        [HttpPost]
        [Route("updateAlarmStatus")]

        public IHttpActionResult UpdateAlarmStatusetails(UpdateAlarmStatusetails objUpdateAlarmStatusetails)
        {
            var responses = new APIResponse();
           
            try
            {
                if (!string.IsNullOrEmpty(objUpdateAlarmStatusetails.reference_id))
                {
                    bool isValid = false;

                    foreach (var obj in objUpdateAlarmStatusetails.Impacted_entities)
                    {
                        if (!string.IsNullOrEmpty(obj.entity_id) && !string.IsNullOrEmpty(obj.entity_type) && !string.IsNullOrEmpty(obj.port_number))
                        {
                            var res = new BLServiceability().UpdateAlarmStatusetails(obj);
                            if (res != null && res.status.Equals("true"))
                            {
                                responses.status = "OK";
                                responses.message = res.message;
                                isValid = true;
                            }
                            else
                            {
                                var errorresponse = new ErrorResponse
                                {
                                    code = (int)HttpStatusCode.NotFound,
                                    message = "Data not found"
                                };
                                isValid = false;
                                return Content(HttpStatusCode.NotFound, errorresponse);
                            }
                            

                        }
                        else
                        {
                            var errorresponse = new ErrorResponse
                            {
                                code = (int)HttpStatusCode.BadRequest,
                                message = "Data Inputs Are Not Valid."

                            };
                            isValid = false;
                            return Content(HttpStatusCode.BadRequest, errorresponse);
                        }
                    }

                    if (isValid)
                    {
                        return Json(responses);
                    }


                }
                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.BadRequest,
                    message = "Data Inputs Are Not Valid."
                };
                return Content(HttpStatusCode.BadRequest, errorResponse);


            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateAlarmStatusetails()", "OSSIntegrationController", "", ex);
                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.InternalServerError,
                    message = "Error While Processing Request."
                };
                return Content(HttpStatusCode.InternalServerError, errorResponse);
            }
        }


        //[HttpGet]
        //[Route("serviceability")]

        //public ApiResponse<Serviceability> Serviceability(string reference_id, string latitude, string longitude)
        //{
        //    var response = new ApiResponse<Serviceability>();
        //    string logUrl = "";
        //    string responseFromServer = "";
        //    var lstEntities = new BLServiceability().Serviceability(Convert.ToDouble(latitude), Convert.ToDouble(longitude));
        //    var lstonlySplit = lstEntities.Where(x => x.entity_title == "Splitter").Take(ApplicationSettings.Serviceability_Entity_Limit).ToList();
        //    string origin = latitude.ToString() + "," + longitude.ToString();
        //    string destination = "";
        //    Serviceability root = new Serviceability();
        //    List<Devices> gp = new List<Devices>();
        //    root.reference_id = reference_id;
        //    if (lstEntities != null && lstEntities.Count > 0)
        //    {
        //        foreach (var item in lstEntities)
        //        {

        //        }

        //    }

        //    return response;
        //}



        #endregion
    }
}
