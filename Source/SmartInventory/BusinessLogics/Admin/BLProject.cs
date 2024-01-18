using DataAccess.Admin;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLProject
    {
        public ProjectMaster SaveEntityProjectDetails(ProjectMaster objAddProject)
        {
            return new DAProject().SaveEntityProjectDetails(objAddProject);
        }


        public int SaveEntityProjectCodeDetails(ProjectCodeMaster objAddProjectCode)
        {
            return new DAProjectCode().SaveEntityProjectCodeDetails(objAddProjectCode);
        }

        public PlanningCodeMaster SaveEntityPlanningCodeDetails(PlanningCodeMaster objAddPlanningCode)
        {
            return new DAPlanningCode().SaveEntityPlanningCodeDetails(objAddPlanningCode);
        }

        public WorkorderCodeMaster SaveEntityWorkorderCodeDetails(WorkorderCodeMaster objAddWorkorderCode)
        {
            return new DAWorkorderCode().SaveEntityWorkorderCodeDetails(objAddWorkorderCode);
        }

        public PurposeCodeMaster SaveEntityPurposeCodeDetails(PurposeCodeMaster objAddPurposeCode)
        {
            return new DAPurposeCode().SaveEntityPurposeCodeDetails(objAddPurposeCode);
        }


        public IList<ViwProjectList> GetProjectDetailsList(ViewProjectDetailsList model)
        {
            return new DAProject().GetProjectDetailsList(model);
        }


        public ProjectMaster GetProjectDetailsByID(int id)
        {
            return new DAProject().GetProjectDetailsByID(id); 
        }

      

        //public int DeleteProjectById(int system_id)
        //{

        //    return new DAProject().DeleteProjectDetailsById(system_id);
        //}


        public int DeleteProjectCodeDetailById(int system_id)
        {
            return new DAProjectCode().DeleteProjectCodeDetailById(system_id);
            
        }


        public int DeletePlanningCodeDetailById(int system_id)
        {
            return new DAPlanningCode().DeletePlanningCodeDetailById(system_id);

        }


        public int DeleteWorkorderCodeDetailById(int system_id)
        {
            return new DAWorkorderCode().DeleteWorkorderCodeDetailById(system_id);

        }



        public int DeletePurposeCodeDetailById(int system_id)
        {
            return new DAPurposeCode().DeletePurposeCodeDetailById(system_id);

        }
        public List<KeyValueDropDown> BindProject(string network_stage)
        {

            return new DAProject().BindProject(network_stage);

        }

        public List<KeyValueDropDown> BindPlanning(string network_stage, int ddlproject_id)
        {

            return new DAProject().BindPlanning(network_stage, ddlproject_id);

        }

        public List<KeyValueDropDown> BindWorkorder(string network_stage, int ddlplanning_id)
        {

            return new DAProject().BindWorkorder(network_stage, ddlplanning_id);

        }

        public List<KeyValueDropDown> BindPurpose(string network_stage, int ddlworkorder_id)
        {

            return new DAProject().BindPurpose(network_stage, ddlworkorder_id);

        }
    

        public ProjectCodeMaster getProjectCodeDetailById(int id)
        {
            return new DAProjectCode().getProjectCodeDetailById(id);
        }

        public List<ProjectCodeMaster> getProjectCodeDetails(string network_stage)
        {
            return new DAProjectCode().getProjectCodeDetails(network_stage);
        }

        public int IsCodeExistsForProjSpeci(string network_stage, string type, string code)
        {
            return new DAProjectCode().IsCodeExistsForProjSpeci(network_stage, type, code);
        }



        public PlanningCodeMaster getPlanningCodeDetailById(int id, int project_id=0)
        {
            return new DAPlanningCode().getPlanningCodeDetailById(id, project_id);
        }


        public List<PlanningCodeMaster> getPlanningDetailByIdList(int project_id=0)
        {
            return new DAPlanningCode().getPlanningDetailByIdList(project_id);
        }

        public List<PlanningCodeMaster> getPlanningDetailByProjectIds(List<int> project_ids)
        {
            return new DAPlanningCode().getPlanningDetailByProjectIds(project_ids);
        }
        public WorkorderCodeMaster getWorkorderCodeDetailById(int id, int planning_id=0)
        {
            return new DAWorkorderCode().getWorkorderCodeDetailById(id, planning_id);
        }

        public List<WorkorderCodeMaster> getWorkorderDetailByIdList(int planning_id=0)
        {
            return new DAWorkorderCode().getWorkorderDetailByIdList(planning_id);
        }
        public List<WorkorderCodeMaster> getWorkorderDetailByPlanningIds(List<int> planning_ids)
        {
            return new DAWorkorderCode().getWorkorderDetailByPlanningIds(planning_ids);
        }
        public PurposeCodeMaster getPurposeCodeDetailById(int id, int workorder_id=0)
        {
            return new DAPurposeCode().getPurposeCodeDetailById(id, workorder_id);
        }

        public List<PurposeCodeMaster> getPurposeDetailByIdList(int workorder_id = 0)
        {
            return new DAPurposeCode().getPurposeDetailByIdList(workorder_id);
        }
        public List<PurposeCodeMaster> getPurposeDetailByWorkOrderIds(List<int> workorder_ids)
        {
            return new DAPurposeCode().getPurposeDetailByWorkOrderIds(workorder_ids);
        }
    }
}
