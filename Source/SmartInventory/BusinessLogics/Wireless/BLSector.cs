using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLSector
    {
        public SectorMaster SaveSectorEntity(SectorMaster objSectorMaster, int userId)
        {
            return new DASector().SaveSectorEntity(objSectorMaster, userId);
        }
        public int DeleteSectorById(int systemId)
        {
            return new DASector().DeleteSectorById(systemId);
        }
        public SectorMaster getSectorDetails(int systemId)
        {
            return new DASector().getSectorDetails(systemId);
        }
        public List<SectorMaster> GetSectorByTowerId(int systemId)
        {
            return new DASector().GetSectorByTowerId(systemId);
        }
    }
}
