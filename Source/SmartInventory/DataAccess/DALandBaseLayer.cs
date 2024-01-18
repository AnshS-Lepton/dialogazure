using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DALandBaseMaster : Repository<LandBaseMaster>
    {
        public List<LandBaseMaster> getlayerNamebyGeom(string geomType, bool is_buffer_enable)
        {
            try
            {               
                 var result = repo.GetAll(u => u.geom_type == geomType && u.is_active == true && u.is_center_line_enable==is_buffer_enable).ToList();
                return result != null ? result : new List<LandBaseMaster>();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<LandBaseMaster> getLandBaseLayers()
        {
            try
            {
                var result = repo.GetAll(u => u.is_active == true).OrderBy(x=>x.layer_name).ToList();
                return result != null ? result : new List<LandBaseMaster>();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<LandBaseMaster> getLandBaseLayersByName( string layerName)
        {
            try
            {
                var result = repo.GetAll(u => u.layer_name.ToUpper()== layerName.ToUpper() && u.is_active == true).ToList();
                return result != null ? result : new List<LandBaseMaster>();
            }
            catch (Exception)
            {

                throw;
            }
        } 
        public LandBaseMaster getLayerNamebyId(int layerId)
        {
            return repo.Get(x => x.id == layerId);
        }
    }
    public class DALandBaseLayer : Repository<LandBaseLayer>
    {
         public LandBaseLayer GetLandbaseEntityById(int id, string geomType)
        {
            try
            {
                var landbaseDetails = repo.ExecuteProcedure<LandBaseLayer>("fn_landbase_get_entitydetails_by_id", new { id = id }, true).FirstOrDefault();

                if (geomType == GeometryType.Point.ToString())
                {
                    var extent = landbaseDetails.sp_geometry.TrimStart("POINT(".ToCharArray()).TrimEnd(")".ToCharArray());
                    string[] lnglat = extent.Split(new string[] { " " }, StringSplitOptions.None);
                    landbaseDetails.latitude = Convert.ToDouble(lnglat[1].ToString());
                    landbaseDetails.longitude = Convert.ToDouble(lnglat[0].ToString());
                }
                return landbaseDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<LandBaseLayerSummaryReport> GetExportReportSummary(ExportLandBaseLayerReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<LandBaseLayerSummaryReport>("fn_landbase_get_export_report_summary",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                       // p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layerids = objReportFilter.SelectedLayerIds,
                      //  p_projectcodes = objReportFilter.SelectedProjectIds,
                      //  p_planningcodes = objReportFilter.SelectedPlanningIds,
                      //  p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                       // p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId,
                      //  p_ownership_type = objReportFilter.SelectedOwnerShipType,
                      //  p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds
                    }, false);
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetLandBaseExportReportSummaryView(ExportLandBaseLayerSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_landbase_get_export_report_summary_view",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds, 
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName, 
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_advancefilter = objReportFilter.advancefilter,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId 

                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetExportLandBaseLayerSummaryViewKML(ExportLandBaseLayerSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_landbase_get_export_report_summary_view_kml",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds, 
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName, 
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_advancefilter = objReportFilter.advancefilter,
                        p_filetype = objReportFilter.fileType,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId

                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public LandBaseLayer SaveLandBase(LandBaseLayer objLandBase, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.id == objLandBase.id);
                if (objitem != null)
                {
                    //PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objLandBase.modified_on, objitem.modified_on, objLandBase.modified_by, objitem.modified_by);
                    //if (objPageValidate.message != null)
                    //{
                    //    objLandBase.pageMsg = objPageValidate;
                    //    return objLandBase;
                    //}
                    //objitem.landbase_layer_id = objLandBase.landbase_layer_id;
                    objitem.name = objLandBase.name;
                    objitem.address = objLandBase.address;
                    objitem.category_id = objLandBase.category_id;
                    objitem.sub_category_id = objLandBase.sub_category_id;
                    objitem.classification_id = objLandBase.classification_id;
                   // objitem.address = objLandBase.address;
                    objitem.attribute_1 = objLandBase.attribute_1;
                    objitem.attribute_2 = objLandBase.attribute_2;
                    objitem.attribute_3 = objLandBase.attribute_3;
                    objitem.attribute_4 = objLandBase.attribute_4;
                    objitem.attribute_5 = objLandBase.attribute_5;
                    objitem.attribute_6 = objLandBase.attribute_6;
                    objitem.attribute_7 = objLandBase.attribute_7;
                    objitem.attribute_8 = objLandBase.attribute_8;
                    objitem.attribute_9 = objLandBase.attribute_9;
                    objitem.attribute_10 = objLandBase.attribute_10;
                    //objitem.sp_geometry = objLandBase.sp_geometry;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    return repo.Update(objitem);
                }
                else
                {
                    LandBaseInputGeom geom = new LandBaseInputGeom();
                 
                    geom.longLat = objLandBase.sp_geometry;
                    geom.geomType = objLandBase.geomType;
                    geom.landBaseLayerId = objLandBase.landbase_layer_id;
                    geom.center_line_geom = objLandBase.buffer_geom;
                  var geomResult =  GetEntityGeom(geom);
                    objLandBase.sp_geometry = geomResult.sp_geometry;
                    objLandBase.buffer_geom = geomResult.buffer_geom;
                    objLandBase.classification_id = objLandBase.classification_id == null?0: objLandBase.classification_id;
                    objLandBase.category_id = objLandBase.category_id == null ? 0 : objLandBase.category_id;
                    objLandBase.sub_category_id = objLandBase.sub_category_id == null ? 0 : objLandBase.sub_category_id;
                    objLandBase.created_by = userId;
                    objLandBase.status = "A";
                    objLandBase.landbase_layer_id = objLandBase.landbase_layer_id;
                    objLandBase.created_on = DateTimeHelper.Now;
                    var resultItem = repo.Insert(objLandBase);

                    //LandBaseInputGeom geom = new LandBaseInputGeom();
                    //geom.id = resultItem.id;
                    //geom.longLat = objLandBase.sp_geometry;
                    //geom.geomType = resultItem.geomType;
                    //geom.landBaseLayerId = resultItem.landbase_layer_id;
                    //geom.buffer_geom = objLandBase.buffer_geom;
                    //SaveEntityGeom(geom);
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public LandBaseNetworkCodeOut GetNetworkId(LandbaseNetworkCodeIn objLandbaseNetworkCode)
        {
            try
            {
                var result = repo.ExecuteProcedure<LandBaseNetworkCodeOut>("fn_landbase_get_network_id", new
                {
                    p_layerId = objLandbaseNetworkCode.landbase_layer_id,
                    p_geomType = objLandbaseNetworkCode.geomType,
                    p_geom = objLandbaseNetworkCode.sp_geometry,
                    p_parent_entity_type = objLandbaseNetworkCode.parent_entity_type,
                    p_parent_network_id = objLandbaseNetworkCode.parent_network_id,
                    p_parent_system_id = objLandbaseNetworkCode.parent_system_id
                });
                return result != null && result.Count > 0 ? result[0] : new LandBaseNetworkCodeOut();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public LandbaseGeomDetail GetEntityGeom(LandBaseInputGeom objgeom)
        {
            var lstGeom = new LandbaseGeomDetail();
            try
            {
                lstGeom = repo.ExecuteProcedure<LandbaseGeomDetail>("fn_landbase_save_geom",
                    new
                    {
                        
                        p_geom_type = objgeom.geomType,
                        p_longlat = objgeom.longLat,
                        p_landbase_layer_id = objgeom.landBaseLayerId,
                        p_center_line_geom = objgeom.center_line_geom
                    }).FirstOrDefault();

            }
            catch { throw; }
            return lstGeom;
        }
        public string SaveEntityGeom(LandBaseInputGeom objgeom)
        {
            try
            {
                var lstGeom = repo.ExecuteProcedure<object>("fn_landbase_save_geom",
                    new
                    {
                        p_id = objgeom.id,
                        p_geom_type = objgeom.geomType,
                        p_longlat = objgeom.longLat,
                        p_landbase_layer_id = objgeom.landBaseLayerId,
                        p_center_line_geom = objgeom.center_line_geom
                    });
                return lstGeom != null && lstGeom.Count > 0 ? lstGeom[0].ToString() : "0";
            }
            catch { throw; }
        }
        public List<LandBaseDetail> getNearByLandbaseEntities(double latitude, double longitude, int bufferInMtr, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<LandBaseDetail>("fn_landbase_get_nearby_entities", new { lat = latitude, lng = longitude, mtrBuffer = bufferInMtr, p_user_id = user_id });
            }
            catch { throw; }
        }

        public Dictionary<string, string> getLandbaseEntityInfo(int systemId, string entityType, string settingType)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_landbase_get_entity_info", new { p_systemId = systemId, p_entityType = entityType, p_settingtype = settingType }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch { throw; }
        }
        public List<T> GetLandbaseEntityExportData<T>(int systemid, string eType, string settingType) where T : new()
        {
            try
            {
                var lstItems = repo.ExecuteProcedure<T>("fn_landbase_get_entity_info", new { p_system_id = systemid, p_entityType = eType, p_settingtype = settingType }, true);
                return lstItems != null ? lstItems : new List<T>();
            }
            catch { throw; }
        }
        public GeometryDetail GetLandbaseGeometryDetails(GeomDetailIn objGeomDetailIn)
        {
            try
            {
                var lstGeomDetails = repo.ExecuteProcedure<GeometryDetail>("fn_landbase_get_geometrydetail", new { p_entitytype = objGeomDetailIn.entityType, p_geomtype = objGeomDetailIn.geomType, p_systemid = objGeomDetailIn.systemId });
                return lstGeomDetails != null && lstGeomDetails.Count > 0 ? lstGeomDetails[0] : new GeometryDetail();
            }
            catch { throw; }
        }

        public DbMessage deleteEntity(int systemId, string entityType, string geomType)
        {
            try
            {
                try
                {
                    return repo.ExecuteProcedure<DbMessage>("fn_landbase_delete_entity", new { p_system_id = systemId, p_entity_type = entityType, p_geom_type = geomType }).FirstOrDefault();

                }
                catch { throw; }
            }
            catch { throw; }
        }
        public bool chkEntityDataExist(int systemid)
        {

            try
            {
                var chk = repo.ExecuteProcedure<bool>("fn_landbase_chk_entity_data_exist", new { p_system_id = systemid });
                if (Convert.ToBoolean(chk[0]))
                {
                    return true;
                }
                else
                    return false;

            }

            catch { throw; }

        }

        public DbMessage EditLandbaseEntityGeometry(EditGeomIn objgeom)
        {
            try
            {
                var response = repo.ExecuteProcedure<DbMessage>("fn_landbase_update_entity_geom", new { p_system_id = objgeom.systemId, p_geom_type = objgeom.geomType, p_entity_type = objgeom.entityType, p_userid = objgeom.userId, p_longlat = objgeom.longLat, p_center_line_geom = objgeom.centerLineGeom }).FirstOrDefault();


                return response;
            }
            catch { throw; }
        }
        public List<LandbaseDropdownMaster> GetLandbaseDropdown(int landbase_layer_id,string category_type,int? category_parent_id)
        {
            try
            {
                return repo.ExecuteProcedure<LandbaseDropdownMaster>("fn_get_landbase_dropdown", new { p_landbase_layer_id = landbase_layer_id, p_type = category_type, p_parent_id = category_parent_id==null?0: category_parent_id }, false);
            }
            catch { throw; }
        }

        //public List<Dictionary<string, string>> GetLBExportReportSummaryView(ExportLandBaseLayerSummaryViewFilter objReportFilter)
        //{
        //    try
        //    {
        //        var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_landbase_get_export_report_summary_view",
        //            new
        //            {
        //                p_regionids = objReportFilter.SelectedRegionIds,
        //                p_provinceids = objReportFilter.SelectedProvinceIds, 
        //                p_parentusers = objReportFilter.SelectedParentUsers,
        //                p_userids = objReportFilter.SelectedUserIds,
        //                p_layer_name = objReportFilter.layerName, 
        //                p_durationbasedon = objReportFilter.durationbasedon,
        //                p_fromdate = objReportFilter.fromDate,
        //                p_todate = objReportFilter.toDate,
        //                p_geom = objReportFilter.geom,
        //                p_pageno = objReportFilter.currentPage,
        //                p_pagerecord = objReportFilter.pageSize,
        //                p_sortcolname = objReportFilter.sort,
        //                p_sorttype = objReportFilter.sortdir,
        //                p_advancefilter = objReportFilter.advancefilter,
        //                p_userid = objReportFilter.userId,
        //                p_roleid = objReportFilter.roleId  
        //            }, true);
        //        return lst;
        //    }
        //    catch { throw; }
        //}

    }
}
