using DataAccess;
using DataAccess.Admin;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BL_Fe_Tools
    {
        public userFeToolMapping SaveFeToolsdetails(userFeToolMapping objfetools, int userId)
        {
            return new DA_Fee_tools().SaveFeToolsdetails(objfetools, userId);


        }
        public List<userFeToolMapping> getfetooldetails(int id)
        {
            return new DA_Fee_tools().getfetooldetails(id);


        }
        public userFeToolMapping getfetoolid(int id)
        {
            return new DA_Fee_tools().getfetoolid(id);
        }
        public List<FE_Tools_Details> GetFettoollist(CommonGridAttributes objGridAttributes)
        {
            return new DA_Fee_tools().GetGroupList(objGridAttributes);
        }
        public List<KeyValueDropDown> GetFEUserDeatils()
        {
            return new DA_Fee_tools().GetFEUserDeatils();

        }
        public int DeleteFetoolsSpecificationById(int id)
        {
            return new DA_Fee_tools().DeleteFetoolsSpecificationById(id);


        }

    }
}
