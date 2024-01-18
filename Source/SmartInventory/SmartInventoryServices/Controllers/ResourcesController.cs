using ApplicationConfig;
using BusinessLogics;
using Lepton.Entities;
using Models;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [RoutePrefix("api/Resources")]
    [HandleException]
    public class ResourcesController : ApiController
    {
        #region GetLanguageResources
        /// <summary>
        /// Get Language Resources Before Login
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Selected Language</returns>
        [HttpPost]
        public ApiResponse<dynamic> GetLanguageResources(ReqInput data)
        {
            ResourcesRequestIn resourcesRequestIn = ReqHelper.GetRequestData<ResourcesRequestIn>(data);
            ResLangKeyListInfo objResLangKeyListInfo = new ResLangKeyListInfo();
            var value = "";
            var response = new ApiResponse<dynamic>();
            try
            {
                if (AppConfig.MobileResourcesKeyPassword != resourcesRequestIn.MobileResourcesKeyPassword)
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = "ResourcesKey not matched";
                }
                else
                {
                    Models.API.HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
                    ResLangKeyListParam obj = new ResLangKeyListParam();
                    obj.purpose = "Mobile";
                    if (string.IsNullOrEmpty(headerAttribute.language))
                        obj.language = "en";
                    else
                    obj.language = headerAttribute.language;
                    objResLangKeyListInfo = new BLResources().GetResLangKeyListInfo(obj);
                    value = new BLGlobalSetting().GetSignUpKeyForBuildingSurvey();
                    objResLangKeyListInfo.IsSignUpAllowed= value == "1" ? "true" :"false";
                    objResLangKeyListInfo.policies = ApplicationSettings.Policies;
                    objResLangKeyListInfo.TermsAndConditions = ApplicationSettings.TermsConditions;
                    objResLangKeyListInfo.isPatternLockEnable = ApplicationSettings.isPatternLockEnable;
                    response.status = ResponseStatus.OK.ToString();
                    response.results =objResLangKeyListInfo;
                    response.error_message = "Success";                   
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetLangugeResources()", "Resources Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
    }
}
