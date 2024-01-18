using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Models;
using Models.API;
using Newtonsoft.Json;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using Utility;
using BusinessLogics;
using System.Configuration;


namespace SmartInventoryServices.Controllers
{
    public class OTPController : ApiController
    {

        [HttpPost]
        [CustomAuthorizationOTP]
        public ApiResponse<OTPResponse> GetOTP(ReqInput data)
        {
            var response = new ApiResponse<OTPResponse>();
            OTPResponse objOTPResponse = new OTPResponse();
            try
            {
                NewOTPRequest objNewOTPRequest = ReqHelper.GetRequestData<NewOTPRequest>(data);
                if (!string.IsNullOrWhiteSpace(objNewOTPRequest.user_name))
                {
                    var isWFMSMS = Convert.ToBoolean(ConfigurationManager.AppSettings["isWFMSMS"]);

                    objOTPResponse = new BLUserOTPDetails().GenerateNewOTP(objNewOTPRequest.user_id, objNewOTPRequest.user_name, objNewOTPRequest.source);
                    objOTPResponse.message = Resources.Helper.MultilingualMessageConvert(objOTPResponse.message);
                    if (objOTPResponse.is_OTP_generated)
                    {
                        objNewOTPRequest.otp_channel = string.IsNullOrEmpty(objNewOTPRequest.otp_channel) ? "mobile" : objNewOTPRequest.otp_channel;
                        bool btOTPSentStatus = false;
                        if (objNewOTPRequest.otp_channel == "email")
                        {
                            //string subject = "OTP-Verification Email";
                            //string mailBody = "Hi " + objOTPResponse.user_name + ",<br/> Your OTP for user login is: " + objOTPResponse.OTP;
                            //string mailSentMsg = "";
                            //btOTPSentStatus = new BLUserOTPDetails().SendOTPToUserEmail(objOTPResponse.user_email, subject, mailBody,out mailSentMsg);
                            if (!isWFMSMS)
                            {
                                btOTPSentStatus = new BLUserOTPDetails().SendOTPToUserMobile(objNewOTPRequest.otp_channel, objOTPResponse.mobile_no, objOTPResponse.user_email, objOTPResponse.OTP);
                            }

                        }
                        else if (objNewOTPRequest.otp_channel == "mobile")
                        {

                            if (isWFMSMS)
                            {
                                string responseMessage;
                                //  btOTPSentStatus = Models.WFM.SMS.SendSms(objOTPResponse.mobile_no, objOTPResponse.OTP);
                                btOTPSentStatus = Models.WFM.SMS.SendSms(objOTPResponse, MiscHelper.Decrypt(objOTPResponse.mobile_no), objOTPResponse.OTP, out responseMessage);
                                objOTPResponse.message = responseMessage;

                            }
                            else
                            {
                                //sed
                                btOTPSentStatus = new BLUserOTPDetails().SendOTPToUserMobile(objNewOTPRequest.otp_channel, objOTPResponse.mobile_no, objOTPResponse.user_email, objOTPResponse.OTP);
                            }
                        }

                        if (!btOTPSentStatus)
                        {
                            new BLUserOTPDetails().ResetUserOTPStatus(objNewOTPRequest.user_id, objNewOTPRequest.user_name, OTPResetType.RESET_RESEND_OTP_FAILED.ToString());
                            objOTPResponse.OTP = null;
                            objOTPResponse.otp_resend_limit_left = objOTPResponse.otp_resend_limit_left - 1;
                            objOTPResponse.is_OTP_generated = false;
                            objOTPResponse.message = "SMS Service Down OTP not sent on  : " + MiscHelper.Decrypt(objOTPResponse.mobile_no);
                        }
                        objOTPResponse.OTP = null;
                    }
                    response.status = StatusCodes.OK.ToString();
                    response.results = objOTPResponse;
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "Invalid Input Values";
                }

                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetOTP()", "OTP Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        //[CustomAuthorizationOTP]
        public ApiResponse<VerifyOTPResponse> VerifyOTP(ReqInput data)
        {
            var response = new ApiResponse<VerifyOTPResponse>();
            VerifyOTPResponse objVerifyOTPResponse = new VerifyOTPResponse();
            try
            {
                ValidateOTPRequest objValidateOTPRequest = ReqHelper.GetRequestData<ValidateOTPRequest>(data);
                if (!string.IsNullOrWhiteSpace(objValidateOTPRequest.user_name) && !string.IsNullOrWhiteSpace(objValidateOTPRequest.OTP))
                {
                    objVerifyOTPResponse = new BLUserOTPDetails().VerifyOTP(objValidateOTPRequest.user_id, objValidateOTPRequest.user_name, objValidateOTPRequest.OTP, objValidateOTPRequest.source);
                    objVerifyOTPResponse.message = Resources.Helper.MultilingualMessageConvert(objVerifyOTPResponse.message);
                    objVerifyOTPResponse.footer_message = Resources.Helper.MultilingualMessageConvert(objVerifyOTPResponse.footer_message);
                    response.status = StatusCodes.OK.ToString();
                    response.results = objVerifyOTPResponse;
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "Invalid Input Values";
                }

                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("VerifyOTP()", "OTP Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }


        [HttpPost]
        
        public ApiResponse<GetOTPInternally> GetDataUserWise(ReqInput data)
        {
            var response = new ApiResponse<GetOTPInternally>();
            GetOTPInternally objVerifyOTPResponse = new GetOTPInternally();
            try
            {
                GetOTPInternally objValidateOTPRequest = ReqHelper.GetRequestData<GetOTPInternally>(data);
                if (!string.IsNullOrWhiteSpace(objValidateOTPRequest.user_name) && !string.IsNullOrWhiteSpace(objValidateOTPRequest.organization) && objValidateOTPRequest.organization.ToLower() == "lepton")
                {
                    objVerifyOTPResponse = new BLUserOTPDetails().GetOTPInternally(objValidateOTPRequest.user_name);                  
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = objVerifyOTPResponse != null? objVerifyOTPResponse.otp.ToString():"OTP not found for user";                  
                }
                else
                {
                    response.status = StatusCodes.INVALID_INPUTS.ToString();
                    response.error_message = "Invalid Input Values";
                }

                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetOTPInternally()", "OTP Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
    }
}