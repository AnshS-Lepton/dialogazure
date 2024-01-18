using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
   public class BLDsa
    {
        public DSA SaveDSA(DSA objDsa, int userId)
        {
            return new DADsa().SaveDSA(objDsa, userId);
        }

        public int DeleteDSAById(int systemId)
        {
            return new DADsa().DeleteDSAById(systemId);
        }

       
    }

    public class BLCsa
    {
        public List<CSAIn> GetDSAExist(string geom)
        {
            return new DACsa().GetDSAExist(geom);
        }


        public CSA SaveCSA(CSA objCsa, int userId)
        {
            return new DACsa().SaveCSA(objCsa, userId);

        }
        public DbMessage CalculateHomePasses(int system_id)
        {
            return new DACsa().CalculateHomePasses(system_id);
        }
        public CSA UpdateRfsStatus(string designId, string rfsStaus)
        {
            return new DACsa().UpdateRfsStatus(designId, rfsStaus);

        }

    }
}
