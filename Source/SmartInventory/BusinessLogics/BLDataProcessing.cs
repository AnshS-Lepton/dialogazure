using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using DataAccess;
namespace BusinessLogics
{
    public class BLDataProcessing
    {
        public DataTable GetStructure(int ProcessId)
        {
            return new DADataProcessing().GetStructure(ProcessId);
        }

        public DataTable GetSpliceclosure(int ProcessId)
        {
            return new DADataProcessing().GetSpliceclosure(ProcessId);
        }

        public DataTable GetEquipment(int ProcessId)
        {
            return new DADataProcessing().GetEquipment(ProcessId);
        }
        public DataTable GetEquipmentChassis(int ProcessId)
        {
            return new DADataProcessing().GetEquipmentChassis(ProcessId);
        }
        public DataTable GetTransmedia(int ProcessId)
        {
            return new DADataProcessing().GetTransmedia(ProcessId);
        }
        public DataTable GetTransmediaUnits(int ProcessId)
        {
            return new DADataProcessing().GetTransmediaUnits(ProcessId);
        }
        public DataTable GetConnections(int ProcessId)
        {
            return new DADataProcessing().GetConnections(ProcessId);
        }
    }
}

