using System;
using System.Collections.Generic;
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
    [RoutePrefix("serviceability/v2.0")]
    public class ServiceController : ApiController
    {
        [HttpPost]
        [Route("GetServiceability")]
        public ApiResponse<ServiceabilityRoot> GetServiceability(ServiceabilityModel obj)
        {
            var response = new ApiResponse<ServiceabilityRoot>();
            string responseFromServer = "";
            string logUrl = "";
            try
            {

                var lstEntities = new BLServiceability().getServiceabilityResponse(obj.latitude, obj.longitude, obj.segment, 1);
                var lstonlySplit = lstEntities.Where(x => x.entity_title == "Splitter").Take(ApplicationSettings.Serviceability_Entity_Limit).ToList();
                string origin = obj.latitude.ToString() + "," + obj.longitude.ToString();
                string destination = "";

                ServiceabilityRoot root = new ServiceabilityRoot();
                List<GPON> gp = new List<GPON>();
                root.requestid = obj.requestId;
                if (lstonlySplit.Count() > 0)
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
                        #region iteration over json response
                        // removing first and last nodes from response legs coming from the api
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
                        // skipping legs because origin and waypoint is same
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
                                GPON gpon = new GPON();
                                gpon.nap_id = lstSplitter.ToList()[d].display_name;
                                gpon.nap_name = lstSplitter.ToList()[d].common_name;
                                gpon.latitude =Convert.ToDouble(coordinates[1]);
                                gpon.longitude= Convert.ToDouble(coordinates[0]);
                                gpon.distance = dis[d];
                                gpon.utilized_port = lstSplitter.ToList()[d].utilized_port;
                                gp.Add(gpon);
                            }
                        }
                        
                        root.devicelist.GPON = gp.OrderBy(x => x.distance).ToList();

                        #endregion
                    }
                    

                }
                // Handling the multiple distance from Google Direction API - Raised by Converge  
                var latlonglist = root.devicelist.GPON.Select(x => new { x.latitude, x.longitude, x.distance }).Distinct();
                var result = latlonglist.GroupBy(g => new { g.latitude, g.longitude }).ToList();

                foreach (var objList in result)
                {
                    var records = objList.ToList().Count();
                    if (records > 1)
                    {
                        var correctdistance = objList.FirstOrDefault();

                        foreach (var first in objList.ToList())
                        {
                            root.devicelist.GPON.Where(x => x.latitude == objList.Key.latitude && x.longitude == objList.Key.longitude).ToList()
                                      .ForEach(record => record.distance = correctdistance.distance);
                        }

                    }

                }
                response.status = StatusCodes.OK.ToString();
                response.results = root;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetServiceability()", "ServiceabilityController","google request start"+ logUrl + " google request end" + responseFromServer, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpGet]
        [Route("GetAPIsettings")]
        public ApiResponse<ApiSettings> GetAPIsettings(ApiSettings obj)
        {
            
            var response = new ApiResponse<ApiSettings>();
            if (obj.key != null)
            {
                try
                {
                    GlobalSetting globalSettings = new BLGlobalSetting().getValueFullText(obj.key);
                    ApiSettings root = new ApiSettings();
                    if (globalSettings != null)
                    {
                        root.value = globalSettings.value;
                        root.key = globalSettings.key;
                    }
                    response.status = StatusCodes.OK.ToString();
                    response.results = root;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("GetAPIsettings()", "ServiceabilityController", "", ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            else
            {               
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.error_message = "Please enter valid request paramaters.";
            }
            return response;
        }
        [HttpPost]
        [Route("UpdateAPIsettings")]
        public ApiResponse<dynamic> UpdateAPIsettings(ApiSettings obj)
        {
            var response = new ApiResponse<dynamic>();
            if (obj.key != null && obj.value!=null)
            {
                try
                {
                    var res = new BLServiceability().updateApiSettings(obj);
                    response.status = res.status;
                    response.results = res.results;
                    response.error_message = res.error_message;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("UpdateAPIsettings()", "ServiceabilityController", "", ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            else
            {
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.error_message = "Please enter valid request paramaters.";
            }
            return response;
        }

        #region Update Export File By Shazia 
        [HttpPost]
        [Filters.CustomActionForXml]
        public ApiResponse<String> UpdateXmlFileStatus(ReqInputs data)
        {
            var response = new ApiResponse<String>();
            bool isValidRequest = true;
            try
            {
                Models.ProcessSummary objUpdateXmlFileStatus = ReqHelpers.GetRequestData<Models.ProcessSummary>(data);
                if (isValidRequest && (string.IsNullOrEmpty(objUpdateXmlFileStatus.file_name)))
                {
                    response.status = StatusCodes.REQUIRED.ToString();
                    response.error_message = "File name is required!";
                    response.results = null;
                    isValidRequest = false;
                }
                //if (isValidRequest && (string.IsNullOrEmpty(objUpdateXmlFileStatus.file_version)))
                //{
                //    response.status = StatusCodes.REQUIRED.ToString();
                //    response.error_message = "File Version is required!";
                //    response.results = null;
                //    isValidRequest = false;
                //}
                if (isValidRequest && (string.IsNullOrEmpty(objUpdateXmlFileStatus.csa_id)))
                {
                    response.status = StatusCodes.REQUIRED.ToString();
                    response.error_message = "CSA Id is required!";
                    response.results = null; 
                    isValidRequest = false;
                }
                if (isValidRequest)
                {
                    var XmlFileStatus = new BLProcess().UpdateXmlFileStatus(objUpdateXmlFileStatus.file_name, objUpdateXmlFileStatus.import_status, objUpdateXmlFileStatus.file_version, objUpdateXmlFileStatus.csa_id,0);
                    APIRequestLog ApiInfo = (APIRequestLog)System.Web.HttpContext.Current.Items["filterobj"];
                    ApiInfo.out_date_time = DateTime.Now;
                    ApiInfo.created_on = DateTime.Now;
                    ApiInfo.latency = (ApiInfo.out_date_time - ApiInfo.in_date_time).TotalMilliseconds;
                    System.Web.HttpContext.Current.Items["filterobj"] = ApiInfo;
                    List<ApiReturnData> obj = new List<ApiReturnData>();
                    obj.Add(new ApiReturnData
                    {
                        transaction_id = ApiInfo.transaction_id,
                        InDateTime = Convert.ToString(ApiInfo.in_date_time),
                        OutDateTime = Convert.ToString(ApiInfo.out_date_time),
                        latency = Convert.ToString(ApiInfo.latency),
                        CreatedOn = Convert.ToString(ApiInfo.created_on)
                    });

                    JavaScriptSerializer jsobj = new JavaScriptSerializer();
                    var results = jsobj.Serialize(obj);
                    response.status = XmlFileStatus.status ? StatusCodes.OK.ToString() : StatusCodes.FAILED.ToString();
                    response.error_message = XmlFileStatus.message;                 
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "Invalid Request!";
                    response.results = null;
                }
            }
            catch (Exception ex)
            { 
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateXmlFileStatus()", "Service Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region UPDATE RFS_STATUS BY SHAZIA
        [HttpPost]
        [Filters.CustomActionForXml]
        public ApiResponse<String> UpdateRfsStatus(ReqInputs data)
        {
            var response = new ApiResponse<String>();
            bool isValidRequest = true;
            try
            {
                CSA objCsa = ReqHelpers.GetRequestData<CSA>(data);
                if (isValidRequest && (string.IsNullOrEmpty(objCsa.gis_design_id)))
                {
                    response.status = StatusCodes.REQUIRED.ToString();
                    response.error_message = "Gis Degin Id is required!";
                    response.results = null;
                    isValidRequest=false;
                }
                else if (isValidRequest && (string.IsNullOrEmpty(objCsa.rfs_status)))
                    {
                    response.status = StatusCodes.REQUIRED.ToString();
                    response.error_message = "Rfs Status is required!";
                    response.results = null;
                    isValidRequest = false;
                }
               if(isValidRequest)
                {
                    var csaRfsStatus = new BLCsa().UpdateRfsStatus(objCsa.gis_design_id, objCsa.rfs_status);
                    APIRequestLog ApiInfo = (APIRequestLog)System.Web.HttpContext.Current.Items["filterobj"];
                    ApiInfo.out_date_time = DateTime.Now;
                    ApiInfo.latency = (ApiInfo.out_date_time - ApiInfo.in_date_time).TotalMilliseconds;
                    System.Web.HttpContext.Current.Items["filterobj"] = ApiInfo;
                    List<ApiReturnData> obj = new List<ApiReturnData>();
                    if (!string.IsNullOrEmpty(csaRfsStatus.gis_design_id))
                    {
                        obj.Add(new ApiReturnData
                        {
                            transaction_id = ApiInfo.transaction_id,
                            InDateTime = Convert.ToString(ApiInfo.in_date_time),
                            OutDateTime = Convert.ToString(ApiInfo.out_date_time),
                            latency = Convert.ToString(ApiInfo.latency)
                        });
                        JavaScriptSerializer jsobj = new JavaScriptSerializer();
                        var results = jsobj.Serialize(obj);
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = "RFS Status Updated successfully!";
                        response.results = null;


                    }
                    else
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.error_message = "Invalid Request";
                        response.results = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateRfsStatus()", "Service Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #region UPDATE Gis_Design_ID BY Ankit Ojha
        [HttpPost]
        public ApiResponse<dynamic> UpdateDuplicateDesignID(UpdateDesignIDInputsDetails InputDetails)
        {
            var response = new ApiResponse<dynamic>();
            for (int i = 0; i < InputDetails.InputDetails.Count; i++)
            {
                if (InputDetails.InputDetails[i].network_id != null
                    && InputDetails.InputDetails[i].old_design_id != null
                    && InputDetails.InputDetails[i].entity_type != null
                     && InputDetails.InputDetails[i].new_design_id != null)
                {
                    try
                    {
                        UpdateDesignIDInputs udiObj = new UpdateDesignIDInputs();
                        udiObj.network_id = InputDetails.InputDetails[i].network_id;
                        udiObj.entity_type = InputDetails.InputDetails[i].entity_type;
                        udiObj.new_design_id = InputDetails.InputDetails[i].new_design_id;
                        udiObj.old_design_id = InputDetails.InputDetails[i].old_design_id;


                        var res = new BLServiceability().UpdateDesignIDInputs(udiObj);

                        udiObj = null;

                        response.status = "Ok";
                        response.results = res.results;
                        response.error_message = "Data Updated Successfully";
                    }
                    catch (Exception ex)
                    {
                        ErrorLogHelper logHelper = new ErrorLogHelper();
                        logHelper.ApiLogWriter("UpdateDesignID()", "ServiceabilityController", "", ex);
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                        response.error_message = "Error While Processing  Request.";
                    }
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "All request paramaters are mandatory.";
                }
            }


            return response;
        }
        #endregion

        #region ResetXml BY Ankit Ojha
        [HttpPost]
        public ApiResponse<dynamic> ResetXml(int System_id)
        {
            var response = new ApiResponse<dynamic>();

            if (System_id > 0)
            {
                try
                {
                    var res = new BLServiceability().ResetXml(System_id);

                    response.status = "Ok";
                    response.results = res.results;
                    response.error_message = "Data Updated Successfully";
                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("ResetXml()", "ServiceabilityController", "", ex);
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    response.error_message = "Error While Processing  Request.";
                }
            }
            else
            {
                response.status = StatusCodes.INVALID_INPUTS.ToString();
                response.error_message = "Please enter valid request paramaters.";
            }
            return response;
        }
        #endregion

    }
}