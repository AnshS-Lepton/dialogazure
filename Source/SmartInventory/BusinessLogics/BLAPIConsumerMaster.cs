using DataAccess;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLAPIConsumerMaster
    {
        public APIConsumerMaster ValidateSource(string source)
        {
            return new DAAPIConsumerMaster().ValidateSource(source);
        }
        public bool ValidateUserName(string userName)
        {
            return new DAAPIConsumerMaster().ValidateUserName(userName);
        }

        public APIConsumerMaster ValidateConsumer(string username, string password, string source)
        {
            return new DAAPIConsumerMaster().ValidateConsumer(username,password,source);
        }
        public APIConsumerMaster ValidateIntegrationServiceConsumer(string username, string password, string source)
        {
            return new DAAPIConsumerMaster().ValidateIntegrationServiceConsumer(username, password, source);
        }

        public List<APIConsumerMasterVw> GetAPIConsumer(VwAPIConsumerMasterFilter objAPIConsumerMasterFilter)
        {
            return new DAAPIConsumerMaster().GetAPIConsumer(objAPIConsumerMasterFilter);
        }
        public APIConsumerMaster GetAPIConsumerById(int Id)
        {
            return new DAAPIConsumerMaster().GetAPIConsumerById(Id);
        }
        public APIConsumerMaster SaveAPIConsumer(APIConsumerMaster objApiConsumer, int userId)
        {
            return new DAAPIConsumerMaster().SaveAPIConsumer(objApiConsumer, userId);
        }
    }
}
