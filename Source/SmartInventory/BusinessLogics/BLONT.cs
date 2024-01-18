using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLONT
    {
        public ONTMaster SaveONTEntity(ONTMaster objONTMaster, int userId)
        {
            return new DAONT().SaveONTEntity(objONTMaster, userId);
        }
        public int DeleteONTById(int systemId)
        {
            return new DAONT().DeleteONTById(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoONT(int systemId)
        {
            return new DAONT().GetOtherInfoONT(systemId);
        }
        #endregion
    }
}
