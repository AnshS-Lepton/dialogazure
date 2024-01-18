using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
   public class BLPrintSavedTemplate
    {
        public int SaveTemplate(PrintSavedTemplate savedTemplate)
        {
            return new DAPrintSavedTemplate().SavePrintTemplate(savedTemplate);
        }

        public bool CheckPrintTemplateName(string template_name, int user_id)
        {
            return new DAPrintSavedTemplate().CheckPrintTemplateName(template_name, user_id);
        }

        public List<PrintSavedTemplate> GetPrintTemplateList(int p_id, int p_user_id)
        {
            return new DAPrintSavedTemplate().GetPrintTemplateList(p_id,p_user_id);
        }
        public Dictionary<string, string> ValidatePrintTemplate(string searchText, int user_id)
        {
            return new DAPrintSavedTemplate().ValidatePrintTemplate(searchText, user_id);
        }
        public List<MapScales> GetMapScaleList(int zoomLevel, double centerLat)
        {
            return new DAPrintSavedTemplate().GetMapScaleList(zoomLevel, centerLat);
        }

        public bool DeleteTemplate(int templateId, int user_id)
        {
            return new DAPrintSavedTemplate().DeletePrintTemplateList(templateId, user_id);
        }
    }
}
