using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;
namespace DataAccess
{
    public class DAPortInfo : Repository<object>
    {
        //public List<ONTPortInfoMaster> GetONTPort(int system_id, int model_id)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<ONTPortInfoMaster>("fn_get_ont_port_info", new { p_systemid = system_id, p_model_id = model_id });
        //    }
        //    catch { throw; }
        //}

        public bool ChkPortEXist(int modelId)
        {
            try
            {
                var chk= repo.ExecuteProcedure<bool>("fn_chk_port_exist", new { P_modelId = modelId });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                  return  false;
            }
            catch { throw; }
        }
        
    }
 
}
