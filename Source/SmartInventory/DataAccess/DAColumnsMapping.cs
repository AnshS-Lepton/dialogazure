using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAMappingTemplate : Repository<ColumnMappingTemplate>
    {
        public List<ColumnMappingTemplate> getMappingTemplates(int layerId, int userId)
        {
            try { return repo.GetAll(m => m.layer_id == layerId && m.created_by == userId).ToList(); }
            catch { throw; }
        }
        public List<ColumnMappingTemplate> getRegionProvinceMappingTemplates(int layerId, int userId, string boundary_type)
        {
            try
            {
                return repo.ExecuteProcedure<ColumnMappingTemplate>("fn_uploader_get_RegionProvinceMapping_template", new { p_entity_type = boundary_type, p_layerId = layerId, p_userId = userId }, true);
            }
            catch { throw; }
        }
        public ColumnMappingTemplate SaveMappingTemplate(ColumnMappingTemplate mappingTemplate, int userId)
        {
            try
            {
                mappingTemplate.created_by = userId;
                mappingTemplate.created_on = DateTime.Now;
                var result = repo.Insert(mappingTemplate);
                new DAColumnsMapping().SaveColumnsMapping(mappingTemplate.listMappedColumns, result.id);
                return result;
            }
            catch { throw; }
        }

    }
    public class DAColumnsMapping : Repository<ColumnMapping>
    {
        public void SaveColumnsMapping(List<ColumnMapping> listMappings, int templateId)
        {
            try
            {
                //if you want to save only imported_column_name is null
                // listMappings = listMappings.Where(m => string.IsNullOrEmpty(m.imported_column_name) == false).ToList();   

                listMappings.Where(m => string.IsNullOrEmpty(m.imported_column_name) == true).Select(u => u.imported_column_name = "").ToList(); 
                 
                var listMappedColums = repo.GetAll(m => m.template_id == templateId).ToList();
                if (listMappedColums.Count > 0)
                {
                    repo.Delete(listMappings);
                }
                listMappings.ForEach(m => m.template_id = templateId);
                repo.Insert(listMappings);
            }
            catch { throw; }
        }
        public List<ColumnMapping> getColumnsMapping(int templateId)
        {
            try
            {
                return repo.GetAll(m => m.template_id == templateId).ToList();
            }
            catch { throw; }
        }

    }
}
