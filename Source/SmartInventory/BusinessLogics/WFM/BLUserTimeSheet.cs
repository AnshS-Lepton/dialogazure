using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;


namespace BusinessLogics
{
    public class BLUserTimeSheet
    {
        public static bool isValid_Timesheet_Detail(int userId, DateTime _date, string strTime, int user_timesheet_id, out string Error)
        {
            return DAUserTimeSheet.isValid_Timesheet_Detail(userId, _date, strTime, user_timesheet_id, out Error);
        }
        public static List<User_Master> GetFeList(int manager_id)
        {
            return DAUserTimeSheet.GetFeListValue(manager_id);
        }
        public static int EditTimeSheet(UserTimeSheetVM ObjUserTimeSheet)
        {
            return DAUserTimeSheet.EditTimeSheet(ObjUserTimeSheet);
        }

        public static bool SaveUserTimeSheet(User_TimeSheet objUserTimeSheet)
        {
            return DAUserTimeSheet.SaveUserTimeSheet(objUserTimeSheet);
        }

        public static void GetUsetTimeSheetDetails(ViewUser_TimeSheet objUserTimeSheetView, out int totalRecords)
        {
            DAUserTimeSheet.GetUsetTimeSheetDetails(objUserTimeSheetView, out totalRecords);
        }



        public static bool DeleteTimeSheet(int user_timesheet_id)
        {
            return DAUserTimeSheet.DeleteTimeSheet(user_timesheet_id);
        }
        public static UserTimeSheetInput GetUserTimeSheetDetail(int user_timesheet_id)
        {
            return DAUserTimeSheet.GetUserTimeSheetDetail(user_timesheet_id);
        }

    }
}
