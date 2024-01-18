using BusinessLogics;
using IntegrationServices.Filters;
using IntegrationServices.Settings;
using Models;
using Models.API;
using Models.WFM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using IntegrationServices.Helper;
using Utility;

namespace IntegrationServices.Controllers
{
    [Authorize]
    [RoutePrefix("v1.0")]
    public class DistanceController : ApiController
    {
        [HttpPost]
        [Route("getnewcustomerdistance")]
        public ApiResponse<dynamic> getnewcustomerdistance(CustDistanceIN obj)
        {
            var response = new ApiResponse<dynamic>();
            string responseFromServer = "";
            try
            {
                var lstEntities = new BLCustomerDistance().getCustomerDistanceResponse(obj.latitude, obj.longitude, obj.customer_refid);
                if (lstEntities.Count > 0)
                {
                    string url = @"https://maps.googleapis.com/maps/api/geocode/json?latlng=" + obj.latitude + "," + obj.longitude + "&key=" + ApplicationSettings.Map_Key_Backend + "";
                    WebRequest request = WebRequest.Create(url);
                    WebResponse respons = request.GetResponse();
                    System.IO.Stream resData = respons.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(resData);
                    // json-formatted string from maps api
                    responseFromServer = reader.ReadToEnd();
                    var json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(responseFromServer);
                    var address = "Legend zone not availiable for this location, Please try again !";
                    if (json["results"].Count() > 0)
                    {
                        address = json["results"][1]["formatted_address"].ToString();
                    }
                    lstEntities[0].customer_add = address;
                    lstEntities[0].customer_refid = obj.customer_refid;
                    respons.Close();
                    response.results = lstEntities;
                }
                else
                    response.results = "No record found !";
                response.status = StatusCodes.OK.ToString();

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getnewcustomerdistance()", "DistanceController", responseFromServer, ex);
                response.status = StatusCodes.EXCEPTION.ToString();
                response.error_message = "message: " + ex.Message + ", inner exception: " + ex.InnerException + "";
            }
            return response;
        }
        [HttpPost]
        [Route("getcustomerdistance")]
        public ApiResponse<dynamic> getcustomerdistance(CustDistanceIN obj)
        {
            var response = new ApiResponse<dynamic>();
            string responseFromServer = "";
            try
            {
                var lstEntities = new BLCustomerDistance().getCustomerSplitterResponse(obj.customer_refid);
                if (lstEntities.Count > 0)
                {
                    string url = @"https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lstEntities[0].latitude + "," + lstEntities[0].longitude + "&key=" + ApplicationSettings.Map_Key_Backend + "";
                    WebRequest request = WebRequest.Create(url);
                    WebResponse respons = request.GetResponse();
                    System.IO.Stream resData = respons.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(resData);
                    // json-formatted string from maps api
                    responseFromServer = reader.ReadToEnd();
                    var json = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(responseFromServer);
                    var address = "Legend zone not availiable for this location, Please try again !";
                    if (json["results"].Count() > 0)
                    {
                        address = json["results"][1]["formatted_address"].ToString();
                    }
                    lstEntities[0].customer_add = address;

                    respons.Close();
                    response.results = lstEntities;
                }
                else
                    response.results = "No record found !";
                response.status = StatusCodes.OK.ToString();

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getcustomerdistance()", "DistanceController", responseFromServer, ex);
                response.status = StatusCodes.EXCEPTION.ToString();
                response.error_message = "message: " + ex.Message + ", inner exception: " + ex.InnerException + "";
            }
            return response;
        }
    }
}