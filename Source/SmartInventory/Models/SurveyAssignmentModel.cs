using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SurveyAssignment:IReference
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string surveyarea_name { get; set; }
        public string surveyarea_status { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime ? due_date { get; set; }
        public string user_name { get; set; }
        public string remarks { get; set; }
        public int total_assigned_user { get; set; }
        public int total_building { get; set; }
        public int completed { get; set; }
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_user { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }

    }
    public class ViewSurveyAssignmentModel
    {
        public List<SurveyAssignment> lstSurveyAssignment { get; set; }
        public CommonFilterAttribute objFilterAttributes { get; set; }
    
        public IList<DropDownMaster> SurveyStatus { get; set; }
        public IList<SurveyUser> lstSurveyUser { get; set; }
        public List<UserReportDetail> lstUserReport { get; set; }
        public ViewSurveyAssignmentModel()
        {
         
   
            objFilterAttributes = new CommonFilterAttribute();
            objFilterAttributes.searchText = string.Empty;
            objFilterAttributes.searchFrom = null;
            objFilterAttributes.searchTo = null;
            // objFilterAttributes.dd_surveyStatus = "New";
            //objFilterAttributes.dd_user = 0;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }

    public class CommonFilterAttribute : CommonGridAttributes
    {
        public DateTime? searchFrom { get; set; }
        public DateTime? searchTo { get; set; }      
        public int userid { get; set; }
        public int  role_id { get; set; }
        public int dd_user { get; set; }
        public string dd_surveyStatus { get; set; }
    }
    public class SurveyUser
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string user_email { get; set; }
        public string assigned_to { get; set; }
        public string assigned_by { get; set; } 
        public string assigned_date { get; set; }
        public int manager_id { get; set; }
        public string manager_name { get; set; }
    }
    public class SurveyAreaUser
    {
        public List<SurveyUser> surveyuser { get; set; }
        public SurveyAreaUser()
        {
            surveyuser = new List<SurveyUser>();
        }
    }
    public class SurveyAssignedUser
    {
        public List<SurveyAssignedDate> assignDate { get; set; }
        public SurveyAssignedUser()
        {
            assignDate = new List<SurveyAssignedDate>();
        }
    }
    public class SurveyAssignedDate
    {
        public string assigned_to { get; set; }
        public string assigned_date { get; set; }
    }
    public class surveyAssign
    {
        public int assigned_to { get; set; }
    }

    public class SurveyBuilding
    { 
        public int building_id { get; set; }
        public string building_code { get; set; }
        public string building_name { get; set; }
        public string building_no { get; set; } 
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string address { get; set; } 
        public string building_status { get; set; }
        public int business_pass { get; set; }
        public int home_pass { get; set; }
        public string tenancy { get; set; }
        public DateTime created_on { get; set; } 
        public string category { get; set; }
        public string remarks { get; set; }
        public int totalRecords { get; set; }
        public int surveyarea_id { get; set; }
        public string surveyarea_name { get; set; }
        public string surveyarea_code { get; set; }
        public string user_name { get; set; } 
        public string buildingcreatedby { get; set; }
    }
    public class ViewSurveyBuildingModel
    {
        public List<SurveyBuilding> lstSurveyBuilding { get; set; }
        public CommonBuildingFilterAttribute objbuildingFilterAttributes { get; set; }

        public IList<DropDownMaster> BuildingStatus { get; set; }
        public IList<SurveyUser> lstSurveyUser { get; set; }

        public List<UserReportDetail> lstUserReport { get; set; }

        public ViewSurveyBuildingModel()
        {
            objbuildingFilterAttributes = new CommonBuildingFilterAttribute();
            objbuildingFilterAttributes.searchText = string.Empty;
            objbuildingFilterAttributes.searchFrom = null;
            objbuildingFilterAttributes.searchTo = null;
           // objbuildingFilterAttributes.dd_BuildingStatus = "New";
          //  objbuildingFilterAttributes.dd_user = 0;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }
     
    public class CommonBuildingFilterAttribute : CommonGridAttributes
    {
        public DateTime? searchFrom { get; set; }
        public DateTime? searchTo { get; set; }
        public int userid { get; set; }
        public int roleid { get; set; }
        public int dd_user { get; set; }
        public string dd_BuildingStatus { get; set; }
    }

    public class CityList
    {
        public int province_id { get; set; }
        public string province_name { get; set; }

    }
    public class AreaList
    {
        public string area_id { get; set; }
        public string area_name { get; set; }

    }
    public class UpdateBuildingCity
    {
        public List<CityList> lstUpdateCity { get; set; }
        public UpdateBuildingCity()
        {
            lstUpdateCity = new List<CityList>();
        }
    }
    public class UpdateBuildingArea
    {
        public List<AreaList> lstArearea { get; set; }
        public UpdateBuildingArea()
        {
            lstArearea = new List<AreaList>();
        }
    }
    public class BulkSurveyBuilding
    {
     
        public string surveyarea_name { get; set; }  
        public string building_code { get; set; }
        public string building_name { get; set; }
        public string address { get; set; }
        public string area { get; set; }
        public string pin_code { get; set; }
        public DateTime created_on { get; set; }
        public int business_pass { get; set; }
        public int home_pass { get; set; }
        public string building_type { get; set; }
        public string category { get; set; }
        public string tenancy { get; set; }
        public string rfs_status { get; set; }
        public DateTime? rfs_date { get; set; }
        public string media { get; set; }
        public string remarks { get; set; }
        public int totalRecords { get; set; }

        
    }
    public class ViewBulkSurveyBuildingModel
    {
        public List<BulkSurveyBuilding> lstBulkSurveyBuilding { get; set; }
        public BulkFilterAttribute objFilterAttributes { get; set; }
        public IList<CityList> lstCity { get; set; }
        public IList<AreaList> lstArea { get; set; }

        public ViewBulkSurveyBuildingModel()
        {
            objFilterAttributes = new BulkFilterAttribute();
            objFilterAttributes.searchText = string.Empty;
            objFilterAttributes.dd_city = 0;
            objFilterAttributes.dd_area = string.Empty;
            lstArea = new List<AreaList>();
           
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }
    public class BulkFilterAttribute : CommonGridAttributes
    {
               
        public int dd_city { get; set; }
        public string dd_area { get; set; }
       
    }

    // Location Tracking
    public class locationTracking
    {
        
        
        public string user_name { get; set; }
        public int user_id { get; set; }
        public int login_id { get; set; }
        public int totalRecords { get; set; }
        public string building_count { get; set; }
        public DateTime? tracking_date { get; set; }
        public DateTime? login_time { get; set; }
        public DateTime? logout_time { get; set; }

    }
    public class ViewlocationTrackingModel
    {
        public List<locationTracking> lstLocationTracking { get; set; }
        public LocationAttribute objFilterAttributes { get; set; }
        public IList<SurveyUser> lstSurveyUser { get; set; }
        public List<UserReportDetail> lstUserReport { get; set; }
        public ViewlocationTrackingModel()
        {
            objFilterAttributes = new LocationAttribute();
            objFilterAttributes.searchText = string.Empty;         
           

        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }
    public class LocationAttribute : CommonGridAttributes
    {
        public int dd_user { get; set; }
        public int userid { get; set; }
        public int roleid { get; set; }
        public DateTime? searchFrom { get; set; }
        public DateTime? searchTo { get; set; }
    }


    // Track Building
    public class BuildingTracking
    {
       
        public string building_code { get; set; }
        public string building_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string address { get; set; }
        public string building_status { get; set; }
        public DateTime? created_on { get; set; }
        public int totalRecords { get; set; }
        public string user_name { get; set; }
    }
    public class ViewBuildingTrackingModel
    {
        public List<BuildingTracking> lstBuildingTracking { get; set; }
        public TrackBuildingAttribute objFilterAttributes { get; set; }
     
        public ViewBuildingTrackingModel()
        {
            objFilterAttributes = new TrackBuildingAttribute();
            objFilterAttributes.searchText = string.Empty;


        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }
    public class TrackBuildingAttribute : CommonGridAttributes
    {
        public string entity_type { get; set; }
        public int user_id { get; set; }
        public int loguserId { get; set; }
        public int role_id { get; set; }
        public int loginid { get; set; }
        public DateTime? searchFrom { get; set; }
        public DateTime? searchTo { get; set; }
        public int surveyarea_id { get; set; }
    }
    public class UserTrackingLocation
    {
        public string track_date { get; set; }
        //public string trackingtime { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string tracked_on { get; set; }
       
    }
    public class UserCurrentLocation
    {
        public List<UserTrackingLocation> lstUserLocation { get; set; }
        public UserCurrentLocation()
        {
            lstUserLocation = new List<UserTrackingLocation>();
        }
    }
    public class SurveyAreaAssignmentExport
    {
        public string region_name { get; set; }
        public string province_name { get; set; }
        public string surveyarea_name { get; set; }
        public string network_id { get; set; }
        public DateTime? created_on { get; set; }
        public string assigned_to { get; set; }
        public DateTime? assigned_date { get; set; }
        public DateTime? due_date { get; set; }
        public string status { get; set; }
        public string assigned_by { get; set; }
    }
    public class SurveyAreaAssigned
    {   [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int system_id { get; set; }
        public int assigned_to { get; set; }
        public int assigned_by { get; set; }
        public DateTime? assigned_date { get; set; }
        public DateTime? due_date { get; set; }
  
    }
    public class ModelSurveyAreaUser
    {
        public List<SurveyUser> surveyuser { get; set; }
        public string network_code { get; set; }
        public string surveyarea_name { get; set; }
        public string due_date { get; set; }
        public int MaxUsersAssign { get; set; }
        public ModelSurveyAreaUser()
        {
            surveyuser = new List<SurveyUser>();
        }
    }
    //public class SurveyRoute
    //{
    //    public string latitude { get; set; }
    //    public string longitude { get; set; }
    //    //public string tracking_time { get; set; }
    //    public string entity_type { get; set; }
    //    public int entity_system_id { get; set; }
    //    public string login_time { get; set; }
    //    public string logout_time { get; set; }
    //    public string showWayPoint { get; set; }
    //    public string entity_name { get; set; }
    //    public string entity_network_id { get; set; }
    //    public string entity_creation_date { get; set; }
    //    public string server_time { get; set; }
    //    public string mobile_time { get; set; }
    //    public string building_status { get; set; }
    //}
    //public class UserSurveyRoute
    //{
    //    public List<SurveyRoute> lstSurveyRoute { get; set; }
    //    public UserSurveyRoute()
    //    {
    //        lstSurveyRoute = new List<SurveyRoute>();
    //    }
    //}
  
}
