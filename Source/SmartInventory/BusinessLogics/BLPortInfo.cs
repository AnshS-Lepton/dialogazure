using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLPortInfo
    {
        //public List<ONTPortInfoMaster> GetONTPort(int system_id, int model_id)
        //{
        //    return new DAPortInfo().GetONTPort(system_id, model_id);
        //}
        public bool ChkPortEXist(int modelId)
        {
            return new DAPortInfo().ChkPortEXist(modelId);
        }
    }
   
}
