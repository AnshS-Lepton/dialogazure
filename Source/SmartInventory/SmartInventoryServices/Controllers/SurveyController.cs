using BusinessLogics.API;
using Models;
using Models.API;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Utility;
namespace API.Controllers.SurveyArea
{
   // [Authorize]
   [CustomAuthorization]
    [APIExceptionFilter]
    [RoutePrefix("api/surveyarea")]
    public class SurveyAreaController : ApiController
    {
        [Authorize]
        [Route("SurveyareaInfo")]
        [HttpPost]
        public ApiResponse<IEnumerable<Models.API.SurveyArea>> GetSurveyAreaInfo(ReqInput data)
        {
            var response = new ApiResponse<IEnumerable<Models.API.SurveyArea>>();
            try
            {
                GetSurveyAreaInfoIn objSurveyAreaInfoIn = ReqHelper.GetRequestData<GetSurveyAreaInfoIn>(data);
                List<Models.API.SurveyArea> obj = new BusinessLogics.API.BLAPISurveyArea().getSurveyAreaById(objSurveyAreaInfoIn.userid);
                if (obj == null)
                {
                    response.error_message = "Opps! something went wrong.";
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                }
                else
                {
                    response.results = obj;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetSurveyAreaInfo()", "Survey Area Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

        [Authorize]
        [Route("SurveyInfoStatusWise")]
        [HttpPost]
        public ApiResponse<IEnumerable<Models.API.SurveyArea>> GetSurveyInfoStatusWise(SurveyStatus ObjRequest)
        {
            var resp = new ApiResponse<IEnumerable<Models.API.SurveyArea>>();

            try
            {
                var obj = new BusinessLogics.API.BLAPISurveyArea().getSurveyAreaById(ObjRequest.userid, ObjRequest.status);
                if (obj == null)
                {
                    resp.error_message = "Opps! something went wrong.";
                    resp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                }
                else
                {
                    resp.results = obj;
                    resp.status = StatusCodes.OK.ToString();
                    resp.error_message = string.Empty;
                }
            }
            catch
            {
                // LogHelper.GetInstance.WriteLog("UsersController => GetUserDetails", ex);
            }

            return resp;
        }

        [Authorize]
        [Route("SurveyInfobyBuildingid")]
        [HttpPost]
        public ApiResponse<IEnumerable<Models.API.SurveyArea>> GetSurveyInfobyBuildingid(ReqInput data)
        {
            var response = new ApiResponse<IEnumerable<Models.API.SurveyArea>>();

            try
            {
                SurveyInfobyBuildingidIn objSurveyInfobyBuildingidIn = ReqHelper.GetRequestData<SurveyInfobyBuildingidIn>(data);
                if (objSurveyInfobyBuildingidIn.userId == 0)
                {
                    response.error_message = "Invalid User id!";
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                }
                else if (objSurveyInfobyBuildingidIn.buildingId == 0)
                {
                    response.error_message = "Invalid Building id!";
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                }
                var obj = new BLAPISurveyArea().GetSurveyInfobyBuildingid(objSurveyInfobyBuildingidIn.buildingId, objSurveyInfobyBuildingidIn.userId);
                if (obj == null)
                {
                    response.error_message = "Opps! something went wrong.";
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                }
                else
                {
                    response.results = obj;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetSurveyInfobyBuildingid()", "Survey Area Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }

    }
}