using DataAccess;
using Models;
using Models.API;
using Models.ISP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BusinessLogics
{
    public class BLBuilding
    {
        BLBuilding()
        {

        }
        private static BLBuilding objBuilding = null;
        private static readonly object lockObject = new object();
        public static BLBuilding Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBuilding == null)
                    {
                        objBuilding = new BLBuilding();
                    }
                }
                return objBuilding;
            }
        }
       
        public BuildingMaster GetBuildingByCode(string networkId)
        {
            return DABuilding.Instance.GetBuildingByCode(networkId);
        }
        public List<BuildingGeom> GetBuildingGeomInfo(string building_code, int system_id)
        {
            return DABuilding.Instance.GetBuildingGeomInfo(building_code,system_id);
        } 

        public List<InRegionProvince> GetRegionProvince(string geom, string enType)
        {
            return DABuilding.Instance.GetRegionProvince(geom, enType);
        }
        public List<InRegionProvince> GetRegionProvinceById(int region_id, int province_id)
        {
            return DABuilding.Instance.GetRegionProvinceById(region_id, province_id);
        }
        public List<InGeographicDetails> GetGeographicDetails(string geom, string geomtype)
        {
            return DABuilding.Instance.GetGeographicDetails(geom, geomtype);
        }
        public AutoCodification UpdateGeographicDetails(string entitytype, int systemid, string geomtype, int p_user_id)
        {
            return DABuilding.Instance.UpdateGeographicDetails(entitytype, systemid, geomtype, p_user_id);
        }

        public List<geocodfication_logs> getGeographicDetailsLogs(int systemid)
        {
            return DABuilding.Instance.getGeographicDetailsLogs(systemid);
        }
        public List<InRegionProvince> GetLBRegionProvince(string geom, string enType)
        {
            return DABuilding.Instance.GetLBRegionProvince(geom, enType);
        }
        public InSuraveyArea GetSurveyAreaExist(string geom, string enType)
        {
            return DABuilding.Instance.GetSurveyAreaExist(geom, enType).FirstOrDefault();
        }

        public BuildingMaster SaveBuilding(BuildingMaster buildingInfo, NetworkStatus status)
        {
            return DABuilding.Instance.SaveBuilding(buildingInfo, status);
        }

        public DbMessage ValidateStructureGeom(string txtGeom, int systemId)
        {
            return DABuilding.Instance.ValidateStructureGeom(txtGeom, systemId);
        }

        public int DeleteBuildingById(int buildingId)
        {
            return DABuilding.Instance.DeleteBuildingById(buildingId);
        }

        public DbMessage BldValidateByGeom(string enType, string txtGeom, int userid, int bldBufferMtr)
        {
            return DABuilding.Instance.BldValidateByGeom(enType, txtGeom, userid, bldBufferMtr);
        }

        public List<BuildingInfo> GetBulidingInfo(string networkId, string entityType, double SWLng, double SWLat, double NELng, double NELat, int userid)
        {
            return DABuilding.Instance.GetBulidingInfo(networkId, entityType, SWLng, SWLat, NELng, NELat, userid);
        }

        //public List<buildingDetail> GetBulidingDetailswithStructure(int systemId, int userId)
        //{
        //    return DABuilding.Instance.GetBulidingDetailswithStructure(systemId, userId);
        //}
        public bool checkAreaAssign(int systemid, int userid)
        {
            DABulkSurveyAssignment objAreaAssign = new DABulkSurveyAssignment();
            return objAreaAssign.checkAreaAssigned(systemid, userid);
        }
        public BuildingMaster UpdateBuildingGeom(double latitude, double longitude, int systemId,int userId)
        {
            return DABuilding.Instance.UpdateBuildingGeom(latitude, longitude, systemId, userId);
        }
        public BuildingMaster UpdateBuildingRFSType(string rfsType, int systemId, int userId)
        {
            return DABuilding.Instance.UpdateBuildingRFSType(rfsType, systemId, userId);
        }

        public List<BuildingInfo> getBuildingInfoByStatus(int userId, int surveyAreaId, string status, int building_system_id)
        {
            return DABuilding.Instance.getBuildingInfoByStatus(userId, surveyAreaId, status, building_system_id);
        }
        public List<BuildingInfo> getNearbybuilding(double latitude, double longitude, int buffer)
        {
            return DABuilding.Instance.getNearbybuilding(latitude, longitude, buffer);
        }
        public bool getbuildingatSameLocation(double latitude, double longitude)
        {
            return DABuilding.Instance.getbuildingatSameLocation(latitude, longitude);
        }
        public BuildingMaster IsBuildingCodeExists(string building_code, int userid = 0)
        {

            return DABuilding.Instance.IsBuildingCodeExists(building_code, userid);
        }

        public List<BuildingInfo> getBuildingInfoById(int userId, int surveyAreaId, string status, int building_system_id)
        {
            return DABuilding.Instance.getBuildingInfoById(userId, surveyAreaId, status, building_system_id); 
        }
        public BuildingSurveyDetails GetBuildingLocality(double lon, double lat)
        {
            return DABuilding.Instance.GetBuildingLocality(lon, lat);
        }
        #region Additional-Attributes
        public string GetOtherInfoBuilding(int systemId)
        {
            return DABuilding.Instance.GetOtherInfoBuilding(systemId);
        }
        #endregion
        public MobileSurveyArea GetGeomFromSubLocality(double longitude, double latitude)
        {
            return DABuilding.Instance.GetGeomFromSubLocality(longitude, latitude);
        }

    }

    public class BLTempBuilding
    {
        private static BLTempBuilding objBuilding = null;
        private static readonly object lockObject = new object();
        public static BLTempBuilding Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBuilding == null)
                    {
                        objBuilding = new BLTempBuilding();
                    }
                }
                return objBuilding;
            }
        }
        public void BulkUploadTempBuilding(List<TempBuildingMaster> BulkUploadTempBuilding)
        {
            DATempBuilding.Instance.BulkUploadTempBuilding(BulkUploadTempBuilding);
        }

        public DbMessage UploadBuildingForUpdate(int userID, List<String> Updatedfields)
        {
            return DATempBuilding.Instance.UploadBuildingForUpdate(userID, Updatedfields);
        }

        public DbMessage UploadBuildingForInsert(int userID)
        {
            return DATempBuilding.Instance.UploadBuildingForInsert(userID);
        }
        public List<TempBuildingMaster> GetUploadBuildingLogs(int userId)
        {
            return DATempBuilding.Instance.GetUploadBuildingLogs(userId);
        }

        public void DeleteTempBuildingData(int UserId)
        {
            DATempBuilding.Instance.DeleteTempBuildingData(UserId);
        }
    
        public Tuple<int, int> getTotalUploadBuildingfailureAndSuccess(int UserId)
        {
            return DATempBuilding.Instance.getTotalUploadBuildingfailureAndSuccess(UserId);
        }


    }
    public class BLBuildingRFS
    {
        public bool isValidRFSStatus(string oldRFS, string newRFS, string entityType)
        {
            return new DABuildingRFS().isValidRFSStatus(oldRFS, newRFS, entityType);
        }
    }
    public class BLBuildingComment
    {
        public List<BuildingComments> getbuildingComments(int building_system_id)
        {
            return new DABuildingComment().getbuildingComments(building_system_id);
        }
        public BuildingComments insertBuildlingStatusComments(BuildingComments objBuildingComments)
        {
            return new DABuildingComment().insertBuildlingStatusComments(objBuildingComments);
        }
        public List<ExportBuildingComments> getBulkbuildingComments(string building_system_ids)
        {
            return new DABuildingComment().getBulkbuildingComments(building_system_ids); 
        }
    }



}
