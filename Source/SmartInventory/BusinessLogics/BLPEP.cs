using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLPEP
    {
       
        public PEPMaster SaveEntityPEP(PEPMaster objPEPMaster, int userId)
        {
            return new DAPEP().SaveEntityPEP(objPEPMaster, userId);
        }
        public int DeletePEPById(int systemId)
        {
            return new DAPEP().DeletePEPById(systemId);
        }
       
        #region Additional-Attributes
        public string GetOtherInfoPEP(int systemId)
        {
            return new DAPEP().GetOtherInfoPEP(systemId);
        }
        #endregion
    }
}
