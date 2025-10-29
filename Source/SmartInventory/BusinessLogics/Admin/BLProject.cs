using DataAccess;
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
        public List<KeyValueDropDown> BindRootId(int userdId,string geom,string selection_type, double buff_Radius,string  networkStatus)
        {

            return new DAProject().BindRootId(userdId,geom, selection_type, buff_Radius, networkStatus);

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
        public List<TopologyGetSites> Bindtopologygetsites(int system_id, int ring_id,int segment_id, int distance, int user_id)
        {

            return new DAProject().Bindtopologygetsites( system_id, ring_id, segment_id, distance, user_id);

        }

        public List<Topologysegment> getSegmentDetailsRoutewise(int system_id, int user_id)
        {

            return new DAProject().getSegmentDetailsRoutewise(system_id,  user_id);

        }
        public List<TopologyGetSites> Bindtopologygetsitessitedissociation(int basesystem_id, int system_id, int ring_id, int distance, int user_id)
        {

            return new DAProject().Bindtopologygetsitessitedissociation(basesystem_id,system_id, ring_id, distance, user_id);

        }
        public List<TopologySegmentCables> Gettopologysegmentcables(int system_id, int ring_id, int user_id)
        {

            return new DAProject().Gettopologysegmentcables(system_id, ring_id, user_id);

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

        public void Savetopsegmentcablemapping(int Agg1SystemId, int Agg2SystemId, int userId,int segment_id, int route_id)
        {

            new DAProjectCode().Savetopsegmentcablemapping(Agg1SystemId, Agg2SystemId, userId, segment_id, route_id);
        }
        public void Savetopsegmentringcablemapping(int Agg1SystemId, int Agg2SystemId, int userId, int ringId, int segmentId,string top_type,int system_id)
        {

            new DAProjectCode().Savetopsegmentringcablemapping(Agg1SystemId, Agg2SystemId, userId, ringId,  segmentId, top_type, system_id);
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
        public List<TopologyRegionMaster> getTopologyRegionDetails()
        {
            return new DAToplologyRegion().getTopologyRegionDetails();
        }
        public List<TopologySegment> getSegmentDetailByIdList(int  id, string aggregate1="", string aggregate2="")
        {
            return new DAToplologySegment().getSegmentDetailByIdList(id, aggregate1, aggregate2);
        }
        public List<TopologyRingMaster> getRingCodeDetailByIdList(int segment_Id)
        {
            return new DAToplologyRing().getRingCodeDetailByIdList(segment_Id);
        }
        public TopologySegment GetSegmentCode()
        {
            return new DAToplologySegment().GetSegmentCode();
        }
        public TopologyRingMaster GetRingCode(int ring,int segmentid)
        {
            return new DAToplologyRing().GetRingCode(ring, segmentid);
        }
        public List<ringinfo> getRingDetailByIdList(int segment_Id = 0, int numberofsites = 0, string ringcapacity="")
        {
            return new DAToplologyRing().getRingDetailByIdList(segment_Id, numberofsites, ringcapacity);
        }
        public PODMaster updatetopology(PODMaster PODMaster)
        {
            return new DAPodMaster().updatetopology(PODMaster);
        }
        public List<TopologySegment> GetSegment(TopologySegment objTopologyPlan)
        {
            return new DASegment().GetSegment(objTopologyPlan);
        }
        public vmRingConnectedElementDetails getRouteConnectedElementDetail(int route_id, int user_id)
        {
            return new DASegment().getRouteConnectedElementDetail(route_id, user_id);
        }
        public GeometryDetail getNearestSiteDetail(int nearestsite_id, string geomType)
        {
            return new DASegment().getNearestSiteDetail(nearestsite_id, geomType);
        }

        public List<routeDetails> GetCableRoute(TopologySegment objTopologyPlan, int user_id)
        {
            return new DASegment().GetCableRoute(objTopologyPlan, user_id);
        }

        public List<bool> getValidRoute(string geom, int agg1, int agg2, int user_id)
        {
            return new DASegment().getValidRoute(geom, agg1, agg2, user_id);
        }
        public List<routeDetails> GetSelectedRoute(string geom, int agg1, int agg2, int user_id)
        {
            return new DASegment().GetSelectedRoute(geom, agg1, agg2, user_id);
        }
        public TopologySegment SaveSegment(TopologySegment objTopologyPlan)
        {
            return new DASegment().SaveSegment(objTopologyPlan);
        }
        public TopologyRingMaster SaveRing(TopologyRingMaster objTopologyPlan)
        {
            return new DAToplologyRing().SaveRing(objTopologyPlan);
        }
        public List<PODMaster> getSiteIdList(string site_id)
        {
            return new DAPodMaster().getSiteIdList(site_id);
        }
        public List<PODMaster> getSiteNameList(string site_name)
        {
            return new DAPodMaster().getSiteNameList(site_name);
        }
        public List<SiteMaster> getAllAGGListRoutewise(int siteid, int user_id, string site_name)
        {
            return new DAPodMaster().getAllAGGListRoutewise(siteid, user_id,site_name);
        }

        public List<PODMaster> getSiteList(string site_name,string siteType)
        {
            return new DAPodMaster().getSiteList(site_name, siteType);
        }

        public List<PODMaster> getAGG1List(string site_name)
        {
            return new DAPodMaster().getAGG1List(site_name);
        }
        public List<PODMaster> getAGG2List(string site_name)
        {
            return new DAPodMaster().getAGG2List(site_name);
        }
        public List<PODMaster> getSiteIdName(int systemId)
        {
            return new DAPodMaster().getSiteIdName(systemId);
        }
        public List<SiteMaster> getSiteDetails(int systemId, int user_id)
        {
            return new DAPodMaster().getSiteDetails(systemId, user_id);
        }
        public List<segmentMaster> getExistingSegmentDetails(int regionId, int agg1_site_id, int agg2_site_id,string route, int user_id)
        {
            return new DAPodMaster().getExistingSegmentDetails(regionId, agg1_site_id, agg2_site_id, route, user_id);
        }
        public List<PODMaster> updateSiteDetails(List<PODMaster> PODMaster)
        {
            return new DASiteDetails().UpdateSiteDetails(PODMaster);
        }
        public List<siteprojectdetails> SaveSiteProjectDetails(List<PODMaster> PODMaster,int userId)
        {
            return new DAProjectDetails().SaveSiteProjectDetails(PODMaster, userId);
        }
        public List<siteprojectdetails> GetProjectDetails(string site_id)
        {
            return new DAProjectDetails().GetProjectDetails(site_id);
        }
        public List<PROJECTDetails> GetProjectByDetails(string site_id)
        {
            return new DAProjectDetails().GetProjectByDetails(site_id);
        }
        public List<siteprojectdetails> GetProjectDetails()
        {
            return new DAProjectDetails().GetProjectDetails();
        }
        public siteprojectdetails GetProjectDetailsById(int id)
        {
            return new DAProjectDetails().GetProjectDetailsById(id);
        }

        public List<siteprojectdetails> GetProjectsuteDetailsById(int id)
        {
            return new DAProjectDetails().GetProjectSiteDetailsById(id);
        }
        public List<SiteBOMOBOQResponse> getSiteBomBoq(int site_id, double pole_span, double manhole_span, int userId)
        {
            
                return new DAProjectDetails().getSiteBomBoq(site_id, pole_span, manhole_span, userId);
        }
        public DbMessage updateSiteBomBoqAmount(int site_id, double amount, int pole_distance, int manhole_distance, int userId)
        {

            return new DAProjectDetails().updateSiteBomBoqAmount(site_id, amount, pole_distance, manhole_distance, userId);
        }

        public DbMessage UpdateSiteProject(siteprojectdetails siteprojectdetails, int userId)
        {
            return new DAProjectDetails().UpdateSiteProject(siteprojectdetails, userId);
        }
        public PODMaster UpdateSiteProjectAdditionDetails(PODMaster PODMaster)
        {
            return new DASiteDetails().UpdateSiteProjectAdditionDetails(PODMaster);
        }
        public DbMessage DeleteProjectById(int ticket_id, int userId)
        {
            return new DAProjectDetails().DeleteProjectById(ticket_id, userId);
        }
        public List<KeyValueDropDown> GetAllProjectwiseRegion()
        {
            return new DAProjectDetails().GetAllProjectwiseRegion();
        }
        public List<KeyValueDropDown> GetAllProvinceProjectwise(string region_id)
        {
            return new DAProjectDetails().GetAllProvinceProjectwise(region_id);
        }
       
        public List<Dictionary<string, object>> GetProjectwiseFiberDistanceReport(int region_id,int province_id)
        {
            return new DAProjectDetails().GetProjectwiseFiberDistanceReport(region_id, province_id);
        }
    }

    public class BLProjectExportReport
    {
        public List<ProjectwiseReportRequestLog> GetAllProvinceReport(int region_id)
        {
            return new DAProjectwiseExportReport().GetAllProvinceReport(region_id);
        }
        public ProjectwiseReportRequestLog SaveProjectwiseReportLog(ProjectwiseReportRequestLog projectwiseReportRequestLog)
        {
            return new DAProjectwiseExportReport().SaveProjectwiseReportLog(projectwiseReportRequestLog);
        }
        public ProjectwiseReportRequestLog GetprojectReportByFileName(string fileName, string status)
        {
            return new DAProjectwiseExportReport().GetProjectwiseReportByFileName(fileName, status);
        }
        public ProjectwiseReportRequestLog GetBOQReportByFileName(string fileName)
        {
            return new DAProjectwiseExportReport().GetBOQReportByFileName(fileName);
        }
    }
}
