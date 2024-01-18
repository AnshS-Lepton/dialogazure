using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAAPIConsumerMaster : Repository<APIConsumerMaster>
    {
        public APIConsumerMaster ValidateSource(string source)
        {
            try
            {

                return repo.Get(u => u.source.ToUpper() == source.ToUpper());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool ValidateUserName(string userName)
        {
            try
            {

                APIConsumerMaster obj = repo.Get(u => u.user_name.ToUpper() == userName.ToUpper());
                return obj != null ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public APIConsumerMaster ValidateConsumer(string username, string password, string source)
        {
            try
            {
                return repo.ExecuteProcedure<APIConsumerMaster>("fn_check_api_consumer_details", new { p_username = username, p_password = password, p_source = source }, true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public APIConsumerMaster ValidateIntegrationServiceConsumer(string username, string password, string source)
        {
            try
            {
                return repo.ExecuteProcedure<APIConsumerMaster>("fn_check_integrationservice_consumer_details", new { p_username = username, p_password = password, p_source = source }, true).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<APIConsumerMasterVw> GetAPIConsumer(VwAPIConsumerMasterFilter objAPIConsumerMasterFilter)
        {
            try
            {
                return repo.ExecuteProcedure<APIConsumerMasterVw>("fn_get_api_consumer_master", new
                {
                    p_PAGENO = objAPIConsumerMasterFilter.currentPage,
                    p_PAGERECORD = objAPIConsumerMasterFilter.pageSize,
                    p_SORTCOLNAME = objAPIConsumerMasterFilter.sort,
                    p_SORTTYPE = objAPIConsumerMasterFilter.orderBy
                }, true);
            }
            catch { throw; }
        }
        public APIConsumerMaster GetAPIConsumerById(int Id)
        {
            try
            {

                return repo.Get(u => u.consumer_id == Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public APIConsumerMaster SaveAPIConsumer(APIConsumerMaster objApiConsumer, int userId)
        {
            try
            {
                var ApiConsumer = repo.Get(x => x.consumer_id == objApiConsumer.consumer_id);
                if (ApiConsumer != null)
                {
                    ApiConsumer.user_name = objApiConsumer.user_name;
                    ApiConsumer.password = objApiConsumer.password;
                    ApiConsumer.source = objApiConsumer.source;
                    ApiConsumer.is_active = objApiConsumer.is_active;
                    ApiConsumer.is_log_required = objApiConsumer.is_log_required;
                    ApiConsumer.modified_by = userId;
                    ApiConsumer.modified_on = DateTimeHelper.Now;
                    return repo.Update(ApiConsumer);
                }

                else
                {
                    objApiConsumer.created_by = userId;
                    objApiConsumer.created_on = DateTimeHelper.Now;
                    return repo.Insert(objApiConsumer);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
