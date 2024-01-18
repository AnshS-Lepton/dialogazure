using DataAccess.DBHelpers;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DACurrentActiveUsers : Repository<CurrentActiveUsers>
    {
        DACurrentActiveUsers()
        {

        }

        private static DACurrentActiveUsers objCurrentActiveUsers = null;
        private static readonly object lockObject = new object();
        public static DACurrentActiveUsers Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objCurrentActiveUsers == null)
                    {
                        objCurrentActiveUsers = new DACurrentActiveUsers();
                    }
                }
                return objCurrentActiveUsers;
            }
        }
        public CurrentActiveUsers CurrentActiveUsers()
        {
            var users = repo.GetAll().FirstOrDefault();
            return users;
        }
        public CurrentActiveUsers getDashboardDetails(int userId)
        {
            try
            {
                var res = repo.ExecuteProcedure<CurrentActiveUsers>("fn_get_dashboard_details", new
                {
                    p_user_id = userId
                }, true).FirstOrDefault();
                return res;
            }
            catch { throw; }
        }
        public DataTable getDashboardData()
        {
            try
            {
                 string sQuery = string.Format("select  * from fn_report_dashboard_data()");
                 return repo.GetDataTable(sQuery);

            }
            catch { throw; }
        }
    }

  


    public class DALastMonthActiveUsers : Repository<LastMonthActiveUsers>
    {
        DALastMonthActiveUsers()
        {

        }

        private static DALastMonthActiveUsers objLastMonthActiveUsers = null;
        private static readonly object lockObject = new object();
        public static DALastMonthActiveUsers Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objLastMonthActiveUsers == null)
                    {
                        objLastMonthActiveUsers = new DALastMonthActiveUsers();
                    }
                }
                return objLastMonthActiveUsers;
            }
        }
        public List<LastMonthActiveUsers> LastMonthActiveUsers()
        {
            var users = repo.GetAll().ToList();
            return users;
        }

        public List<LastMonthActiveUsers> GetLastMonthLoggedInUsersWise(int userId)
        {
            try
            {
                var res = repo.ExecuteProcedure<LastMonthActiveUsers>("fn_get_user_lastlogin", new
                {
                    p_user_id = userId
                }, true);
                return res;
            }
            catch { throw; }
        }


    }

   
}
