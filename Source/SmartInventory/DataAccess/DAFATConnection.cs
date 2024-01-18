using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;


namespace DataAccess
{
    public class DAFATConnection : Repository<FatProcessSummary>
    {
        public List<FATDetail> GetConnectionDetails(FATDetailprocess objFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<FATDetail>("fn_fat_getconnection_details", new
                {
                    p_fsa_system_id = objFilter.systemId,
                    p_connection_status = objFilter.p_connection_status
                }, true);
                return lst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetConnectionCount(int fsa_system_id)
        {
            int iRetVal = 0;
            try
            {
                iRetVal = repo.ExecuteProcedure<int>("fn_fat_getconnected_entity_count", new
                {
                    p_fsa_system_id = fsa_system_id                   
                }, false).FirstOrDefault();              
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return iRetVal;
        }

        public FatProcessRunningStatus GetSetBKGStatus(int fsa_system_id, string sAction, string sMessage = "")
        {
            FatProcessRunningStatus oFatProcessRunningStatus = new FatProcessRunningStatus();
            try
            {
                oFatProcessRunningStatus = repo.ExecuteProcedure<FatProcessRunningStatus>("fn_fat_update_bkg_process", new
                {
                    p_fsa_system_id = fsa_system_id,
                    p_connection_status = sMessage,
                    p_action = sAction
                }, true).FirstOrDefault();
                if(oFatProcessRunningStatus == null)
                {
                    oFatProcessRunningStatus = new FatProcessRunningStatus();
                    oFatProcessRunningStatus.process_message = "";
                    oFatProcessRunningStatus.bt_lock = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oFatProcessRunningStatus;
        }


        

        public FatProcessResult CreateSplicing(FatProcessSummary oFatProcessSummary)
        {
            var oRetVal = new FatProcessResult();
            try
            {
                DAFATConnection oDAFATConnection = new DAFATConnection();
                var resultItem = oDAFATConnection.SaveFatProcessSummary(oFatProcessSummary);
                List<string> lstResultString = repo.ExecuteProcedure<string>("fn_fat_generate_splicing", new
                {
                    p_fsa_system_id = resultItem.sub_area_system_id,
                    p_user_id = resultItem.created_by,
                    p_fat_process_id = resultItem.fat_process_id
                }, false);
                if (resultItem != null && lstResultString != null)
                {
                    string[] separatingStrings = { "##" };
                    string[] sVales = lstResultString[0].Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                    if (sVales.Length >= 2)
                    {
                        oRetVal.fat_process_id = resultItem.fat_process_id;
                        oRetVal.status = sVales[0].Split(':')[1] == "SUCCESS" ? true : false;
                        oRetVal.MESSAGE = sVales[1].Split(':')[1];                        
                        resultItem.process_status = oRetVal.status ? "Success" : "Failed";
                        resultItem.remarks = oRetVal.MESSAGE;
                        resultItem.approval_status = oRetVal.status ? "Pending" : "";
                        resultItem.process_end_time = DateTime.Now;
                        oDAFATConnection.SaveFatProcessSummary(resultItem);
                    }
                    else
                    {
                        oRetVal.status = false;
                    }
                }
                return oRetVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FatProcessResult UpdateSplicingStatus(int sub_area_system_id, int user_id, string action_name)
        {
            var oRetVal = new FatProcessResult();
            List<string> lstResultString = repo.ExecuteProcedure<string>("fn_fat_update_connection_status", new
            {
                p_fsa_system_id = sub_area_system_id,
                p_user_id = user_id,
                p_action = action_name
            }, false);

            if (lstResultString != null)
            {
                string[] separatingStrings = { "##" };
                string[] sVales = lstResultString[0].Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                if (sVales.Length >= 3)
                {                    
                    oRetVal.status = sVales[0].Split(':')[1] == "SUCCESS" ? true : false;
                    oRetVal.MESSAGE = sVales[1].Split(':')[1];
                    oRetVal.fat_process_id = Convert.ToInt32(sVales[2].Split(':')[1]);
                }
                else
                {
                    oRetVal.status = false;
                }
            }
            return oRetVal;
        }
        public FatProcessSummary SaveFatProcessSummary(FatProcessSummary oFatProcessSummary)
        {
            var objitem = repo.Get(x => x.fat_process_id == oFatProcessSummary.fat_process_id);
            if (objitem != null)
            {
                objitem.sub_area_system_id = oFatProcessSummary.sub_area_system_id;
                objitem.sub_area_name = oFatProcessSummary.sub_area_name;
                objitem.process_status = oFatProcessSummary.process_status;
                objitem.created_by = oFatProcessSummary.created_by;
                objitem.created_on = oFatProcessSummary.created_on;
                objitem.modified_by = oFatProcessSummary.modified_by;
                objitem.modified_on = oFatProcessSummary.modified_on;
                objitem.process_start_time = oFatProcessSummary.process_start_time;
                objitem.process_end_time = oFatProcessSummary.process_end_time;
                objitem.remarks = oFatProcessSummary.remarks;
                objitem.approval_status = oFatProcessSummary.approval_status;
                var resultItem = repo.Update(objitem);
                return resultItem;
            }
            else
            {
                objitem = new FatProcessSummary();
                objitem.sub_area_system_id = oFatProcessSummary.sub_area_system_id;
                objitem.sub_area_name = oFatProcessSummary.sub_area_name;
                objitem.process_status = "InProcess";
                objitem.created_by = oFatProcessSummary.created_by;
                objitem.created_on = oFatProcessSummary.created_on;
                objitem.modified_by = oFatProcessSummary.modified_by;
                objitem.modified_on = oFatProcessSummary.modified_on;
                objitem.process_start_time = oFatProcessSummary.process_start_time;
                objitem.process_end_time = oFatProcessSummary.process_end_time;
                objitem.remarks = oFatProcessSummary.remarks;
                objitem.approval_status = "";
                var resultItem = repo.Insert(objitem);
                return resultItem;
            }
        }
        public bool UpdatePortStatus(int processId,string action)
        {
            try
            {
                return repo.ExecuteProcedure<bool>("fn_update_port_status", new
                {
                    p_process_id = processId,
                    p_action = action
                }, false).FirstOrDefault(); ;
            }
            catch { throw; }
        }

       public bool UpdatSnapCableEndPoint(int processId)
        {
            try
            {
                return repo.ExecuteProcedure<bool>("fn_snap_cable_endpoint", new
                {
                    p_process_id = processId
                }, false).FirstOrDefault(); ;
            }
            catch { throw; }
        }
    }
}
