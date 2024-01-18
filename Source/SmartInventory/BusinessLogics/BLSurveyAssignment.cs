using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;
namespace BusinessLogics
{
    public class BLSurveyAssignment
    {
        public List<SurveyAssignment> GetSurveyAssignmetDetail(CommonFilterAttribute objFilterAttributes)
        {
            return new DASurveyAssignment().GetSurveyAssignmetDetail(objFilterAttributes);
        }
        public List<SurveyUser> GetSurveyUser(int userid,  int userGroup)
        {
            return new DASurveyAssignment().GetSurveyUser(userid,userGroup);
        }
        public List<SurveyUser> GetSurveyTrackingUser(int userid, int userGroup)
        {
            return new DASurveyAssignment().GetSurveyTrackingUser(userid, userGroup);
        }

        public List<SurveyUser> GetMultiUserAssignValue(int userid, int system_id)
        {
            return new DASurveyAssignment().GetMultiUserAssignValue(userid, system_id);
        }
        public List<SurveyAssignedDate> GetSurveyAssignedUser(int systemid)
        {
            return new DASurveyAssignment().GetSurveyAssignedUser(systemid);
        }
        public List<SurveyBuilding> GetSurveyBuildingDetail(CommonBuildingFilterAttribute objFilterAttributes)
        {
            return new DASurveyAssignment().GetSurveyBuildingDetail(objFilterAttributes);
        }
        public List<CityList> GetBuildingCity(int userid)
        {
            return new DASurveyAssignment().GetBuildingCity(userid);
        }
        public List<BulkSurveyBuilding> GetBulkUpdateBuildingDetail(BulkFilterAttribute objFilterAttributes)
        {
            return new DASurveyAssignment().GetBulkUpdateBuildingDetail(objFilterAttributes);
        }
        public List<AreaList> GetBuildingArea(int province_id)
        {
            return new DASurveyAssignment().GetBuildingArea(province_id);
        }
        public List<locationTracking> GetSurveyLocationDetail(LocationAttribute objFilterAttributes)
        {
            return new DASurveyAssignment().GetSurveyLocationDetail(objFilterAttributes);
        }
        public List<BuildingTracking> GetTrackBuildingDetail(TrackBuildingAttribute objFilterAttributes)
        {
            return new DASurveyAssignment().GetTrackBuildingDetail(objFilterAttributes);
        }
        public List<UserTrackingLocation> GetCurrentTrackingLocation(int userid)
        {
            return new DASurveyAssignment().GetCurrentTrackingLocation(userid);
        }
        public bool UpdateSurveyBuildingStatus(string systemid, int roleid, string building_status,int userid, string comment)
        {
            return new DASurveyAssignment().UpdateSurveyBuildingStatus(systemid, roleid, building_status, userid, comment);
        }
        public List<T> GetSurveyassignmentExportData<T>(int userid, int roleid) where T : new()
        {
            return new DASurveyAssignment().GetSurveyassignmentExportData<T>(userid, roleid);
        }
        public bool chkSurveyassignmentExportDataExist(int userid, int roleid)
        {
            return new DASurveyAssignment().chkSurveyassignmentExportDataExist(userid, roleid);
        }
        public string GetSurveyAreaStatus(int systemid)
        {
            return new DASurveyAssignment().GetSurveyAreaStatus(systemid);
        }
        public bool updateSurveyareaStatus(int system_id, string surveyareaStatus, int userid)
        {
            return new DASurveyAssignment().updateSurveyareaStatus(system_id, surveyareaStatus, userid);
        }
        public bool DeleteSurveyAssignmentUsers(int system_id, List<string> records)
        {
            return new DASurveyAssignment().DeleteSurveyAssignmentUsers(system_id, records);
        }
        public List<surveyAssign> GetUserAssignment(int systemid)
        {
            return new DASurveyAssignment().GetUserAssignment(systemid);
        }
        public List<UserLocationTracking> GetLocationTracking(int login_id)
        {
            return new DASurveyAssignment().GetLocationTracking(login_id);
        }

    }

    public class BLBulkUserAssignment
    {
        public void BulkInsertUserAssignment(List<SurveyAreaAssigned> obj)
        {
            new DABulkSurveyAssignment().BulkInsertUserAssignment(obj);
        }
    }
}
