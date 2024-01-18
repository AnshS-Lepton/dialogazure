using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{

    public class DATubeColorSettings : Repository<CableColorSettings>
    {
        public void InsertTubeColorSettings(List<CableColorSettings> obj)
        {
            repo.Insert(obj);
        }
        public int DeleteCableCoreById(int colorid)
        {
            int result = 0;
            try
            {
                var objTube = repo.Get(u => u.color_id == colorid);
                if (objTube != null)
                {
                    return result = repo.Delete(objTube.color_id);
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public int getTotalColorCount(string type)
        {
            try
            {
                var colors = repo.GetAll(m=>m.type==type).ToList();
                return colors.Count();
            }
            catch {
                throw;
            }
        }
        public bool SaveTubeCoreColorInfo(ModelCableColorSettings cableColor)
        {
            try
            {
                CableColorSettings obj = new CableColorSettings();
                foreach (CableColorSettings oColor in cableColor.lstCableColor)
                {
                    var objCore = repo.Get(u => u.color_id == oColor.color_id);
                    if (objCore != null)
                    {
                        if (oColor.color_character != "")
                        {
                            objCore.color_name = oColor.color_name;
                            objCore.color_character = oColor.color_character.Trim().ToUpper();
                            objCore.color_code = oColor.color_code;
                            objCore.type = cableColor.type;
                        objCore.modified_by = cableColor.userId;
                        objCore.modified_on = DateTimeHelper.Now;
                            repo.Update(objCore);
                        }

                    }
                    else
                    {
                        if (oColor.color_character != "")
                        { 
                            obj.color_name = oColor.color_name;
                            obj.color_character = oColor.color_character.Trim().ToUpper();
                            obj.color_code = oColor.color_code;
                            obj.type = cableColor.type;
                        obj.created_by = cableColor.userId;
                        obj.created_on = DateTimeHelper.Now;
                            repo.Insert(obj);
                        }
                    }
                }
            }
            catch { throw; }
            return true;
        }
    }
    public class DATubeCoreColorSettings : Repository<object>
    {
        public List<CableColorSettings> GetTubeColorSettings(int systemid, string type)
        {

            try
            {

                return repo.ExecuteProcedure<CableColorSettings>("fn_get_cable_color_info", new
                {
                    p_system_id = systemid,
                    p_type = type
                }, true);
            }
            catch { throw; }
        }
    }
}
