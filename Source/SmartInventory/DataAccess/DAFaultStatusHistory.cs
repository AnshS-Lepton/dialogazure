using DataAccess;
using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
  public  class DAFaultStatusHistory : Repository<FaultStatusHistory>
    {
        DAFaultStatusHistory()
        {

        }
        private static DAFaultStatusHistory objFaultStatus = null;
        private static readonly object lockObject = new object();
        public static DAFaultStatusHistory Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objFaultStatus == null)
                    {
                        objFaultStatus = new DAFaultStatusHistory();
                    }
                }
                return objFaultStatus;
            }
        }
        public FaultStatusHistory SaveFaultStatusHistory(FaultStatusHistory objFaultStatus, int userId)
        {
            try
            {
                bool isChanged = false;
                var faultStatusInfo = repo.GetAll(u => u.fault_system_id == objFaultStatus.fault_system_id).OrderByDescending(x=>x.id).FirstOrDefault(); 
                if (faultStatusInfo!=null)
                {
                     
                    // Compare Fault Information 
                    isChanged = DAUtility.CompareObjectProperties(faultStatusInfo, objFaultStatus);
                   
                }
                if (!isChanged)
                {
                    objFaultStatus.created_on = DateTimeHelper.Now;
                    objFaultStatus.created_by = userId;
                    var response = repo.Insert(objFaultStatus);
                }
                return objFaultStatus;
            }
            catch { throw; }
        } 
        //public bool CompareObjectProperties(object oldValue, object newValue)
        //{
        //    bool isMatched = true; 
        //    FieldInfo[] fields= oldValue.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToArray();
        //    string[] arrIgnoreFields = new string[6] { "<id>k__BackingField", "<created_by>k__BackingField", "<modified_by>k__BackingField", "<created_on>k__BackingField", "<modified_on>k__BackingField", "<lstFaultStatus>k__BackingField" };
        //    foreach (FieldInfo fi in fields)
        //    {
        //        //arrIgnoreFields.Contains(fi.getkey)
        //        if (!arrIgnoreFields.Contains(fi.Name) && Convert.ToString(fi.GetValue(oldValue)) != Convert.ToString(fi.GetValue(newValue)))
        //        {
        //            isMatched = false;
        //            return isMatched;
        //        }               
        //    }
        //    return isMatched;
        //}
        public FaultStatusHistory GetFaultStatusHistoryById(int systemId)
        {
            try
            {
                return repo.GetAll(m => m.fault_system_id == systemId).OrderByDescending(x=>x.id).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<FaultStatusHistory> getFaultStatusHistoryList(int fault_system_id)
        {
            try
            {
                var result = repo.ExecuteProcedure<FaultStatusHistory>("fn_get_fault_stauts_history", new { p_fault_system_id = fault_system_id }, true);
                return result != null && result.Count > 0 ? result : new List<FaultStatusHistory>(); 
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<ExportFaultHistory> exportFaultStatusHistoryList(int fault_system_id)
        {
            try
            { 
                var result = repo.ExecuteProcedure<ExportFaultHistory>("fn_get_export_fault_stauts_history", new { p_fault_system_id = fault_system_id }, true);
                return result != null && result.Count > 0 ? result : new List<ExportFaultHistory>();

            }
            catch (Exception)
            {

                throw;
            }
        }
        public int DeleteStatusHistorybyFaultId(int systemId)
        {
            int result = 0;
            try
            { 
                var objFault = repo.Get(u => u.fault_system_id == systemId);
                if (objFault != null)
                {
                    result = repo.Delete(objFault.fault_system_id);
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        }
}
