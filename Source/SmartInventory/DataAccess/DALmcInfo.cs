using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DALmcInfo : Repository<LMCCableInfo>
    {
        private static DALmcInfo objLMCInfo = null;
        private static readonly object lockObject = new object();
        public static DALmcInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objLMCInfo == null)
                    {
                        objLMCInfo = new DALmcInfo();
                    }
                }
                return objLMCInfo;
            }
        }
        public LMCCableInfo SaveLMCInfo(LMCCableInfo objLMCInfo, int userId)
        {
            try
            {
                var result = repo.Get(u => u.system_id == objLMCInfo.system_id);
                if (result != null)
                {
                    result.lmc_standalone_redundant = objLMCInfo.lmc_standalone_redundant;
                    result.customer_site_id = objLMCInfo.customer_site_id;
                    result.rtn_name = objLMCInfo.rtn_name;
                    result.rtn_latitude = objLMCInfo.rtn_latitude;
                    result.rtn_longitude = objLMCInfo.rtn_longitude;
                    result.rtn_building_side_tapping_point = objLMCInfo.rtn_building_side_tapping_point;
                    result.rtn_building_side_tapping_latitude = objLMCInfo.rtn_building_side_tapping_latitude;
                    result.rtn_building_side_tapping_longitude = objLMCInfo.rtn_building_side_tapping_longitude;
                    result.lmc_cascaded_standalone = objLMCInfo.lmc_cascaded_standalone;
                    result.cascading_from_site_name = objLMCInfo.cascading_from_site_name;
                    result.otdr_length = objLMCInfo.otdr_length;
                    result.no_of_core_used = objLMCInfo.no_of_core_used;
                    result.termination_detail = objLMCInfo.termination_detail;
                    result.pop_type = objLMCInfo.pop_type;
                    result.pop_infra_id = objLMCInfo.pop_infra_id;
                    result.pop_infra_provider = objLMCInfo.pop_infra_provider;
                    result.no_of_patch_cord = objLMCInfo.no_of_patch_cord;
                    result.paf_no = objLMCInfo.paf_no;
                    result.intra_society_length = objLMCInfo.intra_society_length;
                    result.ug_length = objLMCInfo.ug_length;
                    result.row_ri_deposit = objLMCInfo.row_ri_deposit;
                    result.permit_date = objLMCInfo.permit_date;
                    result.handhole = objLMCInfo.handhole;
                    result.otl_length = objLMCInfo.otl_length;
                    result.otm_length = objLMCInfo.otm_length;
                    result.po_number = objLMCInfo.po_number;
                    result.pop_name = objLMCInfo.pop_name;
                    result.pop_latitude = objLMCInfo.pop_latitude;
                    result.pop_longitude = objLMCInfo.pop_longitude;
                    result.route_details = objLMCInfo.route_details;
                    result.route_no = objLMCInfo.route_no;
                    result.ri_length = objLMCInfo.ri_length;
                    result.ibd_length = objLMCInfo.ibd_length;
                    result.no_of_pits = objLMCInfo.no_of_pits;
                    result.modified_by = userId;
                    result.core_numbers = objLMCInfo.core_numbers;
                    result.modified_on = DateTimeHelper.Now;
                    result.objPM.isNewEntity = false;
                    result.customer_system_id = objLMCInfo.customer_system_id;
                    return repo.Update(result);
                }
                else
                {
                    objLMCInfo.created_by = userId;
                    objLMCInfo.created_on = DateTimeHelper.Now;
                    objLMCInfo.objPM.isNewEntity = true;
                    result = repo.Insert(objLMCInfo);
                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LMCCableInfo GetLMCIfo(int cableId)
        {
            var result = repo.Get(u => u.cable_system_id == cableId);
            return result != null ? result : new LMCCableInfo();
        }
        public lmcdetails GetLMCId(string lmcType, string standalone_redundant)
        {
            try
            {
                var result = repo.ExecuteProcedure<lmcdetails>("fn_get_cable_lmc_id", new { p_lmctype = lmcType, p_standalone_redundant = standalone_redundant });
                return result != null && result.Count > 0 ? result[0] : new lmcdetails();
            }
            catch { throw; }
        }
        public cableStartLatlong getCableLatLong(string entity_type, int system_id)
        {
            try
            {
                var result = repo.ExecuteProcedure<cableStartLatlong>("fn_get_cable_start_point_lat_long", new { p_entity_type = entity_type, p_system_id = system_id });
                return result != null && result.Count > 0 ? result[0] : new cableStartLatlong();
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetExportLMCReportData(ExportLMCReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_lmc_get_export_report_data",
                    new
                    {
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,  
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_layer_name = objReportFilter.entityType,
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
                        p_lmc_type = objReportFilter.lmcType,
                        p_geom_type=objReportFilter.geomType,
                        p_userid = objReportFilter.userId
                    }, true);
                return lst;
            }
            catch { throw; }
        }

        public List<Dictionary<string, string>> GetExportLMCReportDataKML(ExportLMCReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_lmc_get_export_report_data_into_kml",
                    new
                    {
                        p_networkstatues = objReportFilter.SelectedNetworkStatues,
                        p_provinceids = objReportFilter.SelectedProvinceIds,
                        p_regionids = objReportFilter.SelectedRegionIds,
                        p_layer_name = objReportFilter.entityType,
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
                        p_lmc_type = objReportFilter.lmcType,
                        p_geom_type = objReportFilter.geomType,
                        p_userid = objReportFilter.userId
                    }, true);
                return lst;
            }
            catch { throw; }
        }
        public string GetColumnNameByDisplayName(string displayName,string lmcType, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn_get_columnName_by_displayName", new { p_display_name = displayName, p_lmc_type = lmcType, p_entity_type=entityType }).FirstOrDefault();
                 
            }
            catch { throw; }
        }
    }
    }
