using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using BusinessLogics;
using SmartInventory.Settings;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using Utility;
using SmartInventory.Filters;
namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class SurveyAreaController : Controller
    {
        //
        // GET: /SurveyArea/
        public ActionResult SurveyAssignment(ViewSurveyAssignmentModel objViewSurveyAssignment, int page = 0, string sort = "", string sortdir = "")
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
           
            objViewSurveyAssignment.objFilterAttributes.userid = user_id;
            objViewSurveyAssignment.lstSearchBy = GetSurveySearchByColumns();
            BindSurveyStatusDropDown(objViewSurveyAssignment);
            BindSurveyUser(objViewSurveyAssignment);
            objViewSurveyAssignment.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objViewSurveyAssignment.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewSurveyAssignment.objFilterAttributes.sort = sort;
            objViewSurveyAssignment.objFilterAttributes.orderBy = sortdir;
            objViewSurveyAssignment.objFilterAttributes.role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);


            if (objViewSurveyAssignment.objFilterAttributes.dd_surveyStatus == null)
            {
                if (role_id != 3)
                {
                    objViewSurveyAssignment.objFilterAttributes.dd_user = 0;
                    objViewSurveyAssignment.objFilterAttributes.dd_surveyStatus = "New";
                }
                else
                {
                    // Sales user
                    objViewSurveyAssignment.objFilterAttributes.dd_user = user_id;
                    objViewSurveyAssignment.objFilterAttributes.dd_surveyStatus = "Assigned";
                }
            }
            
            objViewSurveyAssignment.lstSurveyAssignment = new BLSurveyAssignment().GetSurveyAssignmetDetail(objViewSurveyAssignment.objFilterAttributes);
            objViewSurveyAssignment.objFilterAttributes.totalRecord = objViewSurveyAssignment.lstSurveyAssignment != null && objViewSurveyAssignment.lstSurveyAssignment.Count > 0 ? objViewSurveyAssignment.lstSurveyAssignment[0].totalRecords : 0;
            Session["viewSurveyAreaAssignmentFilter"] = objViewSurveyAssignment.objFilterAttributes;
            return PartialView("_SurveyAreaAssignment", objViewSurveyAssignment);
        }

        private void BindSurveyStatusDropDown(ViewSurveyAssignmentModel obj)
        {
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var objDDL = new BLMisc().GetDropDownList(EntityType.SurveyArea.ToString());
            DropDownMaster drp = new DropDownMaster();
         
           // objDDL.Add(new DropDownMaster { dropdown_key = "All", dropdown_status = false, dropdown_type = DropDownType.Survey_Status.ToString(),dropdown_value="0" });
            objDDL.Insert(0, new DropDownMaster { dropdown_key = "All", dropdown_status = false, dropdown_type = DropDownType.Survey_Status.ToString(), dropdown_value = "0" });

            if (role_id == 3)
            {
                objDDL.RemoveAt(1);
            }


            obj.SurveyStatus = objDDL.Where(x => x.dropdown_type == DropDownType.Survey_Status.ToString()).ToList();
        }
        public void BindSurveyUser(ViewSurveyAssignmentModel objview)
        {
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var obj = new BLSurveyAssignment().GetSurveyUser(objview.objFilterAttributes.userid,  role_id);
            if (role_id != 3)
            {
                obj.Insert(0, new SurveyUser { user_id = "0", user_name = "All" });
              
            }
            objview.lstSurveyUser = obj.Select(i => new SurveyUser() { user_id = i.user_id, user_name = i.user_name.ToString(),manager_id=i.manager_id,manager_name=i.manager_name }).ToList();

        }
        public List<KeyValueDropDown> GetSurveySearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Name", value = "surveyarea_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "SurveyArea Code", value = "network_id" });
       
            return lstSearchBy;
        }
        public List<KeyValueDropDown> GetBuildingSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Building Name", value = "building_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Building Code", value = "network_id" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Address", value = "address" });

            return lstSearchBy;
        }
        public ActionResult GetUserforAssignment(int systemid)
        {
            JsonResponse<SurveyAreaUser> objResp = new JsonResponse<SurveyAreaUser>();
            SurveyAreaUser lst = new SurveyAreaUser();
            int user_id = Convert.ToInt32(Session["user_id"]);
            try
            {
                var obj = new BLSurveyAssignment().GetMultiUserAssignValue(user_id, systemid);
                  lst.surveyuser=obj;
                objResp.result = lst;
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_029;// "Error while fetching users!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssignedUser(int systemid)
        {
            JsonResponse<SurveyAssignedUser> objResp = new JsonResponse<SurveyAssignedUser>();
            SurveyAssignedUser lstdate = new SurveyAssignedUser();
            try
            {
                var obj = new BLSurveyAssignment().GetSurveyAssignedUser(systemid);
                lstdate.assignDate = obj;
                objResp.result = lstdate;
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_030;// "Error while fetching assigned user!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FreezeSurveyarea(int systemId, string status)
        {
            JsonResponse<object> objResp = new JsonResponse<object>();
            int user_id = Convert.ToInt32(Session["user_id"]);
            try
            {
                var chkStatus = new BLSurveyAssignment().updateSurveyareaStatus(systemId, status, user_id);
                if (chkStatus)
                    objResp.status = ResponseStatus.OK.ToString();
                else
                    objResp.status = ResponseStatus.ERROR.ToString();
            }
            catch (Exception ex)
            {

                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_031;// "Error while freezing survey area!";
            }
          

           
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        //  Survey Building
        public ActionResult SurveyBuilding(ViewSurveyBuildingModel objViewSurveyBuilding, int page = 0, string sort = "", string sortdir = "")
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            objViewSurveyBuilding.objbuildingFilterAttributes.userid = user_id;
            objViewSurveyBuilding.lstSearchBy = GetBuildingSearchByColumns();
            BindBuildingStatusDropDown(objViewSurveyBuilding);
            BindBuildingUser(objViewSurveyBuilding);
            objViewSurveyBuilding.objbuildingFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objViewSurveyBuilding.objbuildingFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewSurveyBuilding.objbuildingFilterAttributes.sort = sort;
            objViewSurveyBuilding.objbuildingFilterAttributes.orderBy = sortdir;
            objViewSurveyBuilding.objbuildingFilterAttributes.roleid = role_id;

            if (objViewSurveyBuilding.objbuildingFilterAttributes.dd_BuildingStatus == null)
            {
                if (role_id != 3)
                {
                    objViewSurveyBuilding.objbuildingFilterAttributes.dd_user = 0;
                    objViewSurveyBuilding.objbuildingFilterAttributes.dd_BuildingStatus = "Surveyed";
                }
                else
                {
                    // Sales user
                    objViewSurveyBuilding.objbuildingFilterAttributes.dd_user = user_id;
                    objViewSurveyBuilding.objbuildingFilterAttributes.dd_BuildingStatus = "Surveyed";
                }
            }
            objViewSurveyBuilding.lstSurveyBuilding = new BLSurveyAssignment().GetSurveyBuildingDetail(objViewSurveyBuilding.objbuildingFilterAttributes);
            objViewSurveyBuilding.objbuildingFilterAttributes.totalRecord = objViewSurveyBuilding.lstSurveyBuilding != null && objViewSurveyBuilding.lstSurveyBuilding.Count > 0 ? objViewSurveyBuilding.lstSurveyBuilding[0].totalRecords: 0;
            Session["viewSurveyBuildingFilter"] = objViewSurveyBuilding.objbuildingFilterAttributes;
            return PartialView("_SurveyBuilding", objViewSurveyBuilding);
        }
        private void BindBuildingStatusDropDown(ViewSurveyBuildingModel obj)
        {
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var objDDL = new BLMisc().GetDropDownList(EntityType.Building.ToString());
            DropDownMaster drp = new DropDownMaster();

            objDDL.Insert(0, new DropDownMaster { dropdown_key = "All", dropdown_status = false, dropdown_type = DropDownType.Building_Status.ToString(), dropdown_value = "0" });
            //objDDL.RemoveAt(17);
            obj.BuildingStatus = objDDL.Where(x => x.dropdown_type == DropDownType.Building_Status.ToString()).ToList();
        }
        public void BindBuildingUser(ViewSurveyBuildingModel objview)
        {

            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var obj = new BLSurveyAssignment().GetSurveyUser(objview.objbuildingFilterAttributes.userid, role_id);
            if (role_id != 3)
            {
                obj.Insert(0, new SurveyUser { user_id = "0", user_name = "All" });

            }
         
            objview.lstSurveyUser = obj.Select(i => new SurveyUser() { user_id = i.user_id, user_name = i.user_name.ToString(),manager_id=i.manager_id,manager_name=i.manager_name }).ToList();
            
        }
        // Bulk Building
        public ActionResult SurveyUpdateBuilding(ViewBulkSurveyBuildingModel objViewBulkBuilding, int page = 0, string sort = "", string sortdir = "")
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
           // objViewBulkBuilding.objFilterAttributes.userid = user_id;
            objViewBulkBuilding.lstSearchBy = GetBulkSearchByColumns();
            bindCity(objViewBulkBuilding);
        
           // BindBuildingUser(objViewSurveyBuilding);
            objViewBulkBuilding.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objViewBulkBuilding.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewBulkBuilding.objFilterAttributes.sort = sort;
            objViewBulkBuilding.objFilterAttributes.orderBy = sortdir;

            objViewBulkBuilding.lstBulkSurveyBuilding = new BLSurveyAssignment().GetBulkUpdateBuildingDetail(objViewBulkBuilding.objFilterAttributes);
            objViewBulkBuilding.objFilterAttributes.totalRecord = objViewBulkBuilding.lstBulkSurveyBuilding != null && objViewBulkBuilding.lstBulkSurveyBuilding.Count > 0 ? objViewBulkBuilding.lstBulkSurveyBuilding[0].totalRecords : 0;

            return PartialView("_updateBuilding", objViewBulkBuilding);
        }
        public void bindCity(ViewBulkSurveyBuildingModel objView)
        {
            var obj = new BLSurveyAssignment().GetBuildingCity(0);
            obj.Add(new CityList { province_id = 0, province_name = "Select City" });
            objView.lstCity = obj.Select(i => new CityList() { province_id = i.province_id, province_name = i.province_name.ToString() }).ToList();

        }
        public void bindAreabyCity(ViewBulkSurveyBuildingModel objView)
        {
           

        }
        public ActionResult GetAreaList(int province_id)
        {
            JsonResponse<UpdateBuildingArea> objResp = new JsonResponse<UpdateBuildingArea>();
            UpdateBuildingArea bldArea = new UpdateBuildingArea();
            try
            {
                var obj = new BLSurveyAssignment().GetBuildingArea(province_id);
                bldArea.lstArearea = obj;
                objResp.result = bldArea;
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_032;// "Error while fetching area list!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public List<KeyValueDropDown> GetBulkSearchByColumns()
        {
            List<KeyValueDropDown> lstSearchBy = new List<KeyValueDropDown>();
            lstSearchBy.Add(new KeyValueDropDown { key = "Building Name", value = "building_name" });
            lstSearchBy.Add(new KeyValueDropDown { key = "Building Code", value = "network_id" });

            return lstSearchBy;
        }
        // Location tracking
        public ActionResult GetSurveyLocationTrackingData(ViewlocationTrackingModel objViewTrackingDetail, int page = 0, string sort = "", string sortdir = "")
        {
            int user_id = Convert.ToInt32(Session["user_id"]);
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            // objViewBulkBuilding.objFilterAttributes.userid = user_id;
            objViewTrackingDetail.lstSearchBy = GetBulkSearchByColumns();
            BindLocationUser(objViewTrackingDetail);

            // BindBuildingUser(objViewSurveyBuilding);
            objViewTrackingDetail.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objViewTrackingDetail.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewTrackingDetail.objFilterAttributes.sort = sort;
            objViewTrackingDetail.objFilterAttributes.orderBy = sortdir;
            objViewTrackingDetail.objFilterAttributes.roleid = role_id;
            objViewTrackingDetail.objFilterAttributes.userid = user_id;
            if (objViewTrackingDetail.objFilterAttributes.dd_user == 0)
            {
                if (role_id != 3)
                {
                    objViewTrackingDetail.objFilterAttributes.dd_user = 0;                   
                }
                else
                {
                    // Sales user
                     objViewTrackingDetail.objFilterAttributes.dd_user  = user_id;                    
                }
            }


          
            objViewTrackingDetail.lstLocationTracking = new BLSurveyAssignment().GetSurveyLocationDetail(objViewTrackingDetail.objFilterAttributes);
            objViewTrackingDetail.objFilterAttributes.totalRecord = objViewTrackingDetail.lstLocationTracking != null && objViewTrackingDetail.lstLocationTracking.Count > 0 ? objViewTrackingDetail.lstLocationTracking[0].totalRecords : 0;
            Session["viewSurveyLocationExportFilter"] = objViewTrackingDetail.objFilterAttributes;
            return PartialView("_SurveyTracking", objViewTrackingDetail);
        }
        public void BindLocationUser(ViewlocationTrackingModel objview)
        {
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            int user_id = Convert.ToInt32(Session["user_id"]);
            var obj = new BLSurveyAssignment().GetSurveyTrackingUser(user_id, role_id);
            if (role_id != 3)
            {
               obj.Insert(0, new SurveyUser { user_id = "0", user_name = "All" });
            }
            objview.lstSurveyUser = obj.Select(i => new SurveyUser() { user_id = i.user_id, user_name = i.user_name.ToString(),manager_id=i.manager_id,manager_name=i.manager_name }).ToList();
            objview.lstUserReport = new BLUser().GetUserLocationTrackingList(user_id.ToString(), role_id).ToList();
        }
        /// <summary>
        ///Track Building
        /// </summary>
        /// <param name="objViewTrackingDetail"></param>
        /// <param name="page"></param>
        /// <param name="sort"></param>
        /// <param name="sortdir"></param>
        /// <returns></returns>
        public ActionResult GetTrackBuilding(ViewBuildingTrackingModel objViewTrackingDetail, int page = 0, string sort = "", string sortdir = "", int user_id = 0, int login_id = 0, int survey_system_id = 0)
        {
            int loguserid = Convert.ToInt32(Session["user_id"]);
            int role_id = Convert.ToInt32(((User)Session["userDetail"]).role_id);
           // objViewTrackingDetail.lstSearchBy = GetBulkSearchByColumns();
            
            objViewTrackingDetail.objFilterAttributes.entity_type = "Building";

            objViewTrackingDetail.objFilterAttributes.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objViewTrackingDetail.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objViewTrackingDetail.objFilterAttributes.sort = sort;
            objViewTrackingDetail.objFilterAttributes.orderBy = sortdir;
            objViewTrackingDetail.objFilterAttributes.loguserId = loguserid;
            objViewTrackingDetail.objFilterAttributes.role_id = role_id;   
           
         
            objViewTrackingDetail.lstBuildingTracking = new BLSurveyAssignment().GetTrackBuildingDetail(objViewTrackingDetail.objFilterAttributes);   
          
            objViewTrackingDetail.objFilterAttributes.totalRecord = objViewTrackingDetail.lstBuildingTracking != null && objViewTrackingDetail.lstBuildingTracking.Count > 0 ? objViewTrackingDetail.lstBuildingTracking[0].totalRecords : 0;

            return PartialView("_SurveyTrackBuilding", objViewTrackingDetail);
        }
        public ActionResult GetUserLocationTracking(int loginId)
        {
            JsonResponse<List<UserLocationTracking>> objResp = new JsonResponse<List<UserLocationTracking>>();          

            try
            {
                objResp.result = new BLSurveyAssignment().GetLocationTracking(loginId);
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("GetUserLocationTracking()", "SurveyArea", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_033;// "Error while fetching route!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLocationTracking(int login_id)
        {
            JsonResponse<UserLocationTracking> objResp = new JsonResponse<UserLocationTracking>();           
            try
            {
                objResp.result = new BLUserLocationTracking().GetUserCurrentLocation(login_id);
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("GetLocationTracking()", "SurveyArea", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_GBL_GBL_NET_FRM_034; //"Error while fetching current location!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BulkApproved(string systemId,string status, string comment)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            int user_id = Convert.ToInt32(Session["user_id"]);
            int roleid = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            bool obj = new BLSurveyAssignment().UpdateSurveyBuildingStatus(systemId, roleid, status, user_id, comment);
            if (obj)
            {
                objResp.status = ResponseStatus.OK.ToString();
            }
            else
            {
                objResp.status = ResponseStatus.ERROR.ToString();
            }

            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult CheckExportSurveyAssignment()
        {
            int userid = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            int roleid = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var lstexist = new BLSurveyAssignment().chkSurveyassignmentExportDataExist(userid, roleid);

            return Json(lstexist == true ? true : false, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public void ExportSurveyAssignment()
        {
            int userid = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            int roleid = Convert.ToInt32(((User)Session["userDetail"]).role_id);
            var exportData = new BLSurveyAssignment().GetSurveyassignmentExportData<Dictionary<string, string>>(userid,roleid);
            DataTable dtlogs = Utility.MiscHelper.GetDataTableFromDictionaries(exportData);
            var filename = "SurveyAssignment";
            ExportAssignmentData(dtlogs, "Export_" + filename);

        }
        [HttpGet]
        public void DownloadSurveyAssignmentReport()
        {
            if (Session["viewSurveyAreaAssignmentFilter"] != null)
            {
                CommonFilterAttribute objViewFilter = (CommonFilterAttribute)Session["viewSurveyAreaAssignmentFilter"];
                List<SurveyAssignment> lstSurveyAssignment = new List<SurveyAssignment>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstSurveyAssignment = new BLSurveyAssignment().GetSurveyAssignmetDetail(objViewFilter);

                DataTable dtReport = new DataTable();
                // SurveyAssignment
                dtReport = MiscHelper.ListToDataTable<SurveyAssignment>(lstSurveyAssignment);
                dtReport.Columns.Add("CREATED_DATE", typeof(System.String));
                dtReport.Columns.Add("SURVEY_DUE_DATE", typeof(System.String));
                foreach (DataRow dr in dtReport.Rows)
                {
                    dr["CREATED_DATE"] = MiscHelper.FormatDateTime((dr["created_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                    dr["SURVEY_DUE_DATE"] = MiscHelper.FormatDate((dr["due_date"].ToString()));
                   
                }
                dtReport.Columns.Remove("system_id");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("totalrecords");//
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("due_date");
                dtReport.Columns.Remove("origin_from");
                dtReport.Columns.Remove("origin_ref_id");
                dtReport.Columns.Remove("origin_ref_code");//
                dtReport.Columns.Remove("origin_ref_description");
                dtReport.Columns.Remove("request_ref_id");
                dtReport.Columns.Remove("requested_by");
                dtReport.Columns.Remove("request_approved_by");
                dtReport.Columns.Remove("assigned_by");

                dtReport.Columns["NETWORK_ID"].ColumnName =Resources.Resources.SI_OSP_GBL_NET_FRM_182;
                dtReport.Columns["USER_NAME"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_164;
                dtReport.Columns["surveyarea_name"].ColumnName = Resources.Resources.SI_OSP_SVA_NET_FRM_007;
                dtReport.Columns["surveyarea_status"].ColumnName = Resources.Resources.SI_OSP_SVA_NET_RPT_001;
                //dtReport.Columns["assigned_by"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_011;
                dtReport.Columns["TOTAL_ASSIGNED_USER"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_160;
                dtReport.Columns["TOTAL_BUILDING"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_161;
                dtReport.Columns["COMPLETED"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_162;
                dtReport.Columns["CREATED_DATE"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_055;
                dtReport.Columns["created_by_user"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_056;
                dtReport.Columns["SURVEY_DUE_DATE"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_163;
                dtReport.Columns["REMARKS"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_006;
                 


                var filename = "SurveyAssignment";
                ExportAssignmentData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));

               
            }
        }
        [HttpGet]
        public void DownloadLocationTrackingReport()
        {
            if (Session["viewSurveyLocationExportFilter"] != null)
            {
               
                LocationAttribute objViewFilter = (LocationAttribute)Session["viewSurveyLocationExportFilter"];
                List<locationTracking> lstLocationTracking = new List<locationTracking>();
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstLocationTracking = new BLSurveyAssignment().GetSurveyLocationDetail(objViewFilter);

                DataTable dtReport = new DataTable();
                //location tracking 
                dtReport = MiscHelper.ListToDataTable<locationTracking>(lstLocationTracking);
                dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_RPT_037, typeof(System.String));
                dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_RPT_038, typeof(System.String));
                dtReport.Columns.Add(Resources.Resources.SI_OSP_GBL_NET_RPT_039, typeof(System.String));
                foreach (DataRow dr in dtReport.Rows)
                {
                    dr[Resources.Resources.SI_OSP_GBL_NET_RPT_037] = MiscHelper.FormatDate((dr["tracking_date"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                    dr[Resources.Resources.SI_OSP_GBL_NET_RPT_038] = MiscHelper.FormatDateTime((dr["login_time"].ToString()));
                    dr[Resources.Resources.SI_OSP_GBL_NET_RPT_039] = MiscHelper.FormatDateTime((dr["logout_time"].ToString()));

                }
                dtReport.Columns.Remove("USER_ID");
                dtReport.Columns.Remove("LOGIN_ID");
                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns.Remove("tracking_date");
                dtReport.Columns.Remove("login_time");
                dtReport.Columns.Remove("logout_time");
                dtReport.Columns.Remove("building_count");
                dtReport.Columns["USER_NAME"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_040;
                //dtReport.Columns["login time"].ColumnName = "LOGIN_TIME";
                //dtReport.Columns["logout time"].ColumnName = "LOGOUT_TIME";
                var filename = "LocationTracking";
                ExportAssignmentData(dtReport, "Export_" + filename +"_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));


            }
        }
        [HttpGet]
        public void DownloadSurveyBuildingReport()
        {
            if (Session["viewSurveyBuildingFilter"] != null)
            {

                CommonBuildingFilterAttribute objViewFilter = (CommonBuildingFilterAttribute)Session["viewSurveyBuildingFilter"];
                List<SurveyBuilding> lstSurveyBuilding = new List<SurveyBuilding>();
                List<ExportBuildingComments> lstBuildingComment = new List<ExportBuildingComments>();   
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstSurveyBuilding = new BLSurveyAssignment().GetSurveyBuildingDetail(objViewFilter);
               
                DataTable dtReport = new DataTable(); 
               
                dtReport = MiscHelper.ListToDataTable<SurveyBuilding>(lstSurveyBuilding);
               
                dtReport.Columns.Add("CREATED_DATE", typeof(System.String));
            
                foreach (DataRow dr in dtReport.Rows)
                {
                    dr["CREATED_DATE"] = MiscHelper.FormatDateTime((dr["created_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                  
                }
                dtReport.Columns["USER_NAME"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_049;
                dtReport.Columns["SURVEYAREA_NAME"].ColumnName = Resources.Resources.SI_OSP_SVA_NET_FRM_006;
                dtReport.Columns["SURVEYAREA_CODE"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_007;
                dtReport.Columns["BUILDING_NAME"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_044;
                dtReport.Columns["BUILDING_NO"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_045;
                dtReport.Columns["ADDRESS"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_046;
                dtReport.Columns["BUSINESS_PASS"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_051;
                dtReport.Columns["HOME_PASS"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_083;
                dtReport.Columns["TENANCY"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_084;
                dtReport.Columns["CATEGORY"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_056;
                dtReport.Columns["BUILDING_STATUS"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_047;
                dtReport.Columns["REMARKS"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_006;
                dtReport.Columns["LATITUDE"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_106;
                dtReport.Columns["LONGITUDE"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_012;
                dtReport.Columns["CREATED_DATE"].ColumnName =Resources.Resources.SI_OSP_GBL_NET_RPT_050;
                dtReport.Columns["BUILDING_CODE"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_060;
               // dtReport.Columns["buildingcreatedby"].ColumnName = "Created By";
                dtReport.Columns.Remove("SURVEYAREA_ID");
                dtReport.Columns.Remove("BUILDING_ID");
                dtReport.Columns.Remove("BUILDINGCREATEDBY"); 
                dtReport.Columns.Remove("totalRecords");
                dtReport.Columns.Remove("created_on");              
                var filename = "SurveyBuilding";

                DataSet ds = new DataSet();
                ds.Tables.Add(dtReport);

                //--- Building Comments
                var lstbuildingIds = lstSurveyBuilding.Select(x => x.building_id.ToString()).ToList();
                var strBuildingIds = String.Join(",", lstbuildingIds);
                lstBuildingComment = new BLBuildingComment().getBulkbuildingComments(strBuildingIds);
                DataTable dtReport2 = new DataTable();
                dtReport2 = MiscHelper.ListToDataTable<ExportBuildingComments>(lstBuildingComment);
                dtReport2.Columns.Remove("id");
                dtReport2.Columns.Remove("building_system_id");
                dtReport2.Columns.Add("MODIFIED_ON", typeof(System.String));
                dtReport2.TableName = Resources.Resources.SI_GBL_GBL_GBL_FRM_021;
                dtReport2.Columns["BUILDING_CODE"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_140;
                dtReport2.Columns["BUILDING_STATUS"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_098;
                dtReport2.Columns["Comment"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_312;
                dtReport2.Columns["MODIFIED_BY"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_058;
                
                foreach (DataRow dr in dtReport2.Rows)
                {
                    dr["MODIFIED_ON"] = MiscHelper.FormatDateTime((dr["created_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                    
                }
                dtReport2.Columns["MODIFIED_ON"].ColumnName = Resources.Resources.SF_GBL_GBL_JQ_FRM_007;
                dtReport2.Columns.Remove("created_on");
                ds.Tables.Add(dtReport2);
                ExportSurveyAssignmentData(ds, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));


            }
        }
        public ActionResult GetUserAssignment(int system_id,string network_code,string surveyarea_name,string due_date)
        {
            ModelSurveyAreaUser objSurveyUser = new ModelSurveyAreaUser();
            int userid = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            var obj = new BLSurveyAssignment().GetMultiUserAssignValue(userid, system_id);
            objSurveyUser.surveyuser = obj;
            objSurveyUser.network_code = network_code;
            objSurveyUser.surveyarea_name = surveyarea_name;
           // var _duedate = MiscHelper.FormatDate(due_date);
            objSurveyUser.due_date = MiscHelper.FormatDate(due_date);
            objSurveyUser.MaxUsersAssign = ApplicationSettings.MaxBulkAssignSurveyUser;
         
            return PartialView("_UserAssignment", objSurveyUser);
        }
        public ActionResult SaveAssignedUser(int systemId, string assignedUserId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            int userid = Convert.ToInt32(((User)Session["userDetail"]).user_id);
            string surveyStatus = new BLSurveyAssignment().GetSurveyAreaStatus(systemId);
            var records = new List<SurveyAreaAssigned>();
            string[] accessibleCircles = assignedUserId.Split(',');
            bool chkStatus = false;
            if (accessibleCircles.Length > 0)
            {
                foreach (var circle in accessibleCircles)
                {
                    records.Add(new SurveyAreaAssigned()
                    {
                        assigned_to = Convert.ToInt32(circle),
                        system_id = systemId,
                        assigned_by = userid,
                        assigned_date=DateTimeHelper.Now

                    });
                }
                if (surveyStatus.ToLower() == "new")
                {
                    new BLBulkUserAssignment().BulkInsertUserAssignment(records);

                    chkStatus = new BLSurveyAssignment().updateSurveyareaStatus(systemId, "Assigned", userid);
                }
                else if (surveyStatus.ToLower() == "assigned")
                {
                    try
                    {
                        new BLSurveyAssignment().DeleteSurveyAssignmentUsers(systemId, records.Select(m => m.assigned_to.ToString()).ToList());

                        var exisUsr = new BLSurveyAssignment().GetUserAssignment(systemId);
                        //List<string> existingUser = exisUsr.ConvertAll(x => x.ToString());
                        List<string> existingUser = new List<string>();
                        foreach (var usr in exisUsr)
                        {
                            existingUser.Add(usr.assigned_to.ToString());
                        }

                        records.RemoveAll(m => existingUser.Contains(m.assigned_to.ToString()));
                        if (records.Count > 0)
                        {
                            new BLBulkUserAssignment().BulkInsertUserAssignment(records);
                        }

                        chkStatus = true;
                    }
                    catch (Exception)
                    {
                        chkStatus = false;
                        throw;
                        
                    }
                 
                

                }
            }

            if (chkStatus)
            {
                objResp.status = ResponseStatus.OK.ToString();
            }
            else
            {
                objResp.status = ResponseStatus.ERROR.ToString();
            }         

            return Json(objResp, JsonRequestBehavior.AllowGet);
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
        private void ExportSurveyAssignmentData(DataSet dsReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dsReport != null && dsReport.Tables.Count > 0)
                {
                    IWorkbook workbook = NPOIExcelHelper.DatasetToExcel("xlsx", dsReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }

                Response.End();
            }
        } 

        public ActionResult UploadBuilding()
        {
            return PartialView("_UploadBuilding");
        }

	}
}