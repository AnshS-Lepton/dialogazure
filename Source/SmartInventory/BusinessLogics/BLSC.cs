using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLSC
    {
       
        public SCMaster SaveEntitySC(SCMaster objSCMaster, int userId)
        {
            return new DASC().SaveEntitySC(objSCMaster, userId);
        }
        public int DeleteSCById(int systemId)
        {
            return new DASC().DeleteSCById(systemId);
        }
        public SCMaster getSCDetails(int systemId)
        {
            return new DASC().getSCDetails(systemId);
        }
        #region Additional-Attributes
        public string GetOtherInfoSpliceClosure(int systemId)
        {
            return new DASC().GetOtherInfoSpliceClosure(systemId);
        }
        #endregion
    }
}
