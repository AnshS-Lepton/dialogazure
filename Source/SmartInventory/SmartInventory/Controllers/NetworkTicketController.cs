using BusinessLogics;
using Models;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;
using System.Globalization;
using static Mono.Security.X509.X520;
using BusinessLogics.ISP;
using Models.ISP;
using System.Configuration;
using System.Net;
using SmartInventory.Settings;
using System.Runtime.Remoting;
using Newtonsoft.Json;
using BusinessLogics.Admin;

namespace SmartInventory.Controllers
{
	public class NetworkTicketController : Controller
	{
		// GET: NetworkTicket
		public PartialViewResult AddTicketDetails(NetworkTicket objTicketMaster, string source = "")
		{
			int user_id = Convert.ToInt32(Session["user_id"]);
			BLUser objBLuser = new BLUser();
			User objUserDetails = objBLuser.getUserDetails(user_id);
            if (objTicketMaster.ticket_id > 0)
			{
				User objUser = (User)Session["userDetail"];
				objTicketMaster = new BLNetworkTicket().GetNetworkTicketById(objTicketMaster.ticket_id);
				if (objUser != null)
				{
					objTicketMaster.user_role_id = objUser.role_id;
				}
			}
			if (!string.IsNullOrEmpty(objTicketMaster.geom))
			{
				var objValid = new BLMisc().ValidatePotentialArea(objTicketMaster.geom, Convert.ToInt32(Session["user_id"]), objTicketMaster.modeType, objTicketMaster.radius);
				if (objValid.status == false)
				{
					objTicketMaster.pageMsg.status = ResponseStatus.FAILED.ToString();
					objTicketMaster.pageMsg.message = BLConvertMLanguage.MultilingualMessageConvert(objValid.message);//objValid.message;
				}
				else
				{
					var result = BLBuilding.Instance.GetRegionProvince(objTicketMaster.geom, objTicketMaster.modeType);
					///var result=   GetRegionProvincebyGeom(modeType,objTicketMaster.geom);
					if (result.Count > 0)
					{
						objTicketMaster.region_id = result[0].region_id;
						objTicketMaster.province_id = result[0].province_id;
					}
					//else
					//{
					//    objTicketMaster.pageMsg.status = ResponseStatus.FAILED.ToString();
					//    objTicketMaster.pageMsg.message = "Network Ticket is intersecting multiple region/province!";
					//    //return PartialView("_AddTicketDetails",objTicketMaster);
					//}
				}
			}
			objTicketMaster.ticket_type_roleid_match = new BLTicketTypeRoleMapping().GetTicketTypeRoleMapping(objUserDetails.role_id).Select(x => x.role_id).FirstOrDefault();
			objTicketMaster.user_role_id = objUserDetails.role_id;
			objTicketMaster.source = source;
			BindTicketDropDown(objTicketMaster);
            if (!string.IsNullOrEmpty(objTicketMaster.project_ids))
            {
                var siteDetails = new BLProject().GetProjectDetailsByProjectId(objTicketMaster.project_ids);
				objTicketMaster.name = siteDetails.site_id;
            }
            return PartialView("_AddTicketDetails", objTicketMaster);
		}
		public void BindTicketDropDown(NetworkTicket objTicketMaster)
		{
			var uid = Convert.ToInt32(Session["user_id"]);
			BLUser objBLuser = new BLUser();
			User objUserDetails = objBLuser.getUserDetails(uid);
			objTicketMaster.lstTicketTypeMaster = new BLNetworkTicket().GetTicketTypeByModule(DropDownType.Network_Ticket.ToString(), uid, objUserDetails.role_id, objTicketMaster.ticket_id);
			objTicketMaster.lstReferenceType = new BLMisc().GetTicketDropdownList(DropDownType.Reference_Type.ToString(), EntityType.Network_Ticket.ToString().ToUpper());
			objTicketMaster.lstUserName = new BLUser().GetUserByManagerId(Convert.ToInt32(Session["user_id"]));
			objTicketMaster.lstRegion = new BLLayer().GetAllRegion(new RegionIn() { userId = Convert.ToInt32(Session["user_id"]) });
			objTicketMaster.lstUserModule = (List<string>)Session["ApplicableModuleList"];
			Binddropdown();
			if (objTicketMaster.province_id != 0 && objTicketMaster.region_id != 0 || objTicketMaster.province_id != null && objTicketMaster.region_id != null)
			{
				objTicketMaster.objNetwork_Ticketinput.SelectedProvinceIds = objTicketMaster.province_id.ToString();
				objTicketMaster.objNetwork_Ticketinput.SelectedRegionIds = objTicketMaster.region_id.ToString();
			}
			if (!string.IsNullOrWhiteSpace(objTicketMaster.objNetwork_Ticketinput.SelectedRegionIds))
			{
				objTicketMaster.lstProvince = new BLLayer().GetProvinceByRegionId(new ProvinceIn() { regionIds = objTicketMaster.objNetwork_Ticketinput.SelectedRegionIds, userId = Convert.ToInt32(Session["user_id"]) });
			}
		}
		public ActionResult SaveNetworkTicket(NetworkTicket objTicketMaster)
		{
			ModelState.Clear();
			if (TryValidateModel(objTicketMaster))
			{
				BLUser objBLuser = new BLUser();
				User ManagerDetails = objBLuser.getUserDetails(Convert.ToInt32(Session["user_id"]));
                User objUserDetails = objBLuser.getUserDetails(objTicketMaster.assigned_to);
				var manager_id = new BLUserManagerMapping().GetManagerMapping(objTicketMaster.assigned_to).Where(x => x.manager_id == Convert.ToInt32(Session["user_id"])).Select(x => x.manager_id).FirstOrDefault();
				//if (objUserDetails.manager_id == Convert.ToInt32(Session["user_id"]))
				if (manager_id > 0)
				{
					if(!string.IsNullOrEmpty(objTicketMaster.project_ids)) { 
                    string projectIds = objTicketMaster.project_ids ?? "";
                    string[] arrProjectIds = projectIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

						foreach (string pId in arrProjectIds)
						{
							objTicketMaster.site_project_id = pId;
							objTicketMaster.pageMsg.message = new BLNetworkTicket().SaveNetworkTicket(objTicketMaster, Convert.ToInt32(Session["user_id"]));
							/*var receivers = new string[] { MiscHelper.EncodeTo64(objUserDetails.user_email) };
							 var listEmail = new List<EmailSettingsModel>
							  {
									new EmailSettingsModel
									 {
									   email_address = MiscHelper.EncodeTo64(ManagerDetails.user_email),
									   email_password = "ostl ofqi xckw zwhq",
									   smtp_host = "smtp.gmail.com",
									   port = 587
									}
							 };
							 string mailSentMsg = "";

							 commonUtil.SendSiteAwardEmail(receivers, "The Network Ticket is assigned by", "Network Ticket Notifiation", out mailSentMsg, listEmail);
							*/
							//string WH24AuthBaseURL = ApplicationSettings.WH24AuthBaseURL;
							//string WH24URL = ApplicationSettings.WH24URL;
							//string WH24ClientId = ApplicationSettings.WH24ClientId;
							//string WH24ClientSecret = ApplicationSettings.WH24ClientSecret;
							//string WH24grantType = ApplicationSettings.WH24grantType;

							//ADOIDSecoAuth aDOIDSecoAuth = new ADOIDSecoAuth();
							//aDOIDSecoAuth.CallWH24API(WH24ClientId, WH24ClientSecret, WH24grantType, WH24AuthBaseURL, WH24URL);
						
						}
					}
					else
					{
                        objTicketMaster.pageMsg.message = new BLNetworkTicket().SaveNetworkTicket(objTicketMaster, Convert.ToInt32(Session["user_id"]));
                    }
                    if (!string.IsNullOrEmpty(objTicketMaster.pageMsg.message))
					    {
						if (objTicketMaster.pageMsg.message == "Save")
						{
							objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_153;
							objTicketMaster.pageMsg.status = "Save";
						}
						else if (objTicketMaster.pageMsg.message == "Update")
						{
							objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_318;
							objTicketMaster.pageMsg.status = "Update";
						}
						else
						{
							objTicketMaster.pageMsg.status = ResponseStatus.FAILED.ToString();
							objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_319;
						}
					}
					else
						objTicketMaster.pageMsg.message = Resources.Resources.SI_OSP_GBL_NET_FRM_319;
				}
				else
					objTicketMaster.pageMsg.message = "Assign user not exist in parent user";
			}

			return Json(objTicketMaster, JsonRequestBehavior.AllowGet);
		}
		public ActionResult ShowNetworkDashboard(NetworkTicketFilter objNetworkTicketFilter, int page = 0, string sort = "", string sortdir = "")
		{
			int user_id = Convert.ToInt32(Session["user_id"]);
			// User objUser = (User)Session["userDetail"];
			objNetworkTicketFilter.objNetworkTicket.ticket_type_id = objNetworkTicketFilter.objNetworkTicket.ticket_type_id;
			//objNetworkTicketFilter.lstTicketTypeMaster = new BLNetworkTicket().GetTicketTypeByModule(DropDownType.Network_Ticket.ToString(), user_id, objUser.role_id);
			objNetworkTicketFilter.objNetworkTicket.objNetwork_Ticketinput.SelectedRegionIds = objNetworkTicketFilter.objNetworkTicket.region_id.ToString();
			objNetworkTicketFilter.objNetworkTicket.objNetwork_Ticketinput.SelectedProvinceIds = objNetworkTicketFilter.objNetworkTicket.province_id.ToString();
			objNetworkTicketFilter.pageSize = 10;
			objNetworkTicketFilter.userId = user_id;
			objNetworkTicketFilter.currentPage = page == 0 ? 1 : page;
			objNetworkTicketFilter.sort = sort;
			objNetworkTicketFilter.orderBy = sortdir;
			User objUser = (User)Session["userDetail"];
			if (objUser != null)
			{
				objNetworkTicketFilter.objNetworkTicket.user_role_id = objUser.role_id;
			}
			var result = new BLNetworkTicket().GetNetworkTicket(objNetworkTicketFilter);
			objNetworkTicketFilter.objticketstatus = result.lstNWStatus;
			if (result.lstNWDetails != null)
			{
				foreach (Dictionary<string, string> dic in result.lstNWDetails)
				{
					var obj = (IDictionary<string, object>)new ExpandoObject();

					foreach (var col in dic)
					{
						obj.Add(col.Key, col.Value);
					}
					objNetworkTicketFilter.lstNetworkTicket.Add(obj);
				}
				objNetworkTicketFilter.totalRecord = result.lstNWDetails.Count > 0 ? Convert.ToInt32(result.lstNWDetails[0].FirstOrDefault().Value) : 0;
			}
			Session["viewNetworkTicketDashboardFilter"] = objNetworkTicketFilter;
			objNetworkTicketFilter.objNetworkTicket.ticket_type_roleid_match = new BLTicketTypeRoleMapping().GetTicketTypeRoleMapping(objNetworkTicketFilter.objNetworkTicket.user_role_id).Select(x => x.role_id).FirstOrDefault();
			BindTicketDropDown(objNetworkTicketFilter.objNetworkTicket);
			return PartialView("_NetworkTicketList", objNetworkTicketFilter);
		}

		public void ExportTickets()
		{
			if (Session["viewNetworkTicketDashboardFilter"] != null)
			{
				NetworkTicketFilter objViewFilter = (NetworkTicketFilter)Session["viewNetworkTicketDashboardFilter"];
				objViewFilter.objNetworkTicket.lstUserModule = new BLLayer().GetUserModuleAbbrList(Convert.ToInt32(Session["user_id"]), UserType.Web.ToString());

				objViewFilter.currentPage = 0;
				objViewFilter.pageSize = 0;
				var exportData = new BLNetworkTicket().GetNetworkTicket(objViewFilter);
				DataTable dtReport = new DataTable();
				dtReport = Utility.MiscHelper.GetDataTableFromDictionaries(exportData.lstNWDetails);
				dtReport.Columns["ticket_status"].SetOrdinal(0);
				dtReport.Columns["network_id"].SetOrdinal(1);
				dtReport.Columns["ticket_type"].SetOrdinal(2);
				dtReport.Columns["name"].SetOrdinal(3);
				dtReport.Columns["region_name"].SetOrdinal(5);
				dtReport.Columns["province_name"].SetOrdinal(6);
				dtReport.Columns["assigned_to_text"].SetOrdinal(7);
				dtReport.Columns["Target_Date"].SetOrdinal(8);
				dtReport.Columns["for_network_type"].SetOrdinal(9);

				if (objViewFilter.objNetworkTicket.lstUserModule.Contains("PAT"))
				{
					// dtReport.Columns["project_code"].SetOrdinal(10);
					dtReport.Columns["account_code"].SetOrdinal(11);
					dtReport.Columns["reference_type"].SetOrdinal(12);
					dtReport.Columns["reference_ticket_id"].SetOrdinal(13);
					dtReport.Columns["reference_description"].SetOrdinal(14);
					dtReport.Columns["remarks"].SetOrdinal(15);
					dtReport.Columns["created_by_text"].SetOrdinal(16);
					dtReport.Columns["created_on"].SetOrdinal(17);
					dtReport.Columns["modified_by_text"].SetOrdinal(18);
					dtReport.Columns["modified_on"].SetOrdinal(19);
					if (objViewFilter.objNetworkTicket.lstUserModule.Contains("PROJ"))
						dtReport.Columns["project_code"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_074;
					else
						dtReport.Columns.Remove("project_code");
					dtReport.Columns["account_code"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_436;
				}
				else
				{
					dtReport.Columns.Remove("project_code");
					dtReport.Columns.Remove("account_code");
					dtReport.Columns["reference_type"].SetOrdinal(10);
					dtReport.Columns["reference_ticket_id"].SetOrdinal(11);
					dtReport.Columns["reference_description"].SetOrdinal(12);
					dtReport.Columns["remarks"].SetOrdinal(13);
					dtReport.Columns["created_by_text"].SetOrdinal(14);
					dtReport.Columns["created_on"].SetOrdinal(15);
					dtReport.Columns["modified_by_text"].SetOrdinal(16);
					dtReport.Columns["modified_on"].SetOrdinal(17);
				}


				dtReport.Columns.Remove("ticket_id");
				dtReport.Columns.Remove("ticket_type_id");
				dtReport.Columns.Remove("region_id");
				dtReport.Columns.Remove("province_id");
				dtReport.Columns.Remove("assigned_to");
				dtReport.Columns.Remove("ticket_status_id");
				dtReport.Columns.Remove("ticket_status_color_code");
				dtReport.Columns.Remove("totalrecords");
				dtReport.Columns.Remove("s_no");
				dtReport.Columns.Remove("system_id");
				dtReport.Columns.Remove("status");
				dtReport.Columns.Remove("ticket_type_color_code");
				dtReport.Columns.Remove("created_by");
				dtReport.Columns.Remove("is_new_entity");
				dtReport.Columns.Remove("short_network_type");

				dtReport.Columns["ticket_type"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_133;
				dtReport.Columns["ticket_status"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_026;
				dtReport.Columns["reference_type"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_134;
				dtReport.Columns["network_id"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_GBL_153;
				dtReport.Columns["name"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_059;
				dtReport.Columns["region_name"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_065;
				dtReport.Columns["province_name"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_066;
				dtReport.Columns["reference_description"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_GBL_152;
				dtReport.Columns["remarks"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_006;
				dtReport.Columns["assigned_to_text"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_105;
				dtReport.Columns["target_date"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_141;
				dtReport.Columns["for_network_type"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_GBL_154;
				dtReport.Columns["created_by_text"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_056;
				dtReport.Columns["Created_On"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_007;
				dtReport.Columns["modified_by_text"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_058;
				dtReport.Columns["modified_on"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_057;
				dtReport.Columns["reference_ticket_id"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_FRM_031;
				var Exportfilename = "NetworkTickets";
				dtReport.TableName = "NetworkTicketList";
				ExportNetworkTicket(dtReport, Exportfilename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
			}
		}
		private void ExportNetworkTicket(DataTable dtReport, string fileName)
		{
			using (var exportData = new MemoryStream())
			{
				Response.Clear();
				if (dtReport != null && dtReport.Rows.Count > 0)
				{
					IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
					workbook.Write(exportData);
					Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
					Response.BinaryWrite(exportData.ToArray());
					Response.End();
				}
			}
		}

		public JsonResult DeleteNetworkTicket(int ticket_id)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();


			var result = new BLNetworkTicket().DeleteNetworkTicketById(ticket_id, Convert.ToInt32(Session["user_id"]));
			if (result.status)
			{
				objResp.status = ResponseStatus.OK.ToString();
				objResp.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);
			}
			else
			{
				objResp.status = ResponseStatus.FAILED.ToString();
				objResp.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}

		public JsonResult VerifiedBarcode(int system_id, string barcode)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			var result = new BLISP().VerifiedBarcode(system_id, barcode);
			if (result)
			{
				objResp.status = ResponseStatus.OK.ToString();
			}
			else
			{
				objResp.status = ResponseStatus.FAILED.ToString();
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}

		public JsonResult VerifiedMeterReading(int system_id, double meterReading)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			var result = new BLSplitter().VerifiedMeterReading(system_id, meterReading);
			if (result)
			{
				objResp.status = ResponseStatus.OK.ToString();
			}
			else
			{
				objResp.status = ResponseStatus.FAILED.ToString();
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetCreateTicketPermissionByGeom(string ticketType, string entityType, int? systemID)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			DbMessage objResponse = new BLISP().GetCreateTicketPermissionByGeom(ticketType, entityType, systemID);
			if (objResponse.status)
			{
				objResp.status = ResponseStatus.OK.ToString();
				objResp.message = objResponse.message;
			}
			else
			{
				objResp.status = ResponseStatus.FAILED.ToString();
				objResp.message = objResponse.message;
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}
		public JsonResult getFDBDetailsbySourceRefId(string sourceRefId)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			List<FDBInfo> result = new BLISP().getFDBDetailsbySourceRefId(sourceRefId);
			var isManualBarcodeResult = result.Where(j => j.source_ref_id == sourceRefId && j.is_manual_barcode == true && j.is_barcode_verified == false).ToList();
			if (isManualBarcodeResult.Count > 0)
			{
				objResp.status = ResponseStatus.FAILED.ToString();
			}
			else
			{
				objResp.status = ResponseStatus.OK.ToString();
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}
		public JsonResult getEntityVerificationStatusbySourceRefId(string sourceRefId)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			var result = new BLISP().getEntityVerificationStatusbySourceRefId(sourceRefId);
			if (result.status)
			{
				objResp.status = ResponseStatus.OK.ToString();
				objResp.message = result.message;
			}
			else
			{
				objResp.status = ResponseStatus.FAILED.ToString();
				objResp.message = result.message;
			}

			return Json(objResp, JsonRequestBehavior.AllowGet);
		}
		public JsonResult IsAllSplitterTraceValid(string sourceRefId)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			var result = new BLISP().IsAllSplitterTraceValid(sourceRefId);
			if (!result)
			{
				objResp.status = ResponseStatus.FAILED.ToString();
			}
			else
			{
				objResp.status = ResponseStatus.OK.ToString();
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetRegionProvincebyGeom(string geomType, string txtGeom)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			var objRegPro = BLBuilding.Instance.GetRegionProvince(txtGeom, geomType);
			return Json(objRegPro, JsonRequestBehavior.AllowGet);
		}

		public ActionResult NetworkTicketEntityDetails(NetworkTicketFilter objNetworkTicketFilters, int page = 1, string sort = "", string sortdir = "")
		{
			var viewNWEntitiesDetails = (NetworkTicket)Session["NWTicketDetails"];
			if (viewNWEntitiesDetails != null )
			{
				if(objNetworkTicketFilters.objNetworkTicket.ticket_id == 0)
				{
					objNetworkTicketFilters.objNetworkTicket = (NetworkTicket)Session["NWTicketDetails"];
				}
				else if(objNetworkTicketFilters.objNetworkTicket.ticket_id == viewNWEntitiesDetails.ticket_id)
				{
					objNetworkTicketFilters.objNetworkTicket.assigned_to = objNetworkTicketFilters.objNetworkTicket.assigned_to == 0 ? viewNWEntitiesDetails.assigned_to : objNetworkTicketFilters.objNetworkTicket.assigned_to;
				}
			}		
			objNetworkTicketFilters.pageSize = 10;
			objNetworkTicketFilters.currentPage = page == 0 ? 1 : page;
			objNetworkTicketFilters.userId = Convert.ToInt32(Session["user_id"]);
			objNetworkTicketFilters.sort = sort;
			objNetworkTicketFilters.sortdir = sortdir;
			Session["ticket_id"] = objNetworkTicketFilters.objNetworkTicket.ticket_id;
			var userdetails = (User)Session["userDetail"];
			var result = new BLNetworkTicket().GetNetworkTicketEntityDetails(objNetworkTicketFilters);
			objNetworkTicketFilters.objticketstatus = result.lstNWEntityStatus;
			objNetworkTicketFilters.isAuthorizedToApprove = result.isAuthorizedToApprove;
			objNetworkTicketFilters.isTraceEnabled = @ApplicationSettings.IsTraceEnabled;
			if (objNetworkTicketFilters.objNetworkTicket.ticket_type == "Construction" && objNetworkTicketFilters.isAuthorizedToApprove == true && objNetworkTicketFilters.isTraceEnabled == true)
			{
				objNetworkTicketFilters.isAllSplitterTraceStatus = new BLISP().IsAllSplitterTraceValid(Convert.ToString(objNetworkTicketFilters.objNetworkTicket.ticket_id));
			}
			if (objNetworkTicketFilters.objticketstatus == null)
				objNetworkTicketFilters.objticketstatus = new List<NetworkTicketStatus>();
			if (result.lstNWEntityDetails != null)
			{
				string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
				foreach (Dictionary<string, string> dic in result.lstNWEntityDetails)
				{
					var obj = (IDictionary<string, object>)new ExpandoObject();

					foreach (var col in dic)
					{
						if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
						{
							obj.Add(col.Key, col.Value);
						}
					}
					objNetworkTicketFilters.lstNetworkTicketData.Add(obj);
				}
				objNetworkTicketFilters.totalRecord = result.lstNWEntityDetails.Count > 0 ? Convert.ToInt32(result.lstNWEntityDetails[0].FirstOrDefault().Value) : 0;

			}
			objNetworkTicketFilters.lstRejectComment = new BLNetworkTicket().GetDropDownList(DropDownType.NWTRejectComment.ToString()).ToList();
			//objNetworkTicketFilters.lstRejectComment = objDDL.Where(x => x.dropdown_type == DropDownType.RejectComment.ToString()).ToList();
			objNetworkTicketFilters.objNetworkTicket.Network_Ticket_ID = objNetworkTicketFilters.objNetworkTicket.network_id;
			// objNetworkTicketFilters.objNetworkTicket.ticket_type_roleid_match = new BLTicketTypeRoleMapping().GetTicketTypeRoleMapping(userdetails.role_id).Select(x => x.role_id).FirstOrDefault();            
			return PartialView("_NetworkTicketEntityDetails", objNetworkTicketFilters);
		}

		public void ExportNWentityDetails()
		{
			NetworkTicketFilter objNWEntity = new NetworkTicketFilter();
			string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO", "ID" };
			objNWEntity.objNetworkTicket.ticket_id = Convert.ToInt32(Session["ticket_id"]);
			var exportData = new BLNetworkTicket().GetNetworkTicketEntityDetails(objNWEntity);
			DataTable dtReport = new DataTable();
			dtReport = Utility.MiscHelper.GetDataTableFromDictionaries(exportData.lstNWEntityDetails);
			dtReport.Columns.Remove("totalrecords");
			dtReport.Columns.Remove("s_no");
			dtReport.Columns.Remove("id");
			dtReport.Columns.Remove("geom_type");
			dtReport.Columns.Remove("status");
			dtReport.Columns.Remove("status_color");
			dtReport.Columns.Remove("network_ticket_id");
			dtReport.Columns.Remove("layer_name");
			dtReport.Columns.Remove("entity_type");
			dtReport.Columns.Remove("is_freezed");
			dtReport.Columns.Remove("action_color_code");
			dtReport.Columns.Remove("opacity");
			dtReport.Columns.Remove("created_on");
			dtReport.Columns.Remove("status_remarks");
			dtReport.Columns.Remove("entity_action_atr");
			dtReport.Columns.Remove("entity_status");
			dtReport.Columns.Remove("display_name");
			dtReport.Columns.Remove("entity_action");
			dtReport.Columns.Remove("is_revert_allowed");
			dtReport.Columns.Remove("is_manual_barcode");
			dtReport.Columns.Remove("is_barcode_verified");

			dtReport.Columns["status_description"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_GBL_160;
			dtReport.Columns["entity_title"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_098;
			dtReport.Columns["network_id"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_070;
			dtReport.Columns["entity_name"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_144;
			dtReport.Columns["region"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_065;
			dtReport.Columns["province"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_066;
			//dtReport.Columns["entity_action"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_149;
			var filename = "NetworkTicket_Entity_Report";
			dtReport.TableName = "Network Ticket Entity List";
			ExportNetworkTicket(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
		}
		public ActionResult NW_Entity_History(NetworkTicketFilter objNetworkTicketFilters, int page = 1, string sort = "", string sortdir = "")
		{
			objNetworkTicketFilters.pageSize = 10;
			objNetworkTicketFilters.currentPage = page == 0 ? 1 : page;
			objNetworkTicketFilters.sort = sort;
			objNetworkTicketFilters.sortdir = sortdir;
			objNetworkTicketFilters.userId = Convert.ToInt32(Session["user_id"]);
			objNetworkTicketFilters.entity_type = objNetworkTicketFilters.entity_type == null ? "" : objNetworkTicketFilters.entity_type;
			List<Dictionary<string, string>> lstNWEntityhistory = new BLNetworkTicket().Get_NWEntity_History(objNetworkTicketFilters);
			Session["ExportnNWEntityHistory"] = objNetworkTicketFilters;
			if (lstNWEntityhistory.Count > 0)
			{
				string[] arrIgnoreColumns = { "TOTALRECORDS", "S_NO" };
				foreach (Dictionary<string, string> dic in lstNWEntityhistory)
				{
					var obj = (IDictionary<string, object>)new ExpandoObject();
					foreach (var col in dic)
					{
						if (!Array.Exists(arrIgnoreColumns, m => m == col.Key.ToUpper()))
						{
							obj.Add(col.Key, col.Value);
						}
					}
					objNetworkTicketFilters.lstNWEntityHistory.Add(obj);
				}
				objNetworkTicketFilters.totalRecord = objNetworkTicketFilters.lstNWEntityHistory.Count > 0 ? Convert.ToInt32(lstNWEntityhistory[0].FirstOrDefault().Value) : 0;
			}
			return PartialView("_NwEntityHistory", objNetworkTicketFilters);
		}
		public void ExportNWEntity_History()
		{
			if (Session["ExportnNWEntityHistory"] != null)
			{
				NetworkTicketFilter objNetworkTicketFilters = (NetworkTicketFilter)Session["ExportnNWEntityHistory"];
				var exportData = new BLNetworkTicket().Get_NWEntity_History(objNetworkTicketFilters);
				DataTable dtReport = new DataTable();
				dtReport = MiscHelper.GetDataTableFromDictionaries(exportData);
				dtReport.Columns.Remove("totalrecords");
				dtReport.Columns.Remove("s_no");
				dtReport.Columns.Remove("entity_type");
				dtReport.Columns.Remove("entity_system_id");
				dtReport.Columns["description"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_GBL_160;
				dtReport.Columns["status_updated_on"].ColumnName = Resources.Resources.SI_OSP_BUL_NET_GBL_007;
				dtReport.Columns["status_updated_by"].ColumnName = Resources.Resources.SI_OSP_BUL_NET_GBL_006;
				dtReport.Columns["status_remarks"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_006;
				var filename = "NwEntity_History";
				dtReport.TableName = "NwEntityHistoryList";
				ExportNetworkTicket(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
			}
		}
		public ActionResult GetDraftEntityInfo(NetworkTicketFilter objNetworkTicketFilters)
		{
			var currentLang = CultureInfo.CurrentUICulture;
			string culture = currentLang.Name;
			objNetworkTicketFilters.userId = Convert.ToInt32(Session["user_id"]);
			//var userdetails = (User)Session["userDetail"];
			objNetworkTicketFilters.lstentityInfo = new BLMisc().getEntityInfo(objNetworkTicketFilters.system_id, objNetworkTicketFilters.entity_type, objNetworkTicketFilters.geom_type, Convert.ToInt32(Session["user_id"]));
			string[] arrIgnoreColumns = { };
			//new BLNetworkTicket().GetDraftEntityInfo( objNetworkTicketFilters);
			//if (result != null)
			//{
			//   
			//    result = ConvertMultilingual.MultilingualConvertModel(result, arrIgnoreColumns, culture);

			//    foreach (Dictionary<string, string> dic in result)
			//        {
			//            var obj = (IDictionary<string, object>)new ExpandoObject();

			//            foreach (var col in dic)
			//            {
			//                    obj.Add(col.Key, col.Value);
			//            }
			//            objNetworkTicketFilters.lstNetworkTicketEntityAction.Add(obj);
			//        }
			//}
			objNetworkTicketFilters.entity_action = objNetworkTicketFilters.entity_action;
			objNetworkTicketFilters.lstentityInfo = BLConvertMLanguage.MultilingualConvertModel(objNetworkTicketFilters.lstentityInfo, arrIgnoreColumns, culture);
			return PartialView("_NetworkTicketEntityAction", objNetworkTicketFilters);
		}
		public bool isFileExistOnFTP(string filepath)
		{
			var request = (FtpWebRequest)WebRequest.Create(filepath);
			string UserName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
			string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
			request.Credentials = new NetworkCredential(UserName, PassWord);
			request.Method = WebRequestMethods.Ftp.GetFileSize;
			try
			{
				FtpWebResponse response = (FtpWebResponse)request.GetResponse();
				return true;
			}
			catch (WebException ex)
			{
				FtpWebResponse response = (FtpWebResponse)ex.Response;
				if (response.StatusCode ==
					FtpStatusCode.ActionNotTakenFileUnavailable)
				{
					return false;
				}
				return false;
			}

		}
		public ActionResult GetBarcodeImage(int systemId, string entityType)

		{
			FDBInfo fDBInfoDetail = new FDBInfo();
			try
			{
				string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
				string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
				string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
				//var item = new BLAttachment().getAttachmentDetailsDocs(fDBInfo.system_id, fDBInfo.fdb_type, "Image").FirstOrDefault();
				var itemList = new BLAttachment().getEntityImages(systemId, entityType, "Image");
				var item = (itemList.Where(j => j.is_barcode_image == true).OrderByDescending(j => j.id)).FirstOrDefault();
				//var item = (itemList.Where(j => j.is_barcode_image == true)).FirstOrDefault();
				fDBInfoDetail = BLISP.Instance.getFDBDetails(systemId);
				var _imgSrc = "";
				string imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);
				WebClient request = new WebClient();
				if (!string.IsNullOrEmpty(UserName)) //Authentication require..
					request.Credentials = new NetworkCredential(UserName, PassWord);
				byte[] objdata = null;
				if (isFileExistOnFTP(imageUrl))
				{
					objdata = request.DownloadData(imageUrl);
				}
				if (objdata != null && objdata.Length > 0)
					_imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
				fDBInfoDetail.file_location = _imgSrc;// "data:image//png;base64,/9j/4AAQSkZJRgABAQAAAQABAAD…qgZLHHH24NZWUvG2mW1aF9xbATOGLqQcYVMisrKyui2Z8Uf/Z";//_imgSrc;
			}
			catch (Exception ex)
			{
				ErrorLogHelper.WriteErrorLog("GetBarcodeImage()", "Network", ex);
			}
			return PartialView("_ManualBarcode", fDBInfoDetail);
		}
		public ActionResult GetMeterReadingImage(int systemId, string entityType)

		{
			BLSplitter bLSplitter = new BLSplitter();
			SplitterMaster splitterInfoDetail = new SplitterMaster();
			try
			{
				string FtpUrl = Convert.ToString(ConfigurationManager.AppSettings["FTPAttachment"]);
				string UserName = Convert.ToString(ConfigurationManager.AppSettings["FTPUserNameAttachment"]);
				string PassWord = Convert.ToString(ConfigurationManager.AppSettings["FTPPasswordAttachment"]);
				var itemList = new BLAttachment().getEntityImages(systemId, entityType, "Image");
				var item = (itemList.Where(j => j.is_meter_reading_image == true).OrderByDescending(j => j.id)).FirstOrDefault();
				splitterInfoDetail = bLSplitter.getSplitterDetails(systemId);
				var _imgSrc = "";
				string imageUrl = string.Concat(FtpUrl, item.file_location, item.file_name);
				WebClient request = new WebClient();
				if (!string.IsNullOrEmpty(UserName)) //Authentication require..
					request.Credentials = new NetworkCredential(UserName, PassWord);
				byte[] objdata = null;
				if (isFileExistOnFTP(imageUrl))
				{
					objdata = request.DownloadData(imageUrl);
				}
				if (objdata != null && objdata.Length > 0)
					_imgSrc = string.Concat("data:image//png;base64,", Convert.ToBase64String(objdata));
				splitterInfoDetail.file_location = _imgSrc;// "data:image//png;base64,/9j/4AAQSkZJRgABAQAAAQABAAD…qgZLHHH24NZWUvG2mW1aF9xbATOGLqQcYVMisrKyui2Z8Uf/Z";//_imgSrc;

			}
			catch (Exception ex)
			{
				ErrorLogHelper.WriteErrorLog("GetPowerMeterReadingImage()", "Network", ex);
			}
			return PartialView("_ManualMeterReading", splitterInfoDetail);
		}

		public JsonResult VerifyEntities(int ticket_id, string entity_ids_types, string remarks, string status)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			try
			{
				SubmitNetworkParam objSubmitNetworkParam = new SubmitNetworkParam();
				objSubmitNetworkParam.ticket_id = ticket_id;
				objSubmitNetworkParam.entity_ids_types = entity_ids_types;
				objSubmitNetworkParam.remarks = remarks;
				objSubmitNetworkParam.source = "WEB";
				objSubmitNetworkParam.user_id = Convert.ToInt32(Session["user_id"]);
				objSubmitNetworkParam.status = status;
				var result = new BLMisc().SubmitNetwork(objSubmitNetworkParam);
				if (result.status)
				{
					#region email sending start

					if (objSubmitNetworkParam.source.ToLower() == "web")
					{
						List<Models.NetworkTicketEmailDetail> objTicketMaster = new BLNetworkTicket().GetNetworkTicketDetail(objSubmitNetworkParam.ticket_id);
						if (objTicketMaster[0].ticketcategory.ToLower() == "planning" || objTicketMaster[0].ticketcategory.ToLower() == "construction" || objTicketMaster[0].ticketcategory.ToLower() == "survey")
						{
							string EventName = "";
							if (objTicketMaster[0].ticketcategory.ToLower() == "survey")
								EventName = EmailEventList.SurveyReviewed.ToString();
							if (objTicketMaster[0].ticketcategory.ToLower() == "planning")
								EventName = EmailEventList.DesignReviewed.ToString();
							if (objTicketMaster[0].ticketcategory.ToLower() == "construction")
								EventName = EmailEventList.AcceptanceCompleted.ToString();
							Models.User objUser = new BLUser().GetUserDetailByID(objSubmitNetworkParam.user_id);
							objUser.name = MiscHelper.Decrypt(objUser.name);
							Dictionary<string, string> objDict = new Dictionary<string, string>();
							objDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
							objDict.Add("User", objUser.name);
							objDict.Add("Comments", "Approved");

							BLUser objBLuser = new BLUser();
							List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EventName);
                            System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, EmailSettings.AllEmailSettings, null, objTicketMaster[0].projectname, EventName));
                            //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, null, objTicketMaster[0].projectname);
							if (objTicketMaster[0].ticketcategory.ToLower() == "construction")
								EventName = EmailEventList.ProjectClosure.ToString();
							objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EventName);
                            System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, EmailSettings.AllEmailSettings, null, objTicketMaster[0].projectname, EventName));
                            //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, null, null, objTicketMaster[0].projectname);
						}

					}
					#endregion

					objResp.status = ResponseStatus.OK.ToString();
					objResp.message = result.message;
				}
				else
				{
					objResp.status = ResponseStatus.FAILED.ToString();
					objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
				}
			}
			catch (Exception ex)
			{
				objResp.status = ResponseStatus.ERROR.ToString();
				objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}

		public ActionResult NWEntitySummaryReport(int ticket_id)

		{
			NWEntitySummaryReport objentitySummary = new NWEntitySummaryReport();
			var userdetails = (User)Session["userDetail"];
			Session["ticket_id"] = ticket_id;
			var result = new BLNetworkTicket().NWEntitySummaryReport(ticket_id, Convert.ToInt32(Session["user_id"]));
			foreach (Dictionary<string, string> dic in result)
			{
				var obj = (IDictionary<string, object>)new ExpandoObject();

				foreach (var col in dic)
				{
					obj.Add(col.Key, col.Value);
				}
				objentitySummary.lstEntitySummaryReport.Add(obj);
			}

			return PartialView("_NetworkTicketEntitySummaryReport", objentitySummary);
		}

		public void ExportNWEntitySummaryReport()
		{

			if (Session["ticket_id"] != null)
			{
				int ticket_id = (int)Session["ticket_id"];
				var result = new BLNetworkTicket().NWEntitySummaryReport(ticket_id, Convert.ToInt32(Session["user_id"]));
				DataTable dtReport = new DataTable();
				dtReport = MiscHelper.GetDataTableFromDictionaries(result);
				//dtReport.Columns.Remove("lstEntitySummaryReport");

				dtReport.Columns["layer_title"].ColumnName = "Entity Type";
				dtReport.Columns["new_entity"].ColumnName = "New";
				dtReport.Columns["edit_location"].ColumnName = "Edit Location";
				dtReport.Columns["edit_details"].ColumnName = "Edit Detail";
				dtReport.Columns["network_status"].ColumnName = "Network Status";
				dtReport.Columns["total"].ColumnName = "Total";

				for (int i = 0; i < dtReport.Columns.Count; i++)
				{
					dtReport.Rows[dtReport.Rows.Count - 1][i] = (dtReport.Rows[dtReport.Rows.Count - 1][i]).ToString().Replace("<b>", "").Replace("</b>", "");
				}

				var filename = "NwEntity";
				dtReport.TableName = "NwEntitySummaryReport";
				ExportNetworkTicket(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
			}
		}

		public ActionResult Binddropdown()
		{
			var result = (List<string>)Session["ApplicableModuleList"];
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem { Text = "Search By", Value = "" });
			items.Add(new SelectListItem { Text = "Ticket Status", Value = "ticket_status" });
			items.Add(new SelectListItem { Text = "Network Ticket Id", Value = "network_id" });
			items.Add(new SelectListItem { Text = "Name", Value = "name" });
			items.Add(new SelectListItem { Text = "Created By", Value = "created_by_text" });
			items.Add(new SelectListItem { Text = "Assigned To", Value = "assigned_to_text" });
			items.Add(new SelectListItem { Text = "For Network Type", Value = "for_network_type" });
			items.Add(new SelectListItem { Text = "Reference Type", Value = "reference_type" });
			items.Add(new SelectListItem { Text = "Reference Ticket Id", Value = "reference_ticket_id" });
			items.Add(new SelectListItem { Text = "Reference Remarks", Value = "reference_description" });
			items.Add(new SelectListItem { Text = "Remarks", Value = "remarks" });
			if (result.Contains("PAT"))
			{
				items.Add(new SelectListItem { Text = "Project Code", Value = "project_code" });
				items.Add(new SelectListItem { Text = "Account Code", Value = "account_code" });
			}
			ViewData["listItem"] = items;
			return View();
		}
		public JsonResult getTicketEntityBounds(int ticketId)
		{
			GeometryDetail objGeometryDetail = new GeometryDetail();
			var entityBounds = new BLNetworkTicket().getTicketEntityBounds(ticketId);
			var extent = entityBounds.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
			string[] bounds = extent.Split(',');
			string[] southWest = bounds[0].Split(' ');
			string[] northEast = bounds[1].Split(' ');
			objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
			objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
			return Json(objGeometryDetail, JsonRequestBehavior.AllowGet);
		}
		public JsonResult SubmitNetworkTicket(int ticketId)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			try
			{
			    var isRejectedEntity= (new BLNetworkTicket().GetNetworkTicketRejectedEntityStatus(ticketId)).Where(j => j.status == "R").ToList();
				if (isRejectedEntity.Count == 0)
				{
					SubmitNetworkParam objSubmitNetworkParam = new SubmitNetworkParam();
					objSubmitNetworkParam.ticket_id = ticketId;
					objSubmitNetworkParam.remarks = "Submit network ticket from web";
					objSubmitNetworkParam.source = "Mobile";
					objSubmitNetworkParam.user_id = Convert.ToInt32(Session["user_id"]);
					var result = new BLMisc().SubmitNetwork(objSubmitNetworkParam);
					if (result.status)
					{
						objResp.status = ResponseStatus.OK.ToString();
						objResp.message = result.message;
						var regionProvinceId = (NetworkTicket)Session["NWTicketDetails"];
						objResp.result = JsonConvert.SerializeObject(regionProvinceId);
						Session["NWTicketDetails"] = null;
					}
					else
					{
						objResp.status = ResponseStatus.FAILED.ToString();
						objResp.message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
					}
				}
				else
				{
					objResp.status = ResponseStatus.FAILED.ToString();
					objResp.message = "Please update rejected entity status !";
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper.WriteErrorLog("SubmitNetworkTicket()", "Network", ex);
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}

		public JsonResult NetworkTicketSessionDetails(NetworkTicket objNetworkTicket, string action)
		{
			JsonResponse<string> objResp = new JsonResponse<string>();
			if (action == "Add")
			{
				Session["NWTicketDetails"] = objNetworkTicket;
				objResp.status = ResponseStatus.OK.ToString();
			}
			else
			{
				var regionProvinceId = (NetworkTicket)Session["NWTicketDetails"];
				Session["NWTicketDetails"] = null;
				objResp.status = ResponseStatus.FAILED.ToString();
				objResp.result = JsonConvert.SerializeObject(regionProvinceId);
			}
			return Json(objResp, JsonRequestBehavior.AllowGet);
		}


		public JsonResult getTicketBounds(int ticketId)
		{
			GeometryDetail objGeometryDetail = new GeometryDetail();
			var entityBounds = new BLNetworkTicket().getTicketBounds(ticketId);
			var extent = entityBounds.TrimStart("BOX(".ToCharArray()).TrimEnd(")".ToCharArray());
			string[] bounds = extent.Split(',');
			string[] southWest = bounds[0].Split(' ');
			string[] northEast = bounds[1].Split(' ');
			objGeometryDetail.southWest = new latlong { Lat = southWest[1], Long = southWest[0] };
			objGeometryDetail.northEast = new latlong { Lat = northEast[1], Long = northEast[0] };
			return Json(objGeometryDetail, JsonRequestBehavior.AllowGet);
		}

		public PartialViewResult GetApproveComments(string pEntityType, int pSystemId, string pNetworkId)
		{
			NetworkTicketFilter objNetworkTicketFilters = new NetworkTicketFilter();
			string ticket_id = new BLNetworkTicket().getTicketId(pSystemId, pEntityType, pNetworkId);
			objNetworkTicketFilters.entity_type = pEntityType;
			objNetworkTicketFilters.system_id = pSystemId;
			objNetworkTicketFilters.ticket_id = Convert.ToInt32(ticket_id);
			objNetworkTicketFilters.lstRejectComment = new BLNetworkTicket().GetDropDownList(DropDownType.NWTRejectComment.ToString()).ToList();
			return PartialView("_EntityStatus", objNetworkTicketFilters);

		}

	}

}


