using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLMPOD
    {
        public MPODMaster SaveEntityMPOD(MPODMaster objMPODMaster, int userId)
        {
            return new DAMPOD().SaveEntityMPOD(objMPODMaster, userId);
        }
        public int DeleteMPODById(int systemId)
        {
            return new DAMPOD().DeleteMPODById(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoMPOD(int systemId)
        {
            return new DAMPOD().GetOtherInfoMPOD(systemId);
        }
        #endregion
    }
}
