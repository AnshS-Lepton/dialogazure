using BusinessLogics;
using Models;
using Models.Admin;
using Models.API;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Utility;


namespace SmartInventoryServices.Controllers
{
    [CustomAuthorization]
    [APIExceptionFilter]
    [CustomAction]
    public class ReportController : ApiController
    {
        // GET: Report
        [HttpPost]
        public ApiResponse<ExportEntitiesReportNew> GetExportReportDropdowns(ReqInput data)
        {
            var response = new ApiResponse<ExportEntitiesReportNew>();
            try
            {
                ExportReportIn obj = ReqHelper.GetRequestData<ExportReportIn>(data);
                ExportEntitiesReportNew objExportEntitiesReport = new ExportEntitiesReportNew();
                objExportEntitiesReport.objReportFilters.userId = obj.userId;
                objExportEntitiesReport.objReportFilters.roleId = obj.roleId;
                BindReportDropdownNew(ref objExportEntitiesReport);                
                response.status = StatusCodes.OK.ToString();
                response.results = objExportEntitiesReport;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetExportReportDropdowns()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        
        [HttpPost]
        public ApiResponse<List<EntitySummaryReport>> GetExportReportSummary(ReqInput data)
        {
            var response = new ApiResponse<List<EntitySummaryReport>>();
            try
            {
                List<EntitySummaryReport> lstReportData = new List<EntitySummaryReport>();
                ExportReportFilterNewIn obj = ReqHelper.GetRequestData<ExportReportFilterNewIn>(data);
                ExportReportFilterNew objReportFilters = new ExportReportFilterNew();
                objReportFilters.SelectedRegionIds = obj.SelectedRegionIds;
                objReportFilters.SelectedProvinceIds = obj.SelectedProvinceIds;
                objReportFilters.SelectedNetworkStatues = obj.SelectedNetworkStatues;
                objReportFilters.SelectedParentUsers = obj.SelectedParentUsers;
                objReportFilters.SelectedUserIds = obj.SelectedUserIds;
                objReportFilters.SelectedLayerIds = obj.SelectedLayerIds;
                objReportFilters.SelectedProjectIds = obj.SelectedProjectIds;
                objReportFilters.SelectedPlanningIds = obj.SelectedPlanningIds;
                objReportFilters.SelectedWorkOrderIds = obj.SelectedWorkOrderIds;
                objReportFilters.SelectedPurposeIds = obj.SelectedPurposeIds;
                objReportFilters.userId = obj.userId;
                objReportFilters.roleId = obj.roleId;
                objReportFilters.SelectedOwnerShipType = obj.SelectedOwnerShipType;
                objReportFilters.SelectedThirdPartyVendorIds = obj.SelectedThirdPartyVendorIds;
                objReportFilters.fromDate = obj.fromDate;
                objReportFilters.toDate = obj.toDate;
                objReportFilters.durationbasedon = obj.durationbasedon;
                if (obj.fromDate != "" && obj.fromDate != null)
                {
					lstReportData = new BLLayer().GetExportReportSummary(objReportFilters).OrderBy(m => m.entity_name).ToList();
					response.status = StatusCodes.OK.ToString();
                    response.results = lstReportData;
                }
                else
                {
                    response.status = StatusCodes.VALIDATION_FAILED.ToString();
                    response.error_message = "From Date can not be blank!";
                }
                
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetExportReportSummary()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        public void BindReportDropdownNew(ref ExportEntitiesReportNew objExportEntitiesReport)
        {
            //Bind Layers..
            objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(objExportEntitiesReport.objReportFilters.roleId, "ENTITY");
            //Bind Regions..
            objExportEntitiesReport.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = objExportEntitiesReport.objReportFilters.userId });
            //Bind Provinces..
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedRegionIds))
            {
                objExportEntitiesReport.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objExportEntitiesReport.objReportFilters.SelectedRegionIds, userId = objExportEntitiesReport.objReportFilters.userId });
            }
            //if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
            //{
            //    objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(objExportEntitiesReport.objReportFilters.layerName);
            //}
            //for parent user list deafult all under sa
            List<int> parentUser = new List<int>();
            parentUser.Add(1);
            if (objExportEntitiesReport.objReportFilters.roleId == 1)
                objExportEntitiesReport.lstParentUsers = new BLUser().GetUsersListByMGRIds(parentUser).OrderBy(x => x.user_name).ToList();//new BLUser().GetUsersListByMGRIds(parentUser).Where(x => x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            else
            {
                objExportEntitiesReport.lstParentUsers = new List<Models.User>();
                Models.User objuser = new BLUser().GetUserDetailByID(objExportEntitiesReport.objReportFilters.userId);
               objExportEntitiesReport.lstParentUsers.Add(objuser);// new BLUser().GetUserDetailByID(Convert.ToInt32(Session["user_id"])));// new BLUser().GetUsersListByMGRIds(parentUser).Where(x=> x.user_id == Convert.ToInt32(Session["user_id"])).OrderBy(x => x.user_name).ToList();
            }
            if (objExportEntitiesReport.objReportFilters.SelectedParentUser != null)
            {
                if (objExportEntitiesReport.objReportFilters.roleId != 1)
                {
                    var parentUser_ids = string.Join(",", objExportEntitiesReport.objReportFilters.SelectedParentUser.Select(n => n.ToString()).ToArray());
                    objExportEntitiesReport.lstUsers = new BLUser().GetUserReportDetailsList(parentUser_ids).ToList();
                }
                else
                {
                    objExportEntitiesReport.lstUsers = new BLUser().GetUsersListByMGRIds(objExportEntitiesReport.objReportFilters.SelectedParentUser).OrderBy(x => x.user_name).ToList();
                }
            }
            //for project code,planning code,workordercode & purpose code
            objExportEntitiesReport.lstBindProjectCode = new BusinessLogics.Admin.BLProject().getProjectCodeDetails(string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.SelectedNetworkStatues) ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "PLANNED" ? "P" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "AS BUILT" ? "A" : objExportEntitiesReport.objReportFilters.SelectedNetworkStatues.ToUpper() == "DORMANT" ? "D" : "P");
            if (objExportEntitiesReport.objReportFilters.SelectedProjectId != null)
                objExportEntitiesReport.lstBindPlanningCode = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(objExportEntitiesReport.objReportFilters.SelectedProjectId);
            if (objExportEntitiesReport.objReportFilters.SelectedPlanningId != null)
                objExportEntitiesReport.lstBindWorkorderCode = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(objExportEntitiesReport.objReportFilters.SelectedPlanningId);
            if (objExportEntitiesReport.objReportFilters.SelectedWorkOrderId != null)
                objExportEntitiesReport.lstBindPurposeCode = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(objExportEntitiesReport.objReportFilters.SelectedWorkOrderId);
            //for duration based on 
            objExportEntitiesReport.lstDurationBasedOn = new BLMisc().GetDropDownList("", DropDownType.Export_Report.ToString());
            objExportEntitiesReport.lstNetworkStatus = new BLMisc().GetDropDownList("", DropDownType.Network_Status.ToString());
            objExportEntitiesReport.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            
        }

        [HttpPost]
        public ApiResponse<List<Province>> BindProviceByRegionId(ReqInput data)
        {
            var response = new ApiResponse<List<Province>>();
            try
            {
                ProvinceIn objProvinceIn = ReqHelper.GetRequestData<ProvinceIn>(data);
                objProvinceIn.userId = objProvinceIn.user_id;
                var objResp = new BLLayer().GetProvinceByRegionId(objProvinceIn);
                response.status = ResponseStatus.OK.ToString();
                response.results = objResp;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("BindProviceByRegionId()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<List<UserReportDetail>> GetReportUsersByParentUser(ReqInput data)
        {
            var response = new ApiResponse<List<UserReportDetail>>();
            try
            {
                UserReportDetail objUserReportDtl = ReqHelper.GetRequestData<UserReportDetail>(data);
                var objResp = new BLUser().GetUserReportDetailsList(objUserReportDtl.parentUser_ids, true).ToList();
                if (objUserReportDtl.role_id == 1)
                {
                    objResp = objResp.Where(x => x.groupUser != "Others").ToList();
                }
                response.status = ResponseStatus.OK.ToString();
                response.results = objResp;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetReportUsersByParentUser()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<List<PlanningCodeMaster>> GetPlanningByProjectids(ReqInput data)
        {
            var response = new ApiResponse<List<PlanningCodeMaster>>();
            try
            {
                PlanningCodeMaster obj = new PlanningCodeMaster();
                obj.ddlproject_ids = new List<int>();
                obj.ddlproject_ids = ReqHelper.GetRequestData<PlanningCodeMasterIn>(data).projectIds;
                var objResp = new BusinessLogics.Admin.BLProject().getPlanningDetailByProjectIds(obj.ddlproject_ids);
                response.status = ResponseStatus.OK.ToString();
                response.results = objResp;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetPlanningByProjectids()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<List<WorkorderCodeMaster>> GetWokorderByPlanningids(ReqInput data)
        {
            var response = new ApiResponse<List<WorkorderCodeMaster>>();
            try
            {
                WorkorderCodeMaster obj = new WorkorderCodeMaster();
                obj.ddlplanning_ids = new List<int>();
                obj.ddlplanning_ids = ReqHelper.GetRequestData<WorkorderCodeMasterIn>(data).planningIds;
                var objResp = new BusinessLogics.Admin.BLProject().getWorkorderDetailByPlanningIds(obj.ddlplanning_ids);
                response.status = ResponseStatus.OK.ToString();
                response.results = objResp;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetWokorderByPlanningids()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<List<PurposeCodeMaster>> GetPurposeByWorkOrderids(ReqInput data)
        {
            var response = new ApiResponse<List<PurposeCodeMaster>>();
            try
            {
                PurposeCodeMaster obj = new PurposeCodeMaster();
                obj.ddlworkorder_ids = new List<int>();
                obj.ddlworkorder_ids = ReqHelper.GetRequestData<PurposeCodeMasterIn>(data).workorderIds;
                var objResp = new BusinessLogics.Admin.BLProject().getPurposeDetailByWorkOrderIds(obj.ddlworkorder_ids);
                response.status = ResponseStatus.OK.ToString();
                response.results = objResp;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetPurposeByWorkOrderids()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }

        [HttpPost]
        public ApiResponse<ExportEntitiesSummaryView> getEntityExportList(ReqInput data)
        {
            var response = new ApiResponse<ExportEntitiesSummaryView>();
            try
            {
                ExportReportFilterNewIn objExportReportFilterNew = ReqHelper.GetRequestData<ExportReportFilterNewIn>(data);
                ExportEntitiesSummaryView objExportEntitiesReport = new ExportEntitiesSummaryView();
                //objExportReportFilterNew = (ExportReportFilterNew)Session["ExportReportFilterNew"];

                //objExportEntitiesReport.objReportFilters.SelectedRegionId = objExportReportFilterNew.SelectedRegionId;
                //objExportEntitiesReport.objReportFilters.SelectedProvinceId = objExportReportFilterNew.SelectedProvinceId;
                //objExportEntitiesReport.objReportFilters.SelectedParentUser = objExportReportFilterNew.SelectedParentUser;
                //objExportEntitiesReport.objReportFilters.SelectedUserId = objExportReportFilterNew.SelectedUserId;
                //objExportEntitiesReport.objReportFilters.SelectedProjectId = objExportReportFilterNew.SelectedProjectId;
                //objExportEntitiesReport.objReportFilters.SelectedPlanningId = objExportReportFilterNew.SelectedPlanningId;
                //objExportEntitiesReport.objReportFilters.SelectedWorkOrderId = objExportReportFilterNew.SelectedWorkOrderId;
                //objExportEntitiesReport.objReportFilters.SelectedPurposeId = objExportReportFilterNew.SelectedPurposeId;

                objExportEntitiesReport.objReportFilters.SelectedRegionIds = objExportReportFilterNew.SelectedRegionIds;
                objExportEntitiesReport.objReportFilters.SelectedProvinceIds = objExportReportFilterNew.SelectedProvinceIds;
                objExportEntitiesReport.objReportFilters.SelectedNetworkStatues = objExportReportFilterNew.SelectedNetworkStatues;
                objExportEntitiesReport.objReportFilters.SelectedParentUsers = objExportReportFilterNew.SelectedParentUsers;
                objExportEntitiesReport.objReportFilters.SelectedUserIds = objExportReportFilterNew.SelectedUserIds;
                objExportEntitiesReport.objReportFilters.SelectedLayerIds = objExportReportFilterNew.SelectedLayerIds;
                objExportEntitiesReport.objReportFilters.SelectedProjectIds = objExportReportFilterNew.SelectedProjectIds;
                objExportEntitiesReport.objReportFilters.SelectedPlanningIds = objExportReportFilterNew.SelectedPlanningIds;
                objExportEntitiesReport.objReportFilters.SelectedWorkOrderIds = objExportReportFilterNew.SelectedWorkOrderIds;
                objExportEntitiesReport.objReportFilters.SelectedPurposeIds = objExportReportFilterNew.SelectedPurposeIds;
                objExportEntitiesReport.objReportFilters.durationbasedon = objExportReportFilterNew.durationbasedon;
                objExportEntitiesReport.objReportFilters.fromDate = objExportReportFilterNew.fromDate;
                objExportEntitiesReport.objReportFilters.toDate = objExportReportFilterNew.toDate;
                objExportEntitiesReport.objReportFilters.geom = objExportReportFilterNew.geom;
                objExportEntitiesReport.objReportFilters.pageSize = 10;
                objExportEntitiesReport.objReportFilters.currentPage = 1;
                objExportEntitiesReport.objReportFilters.sort = "";
                objExportEntitiesReport.objReportFilters.sortdir = "";
                objExportEntitiesReport.objReportFilters.userId = objExportReportFilterNew.userId;
                objExportEntitiesReport.objReportFilters.roleId = objExportReportFilterNew.roleId;
                objExportEntitiesReport.objReportFilters.SelectedOwnerShipType = objExportReportFilterNew.SelectedOwnerShipType;
                objExportEntitiesReport.objReportFilters.SelectedThirdPartyVendorIds = objExportReportFilterNew.SelectedThirdPartyVendorIds;
                objExportEntitiesReport.objReportFilters.layerName = objExportReportFilterNew.layerName;
                BindReportDropdownSummaryView(ref objExportEntitiesReport);
                //var userdetails = (User)Session["userDetail"];
                objExportEntitiesReport.objReportFilters.SelectedLayerId = new BLLayer().GetReportLayers(objExportEntitiesReport.objReportFilters.roleId, "ENTITY").Where(x => x.layer_name == objExportEntitiesReport.objReportFilters.layerName).Select(x => x.layer_id).ToList();
                objExportEntitiesReport.objReportFilters.lstAdvanceFilters = objExportEntitiesReport.lstAdvanceFilters;
                if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
                {
                    objExportEntitiesReport.objReportFilters.advancefilter = getAdvanceFilter(objExportEntitiesReport.lstAdvanceFilters);
                    List<Dictionary<string, string>> lstExportEntitiesDetail = new BLLayer().GetExportReportSummaryView(objExportEntitiesReport.objReportFilters);

                    string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "BARCODE" };
                    //objExportEntitiesReport.webColumns = ConvertMultilingual.GetEntityWiseColumns(objExportEntitiesReport.objReportFilters.SelectedLayerId[0], objExportEntitiesReport.objReportFilters.layerName, "REPORT", arrIgnoreColumns, userdetails.role_id, userdetails.user_id);
                    foreach (Dictionary<string, string> dic in lstExportEntitiesDetail)
                    {
                        var obj = (IDictionary<string, object>)new ExpandoObject();

                        foreach (var col in dic)
                        {
                            //if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
                            //{
                            //    obj.Add(col.Key, col.Value);
                            //}
                             obj.Add(col.Key, col.Value);
                        }
                        objExportEntitiesReport.lstReportData.Add(obj);
                    }
                    //objExportEntitiesReport.lstReportData = ConvertMultilingual.MultilingualConvert(objExportEntitiesReport.lstReportData, arrIgnoreColumns);
                    objExportEntitiesReport.objReportFilters.totalRecord = lstExportEntitiesDetail.Count > 0 ? Convert.ToInt32(lstExportEntitiesDetail[0].FirstOrDefault().Value) : 0;
                }
                // if there is no filter and add one row by default...
                if (objExportEntitiesReport.lstAdvanceFilters.Count == 0)
                {
                    objExportEntitiesReport.lstAdvanceFilters.Add(new ReportAdvanceFilter());
                }
                response.status = StatusCodes.OK.ToString();
                response.results = objExportEntitiesReport;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getEntityExportList()", "Report Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        public void BindReportDropdownSummaryView(ref ExportEntitiesSummaryView objExportEntitiesReport)
        {
            //rt
           // var userdetails = (User)Session["userDetail"];
            //Bind Layers..
            objExportEntitiesReport.lstLayers = new BLLayer().GetReportLayers(objExportEntitiesReport.objReportFilters.roleId, "ENTITY");
            var selectedlayerids = objExportEntitiesReport.objReportFilters.SelectedLayerIds;
            if (selectedlayerids != null)
            {
                if (selectedlayerids.Count() > 0)
                    objExportEntitiesReport.lstLayers = objExportEntitiesReport.lstLayers.Where(m => selectedlayerids.Contains(m.layer_id.ToString())).ToList();
            }
            //objExportEntitiesReport.objReportFilters.layerName = objExportEntitiesReport.objReportFilters.layerName == null ? objExportEntitiesReport.lstLayers[0].layer_name : objExportEntitiesReport.objReportFilters.layerName;
            if (!string.IsNullOrWhiteSpace(objExportEntitiesReport.objReportFilters.layerName))
            {
                objExportEntitiesReport.lstLayerColumns = new BLLayer().GetSearchByColumnName(objExportEntitiesReport.objReportFilters.layerName);
            }
        }

        public string getAdvanceFilter(List<ReportAdvanceFilter> lstFilters)
        {
            StringBuilder sbFilter = new StringBuilder();
            if (lstFilters != null)
            {
                foreach (var item in lstFilters)
                {
                    if (!string.IsNullOrEmpty(item.searchBy) && !string.IsNullOrEmpty(item.searchType))
                    {
                        item.searchText = item.searchText ?? "";
                        sbFilter.Append(" and upper(COALESCE(a." + item.searchBy + "::text,'')) " + item.searchType + (item.searchType.ToUpper() == "LIKE" ? "'%" + item.searchText.Trim().ToUpper() + "%'" : "'" + item.searchText.Trim().ToUpper() + "'"));
                    }
                }
            }
            return sbFilter.ToString();
        }
        
    }
}