using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mono.Security.X509.X520;

namespace DataAccess.Admin
{
    public class DAProject : Repository<ProjectMaster>
    {
        public ProjectMaster SaveEntityProjectDetails(ProjectMaster objAddProject)
        {   
            try
            {
                if (objAddProject.system_id != 0)
                {
                   
                    objAddProject.modified_by = objAddProject.user_id;
                    objAddProject.modified_on = DateTimeHelper.Now;
                    return repo.Update(objAddProject);

                }

                else
                {
                    objAddProject.created_by = objAddProject.user_id;
                    objAddProject.modified_by = objAddProject.user_id;
                    objAddProject.modified_on = DateTimeHelper.Now;

                    return repo.Insert(objAddProject);
                    
                }

            }
            catch { throw; }
        }


        public List<ViwProjectList> GetProjectDetailsList(ViewProjectDetailsList model)
        {
            try
            {
               
               string networkStageValue = Convert.ToString (model.viewProjectDetail.networkStage);
               string searchBy = Convert.ToString(model.viewProjectDetail.searchBy);
               string searchByText = Convert.ToString(model.viewProjectDetail.searchText);
               int pagno = model.viewProjectDetail.currentPage;
               int pagerecord = model.viewProjectDetail.pageSize;
               string sortcolumnname = model.viewProjectDetail.sort;
               string SortType = model.viewProjectDetail.orderBy;
               int  totalrecord = model.viewProjectDetail.totalRecord; 
               var recordlist = 0;
              
               var res = repo.ExecuteProcedure<ViwProjectList>("fn_get_projectdetails", new { networkstage = networkStageValue, searchby = searchBy, searchbyText = searchByText,
               P_PAGENO= pagno, P_PAGERECORD = pagerecord, P_SORTCOLNAME =sortcolumnname,  P_SORTTYPE = SortType, P_TOTALRECORDS =totalrecord, P_RECORDLIST=recordlist}, true);

                return res;
            }
            catch { throw; }
        }


        public ProjectMaster GetProjectDetailsByID(int id)
        {
            var objAddProject = repo.Get(m => m.system_id == id);

            return objAddProject;
           
        
        }


        public ProjectMaster UpdateProjectDetails(ProjectMaster objAddProject, int userId)
        {
            try
            {   
                objAddProject.modified_by = userId;
                objAddProject.modified_on = DateTimeHelper.Now;

                var resultItem = repo.Update(objAddProject);

                return resultItem;


            }
            catch { throw; }
        }


        //public int DeleteProjectDetailsById(int systemId)
        //{
        //    try
        //    {
        //        var objSystmId = repo.Get(x => x.system_id == systemId);
        //        if (objSystmId != null)
        //        {
        //            return repo.Delete(objSystmId.system_id);
        //        }
        //        else
        //        {
        //            return 0;
        //        }


        //    }
        //    catch { throw; }
        //}

        public List<KeyValueDropDown> BindProject(string network_stage)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_ProjectCode_list", new { network_stage = network_stage }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> BindRootId(int userdId,string geom, string selection_type, double buff_Radius, string networkStatus)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_rootId_list", 
                    new { p_userId = userdId, p_geom = geom, p_selectiontype= selection_type, p_radius= buff_Radius , p_network_status = networkStatus }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> BindPlanning(string network_stage, int ddlproject_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_planningcode_list", new { network_stage = network_stage, project_id = ddlproject_id }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> BindWorkorder(string network_stage, int ddlplanning_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_workordercode_list", new { network_stage = network_stage, planning_id = ddlplanning_id }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> BindPurpose(string network_stage, int ddlworkorder_id)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_purposecode_list", new { network_stage = network_stage, workorder_id = ddlworkorder_id }, true);

            }
            catch { throw; }

        }

    }



    public class DAProjectCode : Repository<ProjectCodeMaster>
    {
        public int SaveEntityProjectCodeDetails(ProjectCodeMaster objAddProjectCode)
        {
            try
            {
                if (objAddProjectCode.system_id != 0)
                {

                    objAddProjectCode.modified_by = objAddProjectCode.user_id;
                    objAddProjectCode.modified_on = DateTimeHelper.Now;

                    repo.Update(objAddProjectCode);

                    return 1;

                }

                else
                {
                    objAddProjectCode.created_by = objAddProjectCode.user_id;
                    objAddProjectCode.modified_by = objAddProjectCode.user_id;
                    objAddProjectCode.modified_on = DateTimeHelper.Now;
                    repo.Insert(objAddProjectCode);
                    return 0;

                }

            }
            catch { throw; }
        }
        public ProjectCodeMaster getProjectCodeDetailById(int id)
        {
            var objgetProjectCodeById = repo.Get(m => m.system_id == id);

            return objgetProjectCodeById;

        }
        public int DeleteProjectCodeDetailById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }

        public List<ProjectCodeMaster> getProjectCodeDetails(string network_stage)
        {
            return repo.GetAll(m => m.network_stage == network_stage ).ToList();
           // return new List<ProjectCodeMaster>();
         
        }

        public int IsCodeExistsForProjSpeci(string network_stage, string type, string code)
        {
            try
            {
              var getResult = repo.ExecuteProcedure<object>("fn_IsCodeExistsForProjSpeci", new { network_stage = network_stage, type = type, code = code }, true);

                if(getResult != null && getResult.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }

        }


    }


    public class DAPlanningCode : Repository<PlanningCodeMaster>
    {
        public PlanningCodeMaster SaveEntityPlanningCodeDetails(PlanningCodeMaster objAddPlanningCode)
        {
            try
            {
                if (objAddPlanningCode.system_id != 0)
                {

                    objAddPlanningCode.modified_by = objAddPlanningCode.user_id;
                    objAddPlanningCode.modified_on = DateTimeHelper.Now;

                    return repo.Update(objAddPlanningCode);

                    

                }

                else
                {
                    objAddPlanningCode.created_by = objAddPlanningCode.user_id;
                    objAddPlanningCode.modified_by = objAddPlanningCode.user_id;
                    objAddPlanningCode.modified_on = DateTimeHelper.Now;
                    return repo.Insert(objAddPlanningCode);
                  

                }

            }
            catch { throw; }
        }


        public PlanningCodeMaster getPlanningCodeDetailById(int id, int project_id=0)
        {
            var objgetPlanningCodeById = new PlanningCodeMaster();

            if(project_id !=0)
            {
                objgetPlanningCodeById = repo.Get(m => m.project_id == project_id);
            }
            
            else
            {
               objgetPlanningCodeById = repo.Get(m => m.system_id == id);
            }

            return objgetPlanningCodeById;
            
        }

        public List<PlanningCodeMaster> getPlanningDetailByIdList(int project_id=0)
        { 
           return repo.GetAll(m => m.project_id == project_id).ToList();
        }
        public List<PlanningCodeMaster> getPlanningDetailByProjectIds(List<int> project_id)
        {
            //return repo.GetAll(m => project_id.Contains(m.project_id.ToString())).ToList();
            return repo.GetAll().Where(p => project_id.Contains(p.project_id)).ToList();
        }
        

        public int DeletePlanningCodeDetailById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }
        
    }



    public class DAWorkorderCode : Repository<WorkorderCodeMaster>
    {
        public WorkorderCodeMaster SaveEntityWorkorderCodeDetails(WorkorderCodeMaster objAddWorkorderCode)
        {
            try
            {
                if (objAddWorkorderCode.system_id != 0)
                {

                    objAddWorkorderCode.modified_by = objAddWorkorderCode.user_id;
                    objAddWorkorderCode.modified_on = DateTimeHelper.Now;

                   return repo.Update(objAddWorkorderCode);

                   

                }

                else
                {
                    objAddWorkorderCode.created_by = objAddWorkorderCode.user_id;
                    objAddWorkorderCode.modified_by = objAddWorkorderCode.user_id;
                    objAddWorkorderCode.modified_on = DateTimeHelper.Now;
                    return repo.Insert(objAddWorkorderCode);
                   

                }

            }
            catch { throw; }
        }

        public WorkorderCodeMaster getWorkorderCodeDetailById(int id, int planning_id=0)
        {
            var objgetWorkorderCodeById = new WorkorderCodeMaster();

            if (planning_id != 0)
            {
                objgetWorkorderCodeById = repo.Get(m => m.planning_id == planning_id);
            }

            else
            {
                objgetWorkorderCodeById = repo.Get(m => m.system_id == id);
            }

            return objgetWorkorderCodeById;

        }


        public List<WorkorderCodeMaster> getWorkorderDetailByIdList(int planning_id = 0)
        {
         return repo.GetAll(m => m.planning_id == planning_id).ToList();
           
        }
        public List<WorkorderCodeMaster> getWorkorderDetailByPlanningIds(List<int> planning_ids)
        {
            return repo.GetAll(m => planning_ids.Contains(m.planning_id)).ToList();

        }

        public int DeleteWorkorderCodeDetailById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }

    }

    public class DAPurposeCode : Repository<PurposeCodeMaster>
    {
        public PurposeCodeMaster SaveEntityPurposeCodeDetails(PurposeCodeMaster objAddPurposeCode)
        {
            try
            {
                if (objAddPurposeCode.system_id != 0)
                {

                    objAddPurposeCode.modified_by = objAddPurposeCode.user_id;
                    objAddPurposeCode.modified_on = DateTimeHelper.Now;

                    return repo.Update(objAddPurposeCode);

                    

                }

                else
                {
                    objAddPurposeCode.created_by = objAddPurposeCode.user_id;
                    objAddPurposeCode.modified_by = objAddPurposeCode.user_id;
                    objAddPurposeCode.modified_on = DateTimeHelper.Now;
                    return repo.Insert(objAddPurposeCode);
                    
                }

            }
            catch { throw; }
        }

        public PurposeCodeMaster getPurposeCodeDetailById(int id, int workorder_id =0)
        {
            var objgetPurposeCodeById = new PurposeCodeMaster();

            if(workorder_id !=0)
            {
                objgetPurposeCodeById = repo.Get(m => m.workorder_id == workorder_id);
            }

            else
            {
                objgetPurposeCodeById = repo.Get(m => m.system_id == id);
            }


            return objgetPurposeCodeById;

        }

        public List<PurposeCodeMaster> getPurposeDetailByIdList(int workorder_id = 0)
        {  
           return repo.GetAll(m => m.workorder_id == workorder_id).ToList();
         
        }
        public List<PurposeCodeMaster> getPurposeDetailByWorkOrderIds(List<int> workorder_ids)
        {
            return repo.GetAll(m => workorder_ids.Contains(m.workorder_id)).ToList();

        }
        public int DeletePurposeCodeDetailById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
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

    public class DAToplologySegment : Repository<TopologySegmentMaster>
    {
        public List<TopologySegmentMaster> getSegmentDetailByIdList(int region_Id)
        {
            return repo.GetAll(m => m.RegionId == region_Id).ToList();
        }
    }
    public class DAToplologyRing : Repository<TopologyRingMaster>
    {
        public List<TopologyRingMaster> getRingDetailByIdList(int segment_Id)
        {
            return repo.GetAll(m => m.SegmentId == segment_Id).ToList();
        }
    }

    public class DAToplologyPlan : Repository<TopologyPlan>
    {
        public TopologyPlan SaveToploogyPlan(TopologyPlan objTopologyPlan)
        {
            var TopologyResp = repo.Insert(objTopologyPlan);
            return TopologyResp;
        }
    }

    public class DAPodMaster : Repository<PODMaster>
    {
        public List<PODMaster> getSiteIdList(string  siteId)
        {
            var sitname = repo.GetAll(m => m.site_id.ToUpper().Contains(siteId.ToUpper())).ToList();

            return sitname;
           
        }
        public List<PODMaster> getSiteNameList(string site_name)
        {
            var sitname = repo.GetAll(m => m.site_name.ToUpper().Contains(site_name.ToUpper())).ToList();

            return sitname;
        }
        
    }
}
