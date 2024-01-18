using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLCabinet
    {
        public CabinetMaster SaveEntityCabinet(CabinetMaster objCabinetMaster, int userId)
        {
            return new DACabinet().SaveEntityCabinet(objCabinetMaster, userId);
        }
        public int DeleteCabinetById(int systemId)
        {
            return new DACabinet().DeleteCabinetById(systemId);
        }
    }
}
