using Antlr.Runtime.Tree;
using DataAccess.DBHelpers;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DALayer : Repository<layerDetail>
    {

        public List<layerDetail> GetLayerDetails()
        {
            try
            {
                return repo.GetAll().ToList();
            }
            catch { throw; }
        }
        public layerDetail GetLayerDetails(string layerName)
        {
            return repo.Get(m => m.layer_name.ToUpper() == layerName.ToUpper());
        }
        public List<layerDetail> GetOSPLayers()
        {
            try
            {
                return repo.GetAll(m => m.isvisible == true && m.is_osp_layer == true && m.is_label_change_allowed == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }
        public List<layerDetail> GetDataUploadLayers(int roleId)
        {
            try
            {
                return repo.ExecuteProcedure<layerDetail>("fn_uploader_get_upload_entity", new { p_role_id = roleId }, true).ToList();
                //return repo.GetAll(m => m.isvisible==true && m.is_data_upload_enabled == true).OrderBy(m => m.layer_title).ToList();
            }
            catch { throw; }
        }
        public List<LandBaseDetail> GetLandBaseDataUploadLayers(int roleId)
        {
            try
            {
                return repo.ExecuteProcedure<LandBaseDetail>("fn_landbase_uploader_get_upload_entity", new { p_role_id = roleId }, true).ToList();

            }
            catch { throw; }
        }


        public List<layerDetail> GetVendorSpecLayers()
        {
            try
            {
                return repo.GetAll(m => m.isvisible == true && m.is_vendor_spec_required == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }
        
        /// <summary>
        /// In this function we get All OSP Layers 
        /// created by sapna
        /// </summary>
        /// <returns></returns>
        public List<layerDetail> GetAllOSPLayers()
        {
            try
            {
                return repo.GetAll(m => m.is_osp_layer == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }

        //public List<NetworkLayer> GetNetworkLayers(int userId, int groupId)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<NetworkLayer>("fn_get_network_layers", new { userId = userId, groupId = groupId });
        //    }
        //    catch { throw; }
        //}

        public List<NetworkLayer> GetNetworkLayers(int userId, int groupId, int roleId, string connectionString)
        {
            try
            {
                if (!string.IsNullOrEmpty(connectionString))
                    connetionString = connectionString;
                return repo.ExecuteProcedure<NetworkLayer>("fn_get_network_layers", new { userId = userId, groupId = groupId, roleId = roleId });
            }
            catch { throw; }
        }

        public List<DropDownMaster> GetCablecategoryList()
        {
            DataTable dt = new DataTable();
            List<DropDownMaster> lst = new List<DropDownMaster>();
            dt = repo.GetDataTable("select dropdown_value,dropdown_type from dropdown_master where dropdown_type in ('Cable_Category')  and is_active=true");
            foreach (DataRow row in dt.Rows)
            {
                DropDownMaster obj = new DropDownMaster();
                obj.dropdown_value = row["dropdown_value"].ToString();
                obj.dropdown_type = row["dropdown_type"].ToString();
                // obj.file_type = row["filetype"].ToString();
                lst.Add(obj);
            }
            return lst;
            // return  repo.ExecuteSQLCommand(string.Format("select dropdown_value from dropdown_master where dropdown_type='Cable_Category' and is_active=true"));
        }
        public List<landBaseLayres> GetLandBaseLayres(int userId, int roleId)
        {
            try
            {
                return repo.ExecuteProcedure<landBaseLayres>("fn_landbase_get_layers", new { userId = userId, roleId = roleId });
            }
            catch { throw; }
        }

        public List<BusnessLayerforAPI> GetWMSWMTSLayres(int userId, int roleId)
        {
            try
            {
                return repo.ExecuteProcedure<BusnessLayerforAPI>("fn_wms_wmts_layers", new { userId = userId, roleId = roleId });
            }
            catch { throw; }
        }

        public List<NetworkLayer> GetAllNetworkLayersPermissions(int userId)
        {
            try
            {
                return repo.ExecuteProcedure<NetworkLayer>("fn_get_all_network_layers", new { userId = userId });
            }
            catch { throw; }
        }

        public List<string> GetUserModuleAbbrList(int userId, string userType)
        {
            try
            {
                return repo.ExecuteProcedure<UserModule>("fn_get_user_module", new { p_userid = userId, p_usertype = userType }, true).Select(x => x.module_abbr).ToList();
            }
            catch { throw; }
        }
        public NetworkLayer GetNetworkLayers(int userId, int groupId, string layerName)
        {
            try
            {
                return repo.ExecuteProcedure<NetworkLayer>("fn_get_network_layers", new { userId = userId, groupId = groupId, layer_name = layerName })[0];
            }
            catch { throw; }
        }

        //public List<NetworkLayer> GetISPNetworkLayers(int userId, int groupId)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<NetworkLayer>("fn_isp_get_network_layers", new { userId = userId, groupId = groupId }, true);
        //    }
        //    catch { throw; }
        //}
        public List<NetworkLayer> GetISPNetworkLayers(int userId, int groupId, int role_id)
        {
            try
            {
                return repo.ExecuteProcedure<NetworkLayer>("fn_isp_get_network_layers", new { userId = userId, groupId = groupId, p_role_id = role_id }, true);
            }
            catch { throw; }
        }
        public List<Kmp> Getkmp(string kmpType)
        {
            try
            {
                return repo.ExecuteProcedure<Kmp>("fn_get_kmp", new { kmpType = kmpType });
            }
            catch { throw; }
        }

        public List<RegionProvinceLayer> GetRegionProvinceLayers(int userId)

        {
            try
            {
                List<RegionProvinceLayer> lstRegionProvinceLayers = new List<RegionProvinceLayer>();

                var lstRegionProvinces = repo.ExecuteProcedure<RegionProvince>("fn_get_region_province_layers", new { userId = userId });
                if (lstRegionProvinces != null && lstRegionProvinces.Count > 0)
                {
                    lstRegionProvinceLayers = (from rp in lstRegionProvinces.AsEnumerable()
                                               group rp by new { rp.regionId, rp.regionName, rp.CountryName, rp.regionAbbr, rp.regionVisibility, rp.role_id } into g
                                               select new RegionProvinceLayer
                                               {
                                                   regionId = g.Key.regionId,
                                                   regionName = g.Key.regionName,
                                                   countryname = g.Key.CountryName,
                                                   regionAbbr = g.Key.regionAbbr,
                                                   regionVisibility = g.Key.regionVisibility,
                                                   role_id = g.Key.role_id,
                                                   lstProvince = g.Select(
                                                    p => new Province
                                                    {
                                                        provinceId = p.provinceId,
                                                        provinceName = p.provinceName,
                                                        provinceAbbr = p.provinceAbbr,
                                                        provinceVisibility = p.provinceVisibility,
                                                        role_id = p.role_id
                                                    }).ToList()
                                               }).ToList();


                }

                return lstRegionProvinceLayers;
            }
            catch { throw; }
        }
        public List<Province> GetProvincebyRegionID(string regionId)
        {
            try
            {
                return repo.ExecuteProcedure<Province>("fn_get_province_layer", new { p_regionId = regionId });
            }
            catch (Exception ex) { throw ex; }
        }

        public List<SubDistrict> GetSubdistrictByProvinceId(string stateid)
        {
            try
            {
                return repo.ExecuteProcedure<SubDistrict>("fn_get_subdistrict_layer", new { in_state_id = stateid });
            }
            catch (Exception ex) { throw ex; }
        }
        public List<Block> GetBlockBySubDistrictId(string subdistrictid)
        {
            try
            {
                return repo.ExecuteProcedure<Block>("fn_get_block_layer", new { in_block_id = subdistrictid });
            }
            catch (Exception ex) { throw ex; }
        }
        //public List<ISPNetworkLayerElement> GetISPNetworkLayerElements(int structureId)
        //{
        //    try
        //    {
        //        List<ISPNetworkLayerElement> lstISPNetworkLayers = new List<ISPNetworkLayerElement>();
        //        var lstISPNetworkLayerDetail = repo.ExecuteProcedure<ISPNetworkLayerDetail>("fn_isp_get_network_layer_elements", new { structid = structureId });

        //        if (lstISPNetworkLayerDetail.Any())
        //        {
        //            lstISPNetworkLayers = (from NLD in lstISPNetworkLayerDetail.AsEnumerable().Where(x => x.parent_entity_type == string.Empty)
        //                                   group NLD by new { NLD.entity_name, NLD.entity_title } into g
        //                                   select new ISPNetworkLayerElement
        //                                   {
        //                                       entity_name = g.Key.entity_name,
        //                                       entity_title = g.Key.entity_title,
        //                                       entity_count = g.Count(),
        //                                       structure_id = structureId,
        //                                       listChildElement = (from LFD in lstISPNetworkLayerDetail.AsEnumerable().Where(x => x.parent_entity_type == g.Key.entity_name)
        //                                                           group LFD by new { LFD.entity_name, LFD.entity_title } into g2
        //                                                           select new ISPNetworkDisplayElement
        //                                                           {
        //                                                               entity_name = g2.Key.entity_name,
        //                                                               entity_title = g2.Key.entity_title,
        //                                                               entity_count = g2.Count()
        //                                                           }).ToList()
        //                                   }).OrderBy(x => x.entity_count).ToList();
        //        }
        //        return lstISPNetworkLayers;
        //    }
        //    catch { throw; }
        //}

        public List<ISPNetworkLayerElement> GetISPNetworkLayerElements(int structureId, int role_id)
        {
            try
            {
                List<ISPNetworkLayerElement> lstISPNetworkLayers = new List<ISPNetworkLayerElement>();
                var lstISPNetworkLayerDetail = repo.ExecuteProcedure<ISPNetworkLayerDetail>("fn_isp_get_network_layer_elements", new { structid = structureId, p_role_id = role_id });

                if (lstISPNetworkLayerDetail.Any())
                {
                    lstISPNetworkLayers = (from NLD in lstISPNetworkLayerDetail.AsEnumerable().Where(x => x.parent_entity_type == string.Empty)
                                           group NLD by new { NLD.entity_name, NLD.entity_title } into g
                                           select new ISPNetworkLayerElement
                                           {
                                               entity_name = g.Key.entity_name,
                                               entity_title = g.Key.entity_title,
                                               entity_count = g.Count(),
                                               structure_id = structureId,
                                               listChildElement = (from LFD in lstISPNetworkLayerDetail.AsEnumerable().Where(x => x.parent_entity_type == g.Key.entity_name)
                                                                   group LFD by new { LFD.entity_name, LFD.entity_title } into g2
                                                                   select new ISPNetworkDisplayElement
                                                                   {
                                                                       entity_name = g2.Key.entity_name,
                                                                       entity_title = g2.Key.entity_title,
                                                                       entity_count = g2.Count()
                                                                   }).ToList()
                                           }).OrderBy(x => x.entity_count).ToList();
                }
                return lstISPNetworkLayers;
            }
            catch { throw; }
        }


        public Dictionary<string, string> getLayerDetail(int layerId, string layerName)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_layer_detail", new { p_layerId = layerId, p_layername = layerName }, true);
                return result != null && result.Count > 0 ? result[0] : new Dictionary<string, string>();
            }
            catch { throw; }
        }
        public List<LayerMapping> getLayerMapping(string layerName)
        {
            try
            {
                return repo.ExecuteProcedure<LayerMapping>("fn_get_layer_mapping", new { p_layername = layerName }, true).ToList();
            }
            catch { throw; }
        }
        public layerDetail getLayerDetailsByName(string layerName)
        {
            try
            {
                return repo.GetById(m => m.is_isp_layer == true && m.layer_name.Trim().ToLower() == layerName.Trim().ToLower());
            }
            catch
            {
                throw;
            }
        }
        public layerDetail getLayer(string layerName)
        {
            try
            {
                return repo.GetById(m => m.layer_name.Trim().ToLower() == layerName.Trim().ToLower());
            }
            catch
            {
                throw;
            }
        }

        public List<BoundaryPushToGis> GetBoundaryPushToGis(BoundaryPushFilter objPushFilter)
        {
            try
            {
                //BoundaryPushToGis
                return repo.ExecuteProcedure<BoundaryPushToGis>("fn_push_boundary_dashboard", new
                {
                    p_system_id = objPushFilter.system_id,
                    p_entity_type = objPushFilter.entity_type,
                    p_searchby = Convert.ToString(objPushFilter.viewBoundaryPush.searchBy),
                    p_searchtext = Convert.ToString(objPushFilter.viewBoundaryPush.searchText),
                    //p_pageno = objPushFilter.viewBoundaryPush.currentPage,
                    //p_pagerecord = objPushFilter.viewBoundaryPush.pageSize,
                    p_sortcolname = objPushFilter.viewBoundaryPush.sort,
                    p_sorttype = objPushFilter.viewBoundaryPush.orderBy
                }, true);
            }
            catch (Exception ex)
            { throw ex; }
        }

        public BoundaryPushToGis GetBoundaryPushStatus(BoundaryPushFilter objPushFilter)
        {
            try
            {
                return repo.ExecuteProcedure<BoundaryPushToGis>("fn_get_push_boundary_status", new { p_system_id = objPushFilter.system_id, p_entity_type = objPushFilter.entity_type, p_user_id = objPushFilter.user_id }, true).FirstOrDefault();
            }
            catch (Exception ex)
            { throw ex; }
        }





        public List<layerDetail> SaveLayerDetails(List<layerDetail> objLay)
        {
            try
            {
                foreach (var item in objLay)
                {
                    var result = repo.ExecuteProcedure<DbMessage>("fn_update_layer_zoom", new { p_layer_id = item.layer_id, p_minZoomLevel = item.minzoomlevel, p_maxZoomLevel = item.maxzoomlevel }, true);
                }
                return objLay;

            }

            catch (Exception ex)
            {
                throw;
            }
        }
        public List<NetworkLayer> GetMobileNetworkLayers(int userId, int groupId, bool isLibraryElement)
        {
            try
            {
                return repo.ExecuteProcedure<NetworkLayer>("fn_get_mobile_network_layers", new { userId = userId, groupId = groupId, isLibraryElement = isLibraryElement });
            }
            catch { throw; }
        }

        public List<LayerDetails> GetSpltParentBoxDetails()
        {
            try
            {

                return repo.ExecuteProcedure<LayerDetails>("fn_get_network_code_format", new { }, true);
            }
            catch { throw; }
        }


        public List<KeyValueDropDown> GetSearchByColumnName(string layer_name)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_reportcolumn_list", new { p_layer_name = layer_name }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetSearchByLandBaseColumnName(string layer_name)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_landbase_get_report_column_list", new { p_layer_name = layer_name }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetLMCReportSearchByColumnName(string entity_type, string lmc_type)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_lmc_report_column_list", new { p_entity_type = entity_type, p_lmc_type = lmc_type }, true);

            }
            catch { throw; }

        }
        public List<KeyValueDropDown> GetDurationBasedColumnName(string layer_name)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_duration_based_reportcolumn_list", new { p_layer_name = layer_name }, true);

            }
            catch { throw; }

        }
        public List<layerDetail> GetLayerDetailsForReport()
        {

            try
            {
                return repo.GetAll(m => m.is_report_enable == true && m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }

        public List<layerDetail> GetAllLayerDetail()
        {

            try
            {
                return repo.GetAll().OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }

        public List<ViewRCADetail> GetAllRCADetail()
        {
            try
            {
                return repo.ExecuteProcedure<ViewRCADetail>("fn_get_RCA_layers", new { }, true);
            }
            catch { throw; }
        }


        public List<layerDetail> GetAllDropdownLayerDetail()
        {

            try
            {
                return repo.ExecuteProcedure<layerDetail>("fn_get_dropdown_layers", new { }, true);
            }
            catch { throw; }
        }

        public List<layerDetail> GetLayerDetailsForUtilization()
        {
            try
            {
                return repo.GetAll(m => m.is_utilization_enabled == true && m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }

        public List<layerDetail> GetLayerDetailsForAutoPlanning()
        {
            try
            {
                return repo.GetAll(m => m.is_auto_plan_end_point == true && m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }

        public List<layerDetail> GetLayerDetailsForEntityDirection()
        {
            try
            {
                return repo.GetAll(m => m.is_entity_along_direction == true && m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }
        public List<layerReportDetail> GetReportLayers(int roleId, string purpose)
        {

            try
            {
                return repo.ExecuteProcedure<layerReportDetail>("fn_report_get_entity", new { p_roleid = roleId, p_purpose = purpose }, false);
            }
            catch { throw; }
        }
        public List<layerReportDetail> GetSplitReportLayers(int roleId, string purpose)
        {
            try
            {
                return repo.ExecuteProcedure<layerReportDetail>("fn_split_report_get_entity", new { p_roleid = roleId, p_purpose = purpose }, false);
            }
            catch { throw; }
        }
        public List<layerReportDetail> GetLandBaseReportLayers(int roleId, string purpose)
        {

            try
            {
                return repo.ExecuteProcedure<layerReportDetail>("fn_report_get_entity", new { p_roleid = roleId, p_purpose = purpose }, false);
            }
            catch { throw; }
        }

        public List<DropDownMaster> GetDropDownList(string doctype)
        {
            try
            {
                return repo.ExecuteProcedure<DropDownMaster>("fn_get_upload_dropdownlist", new { dropdownType = doctype }); //"LinkType"
            }
            catch { throw; }
        }
        public List<layerDetail> GetLayerDetailsForHistory()
        {

            try
            {
                return repo.GetAll(m => m.is_history_enabled == true && m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }

        public List<layerDetail> GetInfoLayers()
        {

            try
            {
                return repo.GetAll(m => m.is_info_enabled == true && m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }
        public List<layerDetail> GetChangeNetworkInfoLayers()
        {

            try
            {
                return repo.GetAll(x => x.is_networkcode_change_enabled == true && x.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }
        public List<Region> GetAllRegion(RegionIn objRegionIn)
        {
            try
            {
                return repo.ExecuteProcedure<Region>("fn_get_Region_boundary", new { p_userid = objRegionIn.userId, p_geom = objRegionIn.geom, p_radius = objRegionIn.buffRadius, p_geom_type = objRegionIn.geomType }, false);
            }
            catch { throw; }
        }
        public List<DbMessage> CreateEntityAlongDirection(string entity_types, string line_geom, int user_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<DbMessage>("fn_get_entity_along_direction", new { p_entity_types = entity_types, p_line_geom = line_geom, p_user_id = user_id });
                return res;
            }
            catch { throw; }
        }
        public List<Province> GetProvinceByRegionId(ProvinceIn objProvinceIn)
        {
            try
            {
                return repo.ExecuteProcedure<Province>("fn_get_province_by_region_Id", new { p_userid = objProvinceIn.userId, p_region_ids = objProvinceIn.regionIds, p_geom = objProvinceIn.geom, p_radius = objProvinceIn.buffRadius, p_geom_type = objProvinceIn.geomType }, false);
            }
            catch { throw; }
        }
        public List<layerDetail> GetStartEndPointType()
        {
            try
            {
                return repo.GetAll().Where(x => x.is_fiber_link_enabled == true).OrderBy(y => y.layer_name).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public List<ConnectionMaster> GetConnectionString(string moduleAbbr)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<ConnectionMaster>("fn_get_connectionstring", new
        //        {
        //            p_module_abbr = moduleAbbr
        //        }, true);
        //        return lst;
        //    }
        //    catch { throw; }
        //}

        public ConnectionMaster GetConnectionString(string moduleAbbr)
        {
            try
            {
                return repo.ExecuteProcedure<ConnectionMaster>("fn_get_connectionstring", new { p_module_abbr = moduleAbbr }, true).FirstOrDefault();
            }
            catch
            { throw; }
        }
        public List<EntitySummaryReport> GetExportReportSummary(ExportReportFilterNew objReportFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<EntitySummaryReport>("fn_get_export_report_summary",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layerids = objReportFilter.SelectedLayerIds,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId,
                        p_is_all_provience_assigned = objReportFilter.is_all_provience_assigned,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, false);
                return lst;

            }
            catch { throw; }
        }               
        public List<Dictionary<string, string>> GetExportReportSummaryViewNew(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetAuditLogReportSummaryView(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_audit_log_report_allexcel",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch(Exception ex)
            {

                throw;
            }
        }
        
        public List<Dictionary<string, string>> GetExportReportSummaryViewNewAdditional(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_Additional",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewNewCdb(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_Cdb",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }


        public List<Dictionary<string, string>> GetExportReportSummaryView(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                //    ExportReportLog exportReportLog = new ExportReportLog();
                //    exportReportLog.applied_filter = JsonConvert.SerializeObject(objReportFilter);
                //    exportReportLog = new DAExportReportLog().SaveExportReportLog(exportReportLog);
                //}
                //catch (Exception ex)
                //{

                //}
                //try
                //{
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewCdb(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                //    ExportReportLog exportReportLog = new ExportReportLog();
                //    exportReportLog.applied_filter = JsonConvert.SerializeObject(objReportFilter);
                //    exportReportLog = new DAExportReportLog().SaveExportReportLog(exportReportLog);
                //}
                //catch (Exception ex)
                //{

                //}
                //try
                //{
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewAdditional(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                //    ExportReportLog exportReportLog = new ExportReportLog();
                //    exportReportLog.applied_filter = JsonConvert.SerializeObject(objReportFilter);
                //    exportReportLog = new DAExportReportLog().SaveExportReportLog(exportReportLog);
                //}
                //catch (Exception ex)
                //{

                //}
                //try
                //{
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewCSV(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_csv",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<EntitySummaryReport> GetSplitReportSummary(ExportReportFilterNew objReportFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<EntitySummaryReport>("fn_get_Split_report_summary",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layerids = objReportFilter.SelectedLayerIds,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId,
                        p_is_all_provience_assigned = objReportFilter.is_all_provience_assigned,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids,
                    }, false);
                return lst;

            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetSplitReportSummaryViewAllExcel(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_split_report_summary_view_allexcel",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetSplitReportSummaryViewAllCSV(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_split_report_summary_view_allcsv",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetSplitReportSummaryViewAllShape(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_split_report_summary_view_allshape",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<EntitySummaryReport> GetAuditLogReportSummary(ExportReportFilterNew objReportFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<EntitySummaryReport>("fn_get_export_report_summary",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layerids = objReportFilter.SelectedLayerIds,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId,
                        p_is_all_provience_assigned = objReportFilter.is_all_provience_assigned,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids,
                    }, false);
                return lst;

            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetExportReportSummaryViewCSVCdb(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_Cdb",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetExportReportSummaryViewCSVAdditional(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_csv_Additional",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<WebGridColumns> GetEntityWiseColumns(int entity_id, string entity_name, string setting_type, int user_id, int role_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<WebGridColumns>("fn_get_entitywise_columns",
                    new
                    {
                        p_layer_id = entity_id,
                        p_entity_name = entity_name,
                        p_setting_type = setting_type,
                        p_userid = user_id,
                        p_roleid = role_id
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public string getCircleBoundary(double radius, string centelLatLong)
        {
            try
            {
                var lst = repo.ExecuteProcedure<string>("fn_get_circle_boundary",
                new
                {
                    p_radius = radius,
                    p_centelLatLong = centelLatLong
                }, false).FirstOrDefault().ToString();
                return lst;
            }
            catch { throw; }
        }

        public List<WebGridColumns> GetLandbaseEntityWiseColumns(int entity_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<WebGridColumns>("fn_landbase_get_entitywise_columns",
                    new
                    {
                        p_layer_id = entity_id

                    }, true);
                return lst;
            }
            catch { throw; }
        }
    
        public List<WebGridColumns> GetLandBaseLayerWiseColumns(int entity_id, string layer_name, string setting_type, int user_id, int role_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<WebGridColumns>("fn_landbase_report_get_entitywise_columns",
                    new
                    {
                        p_layer_id = entity_id,
                        p_entity_name = layer_name,
                        p_setting_type = setting_type,
                        p_userid = user_id,
                        p_roleid = role_id
                    }, true);
                return lst;
            }
            catch { throw; }
        }
        
        public List<Dictionary<string, string>> GetBuildingStatusHistory(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_building_status_history",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_advancefilter = objReportFilter.advancefilter

                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetFaultStatusHistory(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_fault_status_history",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_advancefilter = objReportFilter.advancefilter

                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetExportBarcodeBulkSummaryViewData(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_bulk_barcode_summaryview_data",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcode = objReportFilter.SelectedProjectIds,
                        p_planningcode = objReportFilter.SelectedPlanningIds,
                        p_workordercode = objReportFilter.SelectedWorkOrderIds,
                        p_purposecode = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds
                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetExportSummaryViewKML(ExportEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_kml",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcode = objReportFilter.SelectedProjectIds,
                        p_planningcode = objReportFilter.SelectedPlanningIds,
                        p_workordercode = objReportFilter.SelectedWorkOrderIds,
                        p_purposecode = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius

                    }, true);
                return lst;
            }
            catch { throw; }
        }


        public List<Dictionary<string, string>> GetExportSummaryViewKMLNew(ExportEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_kml",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcode = objReportFilter.SelectedProjectIds,
                        p_planningcode = objReportFilter.SelectedPlanningIds,
                        p_workordercode = objReportFilter.SelectedWorkOrderIds,
                        p_purposecode = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius

                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public List<UtilizationSummaryReport> GetUtilizationReportSummary(UtilizationFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<UtilizationSummaryReport>("fn_get_utilization_report_summary",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_layerids = objReportFilter.SelectedLayerIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_geom = objReportFilter.geom,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId
                    }, false);
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetUtilizationReportSummaryView(UtilizationEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_utilization_report_view",
                    new
                    { 
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_geom = objReportFilter.geom,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_advancefilter = (objReportFilter.advancefilter ?? "").Replace("'", "@"),
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId

                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetUtilizationSummaryViewKMLShape(UtilizationEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_utilization_report_kml_shape",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcode = objReportFilter.SelectedProjectIds,
                        p_planningcode = objReportFilter.SelectedPlanningIds,
                        p_workordercode = objReportFilter.SelectedWorkOrderIds,
                        p_purposecode = objReportFilter.SelectedPurposeIds,
                        p_geom = objReportFilter.geom,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_advancefilter = objReportFilter.advancefilter,
                        p_filetype = objReportFilter.fileType,
                        p_userid = objReportFilter.userId,
                        p_rolid = objReportFilter.roleId

                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public string ShowUtilizationOnMap(UtilizationEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<string>("fn_get_utilization_show_on_map",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcode = objReportFilter.SelectedProjectIds,
                        p_planningcode = objReportFilter.SelectedPlanningIds,
                        p_workordercode = objReportFilter.SelectedWorkOrderIds,
                        p_purposecode = objReportFilter.SelectedPurposeIds,
                        p_geom = objReportFilter.geom,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_advancefilter = objReportFilter.advancefilter,
                        p_utilizationtype = objReportFilter.utilizationType,
                        p_ductutilization = objReportFilter.ductutilization,
                    }).FirstOrDefault();
                return lst;
            }
            catch (Exception ex) { throw; }
        }
        public List<layerDetail> GetActiveLayers()
        {
            try
            {
                return repo.GetAll(m => m.isvisible == true).OrderBy(m => m.layer_name).ToList();
            }
            catch { throw; }
        }

        public bool GetSpecificationAllowed(string entity_type)
        {
            var status = false;
            var records = repo.GetAll(m => m.layer_name == entity_type).ToList();
            if (records[0].specification_dropdown_type == null || records[0].specification_dropdown_type == "")
            {
                status = false;
            }
            else
            {
                status = true;
            }
            return status;
        }

        #region For Additional Attributes
        public layerDetail GetLayerDetailsbyID(int layerid)
        {
            try
            {
                return repo.Get(m => m.layer_id == layerid);
            }
            catch
            {
                throw;
            }
        }

        public List<layerDetail> GetAdditionalAttributesLayers()
        {
            try
            {
                return repo.GetAll(m => m.isvisible == true
                 && m.is_osp_layer == true
                 && m.is_label_change_allowed == true
                 && m.is_dynamic_control_enable == true).OrderBy(m => m.layer_name).ToList();

            }
            catch { throw; }
        }
        #endregion

        public void UpdateRemarks(int? system_id, string entity_type, string networkId, string remark)
        {
            try
            {
                repo.ExecuteProcedure<string>("fn_update_remarks", new { p_system_id = system_id, p_entity_type = entity_type, p_network_id = networkId, p_remarks = remark }, true).ToList();
                //return repo.GetAll(m => m.isvisible==true && m.is_data_upload_enabled == true).OrderBy(m => m.layer_title).ToList();
            }
            catch { throw; }
        }
        public List<RouteInfo> getRouteInfo(string province_ids)
        {
            try
            {
                return repo.ExecuteProcedure<RouteInfo>("fn_get_route_info", new { p_province_id = province_ids }, true).ToList();
            }
            catch
            { throw; }
        }



        //Association Report
        public List<EntitySummaryReport> GetAssociationReportSummary(AssociationReportFilter objReportFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<EntitySummaryReport>("fn_get_association_report_summary",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layerids = objReportFilter.SelectedLayerIds,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
                        p_durationbasedon = objReportFilter.durationbasedon,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_userid = objReportFilter.userId,
                        p_roleid = objReportFilter.roleId,
                        p_is_all_provience_assigned = objReportFilter.is_all_provience_assigned,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids,
                    }, false);
                return lst;

            }
            catch { throw; }
        }

        public List<layerReportDetail> GetAssociationReportLayers(int roleId, string purpose)
        {

            try
            {
                return repo.ExecuteProcedure<layerReportDetail>("fn_association_report_get_entity", new { p_roleid = roleId, p_purpose = purpose }, false);
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetAssociationReportSummaryView(AssociationEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_association_report_summary_view",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetAssociationReportSummaryViewCSV(AssociationEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_association_report_summary_view_csv",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); ; ;
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetAssociationReportSummaryView(AssociationEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var currentLang = System.Globalization.CultureInfo.CurrentUICulture;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_association_report_summary_view",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcodes = objReportFilter.SelectedProjectIds,
                        p_planningcodes = objReportFilter.SelectedPlanningIds,
                        p_workordercodes = objReportFilter.SelectedWorkOrderIds,
                        p_purposecodes = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_culturename = Convert.ToString(currentLang),
                        p_radious = objReportFilter.radius,
                        p_route = objReportFilter.selected_route_ids
                    }, true); 
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetAssociationSummaryViewKML(AssociationEntitiesSummaryViewFilter objReportFilter)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_association_report_summary_view_kml",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = objReportFilter.layerName,
                        p_projectcode = objReportFilter.SelectedProjectIds,
                        p_planningcode = objReportFilter.SelectedPlanningIds,
                        p_workordercode = objReportFilter.SelectedWorkOrderIds,
                        p_purposecode = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius

                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetAssociationSummaryViewKMLNew(AssociationEntitiesSummaryViewFilter objReportFilter, string layerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(objReportFilter.connectionString))
                    connetionString = objReportFilter.connectionString;
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_export_report_summary_view_kml",
                    new
                    {
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_parentusers = objReportFilter.SelectedParentUsers,
                        p_userids = objReportFilter.SelectedUserIds,
                        p_layer_name = layerName,
                        p_projectcode = objReportFilter.SelectedProjectIds,
                        p_planningcode = objReportFilter.SelectedPlanningIds,
                        p_workordercode = objReportFilter.SelectedWorkOrderIds,
                        p_purposecode = objReportFilter.SelectedPurposeIds,
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
                        p_roleid = objReportFilter.roleId,
                        p_ownership_type = objReportFilter.SelectedOwnerShipType,
                        p_thirdparty_vendor_ids = objReportFilter.SelectedThirdPartyVendorIds,
                        p_radious = objReportFilter.radius

                    }, true);
                return lst;
            }
            catch { throw; }
        }


        //End Association Report

    }
    public class DAParentChildLayer : Repository<ParentChildLayerMapping>
    {
        public List<ParentChildLayerMapping> GetParentChildLayerDetails()
        {
            return repo.GetAll().ToList();
        }
        public DataTable GetParentChildLayerDetails(int layer_id)
        {
            if (layer_id > 0)
            {
                DataTable dt = repo.GetDataTable("select * from vw_layer_mapping where child_layer_id=" + layer_id);
                return dt;
            }
            else
            {
                DataTable dt = repo.GetDataTable("select * from vw_layer_mapping");
                return dt;

            }

        }

    }
   
}