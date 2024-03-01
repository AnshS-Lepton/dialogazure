using DataAccess.DBHelpers;
using Models;
using Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DABuilding : Repository<BuildingMaster>
    {
        DABuilding()
        {

        }
        private static DABuilding objBuilding = null;
        private static readonly object lockObject = new object();
        public static DABuilding Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBuilding == null)
                    {
                        objBuilding = new DABuilding();
                    }
                }
                return objBuilding;
            }
        }



        public BuildingMaster GetBuildingByCode(string networkId)
        {
            try
            {
                return repo.Get(m => m.network_id.ToUpper() == networkId.ToUpper());
                // return objBuildingDetail != null ? objBuildingDetail : new BuildingMaster();
            }
            catch { throw; }
        }
        public List<InRegionProvince> GetRegionProvince(string geom, string enType)
        {
            try
            {
                return repo.ExecuteProcedure<InRegionProvince>("fn_getregionprovince", new { geometry = geom, entitytype = enType });
            }
            catch { throw; }
        }
        public List<InRegionProvince> GetRegionProvinceById(int region_id, int province_id)
        {
            try
            {
                return repo.ExecuteProcedure<InRegionProvince>("fn_getregionprovincebyid", new { p_region_id = region_id, p_province_id = province_id });
            }
            catch { throw; }
        }
        public List<InGeographicDetails> GetGeographicDetails(string geom, string geomtype)
        {
            try
            {
                return repo.ExecuteProcedure<InGeographicDetails>("fn_getgeographicdetails", new { geometry = geom, geomtype = geomtype });
            }
            catch { throw; }
        }
        public AutoCodification UpdateGeographicDetails(string entitytype, int systemid, string geomtype,int p_user_id)
        {
            try
            {
                //return repo.ExecuteProcedure<DbMessage>("fn_updategeographicdetails", new { p_entitytype = entitytype, p_geomtype = geomtype, p_systemid = systemid }).FirstOrDefault();
                return repo.ExecuteProcedure<AutoCodification>("fn_auto_codification", new { P_SYSTEM_ID = systemid, P_ENTITY_TYPE = entitytype,P_userid= p_user_id }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<geocodfication_logs> getGeographicDetailsLogs(int systemid)
        {
            try
            {
                return repo.ExecuteProcedure<geocodfication_logs>("fn_get_geographicdetailslogs", new { p_fsa_id = systemid });
            }
            catch { throw; }
        }
        public List<InRegionProvince> GetLBRegionProvince(string geom, string enType)
        {
            try
            {
                return repo.ExecuteProcedure<InRegionProvince>("fn_landbase_getregionprovince", new { geometry = geom, entitytype = enType });
            }
            catch { throw; }
        }
        public List<BuildingGeom> GetBuildingGeomInfo(string building_code, int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<BuildingGeom>("fn_api_get_building_goem", new { p_building_code = building_code, p_system_id = system_id }, true);
            }
            catch { throw; }
        }


        public List<InSuraveyArea> GetSurveyAreaExist(string geom, string enType)
        {
            try
            {
                return repo.ExecuteProcedure<InSuraveyArea>("fn_get_surveyarea", new { geometry = geom, entitytype = enType });
            }
            catch { throw; }
        }

        public BuildingMaster SaveBuilding(BuildingMaster buildingInfo, NetworkStatus status)
        {
            try
            {
                // UPDATE LOGIC IS PENDING..
                var oldBuildingStatus = string.Empty;
                var objBuilding = repo.Get(u => u.system_id == buildingInfo.system_id);
                PageMessage objPageValidate = new PageMessage();
                if (objBuilding != null)
                {
                    oldBuildingStatus = objBuilding.building_status;
                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = buildingInfo.system_id,
                        entity_type = EntityType.Building.ToString(),
                        home_pass = buildingInfo.home_pass,
                        business_pass = buildingInfo.business_pass,
                        total_tower = buildingInfo.total_tower
                    }, buildingInfo.system_id == 0);
                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                        buildingInfo.pageMsg = objPageValidate;
                        return buildingInfo;
                    }

                    objPageValidate = DAUtility.ValidateModifiedDate(buildingInfo.modified_on, objBuilding.modified_on, buildingInfo.modified_by, objBuilding.modified_by);
                    if (objPageValidate.message != null)
                    {
                        buildingInfo.pageMsg = objPageValidate;
                        return buildingInfo;
                    }
                    if (buildingInfo.bldAction == BuildingAction.Update)
                    {
                        // mark surveyed only if existing status is new
                        if ((objBuilding.building_status == BuildingStatus.New.ToString()) || (objBuilding.building_status == BuildingStatus.Resurveyed.ToString()))
                        {
                            objBuilding.building_status = BuildingStatus.Surveyed.ToString();
                            objBuilding.status_updated_by = buildingInfo.userid;
                            objBuilding.status_updated_on = DateTimeHelper.Now;
                        }

                        //other update fileds...
                        //if (objBuilding.building_status == BuildingStatus.Ne)
                        objBuilding.building_height = buildingInfo.building_height;
                        objBuilding.building_name = buildingInfo.building_name;
                        objBuilding.building_no = buildingInfo.building_no;
                        objBuilding.address = buildingInfo.address;
                        objBuilding.area = buildingInfo.area;
                        objBuilding.pin_code = buildingInfo.pin_code;
                        objBuilding.tenancy = buildingInfo.tenancy;
                        objBuilding.category = buildingInfo.category;
                        objBuilding.home_pass = buildingInfo.home_pass;
                        objBuilding.business_pass = buildingInfo.business_pass;
                        objBuilding.no_of_floor = buildingInfo.no_of_floor;
                        objBuilding.no_of_flat = buildingInfo.no_of_flat;
                        objBuilding.media = buildingInfo.media;
                        objBuilding.pod_code = buildingInfo.pod_code;
                        objBuilding.pod_name = buildingInfo.pod_name;
                        objBuilding.rfs_status = buildingInfo.rfs_status;
                        objBuilding.rfs_date = buildingInfo.rfs_date;
                        objBuilding.rwa = buildingInfo.rwa;
                        objBuilding.building_type = buildingInfo.building_type;
                        objBuilding.remarks = buildingInfo.remarks;
                        objBuilding.surveyarea_id = buildingInfo.surveyarea_id;
                        objBuilding.total_tower = buildingInfo.total_tower;
                        objBuilding.rwa_contact_no = buildingInfo.rwa_contact_no;
                        objBuilding.subcategory = buildingInfo.subcategory;
                        // objBuilding.user_comments = buildingInfo.user_comments;
                    }
                    else if (buildingInfo.bldAction == BuildingAction.Reject)
                    {
                        // mark as resurvyed and assign to same sales user
                        objBuilding.building_status = BuildingStatus.Resurveyed.ToString();
                        objBuilding.status_updated_by = buildingInfo.userid;
                        objBuilding.status_updated_on = DateTimeHelper.Now;
                    }
                    else if (buildingInfo.bldAction == BuildingAction.Approve)
                    {
                        // mark status as approved
                        objBuilding.building_status = BuildingStatus.Approved.ToString();
                        objBuilding.status_updated_by = buildingInfo.userid;
                        objBuilding.status_updated_on = DateTimeHelper.Now;

                    }

                    objBuilding.modified_on = DateTimeHelper.Now;
                    objBuilding.modified_by = buildingInfo.userid;
                    objBuilding.other_info = buildingInfo.other_info;
                    objBuilding.status_remark = buildingInfo.status_remark;
                    objBuilding.requested_by = buildingInfo.requested_by;
                    objBuilding.request_approved_by = buildingInfo.request_approved_by;
                    objBuilding.request_ref_id = buildingInfo.request_ref_id;
                    objBuilding.origin_ref_id = buildingInfo.origin_ref_id;
                    objBuilding.origin_ref_description = buildingInfo.origin_ref_description;
                    objBuilding.origin_from = buildingInfo.origin_from;
                    objBuilding.origin_ref_code = buildingInfo.origin_ref_code;
                    objBuilding.locality = buildingInfo.locality;
                    objBuilding.sub_locality = buildingInfo.sub_locality;
                    objBuilding.road = buildingInfo.road;
                    var response = repo.Update(objBuilding);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.Building.ToString(), response.province_id, 1);
                   // DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Building.ToString(), response.province_id);
                    objBuilding.userid = buildingInfo.userid;
                    objBuilding.user_comments = buildingInfo.user_comments;
                    saveBuildingStatusComments(objBuilding, oldBuildingStatus);
                    return response;
                }
                else
                {
                    //buildingInfo.status = "A";
                    //buildingInfo.network_status = status.ToString();
                    buildingInfo.status = String.IsNullOrEmpty(buildingInfo.status) ? "A" : buildingInfo.status;
                    buildingInfo.network_status = String.IsNullOrEmpty(buildingInfo.network_status) ? status.ToString() : buildingInfo.network_status;

                    if ((!String.IsNullOrEmpty(buildingInfo.building_status)) && (buildingInfo.rfs_status == "A-RFS" || buildingInfo.rfs_status == "A-RFS Type1" || buildingInfo.rfs_status == "A-RFS Type2" || buildingInfo.IsbldApprovedStatus))
                    {
                        buildingInfo.building_status = buildingInfo.building_status;
                    }
                    else
                    {
                        buildingInfo.building_status = BuildingStatus.Surveyed.ToString();
                    }
                    /////// >>10Jan-2022
                    //buildingInfo.building_status = BuildingStatus.Surveyed.ToString();

                    buildingInfo.created_on = DateTimeHelper.Now;
                    buildingInfo.created_by = buildingInfo.userid;
                    if (String.IsNullOrEmpty(buildingInfo.rfs_status))
                    {
                        buildingInfo.rfs_status = "Non-RFS";
                    }
                    buildingInfo.status_updated_by = buildingInfo.userid;
                    buildingInfo.status_updated_on = DateTimeHelper.Now;
                    var response = repo.Insert(buildingInfo);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.Building.ToString(), response.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Building.ToString(), response.province_id);
                    saveBuildingStatusComments(buildingInfo);
                    return response;
                }
            }
            catch { throw; }
        }

        public void saveBuildingStatusComments(BuildingMaster buildingInfo, string oldBuildingStatus = "")
        {
            if ((oldBuildingStatus == buildingInfo.building_status && !string.IsNullOrWhiteSpace(buildingInfo.user_comments) || (oldBuildingStatus != buildingInfo.building_status)))
            {
                BuildingComments objBuildingComments = new BuildingComments();
                objBuildingComments.building_system_id = buildingInfo.system_id;
                objBuildingComments.comment = buildingInfo.user_comments;
                objBuildingComments.created_by = buildingInfo.userid;
                objBuildingComments.created_on = DateTimeHelper.Now;
                objBuildingComments.building_status = buildingInfo.building_status;
                var res = new DABuildingComment().insertBuildlingStatusComments(objBuildingComments);
            }

        }
        public BuildingMaster UpdateBuildingGeom(double latitude, double longitude, int systemId, int userId)
        {
            try
            {
                var objBuilding = repo.Get(u => u.system_id == systemId);
                if (objBuilding != null)
                {
                    objBuilding.latitude = latitude;
                    objBuilding.longitude = longitude;
                    objBuilding.modified_on = DateTimeHelper.Now;
                    objBuilding.modified_by = userId;
                    return repo.Update(objBuilding);
                }
                else return null;
            }
            catch { throw; }
        }
        public BuildingMaster UpdateBuildingRFSType(string rfsType, int systemId, int userId)
        {
            try
            {
                var objBuilding = repo.Get(u => u.system_id == systemId);
                if (objBuilding != null)
                {
                    objBuilding.rfs_status = rfsType;
                    objBuilding.modified_on = DateTimeHelper.Now;
                    objBuilding.modified_by = userId;
                    return repo.Update(objBuilding);
                }
                else return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<BuildingInfo> getBuildingInfoByStatus(int userId, int surveyAreaId, string status, int building_system_id)
        {
            try
            {
                return repo.ExecuteProcedure<BuildingInfo>("fn_get_buildinginfo_by_status", new { p_userid = userId, p_surveyAreaId = surveyAreaId, p_status = status, p_building_system_id = 0 }).ToList();
            }
            catch { throw; }
        }
        public List<BuildingInfo> getBuildingInfoById(int userId, int surveyAreaId, string status, int building_system_id)
        {
            try
            {
                return repo.ExecuteProcedure<BuildingInfo>("fn_get_buildinginfo_by_status", new { p_userid = userId, p_surveyAreaId = surveyAreaId, p_status = status, p_building_system_id = building_system_id }).ToList();
            }
            catch { throw; }
        }
        public List<BuildingInfo> getNearbybuilding(double latitude, double longitude, int buffer)
        {
            try
            {
                return repo.ExecuteProcedure<BuildingInfo>("fn_get_nearbybuilding", new { p_latitude = latitude, p_longitude = longitude, p_buffer = buffer }).ToList();
            }
            catch { throw; }
        }
        public bool getbuildingatSameLocation(double latitude, double longitude)
        {
            try
            {
                return repo.GetAll(m => m.latitude == latitude && m.longitude == longitude).ToList().Count > 0;
            }
            catch { throw; }
        }
        public DbMessage ValidateStructureGeom(string txtGeom, int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_get_structure_validate", new { p_building_id = systemId, p_geom = txtGeom, p_systemId = 0, p_geom_type = GeometryType.Point.ToString() }).FirstOrDefault();
            }
            catch
            {
                throw;
            }
        }

        public int DeleteBuildingById(int buildingId)
        {
            int result = 0;
            try
            {
                //var objBuilding = repo.Get(u => u.system_id == buildingId && u.network_status == "P");
                var objBuilding = repo.Get(u => u.system_id == buildingId);
                if (objBuilding != null)
                {
                    result = repo.Delete(objBuilding.system_id);
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public DbMessage BldValidateByGeom(string enType, string txtGeom, int userid, int bldBufferMtr)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_building_geom", new { p_geom_type = enType, p_longlat = txtGeom, p_userid = userid, buff_in_mtr = bldBufferMtr, p_system_id = 0 }).FirstOrDefault();

            }
            catch { throw; }
        }

        public List<BuildingInfo> GetBulidingInfo(string networkId, string entityType, double SWLng, double SWLat, double NELng, double NELat, int userid)
        {
            try
            {
                return repo.ExecuteProcedure<BuildingInfo>("fn_get_buildinginfo_by_bbox", new { p_network_id = networkId, p_entity_ype = entityType, p_userid = userid, p_SWLng = SWLng, p_SWLat = SWLat, p_NELng = NELng, p_NELat = NELat }).ToList();

            }
            catch { throw; }
        }

        //public List<buildingDetail> GetBulidingDetailswithStructure(int systemId, int userId)
        //{
        //    try
        //    {
        //        var result= repo.ExecuteProcedure<buildingDetail>("fn_get_buildingstructureinfo_by_systemid", new { p_systemid = systemId, p_userid = userId }, true);
        //        return result != null && result.Count > 0 ? result : new List<buildingDetail>();
        //    }
        //    catch { throw; }
        //}




        public BuildingMaster IsBuildingCodeExists(string building_code, int userId)
        {



            var objitem = repo.Get(x => x.network_id.ToLower().Trim() == building_code.ToLower().Trim());

            return objitem;


            //var objitem = repo.Get(x => x.network_id.ToLower().Trim() == building_code.ToLower().Trim() && x.building_status.ToLower().Trim() == "approved");
            //return objitem;
            //if (objitem != null)
            //{
            //    return true;
            //}

            //return false;

        }

        public BuildingMaster getBuildingLocality(string building_code, int userId)
        {



            var objitem = repo.Get(x => x.network_id.ToLower().Trim() == building_code.ToLower().Trim());

            return objitem;

        }
        public BuildingSurveyDetails GetBuildingLocality(double lon, double lat)
        {
            try
            {
                return repo.ExecuteProcedure<BuildingSurveyDetails>("fn_get_building_locality_sublocality", new { lon = lon, lat = lat }).FirstOrDefault();
            }
            catch { throw; }
        }
        #region Additional-Attributes
        public string GetOtherInfoBuilding(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
        public MobileSurveyArea GetGeomFromSubLocality(double longitude, double latitude)
        {
            try
            {
                return repo.ExecuteProcedure<MobileSurveyArea>("fn_get_sublocality_geom", new { p_longitude = longitude, p_latitude = latitude }).FirstOrDefault();
            }
            catch { throw; }
        }
    }
    public class DATempBuilding : Repository<TempBuildingMaster>
    {
        private static DATempBuilding objBuilding = null;
        private static readonly object lockObject = new object();
        public static DATempBuilding Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBuilding == null)
                    {
                        objBuilding = new DATempBuilding();
                    }
                }
                return objBuilding;
            }
        }
        public void BulkUploadTempBuilding(List<TempBuildingMaster> TempBuilding)
        {
            try
            {
                repo.Insert(TempBuilding);
            }
            catch { throw; }
        }

        public DbMessage UploadBuildingForUpdate(int userID, List<String> Updatedfields)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_building_Update", new { P_UserId = userID, p_Updatedfields = Updatedfields[0] }).FirstOrDefault();
            }
            catch { throw; }
        }


        public DbMessage UploadBuildingForInsert(int userID)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_bulk_upload_building_Insert", new { P_UserId = userID }).FirstOrDefault();
            }
            catch { throw; }
        }

        public List<TempBuildingMaster> GetUploadBuildingLogs(int UserId)
        {
            try
            {
                return repo.GetAll().Where(x => x.uploaded_by == UserId).ToList();
            }
            catch { throw; }
        }

        public void DeleteTempBuildingData(int UserId)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_temp_att_details_building", new { P_Userid = UserId });
                //List<TempBuildingMaster> model = repo.GetAll().Where(x => x.uploaded_by == UserId).ToList();
                //repo.DeleteRange(model);
            }
            catch { throw; }
        }



        public Tuple<int, int> getTotalUploadBuildingfailureAndSuccess(int UserId)
        {
            try
            {
                var getTotalUploadBuildingfailure = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == false).Count();
                var getTotalUploadBuildingSuccess = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == true).Count();
                return Tuple.Create(getTotalUploadBuildingSuccess, getTotalUploadBuildingfailure);
            }
            catch { throw; }
        }

    }

    public class DABuildingRFS : Repository<object>
    {
        public bool isValidRFSStatus(string oldRFS, string newRFS, string entityType)
        {
            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_validate_rfs_status", new { p_rfs_status_old = oldRFS, p_rfs_status_new = newRFS, p_entity_type = entityType });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;
            }

            catch { throw; }
        }
    }
    public class DABuildingComment : Repository<BuildingComments>
    {
        public List<BuildingComments> getbuildingComments(int building_system_id)
        {
            try
            {
                var result = repo.ExecuteProcedure<BuildingComments>("fn_get_building_comments", new { p_systemid = building_system_id }, true);
                return result != null && result.Count > 0 ? result : new List<BuildingComments>();
                //var result = repo.GetAll(x => x.building_system_id == building_system_id).OrderByDescending(s=>s.id).ToList();
                //return result != null ? result : new List<BuildingComments>();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public BuildingComments insertBuildlingStatusComments(BuildingComments objobjBuildingComments)
        {
            try
            {
                var response = repo.Insert(objobjBuildingComments);
                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<ExportBuildingComments> getBulkbuildingComments(string building_system_ids)
        {
            try
            {
                var result = repo.ExecuteProcedure<ExportBuildingComments>("fn_get_bulk_building_comments", new { p_systemids = building_system_ids }, true);
                return result != null && result.Count > 0 ? result : new List<ExportBuildingComments>();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
