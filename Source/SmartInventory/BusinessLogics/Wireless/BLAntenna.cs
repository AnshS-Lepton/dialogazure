using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLAntenna
    {
        public AntennaMaster SaveAntennaEntity(AntennaMaster objAntennaMaster, int userId)
        {
            return new DAAntenna().SaveAntennaEntity(objAntennaMaster, userId);
        }
        public int DeleteAntennaById(int systemId)
        {
            return new DAAntenna().DeleteAntennaById(systemId);
        }
        public AntennaMaster getAntennaDetails(int systemId)
        {
            return new DAAntenna().getAntennaDetails(systemId);
        }
        public string SaveVsatAntenna(VSATAntenna objVSATAntenna,int user_id)
        {
            return new DAVSATAntenna().SaveVsatAntenna(objVSATAntenna, user_id);
        }
        public VSATAntenna GetVsatAntennaById(int id)
        {
            return new DAVSATAntenna().GetVsatAntennaById(id);
        }

        public int GetBuildingSystemId(int id)
        {
            return new DAVSATAntenna().GetBuildingSystemId(id);
        }
    }
}
