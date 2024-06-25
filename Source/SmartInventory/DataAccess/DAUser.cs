using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.WFM;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using DataAccess.Contracts;
using System.Data;
using Newtonsoft.Json;
using Npgsql;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using Utility;
using static Mono.Security.X509.X520;
using Models.Admin;

namespace DataAccess
{
    public class DAUser : Repository<User>
    {

        //public User ValidateUser(string username, string password)
        //{
        //    try
        //    {
        //        User ObjUser = new User();
        //        var userDetail = repo.ExecuteProcedure<CheckUserDetails>("fn_check_user_details", new { p_username = username, p_password = password }, true).FirstOrDefault();
        //        if (userDetail != null)
        //        {
        //            ObjUser.user_email = userDetail.user_email;
        //            ObjUser.user_id = userDetail.user_id;
        //            ObjUser.user_name = userDetail.user_name;
        //            ObjUser.role_id = userDetail.role_id;
        //            ObjUser.role_name = userDetail.role_name;
        //            ObjUser.is_active = userDetail.is_active;
        //            ObjUser.user_img = userDetail.user_img;
        //            return ObjUser;
        //        }
        //        return null;
        //        //return repo.Get(u => u.user_name.ToLower() == username.ToLower() && u.password == password);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //Methods

        public int GetNotificationCount(int userId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    //int day = 3;
                    //var LastDay = context.GlobalSetting.Where(x => x.key == "TaskLastNotDay" && x.type == "Web").FirstOrDefault();
                    //if (LastDay != null)
                    //{
                    //    day = Convert.ToInt32(LastDay.value);
                    //}
                    var objUser = context.wfm_notification.Where(s => s.manager_id == userId && s.isread == 0);
                    //if (day > 0)
                    //{
                    //    DateTime PreviousDate = DateTime.Now.AddDays(-day);
                    //    objUser = objUser.Where(y => y.created_on > PreviousDate);
                    //}
                    return objUser.Count();

                    // return 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        public string updateNotification(string Id)
        {
            int id = Convert.ToInt32(Id);
            using (var context = new MainContext())
            {
                wfm_notification entity = context.wfm_notification.Where(m => m.id == id).SingleOrDefault();
                if (entity != null)
                {
                    entity.isread = 1;
                    context.SaveChanges();
                }
                return "";
            }
        }

        public string UpdateNotificationForFastLane(string NotificationType, string JobOrderId, string IsMailSent)
        {
            using (var context = new MainContext())
            {
                wfm_notification entity = context.wfm_notification.Where(m => m.notification_type == NotificationType && m.hpsmid == JobOrderId).SingleOrDefault();
                if (entity != null)
                {
                    entity.email_id = IsMailSent;
                    context.SaveChanges();
                }
                return "";
            }
        }

        public User ValidateUser(string username, string password, string userType = "")
        {
            try
            {
                User ObjUser = new User();
                var userDetail = repo.ExecuteProcedure<CheckUserDetails>("fn_check_user_details", new { p_username = username, p_password = password, p_app_access = userType, p_admin_pass = ApplicationConfig.AppConfig.AdminPassword }, true).FirstOrDefault();
                if (userDetail != null)
                {
                    ObjUser.user_email = userDetail.user_email;// MiscHelper.Decrypt(userDetail.user_email);
                    ObjUser.user_id = userDetail.user_id;
                    ObjUser.user_name = userDetail.user_name;
                    ObjUser.role_id = userDetail.role_id;
                    ObjUser.role_name = userDetail.role_name;
                    ObjUser.is_active = userDetail.is_active;
                    ObjUser.user_img = userDetail.user_img;
                    ObjUser.is_admin_rights_enabled = userDetail.is_admin_rights_enabled;
                    ObjUser.user_type = userDetail.user_type;
                    ObjUser.prms_id = userDetail.prms_id;
                    ObjUser.mobile_number = MiscHelper.Decrypt(userDetail.mobile_number);
                    ObjUser.is_all_provience_assigned = userDetail.is_all_provience_assigned;
                    return ObjUser;
                }
                return null;
                //return repo.Get(u => u.user_name.ToLower() == username.ToLower() && u.password == password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public User getUserDetails(int id)
        {
            try
            {
                return repo.Get(u => u.user_id == id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User getUserDetailsByUserName(string userName)
        {
            try
            {
                return repo.Get(u => u.user_name.ToLower() == userName.ToLower());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<wfm_notification> GetNotificationDetail(ViewNotificationFilter ViewNotificationFilter, out int totalRecords)
        {
            try
            {
                totalRecords = 0;
                List<wfm_notification> NotificationDetail = null;
                using (MainContext context = new MainContext())
                {


                    var rmanagerId = new NpgsqlParameter("@MANAGERID", ViewNotificationFilter.managerId);
                    // var rId = new NpgsqlParameter("@USERID", userLeaveDetailFilter.userId);
                    ///var rtext = new NpgsqlParameter("@SEARCHTEXT", String.IsNullOrEmpty(userLeaveDetailFilter.searchText) ? "" : userLeaveDetailFilter.searchText.ToUpper());
                    // var rfromDate = new NpgsqlParameter("@FROMDATE", string.IsNullOrEmpty(ViewNotificationFilter.fromDate) ? "" : ViewNotificationFilter.fromDate);
                    // var rToDate = new NpgsqlParameter("@TODATE", string.IsNullOrEmpty(ViewNotificationFilter.toDate) ? "" : ViewNotificationFilter.toDate);
                    // var rStatus = new NpgsqlParameter("@STATUS", string.IsNullOrEmpty(ViewNotificationFilter.status) ? "" : ViewNotificationFilter.status.ToUpper());
                    var rpageNo = new NpgsqlParameter("@PAGENO", ViewNotificationFilter.currentPage);
                    var rpageRecord = new NpgsqlParameter("@PAGERECORD", ViewNotificationFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@SORTCOLNAME", string.IsNullOrEmpty(ViewNotificationFilter.sort) ? "" : ViewNotificationFilter.sort.ToUpper());
                    var rSortType = new NpgsqlParameter("@SORTTYPE", string.IsNullOrEmpty(ViewNotificationFilter.sortdir) ? "" : ViewNotificationFilter.sortdir.ToUpper());

                    var parameters = new NpgsqlParameter[5] { rmanagerId, rpageNo, rpageRecord, rSortCol, rSortType };
                    NotificationDetail = DbHelper.ExecutePostgresProcedure<wfm_notification>(context, "FN_GET_WFM_NOTIFI_DETAILS", parameters, out totalRecords);



                    return NotificationDetail;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<UserDetail> GetAllActiveUsersList()
        {
            try
            {
                return repo.ExecuteProcedure<UserDetail>("fn_get_active_users", new { }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<User> GetAllUsersList()
        {
            try
            {
                return repo.GetAll().ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<UserReportDetail> GetUserReportDetailsList(string user_id, bool isShowSelected)
        {
            try
            {
                return repo.ExecuteProcedure<UserReportDetail>("fn_get_report_users", new { p_user_id = user_id, p_isselected = isShowSelected }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<UserReportDetail> GetUserLocationTrackingList(string user_id, int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<UserReportDetail>("fn_get_user_locationtracking", new { p_user_id = user_id, p_role_id = role_id }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<User> GetUserReportDetailsList(string user_id)
        {
            try
            {
                return repo.ExecuteProcedure<User>("fn_get_report_users", new { p_user_id = user_id, p_isselected = true }, true).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void saveChangePassword(string password, int user_id)
        {
            try
            {

                var userDetails = repo.Get(x => x.user_id == user_id);
                if (userDetails != null)
                {
                    userDetails.password = password;
                    repo.Update(userDetails);
                }


            }
            catch (Exception ex)
            { throw ex; }
        }


        public User ChkUserExist(string username)
        {
            try
            {
                User ObjUser = new User();
                var userDetail = repo.ExecuteProcedure<CheckUserDetails>("fn_check_user_details", new { p_username = username, p_password = "", p_app_access = "", p_admin_pass = "" }, true).FirstOrDefault();
                if (userDetail != null)
                {
                    ObjUser.user_email = userDetail.user_email;
                    ObjUser.user_id = userDetail.user_id;
                    ObjUser.user_name = userDetail.user_name;
                    ObjUser.role_id = userDetail.role_id;
                    ObjUser.role_name = userDetail.role_name;
                    ObjUser.is_active = userDetail.is_active;
                    ObjUser.user_img = userDetail.user_img;
                    ObjUser.application_access = userDetail.application_access;
                    ObjUser.user_type = userDetail.user_type;
                    return ObjUser;
                }
                return null;
                //return repo.Get(u => u.user_name.ToLower() == username.ToLower() || u.user_email.ToLower() == username.ToLower());
            }
            catch { throw; }

        }
        public User validateUserLoginHistory(string username)
        {
            try
            {
                User ObjUser = new User();
                var userDetail = repo.ExecuteProcedure<ValidateuserLogInDetails>("fn_get_user_login_details", new { p_username = username }, true).FirstOrDefault();
                if (userDetail != null)
                {

                    ObjUser.user_id = userDetail.user_id;
                    ObjUser.session_id = userDetail.session_id;
                    return ObjUser;
                }
                return null;
                //return repo.Get(u => u.user_name.ToLower() == username.ToLower() || u.user_email.ToLower() == username.ToLower());
            }
            catch { throw; }
        }
        public bool ChkUserExist(string email, string userName)
        {
            try
            {
                email = MiscHelper.Encrypt(email);
                var userDetail = repo.Get(x => x.user_email.ToLower() == email.ToLower() && x.user_name.ToLower() == userName.ToLower() && x.is_active == true);
                if (userDetail != null)
                {
                    return true;
                }
                return false;
            }
            catch { throw; }
        }

        #region Add User / View user
        public List<KeyValueDropDown> GetAllRole(int role_id, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_rolelist", new { p_role_id = role_id, p_user_id = user_id }, true);
            }
            catch { throw; }
        }


        public List<KeyValueDropDown> GetUsernameDetails()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_user_details", new { }, true);
            }
            catch { throw; }
        }

        public List<KeyValueDropDown> GetUsernameDetails(int id )
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_user_details", new {ID =id }, true);
            }
            catch { throw; }
        }
        #region sapna

        public List<KeyValueDropDown> BindReportingManager(int RoleId, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_reportingmanager_list", new { p_roleId = RoleId, p_userId = user_id }, true);
            }
            catch { throw; }

        }
        public List<KeyValueDropDown> BindFETool(int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_user_tool_data", new { p_userId = user_id }, true);
            }
            catch { throw; }

        }
        public List<KeyValueDropDown> BindFETooldropdown(int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_fe_tool_data", new { p_userId = user_id }, true);
            }
            catch { throw; }

        }
        
        public List<KeyValueDropDown> BindFETool()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_user_tool_data", new {}, true);
            }
            catch { throw; }

        }


        public List<KeyValueDropDown> BindWarehouseCode()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_warehousecode_list", true);
            }
            catch { throw; }

        }


        #endregion
        public User SaveUser(User objUser, int userId)
        {
            try
            {
                objUser.user_img = string.IsNullOrWhiteSpace(objUser.user_img) ? "user.png" : objUser.user_img;
                if (objUser.user_id != 0)
                {
                    objUser.modified_by = userId;
                    objUser.modified_on = DateTimeHelper.Now;
                    objUser.mobile_number = MiscHelper.Encrypt(objUser.mobile_number);
                    objUser.name = MiscHelper.Encrypt(objUser.name);
                    //objUser.user_email = MiscHelper.Encrypt(objUser.user_email);
                    return repo.Update(objUser);
                }
                else
                {
                    objUser.created_by = userId;
                    objUser.mobile_number = MiscHelper.Encrypt(objUser.mobile_number);
                    objUser.name = MiscHelper.Encrypt(objUser.name);
                    objUser = repo.Insert(objUser);
                    InsertUserInReportMapping(objUser.user_id, objUser.manager_id);
                    return objUser;
                }

            }

            catch { throw; }

        }
        public User SaveMobileUser(User objUser, int userId)
        {
            try
            {
                objUser.user_img = string.IsNullOrWhiteSpace(objUser.user_img) ? "user.png" : objUser.user_img;
                if (objUser.user_id != 0)
                {
                    objUser.modified_by = userId;
                    objUser.modified_on = DateTimeHelper.Now;
                    objUser.mobile_number = MiscHelper.Encrypt(objUser.mobile_number);
                    objUser.name = MiscHelper.Encrypt(objUser.name);
                    return repo.Update(objUser);
                }
                else
                {
                    objUser.created_by = userId;
                    objUser.mobile_number = MiscHelper.Encrypt(objUser.mobile_number);
                    objUser.name = MiscHelper.Encrypt(objUser.name);
                    objUser = repo.Insert(objUser);
                    InsertMobileUser(objUser.user_id);
                    return objUser;
                }

            }

            catch { throw; }

        }
        public void InsertMobileUser(int user_id)
        {
            repo.ExecuteProcedure<object>("fn_insert_mobile_user_data", new { p_user_id = user_id });
        }
        public void InsertUserInReportMapping(int user_id, int manager_id)
        {

            repo.ExecuteProcedure<object>("fn_insert_new_user_into_report_mapping", new
            {
                p_user_id = user_id,
                p_manager_id = manager_id
            });
        }
        //public bool ChkUserCreationLimit(int userId, int webUserLimit, int mobileUserLimit,string userType)
        //{
        //    var maxUserLimit = webUserLimit + mobileUserLimit;
        //    try
        //    {
        //        var totalUsers = repo.GetAll(x => x.created_by == userId && x.is_deleted == false && x.is_active == true).ToList();
        //        if (userType == ApplicationUserType.Both.ToString())
        //        {
        //            if (totalUsers.Count() + 1 <= maxUserLimit)
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public DbMessage ChkUserCreationLimit(int userId, int webUserLimit, int mobileUserLimit, string userType)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_chk_user_limit", new { p_user_id = userId, p_web_limit = webUserLimit, p_mobile_limit = mobileUserLimit, p_app_access = userType }).FirstOrDefault();

            }
            catch { throw; }


        }

        public User ChkUserExist(User objUser)
        {
            try
            {
                objUser.user_email = MiscHelper.Encrypt(objUser.user_email);
                return repo.Get(u => u.user_name.ToLower().Trim() == objUser.user_name.ToLower().Trim() || u.user_email.ToLower().Trim() == objUser.user_email.ToLower().Trim());
            }
            catch
            {
                throw;
            }
        }

        public User ChkUserPanExist(User objUser)
        {
            try
            {
                return repo.Get(u => u.pan.ToLower().Trim() == objUser.pan.ToLower().Trim());
            }
            catch
            {
                throw;
            }
        }

        public List<User> ChkDuplicateUserExist(User objUser)
        {
            try
            {
                objUser.user_email = MiscHelper.Encrypt(objUser.user_email);
                return repo.GetAll(u => u.user_name.ToLower().Trim() == objUser.user_name.ToLower().Trim() || u.user_email.ToLower().Trim() == objUser.user_email.ToLower().Trim()).ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<UserDetail> GetUserList(CommonGridAttributes objGridAttributes, int role_id, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<UserDetail>("fn_get_user_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = ((objGridAttributes.searchBy == "name" || objGridAttributes.searchBy == "user_email") && (!string.IsNullOrEmpty(objGridAttributes.searchText))) ? MiscHelper.Encrypt(objGridAttributes.searchText) : objGridAttributes.searchText,
                    is_active = objGridAttributes.is_active,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    p_application_access = objGridAttributes.application_access,
                    p_role_id = role_id,
                    p_user_id = user_id
                }, true);
            }
            catch { throw; }
        }
      

        public List<User> GetUsersListByMGRIds(List<int> mgrIds)
        {
            return repo.GetAll().Where(p => mgrIds.Contains(p.manager_id)).ToList();
        }

        public List<userName> GetUserByManagerId(int id)
        {
            return repo.ExecuteProcedure<userName>("fn_get_user_by_manager_id", new { p_userId = id }).ToList();
        }

        public List<AssignedBy> GetAllUsers(int id)
        {
            try
            {
                return repo.ExecuteProcedure<AssignedBy>("fn_get_all_users", new { p_userId = id }).ToList();
            }
            catch (Exception e) { throw; }

        }


        public List<UserLoginHistory> GetUserLoginHistoryList(CommonGridAttributes objGridAttributes, int role_id, int user_id)
        {
            try
            {

                return repo.ExecuteProcedure<UserLoginHistory>("fn_get_user_login_history_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = ((objGridAttributes.searchBy == "mobile_number" || objGridAttributes.searchBy == "user_email") && (!string.IsNullOrEmpty(objGridAttributes.searchText))) ? MiscHelper.Encrypt(objGridAttributes.searchText) : objGridAttributes.searchText,
                    is_active = objGridAttributes.is_active,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    p_application_access = objGridAttributes.application_access,
                    p_role_id = role_id,
                    p_user_id = user_id,
                    p_searchfrom = objGridAttributes.fromDate,
                    p_searchto = objGridAttributes.toDate
                }, true);
            }
            catch { throw; }
        }


        public User GetUserDetailByID(int id)
        {
            var obj = repo.Get(m => m.user_id == id);

            return obj;

        }
        public User GetUserDetailByName(string name)
        {
            try
            {
                var objUser = repo.Get(m => m.name == name);
                return objUser != null ? objUser : new User();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public User GetUserDetailByUserName(string userName)
        {
            //userName = MiscHelper(userName);
            //return repo.GetAll().Where(x => x.user_name.ToLower() == userName.ToLower()).FirstOrDefault();
            return repo.Get(x => x.user_name.ToLower() == userName.ToLower());
        }

        public int DeleteUserById(int id)
        {
            try
            {
                var objUserId = repo.Get(x => x.user_id == id);
                if (objUserId != null)
                {
                    return repo.Delete(objUserId.user_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }

        }
        public List<User> CheckRoleAssignToUser(int roleid)
        {
            return repo.GetAll(m => m.is_active == true && m.role_id == roleid).ToList();
        }
        public List<Dictionary<string, string>> GetUserHistoryDetailById(FilterHistoryAttr objhistoryParam)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_user_history_by_id",
                    new
                    {
                        p_pageno = objhistoryParam.currentPage,
                        p_pagerecord = objhistoryParam.pageSize,
                        p_sortcolname = objhistoryParam.sort,
                        p_sorttype = objhistoryParam.orderBy,
                        p_userid = objhistoryParam.systemid
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public bool EditUserStatus(int user_id, bool status)
        {

            try
            {
                bool success = false;
                var objUser = repo.Get(u => u.user_id == user_id);
                if (objUser != null)
                {
                    objUser.is_active = status;
                    repo.Update(objUser);
                    success = true;
                }
                return success;
            }
            catch { throw; }
        }

        #endregion
        public static List<User_Master> GetSubordinateDetails(int userId, string roleName)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    List<User_Master> objUserDetail = null;
                    //commenting becaus no column shazia 
                    //if (!string.IsNullOrEmpty(roleName))
                    //{
                    //    objUserDetail = context.user_Masters.Where(u => u.manager_id == userId && u.role_name.ToUpper() == roleName.ToUpper()).ToList();
                    //}
                    //else
                    //{
                    //    objUserDetail = context.user_Masters.Where(u => u.manager_id == userId).ToList();
                    //}
                    objUserDetail = context.user_Masters.Where(u => u.manager_id == userId).ToList();
                    return objUserDetail;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static List<User_Master> GetMMSubordinateDetails(int userId, string roleName)
        {
            using (MainContext context = new MainContext())
            {
                List<User_Master> objList = new List<User_Master>();
                try
                {

                    {

                        string query = string.Format(@"select * from vw_multimanager_user_details where managerids = '{0}' ", userId);
                        objList = context.Database.SqlQuery<User_Master>(query).ToList();
                    }
                    return objList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static List<User_Master> GetSubordinateContractorDetails(int userId, string roleName)
        {
            List<User_Master> objUserDetail = null;
            using (MainContext context = new MainContext())
            {
                try
                {
                    var rId = new NpgsqlParameter("@USERID", userId);
                    var parameters = new NpgsqlParameter[1] { rId };
                    objUserDetail = DbHelper.ExecutePostgresProcedure<User_Master>(context, "fn_get_all_dispatecher_role_user", parameters);

                    return objUserDetail;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static List<User_Master> GetSubordinateContractorDetailstt(int userId, string roleName)
        {
            List<User_Master> objUserDetail = null;
            using (MainContext context = new MainContext())
            {
                try
                {
                    var rId = new NpgsqlParameter("@USERID", userId);
                    var parameters = new NpgsqlParameter[1] { rId };
                    objUserDetail = DbHelper.ExecutePostgresProcedure<User_Master>(context, "fn_get_all_dispatecher_role_usertt", parameters);

                    return objUserDetail;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static VW_USER_MANAGER_RELATION GetUserManagerFcmKey(int userId)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    VW_USER_MANAGER_RELATION objUsermanager = context.vw_user_manager_relation.Where(s => s.user_id == userId).FirstOrDefault();
                    return objUsermanager;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public User UpdateUserInfo(User objuser)
        {
            try
            {
                var ObjUser = repo.Get(u => u.pan == objuser.pan);
                if (ObjUser != null)
                {
                    ObjUser.prms_id = objuser.prms_id;
                    ObjUser.vendor_id = objuser.vendor_id;
                    ObjUser.is_active = objuser.is_active;
                    ObjUser.jc_id = objuser.jc_id;
                    return repo.Update(ObjUser);
                }
                return null;
            }
            catch { throw; }
        }

        public static List<TT_Type> GetttSubordinateDetailstt(string id)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    TT_category objTTCategory = null;
                    List<TT_Type> objUserDetail = null;
                    objTTCategory = context.TT_category.Where(u => u.name == id).FirstOrDefault();
                    objUserDetail = context.TT_Type.Where(u => u.tt_category_id == objTTCategory.id).ToList();
                    return objUserDetail;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static List<User_Master> GetSubordinateDetailstt(int userId, string roleName)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    List<User_Master> objUserDetail = null;
                    //commenting becaus no column shazia 
                    //if (!string.IsNullOrEmpty(roleName))
                    //{
                    //    objUserDetail = context.user_Masters.Where(u => u.manager_id == userId && u.role_name.ToUpper() == roleName.ToUpper()).ToList();
                    //}
                    //else
                    //{
                    //    objUserDetail = context.user_Masters.Where(u => u.manager_id == userId).ToList();
                    //}
                    objUserDetail = context.user_Masters.Where(u => u.manager_id == userId).ToList();
                    return objUserDetail;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public User UpdateEmailPhone(int user_id, double mobile, string email)
        {
            try
            {
                var userDetails = repo.Get(x => x.user_id == user_id);
                if (userDetails != null)
                {
                    userDetails.mobile_number = MiscHelper.Encrypt(Convert.ToString(mobile));
                    userDetails.user_email = MiscHelper.Encrypt(email);
                    return repo.Update(userDetails);
                }
                return null;
            }
            catch { throw; }
        }

        public string UpdateUserInfo(List<User> user)
        {

            try
            {
                var result = repo.Update(user);
                return result.ToString();
            }
            catch
            {
                throw;
            }
        }


        public List<EventEmailTemplateDetail> GetEventEmailTemplateDetail(string eventname)
        {
            try
            {
                return repo.ExecuteProcedure<EventEmailTemplateDetail>("fn_get_eventgemailtemplate", new { p_eventname = eventname }, true);
            }
            catch { throw; }


        }

        public List<KeyValueDropDown> GetManagerEmailId(int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_usermanageremail", new { p_user_id = user_id }, true);
            }
            catch { throw; }
        }

    }


    public class DAUserLogin : Repository<UserLogin>
    {
        public UserLogin SaveLoginHistory(UserLogin userLogin, string source)
        {
            try
            {
                var objUserLogin = repo.Get(u => u.user_id == userLogin.user_id && u.source.ToUpper() == source.ToUpper());
                if (objUserLogin != null)
                {

                    //update logout time of previous history id..
                    if (objUserLogin.logout_time == null)
                    {
                        objUserLogin.logout_time = DateTimeHelper.Now;
                        repo.Update(objUserLogin);
                    }
                    // initiate new session..                    
                    objUserLogin.client_ip = userLogin.client_ip;
                    objUserLogin.browser_name = userLogin.browser_name;
                    objUserLogin.browser_version = userLogin.browser_version;
                    objUserLogin.session_id = userLogin.session_id;
                    objUserLogin.login_time = DateTimeHelper.Now;
                    objUserLogin.logout_time = null;
                    objUserLogin.source = userLogin.source;
                    objUserLogin.os_name = userLogin.os_name;
                    objUserLogin.os_type = userLogin.os_type;
                    objUserLogin.os_version = userLogin.os_version;
                    objUserLogin.mac_address = userLogin.mac_address;
                    objUserLogin.mobile_app_version = userLogin.mobile_app_version;
                    objUserLogin.server_ip = userLogin.server_ip;
                    repo.Update(objUserLogin);
                }
                else
                {
                    repo.Insert(userLogin);
                }

                return repo.Get(u => u.user_id == userLogin.user_id && u.source.ToUpper() == source.ToUpper());
            }
            catch { throw; }
        }

        public UserLogin UpdateBrowserInfo(UserLogin objUserLogin)
        {
            try
            {
                var _objUserLogin = repo.Get(u => u.login_id == objUserLogin.login_id && u.source.ToUpper() == "WEB");
                if (_objUserLogin != null)
                {
                    _objUserLogin.client_ip = objUserLogin.client_ip;
                    _objUserLogin.browser_name = objUserLogin.browser_name;
                    _objUserLogin.browser_version = objUserLogin.browser_version;
                    _objUserLogin.server_ip = objUserLogin.server_ip;
                    _objUserLogin.session_id = objUserLogin.session_id;
                    return repo.Update(_objUserLogin);
                }
                return repo.Get(u => u.login_id == objUserLogin.login_id && u.source.ToUpper() == "WEB");
            }
            catch { throw; }
        }

        public bool UpdateLogOutTime(int userId, string source)
        {
            try
            {
                var objUserLogin = repo.Get(u => u.user_id == userId && u.source.ToUpper() == source.ToUpper());
                if (objUserLogin != null)
                {
                    objUserLogin.logout_time = DateTimeHelper.Now;
                    repo.Update(objUserLogin);
                }
                return true;
            }
            catch { throw; }
        }
        public bool UpdateLogOutTime(int userId, int historyId, string source, string signOut_type = null)
        {
            try
            {
                var objUserLogin = repo.Get(u => u.user_id == userId && u.source.ToUpper() == source.ToUpper());
                if (objUserLogin != null && objUserLogin.history_id == historyId)
                {
                    objUserLogin.logout_time = DateTimeHelper.Now;
                    objUserLogin.signout_type = signOut_type;

                    repo.Update(objUserLogin);
                }
                return true;
            }
            catch { throw; }
        }
        public UserLogin GetUserLoginDetailById(int userId, string Source)
        {
            return repo.Get(m => m.user_id == userId && m.source.ToUpper() == Source.ToUpper());
        }
        public int GetUserLoginHistoryId(int userId, string tokenSourceName)
        {
            var result = repo.Get(m => m.user_id == userId && m.source.ToUpper() == tokenSourceName.ToUpper());
            if (result != null)
            {
                return result.history_id;
            }
            else
            {
                return 0;
            }
        }
        public string GetSourceName(int userId)
        {
            var result = repo.Get(m => m.user_id == userId);
            if (result != null)
            {
                return result.source;
            }
            else
            {
                return "invalid";
            }
        }

        public bool UpdateRefreshToken(int userId, int loginId, string refreshToken)
        {
            try
            {
                var objUserLogin = repo.Get(m => m.login_id == loginId && m.user_id == userId);
                if (objUserLogin != null)
                {
                    objUserLogin.refresh_token = refreshToken;
                    repo.Update(objUserLogin);
                }
                return true;
            }
            catch { throw; }
        }

        public DbMessage ValidateUser(string user_name, string user_token, string fsa_id, string integration_source)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_user", new { p_user_name = user_name, p_user_token = user_token, p_fsa_id = fsa_id }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DbMessage LogoutUser(int userId, string application_access)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_logout_user", new { p_user_id = userId, p_application_access = application_access }).FirstOrDefault();
            }
            catch { throw; }
        }


    }


    public class DAUserLocationTracking : Repository<UserLocationTracking>
    {
        public bool SaveUserLocation(string TrackingDetails)
        {
            try
            {
                return repo.ExecuteProcedure<bool>("fn_save_location_tracking_details", new { p_trackingdetails = TrackingDetails }).FirstOrDefault();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserLocationTracking GetUserCurrentLocation(int login_id)
        {
            try
            {
                return repo.GetAll(m => m.login_id == login_id).OrderByDescending(s => s.tracking_id).FirstOrDefault();

            }
            catch { throw; }
        }


    }
    public class DAUserPermissionArea : Repository<UserPermissionArea>
    {
        public List<UserPermissionArea> getUserPermissionArea(int userid)
        {
            return repo.GetAll(m => m.user_id == userid).ToList();
        }
        //public int SaveUserPermissionArea(int userid, string provinceids, int createdby,string regionIds)
        //{
        //    try
        //    {
        //        var userDetail = repo.ExecuteProcedure<int>("fn_user_rights_save_user_permission_area", new { p_userid = userid, p_province_ids = provinceids, p_created_by = createdby, p_region_ids= regionIds }, false).FirstOrDefault();

        //        return userDetail;
        //        //return repo.Get(u => u.user_name.ToLower() == username.ToLower() || u.user_email.ToLower() == username.ToLower());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public int SaveUserPermissionArea(int userid, string provinceids, int createdby, string regionIds, string countryNames, string SubDistrictIds, string blockIds)
        {
            try
            {
                var isWfmUser = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["isWFMUser"]);
                var userDetail = 0;
                if (isWfmUser)
                {
                    userDetail = repo.ExecuteProcedure<int>("fn_user_rights_save_wfm_user_permission_area", new { p_userid = userid, p_province_ids = provinceids, p_created_by = createdby, p_region_ids = regionIds, p_country_names = countryNames != null ? countryNames.ToUpper() : countryNames, p_subdistrict_ids = SubDistrictIds, p_block_ids = blockIds }, false).FirstOrDefault();


                }
                else
                {
                    userDetail = repo.ExecuteProcedure<int>("fn_user_rights_save_user_permission_area", new { p_userid = userid, p_province_ids = provinceids, p_created_by = createdby, p_region_ids = regionIds, p_country_names = countryNames != null ? countryNames.ToUpper() : countryNames }, false).FirstOrDefault();


                }
                return userDetail;
                //return repo.Get(u => u.user_name.ToLower() == username.ToLower() || u.user_email.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }

    public class DAUserReportMapping : Repository<UserReportMapping>
    {
        public List<UserReportMappingVw> SaveUserReportMapping(List<UserReportMapping> lstUserReportMapping, int user_id, bool is_all_users_mapped)
        {
            //List<UserReportMapping> OldLst = GetUserReportMapping(user_id);
            //repo.DeleteRange(OldLst);
            //repo.Insert(lstUserReportMapping);
            //return lstUserReportMapping;
            var selectedUserMappingSerialize = JsonConvert.SerializeObject(lstUserReportMapping);
            var lst = repo.ExecuteProcedure<UserReportMappingVw>("fn_save_user_report_mapping",
                new
                {
                    p_jsonobj = selectedUserMappingSerialize,
                    p_user_id = user_id,
                    p_is_all_users_mapped = is_all_users_mapped
                });
            return lst;
        }
        public List<UserReportMapping> GetUserReportMapping(int user_id)
        {
            return repo.GetAll(m => m.parent_user_id == user_id).ToList();
        }
    }

    public class DARoleModuleMapping : Repository<RoleModuleMapping>
    {
        public List<RoleModuleMapping> SaveRoleModuleMapping(List<RoleModuleMapping> lstRoleModuleMapping, int role_id)
        {
            List<RoleModuleMapping> OldLst = GetRoleModuleMapping(role_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstRoleModuleMapping);
            return lstRoleModuleMapping;
        }
        public List<RoleModuleMapping> GetRoleModuleMapping(int role_id)
        {
            return repo.GetAll(m => m.role_id == role_id).ToList();
        }
    }

    public class DARoleServiceFacilityMapping : Repository<RoleSeviceFacilityMapping>
    {
        public List<RoleSeviceFacilityMapping> SaveRoleServiceFacilityMapping(List<RoleSeviceFacilityMapping> lstRoleSeviceFacilityMapping, int role_id)
        {
            List<RoleSeviceFacilityMapping> OldLst = GetRoleSeviceFacilityMapping(role_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstRoleSeviceFacilityMapping);
            return lstRoleSeviceFacilityMapping;
        }
        public List<RoleSeviceFacilityMapping> GetRoleSeviceFacilityMapping(int role_id)
        {
            return repo.GetAll(m => m.role_id == role_id).ToList();
        }
    }


    public class DAUserServiceFacilityMapping : Repository<UserSeviceFacilityMapping>
    {
        public List<UserSeviceFacilityMapping> SaveUserServiceFacilityMapping(List<UserSeviceFacilityMapping> lstUserSeviceFacilityMapping, int user_id)
        {
            List<UserSeviceFacilityMapping> OldLst = GetUserSeviceFacilityMapping(user_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstUserSeviceFacilityMapping);
            return lstUserSeviceFacilityMapping;
        }
        public List<UserSeviceFacilityMapping> GetUserSeviceFacilityMapping(int user_id)
        {
            return repo.GetAll(m => m.user_id == user_id).ToList();
        }
    }

    public class DARoleJoTypeMapping : Repository<RoleJoTypeMapping>
    {
        public List<RoleJoTypeMapping> SaveRoleJoTypeMapping(List<RoleJoTypeMapping> lstRoleJoTypeMapping, int role_id)
        {
            List<RoleJoTypeMapping> OldLst = GetRoleJoTypeMapping(role_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstRoleJoTypeMapping);
            return lstRoleJoTypeMapping;
        }
        public List<RoleJoTypeMapping> GetRoleJoTypeMapping(int role_id)
        {
            return repo.GetAll(m => m.role_id == role_id).ToList();
        }
    }
    public class DARoleJoCategoryMapping : Repository<RoleJoCategoryMapping>
    {
        public List<RoleJoCategoryMapping> SaveRoleJoCategoryMapping(List<RoleJoCategoryMapping> lstRoleJoCategoryMapping, int role_id)
        {
            List<RoleJoCategoryMapping> OldLst = GetRoleJoCategoryMapping(role_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstRoleJoCategoryMapping);
            return lstRoleJoCategoryMapping;
        }
        public List<RoleJoCategoryMapping> GetRoleJoCategoryMapping(int role_id)
        {
            return repo.GetAll(m => m.role_id == role_id).ToList();
        }
    }

    public class DAUserJoTypeMapping : Repository<UserJoTypeMapping>
    {
        public List<UserJoTypeMapping> SaveUserJoTypeMapping(List<UserJoTypeMapping> lstUserJoTypeMapping, int user_id)
        {
            List<UserJoTypeMapping> OldLst = GetUserJoTypeMapping(user_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstUserJoTypeMapping);
            return lstUserJoTypeMapping;
        }
        public List<UserJoTypeMapping> GetUserJoTypeMapping(int user_id)
        {
            return repo.GetAll(m => m.user_id == user_id).ToList();
        }
    }

    public class DAUserModuleMapping : Repository<UserModuleMapping>
    {
        public List<UserModuleMapping> SaveModuleMapping(List<UserModuleMapping> lstUserModuleMapping, int user_id)
        {
            List<UserModuleMapping> OldLst = GetModuleMapping(user_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstUserModuleMapping);
            return lstUserModuleMapping;
        }
        public List<UserModuleMapping> GetModuleMapping(int User_id)
        {
            return repo.GetAll(m => m.user_id == User_id).ToList();
        }
    }

    public class DAUserLoginHistoryInfo : Repository<UserLoginHistoryInfo>
    {
        public bool UpdateUserLogOutTime(int userId, int historyId, string signOut_type = null)
        {
            try
            {
                var objUserLogin = repo.Get(u => u.history_id == historyId);
                if (objUserLogin != null && objUserLogin.history_id == historyId)
                {
                    objUserLogin.logout_time = DateTimeHelper.Now;
                    objUserLogin.signout_type = signOut_type;
                    repo.Update(objUserLogin);
                }
                return true;
            }
            catch { throw; }
        }
    }

    #region BulkUserUpload
    public class DABulkUserUploadSummary : Repository<BulkUserUploadSummary>
    {

        public BulkUserUploadSummary SaveBulkUserUploadSummary(BulkUserUploadSummary bulkUserUploadSummary)
        {
            try
            {

                if (bulkUserUploadSummary.id != 0)
                {
                    return repo.Update(bulkUserUploadSummary);
                }
                else
                {
                    bulkUserUploadSummary = repo.Insert(bulkUserUploadSummary);

                }
                return bulkUserUploadSummary;

            }

            catch { throw; }

        }


        public List<BulkUserUploadSummary> GetBulkUserUploadSummary(CommonGridAttributes commonGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<BulkUserUploadSummary>("fn_bulk_user_upload_summary",
                    new
                    {
                        p_pageno = commonGridAttributes.currentPage,
                        p_pagerecord = commonGridAttributes.pageSize,
                        p_sortcolname = commonGridAttributes.sort,
                        p_sorttype = commonGridAttributes.orderBy
                    }, true).ToList();
            }
            catch (Exception e) { throw; }

        }


        public BulkUserUploadLimit BulkUploadUserCheckLimit(int userUploadId, int webMaxUser, int mobileMaxUser)
        {
            try
            {
                return repo.ExecuteProcedure<BulkUserUploadLimit>("fn_bulk_user_upload_chk_limit", new { p_upload_id = userUploadId, p_max_web = webMaxUser, p_max_mobile = mobileMaxUser }, false).FirstOrDefault();
            }
            catch (Exception e) { throw; }

        }

    }

    public class DABulkUserUpload : Repository<BulkUserUpload>
    {
        public List<BulkUserUpload> SaveBulkUserUpload(List<BulkUserUpload> bulkUserUpload)
        {
            try
            {
                //objUserUpload =
                repo.Insert(bulkUserUpload);
                return bulkUserUpload;
            }

            catch { throw; }
        }

        public int ProcessBulkUserUpload(int user_upload_id)
        {
            try
            {
                int res = 0;
                var userDetail = repo.ExecuteProcedure<string>("fn_bulk_user_upload_process", new { p_user_upload_id = user_upload_id });

                return res;
                //return userDetail;
                //return repo.Get(u => u.user_name.ToLower() == username.ToLower() || u.user_email.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable GetBulkUserUploadLog(int userUploadId, string status)
        {
            try
            {

                List<string> json = repo.ExecuteProcedure<string>("fn_bulk_user_upload_log", new { uploadid = userUploadId, status = status });
                DataTable dt = new DataTable();
                if (json[0] != null)
                {
                    dt = (DataTable)JsonConvert.DeserializeObject(json[0], (typeof(DataTable)));
                }
                dt.TableName = status + "_Logs";
                return dt;

            }
            catch { throw; }
        }
    }

    public class DABulkUserUploadModuleMapping : Repository<BulkUserUploadModuleMapping>
    {
        public List<BulkUserUploadModuleMapping> SaveBulkUserUploadModuleMapping(List<BulkUserUploadModuleMapping> bulkUserUploadModuleMapping)
        {
            try
            {
                //objUserUpload =
                repo.Insert(bulkUserUploadModuleMapping);
                return bulkUserUploadModuleMapping;
            }
            catch { throw; }
        }

        public List<BulkUserUploadModuleMasterMapping> GetBulkUserUploadModuleMasterMapping()
        {
            try
            {
                return repo.ExecuteProcedure<BulkUserUploadModuleMasterMapping>("fn_bulk_user_upload_module_master_mapping", new { }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }

    public class DABulkUserUploadWorkAreaDetail : Repository<BulkUserUploadWorkAreaDetail>
    {
        public List<BulkUserUploadWorkAreaDetail> SaveBulkUserUploadWorkAreaDetail(List<BulkUserUploadWorkAreaDetail> bulkUserUploadWorkAreaDetail)
        {
            try
            {
                //objUserUpload =
                repo.Insert(bulkUserUploadWorkAreaDetail);
                return bulkUserUploadWorkAreaDetail;
            }
            catch { throw; }
        }
    }
    public class DABulkUserUploadJoTypeMapping : Repository<BulkUserUploadJoTypeMapping>
    {
        public List<BulkUserUploadJoTypeMapping> SaveBulkUserUploadJoTypeMapping(List<BulkUserUploadJoTypeMapping> bulkUserUploadJoTypeMapping)
        {
            try
            {
                //objUserUpload =
                repo.Insert(bulkUserUploadJoTypeMapping);
                return bulkUserUploadJoTypeMapping;
            }
            catch { throw; }
        }
        public List<BulkUserUploadJoTypeMasterMapping> GetBulkUserUploadJoTypeMasterMapping()
        {
            try
            {
                return repo.ExecuteProcedure<BulkUserUploadJoTypeMasterMapping>("fn_bulk_user_upload_wfm_jo_type_master_mapping", new { }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class DABulkUserUploadJoCategoryMapping : Repository<BulkUserUploadJoCategoryMapping>
    {
        public List<BulkUserUploadJoCategoryMapping> SaveBulkUserUploadJoCategoryMapping(List<BulkUserUploadJoCategoryMapping> bulkUserUploadJoCategoryMapping)
        {
            try
            {
                //objUserUpload =
                repo.Insert(bulkUserUploadJoCategoryMapping);
                return bulkUserUploadJoCategoryMapping;
            }
            catch { throw; }
        }
        public List<BulkUserUploadJoCategoryMasterMapping> GetBulkUserUploadJoCategoryMasterMapping()
        {
            try
            {
                return repo.ExecuteProcedure<BulkUserUploadJoCategoryMasterMapping>("fn_bulk_user_upload_wfm_jo_category_master_mapping", new { }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class DABulkUserUploadServiceFacilityMapping : Repository<BulkUserUploadServiceFacilityMapping>
    {
        public List<BulkUserUploadServiceFacilityMapping> SaveBulkUserUploadServiceFacilityMapping(List<BulkUserUploadServiceFacilityMapping> bulkUserUploadServiceFacilityMapping)
        {
            try
            {
                //objUserUpload =
                repo.Insert(bulkUserUploadServiceFacilityMapping);
                return bulkUserUploadServiceFacilityMapping;
            }
            catch { throw; }
        }
        public List<BulkUserUploadServiceFacilityMasterMapping> GetBulkUserUploadServiceFacilityMasterMapping()
        {
            try
            {
                return repo.ExecuteProcedure<BulkUserUploadServiceFacilityMasterMapping>("fn_bulk_user_upload_wfm_service_facility_master_mapping", new { }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class DABulkUserUploadManagerMapping : Repository<BulkUserUploadManagerMapping>
    {
        public List<BulkUserUploadManagerMapping> SaveBulkUserUploadManagerMapping(List<BulkUserUploadManagerMapping> bulkUserUploadManagerMappings)
        {
            try
            {
                //objUserUpload =
                repo.Insert(bulkUserUploadManagerMappings);
                return bulkUserUploadManagerMappings;
            }
            catch { throw; }
        }
    }
    #endregion

    #region UserOtpDetails
    public class DAUserOTPDetails : Repository<UserOTPDetails>
    {
        public OTPResponse GenerateNewOTP(int iUserId, string sUserName, string sSource)
        {
            try
            {
                return repo.ExecuteProcedure<OTPResponse>("fn_generate_new_otp", new { p_user_id = iUserId, p_user_name = sUserName, p_source = sSource }, false).FirstOrDefault();

            }
            catch (Exception e) { throw; }
        }


        public VerifyOTPResponse VerifyOTP(int iUserId, string sUserName, string sOTP, string sSource)
        {
            try
            {
                return repo.ExecuteProcedure<VerifyOTPResponse>("fn_verify_otp", new { p_user_id = iUserId, p_user_name = sUserName, p_OTP = sOTP, p_source = sSource }, false).FirstOrDefault();

            }
            catch (Exception e) { throw; }
        }


        public bool ResetUserOTPStatus(int iUserId, string sUserName, string sResetType)
        {
            try
            {
                return repo.ExecuteProcedure<bool>("fn_reset_user_otp_status", new { p_user_id = iUserId, p_user_name = sUserName, p_reset_type = sResetType }, false).FirstOrDefault<bool>();

            }
            catch (Exception e) { throw; }
        }

        public bool SendOTPToUserMobile(string smsChannel, string sMobileNo, string sEmail, string sOTP)
        {
            bool btRetVal = true;
            //if (btRetVal == true)
            //    return btRetVal;
            try
            {
                string smsUrl = System.Configuration.ConfigurationManager.AppSettings["SMSUrl"];
                string SMSChannel = System.Configuration.ConfigurationManager.AppSettings["SMSChannel"];
                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri("https://api.textlocal.in");
                //    client.DefaultRequestHeaders.Accept.Clear();
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    //GET Method
                //    string sAPIKey = "NmE2ZTM5MzUzODY3Nzk1MTU3NGY2NDUzNmY0YzY3NzA=";
                //    string sMessage = "Hello ";
                //    string data = "apikey=" + System.Web.HttpUtility.UrlEncode(sAPIKey) + "&numbers=" + System.Web.HttpUtility.UrlEncode(sMobileNo) + "&sender=698682&message=" + System.Web.HttpUtility.UrlEncode(sMessage);

                //   HttpResponseMessage response = await client.PostAsync(("/send/?" + data);
                //    if (response.IsSuccessStatusCode)
                //    {
                //        btRetVal = true;
                //    }
                //    else
                //    {
                //        btRetVal = false;
                //    }
                //}

                if (SMSChannel == "JFP")
                {
                    string smsTemplateId = System.Configuration.ConfigurationManager.AppSettings["SMSTemplateId"];
                    using (var client = new HttpClient())
                    {
                        SMSOTPRequest sMSOTPRequest = new SMSOTPRequest();
                        Characteristics characteristics = new Characteristics();
                        if (smsChannel.Equals("mobile", StringComparison.OrdinalIgnoreCase))
                        {
                            characteristics.commonDataList = new List<CommonDataList>
                                {
                                    new CommonDataList() { name="SMS",value=sMobileNo}
                                    //new CommonDataList() { name="EMAIL",value=sEmail}
                                };
                        }
                        else
                        {
                            characteristics.commonDataList = new List<CommonDataList>
                                {
                                    //new CommonDataList() { name="SMS",value=sMobileNo},
                                    new CommonDataList() { name="EMAIL",value=sEmail}
                                };
                        }
                        ContractMedium contractMedium = new ContractMedium();
                        contractMedium.characteristics = characteristics;
                        sMSOTPRequest.contactMedium = contractMedium;
                        PayloadMessage payloadMessage = new PayloadMessage();
                        Attributes attributes = new Attributes();
                        attributes.commonDataList = new List<CommonDataList>
                        {
                            new CommonDataList() { name = "OTP", value =  sOTP}
                        };
                        payloadMessage.attributes = attributes;
                        payloadMessage.customerID = null;
                        payloadMessage.orderRefNo = null;
                        payloadMessage.registeredMobileNo = null;
                        payloadMessage.serviceID = null;
                        sMSOTPRequest.payloadMessage = payloadMessage;
                        sMSOTPRequest.referenceID = DateTime.UtcNow.ToString("yyyy-MM-dd HH\\:mm\\:ss");// "JFP_DATETIME";
                        sMSOTPRequest.templateID = smsTemplateId;
                        sMSOTPRequest.channelID = SMSChannel;

                        var smsjson = JsonConvert.SerializeObject(sMSOTPRequest);
                        var data = new StringContent(smsjson, Encoding.UTF8, "application/json");

                        WriteDebugLog("JFP OTP URL: " + smsUrl.ToString());
                        WriteDebugLog("JFP OTP Request: " + smsjson.ToString());

                        //client.BaseAddress = new Uri(smsUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        try
                        {
                            HttpResponseMessage response = client.PostAsync(smsUrl, data).Result;
                            var json = response.Content.ReadAsStringAsync().Result;
                            WriteDebugLog("JFP OTP Response: " + json.ToString());

                            if (response.IsSuccessStatusCode)
                            {
                                btRetVal = true;
                            }
                            else
                            {
                                btRetVal = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            btRetVal = false;
                        }
                    }
                }
            }
            catch (Exception ex) { throw; }

            return btRetVal;
        }
        public void WriteDebugLog(string LogMessage)
        {
            string errDesc = string.Empty;
            // Log error in file...

            string _logFolderPath = System.Configuration.ConfigurationManager.AppSettings["ApilogFolderPath"].ToString();
            using (StreamWriter sw = File.AppendText(System.Web.Hosting.HostingEnvironment.MapPath(_logFolderPath + "DebugLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
            {
                sw.WriteLine("\r\nLog Entry:==========>");
                sw.WriteLine("Log on Time: {0}", DateTimeHelper.Now);
                sw.WriteLine("Log Message: {0}", LogMessage);
            }
        }

        public GetOTPInternally GetOTPInternally(string sUserName)
        {
            try
            {
                return repo.ExecuteProcedure<GetOTPInternally>("fn_get_otp_internally", new { p_user_name = sUserName }, false).FirstOrDefault();

            }
            catch (Exception e) { throw; }
        }

    }


    #endregion


    public class DAUserManagerMapping : Repository<UserManagerMapping>
    {
        public List<UserManagerMapping> SaveUserManagerMapping(List<UserManagerMapping> lstUserManagerMapping, int user_id)
        {
            List<UserManagerMapping> OldLst = GetManagerMapping(user_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstUserManagerMapping);
            return lstUserManagerMapping;
        }
        public List<UserManagerMapping> GetManagerMapping(int user_id)
        {
            return repo.GetAll(m => m.user_id == user_id).ToList();
            // return repo.GetAll(m => m.manager_id == user_id).ToList();
        }
    }
    public class DAUserToolMapping : Repository<userFeToolMapping>
    {
        public List<userFeToolMapping> SaveUserToolMapping(List<userFeToolMapping> lstUserToolsMapping, int user_id)
        {
            List<userFeToolMapping> OldLst = GetToolMapping(user_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstUserToolsMapping);
            return lstUserToolsMapping;
        }
        public List<userFeToolMapping> GetToolMapping(int user_id)
        {
            return repo.GetAll(m => m.user_id == user_id).ToList();
            // return repo.GetAll(m => m.manager_id == user_id).ToList();
        }

    }


    public class DAWarehouseCodeMapping : Repository<UserWarehouseCodeMapping>
    {
        public List<UserWarehouseCodeMapping> SaveWarehouseCodeMapping(List<UserWarehouseCodeMapping> lstUserWarehouseCodeMapping, int user_id)
        {
            List<UserWarehouseCodeMapping> OldLst = GetWarehouseCodeMapping(user_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstUserWarehouseCodeMapping);
            return lstUserWarehouseCodeMapping;
        }
        public List<UserWarehouseCodeMapping> GetWarehouseCodeMapping(int user_id)
        {
            return repo.GetAll(m => m.user_id == user_id).ToList();
        }
        public List<UserWarehouseCodeMapping> CheckWarehouseCodeExistOrNot(int user_id, string warehouseCode)
        {
            return repo.GetAll(m => m.user_id == user_id && m.warehouse_code == warehouseCode).ToList();
        }
    }
    public class DAUserJoCategoryMapping : Repository<UserJoCategoryMapping>
    {
        public List<UserJoCategoryMapping> SaveUserJoCategoryMapping(List<UserJoCategoryMapping> lstUserJoCategoryMapping, int user_id)
        {
            List<UserJoCategoryMapping> OldLst = GetUserJoCategoryMapping(user_id);
            repo.DeleteRange(OldLst);
            repo.Insert(lstUserJoCategoryMapping);
            return lstUserJoCategoryMapping;
        }
        public List<UserJoCategoryMapping> GetUserJoCategoryMapping(int user_id)
        {
            return repo.GetAll(m => m.user_id == user_id).ToList();
        }
    }
    public class DAUserADOIDAuthentication : Repository<ADOIDAuthentication>
    {
        public ADOIDAuthentication SaveADOIDAuthentication(ADOIDAuthentication aDOIDAuthentication)
        {
            try
            {
                if (aDOIDAuthentication.id != 0)
                {
                    return repo.Update(aDOIDAuthentication);
                }
                else
                {
                    aDOIDAuthentication = repo.Insert(aDOIDAuthentication);
                }
                //repo.ExecuteProcedure<bool>("fn_user_master_upd_phone_email", new { p_user_id = aDOIDAuthentication.user_id, p_phone = aDOIDAuthentication.mobile, p_email = aDOIDAuthentication.email_id }, false).FirstOrDefault<bool>();
                return aDOIDAuthentication;

            }
            catch { throw; }

        }
    }

    public class DAUserLoginHistory : Repository<UserLoginHistoryInfo>
    {
        public UserLoginHistoryInfo UpdateUserLoginById(int id, string token)
        {
            UserLoginHistoryInfo objUserLoginHistory = new UserLoginHistoryInfo();
            try
            {
                var userHistory = repo.GetAll(m => m.user_id == id).OrderByDescending(s => s.login_time).FirstOrDefault();
                if (userHistory.history_id != 0)
                {
                    userHistory.access_token = token;
                    return repo.Update(userHistory);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objUserLoginHistory;
        }
    }
}