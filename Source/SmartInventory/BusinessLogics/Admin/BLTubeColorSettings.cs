using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.Admin;
using DataAccess.Admin;
namespace BusinessLogics.Admin
{
    public class BLTubeColorSettings
    {
        public void InsertTubeColorSettings(List<CableColorSettings> obj)
        {
            new DATubeColorSettings().InsertTubeColorSettings(obj);
        }
        public int DeleteCableCoreById(int colorid)
        {
           return new DATubeColorSettings().DeleteCableCoreById(colorid);
        }
        public int getTotalColorCount(string type)
        {
            return new DATubeColorSettings().getTotalColorCount(type);
        }
        public bool SaveTubeCoreColorInfo(ModelCableColorSettings corelst)
        {
            return new DATubeColorSettings().SaveTubeCoreColorInfo(corelst);
        }
    }
    public class BLTubeCoreColorSettings 
    {
        public List<CableColorSettings> GetTubeColorSettings(int systemid,string type)
        {
           return new DATubeCoreColorSettings().GetTubeColorSettings(systemid, type);
        }
    }
}
