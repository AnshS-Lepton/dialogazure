using BusinessLogics;
using BusinessLogics.Admin;
using Models;
using Models.Admin;
using NPOI.SS.UserModel;
using SmartInventory.Filters;
using SmartInventory.Helper;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Utility;
using System.Text.RegularExpressions;
using System.Web.Security;
using static Mono.Security.X509.X520;
using Models.WFM;
using static NPOI.HSSF.Util.HSSFColor;
//using Models.API;

namespace SmartInventory.Areas.Admin.Controllers
{
    [AdminOnly]
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class UserController : Controller
    {

        public ActionResult AddUser(int id = 0)
        {

            //int user_id = Convert.ToInt32(Session["user_id"]);
            var objLgnUsrDtl = (User)Session["userDetail"];
            int user_id = objLgnUsrDtl.user_id;

            var maxWebLimit = ApplicationConfig.AppConfig.WebUserMaxLimit;
            var maxMobileLimit = ApplicationConfig.AppConfig.MobileUserMaxLimit;
            var isUserCountInLimit = new BLUser().ChkUserCreationLimitExeeds(id, maxWebLimit, maxMobileLimit, ApplicationUserType.Both.ToString());
            User objUser = new User();
            if (!isUserCountInLimit.status && isUserCountInLimit.message == "BOTH" && id == 0)
            {
                objUser.IsUserinLimit = false;
            }
            else
            {

                if (id != 0)
                {
                    objUser = new BLUser().GetUserDetailByID(id);
                    objUser.password = Convert.ToString(Utility.MiscHelper.DecodeTo64(objUser.password));
                    objUser.user_email = MiscHelper.Decrypt(objUser.user_email);
                    objUser.name = MiscHelper.Decrypt(objUser.name);
                    objUser.mobile_number = MiscHelper.Decrypt(objUser.mobile_number);
                    //objUser.lstViewServiceFacilityJobOrder = new BLMisc().GetUserServiceFacilityJobOrder(objUser.role_id, id);

                }
                objUser.lstRole = new BLUser().GetAllRole(objLgnUsrDtl.role_id, user_id);
                objUser.lstRM = GetReportingManagers(objUser.role_id);
                objUser.lstWarehouseCode = new BLUser().BindWarehouseCode();

                objUser.lstUserModule = new BLMisc().GetRoleModule(objUser.role_id);// new BLMisc().GetUserModuleMasterList();
                objUser.lstUserPermissionArea = new BLUserPermissionArea().GetUserPermissionArea(id);
                objUser.lstUserModuleMapping = new BLUserModuleMapping().GetModuleMapping(id);
                foreach (var item in objUser.lstUserModule)
                {
                    var selmoduleId = objUser.lstUserModuleMapping.Where(m => m.module_id == item.Id).Select(m => m.module_id);
                    if (selmoduleId.Count() > 0)
                    {
                        item.is_selected = true;

                    }
                }
                if (objUser.lstUserPermissionArea.Count > 0)
                {
                    var distincts = objUser.lstUserPermissionArea.Select(m => m.region_id).Distinct().ToArray();
                    var distinctCountry = objUser.lstUserPermissionArea.Select(m => m.countryName).Distinct().ToArray();
                    objUser.lstRegion = bindRegionDropDown(distincts);
                    var lstProvince = objUser.lstUserPermissionArea.Select(m => m.province_id).Distinct().ToArray();
                    // objUser.selectedProvinces = string.Join(",", lstProvince);
                    objUser.selectedProvinces = objUser.is_all_provience_assigned == true ? null : string.Join(",", lstProvince);
                    var lstSubDistrict = objUser.lstUserPermissionArea.Select(m => m.subdistrict_id).Distinct().ToArray();
                    objUser.selectedSubDistrict = string.Join(",", lstSubDistrict);
                    var lstBlock = objUser.lstUserPermissionArea.Select(m => m.block_id).Distinct().ToArray();
                    objUser.selectedBlock = string.Join(",", lstBlock);
                    List<RegionProvinceLayer> objRegionProvince = new List<RegionProvinceLayer>();
                    objRegionProvince = new BLLayer().GetRegionProvinceLayers(0);
                    objUser.lstCountry = bindCountryDropDown(distincts);
                    //if (distincts.Count() == objRegionProvince.Count())
                    //{
                    //    objUser.selectedProvinces = null;
                    //    objUser.lstRegion = bindRegionDropDown(null);
                    //    objUser.lstCountry = bindCountryDropDown(null);
                    //}
                    //else
                    //{
                    //objUser.selectedProvinces = string.Join(",", lstProvince);
                    objUser.selectedProvinces = objUser.is_all_provience_assigned == true ? null : string.Join(",", lstProvince);

                    // }
                }
                else
                {
                    objUser.lstRegion = bindRegionDropDown(null);
                    objUser.lstCountry = bindCountryDropDown(null);
                }
                objUser.lstUserType = new BLMisc().GetDropDownList("", DropDownType.UserType.ToString());
                objUser.selectedRegion = objUser.selectedProvinces == null ? null : string.Join(",", objUser.lstRegion.Where(x => x.Selected == true).Select(x => x.Value));
                objUser.selectedCountry = objUser.selectedRegion == null ? null : string.Join(",", objUser.lstCountry.Where(x => x.Selected == true).Select(x => x.Value));
                objUser.IsSubmit = false;
                objUser.formInputSettings = new BLFormInputSettings().getformInputSettings();
                //ViewBag.IsSubmit = false;
                objUser.IsUserinLimit = true;
                GlobalSetting globalSetting = new BLGlobalSetting().getValueFullText("IsMultiManagerAllowed");
                if (globalSetting != null)
                {
                    objUser.is_multi_manager_allowed = globalSetting.value == "1" ? true : false;                   
                    objUser.multi_manager_ids = Convert.ToString(objUser.manager_id);
                    if (objUser.is_multi_manager_allowed)
                    {
                        objUser.multi_manager_ids = string.Join(",", new BLUserManagerMapping().GetManagerMapping(objUser.user_id).Select(x => x.manager_id).ToList());

                        if (string.IsNullOrEmpty(objUser.multi_manager_ids))
                        {

                            objUser.multi_manager_ids = Convert.ToString(objUser.manager_id);
                        }
                    }
                    if (ApplicationSettings.fetoolsenabled)
                    {
                        objUser.multi_tool_ids = string.Join(",", new BLUserToolMapping().GetToolMapping(objUser.user_id).Select(x => x.tool_id).ToList());
                      if (string.IsNullOrEmpty(objUser.multi_tool_ids))
                        {
                            objUser.multi_tool_ids = Convert.ToString(objUser.multi_tool_ids);
                        }
                    }
                }
                objUser.multi_warhouse_code = Convert.ToString(objUser.warehouse_code);
                objUser.multi_warhouse_code = string.Join(",", new BLUserWarehouseCodeMapping().GetWarehouseCodeMapping(objUser.user_id).Select(x => x.warehouse_code).ToList());
                if (string.IsNullOrEmpty(objUser.multi_warhouse_code))
                    if (string.IsNullOrEmpty(objUser.user_type))
                    {
                        objUser.multi_warhouse_code = Convert.ToString(objUser.warehouse_codes);
                    }
                if (string.IsNullOrEmpty(objUser.user_type))
                {
                    objUser.user_type = objUser.lstUserType.FirstOrDefault().dropdown_value;
                }
                objUser.lstFEtool = new BLUser().BindFETool(0);
            }

           

            return View(objUser);
        }

        public ActionResult UserReportMapping()
        {
            UserReportMappingVM mapping = new UserReportMappingVM();
            mapping.lstUser = new BLUser().GetAllActiveUsersList();
            return View(mapping);
        }

        public ActionResult UserReportList(int user_id)
        {
            var userReportMappingList = new BLUser().GetUserReportDetailsList(user_id.ToString(), false);

            if (userReportMappingList.Count > 0)
            {
                for (int i = 0; i <= userReportMappingList.Count - 1; i++)
                {
                    userReportMappingList[i].user_email = MiscHelper.Decrypt(userReportMappingList[i].user_email);

                    userReportMappingList[i].name = MiscHelper.Decrypt(userReportMappingList[i].name);

                    userReportMappingList[i].mobile_number = MiscHelper.Decrypt(userReportMappingList[i].mobile_number);

                }

            }

            return PartialView("_UserReportList", userReportMappingList);
        }

        [HttpPost]
        public ActionResult SaveReportUserMapping(UserReportMappingVM mapping)
        {
            PageMessage objPM = new PageMessage();
            try
            {
                var selectedUsersMapping = mapping.UserReportMappingList.Where(m => m.isSelected == true).
                    Select(m => new UserReportMapping() { child_user_id = m.user_id, parent_user_id = mapping.user_id }).ToList();
                selectedUsersMapping = mapping.is_all_users_mapped == false ? selectedUsersMapping : selectedUsersMapping = null;
                var result = new BLUserReportMapping().SaveUserReportMapping(selectedUsersMapping, mapping.user_id, mapping.is_all_users_mapped);
                objPM.status = ResponseStatus.OK.ToString();
                objPM.message = "User Report Mapping has been saved successfully.";
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("SaveUserReportMapping()", "Admin/User", ex);
                objPM.status = ResponseStatus.ERROR.ToString();
                objPM.message = "Something went wrong while mapping users!!";
            }
            mapping.UserReportMappingList = new BLUser().GetUserReportDetailsList(mapping.user_id.ToString(), false);
            if (mapping.UserReportMappingList.Count > 0)
            {
                for (int i = 0; i <= mapping.UserReportMappingList.Count - 1; i++)
                {
                    mapping.UserReportMappingList[i].user_email = MiscHelper.Decrypt(mapping.UserReportMappingList[i].user_email);

                    mapping.UserReportMappingList[i].name = MiscHelper.Decrypt(mapping.UserReportMappingList[i].name);

                    mapping.UserReportMappingList[i].mobile_number = MiscHelper.Decrypt(mapping.UserReportMappingList[i].mobile_number);

                }

            }
            mapping.lstUser = new BLUser().GetAllActiveUsersList();
            mapping.pageMsg = objPM;
            return View("UserReportMapping", mapping);
        }

        [HttpPost]
        public JsonResult CheckDuplicateUserNameandEmail(int user_id, string userName, string emailId)
        {

            JsonResponse<string> objResp = new JsonResponse<string>();
            PageMessage objMsg = new PageMessage();
            List<User> objUserList = new List<User>();
            User objUserExist = new User();
            objUserExist.user_name = userName;
            objUserExist.user_email = emailId;
            try
            {
                objUserList = new BLUser().ChkDuplicateUserExist(objUserExist);
                if (objUserList.Count > 0)
                {
                    objUserList[0].user_email = MiscHelper.Decrypt(objUserList[0].user_email);
                }
                if ((objUserList != null && user_id == 0) || (objUserList != null && objUserList.Count > 1) || (objUserExist != null && objUserList[0].user_id != user_id))
                {
                    objMsg.status = ResponseStatus.ERROR.ToString();
                    if (objUserList[0].user_name.Trim().ToLower() == userName.ToLower())
                        objMsg.message = "User Name already exist!";
                    else if (objUserList[0].user_email.Trim().ToLower() == emailId.ToLower())
                        objMsg.message = "Email Id already exist!";
                }

            }
            catch (Exception ex)
            {
                objMsg.status = "";
            }
            return Json(objMsg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveUser(User objUser)
        {

            var objLgnUsrDtl = (User)Session["userDetail"];
            List<UserSeviceFacilityMapping> lstUserSeviceFacilityMapping = null;
            List<UserJoTypeMapping> lstUserJoTypeMapping = null;
            List<UserManagerMapping> lstUserManagerMapping = new List<UserManagerMapping>();
            List<userFeToolMapping> lstUserToolMapping = new List<userFeToolMapping>();
            List<UserJoCategoryMapping> lstUserJoCategoryMapping = null;
            List<UserWarehouseCodeMapping> lstUserWarehouseCodeMapping = new List<UserWarehouseCodeMapping>();
           
            // JsonResponse<string> objResp = new JsonResponse<string>();
            ModelState.Clear();
            PageMessage objMsg = new PageMessage();
            if (objUser.selectedRegion != null)
            {
                var distincts = objUser.selectedRegion.Split(',').Select(int.Parse).ToArray();
                objUser.lstRegion = bindRegionDropDown(distincts);
                objUser.lstCountry = bindCountryDropDown(distincts);
            }
            else
            {
                objUser.lstRegion = bindRegionDropDown(null);
                objUser.lstCountry = bindCountryDropDown(null);
            }


            int user_id = Convert.ToInt32(Session["user_id"]);

            var objUserExist = new BLUser().ChkUserExist(objUser);
            if (objUserExist != null)
            {
                objUserExist.user_email = MiscHelper.Decrypt(objUserExist.user_email);
            }
            var objUserPanExist = new BLUser().ChkUserPanExist(objUser);

            if ((objUserExist != null && objUser.user_id == 0) || (objUserExist != null && objUser.user_id != objUserExist.user_id))
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                if (objUserExist.user_name.Trim().ToLower() == objUser.user_name.Trim().ToLower())
                    objMsg.message = "User Name already exist!";
                else if (objUserExist.user_email.Trim().ToLower() == objUser.user_email.Trim().ToLower())
                    objMsg.message = "Email Id already exist!";
            }
            else if ((!string.IsNullOrEmpty(objUser.pan) && ((objUserPanExist != null && objUser.user_id == 0) || (objUserPanExist != null && objUser.user_id != objUserPanExist.user_id))))
            {
                objMsg.status = ResponseStatus.ERROR.ToString();
                if (objUserPanExist.pan.Trim().ToLower() == objUser.pan.Trim().ToLower())
                    objMsg.message = "PAN already exist!";
            }

            else
            {
                var isNew = objUser.user_id > 0 ? false : true;
                //objUser.password = Convert.ToString(Utility.MiscHelper.EncodeTo64(objUser.password));
                //set default password. this is done password is not require  due to ADFS integration..
                objUser.password = Convert.ToString(Utility.MiscHelper.EncodeTo64(string.IsNullOrWhiteSpace(objUser.password) ? "12345678" : objUser.password));
                objUser.pan = !string.IsNullOrEmpty(objUser.pan) ? objUser.pan.ToUpper().Trim() : null;
                objUser = new BLUser().SaveUser(objUser, user_id);
                //Save User Permission after user details saved                
                var res = new BLUserPermissionArea().SaveUserPermissionArea(objUser.user_id, objUser.selectedProvinces, user_id, objUser.selectedRegion, objUser.selectedCountry, objUser.selectedSubDistrict, objUser.selectedBlock);
                objUser.lstUserModuleMapping = objUser.lstUserModule.Where(m => m.is_selected == true).Select(m => new UserModuleMapping() { module_id = m.Id, user_id = objUser.user_id, created_by = user_id, created_on = DateTimeHelper.Now }).ToList();
                objUser.lstUserModuleMapping = new BLUserModuleMapping().SaveModuleMapping(objUser.lstUserModuleMapping, objUser.user_id);

                if (objUser.lstRoleServiceFacilityMapping != null)
                {
                    lstUserSeviceFacilityMapping = objUser.lstRoleServiceFacilityMapping.Where(m => m.is_selected == true).Select(m => new UserSeviceFacilityMapping() { service_facility_id = m.id, user_id = objUser.user_id }).ToList();
                }
                if (objUser.lstRoleJobOrderMapping != null)
                {
                    lstUserJoTypeMapping = objUser.lstRoleJobOrderMapping.Where(m => m.is_selected == true).Select(m => new UserJoTypeMapping() { jo_type_id = m.id, user_id = objUser.user_id }).ToList();
                }

                if (objUser.lstRoleJobCategoryMapping != null)
                {
                    lstUserJoCategoryMapping = objUser.lstRoleJobCategoryMapping.Where(m => m.is_selected == true).Select(m => new UserJoCategoryMapping() { jo_category_id = m.id, user_id = objUser.user_id }).ToList();
                }
                lstUserSeviceFacilityMapping = new BLUserServiceFacilityMapping().SaveUserServiceFacilityMapping(lstUserSeviceFacilityMapping, objUser.user_id);
                lstUserJoTypeMapping = new BLUserJoTypeMapping().SaveUserJoTypeMapping(lstUserJoTypeMapping, objUser.user_id);
                lstUserJoCategoryMapping = new BLUserJoCategoryMapping().SaveUserJoCategoryMapping(lstUserJoCategoryMapping, objUser.user_id);
                ////Multimanager implementation
                if (objUser.multi_manager_ids != null && objUser.is_multi_manager_allowed == true)
                {
                    var listManager = objUser.multi_manager_ids.Split(',').ToList();
                    if (listManager.Count > 0)
                    {
                        foreach (string item in listManager)
                        {
                            lstUserManagerMapping.Add(new UserManagerMapping() { user_id = objUser.user_id, manager_id = Convert.ToInt32(item) });
                        }
                    }
                }
                lstUserManagerMapping = new BLUserManagerMapping().SaveUserManagerMapping(lstUserManagerMapping, objUser.user_id);

                if (objUser.multi_tool_ids != null)
                {
                    var listToolsId = objUser.multi_tool_ids.Split(',').ToList();
                        foreach (string item in listToolsId)
                        {
                            lstUserToolMapping.Add(new userFeToolMapping() { user_id = objUser.user_id, tool_id = Convert.ToInt32(item) });
                        }
                }
                lstUserToolMapping = new BLUserToolMapping().SaveUserToolMapping(lstUserToolMapping, objUser.user_id);
                if (objUser.multi_warhouse_code != null)
                {
                    var listWarehouseCode = objUser.multi_warhouse_code.Split(',').ToList();
                    if (listWarehouseCode.Count > 0)
                    {
                        foreach (string item in listWarehouseCode)
                        {
                            lstUserWarehouseCodeMapping.Add(new UserWarehouseCodeMapping() { user_id = objUser.user_id, warehouse_code = (item) });
                        }
                    }
                }
                lstUserWarehouseCodeMapping = new BLUserWarehouseCodeMapping().SaveUserWarehouseCodeMapping(lstUserWarehouseCodeMapping, objUser.user_id);

                if (isNew)
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.isNewEntity = isNew;
                    objMsg.message = "User details saved successfully.";
                    //objUser.role_id = 0; 
                    #region email sending start                 
                    objUser.lstRole = new BLUser().GetAllRole(objLgnUsrDtl.role_id, user_id);
                    string rolevalue = Convert.ToString(objUser.lstRole.FirstOrDefault(obj => obj.value == objUser.role_id.ToString()).key);

                    Dictionary<string, string> objUserDict = new Dictionary<string, string>();
                    objUserDict.Add("Name", MiscHelper.Decrypt(objUser.name));
                    objUserDict.Add("Username", objUser.user_name);
                    objUserDict.Add("UserRole", rolevalue);
                    objUserDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                    List<HttpPostedFileBase> objHttpPostedFileBase = null;
                    BLUser objBLuser = new BLUser();
                    List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.UserCreation.ToString());
                    string managerEmailId = "";
                    List<KeyValueDropDown> objMangerDetail = new BLUser().GetManagerEmailId(lstUserManagerMapping[0].user_id);
                    if (objMangerDetail != null)
                    {
                        int cnt = 0;
                        foreach (KeyValueDropDown objList in objMangerDetail)
                        {
                            if (cnt == 0)
                                managerEmailId = MiscHelper.Decrypt(objMangerDetail[cnt].value.ToString());
                            else
                                managerEmailId = managerEmailId + "," + MiscHelper.Decrypt(objMangerDetail[cnt].value.ToString());
                            cnt++;
                        }

                        objEventEmailTemplateDetail[0].recipient_list = managerEmailId;
                    }

                    System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objUserDict, objHttpPostedFileBase, EmailSettings.AllEmailSettings, null, "", EmailEventList.UserCreation.ToString()));
                    //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objUserDict, objHttpPostedFileBase);
                    #endregion
                }
                else
                {
                    objMsg.status = ResponseStatus.OK.ToString();
                    objMsg.message = "User details updated successfully.";
                }
                objUser.password = Convert.ToString(MiscHelper.DecodeTo64(objUser.password));
                objUser.IsSubmit = true;
                //ViewBag.IsSubmit = true;
            }
            //BIND ROLES
            objUser.lstRole = new BLUser().GetAllRole(objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);
            //DECODE PASSWORD AGAIN
            objUser.lstRM = GetReportingManagers(objUser.role_id);
            objUser.lstWarehouseCode = new BLUser().BindWarehouseCode();

            objUser.lstUserType = new BLMisc().GetDropDownList("", DropDownType.UserType.ToString());
            objUser.lstFEtool = new BLUser().BindFETool(0);//.OrderBy(m => m.key).ToList();
            objUser.role_id = objUser.role_id;
            objUser.manager_id = objUser.manager_id;
            objUser.warehouse_code = objUser.warehouse_code;
            objUser.pageMsg = objMsg;
            objUser.IsUserinLimit = true;
            return View("AddUser", objUser);
        }

        [HttpPost]
        public JsonResult CheckUserLimit(int user_id, string app_access)
        {
            // int user_id = Convert.ToInt32(Session["user_id"]);

            var maxWebLimit = ApplicationConfig.AppConfig.WebUserMaxLimit;
            var maxMobileLimit = ApplicationConfig.AppConfig.MobileUserMaxLimit;
            var status = new BLUser().ChkUserCreationLimitExeeds(user_id, maxWebLimit, maxMobileLimit, app_access);

            return Json(status, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ViewPageMessage(string data)
        {
            User ObjUser = new User();
            PageMessage objMsg = new PageMessage();
            objMsg.message = data;
            return PartialView("_PageMessage", objMsg);
        }
        //public PartialViewResult GetChangePassword()
        //{
        //    var usrId = Convert.ToInt32(Session["user_id"]);

        //    return PartialView("_ChangePassword");
        //}
        //[HttpPost]
        //public JsonResult SavePassword(ChangePassword objChangePassword)
        //{
        //    JsonResponse<string> objResp = new JsonResponse<string>();
        //    int user_id = Convert.ToInt32(Session["user_id"]);
        //    BLUser objBLuser = new BLUser();
        //    User objUserDetails = objBLuser.getUserDetails(user_id);
        //    if (objChangePassword.currentPassword != Utility.MiscHelper.DecodeTo64(objUserDetails.password))
        //    {
        //        objResp.status = "Invalid";
        //        objResp.message = "Incorrect Current Password!";
        //    }
        //    else
        //    {
        //        objBLuser.saveChangePassword(Utility.MiscHelper.EncodeTo64(objChangePassword.confirmNewPassword), user_id);
        //        objResp.status = "Valid";
        //        objResp.message = "Password changed successfully"; ;
        //    }

        //    return Json(objResp, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ViewUsers(ViewUserModel objViewUser, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            string SearchVar = "";
            //if (sort != "" || page != 0 && (objViewUser.objGridAttributes.searchBy != "name" && objViewUser.objGridAttributes.searchBy != "user_email"))
            //{
            //    objViewUser.objGridAttributes = (CommonGridAttributes)Session["viewUserDetails"];
            //}


            if (sort != "" || page != 0)
            {
                objViewUser.objGridAttributes = (CommonGridAttributes)Session["viewUserDetails"];
            }


            objViewUser.lstSearchBy = GetUserSearchByColumns();
            objViewUser.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objViewUser.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objViewUser.objGridAttributes.sort = sort;
            objViewUser.objGridAttributes.orderBy = sortdir;


            if ((objViewUser.objGridAttributes.searchBy == "name" || objViewUser.objGridAttributes.searchBy == "user_email")
                && !string.IsNullOrEmpty(objViewUser.objGridAttributes.searchText))
            {
                SearchVar = objViewUser.objGridAttributes.searchText;
                objViewUser.objGridAttributes.searchText = "";
                objViewUser.objGridAttributes.currentPage = 0;
                //if(page<=0)
                //{
                //   
                //}
            }

            var user = new BLUser().GetUserList(objViewUser.objGridAttributes, objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);

            if (SearchVar != "" && objViewUser.objGridAttributes.searchBy == "name")
            {
                user = user.Where(c => MiscHelper.Decrypt(c.name).ToLower().Contains(SearchVar.ToLower())).ToList();
                objViewUser.objGridAttributes.pageSize = 1000;
            }
            else if (SearchVar != "" && objViewUser.objGridAttributes.searchBy == "user_email")
            {
                user = user.Where(c => MiscHelper.Decrypt(c.user_email).ToLower().Contains(SearchVar.ToLower())).ToList();
                objViewUser.objGridAttributes.pageSize = 1000;
            }



            foreach (var item in user)
            {
                item.user_email = MiscHelper.Decrypt(item.user_email);
                item.mobile_number = MiscHelper.Decrypt(item.mobile_number);
                item.name = MiscHelper.Decrypt(item.name);
            }
            objViewUser.lstUsers = user;

            objViewUser.loginId_UserId = objLgnUsrDtl.is_admin_rights_enabled ? objLgnUsrDtl.user_id : 0;
            //  objViewUser.objGridAttributes.totalRecord = objViewUser.lstUsers != null && objViewUser.lstUsers.Count > 0 ?  objViewUser.lstUsers[0].totalRecords : 0;
            objViewUser.objGridAttributes.totalRecord = objViewUser.lstUsers != null && objViewUser.lstUsers.Count > 0 ? SearchVar != "" ? objViewUser.lstUsers.Count : objViewUser.lstUsers[0].totalRecords : 0;
            Session["viewUserDetails"] = objViewUser.objGridAttributes;
            return View("ViewUsers", objViewUser);
        }

        public List<KeyValueDropDown> GetUserSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "User Name", value = "user_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Name", value = "name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Email", value = "user_email" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Role Name", value = "role_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Reporting Manager", value = "reporting_manager" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }

        public List<KeyValueDropDown> GetUserLoginHistorySearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "User Name", value = "user_name" });
            // lstSearchBy.Add(new KeyValueDropDown { key = "Email", value = "user_email" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Role Name", value = "role_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Mobile", value = "mobile_number" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Source", value = "source" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Browser Name", value = "browser_name" });
            return lstSearchBy.OrderBy(m => m.key).ToList();
        }

        public ActionResult ViewUserDetailUpdateMode(int id)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];

            User objUser = new User();
            objUser.lstRM = GetReportingManagers(); 
            objUser = new BLUser().GetUserDetailByID(id);
            objUser.password = Convert.ToString(Utility.MiscHelper.DecodeTo64(objUser.password));
            objUser.lstRole = new BLUser().GetAllRole(objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);
            objUser.IsUserinLimit = true;
            return View("AddUser", objUser);
        }

        [HttpPost]
        public JsonResult DeleteUserById(int id)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            int user_id = Convert.ToInt32(Session["user_id"]);
            User objUser = new BLUser().GetUserDetailByID(id);


            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {


                var output = new BLUser().DeleteUserById(id);
                if (output > 0)
                {


                    #region email sending start
                    objUser.lstRole = new BLUser().GetAllRole(objLgnUsrDtl.role_id, user_id);
                    string rolevalue = Convert.ToString(objUser.lstRole.FirstOrDefault(obj => obj.value == objUser.role_id.ToString()).key);
                    Dictionary<string, string> objUserDict = new Dictionary<string, string>();
                    objUserDict.Add("Name", MiscHelper.Decrypt(objUser.name));
                    objUserDict.Add("Username", objUser.user_name);
                    objUserDict.Add("UserRole", rolevalue);
                    objUserDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                    List<HttpPostedFileBase> objHttpPostedFileBase = null;
                    BLUser objBLuser = new BLUser();
                    List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.UserDeletion.ToString());
                    string managerEmailId = "";
                    List<KeyValueDropDown> objMangerDetail = new BLUser().GetManagerEmailId(objUser.user_id);
                    if (objMangerDetail != null)
                    {
                        int cnt = 0;
                        foreach (KeyValueDropDown objList in objMangerDetail)
                        {
                            if (cnt == 0)
                                managerEmailId = MiscHelper.Decrypt(objMangerDetail[cnt].value.ToString());
                            else
                                managerEmailId = managerEmailId + "," + MiscHelper.Decrypt(objMangerDetail[cnt].value.ToString());
                            cnt++;
                        }
                        objEventEmailTemplateDetail[0].recipient_list = managerEmailId;
                    }
                    System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objUserDict, objHttpPostedFileBase, EmailSettings.AllEmailSettings, null, "", EmailEventList.UserDeletion.ToString()));
                    //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objUserDict, objHttpPostedFileBase);
                    #endregion

                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = "User Deleted successfully!";
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = "Something went wrong while deleting User!";
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "User not deleted!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        #region sapna
        public List<KeyValueDropDown> GetReportingManagers(int RoleId = 0)
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            User objUser = new User();
            return objUser.lstRM = new BLUser().BindReportingManager(RoleId, user_id).OrderBy(m => m.key).ToList();
        }
        public JsonResult BindWarehouseCodeOnChange()
        {
            var objResp = new BLUser().BindWarehouseCode();
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }

        public JsonResult BindReportingManagerOnChange(int RoleId)
        {
            var objResp = GetReportingManagers(RoleId);
            return Json(new { Data = objResp, JsonRequestBehavior.AllowGet });
        }
        #endregion

        #region Nihal
        public ActionResult ViewUsersLoginHistory(UserLoginHistoryVM objUserLoginHistoryVM, int page = 0, string sort = "", string sortdir = "")
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            string Searchvar = "";
            if (sort != "" || page != 0)
            {
                objUserLoginHistoryVM.objGridAttributes = (CommonGridAttributes)Session["viewUserLoginHistoryFilter"];
            }
            objUserLoginHistoryVM.lstSearchBy = GetUserLoginHistorySearchByColumns();
            objUserLoginHistoryVM.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            objUserLoginHistoryVM.objGridAttributes.currentPage = page == 0 ? 1 : page;
            objUserLoginHistoryVM.objGridAttributes.sort = sort;
            objUserLoginHistoryVM.objGridAttributes.orderBy = sortdir;
            //pk

            if ((objUserLoginHistoryVM.objGridAttributes.searchBy == "user_email" || objUserLoginHistoryVM.objGridAttributes.searchBy == "mobile_number")
                && !string.IsNullOrEmpty(objUserLoginHistoryVM.objGridAttributes.searchText))
            {
                Searchvar = objUserLoginHistoryVM.objGridAttributes.searchText;
                objUserLoginHistoryVM.objGridAttributes.searchText = "";
                objUserLoginHistoryVM.objGridAttributes.currentPage = 0;
                objUserLoginHistoryVM.objGridAttributes.pageSize = 15;
            }
            //pkend


            var user = new BLUser().GetUserLoginHistoryList(objUserLoginHistoryVM.objGridAttributes, objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);


            //pk
            if (Searchvar != "" && objUserLoginHistoryVM.objGridAttributes.searchBy == "mobile_number")
            {

                user = user.Where(c => MiscHelper.Decrypt(c.mobile_number).Contains(Searchvar)).ToList();
                objUserLoginHistoryVM.objGridAttributes.pageSize = 15;
            }
            if (Searchvar != "" && objUserLoginHistoryVM.objGridAttributes.searchBy == "user_email")
            {

                user = user.Where(c => MiscHelper.Decrypt(c.user_email).Contains(Searchvar)).ToList();
                objUserLoginHistoryVM.objGridAttributes.pageSize = 15;

            }
            //pk end


            foreach (var item in user)
            {
                item.user_email = MiscHelper.Decrypt(item.user_email);
                item.mobile_number = MiscHelper.Decrypt(item.mobile_number);
            }
            objUserLoginHistoryVM.lstUsers = user;
            //pk
            objUserLoginHistoryVM.loginId_UserId = objLgnUsrDtl.is_admin_rights_enabled ? objLgnUsrDtl.user_id : 0;
            objUserLoginHistoryVM.objGridAttributes.totalRecord = objUserLoginHistoryVM.lstUsers != null && objUserLoginHistoryVM.lstUsers.Count > 0 ? Searchvar != "" ? objUserLoginHistoryVM.lstUsers.Count : objUserLoginHistoryVM.lstUsers[0].totalRecords : 0;
            //pkend
            Session["viewUserLoginHistoryFilter"] = objUserLoginHistoryVM.objGridAttributes;
            return View("ViewUsersLoginHistory", objUserLoginHistoryVM);
        }

        [HttpGet]
        public void DownloadUserLoginHistory()
        {
            if (Session["viewUserLoginHistoryFilter"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                CommonGridAttributes objViewFilter = (CommonGridAttributes)Session["viewUserLoginHistoryFilter"];
                List<UserLoginHistory> lstUserLoginHistory = new List<UserLoginHistory>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstUserLoginHistory = new BLUser().GetUserLoginHistoryList(objViewFilter, objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);

                DataTable dtReport = new DataTable();
                // SurveyAssignment
                dtReport = MiscHelper.ListToDataTable<UserLoginHistory>(lstUserLoginHistory);

                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["APPLICATION_ACCESS"] = dtReport.Rows[i]["APPLICATION_ACCESS"].ToString().Substring(0, 1).ToUpper() + dtReport.Rows[i]["APPLICATION_ACCESS"].ToString().Substring(1).ToLower();
                    dtReport.Rows[i]["user_email"] = MiscHelper.Decrypt(dtReport.Rows[i]["user_email"].ToString());
                    dtReport.Rows[i]["mobile_number"] = MiscHelper.Decrypt(dtReport.Rows[i]["mobile_number"].ToString());
                }
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("TOTALRECORDS");
                var filename = "UserLoginHistory";
                ExportAssignmentData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));


            }
        }

        private void ExportAssignmentData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        public static string entityExportToKml()
        {
            string resp = "{\"status\":false,\"data\":\"\"}";
            StringBuilder sbLine = new StringBuilder();
            StringBuilder sbPoint = new StringBuilder();
            StringBuilder sbPolygon = new StringBuilder();
            sbLine.Append("<Folder>");
            sbPoint.Append("<Folder>");
            sbPolygon.Append("<Folder>");
            try
            {
                BLLayer objBLLayer = new BLLayer();
                List<Kmp> lstkmp = new List<Kmp>();
                lstkmp = objBLLayer.Getkmp("polygon");
                IDataReader dr = (IDataReader)lstkmp;
                string jsonData = "";
                while (dr.Read())
                {
                    if (dr[1].ToString() == "line")
                    {
                        sbLine.Append("<Placemark><description>" + dr[3].ToString() + "</description><name>" + dr[4].ToString() + "</name>");
                        if (dr[4].ToString().ToLower() == "logicallink")
                            sbLine.Append("<styleUrl>#logical</styleUrl><LineString><coordinates>");
                        else
                            sbLine.Append("<styleUrl>#cable</styleUrl><LineString><coordinates>");
                        string temp = dr[2].ToString();
                        string t = temp.Substring(11, temp.Length - 13);
                        string[] x = t.Split(',');
                        foreach (string y in x)
                        {
                            sbLine.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                        }

                        sbLine.Append("</coordinates></LineString></Placemark>");
                    }
                    if (dr[1].ToString() == "point")
                    {
                        sbPoint.Append("<Placemark><name>" + dr[4].ToString() + "(" + dr[3].ToString() + ")" + "</name>");
                        sbPoint.Append("<styleUrl>#downArrowIcon</styleUrl><Point><coordinates>");
                        string temp = dr[2].ToString();
                        string t = temp.Substring(6, temp.Length - 8);
                        sbPoint.Append(t.Split(' ')[0] + "," + t.Split(' ')[1] + "," + 0 + " ");
                        sbPoint.Append("</coordinates></Point></Placemark>");

                    }
                    if (dr[1].ToString() == "polygon")
                    {
                        sbPolygon.Append("<Placemark><name>" + dr[4].ToString() + "</name><description>" + dr[3].ToString() + "</description><styleUrl>#transGreenPoly</styleUrl>");
                        sbPolygon.Append("<Polygon><outerBoundaryIs><LinearRing><coordinates>");
                        string temp = dr[2].ToString();
                        string t = temp.Substring(9, temp.Length - 12);
                        string[] x = t.Split(',');
                        foreach (string y in x)
                        {
                            sbPolygon.Append(y.Split(' ')[0] + "," + y.Split(' ')[1] + "," + 0 + " ");
                        }

                        sbPolygon.Append("</coordinates></LinearRing></outerBoundaryIs></Polygon></Placemark>");
                    }
                }

                sbLine.Append("</Folder>");
                sbPoint.Append("</Folder>");
                sbPolygon.Append("</Folder>");
                string k_path = System.Configuration.ConfigurationManager.AppSettings["KmlPath"];
                string root = AppDomain.CurrentDomain.BaseDirectory;
                string file_path = root + k_path;

                if (!Directory.Exists(file_path))
                    Directory.CreateDirectory(file_path);
                //else Array.ForEach(Directory.GetFiles(file_path));

                string abc = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                            "<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">" +
                            "<Document>  <!-- Begin Style Definitions -->" +
                            "<Style id=\"cable\"><LineStyle><color>55F0AA14</color><width>4</width></LineStyle></Style>" +
                            "<Style id=\"logical\"><LineStyle><color>501478FF</color><width>4</width></LineStyle></Style>" +
                            "<Style id=\"transGreenPoly\"><LineStyle><color>501E78F0</color><width>2</width></LineStyle><PolyStyle><color>501E78F0</color></PolyStyle>" +
                            //"</Style><Style id=\"downArrowIcon\"><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href></Icon></IconStyle></Style>" + 
                            "</Style><Style id=\"downArrowIcon\"><IconStyle><hotSpot x=\"20\" y=\"2\" xunits=\"pixels\" yunits=\"pixels\"/></IconStyle></Style>" +
                            sbPoint.ToString() + sbLine.ToString() + sbPolygon.ToString() + "</Document></kml>";

                System.IO.File.WriteAllText(file_path + @"\userkml.kml", abc);

                if (!dr.IsClosed)
                    dr.Close();
                resp = "{\"status\":true,\"data\":\"" + (file_path + @"\WriteText.kml").Replace(@"\", @"\\") + "\"}";
            }
            catch
            {
                resp = "{\"status\":false,\"data\":\"\"}";
            }
            return resp;
        }
        #endregion

        #region Rahul
        [HttpGet]
        public void DownloadViewUserDetail()
        {
            if (Session["viewUserDetails"] != null)
            {
                var objLgnUsrDtl = (User)Session["userDetail"];
                CommonGridAttributes objViewFilter = (CommonGridAttributes)Session["viewUserDetails"];
                List<UserDetail> lstViewUserDetails = new List<UserDetail>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstViewUserDetails = new BLUser().GetUserList(objViewFilter, objLgnUsrDtl.role_id, objLgnUsrDtl.user_id);

                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<UserDetail>(lstViewUserDetails);
                dtReport.Columns.Add("Status", typeof(string));
                dtReport.Columns.Add("Created On", typeof(string));
                dtReport.Columns.Add("Modified On", typeof(string));

                for (int i = 0; i < dtReport.Rows.Count; i++)
                {
                    dtReport.Rows[i]["APPLICATION_ACCESS"] = dtReport.Rows[i]["APPLICATION_ACCESS"].ToString().Substring(0, 1).ToUpper() + dtReport.Rows[i]["APPLICATION_ACCESS"].ToString().Substring(1).ToLower();
                    dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                    dtReport.Rows[i]["Created On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                    dtReport.Rows[i]["Modified On"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                    dtReport.Rows[i]["user_email"] = MiscHelper.Decrypt(dtReport.Rows[i]["user_email"].ToString());
                    dtReport.Rows[i]["mobile_number"] = MiscHelper.Decrypt(dtReport.Rows[i]["mobile_number"].ToString());
                    dtReport.Rows[i]["name"] = MiscHelper.Decrypt(dtReport.Rows[i]["name"].ToString());
                }
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("PASSWORD");
                dtReport.Columns.Remove("ISACTIVE");
                dtReport.Columns.Remove("MANAGER_ID");
                dtReport.Columns.Remove("ROLE_ID");
                dtReport.Columns.Remove("MODULE_ID");
                dtReport.Columns.Remove("USER_IMG");
                dtReport.Columns.Remove("TEMPLATE_ID");
                dtReport.Columns.Remove("GROUP_ID");
                dtReport.Columns.Remove("IS_DELETED");
                dtReport.Columns.Remove("REMARKS");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("TOTALRECORDS");
                dtReport.Columns.Remove("MODIFIED_BY");
                dtReport.Columns.Remove("IS_ACTIVE");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("MODIFIED_ON");
                dtReport.Columns.Remove("HISTORY_ID");

                dtReport.Columns["s_no"].SetOrdinal(0);
                dtReport.Columns["user_name"].SetOrdinal(1);
                dtReport.Columns["name"].SetOrdinal(2);
                dtReport.Columns["user_email"].SetOrdinal(3);
                dtReport.Columns["role_name"].SetOrdinal(4);
                dtReport.Columns["mobile_number"].SetOrdinal(5);
                dtReport.Columns["APPLICATION_ACCESS"].SetOrdinal(6);
                dtReport.Columns["user_type"].SetOrdinal(7);
                dtReport.Columns["status"].SetOrdinal(8);
                dtReport.Columns["created_by_text"].SetOrdinal(9);
                dtReport.Columns["created on"].SetOrdinal(10);
                dtReport.Columns["modified_by_text"].SetOrdinal(11);
                dtReport.Columns["modified on"].SetOrdinal(12);



                dtReport.Columns["user_name"].ColumnName = "User Name";
                dtReport.Columns["name"].ColumnName = "Name";
                dtReport.Columns["user_email"].ColumnName = "Email";
                dtReport.Columns["role_name"].ColumnName = "Role Name";
                dtReport.Columns["created_by_text"].ColumnName = "Created By";
                dtReport.Columns["modified_by_text"].ColumnName = "Modified By";
                dtReport.Columns["mobile_number"].ColumnName = "Mobile No.";
                dtReport.Columns["APPLICATION_ACCESS"].ColumnName = "Application Status";
                dtReport.Columns["user_type"].ColumnName = "User Type";
                var filename = "UserDetails";
                ExportAssignmentData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }
        #endregion
        #region Nitya
        public List<SelectListItem> bindRegionDropDown(int[] selRegions)
        {
            List<SelectListItem> lstRegion = new List<SelectListItem>();
            List<RegionProvinceLayer> objRegionProvince = new List<RegionProvinceLayer>();
            objRegionProvince = new BLLayer().GetRegionProvinceLayers(0);
            if (objRegionProvince.Count > 0)
            {
                lstRegion = objRegionProvince.Select(m => new SelectListItem() { Text = m.regionName, Value = m.regionId.ToString(), Selected = (selRegions == null) ? false : selRegions.Contains(m.regionId) }).ToList();
            }
            return lstRegion;
        }

        public List<SelectListItem> bindCountryDropDown(int[] selRegions)
        {
            List<SelectListItem> lstCountry = new List<SelectListItem>();
            List<RegionProvinceLayer> objRegionProvince = new List<RegionProvinceLayer>();
            objRegionProvince = new BLLayer().GetRegionProvinceLayers(0);

            var countrylist = objRegionProvince.GroupBy(x => x.countryname.ToLower()).Select(y => y.First()).ToList();

            if (countrylist.Count > 0)
            {
                var selectedCountryList = new List<string>();
                if (selRegions != null)
                {
                    selectedCountryList = objRegionProvince.Where(m => selRegions.Contains(m.regionId)).Select(x => x.countryname).Distinct().ToList();
                }
                lstCountry = countrylist.Select(m => new SelectListItem() { Text = m.countryname, Value = m.countryname, Selected = (selectedCountryList == null) ? false : selectedCountryList.Contains(m.countryname) }).ToList();
            }
            return lstCountry;
        }

        public JsonResult GetProvinceByRegionid(string regionid)
        {
            List<Province> lstProvince = new BLLayer().GetProvincebyRegionID(regionid);
            return Json(lstProvince, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRegionByCountryid(string countryName)
        {
            var objRegionProvince = new BLLayer().GetRegionProvinceLayers(0);
            List<Region> lstRegion = objRegionProvince.Where(x => countryName.ToLower().Contains(x.countryname.ToLower())).Select(x => new Region { regionId = x.regionId, regionName = x.regionName, regionAbbr = x.regionAbbr }).ToList();
            return Json(lstRegion, JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult GetUserHistory(int systemId = 0, int page = 0, string sort = "", string sortdir = "")
        {
            if (page <= 0)
            {
                Session["ViewUserHistory"] = null;
            }
            ViewAuditMasterModel objAudit = new ViewAuditMasterModel();
            objAudit.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objAudit.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objAudit.objFilterAttributes.sort = sort == "" ? "audit_id" : sort;
            objAudit.objFilterAttributes.orderBy = sortdir == "" ? "desc" : sortdir;
            objAudit.objFilterAttributes.systemid = systemId;
            //  objAudit.objFilterAttributes.entityType = eType;

            if (Session["ViewUserHistory"] != null)
            {
                var sessionValues = (FilterHistoryAttr)Session["ViewUserHistory"];
                objAudit.objFilterAttributes.systemid = sessionValues.systemid;
                // objAudit.objFilterAttributes.entityType = sessionValues.entityType;
            }
            List<Dictionary<string, string>> lstReportData = new BLUser().GetUserHistoryDetailById(objAudit.objFilterAttributes);
            foreach (Dictionary<string, string> dic in lstReportData)
            {
                var obj = (IDictionary<string, object>)new System.Dynamic.ExpandoObject();
                string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
                foreach (var col in dic)
                {
                    if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                    {
                        if (col.Key.ToUpper() == "PASSWORD")
                        {
                            var pass = Convert.ToString(Utility.MiscHelper.DecodeTo64(string.IsNullOrWhiteSpace(col.Value) ? "12345678" : col.Value));
                            obj.Add(col.Key, pass);
                        }
                        else if (col.Key.ToUpper() == "NAME")
                        {
                            var name = MiscHelper.Decrypt(col.Value);
                            obj.Add(col.Key, name);
                        }
                        else if (col.Key.ToUpper() == "USER_EMAIL")
                        {
                            var email = MiscHelper.Decrypt(col.Value);
                            obj.Add(col.Key, email);
                        }
                        else if (col.Key.ToUpper() == "MOBILE_NUMBER")
                        {
                            var mobile = MiscHelper.Decrypt(col.Value);
                            obj.Add(col.Key, mobile);
                        }
                        else
                        {
                            obj.Add(col.Key, col.Value);
                        }
                    }

                }
                objAudit.lstData.Add(obj);
            }
            objAudit.objFilterAttributes.totalRecord = lstReportData.Count > 0 ? Convert.ToInt32(lstReportData[0].FirstOrDefault().Value) : 0;
            Session["ViewUserHistory"] = objAudit.objFilterAttributes;
            return PartialView("ViewUserHistory", objAudit);
        }

        public void DownloadUserHistory()
        {
            if (Session["ViewUserHistory"] != null)
            {
                FilterHistoryAttr objViewFilter = (FilterHistoryAttr)Session["ViewUserHistory"];
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                List<Dictionary<string, string>> lstReportData = new BLUser().GetUserHistoryDetailById(objViewFilter);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.GetDataTableFromDictionaries(lstReportData);
                dtReport.TableName = "User_History";
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (dtReport.Columns.Contains("totalrecords")) { dtReport.Columns.Remove("totalrecords"); }
                    if (dtReport.Columns.Contains("S_No")) { dtReport.Columns.Remove("S_No"); }

                }
                if (dtReport.Rows.Count > 0)
                {
                    dtReport.Columns.Add("Status", typeof(string));
                    dtReport.Columns.Add("Has Admin Rights", typeof(string));
                    for (int i = 0; i < dtReport.Rows.Count; i++)
                    {
                        dtReport.Rows[i]["created_on"] = !String.IsNullOrEmpty(dtReport.Rows[i]["created_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["created_on"].ToString()) : dtReport.Rows[i]["created_on"];
                        dtReport.Rows[i]["modified_on"] = !String.IsNullOrEmpty(dtReport.Rows[i]["modified_on"].ToString()) ? MiscHelper.FormatDateTime(dtReport.Rows[i]["modified_on"].ToString()) : dtReport.Rows[i]["modified_on"];
                        dtReport.Rows[i]["Status"] = Convert.ToBoolean(dtReport.Rows[i]["is_active"]) == true ? "Active" : "InActive";
                        dtReport.Rows[i]["Has Admin Rights"] = Convert.ToBoolean(dtReport.Rows[i]["is_admin_rights_enabled"]) == true ? "Yes" : "No";
                        dtReport.Rows[i]["user_email"] = MiscHelper.Decrypt(dtReport.Rows[i]["user_email"].ToString());
                        dtReport.Rows[i]["mobile_number"] = MiscHelper.Decrypt(dtReport.Rows[i]["mobile_number"].ToString());
                        dtReport.Rows[i]["name"] = MiscHelper.Decrypt(dtReport.Rows[i]["name"].ToString());
                    }
                    dtReport.Columns.Remove("is_active");
                    dtReport.Columns.Remove("is_admin_rights_enabled");

                    dtReport.Columns["user_name"].SetOrdinal(0);
                    dtReport.Columns["name"].SetOrdinal(1);
                    dtReport.Columns["user_email"].SetOrdinal(2);
                    dtReport.Columns["role_name"].SetOrdinal(3);
                    dtReport.Columns["reporting_manager"].SetOrdinal(4);
                    dtReport.Columns["mobile_number"].SetOrdinal(5);
                    dtReport.Columns["password"].SetOrdinal(6);
                    dtReport.Columns["application_access"].SetOrdinal(7);
                    dtReport.Columns["Has Admin Rights"].SetOrdinal(8);
                    dtReport.Columns["Status"].SetOrdinal(10);
                    dtReport.Columns["user_type"].SetOrdinal(9);


                    dtReport.Columns["mobile_number"].ColumnName = "Mobile Number";
                    dtReport.Columns["user_name"].ColumnName = "User Name";
                    dtReport.Columns["name"].ColumnName = "Name";
                    dtReport.Columns["user_email"].ColumnName = "Email";
                    dtReport.Columns["application_access"].ColumnName = "Application Access";
                    dtReport.Columns["role_name"].ColumnName = "Role Name";
                    dtReport.Columns["reporting_manager"].ColumnName = "Reporting Manager";
                    dtReport.Columns["created_by"].ColumnName = "Created By";
                    dtReport.Columns["created_on"].ColumnName = "Created On";
                    dtReport.Columns["modified_by"].ColumnName = "Modified By";
                    dtReport.Columns["modified_on"].ColumnName = "Modified On";
                    dtReport.Columns["user_type"].ColumnName = "User Type";
                    ExportVendorSpecificationData(dtReport, "" + dtReport.TableName.ToString() + "_" + Utility.MiscHelper.getTimeStamp());
                }
            }
        }
        private void ExportVendorSpecificationData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        public ActionResult EditUserStatus(int user_id, bool status)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            int userid = Convert.ToInt32(Session["user_id"]);
            User objUser = new BLUser().GetUserDetailByID(user_id);
            var Result = false;
            PageMessage objPM = new PageMessage();
            Result = new BLUser().EditUserStatus(user_id, status);
            if (Result)
            {
                //UpdateAllKeyValue();
                objPM.status = ResponseStatus.OK.ToString();
                if (status == true) { objPM.message = "User Activated successfully."; }
                else
                {
                    #region email sending start
                    objUser.lstRole = new BLUser().GetAllRole(objLgnUsrDtl.role_id, userid);
                    string rolevalue = Convert.ToString(objUser.lstRole.FirstOrDefault(obj => obj.value == objUser.role_id.ToString()).key);
                    Dictionary<string, string> objUserDict = new Dictionary<string, string>();
                    objUserDict.Add("Name", MiscHelper.Decrypt(objUser.name));
                    objUserDict.Add("Username", objUser.user_name);
                    objUserDict.Add("UserRole", rolevalue);
                    objUserDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                    List<HttpPostedFileBase> objHttpPostedFileBase = null;
                    BLUser objBLuser = new BLUser();
                    List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.UserDeletion.ToString());
                    string managerEmailId = "";

                    List<KeyValueDropDown> objMangerDetail = new BLUser().GetManagerEmailId(objUser.user_id);
                    if (objMangerDetail != null)
                    {
                        int cnt = 0;
                        foreach (KeyValueDropDown objList in objMangerDetail)
                        {
                            if (cnt == 0)
                                managerEmailId = MiscHelper.Decrypt(objMangerDetail[cnt].value.ToString());
                            else
                                managerEmailId = managerEmailId + "," + MiscHelper.Decrypt(objMangerDetail[cnt].value.ToString());
                            cnt++;
                        }
                        if (cnt > 0)
                            objEventEmailTemplateDetail[0].recipient_list = managerEmailId;
                    }
                    System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objUserDict, objHttpPostedFileBase,EmailSettings.AllEmailSettings, null,"", EmailEventList.UserDeletion.ToString()));
                    //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objUserDict, objHttpPostedFileBase);
                    #endregion

                    objPM.message = "User De-activated successfully.";
                }

                ViewBag.message = objPM.message;
            }
            else
            {
                objPM.status = ResponseStatus.FAILED.ToString();
                objPM.message = "Failed to Update User Status";
                ViewBag.message = objPM.message;
            }
            return Json(objPM, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LogoutUser(int user_id = 0, string application_access = "")
        {
            DbMessage Result = new DbMessage();
            Result = new BLUserLogin().LogoutUser(user_id, application_access);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        #region BulkUserUpload
        [HttpGet]
        public ActionResult BulkUserUpload(ViewBulkUserUploadSummary bulkUserUploadSummary, int page = 0, string sort = "", string sortdir = "")
        {

            var objLgnUsrDtl = (User)Session["userDetail"];
            PageMessage pg = new PageMessage();

            if (sort != "" || page != 0)
            {
                //lstBulkUserUploadSummary.objGridAttributes = (CommonGridAttributes)Session["viewUserDetails"];
            }
            //objViewUser.lstSearchBy = GetUserSearchByColumns();
            bulkUserUploadSummary.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            bulkUserUploadSummary.objGridAttributes.currentPage = page == 0 ? 1 : page;
            bulkUserUploadSummary.objGridAttributes.sort = sort;
            bulkUserUploadSummary.objGridAttributes.orderBy = sortdir;
            bulkUserUploadSummary.lstBulkUserUploadSummary = new BLBulkUserUploadSummary().GetBulkUserUploadSummary(bulkUserUploadSummary.objGridAttributes);
            bulkUserUploadSummary.loginId_UserId = objLgnUsrDtl.is_admin_rights_enabled ? objLgnUsrDtl.user_id : 0;
            bulkUserUploadSummary.objGridAttributes.totalRecord = bulkUserUploadSummary.lstBulkUserUploadSummary != null && bulkUserUploadSummary.lstBulkUserUploadSummary.Count > 0 ? bulkUserUploadSummary.lstBulkUserUploadSummary[0].grid_total_record : 0;
            bulkUserUploadSummary.IsSubmit = false;

            bulkUserUploadSummary.pageMsg = pg;

            bulkUserUploadSummary.loginId_UserId = objLgnUsrDtl.user_id;
            bulkUserUploadSummary.access_token = ((TokenDetail)Session["TokenDetail"]).access_token;
            return View("BulkUploadUsers", bulkUserUploadSummary);
        }

        [HttpPost]
        public ActionResult BulkUserUpload(HttpPostedFileBase postedFile)
        {
            var objLgnUsrDtl = (User)Session["userDetail"];
            int login_user_id = objLgnUsrDtl.user_id;
            ViewBulkUserUploadSummary bulkUserUploadSummary = new ViewBulkUserUploadSummary();
            bulkUserUploadSummary.IsSubmit = true;
            PageMessage pg = new PageMessage();
            bulkUserUploadSummary.IsSubmit = true;
            BulkUserUploadSummary userUploadSummary = new BulkUserUploadSummary();
            int page = 0; string sort = ""; string sortdir = "";

            if (sort != "" || page != 0)
            {
                //lstBulkUserUploadSummary.objGridAttributes = (CommonGridAttributes)Session["viewUserDetails"];
            }
            //objViewUser.lstSearchBy = GetUserSearchByColumns();
            bulkUserUploadSummary.objGridAttributes.pageSize = ApplicationSettings.ViewAdminDashboardGridPageSize;
            bulkUserUploadSummary.objGridAttributes.currentPage = page == 0 ? 1 : page;
            bulkUserUploadSummary.objGridAttributes.sort = sort;
            bulkUserUploadSummary.objGridAttributes.orderBy = sortdir;
            bulkUserUploadSummary.loginId_UserId = objLgnUsrDtl.is_admin_rights_enabled ? objLgnUsrDtl.user_id : 0;

            try
            {
                if (ModelState.IsValid)
                {
                    //if (postedFile != null && postedFile.ContentLength > (1024 * 1024 * 50))  // 50MB limit  
                    //{
                    //    ModelState.AddModelError("postedFile", "Your file is to large. Maximum size allowed is 50MB !");
                    //}
                    //else
                    //{
                    ////save the uploaded file
                    string filePath = string.Empty;
                    string path = Server.MapPath("~/Uploads/User");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //string datefomat = DateTime.Now.ToString("yyyy'_'MM'_'dd'_'HH'_'mm'_'ss");
                    string datefomat = DateTime.Now.ToString("ddMMyyyy_HHmmss");
                    string extension = Path.GetExtension(postedFile.FileName);
                    string filename = Path.GetFileNameWithoutExtension(postedFile.FileName) + "_" + datefomat + extension;
                    filePath = Path.Combine(path, filename);
                    postedFile.SaveAs(filePath);




                    bool isheader = true;

                    DataTable dtUserDetails = NPOIExcelHelper.ExcelToTable(filePath, "User Details", out isheader);
                    DataTable dtWebModule = NPOIExcelHelper.ExcelToTable(filePath, "Web Module", out isheader);
                    DataTable dtMobileModule = NPOIExcelHelper.ExcelToTable(filePath, "Mobile Module", out isheader);
                    DataTable dtAdminModule = NPOIExcelHelper.ExcelToTable(filePath, "Admin Module", out isheader);
                    DataTable dtWorkAreaDetails = NPOIExcelHelper.ExcelToTable(filePath, "Work Area Details", out isheader);


                    userUploadSummary.file_name = filename;
                    userUploadSummary.created_by = login_user_id;
                    userUploadSummary.status = "OK";
                    userUploadSummary.err_description = "";
                    userUploadSummary.total_record = dtUserDetails.Rows.Count;

                    userUploadSummary = new BLBulkUserUploadSummary().SaveBulkUserUploadSummary(userUploadSummary);

                    ////for Creating the User Object
                    if (dtUserDetails != null && dtUserDetails.Rows.Count > 0)
                    {
                        List<BulkUserUpload> lstUser = dtUserDetails.AsEnumerable().Select(item => new BulkUserUpload
                        {
                            user_upload_id = userUploadSummary.id,
                            user_name = Convert.ToString(item.Field<string>("User Name")).Trim(),
                            name = Convert.ToString(item.Field<string>("Full Name")).Trim(),
                            user_email = Convert.ToString(item.Field<string>("Email ID")).Trim(),
                            password = Convert.ToString(item.Field<string>("Password")).Trim(),
                            mobile_number = Convert.ToString(item.Field<string>("Mobile number")).Trim(),
                            role_name = Convert.ToString(item.Field<string>("User Role")).Trim(),
                            reporting_manager = Convert.ToString(item.Field<string>("Reporting Manager")).Trim(),
                            application_access = Convert.ToString(item.Field<string>("Application Access")).ToUpper().Trim(),
                            is_admin_rights_enabled = Convert.ToString(item.Field<string>("is Admin right Allowed")).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false,
                            is_active = Convert.ToString(item.Field<string>("Is Active")).Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) ? true : false,
                            created_by = login_user_id,
                            user_img = "user.png",
                            user_type = Convert.ToString(item.Field<string>("User Type")).Trim(),
                            pan = Convert.ToString(item.Field<string>("PAN")).Trim().ToUpper(),
                            prms_id = Convert.ToString(item.Field<string>("PRMS Id")).Trim(),
                            vendor_id = Convert.ToString(item.Field<string>("Vendor Id")).Trim()

                        }).ToList<BulkUserUpload>();

                        ////Validation code goes here 
                        lstUser = ValidateUserUploadedFile(lstUser);
                        new BLBulkUserUpload().SaveBulkUserUpload(lstUser);
                        //check for both type of user limit 
                        BulkUserUploadLimit bulkUserUploadLimit = CheckBulkUploadUserLimit(userUploadSummary.id);
                        if (bulkUserUploadLimit.status)
                        {

                            List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping = new BLBulkUserUploadModuleMapping().GetBulkUserUploadModuleMasterMapping();

                            ////insert user module mapping details
                            List<BulkUserUploadModuleMapping> lstUserUploadModuleMapping = new List<BulkUserUploadModuleMapping>();
                            CreateUserUploadMobileModuleMapping(userUploadSummary.id, dtMobileModule, "Mobile", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
                            CreateUserUploadWebModuleMapping(userUploadSummary.id, dtWebModule, "Web", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
                            CreateUserUploadAdminModuleMapping(userUploadSummary.id, dtAdminModule, "Admin", lstUserUploadModuleMapping, lstBulkUserUploadModuleMasterMapping);
                            new BLBulkUserUploadModuleMapping().SaveBulkUserUploadModuleMapping(lstUserUploadModuleMapping);

                            ////insert user work area details
                            if (dtWorkAreaDetails != null && dtWorkAreaDetails.Rows.Count > 0)
                            {
                                List<BulkUserUploadWorkAreaDetail> lstUserUploadWorkAreaDetail = dtWorkAreaDetails.AsEnumerable().Select(item => new BulkUserUploadWorkAreaDetail
                                {
                                    user_upload_id = userUploadSummary.id,
                                    user_name = Convert.ToString(item.Field<string>("User Name")).Trim(),
                                    country_name = Convert.ToString(item.Field<string>("Country Name")).Trim(),
                                    region_name = Convert.ToString(item.Field<string>("Region Name")).Trim(),
                                    province_name = Convert.ToString(item.Field<string>("Province Name")).Trim()
                                }).ToList<BulkUserUploadWorkAreaDetail>();
                                new BLBulkUserUploadWorkAreaDetail().SaveBulkUserUploadWorkAreaDetail(lstUserUploadWorkAreaDetail);
                            }
                            new BLBulkUserUpload().ProcessBulkUserUpload(userUploadSummary.id);
                        }
                        else
                        {
                            userUploadSummary.err_description = bulkUserUploadLimit.message;
                            userUploadSummary = new BLBulkUserUploadSummary().SaveBulkUserUploadSummary(userUploadSummary);
                        }
                    }
                    else
                    {
                        userUploadSummary.err_description = "Records not found";
                        userUploadSummary = new BLBulkUserUploadSummary().SaveBulkUserUploadSummary(userUploadSummary);
                    }
                    //}

                }

                bulkUserUploadSummary.lstBulkUserUploadSummary = new BLBulkUserUploadSummary().GetBulkUserUploadSummary(bulkUserUploadSummary.objGridAttributes);
                bulkUserUploadSummary.objGridAttributes.totalRecord = bulkUserUploadSummary.lstBulkUserUploadSummary != null && bulkUserUploadSummary.lstBulkUserUploadSummary.Count > 0 ? bulkUserUploadSummary.lstBulkUserUploadSummary[0].grid_total_record : 0;

                pg.message = "Bulk user uploaded successfully.";
                pg.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("Cannot perform runtime binding on a null reference"))
                {
                    pg.message = "Invalid Excel template";
                }
                else
                {
                    pg.message = "Error while processing request.";
                }
                userUploadSummary.err_description = pg.message;
                userUploadSummary = new BLBulkUserUploadSummary().SaveBulkUserUploadSummary(userUploadSummary);
                pg.status = ResponseStatus.ERROR.ToString();
                ErrorLogHelper.WriteErrorLog("BulkUserUpload", "UserController", ex);
            }
            bulkUserUploadSummary.lstBulkUserUploadSummary = new BLBulkUserUploadSummary().GetBulkUserUploadSummary(bulkUserUploadSummary.objGridAttributes);
            bulkUserUploadSummary.objGridAttributes.totalRecord = bulkUserUploadSummary.lstBulkUserUploadSummary != null && bulkUserUploadSummary.lstBulkUserUploadSummary.Count > 0 ? bulkUserUploadSummary.lstBulkUserUploadSummary[0].grid_total_record : 0;

            bulkUserUploadSummary.loginId_UserId = objLgnUsrDtl.user_id;
            bulkUserUploadSummary.access_token = ((TokenDetail)Session["TokenDetail"]).access_token;
            bulkUserUploadSummary.pageMsg = pg;
            return View("BulkUploadUsers", bulkUserUploadSummary);
        }

        private List<BulkUserUpload> ValidateUserUploadedFile(List<BulkUserUpload> lstUploadUser)
        {
            List<BulkUserUpload> lsttempUploadUser = new List<BulkUserUpload>();
            try
            {
                foreach (BulkUserUpload objUser in lstUploadUser)
                {
                    string err_message = string.Empty;
                    string reg_err_message = string.Empty;
                    ////Check user validation
                    if (!string.IsNullOrEmpty(objUser.user_name) && !string.IsNullOrEmpty(objUser.user_email))
                    {
                        User obj = new User();
                        obj.user_name = objUser.user_name;
                        obj.user_email = objUser.user_email;
                        var objUserExist = new BLUser().ChkUserExist(obj);
                        if (objUserExist != null)
                        {
                            objUserExist.user_email = MiscHelper.Decrypt(objUserExist.user_email);
                        }
                        if ((objUserExist != null && objUser.user_id != objUserExist.user_id))
                        {
                            if (objUserExist.user_name.Trim().ToLower() == objUser.user_name.Trim().ToLower())
                            {
                                err_message = " User Name already exist!";
                            }
                            else if (objUserExist.user_email.Trim().ToLower() == objUser.user_email.Trim().ToLower())
                            {
                                err_message += " # Email Id already exist!";
                            }

                        }
                        //check for user name
                        if (!string.IsNullOrEmpty(objUser.user_name))
                        {
                            string reg = @"^[^<>,?;:'()!~%\-@#/*""\s]+$";
                            CheckMatch(reg, objUser.user_name, "user_name", out reg_err_message);
                            err_message += reg_err_message;
                        }
                        if (!string.IsNullOrEmpty(objUser.user_email))
                        {
                            string reg = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
                            CheckMatch(reg, objUser.user_email, "user_email", out reg_err_message);
                            err_message += reg_err_message;

                        }

                    }
                    else
                    {
                        err_message += " # Either User name or User email is empty";
                    }

                    if (string.IsNullOrEmpty(objUser.name))
                    {
                        err_message += " # name is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.password))
                    {
                        err_message += " # Password is empty";
                    }
                    if (!string.IsNullOrEmpty(objUser.password))
                    {
                        string reg = @"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$";
                        bool ismatch = CheckMatch(reg, objUser.password, "password", out reg_err_message);
                        err_message += reg_err_message;
                        if (ismatch)
                        {
                            objUser.user_password = Convert.ToString(Utility.MiscHelper.EncodeTo64(objUser.password));
                        }
                    }
                    if (string.IsNullOrEmpty(Convert.ToString(objUser.mobile_number)))
                    {
                        err_message += " # Mobile number  is empty";
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(objUser.mobile_number)))
                    {
                        string reg = @"^[0-9]{1,10}$";
                        bool ismatch = CheckMatch(reg, objUser.mobile_number.ToString(), "mobile_number", out reg_err_message);
                        err_message += reg_err_message;
                    }

                    if (string.IsNullOrEmpty(objUser.role_name))
                    {
                        err_message += " # Role name is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.reporting_manager))
                    {
                        err_message += " # Reporting Manager is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.application_access))
                    {
                        err_message += " # Application Access is empty";
                    }
                    if (string.IsNullOrEmpty(objUser.user_type))
                    {
                        err_message += " # User Type is empty";
                    }
                    if (!string.IsNullOrEmpty(objUser.application_access))
                    {
                        if (objUser.application_access.ToLower() != "mobile" && objUser.application_access.ToLower() != "web" && objUser.application_access.ToLower() != "both")
                        {
                            err_message += " # Application Access value should be mobile/web/both";
                        }
                    }

                    if (!string.IsNullOrEmpty(objUser.role_name))
                    {
                        objUser.role_id = new BLRoles().GetRoleByName(objUser.role_name).role_id;
                    }
                    if (!string.IsNullOrEmpty(objUser.reporting_manager))
                    {

                        objUser.manager_id = new BLUser().GetUserDetailByUserName(objUser.reporting_manager).user_id;

                        if (objUser.role_id > 0)
                        {
                            objUser.lstRM = GetReportingManagers(objUser.role_id);
                            if (objUser.manager_id > 0)
                            {
                                int reportingManagerExist = objUser.lstRM.Where(x => x.value == Convert.ToString(objUser.manager_id)).Count();
                                if (reportingManagerExist == 0)
                                {
                                    err_message += " # Reporting Manager not exist";
                                }
                            }
                            else
                            {
                                err_message += " # Reporting User not exist";
                            }
                        }
                        else
                        {
                            err_message += " # User role not exist";
                        }

                    }
                    objUser.err_message = err_message;

                    if (string.IsNullOrEmpty(objUser.err_message))
                    {
                        objUser.isvalid = true;
                    }
                    lsttempUploadUser.Add(objUser);
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
            return lsttempUploadUser;

        }

        private bool CheckMatch(string regex, string input, string type, out string message)
        {
            bool isMatch = true;
            message = String.Empty;
            try
            {
                var reg = new Regex(regex);
                if (!reg.IsMatch(input))
                {
                    isMatch = false;
                    if (type == "password")
                    {
                        message = " # password should be : 1 lower and 1 upper case letter. 1 number and 1 special character. Minimum 8 character long!";
                    }
                    if (type == "user_name")
                    {
                        message = " # user_name accept : Only dot and underscore are allowed";
                    }
                    if (type == "mobile_number")
                    {
                        message = "# Mobile number allowed only ten digit ";
                    }
                    if (type == "user_email")
                    {
                        message = "- # Invalid user email!";
                    }

                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
            return isMatch;
        }

        private List<BulkUserUploadModuleMapping> CreateUserUploadMobileModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {

            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "mobile").Select(x => x.module_name).ToList<string>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (lstUploadUserModule.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count - 1; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.module_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }

        private List<BulkUserUploadModuleMapping> CreateUserUploadWebModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "web").Select(x => x.module_name).ToList<string>();
                if (lstUploadUserModule.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count - 1; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.module_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }

        private List<BulkUserUploadModuleMapping> CreateUserUploadAdminModuleMapping(int userUpoadId, DataTable dt, string moduleType, List<BulkUserUploadModuleMapping> lstUploadModuleMappings, List<BulkUserUploadModuleMasterMapping> lstBulkUserUploadModuleMasterMapping)
        {
            try
            {
                List<string> lstUploadUserModule = lstBulkUserUploadModuleMasterMapping.Where(x => x.type.ToLower() == "admin").Select(x => x.module_name).ToList<string>();
                if (lstUploadUserModule.Count > 0)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count - 1; i++)
                        {

                            foreach (string item in lstUploadUserModule)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["User Name"])))
                                {
                                    if (dt.Columns.Contains(item))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i][item])) && Convert.ToString(dt.Rows[i][item]).ToLower() == "yes")
                                        {
                                            BulkUserUploadModuleMapping bulkUserUploadModuleMapping = new BulkUserUploadModuleMapping();
                                            bulkUserUploadModuleMapping.user_name = Convert.ToString(dt.Rows[i]["User Name"]);
                                            bulkUserUploadModuleMapping.module_name = item;
                                            bulkUserUploadModuleMapping.value = Convert.ToString(dt.Rows[i][item]);
                                            bulkUserUploadModuleMapping.module_type = moduleType;
                                            bulkUserUploadModuleMapping.user_upload_id = userUpoadId;
                                            lstUploadModuleMappings.Add(bulkUserUploadModuleMapping);
                                        }
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return lstUploadModuleMappings;
        }

        private BulkUserUploadLimit CheckBulkUploadUserLimit(int userUploadId)
        {
            int WebUserMaxLimit = Convert.ToInt32(ApplicationConfig.AppConfig.WebUserMaxLimit);
            int MobileUserMaxLimit = Convert.ToInt32(ApplicationConfig.AppConfig.MobileUserMaxLimit);
            BulkUserUploadLimit bulkUserUploadLimit = new BulkUserUploadLimit();
            try
            {
                bulkUserUploadLimit = new BLBulkUserUploadSummary().BulkUploadUserCheckLimit(userUploadId, WebUserMaxLimit, MobileUserMaxLimit);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw ex;
            }
            return bulkUserUploadLimit;
        }
        // Changes by Ankit
        public ActionResult DownloadBulkUserUploadLogs(int id, string status)
        {
            Dictionary<string, string> lstReportColumn = new Dictionary<string, string>();
            lstReportColumn.Add("user_name", "User Name");
            lstReportColumn.Add("name", "Full Name");
            lstReportColumn.Add("user_email", "Email ID");
            lstReportColumn.Add("password", "Password");
            lstReportColumn.Add("mobile_number", "Mobile number");
            lstReportColumn.Add("role_name", "User Role");
            lstReportColumn.Add("reporting_manager", "Reporting Manager");
            lstReportColumn.Add("application_access", "Application Access");
            lstReportColumn.Add("is_admin_rights_enabled", "is Admin right Allowed");
            lstReportColumn.Add("is_active", "Is Active");
            lstReportColumn.Add("user_type", "User Type");
            lstReportColumn.Add("pan", "PAN");
            lstReportColumn.Add("prms_id", "PRMS Id");
            lstReportColumn.Add("vendor_id", "Vendor Id");
            lstReportColumn.Add("uploaded_by", "Upload By");
            lstReportColumn.Add("uploaded_date", "Upload Date");
            lstReportColumn.Add("err_message", "Error Message");
            lstReportColumn.Add("status", "Status");
            lstReportColumn.Add("invalid_reporting_manager", "Invalid Reporting Manager");

            try
            {
                string[] BulkUserUpoladLogColName = lstReportColumn.Select(i => i.Key.ToString()).ToArray();
                DataTable dt1 = new BLBulkUserUpload().GetBulkUserUploadLog(id, status);
                if (dt1.Rows.Count > 0)
                {
                    DataView view = new DataView(dt1);
                    DataTable dt = view.ToTable(false, BulkUserUpoladLogColName);
                    foreach (var item in lstReportColumn)
                    {
                        dt.Columns[item.Key].ColumnName = item.Value;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        var filename = "Bulk_User_UploadLogs_" + status + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                        string filepath = System.Web.HttpContext.Current.Server.MapPath(Settings.ApplicationSettings.DownloadTempPath) + filename;

                        // string filepath = System.Web.HttpContext.Current.Server.MapPath("~/uploads/temp/") + filename;
                        string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                    }
                    else
                    {
                        return Json("File not Exists");
                    }
                }
                else
                {
                    return Json("File not Exists");
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                ErrorLogHelper.WriteErrorLog("DownloadBulkUserUploadLogs", "UserController", ex);
                return null;
            }
        }

        //        public ActionResult DownloadBulkUserUploadLogs(int id, string status)
        //        {

        //            Dictionary<string, string> lstReportColumn = new Dictionary<string, string>();
        //            lstReportColumn.Add("user_name", "User Name");
        //            lstReportColumn.Add("name", "Full Name");
        //            lstReportColumn.Add("user_email", "Email ID");
        //            lstReportColumn.Add("password", "Password");
        //            lstReportColumn.Add("mobile_number", "Mobile number");
        //            lstReportColumn.Add("role_name", "User Role");
        //            lstReportColumn.Add("reporting_manager", "Reporting Manager");
        //            lstReportColumn.Add("application_access", "Application Access");
        //            lstReportColumn.Add("is_admin_rights_enabled", "is Admin right Allowed");
        //            lstReportColumn.Add("is_active", "Is Active");
        //            lstReportColumn.Add("user_type", "User Type");
        //            lstReportColumn.Add("pan", "PAN");
        //            lstReportColumn.Add("prms_id", "PRMS Id");
        //            lstReportColumn.Add("vendor_id", "Vendor Id");
        //            lstReportColumn.Add("uploaded_by", "Upload By");
        //            lstReportColumn.Add("uploaded_date", "Upload Date");
        //            lstReportColumn.Add("err_message", "Error Message");
        //            lstReportColumn.Add("status", "Status");

        //            try
        //            {
        //                string[] BulkUserUpoladLogColName = lstReportColumn.Select(i => i.Key.ToString()).ToArray();
        //                DataTable dt1 = new BLBulkUserUpload().GetBulkUserUploadLog(id, status);
        //                if (dt1.Rows.Count > 0)
        //                {
        //                    DataView view = new DataView(dt1);
        //                    DataTable dt = view.ToTable(false, BulkUserUpoladLogColName);
        //                    foreach (var item in lstReportColumn)
        //                    {
        //                        dt.Columns[item.Key].ColumnName = item.Value;
        //                    }
        //                    if (dt.Rows.Count > 0)
        //                    {
        //                        var filename = "Bulk_User_UploadLogs_" + status + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
        //                        string filepath = System.Web.HttpContext.Current.Server.MapPath("~/uploads/temp/") + filename;
        //                        string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
        //                        byte[] fileBytes = System.IO.File.ReadAllBytes(file);
        //                        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        //                    }
        //                    else
        //                    {
        //                        return Json("File not Exists");
        //                    }
        //                }
        //                else
        //                {
        //                    return Json("File not Exists");
        //                }
        //            }
        //#pragma warning disable CS0168 // The variable 'ex' is declared but never used
        //            catch (Exception ex)
        //#pragma warning restore CS0168 // The variable 'ex' is declared but never used
        //            {
        //                ErrorLogHelper.WriteErrorLog("DownloadBulkUserUploadLogs", "UserController", ex);
        //                return null;
        //            }
        //        }

        #endregion

        public JsonResult GetSubdistrictByProvinceId(string stateid)
        {
            List<SubDistrict> lstSubDistrict = new BLLayer().GetSubdistrictByProvinceId(stateid);
            return Json(lstSubDistrict, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBlockBySubDistrictId(string subdistrictid)
        {
            List<Block> lstBlock = new BLLayer().GetBlockBySubDistrictId(subdistrictid);
            return Json(lstBlock, JsonRequestBehavior.AllowGet);
        }

    }
}