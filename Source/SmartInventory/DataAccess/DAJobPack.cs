using System.Collections.Generic;
using System.Linq;
using Models;
using DataAccess.DBHelpers;
using System;

namespace DataAccess
{

    public class DAJobPack : Repository<JobPackMaster>
    {
        public JobPackMaster SaveJobPack(JobPackMaster objJobPack, int userId)
        {
            try
            {
                var objJobItem = repo.Get(x => x.system_id == objJobPack.system_id);
                if (objJobItem != null)
                {
                    objJobItem.job_type = objJobPack.job_type;
                    objJobItem.job_description = objJobPack.job_description;
                    objJobItem.nims_no = objJobPack.nims_no;
                    objJobItem.ngwfmt_project_id = objJobPack.ngwfmt_project_id;
                    objJobItem.node1 = objJobPack.node1;
                    objJobItem.node2 = objJobPack.node2;
                    objJobItem.highway_authority = objJobPack.highway_authority;
                    objJobItem.latitude = objJobPack.latitude;
                    objJobItem.longitude = objJobPack.longitude;
                    objJobItem.gis_address = objJobPack.gis_address;
                    objJobItem.manual_address = objJobPack.manual_address;
                    objJobItem.planner_contact_no = objJobPack.planner_contact_no;
                    objJobItem.planner_tel_no = objJobPack.planner_tel_no;
                    objJobItem.planner_email = objJobPack.planner_email;
                    objJobItem.delivery_contact_no = objJobPack.delivery_contact_no;
                    objJobItem.delivery_tel_no = objJobPack.delivery_tel_no;
                    objJobItem.remarks = objJobPack.remarks;
                    objJobItem.work_instructions = objJobPack.work_instructions;
                    objJobItem.delivery_email = objJobPack.delivery_email;
                    objJobItem.modified_by = userId;
                    objJobItem.modified_on = DateTimeHelper.Now;
                    return repo.Update(objJobItem);
                }
                else
                {
                    objJobPack.created_by = userId;
                    objJobPack.created_on = DateTimeHelper.Now;
                    objJobPack.status = "New";
                    return repo.Insert(objJobPack);
                }
            }
            catch { throw; }
        }
        public List<JobPackMaster> GetAssignedJobs(int userId, string Status)
        {
            if (string.IsNullOrEmpty(Status))
            {
                return repo.GetAll(m => m.assigned_to == userId).OrderByDescending(p=>p.created_on).ToList();
            }
            else
            {
                return repo.GetAll(m => m.assigned_to == userId && m.status.ToUpper().Trim()==Status.ToUpper().Trim()).OrderByDescending(p => p.created_on).ToList();
            }
        }

        public JobPackMaster getJobPackDetail(int systemId)
        {
            return repo.Get(m => m.system_id == systemId);
        }


        public JobPackMaster UpdateJobPackStage(JobPackMaster objJobPack, int userId)
        {
            try
            {
                objJobPack.modified_by = userId;
                objJobPack.modified_on = DateTimeHelper.Now;
                return repo.Update(objJobPack);
            }
            catch { throw; }
        }
        public List<JobContractUser> GetJobContractUser(int userid, int userGroup)
        {
            try
            {

                return repo.ExecuteProcedure<JobContractUser>("fn_get_jobcontract_user", new
                {
                    p_userid = userid,
                    p_usergroup = userGroup

                }, true);
            }
            catch { throw; }
        }
        public List<JobAssignment> GetJobAssignmetDetail(JobFilterAttribute objFilterAttributes)
        {

            try
            {
                return repo.ExecuteProcedure<JobAssignment>("fn_get_job_assigned", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_userid = objFilterAttributes.userid,
                    p_grouptype = objFilterAttributes.role_id,
                    p_dduser = objFilterAttributes.JobContractUser,
                    p_status = objFilterAttributes.JobStatus,
                    p_searchfrom = objFilterAttributes.searchFrom,
                    p_searchto = objFilterAttributes.searchTo
                }, true);
            }
            catch { throw; }

        }
        public JobPackMaster assignJob(assignJob objJob)
        {
            try
            {
                var jobDetails = repo.Get(m => m.system_id == objJob.systemId);
                if (jobDetails != null)
                {
                    jobDetails.status = "Assigned";
                    jobDetails.assigned_to = objJob.assignedTo;
                    jobDetails.assigned_on = DateTimeHelper.Now;
                    jobDetails.assigned_by = objJob.assignedBy;
                    repo.Update(jobDetails);
                }
                return jobDetails;
            }
            catch { throw; }

        }
        public bool deleteJob(int systemId)
        {
            try
            {
                var jobDetails = repo.Get(m => m.system_id == systemId);
                if (jobDetails != null)
                {
                    repo.Delete(jobDetails);
                }
                return true;
            }
            catch { throw; }
        }
    }
}
