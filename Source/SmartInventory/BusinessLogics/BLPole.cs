using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLPole
    {
       
        public PoleMaster SaveEntityPole(PoleMaster objPoleMaster, int userId)
        {
            return new DAPole().SaveEntityPole(objPoleMaster, userId);
        }
        public int DeletePoleById(int systemId)
        {
            return new DAPole().DeletePoleById(systemId);
        }
        public List<PoleArea> GetPoleArea(string geom)
        {
            return new DAPole().GetPoleArea(geom);
        }
        #region Additional-Attributes
        public string GetOtherInfoPole(int systemId)
        {
            return new DAPole().GetOtherInfoPole(systemId);
        }
        #endregion
    }
}
