using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAPrintSavedTemplate : Repository<PrintSavedTemplate>
    {
        public int SavePrintTemplate(PrintSavedTemplate entity)
        {
            int result = 0;
            try
            {
                var objTemp = repo.Get(u => (u.user_id == entity.user_id) && (u.template_name==entity.template_name || u.id == entity.id));
                if (objTemp != null)
                {
                    objTemp.template_name = string.IsNullOrEmpty(entity.template_name) ? objTemp.template_name : entity.template_name;              
                    objTemp.job_id = string.IsNullOrEmpty(entity.job_id) ? objTemp.job_id : entity.job_id;
                    objTemp.department = string.IsNullOrEmpty(entity.department) ? objTemp.department : entity.department;
                    objTemp.plotted_by = string.IsNullOrEmpty(entity.plotted_by) ? objTemp.plotted_by : entity.plotted_by;
                    objTemp.team = string.IsNullOrEmpty(entity.team) ? objTemp.team : entity.team;
                    objTemp.drawn_by = string.IsNullOrEmpty(entity.drawn_by) ? objTemp.drawn_by : entity.drawn_by;
                    objTemp.checked_by = string.IsNullOrEmpty(entity.checked_by) ? objTemp.checked_by : entity.checked_by;
                    objTemp.rechecked_by = string.IsNullOrEmpty(entity.rechecked_by) ? objTemp.rechecked_by : entity.rechecked_by;
                    objTemp.approved_by = string.IsNullOrEmpty(entity.approved_by) ? objTemp.approved_by : entity.approved_by;
                    objTemp.x_document_index = string.IsNullOrEmpty(entity.x_document_index) ? objTemp.x_document_index : entity.x_document_index;
                    objTemp.y_document_index = string.IsNullOrEmpty(entity.y_document_index) ? objTemp.y_document_index : entity.y_document_index;
                    objTemp.phase = string.IsNullOrEmpty(entity.phase) ? objTemp.phase : entity.phase;
                    objTemp.plan = string.IsNullOrEmpty(entity.plan) ? objTemp.plan : entity.plan;
                    objTemp.prov_dir = string.IsNullOrEmpty(entity.prov_dir) ? objTemp.prov_dir : entity.prov_dir;
                    objTemp.fdc_no = string.IsNullOrEmpty(entity.fdc_no) ? objTemp.fdc_no : entity.fdc_no;
                    objTemp.olt = string.IsNullOrEmpty(entity.olt) ? objTemp.olt : entity.olt;
                    repo.Update(objTemp);
                }
                else
                {
                    result = 1;
                    objTemp = repo.Insert(entity);
                }
                return result;
            }
            catch (Exception ex) { throw ex; }
        }


        public bool CheckPrintTemplateName(string template_name, int user_id)
        {
            bool isNameExist = false;
            try
            {
                var objTemp = repo.Get(u => (u.user_id == user_id) && (u.template_name == template_name ));
                if (objTemp != null)
                {
                    isNameExist = true;
                }
              
                return isNameExist;
            }
            catch (Exception ex) { throw ex; }
        }


        public List<PrintSavedTemplate> GetSavePrintTemplateList(int id, int user_id)
        {
            try
            {                
                var result = repo.ExecuteProcedure<PrintSavedTemplate>("fn_get_print_saved_templates", new { p_id = id, p_user_id = user_id }, true);
                return result != null && result.Count > 0 ? result : new List<PrintSavedTemplate>();
            }
            catch { throw; }
        }

       

        public List<PrintSavedTemplate> GetPrintTemplateList(int id, int user_id)
        {
            try
            {
                var result = repo.ExecuteProcedure<PrintSavedTemplate>("fn_get_print_saved_templates", new { p_id = id, p_userid = user_id }, true);
                return result != null && result.Count > 0 ? result : new List<PrintSavedTemplate>();
            }
            catch { throw; }
        }

        public Dictionary<string, string> ValidatePrintTemplate(string searchText, int user_id)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_print_saved_template_validate", new { p_searchtext = searchText,p_userid = user_id }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch { throw; }
        }

        public List<MapScales> GetMapScaleList(int zoomLevel, double centerLat)
        {
            try
            {
                var result = repo.ExecuteProcedure<MapScales>("fn_get_map_scaleList", new { p_zoomLevel = zoomLevel, p_centerLat = centerLat }, true);
                return result != null && result.Count > 0 ? result : new List<MapScales>();
            }
            catch { throw; }
        }
        public string GetNetworkId(int systemId, string entityType)
        {
            try
            {
                var result = repo.ExecuteProcedure<string>("fn_get_network_id", new { p_entity_type = entityType, p_entity_system_id = systemId },false).FirstOrDefault();
                return result;
            }
            catch { throw; }
        }


        
        public bool DeletePrintTemplateList(int templateId, int user_id)
        {
            
            try
            {
               return repo.ExecuteProcedure<bool>("fn_Delete_print_templates", new { p_templateId = templateId, p_userid = user_id }, false).FirstOrDefault();
            }
            catch { throw; }
        }

    }
}
