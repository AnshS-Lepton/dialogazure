using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DATemplate : Repository<LayerRightsTemplateMaster>
    {
        public TemplateViewModel SaveTemplate(TemplateViewModel objModel, int userid)
        {
            try
            {
                LayerRightsTemplateMaster obj = new LayerRightsTemplateMaster();
                var objTemplateDetail = repo.Get(u => u.id == objModel.template_Id);
                if (objTemplateDetail == null)
                {
                    obj.template_name = objModel.template_name;
                    obj.is_active = true;
                    obj.created_on = DateTimeHelper.Now;
                    obj.created_by = userid;
                    repo.Insert(obj);
                    objModel.template_Id = obj.id;
                }
                else
                {
                    objTemplateDetail.template_name = objModel.template_name.Trim();
                    objTemplateDetail.modified_on = DateTimeHelper.Now;
                    objTemplateDetail.modified_by = userid;
                    repo.Update(objTemplateDetail);
                    objModel.template_Id = objTemplateDetail.id;
                }
            }
            catch { throw; }
            return objModel;
        }
        public LayerRightsTemplateMaster GetTemplate(int template_id)
        {
            return repo.Get(a => a.id == template_id);
        }

        public List<LayerRightsTemplateMaster> GetAllTemplates(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<LayerRightsTemplateMaster>("fn_user_rights_get_templates", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy
                }, true);
            }
            catch (Exception ex) { throw ex; }
        }
        public List<LayerRightsTemplateMaster> GetAllTemplates()
        {
            return repo.GetAll(a => a.is_active == true).OrderBy(m => m.template_name).ToList();
        }

        public TemplateViewModel getTemplate_with_permission(int templateid)
        {
            try
            {
                var templateDetail = repo.ExecuteProcedure<TemplateViewModel>("fn_user_rights_get_layer_template_permission", new { vtemplateid = templateid }, true);
                return templateDetail.Count() > 0 ? templateDetail[0] : null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DbMessage DeleteLayerTemplate(int templateid)
        {
            return repo.ExecuteProcedure<DbMessage>("fn_user_rights_delete_layer_template", new { vtemplate_id = templateid }).FirstOrDefault();
        }
        public bool checkTemplateExists(string templateName, int templateId)
        {
            var objLayer = repo.Get(m => m.template_name.ToLower() == templateName.ToLower() && m.id != templateId);
            return objLayer != null;
        }
    }

    public class DATemplatePermission : Repository<LayerRightsTemplatePermission>
    {
        public bool SaveTemplatePermission(List<LayerRightsTemplatePermission> lstTemplatePermission, int TemplateId, int userId, bool isNew)
        {
            if (isNew)
            {
                //insert
                lstTemplatePermission.ForEach(m => { m.template_id = TemplateId; m.created_by = userId; m.created_on = DateTimeHelper.Now; });
                repo.Insert(lstTemplatePermission);
            }
            else
            {
                //Delete previous permission of the same plan before save updated values
                var lstpermission = repo.GetAll(a => a.template_id == TemplateId).ToList();
                repo.DeleteRange(lstpermission);

                //update..
                lstTemplatePermission.ForEach(m => { m.modified_by = userId; m.modified_on = DateTimeHelper.Now; m.template_id = TemplateId; });
                repo.Insert(lstTemplatePermission);

            }
            return true;
        }

        public List<LayerRightsTemplatePermission> GetTemplatePermissionDetails(int TemplateId)
        {
            return repo.GetAll(a => a.template_id == TemplateId).ToList();
        }
        public int DeleteRange(List<LayerRightsTemplatePermission> lstRolePermission)
        {
            var IsDeleted = repo.DeleteRange(lstRolePermission);
            return IsDeleted;
        }
    }
}
