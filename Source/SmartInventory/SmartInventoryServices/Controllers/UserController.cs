using BusinessLogics;
using Models;
using Models.API;
using Newtonsoft.Json;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Utility;
namespace SmartInventoryServices.Controllers
{
    //[Authorize]
    [CustomAuthorization]
    [APIExceptionFilter]
    public class UserController : ApiController
    {
        [HttpPost]
        public ApiResponse<string> UserLogout(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                UserLogOutIn objUserLogOutIn = ReqHelper.GetRequestData<UserLogOutIn>(data);
                if (!string.IsNullOrWhiteSpace(objUserLogOutIn.IsMasterLogin) && objUserLogOutIn.IsMasterLogin.ToUpper() == "FALSE")
                {
                    new BLUserLogin().UpdateLogOutTime(objUserLogOutIn.userId, "Mobile");
                }
                response.status = StatusCodes.OK.ToString();

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UserLogout()", "User Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<string> SaveUserLocation(ReqInput userData)
        {
            var response = new ApiResponse<string>();
            try
            {
                List<UserLocationTracking> objTrackingDetail = ReqHelper.GetRequestData<List<UserLocationTracking>>(userData);

                if (ModelState.IsValid)
                {
                    if (!new BLUserLocationTracking().SaveUserLocation(Newtonsoft.Json.JsonConvert.SerializeObject(objTrackingDetail)))
                    {
                        response.error_message = "Data is not inserted.";
                        response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                    }
                    else
                    {
                        response.status = StatusCodes.OK.ToString();
                    }
                }
                else
                {
                    response.error_message = ModelState.Values.ToString();
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                }
                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveUserLocation()", "User Controller", userData.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<String> ValidateUser(ReqInputs data)
        {
            var response = new ApiResponse<String>();
            bool isValidRequest = true;
            try
            {
                validateUser objvalidateuser = ReqHelpers.GetRequestData<validateUser>(data);
                if (isValidRequest && (string.IsNullOrEmpty(objvalidateuser.user_name)))
                {
                    response.status = StatusCodes.REQUIRED.ToString();
                    response.error_message = "User Name is required!";
                    response.results = null;
                    isValidRequest = false;
                }
                else if (isValidRequest && (string.IsNullOrEmpty(objvalidateuser.user_token)))
                {
                    response.status = StatusCodes.REQUIRED.ToString();
                    response.error_message = "User Token is required!";
                    response.results = null;
                    isValidRequest = false;
                }
                else if (isValidRequest && (string.IsNullOrEmpty(objvalidateuser.integration_source)))
                {
                    response.status = StatusCodes.REQUIRED.ToString();
                    response.error_message = "Integration Source is required!";
                    response.results = null;
                    isValidRequest = false;
                }
                if (isValidRequest)
                {
                    var validateuser = new BLUserLogin().ValidateUser(objvalidateuser.user_name, objvalidateuser.user_token, objvalidateuser.fsa_id, objvalidateuser.integration_source);
                    if (validateuser.status)
                    {
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = validateuser.message;
                        response.results = null;

                    }
                    else
                    {
                        response.status = StatusCodes.INVALID_INPUTS.ToString();
                        response.error_message = validateuser.message;
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

        [Route("User/v1.0/UpdateFCMKey")]
        [HttpPost]
        public ApiResponse<String> UpdateFCMKey(fcmKeyInfo fcmkeyinfo)
        {

            var response = new ApiResponse<String>();
            var req = JsonConvert.SerializeObject(fcmkeyinfo);
            bool isValidRequest = true;
            try
            {
                Models.User user = new BLUser().getUserDetails(fcmkeyinfo.userId);
                if (user == null)
                {
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = "Invalid UserId";
                    response.results = null;
                    isValidRequest = false;
                    return response;
                    //response.Add(new UpdateFCMKeyApiResponse<dynamic>() { status = "Failed", error_message = "Invalid UserId", result = null });
                }

                if (string.IsNullOrEmpty(fcmkeyinfo.fcmKey))
                {
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = "Invalid FCMKey";
                    response.results = null;
                    isValidRequest = false;
                    return response;
                    //response.Add(new UpdateFCMKeyApiResponse<dynamic>() { status = "Failed", error_message = "Invalid UserId", result = null });
                }

                if (isValidRequest)
                {
                    new BLUser().UpdateFCMKey(fcmkeyinfo.userId, fcmkeyinfo.fcmKey);
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "FCMKey Updated Successfully";
                    response.results = null;
                    //response.Add(new UpdateFCMKeyApiResponse<dynamic>() { status = "OK", error_message = "FCMKey Updated Successfully", result = null });
                    //return this.Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    response.status = StatusCodes.REQUEST_DENIED.ToString();
                    response.error_message = "Failed to update FCMKey. Please check your input.";
                    response.results = null;
                    // return this.Request.CreateResponse(HttpStatusCode.BadRequest, response);
                }


            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateFCMKey()", "Service Controller", req, ex);
                // return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
            return response;
        }
    }
}