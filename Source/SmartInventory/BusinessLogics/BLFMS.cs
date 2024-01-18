using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLFMS
    {
       
        public FMSMaster SaveEntityFMS(FMSMaster objFMSMaster, int userId)
        {
            return new DAFMS().SaveEntityFMS(objFMSMaster, userId);
        }
        public int DeleteFMSById(int systemId)
        {
            return new DAFMS().DeleteFMSById(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoFMS(int systemId)
        {
            return new DAFMS().GetOtherInfoFMS(systemId);
        }
        #endregion
    }
}
