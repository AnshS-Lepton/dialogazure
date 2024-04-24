using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using Models.WFM;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.TempUpload
{
    public class DATempCDBAttributes : Repository<TempCDBAttributes>
    {
        public void Save(List<TempCDBAttributes> lst)
        {
            repo.Insert(lst);
        }

        public void DeleteFromTempCDBTable(int uploadid)
        {
            try
            {
                var obj = repo.Get(u => u.upload_id == uploadid);
                if (obj != null)
                {
                     repo.Delete(obj.upload_id);
                }
            }
            catch
            {
                throw;
            }
        }

        public void ValidateCDBAttributes(int uploadid)
        {
            repo.ExecuteProcedure("fn_uploader_validation_cdb_attributes", new
            {
                p_upload_id = uploadid
            });
        }
           
        public List<Mapping> GetMappingForCDBCable(string layerName)
        {
            try
            {
                return repo.ExecuteProcedure<Mapping>("fn_uploader_get_entity_template_for_cdb_attributes", new { p_entity_type = layerName }, true);
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> getUploadTemplateCDBAttributesRecords(string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_uploader_get_template_cdb_attributes_records", new { p_entity_type = entityType }, true);
            }
            catch { throw; }
        }
    }
}
