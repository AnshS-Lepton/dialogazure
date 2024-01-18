using DataAccess.Admin;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLDashboard
    {
        public BLDashboard()
        {

        }
        private static BLDashboard objBLDashboard = null;
        private static readonly object lockObject = new object();
        public static BLDashboard Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLDashboard == null)
                    {
                        objBLDashboard = new BLDashboard();
                    }
                }
                return objBLDashboard;
            }
        }

        public CurrentActiveUsers CurrentActiveUsers()
        {
          return  DACurrentActiveUsers.Instance.CurrentActiveUsers();
        }

        public CurrentActiveUsers getDashboardDetails(int userId)
        {
            return DACurrentActiveUsers.Instance.getDashboardDetails(userId);
        }
        public List<LastMonthActiveUsers> LastMonthActiveUsers()
        {
            return DALastMonthActiveUsers.Instance.LastMonthActiveUsers();
        }
        public List<LastMonthActiveUsers> GetLastMonthLoggedInUsersWise(int userId)
        {
            return DALastMonthActiveUsers.Instance.GetLastMonthLoggedInUsersWise(userId);
        }
        public DataTable getDashboardData()
        {
            return DACurrentActiveUsers.Instance.getDashboardData();
        }

    }
}
