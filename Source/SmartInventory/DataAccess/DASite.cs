using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class DASite : Repository<Site>
    {
        public Site Save(Site site, int userId)
        {
            try
            {
                var objSite = repo.Get(x => x.system_id == site.system_id);
                if (objSite != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(site.modified_on, objSite.modified_on, site.modified_by, objSite.modified_by);
                    if (objPageValidate.message != null)
                    {
                        site.objPM = objPageValidate;
                        return site;
                    }

                    objSite.site_id = site.site_id;
                    objSite.address = site.address;
                    objSite.site_name = site.site_name;
                    objSite.on_air_date = site.on_air_date;
                    objSite.removed_date = site.removed_date;
                    objSite.tx_type = site.tx_type;
                    objSite.tx_technology = site.tx_technology;
                    objSite.tx_segment = site.tx_segment;
                    objSite.tx_ring = site.tx_ring;
                    objSite.region = site.region;
                    objSite.province = site.province;
                    objSite.district = site.district;
                    objSite.region_address = site.region_address;
                    objSite.depot = site.depot;
                    objSite.ds_division = site.ds_division;
                    objSite.local_authority = site.local_authority;
                    objSite.latitude = site.latitude;
                    objSite.longitude = site.longitude;
                    objSite.owner_name = site.owner_name;
                    objSite.modified_by = userId;
                    objSite.modified_on = DateTimeHelper.Now;

                    objSite.access_24_7 = site.access_24_7;
                    objSite.tower_type = site.tower_type;
                    objSite.tower_height = site.tower_height;
                    objSite.cabinet_type = site.cabinet_type;
                    objSite.solution_type = site.solution_type;
                    objSite.site_rank = site.site_rank;
                    objSite.self_tx_traffic = site.self_tx_traffic;
                    objSite.agg_tx_traffic = site.agg_tx_traffic;
                    objSite.metro_ring_utilization = site.metro_ring_utilization;
                    objSite.csr_count = site.csr_count;
                    objSite.dti_circuit = site.dti_circuit;
                    objSite.agg_01 = site.agg_01;
                    objSite.agg_02 = site.agg_02;
                    objSite.bandwidth = site.bandwidth;
                    objSite.ring_type = site.ring_type;    //for additional-attributes
                    objSite.link_id = site.link_id;
                    objSite.alias_name = site.alias_name;
                    objSite.project_id = site.project_id;
                    objSite.planning_id = site.planning_id;
                    objSite.purpose_id = site.purpose_id;
                    objSite.workorder_id = site.workorder_id;
                    objSite.tx_agg = site.tx_agg;
                    objSite.bh_status = site.bh_status;
                    objSite.elevation = site.elevation;
                    objSite.segment = site.segment;
                    objSite.ring = site.ring;
                    objSite.maximum_cost = site.maximum_cost;
                    objSite.project_category = site.project_category;
                    objSite.priority = site.priority;
                    objSite.no_of_cores = site.no_of_cores;
                    objSite.fiber_link_type = site.fiber_link_type;
                    objSite.comment = site.comment;
                    objSite.plan_cost = site.plan_cost;
                    objSite.fiber_distance = site.fiber_distance;
                    objSite.fiber_link_code = site.fiber_link_code;
                    objSite.port_type = site.port_type;
                    objSite.destination_site_id = site.destination_site_id;
                    objSite.destination_port_type = site.destination_port_type;
                    objSite.destination_no_of_cores = site.destination_no_of_cores;
                    objSite.project_id_dialog = site.project_id_dialog;

                    //objSite.served_by_ring=site.served_by_ring;
                    var Resp = repo.Update(objSite);
                    // DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(Resp.system_id, Models.EntityType.Coupler.ToString(), Resp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Coupler.ToString(), CouplerResp.province_id);
                    return Resp;

                }
                else
                {
                    site.created_by = userId;
                    site.created_on = DateTimeHelper.Now;
                    site.network_status = String.IsNullOrEmpty(site.network_status) ? "P" : site.network_status;

                    var resultItem = repo.Insert(site);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = (int)resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Site.ToString();
                    geom.commonName = resultItem.site_name;
                    geom.geomType = GeometryType.Point.ToString();
                    // geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    // DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Coupler.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Coupler.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        public int DeleteById(int systemId)
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
            catch (Exception ex) { throw ex; }
        }
        public List<Site> GelAll(DateTime lastSuccessDate)
        {
            List<Site> lst = new List<Site>();
            try
            {
                lst = repo.GetAll(a => a.created_on >= lastSuccessDate).ToList();
            }
            catch (Exception ex) { throw ex; }
            return lst;
        }
        public List<Dictionary<string, string>> GetSiteReportData(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_sr_get_export_report_data",
                    new
                    {
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_layer_name = objReportFilter.layerName,
                        P_searchby = objReportFilter.SearchbyColumnName,
                        p_searchbytext = objReportFilter.SearchbyText,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_geom = objReportFilter.geom,
                        p_duration_based_column = objReportFilter.DurationBasedColumnName,
                        p_radius = objReportFilter.radius,
                        p_userid = objReportFilter.userId
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }

        
        public List<NearestSiteDetails> getUpdateSiteFiberDistance( string linestring, int nearestsite_system_id,int system_id, double nearestsiteDistance, string nearest_cable_geom, string nearlinegeom, int? nearest_cable_system_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<NearestSiteDetails>("fn_get_update_site_fiber_details",
                    new
                    {
                        linestring = linestring,
                        nearestsite_system_id = nearestsite_system_id,
                        p_system_id = system_id,
                        nearestsite_distance = nearestsiteDistance,
                        p_nearest_cable_geom = nearest_cable_geom,
                        p_nearest_cable_system_id = nearest_cable_system_id,
                        p_cable_end_to_site_geom = nearlinegeom
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }
        public List<NearestSiteDetails> getNearestSitelistData(int system_id, string network_id, int buffer, int PageNo)
        {
            try
            {
                var lst = repo.ExecuteProcedure<NearestSiteDetails>("fn_get_nearest_site_records",
                    new
                    {
                        p_system_id = system_id,
                        p_network_id = network_id,
                        v_buffer = buffer,
                        p_page_number = PageNo
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<NearestSiteDetails> GetSitelistData(int systemId)
        {
            try
            {
                var lst = repo.ExecuteProcedure<NearestSiteDetails>("fn_get_site_list",
                    new
                    {
                        p_system_id = systemId
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }
        public List<ExportReportKML> GetExportReportDataKML(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ExportReportKML>("fn_site_get_export_report_data_kml",
                    new
                    {
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_layer_name = objReportFilter.layerName,
                        P_searchby = objReportFilter.SearchbyColumnName,
                        p_searchbytext = objReportFilter.SearchbyText,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_duration_based_column = objReportFilter.DurationBasedColumnName,
                        p_radius = objReportFilter.radius,
                        p_userid = objReportFilter.userId
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<ExportReportKML> GetExportReportDataNearestKML(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ExportReportKML>("fn_site_get_export_report_nearest_data_kml",
                    new
                    {
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_layer_name = objReportFilter.layerName,
                        P_searchby = objReportFilter.SearchbyColumnName,
                        p_searchbytext = objReportFilter.SearchbyText,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_geom = objReportFilter.geom,
                        p_duration_based_column = objReportFilter.DurationBasedColumnName,
                        p_radius = objReportFilter.radius,
                        p_userid = objReportFilter.userId
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<Dictionary<string, string>> GetSegmentReportData(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_segment_report_data",
                    new
                    {
                       
                        P_searchby = objReportFilter.SearchbyColumnName,
                        p_searchbytext = objReportFilter.SearchbyText,
                        p_fromdate = objReportFilter.fromDate,
                        p_todate = objReportFilter.toDate,
                        p_pageno = objReportFilter.currentPage,
                        p_pagerecord = objReportFilter.pageSize,
                        p_sortcolname = objReportFilter.sort,
                        p_sorttype = objReportFilter.sortdir,
                        p_duration_based_column = objReportFilter.DurationBasedColumnName,
                        p_userid = objReportFilter.userId
                    }, true);
                return lst;
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
