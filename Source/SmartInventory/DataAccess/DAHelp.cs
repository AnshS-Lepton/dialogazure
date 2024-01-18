using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAHelp : Repository<FAQMaster>
    {
        public List<FAQMaster> getFAQ()
        {
            return repo.GetAll().OrderByDescending(m => m.created_on).ToList();
        }
        public List<FAQMaster> searchFAQs(string searchText)
        {
            var lstItems = repo.ExecuteProcedure<FAQMaster>("fn_help_searchFAQs", new { p_searchText = searchText }, true);
            return lstItems != null ? lstItems : new List<FAQMaster>();
        }

        public List<FAQMaster> GetFaqDetails(ViewFaqFilter objfaqFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<FAQMaster>("fn_get_faq_details", new
                {
                    p_pageno = objfaqFilter.currentPage,
                    p_pagerecord = objfaqFilter.pageSize,
                    p_sortcolname = objfaqFilter.sort,
                    p_sorttype = objfaqFilter.orderBy,
                    p_searchBy = objfaqFilter.searchBy,
                    p_searchText = objfaqFilter.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }
        public FAQMaster GetFaqById(int id)
        {
            try
            {
                return repo.Get(u => u.id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public FAQMaster SaveFaq(FAQMaster input, int userId)
        {
            try
            {
                var objFaqMst = repo.Get(x => x.id == input.id);
                if (input.id > 0)
                {

                    objFaqMst.category = input.category;
                    objFaqMst.question = input.question;
                    objFaqMst.answer = input.answer;
                    objFaqMst.modified_by = userId;
                    objFaqMst.modified_on = DateTimeHelper.Now;
                    return repo.Update(objFaqMst);

                }
                else
                {
                    input.created_by = userId;
                    input.created_on = DateTimeHelper.Now; ;
                    return repo.Insert(input);


                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int DeleteFaqById(int id)
        {
            try
            {
                var objId = repo.Get(x => x.id == id);
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
        public string getFullText(int id)
        {
            try
            {
                var otherchangesDetails = repo.GetById(m => m.id == id);
                if (otherchangesDetails != null) { return otherchangesDetails.answer; }
                return "";
            }
            catch { throw; }
        }
    }
}
    public class DAHelp_UserManual : Repository<FAQ_UserManual>
        {
            public List<FAQ_UserManual> getUserManual()
            {
                return repo.GetAll().ToList();
            }
        public  FAQ_UserManual getUserManualById(int id)
        {
            var result = repo.Get(x => x.id == id);
            return result != null ? result : new FAQ_UserManual();
        }
       
        public List<FAQ_UserManual> GetFaqUserManualDetails(ViewFaqFilter objfaqFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<FAQ_UserManual>("fn_get_faq_usermanual_details", new
                {
                    p_pageno = objfaqFilter.currentPage,
                    p_pagerecord = objfaqFilter.pageSize,
                    p_sortcolname = objfaqFilter.sort,
                    p_sorttype = objfaqFilter.orderBy,
                    p_searchBy = objfaqFilter.searchBy,
                    p_searchText = objfaqFilter.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public FAQ_UserManual SaveUserManual(FAQ_UserManual input)
        {
            try
            {
                var objFaqMst = repo.Get(x => x.id == input.id);
                if (input.id > 0)
                {

                    objFaqMst.category = input.category;
                    objFaqMst.file_name = input.file_name;
                    objFaqMst.display_name = input.display_name;
                    objFaqMst.file_size = input.file_size;
                    objFaqMst.file_extension = input.file_extension;
                    objFaqMst.file_url = input.file_url;
                    objFaqMst.modified_by =input.created_by;
                    objFaqMst.modified_on = DateTimeHelper.Now;
                    return repo.Update(objFaqMst);

                }
                else
                {
                    input.created_by =input.created_by;
                    input.created_on = DateTimeHelper.Now; ;
                    return repo.Insert(input);


                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public FAQ_UserManual GetUserManualById(int id)
        {
            try
            {
                return repo.Get(u => u.id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int DeleteUserManualById(int id)
        {
            try
            {
                var objId = repo.Get(x => x.id == id);
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
    }
         

