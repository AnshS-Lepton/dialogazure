using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLJobPack
    {
        public JobPackMaster SaveJobPack(JobPackMaster objJobPack, int userId)
        {
            return new DAJobPack().SaveJobPack(objJobPack, userId);
        }
        public List<JobPackMaster> GetAssignedJobs(int userId,string Status)
        {
            return new DAJobPack().GetAssignedJobs(userId, Status);
        }
        public JobPackMaster getJobPackDetail(int systemId)
        {
            return new DAJobPack().getJobPackDetail(systemId);
        }

        public JobPackMaster UpdateJobPackStage(JobPackMaster objJobPack, int userId)
        {
            return new DAJobPack().UpdateJobPackStage(objJobPack, userId);
        }
        public List<JobContractUser> GetJobContractUser(int userid, int userGroup)
        {
            return new DAJobPack().GetJobContractUser(userid, userGroup);
        }
        public List<JobAssignment> GetJobAssignmetDetail(JobFilterAttribute objFilterAttributes)
        {
            return new DAJobPack().GetJobAssignmetDetail(objFilterAttributes);
        }
        public JobPackMaster assignJob(assignJob objJob)
        {
            return new DAJobPack().assignJob(objJob);
        }
        public bool deleteJob(int systemId)
        {
            return new DAJobPack().deleteJob(systemId);
        }
    }
}
