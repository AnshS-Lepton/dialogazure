using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
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

        public List<TopologyGetSites> Bindtopologygetsites(int system_id, int ring_id,int segment_id,int distance, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<TopologyGetSites>("fn_topology_get_sites", new { p_system_id = system_id, p_ring_id = ring_id, p_segment_id = segment_id, p_distance = distance, p_user_id =user_id }, false);

            }
            catch { throw; }

        }
        public List<Topologysegment> getSegmentDetailsRoutewise(int system_id,int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<Topologysegment>("fn_topology_get_segmentdetailssitewise", new { p_system_id = system_id, p_user_id = user_id }, false);

            }
            catch { throw; }

        }
        public List<TopologyGetSites> Bindtopologygetsitessitedissociation(int basesystem_id, int system_id, int ring_id, int distance, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<TopologyGetSites>("fn_topology_sites_dissociation", new { p_base_site_id= basesystem_id, p_system_id = system_id, p_ring_id = ring_id, p_distance = distance, p_user_id = user_id }, false);

            }
            catch { throw; }

        }
        public List<TopologySegmentCables> Gettopologysegmentcables(int agg1_system_id, int agg2_system_id, int user_id)
        {
            try
            {
                // Execute stored procedure and get list
                var segmentCables = repo.ExecuteProcedure<TopologySegmentCables>(
                    "fn_topology_get_segment_cables",
                    new
                    {
                        p_agg1_site_id = agg1_system_id,
                        p_agg2_site_id = agg2_system_id,
                        p_user_id = user_id
                    },
                    false
                ).ToList();

                return segmentCables; // Return the retrieved list (optional)
            }
            catch (Exception ex)
            {
                // Log the error message (optional)
                Console.WriteLine($"Error in Gettopologysegmentcables: {ex.Message}");
                throw;
            }
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

        public void Savetopsegmentcablemapping(int Agg1SystemId, int Agg2SystemId, int userId,int segment_id, int route_id)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_insert_top_segment_cable_mapping", new
                    {
                        p_agg1_system_id = Agg1SystemId,
                        p_agg2_system_id = Agg2SystemId,
                    p_route_id = route_id,
                    p_user_id = userId,
                       p_segment_id=segment_id

                }, false);
                

            }

            catch { throw; }

        }
        public void Savetopsegmentringcablemapping(int Agg1SystemId, int Agg2SystemId, int userId, int ringId,int segmentId,string top_type,int system_id)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_insert_top_segment_ring_cable_mapping", new
                {
                    p_agg1_system_id = Agg1SystemId,
                    p_agg2_system_id = Agg2SystemId,
                    p_user_id = userId,
                    p_ring_id = ringId,
                    p_segment_id = segmentId,
                    p_top_type= top_type,
                    p_system_id = system_id

                }, false);


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
    public class DAToplologyRegion : Repository<TopologyRegionMaster>
    {
    public List<TopologyRegionMaster> getTopologyRegionDetails()
    {
        return repo.GetAll().ToList();
        // return new List<ProjectCodeMaster>();
    }
  }
    public class DAToplologySegment : Repository<TopologySegment>
    {
        public List<TopologySegment> getSegmentDetailByIdList(int id, string aggregate1, string aggregate2)
        {
            if (aggregate1 != "" && aggregate1 != null && aggregate2 !="" && aggregate2 != null)
            return repo.GetAll(m => m.region_id == id && m.agg1_site_id== aggregate1 && m.agg2_site_id== aggregate2).ToList();
            else
                return repo.GetAll(m => m.region_id == id).ToList();
        }
        public TopologySegment GetSegmentCode()
        {
            TopologySegment objTopologyPlan = new TopologySegment();
            int maxSequence = repo.GetAll().Max(m => (int?)m.sequence) ?? 0;
            int newSequence = maxSequence + 1;

            // Get the latest segment_code and increment
            string lastSegmentCode = repo.GetAll()
                .OrderByDescending(m => m.sequence)
                .Select(m => m.segment_code)
                .FirstOrDefault();

            string newSegmentCode = GenerateNextSegmentCode(lastSegmentCode);
            objTopologyPlan.sequence = newSequence;
            objTopologyPlan.segment_code = newSegmentCode;
            return objTopologyPlan;
        }
        private string GenerateNextSegmentCode(string lastSegmentCode)
        {
            if (string.IsNullOrEmpty(lastSegmentCode) || !lastSegmentCode.StartsWith("ACC"))
            {
                return "ACC01"; // Default value if no valid segment code exists
            }

            // Extract the number part (e.g., "ACC08" -> "08")
            string numberPart = lastSegmentCode.Substring(3);

            // Convert to integer and increment
            if (int.TryParse(numberPart, out int numericValue))
            {
                return $"ACC{(numericValue + 1):D2}"; // Ensures two-digit format (e.g., ACC09, ACC10)
            }

            return "ACC01"; // Fallback in case of parsing error
        }

    }
    public class DAToplologyRing : Repository<TopologyRingMaster>
    {

       
        public List<ringinfo> getRingDetailByIdList(int segment_Id, int numberofsites,string ringcapacity)
        {
            var res = repo.ExecuteProcedure<ringinfo>("fn_get_ring_details", new
            {
                p_segment_id = segment_Id,
                p_numberofsites = numberofsites,
                p_ringcapacity = ringcapacity
            }, false);
            return res;

            //return repo.GetAll(m => m.segment_id == segment_Id).ToList();
        }
        public List<TopologyRingMaster> getRingCodeDetailByIdList(int segment_Id)
        {
            return repo.GetAll(m => m.segment_id == segment_Id).ToList();

        }
        public TopologyRingMaster GetRingCode(int ring, int segmentid)
        {
            TopologyRingMaster objTopologyPlan = new TopologyRingMaster();
           // int segment_Id = repo.GetAll().Where(a=>a.id== ring).Select(a=>a.segment_id).FirstOrDefault();
            var SequenceNum=  1 + repo.GetAll()
            .Where(r => r.ring_code != null && r.segment_id== segmentid)
            .Select(r => new
            {
                ring_code = r.ring_code,
                RingNumber = int.Parse(Regex.Match(r.ring_code, @"\d+$").Value) // Extract the last integer
            })
            .OrderByDescending(r => r.RingNumber)
            .Select(r => r.RingNumber)
            .FirstOrDefault();
           
            string ringCode = "";
            ringCode = $"R{SequenceNum}";
            objTopologyPlan.ring_code = ringCode;
            objTopologyPlan.sequence = SequenceNum;
            objTopologyPlan.segment_id = segmentid;
            return objTopologyPlan;





            //int maxSegmentId = repo.GetAll().Max(m => (int?)m.segment_id) ?? 0;
            //string lastRingCode = repo.GetAll()
            //                          .Where(r => r.segment_id == maxSegmentId)
            //                          .OrderByDescending(r => r.sequence)
            //                          .Select(r => r.ring_code)
            //                          .FirstOrDefault();

            //int lastSequence = 0;
            //if (!string.IsNullOrEmpty(lastRingCode))
            //{
            //    var match = System.Text.RegularExpressions.Regex.Match(lastRingCode, @"R(\d+)$");
            //    if (match.Success)
            //    {

            //        lastSequence = int.Parse(match.Groups[1].Value);
            //    }        
            //}
            //if (ring == 1)
            //    lastSequence = 0;
            
            //int newSequence = lastSequence + 1;
            //string ringCode = "";
          
            //if (maxSegmentId == 0)
            //{
            //     ringCode = $"R{newSequence}";
            //}
            //else { 
            //    ringCode = $"R{newSequence}"; 
            //}
              
            //objTopologyPlan.ring_code = ringCode;
            //objTopologyPlan.sequence = newSequence;
            //objTopologyPlan.segment_id = maxSegmentId;

            //return objTopologyPlan;
        }


        public TopologyRingMaster GetRingCode1()
        {
            TopologyRingMaster objTopologyPlan = new TopologyRingMaster();
            int ringCounter = 0;
            ringCounter++; // Increment the counter
            string ringCode = "R" + ringCounter; // Generate new ring code

            objTopologyPlan.ring_code = ringCode;
            objTopologyPlan.sequence = ringCounter; // Assign sequence

            return objTopologyPlan;
        }
        

        public TopologyRingMaster SaveRing(TopologyRingMaster topologyRingMaster)
        {
            
            int maxSequence = repo.GetAll().Max(m => (int?)m.sequence) ?? 0;
            int newSequence = maxSequence + 1;
            topologyRingMaster.sequence = newSequence;
            // Insert the new segment into the database
            var topologyResp = repo.Insert(topologyRingMaster);
            return topologyResp;
        }

    }

    //public class DAToplologyPlan : Repository<TopologyPlan>
    //{
    //    //public TopologyPlan SaveToploogyPlan(TopologyPlan objTopologyPlan)
    //    //{
    //    //    var TopologyResp = repo.Insert(objTopologyPlan);
    //    //    return TopologyResp;
    //    //}
    //}
    public class DASegment : Repository<TopologySegment>
    {
        public List<TopologySegment> GetSegment(TopologySegment topologySegment)
        {
            if (topologySegment == null || string.IsNullOrEmpty(topologySegment.agg1_site_id) || string.IsNullOrEmpty(topologySegment.agg2_site_id))
            {
                return new List<TopologySegment>(); // Return empty list instead of null
            }

            //return repo.GetAll(m => m.agg1_site_id != null && m.agg2_site_id != null && m.agg1_site_id.ToUpper() == topologySegment.agg1_site_id.ToString().ToUpper() && m.agg2_site_id.ToUpper() == topologySegment.agg2_site_id.ToUpper()).ToList();
            return repo.GetAll(m =>  m.agg1_site_id.ToUpper() == topologySegment.agg1_site_id.ToString().ToUpper() && m.agg2_site_id.ToUpper() == topologySegment.agg2_site_id.ToString().ToUpper()).ToList();

        }

        public vmRingConnectedElementDetails getRouteConnectedElementDetail(int route_id, int user_id)
        {
            try
            {

                return repo.ExecuteProcedure<vmRingConnectedElementDetails>("fn_get_route_connectedelement_details", new
                {
                    p_route_id = route_id,
                    p_user_id = user_id
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<routeDetails> GetCableRoute(TopologySegment topologySegment,int user_id)
        {
            if (topologySegment == null || string.IsNullOrEmpty(topologySegment.agg1_site_id) || string.IsNullOrEmpty(topologySegment.agg2_site_id))
            {
                return new List<routeDetails>(); // Return empty list instead of null
            }

            var sites = repo.ExecuteProcedure<routeDetails>(
                 "fn_topology_get_cableroute",
                 new
                 {
                     p_agg1site_id = Convert.ToInt32(topologySegment.agg1_site_id),
                     p_agg2site_id = Convert.ToInt32(topologySegment.agg2_site_id),
                     p_user_id = user_id
                 },
                 false
             ).ToList();
            return sites;

        }

        public TopologySegment SaveSegment(TopologySegment objTopologyPlan)
        {
            if (objTopologyPlan.id != 0)
            {

            

                return repo.Update(objTopologyPlan);



            }

            else
            {
                int maxSequence = repo.GetAll().Max(m => (int?)m.sequence) ?? 0;
                int newSequence = maxSequence + 1;
                objTopologyPlan.sequence = newSequence;
                // Insert the new segment into the database
                var topologyResp = repo.Insert(objTopologyPlan);
                return topologyResp;

            }

            
            
        }



    }

    public class DAPodMaster : Repository<PODMaster>
    {
        public List<PODMaster> getSiteIdList(string  siteId)
        {
            var sitname = repo.GetAll(m => m.site_id.ToUpper().Contains(siteId.ToUpper()) && m.is_agg_site != true).ToList();

            return sitname;
           
        }
        public List<PODMaster> getSiteIdName(int systemid)
        {
            var siteList = repo.GetAll(m => m.system_id == systemid)
                               .Select(m => new PODMaster
                               {
                                   site_id = m.site_id,
                                   site_name = m.site_name
                               })
                               .ToList();

            return siteList;
        }
        public List<SiteMaster> getSiteDetails(int siteid, int user_id)
        {
            var sites = repo.ExecuteProcedure<SiteMaster>(
                "fn_topology_get_siteDetails",
                new
                {
                    p_site_id = siteid,
                    p_user_id = user_id
                },
                false
            ).ToList();
            return sites;
          
        }

        public List<segmentMaster> getExistingSegmentDetails(int regionId, int agg1_site_id, int agg2_site_id, string route, int user_id)
        {
            var sites = repo.ExecuteProcedure<segmentMaster>(
                "fn_topology_get_existingsegmentdetails",
                new
                {
                    p_regionId = regionId,
                    p_agg1_site_id = agg1_site_id,
                    p_agg2_site_id = agg2_site_id,
                    p_route= route,
                    p_user_id = user_id
                },
                false
            ).ToList();
            return sites;

        }


        public List<PODMaster> getSiteNameList(string site_name)
        {
            var sitname = repo.GetAll(m => m.site_name.ToUpper().Contains(site_name.ToUpper())).ToList();

            return sitname;
        }

        public List<SiteMaster> getAllAGGListRoutewise(int siteid, int user_id,string site_name)
        {
            // Execute stored procedure and get list
            var sitname = repo.ExecuteProcedure<SiteMaster>(
                "fn_get_agg_sitedetail_routewise",
                new
                {
                    p_site_id = siteid,
                    p_user_id = user_id,
                      p_site_name = site_name
                },
                false
            ).ToList();
            return sitname;
        }

        public List<PODMaster> getAGG1List(string site)
        {

            var sitname = repo.GetAll(m =>
                (m.site_id.ToString().Contains(site.ToUpper()) ||
                m.site_name.ToUpper().Contains(site.ToUpper())) &&
                m.is_agg_site == true
            ).ToList();

            return sitname;
        }
        public List<PODMaster> getAGG2List(string site)
        {
            // var sitname = repo.GetAll(m => m.agg_02.ToUpper().Contains(agg2.ToUpper())).ToList();
            var sitname = repo.GetAll(m =>
                  (m.site_id.ToString().Contains(site.ToUpper()) ||
                  m.site_name.ToUpper().Contains(site.ToUpper())) &&
                  m.is_agg_site == true
              ).ToList();
            return sitname;
        }

        public PODMaster updatetopology(PODMaster objPODMaster)
        {
            // Retrieve the existing record

            var agg1SystemId = repo.Get(x => x.agg_01 == objPODMaster.agg_01)?.system_id;
            var agg2SystemId = repo.Get(x => x.agg_02 == objPODMaster.agg_02)?.system_id;

            var objPOD = repo.Get(x => x.system_id == objPODMaster.system_id);
            var objPODring = repo.Get(x => x.system_id == objPODMaster.system_id && x.ring_id == objPODMaster.ring_id);
            if(objPODring !=null)
            {
                PODMaster ObjPODMaster = new PODMaster();
                ObjPODMaster.ring_id = objPODring.ring_id;
              return ObjPODMaster;
            }

            if (objPOD == null)
            {
                throw new Exception("Record not found");
            }

            // Update properties with new values (only update if new values are provided)
            objPOD.ring_site_id = objPODMaster.ring_site_id;
            objPOD.ring_site_seq = objPODMaster.ring_site_seq;
            objPOD.site_id = objPODMaster.site_id;
            //objPOD.site_name = objPODMaster.site_name;
            objPOD.agg_01 = objPODMaster.agg_01; 
            objPOD.agg_02 = objPODMaster.agg_02; 
            objPOD.ring_a_site_id = objPODMaster.ring_a_site_id;
            objPOD.ring_b_site_id = objPODMaster.ring_b_site_id;
            objPOD.ring_a_site_distance = objPODMaster.ring_a_site_distance;
            objPOD.ring_b_site_distance = objPODMaster.ring_b_site_distance;
            objPOD.no_of_sites = objPODMaster.no_of_sites;
            objPOD.max_distance_peer = objPODMaster.max_distance_peer;
            objPOD.top_type = objPODMaster.top_type;
            objPOD.ring_id = objPODMaster.ring_id;

            objPOD.ring_site_seq = objPODMaster.ring_site_seq;

            // Save changes
            var TopologyResp = repo.Update(objPOD);

            return TopologyResp;
        }

    }

    public class DASegmentCableMapping : Repository<TopSegmentCableMapping>
    {
        public TopSegmentCableMapping updateSegmentCableMappings(TopSegmentCableMapping topSegmentCableMapping)
        {
           
            

            var resultItem = repo.Update(topSegmentCableMapping);

            return resultItem;
        }
    }
}
