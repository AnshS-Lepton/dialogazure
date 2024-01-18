using BusinessLogics;
using BusinessLogics.API;
using Models;
using Models.API;
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
    public class JobPackController : ApiController
    {
        [HttpPost]
        public ApiResponse<List<JobPackMaster>> GetAssignedJobPacks(ReqInput data)
        {
            var response = new ApiResponse<List<JobPackMaster>>();
            try
            {
                JobPackIn objJobPackIn = ReqHelper.GetRequestData<JobPackIn>(data);

                List<JobPackMaster> lstJobPacks = new BLJobPack().GetAssignedJobs(objJobPackIn.userId,objJobPackIn.status);
                if (lstJobPacks == null)
                {
                    response.error_message = "Opps! something went wrong.";
                    response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                }
                else
                {
                    response.results = lstJobPacks;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAssignedJobPacks()", "Job Pack Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<JobPackMaster> getJobPackDetail(ReqInput data)
        {
            var response = new ApiResponse<JobPackMaster>();
            try
            {
                JobPackIn objJobPackIn = ReqHelper.GetRequestData<JobPackIn>(data);

                JobPackMaster objJobPack = new BLJobPack().getJobPackDetail(objJobPackIn.systemId);
                if (objJobPack == null)
                {
                    response.error_message = "Job Pack does not exist!";
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                }
                else
                {
                    response.results = objJobPack;
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getJobPackDetail()", "Job Pack Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        [HttpPost]
        public ApiResponse<string> UpdateJobPackStage(ReqInput data)
        {
            var response = new ApiResponse<string>();
            try
            {
                JobPackIn objJobPackIn = ReqHelper.GetRequestData<JobPackIn>(data);

                JobPackMaster objExisitngJobPack = new BLJobPack().getJobPackDetail(objJobPackIn.systemId);
                if (objExisitngJobPack == null)
                {
                    response.error_message = "Job Pack does not exist!";
                    response.status = StatusCodes.ZERO_RESULTS.ToString();
                }
                else
                {
                    objExisitngJobPack.stage1 = objJobPackIn.stage1;
                    objExisitngJobPack.stage2 = objJobPackIn.stage2;
                    objExisitngJobPack.stage3 = objJobPackIn.stage3;
                    //get job pack detail...
                    if (!string.IsNullOrEmpty(objJobPackIn.stage1))
                    {                      
                        objExisitngJobPack.status = "Stage1";
                    }
                    if (!string.IsNullOrEmpty(objJobPackIn.stage2))
                    {                      
                        objExisitngJobPack.status = "Stage2";
                    }
                    if (!string.IsNullOrEmpty(objJobPackIn.stage3))
                    {                     
                        objExisitngJobPack.status = "Stage3";
                    } 
                    if(objJobPackIn.isCompleted)
                    {
                        objExisitngJobPack.status = "Completed";
                    }                   
                                  
                    var objJobPack = new BLJobPack().UpdateJobPackStage(objExisitngJobPack, objJobPackIn.userId);
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("UpdateJobPackStage()", "Job Pack Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

    }
}
