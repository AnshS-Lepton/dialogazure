using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models.Admin;

namespace DataAccess
{
    public class DADataProcessing : Repository<downloadbckupfile>
    {
        public DataTable GetStructure(int ProcessId)
        {
            try
            {
                string sQuery = string.Format("select  * from vw_process_structure_details  where process_id = {0} ", ProcessId);
                return repo.GetDataTable(sQuery);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetSpliceclosure(int ProcessId)
        {
            try
            {
                string sQuery = string.Format("select  * from vw_process_spliceclosure_details  where process_id = {0} ", ProcessId);
                return repo.GetDataTable(sQuery);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetEquipment(int ProcessId)
        {
            try
            {
                string sQuery = string.Format("select  * from vw_process_equipment_details  where process_id = {0} ", ProcessId);
                return repo.GetDataTable(sQuery);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetEquipmentChassis(int ProcessId)
        {
            try
            {
                string sQuery = string.Format("select  * from vw_process_equipment_chassis_details  where process_id = {0} ", ProcessId);
                return repo.GetDataTable(sQuery);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetTransmedia(int ProcessId)
        {
            try
            {
                string sQuery = string.Format("select  * from vw_process_transmedia_details  where process_id = {0} ", ProcessId);
                return repo.GetDataTable(sQuery);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetTransmediaUnits(int ProcessId)
        {
            try
            {
                string sQuery = string.Format("select  * from vw_process_transmedia_unit_details  where process_id = {0} ", ProcessId);
                return repo.GetDataTable(sQuery);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetConnections(int ProcessId)
        {
            try
            {
                string sQuery = string.Format("select  * from vw_process_connection_details  where process_id = {0} ", ProcessId);
                return repo.GetDataTable(sQuery);
            }
            catch
            {
                throw;
            }
        }
    }    
}
