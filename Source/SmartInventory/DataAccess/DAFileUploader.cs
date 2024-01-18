using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAExternalFileUploader : Repository<ExternalDataUploader>
    {
        public int DeleteExtrnlDataFileDtl(int fileid)
        {
            try
            {
                var objId = repo.Get(x => x.id == fileid);
                if (objId != null)
                {
                    return repo.Delete(objId.id);
                }
                else
                {
                    return 0;
                }

            }
            catch { throw; }
        }
        public bool SaveExternalFileDtl(ExternalDataUploader obj, int user_id)
        {
            try
            {
                var dataFound = repo.Get(u => u.id == obj.id);
                if (dataFound == null)
                {
                    obj.created_on = DateTimeHelper.Now;
                    obj.created_by = user_id;
                    obj = repo.Insert(obj);
                    return true;
                }
                else
                    return false;
            }
            catch { throw; }
        }
        public List<ExternalDataFileList> GetExternalDataFileDetails(ExternalDataFilter objExtnlDtaFilter, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ExternalDataFileList>("fn_get_External_filedetails", new
                {
                    p_user_id = user_id,
                    p_searchby = objExtnlDtaFilter.searchBy,
                    p_searchtext = objExtnlDtaFilter.searchText,
                    p_pageno = objExtnlDtaFilter.currentPage,
                    p_pagerecord = objExtnlDtaFilter.pageSize,
                    p_sortcolname = objExtnlDtaFilter.orderBy,
                    p_sorttype = objExtnlDtaFilter.sort,
                    p_totalrecords = objExtnlDtaFilter.totalRecord
                }, true);
                return res;
            }
            catch { throw; }
        }

        public ExternalDataUploader getDownloadFileDetails(int fileid)
        {
            try
            {
                return repo.GetById(m => m.id == fileid);
            }
            catch { throw; }
        }
        public bool getUpdatedFileSize(ExternalDataUploader obj)
        {
            try
            {
                var dataFound = repo.Get(u => u.id == obj.id);
                if (dataFound != null)
                {
                    dataFound.file_size = obj.file_size;
                    repo.Update(dataFound);
                    return true;
                }
                else
                    return false;
            }
            catch { throw; }
        }


    }
}
