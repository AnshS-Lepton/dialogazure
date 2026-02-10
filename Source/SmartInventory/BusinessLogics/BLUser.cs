using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Models;
using Models.WFM;
using DataAccess;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security;
//using Thinktecture.IdentityModel.Constants;
using Thinktecture.IdentityModel.WSTrust;

namespace BusinessLogics
{
    public class BLUser
    {
        DAUser objDAUser = new DAUser();
        public User ValidateUser(string username, string password, string userType)
        {
            return objDAUser.ValidateUser(username, password, userType);
        }
        public static int getnotificationcount(int userid)
        {
            return new DAUser().GetNotificationCount(userid);
        }


        public static string updateNotification(string Id)
        {
            return new DAUser().updateNotification(Id);
        }

        public static string UpdateNotificationForFastLane(string NotificationType, string JobOrderId, string IsMailSent)
        {
            return new DAUser().UpdateNotificationForFastLane(NotificationType, JobOrderId, IsMailSent);
        }
        public User getUserDetails(int id)
        {
            return objDAUser.getUserDetails(id);
        }
        public void saveChangePassword(string password, int user_id)
        {
            objDAUser.saveChangePassword(password, user_id);
        }
        public User ChkUserExist(string username)
        {
            return objDAUser.ChkUserExist(username);
        }
        public User validateUserLoginHistory(string username)
        {
            return objDAUser.validateUserLoginHistory(username);
        }

        public static List<wfm_notification> GetNotificationDetail(ViewNotificationFilter ViewNotificationFilter, out int totalRecords)
        {
            return new DAUser().GetNotificationDetail(ViewNotificationFilter, out totalRecords);
        }

        public bool ChkUserExist(string email, string userName)
        {
            return objDAUser.ChkUserExist(email, userName);
        }
        public static ADFSDetail AuthenticateADFS(ADFSInput objADFSInput)
        {

            WSTrustChannelFactory factory = null;
            ADFSDetail ADFSDetail = null;
            try
            {

                factory = new WSTrustChannelFactory(
                new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential),
                new EndpointAddress(objADFSInput.ADFSEndPoint));

                factory.TrustVersion = TrustVersion.WSTrust13;

                // Username and Password here...             
                if (!string.IsNullOrEmpty(objADFSInput.ADFSAutheticationBasedOn)
                    && objADFSInput.ADFSAutheticationBasedOn.ToUpper() == "USERNAME")
                {
                    factory.Credentials.UserName.UserName = objADFSInput.ADFSUserNamePreFix + objADFSInput.user_name;
                }
                else
                {
                    factory.Credentials.UserName.UserName = objADFSInput.user_email;
                }
                factory.Credentials.UserName.Password = objADFSInput.password;

                RequestSecurityToken rst = new RequestSecurityToken
                {
                    RequestType = RequestTypes.Issue,
                    AppliesTo = new EndpointReference(objADFSInput.ADFSRelPartyUri),
                    KeyType = KeyTypes.Bearer,
                };

                IWSTrustChannelContract channel = factory.CreateChannel();
                RequestSecurityTokenResponse rstr;
                SecurityToken token = channel.Issue(rst, out rstr);

                ADFSDetail = new ADFSDetail() { tokenId = token.Id, validFrom = token.ValidFrom, validTo = token.ValidTo };

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("ID3242"))
                {
                    ADFSDetail = new ADFSDetail() { errorMsg = "Invalid ADFS username or password" };
                }
                else if (ex.Message.Contains("ID3082"))
                {
                    ADFSDetail = new ADFSDetail() { errorMsg = "Invalid ADFS uri configuration" };
                }
                else
                {
                    ADFSDetail = new ADFSDetail() { errorMsg = "Unable to access ADFS service. Please contact administrator!" };
                }
                ADFSDetail.ADFSException = ex;
            }
            finally
            {
                if (factory != null)
                {
                    try
                    {
                        factory.Close();
                    }
                    catch (CommunicationObjectFaultedException)
                    {
                        factory.Abort();
                    }
                }
            }
            return ADFSDetail;


        }

        public List<UserDetail> GetAllActiveUsersList()
        {
            return new DAUser().GetAllActiveUsersList();

        }
        public List<User> GetAllUsersList()
        {
            return new DAUser().GetAllUsersList();

        }
        public string UpdateUserInfo(List<User> user)
        {
            return new DAUser().UpdateUserInfo(user);
        }

        public List<UserReportDetail> GetUserReportDetailsList(string user_id, bool isShowSelected)
        {
            return new DAUser().GetUserReportDetailsList(user_id, isShowSelected);
        }
        public List<UserReportDetail> GetUserLocationTrackingList(string user_id, int role_id)
        {
            return new DAUser().GetUserLocationTrackingList(user_id, role_id);
        }
        public List<User> GetUserReportDetailsList(string user_id)
        {
            return new DAUser().GetUserReportDetailsList(user_id);
        }


        #region add User
        public List<KeyValueDropDown> GetAllRole(int role_id, int user_id)
        {
            return new DAUser().GetAllRole(role_id, user_id);

        }
        public List<KeyValueDropDown> GetUsernameDetails()
        {
            return new DAUser().GetUsernameDetails( );

        }
        public List<KeyValueDropDown> GetUsernameDetails(int id)
        {
            return new DAUser().GetUsernameDetails(id);

        }



        #region sapna

        public List<KeyValueDropDown> BindReportingManager(int RoleId, int user_id)
        {
            return new DAUser().BindReportingManager(RoleId, user_id);
        }
        public List<KeyValueDropDown> BindFETool( int user_id)
        {
            return new DAUser().BindFETool(user_id);
        }
        public List<KeyValueDropDown> BindFETooldropdown(int user_id)
        {
            return new DAUser().BindFETooldropdown(user_id);
        }
        

        public List<KeyValueDropDown> BindFETool()
        {
            return new DAUser().BindFETool();
        }
        #endregion
        public List<KeyValueDropDown> BindWarehouseCode()
        {
            return new DAUser().BindWarehouseCode();
        }

        public User SaveUser(User objUser, int userId)
        {
            return new DAUser().SaveUser(objUser, userId);


        }
        public User SaveMobileUser(User objUser, int userId)
        {
            return new DAUser().SaveMobileUser(objUser, userId);
        }
        public DbMessage ChkUserCreationLimitExeeds(int userId, int maxWebLimit, int maxMobileLimit, string userType)
        {
            return new DAUser().ChkUserCreationLimit(userId, maxWebLimit, maxMobileLimit, userType);
        }

        public User ChkUserExist(User objUser)
        {
            return objDAUser.ChkUserExist(objUser);
        }
        public User ChkUserPanExist(User objUser)
        {
            return objDAUser.ChkUserPanExist(objUser);
        }
        public List<User> ChkDuplicateUserExist(User objUser)
        {
            return objDAUser.ChkDuplicateUserExist(objUser);
        }
        public List<UserDetail> GetUserList(CommonGridAttributes objGridAttributes, int role_id, int user_id)
        {
            return new DAUser().GetUserList(objGridAttributes, role_id, user_id);
        }
        public List<UserDetail> GetVendorList(CommonGridAttributes objGridAttributes, int role_id, int user_id)
        {
            return new DAUser().GetVendorList(objGridAttributes, role_id, user_id);
        }

        public List<userName> AwardSiteToSelectedVendor(int reference_id, int user_id,double vendorCost)
        {
            return new DAUser().AwardSiteToSelectedVendor(reference_id, user_id, vendorCost);
        }

        public List<User> GetUsersListByMGRIds(List<int> mgrIds)
        {
            return new DAUser().GetUsersListByMGRIds(mgrIds);
        }


        public List<UserLoginHistory> GetUserLoginHistoryList(CommonGridAttributes objGridAttributes, int role_id, int user_id)
        {
            return new DAUser().GetUserLoginHistoryList(objGridAttributes, role_id, user_id);
        }

        public User GetUserDetailByID(int id)
        {
            return new DAUser().GetUserDetailByID(id);
        }
        public List<User> GetPartnerUser()
        {
            return new DAUser().GetPartnerUser();
        }

        public int DeleteUserById(int id)
        {
            return new DAUser().DeleteUserById(id);

        }
        public List<userName> GetUserByManagerId(int user_id)
        {
            return new DAUser().GetUserByManagerId(user_id);
        }
        public List<AssignedBy> GetAllUsers(int user_id)
        {
            return new DAUser().GetAllUsers(user_id);
        }
        public User GetUserDetailByName(string name)
        {
            return new DAUser().GetUserDetailByName(name);
        }
        public User GetUserDetailByUserName(string username)
        {
            return new DAUser().GetUserDetailByUserName(username);
        }
        public List<User> CheckRoleAssignToUser(int role_id)
        {
            return new DAUser().CheckRoleAssignToUser(role_id);
        }
        public List<Dictionary<string, string>> GetUserHistoryDetailById(FilterHistoryAttr objhistoryParam)
        {
            return new DAUser().GetUserHistoryDetailById(objhistoryParam);
        }
        public bool EditUserStatus(int user_id, bool status)
        {
            return new DAUser().EditUserStatus(user_id, status);
        }
        #endregion
        public static List<User_Master> GetSubordinateDetails(int empId, string roleName)
        {
            return DAUser.GetSubordinateDetails(empId, roleName);
        }
        public static List<User_Master> GetMMSubordinateDetails(int empId, string roleName)
        {
            return DAUser.GetMMSubordinateDetails(empId, roleName);
        }
        public static List<User_Master> GetSubordinateContractorDetails(int empId, string roleName)
        {
            return DAUser.GetSubordinateContractorDetails(empId, roleName);
        }

        public static List<User_Master> GetSubordinateContractorDetailstt(int empId, string roleName)
        {
            return DAUser.GetSubordinateContractorDetailstt(empId, roleName);
        }


        public static VW_USER_MANAGER_RELATION GetUserManagerFcmKey(int userId)
        {
            return DAUser.GetUserManagerFcmKey(userId);
        }
        public User UpdateUserInfo(User objUser)
        {
            return new DAUser().UpdateUserInfo(objUser);


        }
        public static List<User_Master> GetSubordinateDetailstt(int empId, string roleName)
        {
            return DAUser.GetSubordinateDetailstt(empId, roleName);
        }

        public static List<TT_Type> GetttSubordinateDetailstt(string id)
        {
            return DAUser.GetttSubordinateDetailstt(id);
        }
        public User UpdateEmailPhone(int user_id, double mobile, string email)
        {
            return new DAUser().UpdateEmailPhone(user_id, mobile, email);

        }
        public User getUserDetailsByUserName(string userName)
        {
            return new DAUser().getUserDetailsByUserName(userName);
        }
        public List<EventEmailTemplateDetail> GetEventEmailTemplateDetail(string eventname)
        {
            return new DAUser().GetEventEmailTemplateDetail(eventname);
        }

        public List<KeyValueDropDown> GetManagerEmailId(int user_id)
        {
            return new DAUser().GetManagerEmailId(user_id);
        }
        public User UpdateFCMKey(int user_id, string fcmkey)
        {
            return new DAUser().UpdateFCMKey(user_id, fcmkey);

        }
    }

    public class BLUserLogin
    {
        public UserLogin SaveLoginHistory(UserLogin objUserLogin, string source)
        {
            return new DAUserLogin().SaveLoginHistory(objUserLogin, source);
        }
        public UserLogin UpdateBrowserInfo(UserLogin objUserLogin)
        {
            return new DAUserLogin().UpdateBrowserInfo(objUserLogin);
        }
        public bool UpdateLogOutTime(int userId, string source)
        {
            return new DAUserLogin().UpdateLogOutTime(userId, source);
        }
        public bool UpdateLogOutTime(int userId, int historyId, string source,string signOut_type=null)
        {
            return new DAUserLogin().UpdateLogOutTime(userId, historyId,source, signOut_type);
        }
        public UserLogin GetUserLoginDetailById(int userId, string Source)
        {
            return new DAUserLogin().GetUserLoginDetailById(userId, Source);
        }
        
        
        public bool UpdateRefreshToken(int userId, int loginId, string refreshToken)
        {
            return new DAUserLogin().UpdateRefreshToken(userId, loginId, refreshToken);
        }
        public DbMessage ValidateUser(string user_name, string user_token, string fsa_id, string integration_source)
        {
            return new DAUserLogin().ValidateUser(user_name, user_token, fsa_id, integration_source);
        }
        public int GetUserLoginHistoryId(int userId,string tokenSourceName)
        {
            return new DAUserLogin().GetUserLoginHistoryId(userId, tokenSourceName);
        }

        public string GetSourceName(int userId)
        {
            return new DAUserLogin().GetSourceName(userId);
        }
        public DbMessage LogoutUser(int userId, string application_access)
        {
            return new DAUserLogin().LogoutUser(userId, application_access);
        }
    }

    public class BLUserLocationTracking
    {
        public bool SaveUserLocation(string TrackingDetails)
        {
            return new DAUserLocationTracking().SaveUserLocation(TrackingDetails);
        }
        public UserLocationTracking GetUserCurrentLocation(int login_id)
        {
            return new DAUserLocationTracking().GetUserCurrentLocation(login_id);
        }
    }
    public class BLUserPermissionArea
    {
        public List<UserPermissionArea> GetUserPermissionArea(int userid)
        {
            return new DAUserPermissionArea().getUserPermissionArea(userid);
        }
        public int SaveUserPermissionArea(int userid, string provinceids, int createdby, string regionIds, string countryNames, string SubDistrictIds, string blockIds)
        {
            return new DAUserPermissionArea().SaveUserPermissionArea(userid, provinceids, createdby, regionIds, countryNames, SubDistrictIds, blockIds);
        }

    }
    public class BLRoleModuleMapping
    {
        public List<RoleModuleMapping> SaveRoleModuleMapping(List<RoleModuleMapping> lstUserModuleMapping, int role_id)
        {
            return new DARoleModuleMapping().SaveRoleModuleMapping(lstUserModuleMapping, role_id);
        }
        public List<RoleModuleMapping> GetRoleModuleMapping(int role_id)
        {
            return new DARoleModuleMapping().GetRoleModuleMapping(role_id);
        }
    }

    public class BLRoleServiceFacilityMapping
    {
        public List<RoleSeviceFacilityMapping> SaveRoleServiceFacilityMapping(List<RoleSeviceFacilityMapping> lstRoleServiceFacilityMapping, int role_id)
        {
            return new DARoleServiceFacilityMapping().SaveRoleServiceFacilityMapping(lstRoleServiceFacilityMapping, role_id);
        }
        public List<RoleSeviceFacilityMapping> GetRoleServiceFacilityMapping(int role_id)
        {
            return new DARoleServiceFacilityMapping().GetRoleSeviceFacilityMapping(role_id);
        }
    }

    public class BLRoleRoleJoTypeMapping
    {
        public List<RoleJoTypeMapping> SaveRoleJoTypeMapping(List<RoleJoTypeMapping> lstRoleJoTypeMapping, int role_id)
        {
            return new DARoleJoTypeMapping().SaveRoleJoTypeMapping(lstRoleJoTypeMapping, role_id);
        }
        public List<RoleJoTypeMapping> GetRoleServiceFacilityMapping(int role_id)
        {
            return new DARoleJoTypeMapping().GetRoleJoTypeMapping(role_id);
        }
    }
    public class BLRoleRoleJoCategoryMapping
    {
        public List<RoleJoCategoryMapping> SaveRoleJoCategoryMapping(List<RoleJoCategoryMapping> lstRoleJoCategoryMapping, int role_id)
        {
            return new DARoleJoCategoryMapping().SaveRoleJoCategoryMapping(lstRoleJoCategoryMapping, role_id);
        }
        public List<RoleJoCategoryMapping> GetRoleServiceFacilityMapping(int role_id)
        {
            return new DARoleJoCategoryMapping().GetRoleJoCategoryMapping(role_id);
        }
    }

    public class BLUserReportMapping
    {
        public List<UserReportMappingVw> SaveUserReportMapping(List<UserReportMapping> lstUserReportMapping, int user_id,bool is_all_users_mapped)
        {
            return new DAUserReportMapping().SaveUserReportMapping(lstUserReportMapping, user_id, is_all_users_mapped);
        }
        public List<UserReportMapping> GetUserReportMapping(int user_id)
        {
            return new DAUserReportMapping().GetUserReportMapping(user_id);
        }
    }

    public class BLUserModuleMapping
    {
        public List<UserModuleMapping> SaveModuleMapping(List<UserModuleMapping> lstUserModuleMapping, int user_id)
        {
            return new DAUserModuleMapping().SaveModuleMapping(lstUserModuleMapping, user_id);
        }
        public List<UserModuleMapping> GetModuleMapping(int User_id)
        {
            return new DAUserModuleMapping().GetModuleMapping(User_id);
        }
    }


    public class BLUserServiceFacilityMapping
    {
        public List<UserSeviceFacilityMapping> SaveUserServiceFacilityMapping(List<UserSeviceFacilityMapping> lstUserSeviceFacilityMapping, int user_id)
        {
            return new DAUserServiceFacilityMapping().SaveUserServiceFacilityMapping(lstUserSeviceFacilityMapping, user_id);
        }
        //public List<UserModuleMapping> GetModuleMapping(int user_id)
        //{
        //    return new DAUserModuleMapping().GetModuleMapping(user_id);
        //}
    }

    public class BLUserJoTypeMapping
    {
        public List<UserJoTypeMapping> SaveUserJoTypeMapping(List<UserJoTypeMapping> lstUserJoTypeMapping, int user_id)
        {
            return new DAUserJoTypeMapping().SaveUserJoTypeMapping(lstUserJoTypeMapping, user_id);
        }
        //public List<UserModuleMapping> GetModuleMapping(int user_id)
        //{
        //    return new DAUserModuleMapping().GetModuleMapping(user_id);
        //}
    }

    public class BLUserLoginHistoryInfo
    {
        public bool UpdateUserLogOutTime(int userId, int historyId,string signOut_type=null)
        {
            return new DAUserLoginHistoryInfo().UpdateUserLogOutTime(userId, historyId, signOut_type);
        }

    }
    #region BulkUserUpload
    public class BLBulkUserUploadSummary
    {
        public BulkUserUploadSummary SaveBulkUserUploadSummary(BulkUserUploadSummary bulkUserUploadSummary)
        {
            return new DABulkUserUploadSummary().SaveBulkUserUploadSummary(bulkUserUploadSummary);
        }

        public List<BulkUserUploadSummary> GetBulkUserUploadSummary(CommonGridAttributes commonGridAttributes)
        {
            return new DABulkUserUploadSummary().GetBulkUserUploadSummary(commonGridAttributes);
        }

        public BulkUserUploadLimit BulkUploadUserCheckLimit(int userUploadId, int webMaxUser, int mobileMaxUser)
        {
            return new DABulkUserUploadSummary().BulkUploadUserCheckLimit(userUploadId, webMaxUser, mobileMaxUser);
        }
     

    }
    public class BLBulkUserUploadManagerMapping
    {
        public void SaveUserUploadManagerMapping(List<BulkUserUploadManagerMapping> bulkUserUploadManagerMappings)
        {
            new DABulkUserUploadManagerMapping().SaveBulkUserUploadManagerMapping(bulkUserUploadManagerMappings);
        }
    }
    public class BLBulkUserUpload
    {
        public void SaveBulkUserUpload(List<BulkUserUpload> bulkUserUpload)
        {
            new DABulkUserUpload().SaveBulkUserUpload(bulkUserUpload);
        }
        public int ProcessBulkUserUpload(int user_upload_id)
        {
            return new DABulkUserUpload().ProcessBulkUserUpload(user_upload_id);
        }

        public DataTable GetBulkUserUploadLog(int userUploadId, string status)
        {
            return new DABulkUserUpload().GetBulkUserUploadLog(userUploadId, status);
        }
    }
    public class BLBulkUserUploadModuleMapping
    {
        public void SaveBulkUserUploadModuleMapping(List<BulkUserUploadModuleMapping> bulkUserUploadModuleMapping)
        {
            new DABulkUserUploadModuleMapping().SaveBulkUserUploadModuleMapping(bulkUserUploadModuleMapping);
        }
        public List<BulkUserUploadModuleMasterMapping> GetBulkUserUploadModuleMasterMapping()
        {
            return new DABulkUserUploadModuleMapping().GetBulkUserUploadModuleMasterMapping();
        }
    }
    public class BLBulkUserUploadWorkAreaDetail
    {
        public void SaveBulkUserUploadWorkAreaDetail(List<BulkUserUploadWorkAreaDetail> bulkUserUploadWorkAreaDetail)
        {
            new DABulkUserUploadWorkAreaDetail().SaveBulkUserUploadWorkAreaDetail(bulkUserUploadWorkAreaDetail);
        }
    }
    public class BLBulkUserUploadJoCatehoryMapping
    {
        public void SaveBulkUserUploadJoCategoryMapping(List<BulkUserUploadJoCategoryMapping> bulkUserUploadJoCategoryMapping)
        {
            new DABulkUserUploadJoCategoryMapping().SaveBulkUserUploadJoCategoryMapping(bulkUserUploadJoCategoryMapping);
        }
        public List<BulkUserUploadJoCategoryMasterMapping> GetBulkUserUploadJoCategoryMasterMapping()
        {
            return new DABulkUserUploadJoCategoryMapping().GetBulkUserUploadJoCategoryMasterMapping();
        }
    }
    public class BLBulkUserUploadJoTypeMapping
    {
        public void SaveBulkUserUploadJoTypeMapping(List<BulkUserUploadJoTypeMapping> bulkUserUploadJoTypeMapping)
        {
            new DABulkUserUploadJoTypeMapping().SaveBulkUserUploadJoTypeMapping(bulkUserUploadJoTypeMapping);
        }
        public List<BulkUserUploadJoTypeMasterMapping> GetBulkUserUploadJoTypeMasterMapping()
        {
            return new DABulkUserUploadJoTypeMapping().GetBulkUserUploadJoTypeMasterMapping();
        }
    }
    public class BLBulkUserUploadServiceFacilityMapping
    {
        public void SaveBulkUserUploadServiceFacilityMapping(List<BulkUserUploadServiceFacilityMapping> bulkUserUploadServiceFacilityMapping)
        {
            new DABulkUserUploadServiceFacilityMapping().SaveBulkUserUploadServiceFacilityMapping(bulkUserUploadServiceFacilityMapping);
        }
        public List<BulkUserUploadServiceFacilityMasterMapping> GetBulkUserUploadServiceFacilityMasterMapping()
        {
            return new DABulkUserUploadServiceFacilityMapping().GetBulkUserUploadServiceFacilityMasterMapping();
        }
    }
    #endregion

    #region UserOtpDetails
    public class BLUserOTPDetails
    {
        public OTPResponse GenerateNewOTP(int iUserId, string sUserName, string sSource)
        {
            return new DAUserOTPDetails().GenerateNewOTP(iUserId, sUserName, sSource);
        }

        public VerifyOTPResponse VerifyOTP(int iUserId, string sUserName, string sOTP, string sSource)
        {
            return new DAUserOTPDetails().VerifyOTP(iUserId, sUserName, sOTP, sSource);
        }

        public bool ResetUserOTPStatus(int iUserId, string sUserName, string sResetType)
        {
            return new DAUserOTPDetails().ResetUserOTPStatus(iUserId, sUserName, sResetType);
        }

        public bool SendOTPToUserMobile(string smsChannel, string sMobileNo, string sEmail, string sOTP)
        {
            bool btRetval = new DAUserOTPDetails().SendOTPToUserMobile(smsChannel, sMobileNo, sEmail, sOTP);
            return btRetval;
        }

        //public bool SendOTPToUserEmail(string userEmail, string subject, string messageBody, out string mailSentMsg)
        //{

        //    //EmailSettingsModel model = new BLMisc().getEmailSettings();
        //    return Utility.commonUtil.SendEmail(userEmail, subject, messageBody, model, out mailSentMsg);
        //    //return  commonUtil.SendEmail("pankaj.kumar@leptonsoftware.com", "TEST MAIL1", "HI TEST", model, out mailSentMsg);
        //}
        public GetOTPInternally GetOTPInternally(string sUserName)
        {
            return new DAUserOTPDetails().GetOTPInternally(sUserName);
        }
    }

    #endregion



    public class BLUserManagerMapping
    {
        public List<UserManagerMapping> SaveUserManagerMapping(List<UserManagerMapping> lstUserManagerMapping, int user_id)
        {
            return new DAUserManagerMapping().SaveUserManagerMapping(lstUserManagerMapping, user_id);
        }
        public List<UserManagerMapping> GetManagerMapping(int user_id)
        {
            return new DAUserManagerMapping().GetManagerMapping(user_id);
        }
    }

    public class BLUserToolMapping
    {
        public List<userFeToolMapping> SaveUserToolMapping(List<userFeToolMapping> lstUserToolMapping, int user_id)
        {
            return new DAUserToolMapping().SaveUserToolMapping(lstUserToolMapping, user_id);
        }
        public List<userFeToolMapping> GetToolMapping(int user_id)
        {
            return new DAUserToolMapping().GetToolMapping(user_id);
        }

    }

    public class BLUserWarehouseCodeMapping
    {
        public List<UserWarehouseCodeMapping> SaveUserWarehouseCodeMapping(List<UserWarehouseCodeMapping> lstWarehouseCodeMapping, int user_id)
        {
            return new DAWarehouseCodeMapping().SaveWarehouseCodeMapping(lstWarehouseCodeMapping, user_id);
        }
        public List<UserWarehouseCodeMapping> GetWarehouseCodeMapping(int user_id)
        {
            return new DAWarehouseCodeMapping().GetWarehouseCodeMapping(user_id);
        }
        public List<UserWarehouseCodeMapping> CheckWarehouseCodeExistOrNot(int user_id, string warehouseCode)
        {
            return new DAWarehouseCodeMapping().CheckWarehouseCodeExistOrNot(user_id, warehouseCode);
        }
    }

    public class BLUserJoCategoryMapping
    {
        public List<UserJoCategoryMapping> SaveUserJoCategoryMapping(List<UserJoCategoryMapping> lstUserJoCategoryMapping, int user_id)
        {
            return new DAUserJoCategoryMapping().SaveUserJoCategoryMapping(lstUserJoCategoryMapping, user_id);
        }

    }




    public class BLUserADOIDAuthentication
    {
        public ADOIDAuthentication SaveADOIDAuthentication(ADOIDAuthentication aDOIDAuthentication)
        {
            return new DAUserADOIDAuthentication().SaveADOIDAuthentication(aDOIDAuthentication);
        }
    }

    public class BLUserLoginHistory
    {
        public UserLoginHistoryInfo UpdateUserLoginById(int id, string token)
        {
            return new DAUserLoginHistory().UpdateUserLoginById(id, token);
        }

    }

}

