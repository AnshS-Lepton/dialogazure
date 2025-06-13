using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;
using NPOI.SS.Formula.PTG;
using Utility;

namespace DataAccess
{
    public class DAArea : Repository<Area>
    {
        public Area SaveArea(Area objArea, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objArea.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objArea.modified_on, objitem.modified_on, objArea.modified_by, objitem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objArea.objPM = objPageValidate;
                        return objArea;
                    }
                    objitem.area_name = objArea.area_name;
                    objitem.remarks = objArea.remarks;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.area_rfs = objArea.area_rfs;
                    objitem.status_remark = objArea.status_remark;
                    objitem.requested_by = objArea.requested_by;
                    objitem.request_approved_by = objArea.request_approved_by;
                    objitem.request_ref_id = objArea.request_ref_id;
                    objitem.origin_ref_id = objArea.origin_ref_id;
                    objitem.origin_ref_description = objArea.origin_ref_description;
                    objitem.origin_from = objArea.origin_from;
                    objitem.origin_ref_code = objArea.origin_ref_code;
                    objitem.no_of_home_pass = objArea.no_of_home_pass;
                    objitem.gis_design_id = objArea.gis_design_id;
                    var AreaResp = repo.Update(objitem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(AreaResp.system_id,Models.EntityType.Area.ToString(), AreaResp.province_id,1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata( Models.EntityType.Area.ToString(), AreaResp.province_id);
                    return AreaResp;
                }
                else
                {
                    objArea.created_by = userId;
                    objArea.created_on = DateTimeHelper.Now;
                    objArea.status = (string.IsNullOrEmpty(objArea.status) ? "A" : objArea.status);
                    var resultItem = repo.Insert(objArea);
                    // Save geometry
                   
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.Area.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Area.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Area.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public int DeleteAreaById(int systemId)
        {
            try
            {
                var objArea = repo.Get(x => x.system_id == systemId);
                if (objArea != null)
                {
                    return repo.Delete(objArea.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
      
    }

    public class DASubArea : Repository<SubArea>
    {
        public List<SubAreaIn> GetAreaExist(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<SubAreaIn>("fn_get_area_exist", new { p_geometry = geom, p_geomtype = GeometryType.Polygon.ToString() });
            }
            catch { throw; }
        }

        public int DeleteSubAreaById(int systemId)
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


        public SubArea UpdateSubAreaBuildingCode(SubArea objSubArea, int userId)
        {
            try
            {
                SubArea objResult = new SubArea();
                var objitem = repo.Get(x => x.system_id == objSubArea.system_id);
                if (objitem != null)
                {
                    //if (objSubArea.subarea_rfs != "A-RFS")
                    if (objSubArea.subarea_rfs == "C-RFS" || string.IsNullOrEmpty(objSubArea.subarea_rfs))
                    {
                        objitem.building_code = null;
                        var deleteChk = DABuilding.Instance.DeleteBuildingById(objSubArea.building_system_id);
                        objitem.building_system_id = 0;
                    }
                    else
                    {
                        objitem.building_code = objSubArea.building_code;
                        objitem.building_system_id = objSubArea.building_system_id;
                    }
                    objResult = repo.Update(objitem);
                }
                return objResult;
            }
            catch { throw; }
        }

        public SubArea SaveSubArea(SubArea objSubArea, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objSubArea.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objSubArea.modified_on, objitem.modified_on, objSubArea.modified_by, objitem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objSubArea.objPM = objPageValidate;
                        return objSubArea;
                    }
                    objitem.subarea_name = objSubArea.subarea_name;
                    objitem.remarks = objSubArea.remarks;
                    objitem.subarea_rfs = objSubArea.subarea_rfs;
                    //objitem.building_system_id = objSubArea.building_system_id;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    //if (objSubArea.subarea_rfs!="A-RFS")
                    if (objSubArea.subarea_rfs == "C-RFS" || string.IsNullOrEmpty(objSubArea.subarea_rfs))
                    {
                        objitem.building_code = null;
                        var deleteChk = DABuilding.Instance.DeleteBuildingById(objSubArea.building_system_id);
                        objitem.building_system_id = 0;
                    }
                    else
                    {
                        objitem.building_code = objSubArea.building_code;
                        objitem.building_system_id = objSubArea.building_system_id;
                    }
                    objitem.status_remark = objSubArea.status_remark;
                    objitem.requested_by = objSubArea.requested_by;
                    objitem.request_approved_by = objSubArea.request_approved_by;
                    objitem.request_ref_id = objSubArea.request_ref_id;
                    objitem.origin_ref_id = objSubArea.origin_ref_id;
                    objitem.origin_ref_description = objSubArea.origin_ref_description;
                    objitem.origin_from = objSubArea.origin_from;
                    objitem.origin_ref_code = objSubArea.origin_ref_code;
                    objitem.no_of_home_pass = objSubArea.no_of_home_pass;
                    var resultItem= repo.Update(objitem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.SubArea.ToString(), resultItem.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.SubArea.ToString(), resultItem.province_id);
                    return resultItem;
                }
                else
                {
                    objSubArea.created_by = userId;
                    objSubArea.created_on = DateTimeHelper.Now;
                    objSubArea.building_code = objSubArea.building_code;
                    objSubArea.status = (string.IsNullOrEmpty(objSubArea.status) ? "A" : objSubArea.status);
                    var resultItem = repo.Insert(objSubArea);
                    
                    //TRANSACTION NEED TO IMPLEMENT THERE...
                    //Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.SubArea.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.SubArea.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.SubArea.ToString(), resultItem.province_id);
                    return resultItem;

                }
            }
            catch { throw; }
        }
       
    }

    public class DASurveyArea : Repository<SurveyArea>
    {

        public SurveyArea SaveSurveyArea(SurveyArea objsurveyArea, int userId)
        {
            try
            {
                var objItem = repo.Get(x => x.system_id == objsurveyArea.system_id);
                if (objItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objsurveyArea.modified_on, objItem.modified_on, objsurveyArea.modified_by, objItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objsurveyArea.objPM = objPageValidate;
                        return objsurveyArea;
                    }
                    objItem.surveyarea_name = objsurveyArea.surveyarea_name;
                    objItem.remarks = objsurveyArea.remarks;
                    objItem.modified_by = userId;
                    objItem.modified_on = DateTimeHelper.Now;
                    objItem.due_date = objsurveyArea.due_date;
                    objItem.status_remark = objsurveyArea.status_remark;
                    objItem.requested_by = objsurveyArea.requested_by;
                    objItem.request_approved_by = objsurveyArea.request_approved_by;
                    objItem.request_ref_id = objsurveyArea.request_ref_id;
                    objItem.origin_ref_id = objsurveyArea.origin_ref_id;
                    objItem.origin_ref_description = objsurveyArea.origin_ref_description;
                    objItem.origin_from = objsurveyArea.origin_from;
                    objItem.origin_ref_code = objsurveyArea.origin_ref_code;
                    var SAResp = repo.Update(objItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(SAResp.system_id, Models.EntityType.SurveyArea.ToString(), SAResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.SurveyArea.ToString(), SAResp.province_id);
                    return SAResp;
                }
                else
                {
                    objsurveyArea.created_by = userId;
                    objsurveyArea.created_on = DateTimeHelper.Now;
                    objsurveyArea.status = (string.IsNullOrEmpty(objsurveyArea.status) ? "A" : objsurveyArea.status);
                    var resultItem = repo.Insert(objsurveyArea);
                    //Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.SurveyArea.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.SurveyArea.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.SurveyArea.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteSurveyAreaById(int systemId)
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
        public SurveyArea getSurveyAreaById(int systemId)
        {
            try
            {
                return repo.Get(u => u.system_id == systemId);
            }
            catch
            {
                throw;
            }
        }

        public void SaveMobileSurveyAreaAssigned(int user_id, int systemId)
        {
            try
            {
                repo.ExecuteProcedure<object>("fn_save_surveyarea_assigned_mobile", new { p_user_id = user_id, p_system_id = systemId });
            }
            catch { throw; }
        }


    }
    public class DArestricted_area : Repository<RestrictedArea>
    {

        public RestrictedArea SaveRestrictedArea(RestrictedArea restricted_Area, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == restricted_Area.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(restricted_Area.modified_on, restricted_Area.modified_on, restricted_Area.modified_by, restricted_Area.modified_by);
                    if (objPageValidate.message != null)
                    {
                        restricted_Area.objPM = objPageValidate;
                        return restricted_Area;
                    }
                    objitem.restricted_area_name = restricted_Area.restricted_area_name;
                    objitem.remarks = restricted_Area.remarks;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    // objitem.area_rfs = restricted_Area.area_rfs;
                    objitem.status = restricted_Area.status;
                    objitem.network_status = restricted_Area.network_status;
                    objitem.requested_by = restricted_Area.requested_by;
                    objitem.request_approved_by = restricted_Area.request_approved_by;
                    objitem.request_ref_id = restricted_Area.request_ref_id;
                    objitem.origin_ref_id = restricted_Area.origin_ref_id;
                    objitem.origin_ref_description = restricted_Area.origin_ref_description;
                    objitem.origin_from = restricted_Area.origin_from;
                    objitem.origin_ref_code = restricted_Area.origin_ref_code;
                    objitem.qualification_type = restricted_Area.qualification_type;
                    objitem.category = restricted_Area.category;
                    objitem.sub_category = restricted_Area.sub_category;
                    objitem.is_network_creation_allowed = restricted_Area.is_network_creation_allowed;
                    objitem.is_feasibility_allowed = restricted_Area.is_feasibility_allowed;
                    objitem.allowed_network= restricted_Area.allowed_network;
                    var RAreaResp = repo.Update(objitem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(RAreaResp.system_id, Models.EntityType.RestrictedArea.ToString(), RAreaResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.RestrictedArea.ToString(), RAreaResp.province_id);
                    return RAreaResp;
                }
                else
                {
                    restricted_Area.created_by = userId;
                    restricted_Area.created_on = DateTimeHelper.Now;
                    restricted_Area.status = (string.IsNullOrEmpty(restricted_Area.status) ? "A" : restricted_Area.status);
                    restricted_Area.network_status = restricted_Area.status;
                    var resultItem = repo.Insert(restricted_Area);
                    
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.RestrictedArea.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.RestrictedArea.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.RestrictedArea.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteRestrictedAreaById(int systemId)
        {
            try
            {
                var objArea = repo.Get(x => x.system_id == systemId);
                if (objArea != null)
                {
                    return repo.Delete(objArea.system_id);
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
