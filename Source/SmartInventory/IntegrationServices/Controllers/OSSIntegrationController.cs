using BusinessLogics;
using Lepton.Entities;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Utility;
using IntegrationServices.Settings;

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


        [HttpGet]
        [Route("serviceability")]

        public IHttpActionResult Serviceability([FromUri] ServiceabilityRequest ServiceabilityRequest)
        {
            var response = new ApiResponse<dynamic>();
            string logUrl = "";
            string responseFromServer = "";
            try
            {
                if (ModelState.IsValid)
                {
                    double latitude;
                    double longitude;
                   
                    if (!double.TryParse(ServiceabilityRequest.latitude.Trim(), out latitude) || ServiceabilityRequest.latitude != ServiceabilityRequest.latitude.Trim())
                    {
                        var errorResponse = new ErrorResponse
                        {
                            code = (int)HttpStatusCode.BadRequest,
                            message = "Invalid input for latitude"
                        };
                        return Content(HttpStatusCode.BadRequest, errorResponse);
                    }
                    if (!double.TryParse(ServiceabilityRequest.longitude.Trim(), out longitude) || ServiceabilityRequest.longitude != ServiceabilityRequest.longitude.Trim())
                    {
                        var errorResponse = new ErrorResponse
                        {
                            code = (int)HttpStatusCode.BadRequest,
                            message = "Invalid input for longitude"
                        };
                        return Content(HttpStatusCode.BadRequest, errorResponse);
                    }

                    var lstEntities = new BLServiceability().Serviceability(latitude, longitude);
                    var lstonlySplit = lstEntities.ToList();
                    string origin = ServiceabilityRequest.latitude.ToString() + "," + ServiceabilityRequest.longitude.ToString();
                    string destination = "";
                    Serviceability root = new Serviceability();
                    List<Devices> gp = new List<Devices>();
                    if (lstonlySplit.Count > 0)
                    {
                        var breakSplitter = ListExtensions.SplitList(lstonlySplit, 5);
                        foreach (var lstSplitter in breakSplitter)
                        {
                            string waypoints = "";
                            var i = 0;
                            foreach (var item in lstSplitter)
                            {
                                var latlong = item.geom.Replace("POINT(", "").Replace(")", "").Split(' ');
                                if (lstSplitter.Count() == 1)
                                {
                                    destination = latlong[1] + "," + latlong[0];
                                }
                                else
                                {

                                    i++;
                                    waypoints += origin + "|";
                                    waypoints += latlong[1] + "," + latlong[0] + "|";

                                    if (i == lstSplitter.Count())
                                    {
                                        destination = latlong[1] + "," + latlong[0];
                                    }

                                }

                            }
                            string url = @"https://maps.googleapis.com/maps/api/directions/json?";
                            if (lstSplitter.Count() == 1)
                            {
                                url += "origin=" + origin + "" + "&destination=" + destination + "&mode=walking&key=" + ApplicationSettings.Map_Key + "";
                            }
                            else
                            {
                                url += "origin=" + origin + "" + "&destination=" + destination + "" + "&waypoints=" + waypoints.Remove(waypoints.Length - 1, 1) + "&mode=walking&key=" + ApplicationSettings.Map_Key + "";
                            }
                            logUrl = url;
                            WebRequest request = WebRequest.Create(url);
                            WebResponse respons = request.GetResponse();
                            System.IO.Stream data = respons.GetResponseStream();
                            System.IO.StreamReader reader = new System.IO.StreamReader(data);
                            // json-formatted string from maps api
                            responseFromServer = reader.ReadToEnd();
                            url = string.Empty;
                            var json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(responseFromServer);
                            respons.Close();
                            List<decimal> dis = new List<decimal>();
                            var legs = new List<Newtonsoft.Json.Linq.JToken>();
                            for (int x = 0; x < json["routes"][0]["legs"].Count(); x++)
                            {
                                if (lstSplitter.Count() == 1)
                                {
                                    legs.Add(json["routes"][0]["legs"][x]);
                                }
                                else
                                {
                                    if (x != 0)
                                    {
                                        if (x != json["routes"][0]["legs"].Count() - 1)
                                        {
                                            legs.Add(json["routes"][0]["legs"][x]);
                                        }
                                    }
                                }

                            }
                            for (int r = 0; r < legs.Count(); r++)
                            {
                                if (r % 2 == 0)
                                {
                                    var distance = legs[r]["distance"]["value"].ToString();
                                    dis.Add(Convert.ToDecimal(distance));
                                }

                            }
                            for (int d = 0; d < dis.Count(); d++)
                            {
                                if (dis[d] < ApplicationSettings.Serviceability_Buffer)
                                {
                                    var coordinates = lstSplitter.ToList()[d].geom.Replace("POINT(", "").Replace(")", "").Split(' ');
                                    Devices objDevices = new Devices();
                                    objDevices.entity_id = lstSplitter.ToList()[d].display_name;
                                    objDevices.entity_type = lstSplitter.ToList()[d].entity_type;

                                    objDevices.distance = dis[d];

                                    gp.Add(objDevices);
                                }
                            }
                            

                        }
                        root.devices = gp.OrderBy(x => x.distance).ToList();
                        root.status = "FEASIBLE";
                        root.reference_id = ServiceabilityRequest.reference_id;
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
                    return Json(root);

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
                logHelper.ApiLogWriter("Serviceability()", "OSSIntegrationController", "", ex);
                var errorResponse = new ErrorResponse
                {
                    code = (int)HttpStatusCode.InternalServerError,
                    message = "Error While Processing Request."
                };
                return Content(HttpStatusCode.InternalServerError, errorResponse);
            }
            
        }



        #endregion
    }
}
