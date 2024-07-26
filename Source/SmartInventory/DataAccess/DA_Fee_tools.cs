using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using Models;
using Models.Admin;
using static Mono.Security.X509.X520;

namespace DataAccess
{
    public class DA_Fee_tools : Repository<userFeToolMapping>
    {
        public userFeToolMapping SaveFeToolsdetails(userFeToolMapping ObjFeTools,int user_id)
        {
            try
            {
                var result1 = new userFeToolMapping() ;
                var chkExistingUser = repo.GetById(m => m.user_id == ObjFeTools.user_id && m.tool_id == ObjFeTools.tool_id && m.barcode== ObjFeTools.barcode && m.serial_number ==ObjFeTools.serial_number);
                if (chkExistingUser == null)
                {
                    if (ObjFeTools.id != 0)
                    {
                        var objExisiting = repo.GetById(m => m.id == ObjFeTools.id);

                        if (objExisiting != null)
                        {


                            ObjFeTools.created_on = objExisiting.created_on;
                            ObjFeTools.date_value = Convert.ToDateTime(ObjFeTools.date_v);
                            ObjFeTools.modified_by = user_id;
                            ObjFeTools.modified_on = DateTimeHelper.Now;
                            repo.Update(ObjFeTools);
                            result1.action_type = "Update";
                        }
                        else
                        {
                            result1.action_type = "Update";

                        }
                    }

                    else
                    {
                        ObjFeTools.date_value = Convert.ToDateTime(ObjFeTools.date_v);

                        ObjFeTools.created_by = user_id;
                        ObjFeTools.created_on = DateTimeHelper.Now;
                        result1 = repo.Insert(ObjFeTools);
                        result1.action_type = "Save";
                    }
                }
                else
                {
                    result1.action_type = "Duplicate";

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
        public List<FE_Tools_Details> GetGroupList(CommonGridAttributes objGridAttributes,int userid)
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
                    p_user_id = userid
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
        public List<KeyValueDropDown> GetFEUserDeatils(int user_id, string active)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_user_details", new {ID = user_id, Active = active }, true);
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
        public DateTime GetUserDetailsbyid(int userId)
        {
            
            try
            {
                using (MainContext context = new MainContext())
                {
                    string query = string.Format(@"select created_on from vw_user_details where user_id = '{0}' ", userId);
                    var date_value  = context.Database.SqlQuery<DateTime>(query).FirstOrDefault();
                    //created_on = date_value.ToString("yyyy-MM-dd");
                    return date_value;
                }
            }
            catch { throw; }
        }

        public  int AcceptedUserTool(int id)
        {
            try
            {
                using (MainContext context =new MainContext())
                {
                    string query = string.Format(@"update user_tools_mapping utm set is_accepted ='Accept' where utm.id ={0}", id);
                    var value_ = context.Database.ExecuteSqlCommand(query);
                    //context.Database.SqlQuery<bool>(query).FirstOrDefault();
                    return value_;
                }
            }
            catch { throw; }
        }
        public int RejectedUserTool(int id)
        {
            try
            {
                using (MainContext context = new MainContext())
                {
                    string query = string.Format(@"update user_tools_mapping utm set is_accepted ='Reject' where utm.id ={0}", id);
                    var value_ = context.Database.ExecuteSqlCommand(query);
                    //context.Database.SqlQuery<bool>(query).FirstOrDefault();
                    return value_;
                }
            }
            catch { throw; }
        }
    }
}
