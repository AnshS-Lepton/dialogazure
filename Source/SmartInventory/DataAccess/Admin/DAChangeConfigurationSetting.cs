using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DAChangeConfigurationSetting : Repository<ChangeConfigurationSetting>
    {
        ////// for saving data in database //////
        public bool SavConfigSetting(ChangeConfigurationSetting obj, int userId)
        {
            try
            {
                obj.created_by = userId;
                obj.created_on = DateTimeHelper.Now;
                repo.Insert(obj);
                return true;
            }
            catch { throw; }
        }
    }
}
