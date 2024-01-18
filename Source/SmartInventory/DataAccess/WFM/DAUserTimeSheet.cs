using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBContext;
using Models;
using Npgsql;
using System.Data;

namespace DataAccess
{
    public class DAUserTimeSheet
    {
        public static List<User_Master> GetFeListValue(int mangerid)
        {
            List<User_Master> usermaster = new List<User_Master>();
            try
            {
                using (MainContext context = new MainContext())
                {

                    usermaster = context.user_Masters.Where(x => x.manager_id == mangerid).ToList();

                    return usermaster;
                }
            }
            catch { throw; }
        }

        public static bool SaveUserTimeSheet(User_TimeSheet objData)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    context.user_timesheet.Add(objData);
                    return context.SaveChanges() > 0;
                }
            }
            catch { throw; }
        }




        public static void GetUsetTimeSheetDetails(ViewUser_TimeSheet objViewUserTimesheetRpt, out int totalRecords)
        {


            try
            {
                totalRecords = 0;

                List<User_TimeSheet> lstUserTimesheet = null;
                using (MainContext context = new MainContext())
                {



                    var rUserId = new NpgsqlParameter("@P_USER_ID", objViewUserTimesheetRpt.viewUserTimeSheetFilter.userId);
                    var rManagerId = new NpgsqlParameter("@P_MANAGER_ID", objViewUserTimesheetRpt.viewUserTimeSheetFilter.managerId);
                    var rRoleId = new NpgsqlParameter("@P_ROLE_ID", objViewUserTimesheetRpt.viewUserTimeSheetFilter.roleId);
                    var rFDate = new NpgsqlParameter("@P_FROMDATE", string.IsNullOrEmpty(objViewUserTimesheetRpt.viewUserTimeSheetFilter.fromDate) ? "" : objViewUserTimesheetRpt.viewUserTimeSheetFilter.fromDate);
                    var rTDate = new NpgsqlParameter("@P_TODATE", string.IsNullOrEmpty(objViewUserTimesheetRpt.viewUserTimeSheetFilter.toDate) ? "" : objViewUserTimesheetRpt.viewUserTimeSheetFilter.toDate);
                    var rPNo = new NpgsqlParameter("@P_PAGENO", objViewUserTimesheetRpt.viewUserTimeSheetFilter.currentPage);
                    var rPRecord = new NpgsqlParameter("@P_PAGERECORD", objViewUserTimesheetRpt.viewUserTimeSheetFilter.pageSize);
                    var rSortCol = new NpgsqlParameter("@P_SORTCOLNAME", string.IsNullOrEmpty(objViewUserTimesheetRpt.viewUserTimeSheetFilter.sort) ? "" : objViewUserTimesheetRpt.viewUserTimeSheetFilter.sort.ToUpper());
                    var rSortDir = new NpgsqlParameter("@P_SORTTYPE", string.IsNullOrEmpty(objViewUserTimesheetRpt.viewUserTimeSheetFilter.sortdir) ? "" : objViewUserTimesheetRpt.viewUserTimeSheetFilter.sortdir.ToUpper());
                    var rtext = new NpgsqlParameter("@SEARCHTEXT", string.IsNullOrEmpty(objViewUserTimesheetRpt.viewUserTimeSheetFilter.searchText) ? "" : objViewUserTimesheetRpt.viewUserTimeSheetFilter.searchText.ToUpper());

                    var parameters = new NpgsqlParameter[10] { rUserId, rManagerId, rRoleId, rFDate, rTDate, rPNo, rPRecord, rSortCol, rSortDir, rtext };
                    objViewUserTimesheetRpt.lstUserTimeSheetDetails = DbHelper.ExecutePostgresProcedure<User_TimeSheetVW>(context, "fn_get_user_timesheet", parameters, out totalRecords);

                }
            }
            catch
            {
                throw;
            }



        }

        public static bool DeleteTimeSheet(int user_timesheet_id)
        {
            User_TimeSheet objTS = new User_TimeSheet();
            try
            {
                using (MainContext context = new MainContext())
                {
                    objTS = context.user_timesheet.Find(user_timesheet_id);
                    if (objTS != null)
                    {
                        context.Entry(objTS).State = System.Data.Entity.EntityState.Deleted;
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static UserTimeSheetInput GetUserTimeSheetDetail(int user_timesheet_id)
        {
            using (MainContext context = new MainContext())
            {
                try
                {
                    UserTimeSheetInput objUserTimeSheetIn = new UserTimeSheetInput();
                    User_TimeSheet objUserTimeSheet = context.user_timesheet.Where(u => u.user_timesheet_id == user_timesheet_id).FirstOrDefault();
                    objUserTimeSheetIn.user_timesheet_id = objUserTimeSheet.user_timesheet_id;
                    objUserTimeSheetIn.user_id = objUserTimeSheet.user_id;
                    objUserTimeSheetIn.manager_id = objUserTimeSheet.manager_id;
                    objUserTimeSheetIn.working_days = objUserTimeSheet.working_days;
                    objUserTimeSheetIn.start_date = objUserTimeSheet.start_date.Value.ToString("dd-MMM-yyyy");
                    objUserTimeSheetIn.end_date = objUserTimeSheet.end_date.Value.ToString("dd-MMM-yyyy");
                    objUserTimeSheetIn.start_time = objUserTimeSheet.start_time;
                    objUserTimeSheetIn.end_time = objUserTimeSheet.end_time;
                    objUserTimeSheetIn.is_active = objUserTimeSheet.is_active;
                    return objUserTimeSheetIn;
                }
                catch
                {
                    throw;
                }
            }
        }

        public static int EditTimeSheet(UserTimeSheetVM objUser_TimeSheet)
        {

            try
            {
                MainContext db = new MainContext();
                User_TimeSheet objUT = db.user_timesheet.Where(x => x.user_timesheet_id == objUser_TimeSheet.userTimeSheetInput.user_timesheet_id).FirstOrDefault();
                objUT.user_id = objUser_TimeSheet.userTimeSheetInput.user_id;
                objUT.manager_id = objUser_TimeSheet.userTimeSheetInput.manager_id;
                objUT.is_active = objUser_TimeSheet.userTimeSheetInput.is_active;
                objUT.start_date = Convert.ToDateTime(objUser_TimeSheet.userTimeSheetInput.start_date);
                objUT.end_date = Convert.ToDateTime(objUser_TimeSheet.userTimeSheetInput.end_date);
                // objUT.start_time = DateTime.ParseExact(objUser_TimeSheet.start_time, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

                objUT.start_time = objUser_TimeSheet.userTimeSheetInput.start_time;
                objUT.end_time = objUser_TimeSheet.userTimeSheetInput.end_time;

                objUT.modified_by = objUser_TimeSheet.userTimeSheetInput.manager_id;
                objUT.modified_on = DateTime.Now;
                objUT.created_by = objUser_TimeSheet.userTimeSheetInput.created_by;
                objUT.created_on = DateTime.Now;
                objUT.working_days = objUser_TimeSheet.userTimeSheetInput.working_days;

                int value = db.SaveChanges();
                return value;
            }
            catch { throw; }




        }

        public static bool isValid_Timesheet_Detail(int userId, DateTime date_check_in, string strTime, int user_timesheet_id, out string errorMessage) //ValidateLeaveDetail(Leave_Details objLeaveDetail, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                bool status = false;
                using (MainContext context = new MainContext())
                {
                    
                                //DayOfWeek wk = DateTime.Now.DayOfWeek;
                                //strTime = "09:30";
                                string query = string.Format("select * from fn_validate_user_timesheet_date_time({0},'{1}','{2}',{3})", userId, date_check_in.ToString("dd-MMM-yyyy"), strTime, user_timesheet_id);
                                errorMessage = context.Database.SqlQuery<string>(query).FirstOrDefault();
                                status = errorMessage.Equals("success") ? true : false;
                          
                    

                }
                return status;
            }
            catch
            {
                return false;
                throw;
            }

        }

    }





}
