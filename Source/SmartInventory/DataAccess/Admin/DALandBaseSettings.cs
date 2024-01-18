using DataAccess.Admin;
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
    public class DALandBaseSettings : Repository<LandBaseMaster>
    {
        public LandBaseMaster SaveLandBaseLayer(LandBaseMaster obj, int userId)
        {
            try
            {
                LandBaseMaster obj1 = new LandBaseMaster();
                obj1.id = obj.id;
                obj1.map_border_color = obj.map_border_color;
                obj1.map_seq = obj.map_seq;
                obj1.map_border_thickness = obj.map_border_thickness;
                obj1.map_bg_color = obj.map_bg_color;
                obj1.map_bg_opacity = obj.map_bg_opacity;
                obj1.is_active = obj.is_active;
                obj1.is_center_line_enable = obj.is_center_line_enable;
                obj1.network_id_type = obj.network_id_type;
                obj1.layer_name = obj.layer_name.Trim();
                obj1.is_icon_display_enabled = obj.is_icon_display_enabled;
                obj1.icon_name = obj.icon_name;
                obj1.icon_path = obj.icon_path;
                obj1.user_id = userId;
                if (obj.id > 0)
                {
                    var objData = new DALandBaseSettings().GetLandBaseDetailByID(obj.id);
                    //obj1.layer_name = objData.layer_name.Trim();
                    obj1.geom_type = objData.geom_type;
                    obj1.layer_abbr = objData.layer_abbr;
                    obj1.map_abbr = objData.map_abbr;
                    obj1.network_code_seperator = objData.network_code_seperator; 
                    obj1.report_view_name = objData.report_view_name;
                    obj1.audit_view_name = objData.audit_view_name; 
                    obj1.created_on = objData.created_on;
                    obj1.created_by = objData.created_by;
                    obj1.modified_by = userId;
                    obj1.modified_on = DateTimeHelper.Now;
                    obj1.is_icon_display_enabled = obj.is_icon_display_enabled;
                    if (obj1.is_icon_display_enabled == true)
                    {
                        new DALayerIcon().saveLandbaseDetailsInLayerIcon(obj1);
                    }
                    return repo.Update(obj1);
                }
                else
                {
                    obj1.layer_name = obj.layer_name.Trim();
                    obj1.geom_type = obj.geom_type;
                    obj1.layer_abbr = obj.layer_abbr;
                    obj1.map_abbr = obj.map_abbr;
                    obj1.network_code_seperator = obj.network_code_seperator;
                    if (obj1.geom_type != null)
                    {
                        obj1.report_view_name = "vw_att_details_landbase_" + (obj1.geom_type).ToLower() + "_report";
                        obj1.audit_view_name = "vw_att_details_landbase_" + (obj1.geom_type).ToLower() + "_audit";
                    } 
                    obj1.created_by = userId;
                    obj1.created_on = DateTimeHelper.Now;                   
                    var resp= repo.Insert(obj1);
                    obj1.id = resp.id;
                    if (obj1.is_icon_display_enabled == true)
                    {
                        new DALayerIcon().saveLandbaseDetailsInLayerIcon(obj1);
                    }
                    return resp;
                } 
            }
            catch { throw; }
        }

        public List<LandBaseMaster> GetLayerList(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<LandBaseMaster>("fn_landbase_get_Settings_details", new
                {
                    p_searchtext = objGridAttributes.searchText,
                    is_active = objGridAttributes.is_active,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,

                }, true);
            }
            catch { throw; }
        }
        public LandBaseMaster GetLandBaseDetailByID(int id)
        {
            var obj = repo.Get(m => m.id == id);
            return obj;
        }
        public List<LandBaseMaster> GetLandBaseLayerDetailByID(int id)
        {
            return repo.ExecuteProcedure<LandBaseMaster>("fn_get_landbase_layer_details", new { p_id = id }, true);
        }


        public List<RowCountResult> GetLandBaseLayerSettingRowCount(int layer_id)
        {
            try
            {
                var rowcount = repo.ExecuteProcedure<RowCountResult>("fn_LandBase_get_Dropdown_value_count", new
                {

                    layer_id = layer_id
                }, true);
                return rowcount;
            }

            catch (Exception ex) { throw ex; }

        }

        public int DeleteLandBaseSettingById(int id)
        {
            try
            {
                var objUserId = repo.Get(x => x.id == id);
                if (objUserId != null)
                {
                    return repo.Delete(objUserId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }

        }

        public List<LandBaseMaster> ChkDuplicate_abbrExist(LandBaseMaster obj)
        {
            try
            {
                return repo.GetAll(u => u.layer_abbr.ToLower().Trim() == obj.layer_abbr.ToLower().Trim() || u.map_abbr.ToLower().Trim() == obj.map_abbr.ToLower().Trim() || u.layer_name.ToLower().Trim() == obj.layer_name.ToLower().Trim()).ToList();
            }
            catch
            {
                throw;
            }
        }
        public List<RowCountResult> GetLandBaseDropdownMasterRowCount(int layer_id, int id, string layer_name, string fieldname, string value)
        {
            try
            {
                var rowcount = repo.ExecuteProcedure<RowCountResult>("fn_get_LandBaseDropdownMaster_value_count", new
                {
                    layer_id = layer_id,
                    ddm_id = id,
                    layername = layer_name,
                    fieldname = fieldname,
                    value = value
                }, true);
                return rowcount;
            }

            catch (Exception ex) { throw ex; }

        }
    }
}
