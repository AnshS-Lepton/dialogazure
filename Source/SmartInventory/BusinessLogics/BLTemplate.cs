using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLTemplate
    {
        public TemplateViewModel SaveTemplate(TemplateViewModel objModel, int userid)
        {
            return new DATemplate().SaveTemplate(objModel, userid);
        }
        public List<LayerRightsTemplateMaster> GetAllTemplates(CommonGridAttributes objGridAttributes)
        {
            return new DATemplate().GetAllTemplates(objGridAttributes);
        }

        public List<LayerRightsTemplateMaster> GetAllTemplates()
        {
            return new DATemplate().GetAllTemplates();
        }

        public TemplateViewModel getTemplate_with_permission(int templateid)
        {
            return new DATemplate().getTemplate_with_permission(templateid);
        }

        public LayerRightsTemplateMaster GetTemplate(int templateid)
        {
            return new DATemplate().GetTemplate(templateid);
        }

        public DbMessage DeleteLayerTemplate(int templateid)
        {
            return new DATemplate().DeleteLayerTemplate(templateid);
        }
        public bool checkTemplateExists(string templateName, int templateId)
        {
            return new DATemplate().checkTemplateExists(templateName, templateId);
        }
    }

    public class BLTemplatePermission
    {
        public bool SaveTemplatePermission(List<LayerRightsTemplatePermission> lstTemplatePermission, int TemplateId, int userId, bool isNew)
        {
            return new DATemplatePermission().SaveTemplatePermission(lstTemplatePermission, TemplateId, userId, isNew);
        }
        public List<LayerRightsTemplatePermission> GetTemplatePermissionDetails(int template_id)
        {
            return new DATemplatePermission().GetTemplatePermissionDetails(template_id);
        }
        public int DeleteRange(List<LayerRightsTemplatePermission> lstRolePermission)
        {
            return new DATemplatePermission().DeleteRange(lstRolePermission);
        }
    }
}

