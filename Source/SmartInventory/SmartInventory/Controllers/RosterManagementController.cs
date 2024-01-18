using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using BusinessLogics;
using System.Globalization;
using Utility;
using SmartInventory.Settings;
using SmartInventory.Filters;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class RosterManagementController : Controller
    {
        // GET: RosterManagement
        public ActionResult Index()
        {
            return View();
        }



        //[HttpGet]
        public ActionResult ViewUserTimeSheet(int user_timesheet_id = 0)
        {
            UserTimeSheetVM objUTVVM = new UserTimeSheetVM();
            UserTimeSheetInput objIn = new UserTimeSheetInput();
            // List<User_Master> RoleMasters = null;
            //int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //lstGroupDetail = BLUser.GetSubordinateDetails(managerId, "").ToList();
            // objUTVVM.lstGroupDetails = lstGroupDetail;
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            BLUser objBLuser = new BLUser();
            BLRoles objBLroles = new BLRoles();
            User objUserDetails = objBLuser.getUserDetails(managerId);
            RoleMaster objRoleMaster = objBLroles.getUserRoleNameByRoleId(objUserDetails.role_id);
            objUTVVM.objRoleMaster = objRoleMaster;


            if (user_timesheet_id != 0)
            {

                objUTVVM.userTimeSheetInput = BLUserTimeSheet.GetUserTimeSheetDetail(user_timesheet_id);
                objUTVVM.userTimeSheetInput.workingmode = "EditMode";
            }
            else
            {
                objIn.workingmode = "CreationMode";
                objUTVVM.userTimeSheetInput = objIn;
            }



            //int managerId = Convert.ToInt32(Session["user_id"].ToString());
            BindUserTimeSheet(ref objUTVVM);
            //ViewBag.User = new SelectList(BLUserTimeSheet.GetFeList(managerId), "user_id", "name");


            return PartialView("_ViewUserTimeSheet", objUTVVM);



        }
        public void BindUserTimeSheet(ref UserTimeSheetVM objUserTimeSheet)
        {
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            List<User_Master> users = new List<User_Master>();
            List<User_Master> userslist = new List<User_Master>();
            users = BLUser.GetMMSubordinateDetails(managerId, "").ToList();
            foreach (var new_user in users)
            {
                User_Master user_ = new User_Master();
                user_.user_id = new_user.user_id;
                user_.name = MiscHelper.Decrypt(new_user.name);
                user_.user_name = new_user.user_name;
                userslist.Add(user_);
            }
            objUserTimeSheet.lstUserDetail = userslist;

            //if (objUserTimeSheet.userTimeSheetInput.confirmmsg != null)
            //{
            //    var confMess = objUserTimeSheet.userTimeSheetInput.confirmmsg.Split(':');
            //    if (confMess.Count() != 0)
            //    {
            //        if (confMess[0] == "ValidationError")
            //        {
            //            return;
            //        }
            //    }
            //}

            if (objUserTimeSheet.userTimeSheetInput.workingmode != "EditMode")
            {

                //objUserTimeSheet.userTimeSheetInput.user_id = 0;
                //objUserTimeSheet.userTimeSheetInput.is_active = true;
                //objUserTimeSheet.userTimeSheetInput.start_date = null;
                //objUserTimeSheet.userTimeSheetInput.end_date = null;
                //objUserTimeSheet.userTimeSheetInput.start_time = "";
                //objUserTimeSheet.userTimeSheetInput.end_time = "";
                //objUserTimeSheet.userTimeSheetInput.working_days = "";
                //objUserTimeSheet.userTimeSheetInput.user_timesheet_id = 0;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult SaveUserTimeSheet([System.Web.Http.FromBody] UserTimeSheetVM objUser_TimeSheet)
        {
            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            string strError = ""; string validStatus = "ADD";

            UserTimeSheetVM objUTVVM = new UserTimeSheetVM();
            UserTimeSheetInput objIn = new UserTimeSheetInput();
            // List<User_Master> RoleMasters = null;
            //int managerId = Convert.ToInt32(Session["user_id"].ToString());
            //lstGroupDetail = BLUser.GetSubordinateDetails(managerId, "").ToList();
            // objUTVVM.lstGroupDetails = lstGroupDetail;
            BLUser objBLuser = new BLUser();
            BLRoles objBLroles = new BLRoles();
            User objUserDetails = objBLuser.getUserDetails(managerId);
            RoleMaster objRoleMaster = objBLroles.getUserRoleNameByRoleId(objUserDetails.role_id);
            objUTVVM.objRoleMaster = objRoleMaster;


            if (objUser_TimeSheet.userTimeSheetInput.user_timesheet_id != 0 && objUser_TimeSheet.userTimeSheetInput.workingmode == "EditMode")
            {
                validStatus = "EDIT";

            }






            //if (blnDate == true)
            //{



            User_TimeSheet objUT = new User_TimeSheet();

            if (ModelState.IsValid)
            {

                bool blnDate = BLUserTimeSheet.isValid_Timesheet_Detail(objUser_TimeSheet.userTimeSheetInput.user_id,
               Convert.ToDateTime(objUser_TimeSheet.userTimeSheetInput.start_date), objUser_TimeSheet.userTimeSheetInput.start_time, objUser_TimeSheet.userTimeSheetInput.user_timesheet_id, out strError);

                if (!blnDate)
                {



                    objUT.user_id = objUser_TimeSheet.userTimeSheetInput.user_id;
                    objUT.manager_id = managerId;
                    objUT.is_active = objUser_TimeSheet.userTimeSheetInput.is_active;
                    objUT.start_date = Convert.ToDateTime(objUser_TimeSheet.userTimeSheetInput.start_date);
                    objUT.end_date = Convert.ToDateTime(objUser_TimeSheet.userTimeSheetInput.end_date);
                    // objUT.start_time = DateTime.ParseExact(objUser_TimeSheet.start_time, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

                    //objUT.start_time = DateTime.ParseExact(objUser_TimeSheet.userTimeSheetInput.start_time, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

                    //objUT.end_time = DateTime.ParseExact(objUser_TimeSheet.userTimeSheetInput.end_time, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

                    objUT.start_time = objUser_TimeSheet.userTimeSheetInput.start_time;
                    objUT.end_time = objUser_TimeSheet.userTimeSheetInput.end_time;

                    objUT.created_by = objUser_TimeSheet.userTimeSheetInput.created_by;
                    objUT.created_on = DateTime.Now;
                    objUT.working_days = objUser_TimeSheet.userTimeSheetInput.working_days;
                    objUT.user_timesheet_id = objUser_TimeSheet.userTimeSheetInput.user_timesheet_id;


                    if (validStatus == "ADD")
                    {

                        bool result = BLUserTimeSheet.SaveUserTimeSheet(objUT);
                        if (result == true)
                        { ViewBag.Result = "Saved Successfully"; }
                        else
                        {
                            ViewBag.Result = "Something went wrong";
                        }
                    }


                }
                else
                {
                    ViewBag.DateTimeValidation = "Roster has already creted for this user on same date";
                }



                if (validStatus == "EDIT")
                {
                    int value = BLUserTimeSheet.EditTimeSheet(objUser_TimeSheet);
                    if (value > 0)
                    {
                        ViewBag.Edit = "Edited Successfully";
                    }
                    else
                    {
                        ViewBag.Edit = "Some thing went wrong";
                    }

                }
            }
            else
            {

                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);


            }

            //}


            //UserTimeSheetVM objUTVVM = new UserTimeSheetVM();
            BindUserTimeSheet(ref objUTVVM);


            return PartialView("_ViewUserTimeSheet", objUTVVM);

        }

        public ActionResult ViewUserTimeSheetDetails(ViewUser_TimeSheet objViewUser_TimeSheet, int page = 0, string sort = "", string sortdir = "")
        {

            int managerId = Convert.ToInt32(Session["user_id"].ToString());
            int totalRecords = 0;
            //  objViewUser_TimeSheet.viewUserTimeSheetFilter.userId = Convert.ToInt32(Session["user_id"].ToString());

            objViewUser_TimeSheet.viewUserTimeSheetFilter.userId = objViewUser_TimeSheet.viewUserTimeSheetFilter.userId;


            objViewUser_TimeSheet.viewUserTimeSheetFilter.managerId = managerId;


            objViewUser_TimeSheet.viewUserTimeSheetFilter.pageSize = 10;
            objViewUser_TimeSheet.viewUserTimeSheetFilter.fromDate = MiscHelper.FormatDate(objViewUser_TimeSheet.viewUserTimeSheetFilter.fromDate);
            objViewUser_TimeSheet.viewUserTimeSheetFilter.toDate = MiscHelper.FormatDate(objViewUser_TimeSheet.viewUserTimeSheetFilter.toDate);
            //objViewRoute.viewRouteFilter.fromDate = MiscHelper.FormatDate(DateTime.Now.AddDays(-31).ToString());
            //objViewRoute.viewRouteFilter.toDate = MiscHelper.FormatDate(DateTime.Now.ToString());
            objViewUser_TimeSheet.viewUserTimeSheetFilter.page = page;
            objViewUser_TimeSheet.viewUserTimeSheetFilter.sort = sort;
            objViewUser_TimeSheet.viewUserTimeSheetFilter.sortdir = sortdir;
            BLUserTimeSheet.GetUsetTimeSheetDetails(objViewUser_TimeSheet, out totalRecords);

            objViewUser_TimeSheet.viewUserTimeSheetFilter.totalRecord = totalRecords;
            return PartialView("_ViewUserTimeSheetDetails", objViewUser_TimeSheet);
        }
        public JsonResult DeleteUserTimeSheet(int user_timesheet_id)
        {
            User_TimeSheet objTS = new User_TimeSheet();
            objTS.user_timesheet_id = user_timesheet_id;

            string errorMessage = string.Empty;

            //var isValidateRoute = BLRoute.ValidateRoute(routeId, routeRefId, out errorMessage);// BLRoute.ValidateRouteAssign(routeAssignId, 0, assignType, out errorMessage);

            try
            {

                var value = BLUserTimeSheet.DeleteTimeSheet(user_timesheet_id);
                if (value)
                {
                    return Json(new { status = true, message = "Roster Plan for this user Deleted Successfully." }, JsonRequestBehavior.AllowGet);

                }

                else
                {
                    return Json(new { status = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
                }

                //}
                //catch (Exception ex)
                //{
                //    return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                //}


            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult EditUserTimeSheetDetails(int user_timesheet_id = 0)
        {

            int managerId = Convert.ToInt32(Session["user_id"].ToString());




            //ViewBag.User = new SelectList(BLUserTimeSheet.GetFeList(managerId), "user_id", "name");
            //UserTimeSheetVM objUTVVM = new UserTimeSheetVM();
            UserTimeSheetVM objUTVVM = new UserTimeSheetVM();
            if (user_timesheet_id != 0)
            {

                objUTVVM.userTimeSheetInput = BLUserTimeSheet.GetUserTimeSheetDetail(user_timesheet_id);

            }
            //else
            //{
            //    objIn.workingmode = "CreationMode";
            //    objUTVVM.userTimeSheetInput = objIn;
            //}

            BindUserTimeSheet(ref objUTVVM);
            return PartialView("_EditUserTimeSheet", objUTVVM);


        }

        [HttpPost]
        public ActionResult EditUserTimeSheet(UserTimeSheetVM objUser_TimeSheet)
        {
            int managerId = Convert.ToInt32(Session["user_id"].ToString());


            int value = BLUserTimeSheet.EditTimeSheet(objUser_TimeSheet);
            if (value > 0)
            {
                ViewBag.Edit = "Edited Successfully";
            }
            else
            {
                ViewBag.Edit = "Some thing went wrong";
            }

            UserTimeSheetVM objUTVVMS = new UserTimeSheetVM();
            BindUserTimeSheet(ref objUTVVMS);

            return PartialView("_ViewUserTimeSheet", objUTVVMS);
        }


        //public JsonResult ValidateUserTimeSheet(int user_id, string roster_date, string roster_time, int user_timesheet_id)
        //{


        //    CultureInfo objCulture = CultureInfo.CreateSpecificCulture("en-GB");
        //    DateTime dtRosterDate;

        //    if (!DateTime.TryParse(roster_date, objCulture, DateTimeStyles.None, out dtRosterDate))
        //    {

        //    }

        //    //roster_time = "9:30";

        //    var tmTime = DateTime.ParseExact(roster_time, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
        //    string strTime = "";
        //    string strError = "";
        //    strTime = tmTime.Hours.ToString("0#") + ":" + tmTime.Minutes.ToString("0#");
        //    bool blnDate = BLUserTimeSheet.isValid_Timesheet_Detail(user_id, dtRosterDate, strTime, user_timesheet_id, out strError);



        //    return Json(new { status = blnDate, message = strError }, JsonRequestBehavior.AllowGet);


        //}


    }
}

