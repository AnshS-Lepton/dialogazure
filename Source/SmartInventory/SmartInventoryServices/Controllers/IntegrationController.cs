using BusinessLogics;
using Models;
using Models.API;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [RoutePrefix("api/integration")]
    [HandleException]
    public class IntegrationController : ApiController
    {
        [Route("smartplannerintegration")]
        [HttpPost]
        public ApiResponse<string> IntegrationWithSmartPlanner(List<IntegrationSuperSet> listIntegrationSuperSet)
        {
            var response = new ApiResponse<string>();
            try
            {

                if (listIntegrationSuperSet != null)
                {
                    var deleteStatus = new BLIntegrationSuperSet().DeleteIntegrationSuperset(listIntegrationSuperSet[0].plan_id);

                    if (Convert.ToBoolean(deleteStatus[0].status) == false)
                    {
                        new BLIntegrationSuperSet().SaveIntegrationSuperSet(listIntegrationSuperSet);
                        response.error_message = "Integration superset data inserted successfully";
                        response.status = StatusCodes.OK.ToString();
                    }
                    else
                    {
                        response.error_message = deleteStatus[0].message;
                        response.status = StatusCodes.OK.ToString();
                    }
                }
                else
                {
                    response.error_message = "Request input data is null";
                    response.status = StatusCodes.INVALID_REQUEST.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("IntegrationWithSmartPlanner()", "Integration Controller", JsonConvert.SerializeObject(listIntegrationSuperSet), ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [Route("isvaliduser")]
        [HttpGet]
        public ApiResponse<string> ChkUserExist(string userName, string email)
        {
            var response = new ApiResponse<string>();
            try
            {

                var userStatus = new BLUser().ChkUserExist(email, userName);
                if (userStatus)
                {
                    response.status = StatusCodes.OK.ToString();
                }
                else
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "SmartPlanner login user does not exist in SmartInventory. Please contact to administrator.";
                }

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ChkUserExist()", "Integration Controller", userName + " " + email, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        #region Update the NE ID and NE Code on JFP System
        [HttpPost]
        public ApiResponse<dynamic> UpdateTargetDetails(ReqInput data)
        {
            
            var response = new ApiResponse<dynamic>();
            try
            {
                string jsonFormattedString = data.data.Replace("\\\"", "\"");
                UpdateTargetDetails updateTargetDetails = new JavaScriptSerializer().Deserialize<UpdateTargetDetails>(jsonFormattedString);
                if (updateTargetDetails.TargetRefID == 0) {
                    response.error_message = "Target ref id is required!";
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    return response;
                }
                if (string.IsNullOrEmpty(updateTargetDetails.TargetRefCode))
                {
                    response.error_message = "Target ref code is required!";
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    return response;
                }
                if (string.IsNullOrEmpty(updateTargetDetails.DesignID))
                {
                    response.error_message = "Design id is required!";
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    return response;
                }
                if (string.IsNullOrEmpty(updateTargetDetails.CategoryName))
                {
                    response.error_message = "Category name is required!";
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    return response;
                }

                new BLIntegrationSuperSet().BLUpdateTargetDetails(updateTargetDetails.TargetRefID, updateTargetDetails.TargetRefCode, updateTargetDetails.DesignID, updateTargetDetails.ClassName, updateTargetDetails.CategoryName);
                response.error_message = "Target details updated successfully";
                response.status = StatusCodes.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateTargetDetails()", "Integration Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
    }
}