using BusinessLogics;
using Lepton.Entities;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
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
        public IHttpActionResult GetEntityLocation([FromUri] EntityLocationRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var res = new BLServiceability().GetEntityLocation(request.entity_type, request.entity_network_id);

                    if (res != null && res.error == null)
                    {
                        var jsonObject = JObject.Parse(JsonConvert.SerializeObject(res));
                        jsonObject.Remove("error");
                        var responses = new JObject
                        {
                            ["entity_id"] = jsonObject["entity_id"]?.ToString(),
                            ["entity_type"] = jsonObject["entity_type"]?.ToString(),
                            ["province"] = jsonObject["province"]?.ToString(),
                            ["region"] = jsonObject["region"]?.ToString(),
                            ["location"] = jsonObject["location"]
                        };

                        return Json(responses);
                    }
                    else if (res != null && res.error != null)
                    {
                        var errorResponse = new ErrorResponse
                        {
                            code = (int)HttpStatusCode.BadRequest,
                            message = res.error
                        };
                        return Content(HttpStatusCode.BadRequest, errorResponse);
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
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    var errorMessageString = string.Join("; ", errorMessages);
                    var errorResponse = new ErrorResponse
                    {
                        code = (int)HttpStatusCode.BadRequest,
                        message = errorMessageString
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
        public IHttpActionResult GetIntermediateEntities([FromUri] IntermediateEntitiesRequest request)
        {
            var response = new ApiResponse<dynamic>();
            try
            {
                if (ModelState.IsValid)
                {

                    var res = new BLServiceability().GetIntermediateEntities(request.source_entity_type, request.source_id, request.destination_entity_type, request.destination_id, request.port);
                    if (res != null && res.error == null && res.intermediate_entities != null && res.intermediate_entities.Count > 0)
                    {
                        var jsonObject = JObject.Parse(JsonConvert.SerializeObject(res));
                        jsonObject.Remove("error");
                        var responses = new JObject
                        {
                            ["source_entity"] = jsonObject["source_entity"],
                            ["destination_entity"] = jsonObject["destination_entity"],
                            ["intermediate_entities"] = jsonObject["intermediate_entities"],
                            ["distance_meters"] = jsonObject["distance_meters"]?.ToString()
                        };
                        return Json(responses);

                        //var responses = new IntermediateEntitiesDetails();
                        //responses = res;
                        //return Json(responses);
                    }
                    else if (res != null && res.error != null)
                    {
                        var errorResponse = new ErrorResponse
                        {
                            code = (int)HttpStatusCode.BadRequest,
                            message = res.error
                        };
                        return Content(HttpStatusCode.BadRequest, errorResponse);
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
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    var errorMessageString = string.Join("; ", errorMessages);
                    var errorResponse = new ErrorResponse
                    {
                        code = (int)HttpStatusCode.BadRequest,
                        message = errorMessageString
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
                if (ModelState.IsValid)
                {
                    bool isValid = false;

                    foreach (var obj in objUpdateAlarmStatusetails.Impacted_entities)
                    {
                        if (ModelState.IsValid)
                        {
                            var res = new BLServiceability().UpdateAlarmStatusetails(obj);
                            if (res != null && res.status.Equals("true"))
                            {
                                responses.status = "OK";
                                responses.message = res.message;
                                isValid = true;
                            }
                            else if (res != null && res.status.Equals("false"))
                            {
                                var errorResponses = new ErrorResponse
                                {
                                    code = (int)HttpStatusCode.BadRequest,
                                    message = res.message
                                };
                                return Content(HttpStatusCode.BadRequest, errorResponses);
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
                            var errorMessages_ = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                            var errorMessageString_ = string.Join("; ", errorMessages_);
                            var errorResponses = new ErrorResponse
                            {
                                code = (int)HttpStatusCode.BadRequest,
                                message = errorMessageString_
                            };
                            return Content(HttpStatusCode.BadRequest, errorResponses);
                        }
                    }

                    if (isValid)
                    {
                        return Json(responses);
                    }


                }
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorMessageString = string.Join("; ", errorMessages);
                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.BadRequest,
                    message = errorMessageString
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
