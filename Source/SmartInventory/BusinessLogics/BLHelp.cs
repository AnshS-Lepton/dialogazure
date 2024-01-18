using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLHelp
    {
        public List<FAQMaster> getFAQ()
        {
            return new DAHelp().getFAQ();
        }
        public List<FAQ_UserManual> getUserManual()
        {
            return new DAHelp_UserManual().getUserManual();
        }
        public List<FAQMaster> searchFAQs(string searchText)
        {
            return new DAHelp().searchFAQs(searchText);
        }
        public  FAQ_UserManual getUserManualById(int id)
        {
            return new DAHelp_UserManual().getUserManualById(id);
        }
        public List<FAQMaster> GetFaqDetails(ViewFaqFilter objFaqFilter)
        {
            return new DAHelp().GetFaqDetails(objFaqFilter);
        }
        public List<FAQ_UserManual> GetFaqUserManualDetails(ViewFaqFilter objFaqFilter)
        {
            return new DAHelp_UserManual().GetFaqUserManualDetails(objFaqFilter);
        }
        public FAQMaster GetFaqById(int id)
        {
            return new DAHelp().GetFaqById(id);
        }
        public FAQMaster SaveFaq(FAQMaster objFaqMst, int userId)
        {
            return new DAHelp().SaveFaq(objFaqMst, userId);
        }
        public FAQ_UserManual SaveUserManual(FAQ_UserManual objFaqFilter)
        {
            return new DAHelp_UserManual().SaveUserManual(objFaqFilter);
        }
        public FAQ_UserManual GetUserManualById(int id)
        {
            return new DAHelp_UserManual().GetUserManualById(id);
        }
        public int DeleteFaqById(int id)
        {
            return new DAHelp().DeleteFaqById(id);
        }
        public int DeleteUserManualById(int id)
        {
            return new DAHelp_UserManual().DeleteUserManualById(id);
        }
        public string getFullText(int id)
        {
            return new DAHelp().getFullText(id);
        }
    }
}
