using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;


namespace DataAccess
{
    public class DAROW : Repository<ROWMaster>
    {
        private static DAROW objROW = null;
        private static readonly object lockObject = new object();
        public static DAROW Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objROW == null)
                    {
                        objROW = new DAROW();
                    }
                }
                return objROW;
            }
        }
        public ROWMaster SaveROW(ROWMaster objRow, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objRow.system_id);
                if (objitem != null)
                {
                    PageMessage objPageValidate = new PageMessage();
                    //    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objRow.modified_on, objitem.modified_on, objRow.modified_by, objitem.modified_by);
                    //    if (objPageValidate.message != null)
                    //    {
                    //        objRow.objPM = objPageValidate;
                    //        return objRow;
                    //    }
                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = objRow.system_id,
                        entity_type = EntityType.ROW.ToString(),
                        old_row_type = objitem.row_type,
                        new_row_type = objRow.row_type
                    }, true);

                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                        objRow.objPM = objPageValidate;
                        return objRow;
                    }
                    objitem.row_name = objRow.row_name;
                    objitem.remarks = objRow.remarks;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.row_type = objRow.row_type;
                    objitem.project_id = objRow.project_id;
                    objitem.purpose_id = objRow.purpose_id;
                    objitem.planning_id = objRow.planning_id;
                    objitem.workorder_id = objRow.workorder_id;
                    objitem.status_remark=objRow.status_remark;
                    objitem.requested_by = objRow.requested_by;
                    objitem.request_approved_by = objRow.request_approved_by;
                    objitem.request_ref_id = objRow.request_ref_id;
                    objitem.origin_ref_id = objRow.origin_ref_id;
                    objitem.origin_ref_description = objRow.origin_ref_description;
                    objitem.origin_from = objRow.origin_from;
                    objitem.origin_ref_code = objRow.origin_ref_code;
                    ROWMaster response = repo.Update(objitem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.ROW.ToString(), response.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.ROW.ToString(), response.province_id);
                    return response;
                }
                else
                {
                    objRow.created_by = userId;
                    objRow.created_on = DateTimeHelper.Now;
                    objRow.row_stage = "New";
                    var resultItem = repo.Insert(objRow);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.ROW.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Polygon.ToString(); //objRow.geom_type;
                    geom.centerLineGeom = objRow.centerLineGeom;
                    geom.buffer_width = objRow.buffer_width;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.ROW.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.ROW.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public List<ROWAssociateEntityList> getAssociateEntityList(int pSystemId, string pEntityType)
        {
            try
            {
                return repo.ExecuteProcedure<ROWAssociateEntityList>("fn_row_get_associate_entity_list", new { p_system_id = pSystemId, p_entity_type = pEntityType }, true).ToList();
            }
            catch { throw; }
        }
        public DbMessage saveROWAssocition(string objEntityList, int parentSystemId, string parentEntityType, string parentNetworkId, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_row_save_assocition", new { p_entity_list = objEntityList, p_parent_system_id = parentSystemId, p_parent_entity_type = parentEntityType, p_parent_network_id = parentNetworkId, p_user_id = userId }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> getAssociatedEntitylist(int entityid)
        {
            try
            {

                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_row_get_associated_entity_data", new { p_system_id = entityid }, true);

            }
            catch { throw; }
        }
        public double getPITDefaultRadius(int rowSystemId, int userId)
        {
            try
            {
                return repo.ExecuteProcedure<double>("fn_row_get_pit_default_radius", new { p_row_system_id = rowSystemId, p_user_id = userId }).FirstOrDefault();
            }
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetExportApprovelReportData(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_row_get_project_approvel_report",
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
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetExportReportData(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_row_get_export_report_data",
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
            catch { throw; }
        }
        public List<Dictionary<string, string>> GetExportBudgetReportData(int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_row_get_budget_approvel_report", new { p_system_id = systemId }, true);
            }
            catch { throw; }
        }
        public ROWStageRecordCount GetROWStageRecordCount(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ROWStageRecordCount>("fn_row_get_stages_record_count",
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
                        p_radius = objReportFilter.radius
                    }, true).FirstOrDefault();
                return lst;
            }
            catch { throw; }
        }
        public ROWMaster getROWById(int systemId)
        {
            return repo.GetById(m => m.system_id == systemId);
        }
        public List<Dictionary<string, string>> GetExportRecurringReportData(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<Dictionary<string, string>>("fn_row_get_recurring_export_report_data",
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
            catch { throw; }
        }
        public List<ROWAttachments> getROWAttachments(int systemId, string rowStage, string uploadType)
        {
            try
            {
                List<ROWAttachments> response = new List<ROWAttachments>();
                if (!string.IsNullOrEmpty(rowStage))
                {
                    response = repo.ExecuteProcedure<ROWAttachments>("fn_row_get_attachment_details",
                    new { p_system_id = systemId, p_row_stage = rowStage, p_upload_type = uploadType }).ToList();
                }
                else if (string.IsNullOrEmpty(rowStage))
                {
                    response = repo.ExecuteProcedure<ROWAttachments>("fn_row_get_attachment_details",
                    new { p_system_id = systemId, p_row_stage = "Approved", p_upload_type = uploadType }).ToList();

                    if (response.Count == 0)
                    {
                        response = repo.ExecuteProcedure<ROWAttachments>("fn_row_get_attachment_details",
                      new { p_system_id = systemId, p_row_stage = "Rejected", p_upload_type = uploadType }).ToList();
                    }
                }
                if (response.Count > 0)
                {
                    var rowDetails = DAROW.Instance.getROWById(systemId);
                    if (rowDetails != null) { response[0].current_row_stage = rowDetails.row_stage; }

                }
                return response;
            }
            catch { throw; }
        }
        public List<ExportReportKML> GetExportReportDataKML(ExportReportFilter objReportFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ExportReportKML>("fn_row_get_export_report_data_kml",
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
            catch { throw; }
        }
    }

    public class DAROWApply : Repository<rowApplyStage>
    {
        private static DAROWApply objROWApply = null;
        private static readonly object lockObject = new object();
        public static DAROWApply Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objROWApply == null)
                    {
                        objROWApply = new DAROWApply();
                    }
                }
                return objROWApply;
            }
        }
        public rowApplyStage ApplyROW(rowApplyStage objRow, int userId)
        {
            try
            {
                objRow.created_by = userId;
                objRow.created_on = DateTimeHelper.Now;
                objRow.apply_date = Convert.ToDateTime(objRow.applyDate);
                return repo.Insert(objRow);
            }
            catch { throw; }
        }
        public rowApplyStage ApplyLineROW(rowApplyStage objRow, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objRow.system_id);
                if (objitem != null)
                {
                    return repo.Update(objitem);
                }
                else
                {
                    return repo.Insert(objRow);
                }
            }
            catch { throw; }
        }
        public rowApplyStage getROWApplyDetails(int rowSystemid)
        {
            try
            {
                var appliedDetails = repo.Get(x => x.row_system_id == rowSystemid);
                return appliedDetails != null ? appliedDetails : new rowApplyStage();
            }
            catch { throw; }
        }
        public bool isROWApplied(int rowSystemId)
        {
            return repo.GetAll(m => m.row_system_id == rowSystemId).ToList().Count > 0;
        }

    }
    public class DAROWApprove : Repository<rowApproveRejectStage>
    {
        private static DAROWApprove objROWApprove = null;
        private static readonly object lockObject = new object();
        public static DAROWApprove Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objROWApprove == null)
                    {
                        objROWApprove = new DAROWApprove();
                    }
                }
                return objROWApprove;
            }
        }
        public rowApproveRejectStage ApproveROW(rowApproveRejectStage objRow, int userId)
        {
            try
            {
                objRow.created_by = userId;
                objRow.created_on = DateTimeHelper.Now;
                return repo.Insert(objRow);
            }
            catch { throw; }
        }
        public rowApproveRejectStage getROWApproveDetails(int rowSystemid)
        {
            try
            {
                var rowStageDetails = repo.Get(x => x.row_system_id == rowSystemid);
                return rowStageDetails != null ? rowStageDetails : new rowApproveRejectStage();
            }
            catch { throw; }
        }
    }
    public class DAROWPIT : Repository<ROWPIT>
    {
        private static DAROWPIT objROWPIT = null;
        private static readonly object lockObject = new object();
        public static DAROWPIT Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objROWPIT == null)
                    {
                        objROWPIT = new DAROWPIT();
                    }
                }
                return objROWPIT;
            }
        }
        public ROWPIT SavePIT(ROWPIT objPIT, int userId)
        {
            try
            {
                var pitDetails = repo.Get(m => m.system_id == objPIT.system_id);
                if (pitDetails != null)
                {
                    return pitDetails;
                }
                else
                {
                    objPIT.created_by = userId;
                    objPIT.created_on = DateTimeHelper.Now;
                    var result = repo.Insert(objPIT);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = result.system_id;
                    geom.longLat = result.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.PIT.ToString();
                    geom.geomType = result.geomType;
                    geom.commonName = result.network_id;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.PIT.ToString(), result.province_id, 0);
                   // DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.PIT.ToString(), result.province_id);
                    return result;
                }
            }
            catch { throw; }

        }
        public List<ROWDetails> GetROWExist(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<ROWDetails>("fn_row_get_exist", new { p_geometry = geom, p_geomtype = GeometryType.Polygon.ToString() });
            }
            catch { throw; }
        }
        public double GetPITRadius(int systemId, string entityType)
        {
            try
            {
                var radius = repo.ExecuteProcedure<double>("fn_get_circle_radius", new { p_system_id = systemId, p_entity_type = EntityType.PIT.ToString() }).FirstOrDefault();
                return radius;
            }
            catch { throw; }
        }
        public ROWArea GetAreaLength(int systemId, string entityType)
        {
            try
            {
                return repo.ExecuteProcedure<ROWArea>("fn_row_get_area_length", new { p_system_id = systemId, p_entity_type = EntityType.ROW.ToString() }).FirstOrDefault();
            }
            catch { throw; }
        }
        public bool isPITApplied(int rowSystemId)
        {
            var pitDetails = repo.GetAll(m => m.parent_system_id == rowSystemId && m.parent_entity_type == EntityType.ROW.ToString()).ToList();
            return pitDetails.Count() > 0;
        }
        public ROWMaster getROWByPIT(int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<ROWMaster>("fn_row_get_details_by_pit", new { p_system_id = systemId }, true).FirstOrDefault();
            }
            catch { throw; }
        }

    }
    public class DAROWRemarks : Repository<ROWRemarks>
    {
        private static DAROWRemarks objROWRemarks = null;
        private static readonly object lockObject = new object();
        public static DAROWRemarks Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objROWRemarks == null)
                    {
                        objROWRemarks = new DAROWRemarks();
                    }
                }
                return objROWRemarks;
            }
        }
        public void SaveROWRemarks(List<ROWRemarks> objRemarks, int userId)
        {
            try
            {

                foreach (var item in objRemarks)
                {
                    var remarksDetails = repo.GetById(m => m.id == item.id);
                    if (remarksDetails != null)
                    {
                        remarksDetails.remarks = item.remarks;
                        remarksDetails.modified_on = DateTimeHelper.Now;
                        remarksDetails.modified_by = userId;
                        repo.Update(remarksDetails);
                    }
                    else if (!string.IsNullOrEmpty(item.remarks))
                    {
                        item.created_by = userId;
                        item.created_on = DateTimeHelper.Now;
                        repo.Insert(item);
                    }
                }



            }
            catch { throw; }
        }
        public List<ROWRemarks> getROWRemarks(int systemId, string rowStage)
        {
            try
            {
                return repo.ExecuteProcedure<ROWRemarks>("fn_row_get_remarks_details", new { p_system_id = systemId, p_row_stage = rowStage }, true);
            }
            catch { throw; }
        }
    }
    public class DAPITTemplate : Repository<PITTemplateMaster>
    {
        private static DAPITTemplate objPITTemp = null;
        private static readonly object lockObject = new object();
        public static DAPITTemplate Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objPITTemp == null)
                    {
                        objPITTemp = new DAPITTemplate();
                    }
                }
                return objPITTemp;

            }
        }
        public PITTemplateMaster SavePitTemplate(PITTemplateMaster objPITTemplate, int userId)
        {
            try
            {
                var templateDetails = repo.GetById(m => m.id == objPITTemplate.id);
                if (templateDetails != null)
                {
                    templateDetails.radius = objPITTemplate.radius;
                    templateDetails.modified_by = userId;
                    templateDetails.modified_on = DateTimeHelper.Now;
                    return repo.Update(templateDetails);
                }
                else
                {
                    objPITTemplate.created_by = userId;
                    objPITTemplate.created_on = DateTimeHelper.Now;
                    return repo.Insert(objPITTemplate);
                }
            }
            catch { throw; }
        }
    }
    public class DAROWAuthority : Repository<ROWAuthority>
    {
        private static DAROWAuthority objAuthority = null;
        private static readonly object lockObject = new object();
        public static DAROWAuthority Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objAuthority == null)
                    {
                        objAuthority = new DAROWAuthority();
                    }
                }
                return objAuthority;
            }
        }
        public List<DropDownMaster> getAuthorityList()
        {
            return repo.GetAll().Where(m => m.is_active == true).Select(m => new DropDownMaster { dropdown_key = m.authority, dropdown_value = m.authority }).ToList();
        }
    }
    public class DAROWOtherCharges : Repository<ROWChargesTemplate>
    {
        private static DAROWOtherCharges objOtherCharges = null;
        private static readonly object lockObject = new object();
        public static DAROWOtherCharges Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objOtherCharges == null)
                    {
                        objOtherCharges = new DAROWOtherCharges();
                    }
                }
                return objOtherCharges;
            }
        }
        public List<ROWChargesTemplate> getChargesTemplates()
        {
            return repo.GetAll().ToList();
        }
        public ROWChargesTemplate getTemplateById(int id)
        {
            try
            {
                return repo.ExecuteProcedure<ROWChargesTemplate>("fn_row_get_cost_template_by_id", new { p_template_id = id }, true).FirstOrDefault();
            }
            catch { throw; }
        }
    }
}
