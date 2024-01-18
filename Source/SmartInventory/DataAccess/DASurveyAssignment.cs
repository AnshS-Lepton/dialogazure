using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DASurveyAssignment : Repository<object>
    {
        public List<SurveyAssignment> GetSurveyAssignmetDetail(CommonFilterAttribute objFilterAttributes)
        {

            try
            {
                return repo.ExecuteProcedure<SurveyAssignment>("fn_get_surveyarea_assigned", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_userid = objFilterAttributes.userid,
                    p_grouptype = objFilterAttributes.role_id,
                    p_dduser = objFilterAttributes.dd_user,
                    p_status = objFilterAttributes.dd_surveyStatus,
                    p_searchfrom = objFilterAttributes.searchFrom,
                    p_searchto = objFilterAttributes.searchTo
                }, true);
            }
            catch { throw; }

        }
        public List<SurveyUser> GetSurveyUser(int userid, int userGroup)
        {
            try
            {

                return repo.ExecuteProcedure<SurveyUser>("fn_get_surveyuser", new
                       {
                           p_userid = userid,
                           p_usergroup = userGroup

                       }, true);
            }
            catch { throw; }
        }
        public List<SurveyUser> GetSurveyTrackingUser(int userid, int userGroup)
        {
            try
            { 

                return repo.ExecuteProcedure<SurveyUser>("fn_get_survey_tracking_user", new
                {
                    p_userid = userid,
                    p_usergroup = userGroup

                }, true);
            }
            catch { throw; }
        }

        public List<SurveyUser> GetMultiUserAssignValue(int userid, int system_id)
        {

            try
            {

                return repo.ExecuteProcedure<SurveyUser>("fn_get_multiuser_assignment", new
                {
                    p_user_id = userid,
                    p_system_id = system_id

                }, true);
            }
            catch { throw; }
        }

        public List<SurveyAssignedDate> GetSurveyAssignedUser(int systemid)
        {

            try
            {

                return repo.ExecuteProcedure<SurveyAssignedDate>("fn_get_survey_assigned_user", new
                {
                    p_system_id = systemid

                }, true);
            }
            catch { throw; }
        }
        public List<SurveyBuilding> GetSurveyBuildingDetail(CommonBuildingFilterAttribute objFilterAttributes)
        {

            try
            {
                return repo.ExecuteProcedure<SurveyBuilding>("fn_get_survey_building_data", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    //is_active = objFilterAttributes.is_active,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_dduserid = objFilterAttributes.dd_user,
                    p_roleid = objFilterAttributes.roleid,
                    p_userid = objFilterAttributes.userid,
                    //p_grouptype = objFilterAttributes.group_type,                
                    p_ddstatus = objFilterAttributes.dd_BuildingStatus,
                    p_searchfrom = objFilterAttributes.searchFrom,
                    p_searchto = objFilterAttributes.searchTo
                }, true);
            }
            catch { throw; }

        }
        public List<CityList> GetBuildingCity(int userid)
        {

            try
            {

                return repo.ExecuteProcedure<CityList>("fn_get_bulk_building_city", new { p_userid = userid },
                 true);
            }
            catch { throw; }
        }
        public List<AreaList> GetBuildingArea(int province_id)
        {

            try
            {

                return repo.ExecuteProcedure<AreaList>("fn_get_bulk_building_Area", new { p_province_id = province_id },
                 true);
            }
            catch { throw; }
        }
        public List<BulkSurveyBuilding> GetBulkUpdateBuildingDetail(BulkFilterAttribute objFilterAttributes)
        {

            try
            {
                return repo.ExecuteProcedure<BulkSurveyBuilding>("fn_get_bulk_building_data", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    //is_active = objFilterAttributes.is_active,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_provinceid = objFilterAttributes.dd_city,
                    p_area = objFilterAttributes.dd_area
                }, true);
            }
            catch { throw; }

        }
        public List<locationTracking> GetSurveyLocationDetail(LocationAttribute objFilterAttributes)
        {

            try
            {
                return repo.ExecuteProcedure<locationTracking>("fn_get_survey_tracking_data", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_dduserid = objFilterAttributes.dd_user,
                    p_user_id = objFilterAttributes.userid,
                    p_role_id = objFilterAttributes.roleid,
                    p_searchfrom = objFilterAttributes.searchFrom,
                    p_searchto = objFilterAttributes.searchTo
                }, true);
            }
            catch { throw; }

        }
        public List<BuildingTracking> GetTrackBuildingDetail(TrackBuildingAttribute objFilterAttributes)
        {

            try
            {
                return repo.ExecuteProcedure<BuildingTracking>("fn_get_survey_tracked_building", new
                {
                    p_searchby = objFilterAttributes.searchBy,
                    p_searchtext = objFilterAttributes.searchText,
                    P_PAGENO = objFilterAttributes.currentPage,
                    P_PAGERECORD = objFilterAttributes.pageSize,
                    P_SORTCOLNAME = objFilterAttributes.sort,
                    P_SORTTYPE = objFilterAttributes.orderBy,
                    p_login_id = objFilterAttributes.loginid,
                    p_user_id = objFilterAttributes.user_id,
                    p_entity_type = objFilterAttributes.entity_type,
                    p_survey_id = objFilterAttributes.surveyarea_id,
                    p_roleid = objFilterAttributes.role_id,
                    p_loguser = objFilterAttributes.loguserId,
                    p_searchfrom = objFilterAttributes.searchFrom,
                    p_searchto = objFilterAttributes.searchTo
                }, true);
            }
            catch { throw; }

        }
        public List<UserTrackingLocation> GetCurrentTrackingLocation(int userid)
        {

            try
            {
                return repo.ExecuteProcedure<UserTrackingLocation>("fn_get_user_current_location", new
                {

                    p_user_id = userid

                }, true);
            }
            catch { throw; }

        }
        public bool UpdateSurveyBuildingStatus(string systemid, int roleid, string building_status, int userid, string comment)
        {

            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_update_bulkbuilding_status", new { p_building_system_ids = systemid, P_user_role = roleid, p_status = building_status, p_user_id = userid, p_comment = comment });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }
        public bool chkSurveyassignmentExportDataExist(int userid, int roleid)
        {

            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_chk_surveyassignment_export_exist", new { p_user_id = userid, p_roleid = roleid });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }

        public List<T> GetSurveyassignmentExportData<T>(int userid, int roleid) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_get_survey_assignment_export", new { p_user_id = userid, p_roleid = roleid }, true);
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }

        public string GetSurveyAreaStatus(int systemid)
        {

            try
            {
                var chk = repo.ExecuteProcedure<string>("fn_get_surveyarea_status", new { p_system_id = systemid });
                return chk[0];

            }

            catch { throw; }

        }
        public bool updateSurveyareaStatus(int system_id, string surveyareaStatus, int userid)
        {

            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_update_surveyarea_status", new { p_system_id = system_id, p_surveyarea_status = surveyareaStatus, p_userid = userid });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }
        public bool DeleteSurveyAssignmentUsers(int system_id, List<string> records)
        {
            var assignedUser = string.Join(",", records);
            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_delete_surveyarea_assignment", new { p_system_id = system_id, p_assigned_user = assignedUser });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }
        public List<surveyAssign> GetUserAssignment(int systemid)
        {

            try
            {

                return repo.ExecuteProcedure<surveyAssign>("fn_get_distinct_survey_user", new
                {
                    p_system_id = systemid

                }, true);
            }
            catch { throw; }
        }
        public List<UserLocationTracking> GetLocationTracking(int login_id)
        {

            try
            {
                return repo.ExecuteProcedure<UserLocationTracking>("fn_get_location_tracking", new { p_login_id = login_id }, true);
            }
            catch { throw; }

        }



    }
    public class DABulkSurveyAssignment : Repository<SurveyAreaAssigned>
    {
        public void BulkInsertUserAssignment(List<SurveyAreaAssigned> obj)
        {
            try
            {
                repo.Insert(obj);
            }
            catch { throw; }
        }

        public bool checkAreaAssigned(int systemId, int userId)
        {
            try
            {
                var result = repo.GetAll(m => m.assigned_to == userId && m.system_id == systemId).ToList();
                return result.Count > 0;
            }
            catch { throw; }
        }
    }




}
