using BusinessLogics;
using Models;
using Models.API;
using SmartInventoryServices.Filters;
using System;
using System.Web.Http;
using Utility;
using System.Text.RegularExpressions;

namespace SmartInventoryServices.Controllers
{
    [AllowAnonymous]
    [HandleException]
    public class SignInController : ApiController
    {
        [HttpPost]
        public ApiResponse<Models.User> SaveUser(ReqInputs data)
        {

            var response = new ApiResponse<Models.User>();
            Models.User objUser = ReqHelpers.GetRequestData<Models.User>(data);
            objUser.application_access = "MOBILE";
            objUser.user_type = "partner";
            objUser.is_active = true;
            objUser.manager_id = 1;
            objUser.role_id = 2;
            bool isEmailIdValid = ChkUserEmailId(objUser);
            var objUserExist = new BLUser().ChkUserExist(objUser);
            bool isMobileNumberValid = ChkUserMobileNumber(objUser);
            if (objUserExist != null)
            {
                objUserExist.user_email = MiscHelper.Decrypt(objUserExist.user_email);
                objUser.user_email = MiscHelper.Decrypt(objUser.user_email);
            }
            if ((objUserExist != null && objUser.user_id == 0) || (objUserExist != null && objUser.user_id != objUserExist.user_id))
            {
                response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                if (objUserExist.user_name.Trim().ToLower() == objUser.user_name.Trim().ToLower())
                    response.error_message = "User Name already exist!";
                else if (string.IsNullOrEmpty(objUserExist.user_email))
                    response.error_message = "Please enter email Id!";
                else if (objUserExist.user_email.Trim().ToLower() == objUser.user_email.Trim().ToLower())
                    response.error_message = "Email Id already exist!";
            }
            else if ((string.IsNullOrEmpty(objUser.user_name)))
            {
                response.status = StatusCodes.REQUIRED.ToString();
                response.error_message = "User Name is required!";
            }
            else if (string.IsNullOrEmpty(objUser.name))
            {
                response.status = StatusCodes.REQUIRED.ToString();
                response.error_message = "Name is required!";
            }
            else if (string.IsNullOrEmpty(objUser.user_email))
            {
                response.status = StatusCodes.REQUIRED.ToString();
                response.error_message = "User Email is required!";
            }
            else if (isEmailIdValid == false)
            {
                response.status = StatusCodes.VALIDATION_FAILED.ToString();
                response.error_message = "Please enter a valid emailId!";
            }
            else if (string.IsNullOrEmpty(objUser.password))
            {
                objUser.password = Convert.ToString(Utility.MiscHelper.EncodeTo64(string.IsNullOrWhiteSpace(objUser.password) ? "12345678" : objUser.password));
                response.status = StatusCodes.REQUIRED.ToString();
                response.error_message = "Password is required!";
            }
            else if (string.IsNullOrEmpty(objUser.mobile_number))
            {
                response.status = StatusCodes.REQUIRED.ToString();
                response.error_message = "Mobile Number is required!";
            }
            else if (objUser.mobile_number.Length < 10 || objUser.mobile_number.Length > 10 || isMobileNumberValid == false)
            {
                response.status = StatusCodes.VALIDATION_FAILED.ToString();
                response.error_message = "Please enter a valid mobile number!";
            }
            else
            {
                var isNew = objUser.user_id > 0 ? false : true;
                objUser.password = Convert.ToString(Utility.MiscHelper.EncodeTo64(string.IsNullOrWhiteSpace(objUser.password) ? "12345678" : objUser.password));
                objUser = new BLUser().SaveMobileUser(objUser, objUser.user_id);
                if (isNew)
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "User created successfully!";
                    response.results = objUser;
                }
                else
                {
                    response.status = StatusCodes.OK.ToString();
                    response.error_message = "User updated successfully!";
                    response.results = objUser;
                }
            }
            return response;
        }
        public bool ChkUserEmailId(Models.User objUser)
        {
            try
            {
                return Regex.IsMatch(objUser.user_email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            }
            catch
            {
                throw;
            }
        }
        public bool ChkUserMobileNumber(Models.User objUser)
        {
            try
            {

                return Regex.IsMatch(objUser.mobile_number, "[0-9]{10}");
            }
            catch
            {
                throw;
            }
        }
    }
}