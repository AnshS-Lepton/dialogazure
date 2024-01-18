using BusinessLogics;
using Models;
using Models.API;
using PartnetServices.Filter;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Script.Serialization;
using Utility;
using CustomAction = PartnetServices.Filter.CustomAction;

namespace PartnetServices.Controllers
{
    [RoutePrefix("api/Services")]   
    [CustomAction]
    //[CustomAuthorization]
    public class ServicesController : ApiController
    {
        [HttpPost]
        public ApiResponse<String> UpdateUserInfo(ReqInput data)
        {
            var response = new ApiResponse<String>();
            try
            {
                Models.User objUpdatePartnerUser = ReqHelper.GetRequestData<Models.User>(data);
                var Partner = new BLUser().UpdateUserInfo(objUpdatePartnerUser);
                //var filterObj = System.Web.HttpContext.Current.Session["trans"];
                APIRequestLog a =(APIRequestLog) System.Web.HttpContext.Current.Items["filterobj"];
                a.out_date_time = DateTime.Now;
                a.latency = (a.out_date_time - a.in_date_time).TotalMilliseconds;
                System.Web.HttpContext.Current.Items["filterobj"] = a;
                List<parterreturnapi> obj = new List<parterreturnapi>();
                if (Partner!=null)
                {
                    obj.Add(new parterreturnapi
                    {
                        pancard = Partner.pan,
                        transaction_id = a.transaction_id,
                        InDateTime = Convert.ToString(a.in_date_time),
                        OutDateTime = Convert.ToString(a.out_date_time),
                        latency = Convert.ToString(a.latency)
                    });
                
                    JavaScriptSerializer jsobj=new JavaScriptSerializer();
                    var results = jsobj.Serialize(obj);
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "User info Updated successfully!";
                    response.results = results;
                  
                }
                else
                {
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = "User not Found";
                    response.results = null;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateUserInfo()", "Services Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

    }


    public class parterreturnapi
    {
        public string transaction_id { get; set; }
        public string pancard { get; set; }
        public string InDateTime { get; set; }
        public string OutDateTime { get; set; }
        public string latency { get; set; }

    }
}
