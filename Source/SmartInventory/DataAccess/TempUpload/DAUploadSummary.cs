using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;
using System.Data;
using Newtonsoft.Json;

namespace DataAccess
{
    public sealed class DAUploadSummary : Repository<UploadSummary>
    {
        private static DAUploadSummary objDAUploadSummary = null;
        private static readonly object lockObject = new object();
        public static DAUploadSummary Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAUploadSummary == null)
                    {
                        objDAUploadSummary = new DAUploadSummary();
                    }
                }
                return objDAUploadSummary;
            }
        }
        private DAUploadSummary()
        {

        }
        public UploadSummary Save(UploadSummary uploadSummary)
        {
            var obj = repo.Insert(uploadSummary);
            return obj;
        }
        public UploadSummary Get(int uploadId)
        {
            return repo.Get(m => m.id == uploadId);
        }
        public DataTable GetUploadLogs(int uploadid, string status)
        {
            try
            {
                List<string> json = repo.ExecuteProcedure<string>("fn_uploader_getuploadlogs", new { uploadid = uploadid, status = status });
                DataTable dt = new DataTable();
                if (json[0] != null)
                {
                    dt = (DataTable)JsonConvert.DeserializeObject(json[0], (typeof(DataTable)));
                }
                dt.TableName = status + "_Logs";
                return dt;

            }
            catch { throw; }
        }

        public DataTable GetUploadId(string planId, int user_id,string entity_type)
        {
            try
            {
                List<string> json = repo.ExecuteProcedure<string>("fn_uploader_getuploadId", new { planId = planId, userId = user_id, entity_type= entity_type });
                DataTable dt = new DataTable();
                if (json[0] != null)
                {
                    dt = (DataTable)JsonConvert.DeserializeObject(json[0], (typeof(DataTable)));
                } 
                return dt;

            }
            catch { throw; }
        }

        public bool Delete(int id)
        {
            var obj = repo.Delete(id);
            return obj == 1 ? true : false;
        }

        public List<ViewUploadSummary> GetUploadSummaryForGrid(UploadLogFilter objFilter)
        {
            var lst = repo.ExecuteProcedure<ViewUploadSummary>("fn_upload_summary", new
            {
                p_pagerecord = objFilter.pageSize,
                p_pageno = objFilter.page,
                p_fromdate = objFilter.fromDate,
                p_todate = objFilter.toDate,
                p_entityname = objFilter.entityName,
                p_userid = objFilter.userId
            }, true);
            return lst;
        }

        public void UpdateStatus(UploadSummary uploadSummary)
        {
            repo.Update(uploadSummary);
        }

        public string ShowOnMap(int id)
        {
            var json = repo.ExecuteProcedure<string>("fn_uploader_getuploadlogs", new { uploadid = id, status = "SHOW_ON_MAP" }, false)[0];
            return json.ToString();
        }
        public List<Dictionary<string, string>> getUploadTemplateSampleRecords(string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_uploader_get_template_sample_records", new { p_entity_type = entityType }, true);
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> getUploadTemplateGuideLines(string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_uploader_get_template_guideline", new { p_entity_type = entityType }, true);
            }
            catch { throw; }
        }
        public DbMessage checkTemplateExists(string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_uploader_check_template", new { p_entity_type = entityType }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> getKMLTemplate(string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_uploader_get_template_kml", new { p_entity_type = entityType }, true).ToList();
            }
            catch { throw; }
        }
    }
}
