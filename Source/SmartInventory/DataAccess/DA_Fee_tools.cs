using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using Models;
using Models.Admin;

namespace DataAccess
{
    public class DA_Fee_tools : Repository<userFeToolMapping>
    {
        public userFeToolMapping SaveFeToolsdetails(userFeToolMapping objLyrGroup,int user_id)
        {
            try
            {
                var result1 = new userFeToolMapping() ;
                if (objLyrGroup.id != 0)
                {
                    var objExisiting = repo.GetById(m => m.id == objLyrGroup.id);
                    
                    if (objExisiting != null)
                    {
                        if (objExisiting.image_value != false)
                        {
                            objLyrGroup.image_value = objExisiting.image_value;
                        }
                        if (objExisiting.document_value != false)
                        {
                            objLyrGroup.document_value = objExisiting.document_value;
                        }
                        objLyrGroup.date_value = Convert.ToDateTime(objLyrGroup.date_v);
                        objLyrGroup.modified_by = user_id;
                        objLyrGroup.modified_on = DateTimeHelper.Now;
                        repo.Update(objLyrGroup);
                        result1.action_type = "Update";
                    }
                }
                else
                {
                    objLyrGroup.date_value = Convert.ToDateTime(objLyrGroup.date_v);

                    objLyrGroup.created_by = user_id;
                    objLyrGroup.created_on = DateTimeHelper.Now;
                    result1 = repo.Insert(objLyrGroup);
                    result1.action_type = "Save";
                }
                return result1;
            }
            catch { throw; }
        }
        public userFeToolMapping getfetoolid (int Id)
        {
            try
            {
                var data = repo.Get(m => m.id == Id);
                return data;
            }
            catch { throw; }
        }
        public List<userFeToolMapping> getfetooldetails(int id)
        {
            try
            {
                return repo.ExecuteProcedure<userFeToolMapping>("fn_get_fetools_record", new
                {
                    ID = id
                }, true); ;
            }
            catch { throw; }
        }
        public List<FE_Tools_Details> GetGroupList(CommonGridAttributes objGridAttributes)
        {
            try
            {
                return repo.ExecuteProcedure<FE_Tools_Details>("fn_get_fetools_details", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    P_TOTALRECORDS = objGridAttributes.totalRecord,
                }, true);
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetFEUserDeatils()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_user_details", new { }, true);
            }
            catch { throw; }
        }
        public int DeleteFetoolsSpecificationById(int id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
    }
}
