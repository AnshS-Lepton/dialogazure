using BusinessLogics;
using BusinessLogics.Admin;
using BusinessLogics.ISP;
using Models;
using Models.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Serialization;
using Models.ISP;
using System.Configuration;
using System.IO;
using System.Web;
using System.Data;
using Utility;



namespace SmartInventoryServices.Controllers
{
	[RoutePrefix("api/Library")]
	[CustomAuthorization]
	[APIExceptionFilter]
	[CustomAction]
	public class LibraryController : ApiController
	{

		#region Entity Operations
		/// <summary>
		/// Entity Operations Generic Api
		/// </summary>
		/// <param name="data">Json Data</param>
		/// <returns>Result According TO pass Action</returns>
		/// <Created By>Sumit Poonia</Created By>
		/// <Created Date>01-Jan-2021</Created Date>
		private readonly BLMisc objBLMisc;
		public DataTable dataTable;
		public LibraryController()
		{
			//objCommon = new Common();
			objBLMisc = new BLMisc();
		}

		[HttpPost]
		public dynamic EntityOperations(ReqInput data)
		{
			var response = new ApiResponse<dynamic>();
			HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
			dynamic objIn = ReqHelper.GetRequestData<dynamic>(data);
			if (headerAttribute.source.ToUpper() != "MOBILE") 
			{
				if (Convert.ToString(objIn.source_ref_id) != "" && Convert.ToString(objIn.source_ref_type) != "")
				{
					headerAttribute.source_ref_id = Convert.ToString(objIn.source_ref_id);
					headerAttribute.source_ref_type = Convert.ToString(objIn.source_ref_type);
				}
			}

			data.data = ReqHelper.MergeHeaderAttributeInJsonObject(data, headerAttribute);
			Api_Logs objApiLogs = ReqHelper.GetRequestData<Api_Logs>(data);
			objApiLogs.attribute_info = data.data;
			objApiLogs.header_attribute = new JavaScriptSerializer().Serialize(headerAttribute);
			if (ApplicationSettings.isApiRequestLogRequired)
			{
				var result = new BLMisc().insertApiLogs(objApiLogs);
			}
			this.Validate(headerAttribute);
			if (ModelState.IsValid)
			{

				EntityInfo objEntityInfo = ReqHelper.GetRequestData<EntityInfo>(data);
				if (headerAttribute.source.ToUpper() != "MOBILE" || (headerAttribute.source_ref_id != "0" && headerAttribute.source_ref_type.ToUpper() != "NETWORK_TICKET") || objEntityInfo.system_id == 0 || headerAttribute.is_new_entity)
				{
					if (headerAttribute.structure_id > 0)
					{
						#region POD
						if (headerAttribute.entity_type.ToUpper() == EntityType.POD.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPPod(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPPod(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region MPOD
						else if (headerAttribute.entity_type.ToUpper() == EntityType.MPOD.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPMpod(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPMpod(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region ONT
						else if (headerAttribute.entity_type.ToUpper() == EntityType.ONT.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPONT(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPONT(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region HTB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.HTB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPHtb(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPHtb(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region FDB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.FDB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPFdb(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPFdb(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region BDB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.BDB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPBdb(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPBdb(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region FMS
						else if (headerAttribute.entity_type.ToUpper() == EntityType.FMS.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPFMS(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPFMS(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region OpticalRepeater
						else if (headerAttribute.entity_type.ToUpper() == EntityType.OpticalRepeater.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPOpticalRepeater(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPOpticalRepeater(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Cabinet
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Cabinet.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPCabinet(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPCabinet(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region SpliceClosure
						else if (headerAttribute.entity_type.ToUpper() == EntityType.SpliceClosure.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPSpliceClosure(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPSC(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region ADB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.ADB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPAdb(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPAdb(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region CDB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.CDB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPCdb(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPCdb(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region PatchPanel
						else if (headerAttribute.entity_type.ToUpper() == EntityType.PatchPanel.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddISPPatchPanel(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveISPPatchPanel(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
					}
					else
					{
						#region Pole
						if (headerAttribute.entity_type.ToUpper() == EntityType.Pole.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddPole(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SavePole(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Manhole
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Manhole.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddManhole(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveManhole(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Tree
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Tree.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddTree(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveTree(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region wallMount
						else if (headerAttribute.entity_type.ToUpper() == EntityType.WallMount.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddWallMount(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveWallmount(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region POD
						else if (headerAttribute.entity_type.ToUpper() == EntityType.POD.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddPod(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SavePod(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region MPOD
						else if (headerAttribute.entity_type.ToUpper() == EntityType.MPOD.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddMpod(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveMpod(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region spliceclosure
						else if (headerAttribute.entity_type.ToUpper() == EntityType.SpliceClosure.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddSpliceClosure(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveSpliceClosure(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Cable
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Cable.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddCable(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveCable(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region ADB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.ADB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddADB(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveADB(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Splitter
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Splitter.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddSplitter(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveSplitter(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Area
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Area.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddArea(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveArea(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region SurveyArea
						else if (headerAttribute.entity_type.ToUpper() == EntityType.SurveyArea.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddSurveyArea(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveSurveyArea(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion

						#region RestrictedArea
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Restricted_Area.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddRestrictedArea(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveRestrictedArea(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region ONT
						else if (headerAttribute.entity_type.ToUpper() == EntityType.ONT.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddONT(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveONT(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region FDB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.FDB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddFDB(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveFDB(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region BDB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.BDB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddBdb(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveBdb(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region CDB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.CDB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddCdb(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveCdb(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region FMS
						else if (headerAttribute.entity_type.ToUpper() == EntityType.FMS.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddFMS(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveFMS(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region HTB
						else if (headerAttribute.entity_type.ToUpper() == EntityType.HTB.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddHTB(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveHTB(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Building
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Building.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddBuilding(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveBuilding(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Structure
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Structure.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddStructure(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveStructure(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region CSA
						else if (headerAttribute.entity_type.ToUpper() == EntityType.CSA.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddCSA(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveCSA(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region DSA
						else if (headerAttribute.entity_type.ToUpper() == EntityType.DSA.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddDSA(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveDSA(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region SubArea
						else if (headerAttribute.entity_type.ToUpper() == EntityType.SubArea.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddSubArea(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveSubArea(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Coupler
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Coupler.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddCoupler(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveCoupler(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Fault
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Fault.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddFault(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveFault(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Competitor
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Competitor.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddCompetitor(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveCompetitor(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Trench
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Trench.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddTrench(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveTrench(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Duct
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Duct.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddDuct(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveDuct(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
                        #endregion
                        #region Microductuct
                        else if (headerAttribute.entity_type.ToUpper() == EntityType.Microduct.ToString().ToUpper())
                        {
                            if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                            {
                                return AddMicroduct(data);
                            }
                            else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                            {
                                return SaveMicroduct(data);
                            }
                            else
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = "Entity_Action not matched";
                                return response;
                            }
                        }
                        #endregion
                        else if (headerAttribute.entity_type.ToUpper() == EntityType.Gipipe.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddGipipe(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveGipipe(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#region Conduit
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Conduit.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddConduit(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveConduit(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region Province
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Province.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddProvince(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveProvince(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Delete.ToString().ToUpper())
							{
								return DeleteProvince(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						#region OpticalRepeater
						else if (headerAttribute.entity_type.ToUpper() == EntityType.OpticalRepeater.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddOpticalRepeater(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveOpticalRepeater(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Cabinet.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddCabinet(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveCabinet(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}

						else if (headerAttribute.entity_type.ToUpper() == EntityType.Vault.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddVault(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveVault(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						//vault shazia end 

						else if (headerAttribute.entity_type.ToUpper() == EntityType.Loop.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddLoop(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveLoop(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#region Tower
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Tower.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddTower(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveTower(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion

						//HANDHOLE ENTITY BY ANTRA
						#region Handhole
						else if (headerAttribute.entity_type.ToUpper() == EntityType.Handhole.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddHandhole(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SaveHandhole(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
						#endregion

						//PatchPanel
						#region PatchPanel
						else if (headerAttribute.entity_type.ToUpper() == EntityType.PatchPanel.ToString().ToUpper())
						{
							if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
							{
								return AddPatchPanel(data);
							}
							else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
							{
								return SavePatchPanel(data);
							}
							else
							{
								response.status = ResponseStatus.FAILED.ToString();
								response.error_message = "Entity_Action not matched";
								return response;
							}
						}
                        #endregion
                        //
						//SLACK BY ANTRA

                        else if (headerAttribute.entity_type.ToUpper() == EntityType.Slack.ToString().ToUpper())
                        {
                            if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                            {
                                return AddSlack(data);
                            }
                            else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                            {
                                return SaveSlack(data);
                            }
                            else
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = "Entity_Action not matched";
                                return response;
                            }
                        }
						//Site by Pawan
                        else if (headerAttribute.entity_type.ToUpper() == EntityType.Site.ToString().ToUpper())
                        {
                            if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                            {
                                return AddSite(data);
                            }
                            else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                            {
                                return SaveSite(data);
                            }
                            else
                            {
                                response.status = ResponseStatus.FAILED.ToString();
                                response.error_message = "Entity_Action not matched";
                                return response;
                            }
                        }

                    }
				}
				else if (objEntityInfo.system_id > 0)// headerAttribute.entity_action.ToUpper() == EntityAction.Update.ToString().ToUpper())
				{
					return EditEntityInfo(data, headerAttribute, "Edit Detail");
				}
				else
				{
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = "Invalid input request.";
					return response;
				}
			}
			else
			{
				response.status = ResponseStatus.FAILED.ToString();
				response.error_message = getFirstErrorFromModelState();
				return response;
			}
			return response;

		}
		#endregion

		#region Pole
		#region Add Pole
		/// <summary> Add Pole </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Pole Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<PoleMaster> AddPole(ReqInput data)
		{
			var response = new ApiResponse<PoleMaster>();
			try
			{
				PoleMaster objRequestIn = ReqHelper.GetRequestData<PoleMaster>(data);
				PoleMaster objPoleMaster = GetPoleDetail(objRequestIn);
				BindPoleDropDown(objPoleMaster);
                BindPoleRoute(objPoleMaster);
                List<int> listI = new List<int>();
                foreach (var i in objPoleMaster.lstRouteInfo.Where(x => x.is_associated))
                {
                    listI.Add(i.cable_id);
                }
                objPoleMaster.selected_route_ids = listI;
                BLItemTemplate.Instance.BindItemDropdowns(objPoleMaster, EntityType.Pole.ToString());
				fillProjectSpecifications(objPoleMaster);
				objPoleMaster.is_specification_allowed = new BLLayer().GetSpecificationAllowed(EntityType.Manhole.ToString()).ToString();
				//Get the layer details to bind additional attributes Pole
				var layerdetails = new BLLayer().getLayer(EntityType.Pole.ToString());
				objPoleMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Pole
				response.status = StatusCodes.OK.ToString();
				response.results = objPoleMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddPole()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get Pole Details
		/// <summary> GetPoleDetail</summary>
		/// <param >networkIdType,systemId,geom,userId</param>
		/// <returns>Pole Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public PoleMaster GetPoleDetail(PoleMaster objPole)
		{
			int user_id = objPole.user_id;
			if (objPole.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objPole, GeometryType.Point.ToString(), objPole.geom);
				//Fill Parent detail...              
				fillParentDetail(objPole, new NetworkCodeIn() { eType = EntityType.Pole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPole.geom }, objPole.networkIdType);
				objPole.longitude = Convert.ToDouble(objPole.geom.Split(' ')[0]);
				objPole.latitude = Convert.ToDouble(objPole.geom.Split(' ')[1]);
				objPole.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<PoleTemplateMaster>(objPole.user_id, EntityType.Pole);
				MiscHelper.CopyMatchingProperties(objItem, objPole);
				//For Additional-Attributes
				objPole.other_info = null;
			}
			else
			{
				// Get entity detail by Id...
				objPole = new BLMisc().GetEntityDetailById<PoleMaster>(objPole.system_id, EntityType.Pole, objPole.user_id);
				objPole.other_info = new BLPole().GetOtherInfoPole(objPole.system_id);
				fillRegionProvAbbr(objPole);
			}
			objPole.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objPole;
		}
		#endregion

		#region Save Pole
		/// <summary> SavePole </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<PoleMaster> SavePole(ReqInput data)
		{
			var response = new ApiResponse<PoleMaster>();
			PoleMaster objPoleMaster = ReqHelper.GetRequestData<PoleMaster>(data);
			try
			{
				if (objPoleMaster.networkIdType == NetworkIdType.A.ToString() && objPoleMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Pole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPoleMaster.geom, parent_eType = objPoleMaster.pEntityType, parent_sysId = objPoleMaster.pSystemId });                
					if (objPoleMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objPoleMaster = GetPoleDetail(objPoleMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objPoleMaster.pole_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objPoleMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objPoleMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objPoleMaster.network_id = objNetworkCodeDetail.network_code;
					objPoleMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objPoleMaster.pole_name))
				{
					objPoleMaster.pole_name = objPoleMaster.network_id;
				}
				this.Validate(objPoleMaster);
				if (ModelState.IsValid)
				{
					var isNew = objPoleMaster.system_id > 0 ? false : true;
					objPoleMaster.is_new_entity = (isNew && objPoleMaster.source_ref_id != "0" && objPoleMaster.source_ref_id != "");

                    BindPoleRoute(objPoleMaster);
                    var resultItem = new BLPole().SaveEntityPole(objPoleMaster, objPoleMaster.user_id);
                    List<RouteInfo> objL = new List<RouteInfo>();
                    foreach (var itm in objPoleMaster.lstRouteInfo)
                    {
                        bool f = false;
						if (objPoleMaster.selected_route_ids != null)
						{
							foreach (var ids in objPoleMaster.selected_route_ids)
							{
								if (ids == itm.cable_id)
								{
									f = true;

								}
							}
						}
                        RouteInfo objS = new RouteInfo();
                        objS.entity_id = resultItem.system_id;
                        objS.entity_type = EntityType.Pole.ToString();
                        objS.created_by = objPoleMaster.user_id.ToString();
                        objS.cable_id = itm.cable_id;
                        objS.is_associated = f;
                        objL.Add(objS);
                    }
                    var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objL), resultItem.system_id, EntityType.Pole.ToString(), objPoleMaster.user_id);
                    BindPoleRoute(objPoleMaster);
                    List<int> listI = new List<int>();
                    foreach (var i in objPoleMaster.lstRouteInfo.Where(x => x.is_associated))
                    {
                        listI.Add(i.cable_id);
                    }
                    objPoleMaster.selected_route_ids = listI;
                    if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						string[] LayerName = { EntityType.Pole.ToString() };
						if (objPoleMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objPoleMaster.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							objPoleMaster.objPM.status = ResponseStatus.OK.ToString();
							objPoleMaster.objPM.isNewEntity = isNew;
							objPoleMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
							response.error_message = objPoleMaster.objPM.message;
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							BLLoopMangment.Instance.UpdateLoopDetails(objPoleMaster.system_id, "Pole", objPoleMaster.network_id, objPoleMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPoleMaster.longitude + " " + objPoleMaster.latitude }, objPoleMaster.user_id);
							objPoleMaster.objPM.status = ResponseStatus.OK.ToString();
							objPoleMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = objPoleMaster.objPM.message;
							response.status = ResponseStatus.OK.ToString();
						}
					}
					else
					{
						objPoleMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
						objPoleMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					}
				}
				else
				{
					objPoleMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objPoleMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();

				}
				if (objPoleMaster.isDirectSave == true)
				{
					response.results = objPoleMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objPoleMaster, EntityType.Pole.ToString());
					BindPoleDropDown(objPoleMaster);
                    BindPoleRoute(objPoleMaster);
                    fillProjectSpecifications(objPoleMaster);
					//Get the layer details to bind additional attributes Pole
					var layerdetails = new BLLayer().getLayer(EntityType.Pole.ToString());
					objPoleMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Pole
					response.results = objPoleMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SavePole()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind Pole Dropdown
		/// <summary>BindPoleDropDown </summary>
		/// <param name="objPoleMaster"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindPoleDropDown(PoleMaster objPoleMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Pole.ToString());
			objPoleMaster.lstPoleType = objDDL.Where(x => x.dropdown_type == DropDownType.Pole_Type.ToString()).ToList();
			objPoleMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objPoleMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList("");
			objPoleMaster.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
           
            //objPoleMaster.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        }
		private void BindPoleRoute(PoleMaster objPoleMaster)
		{
            if (objPoleMaster.system_id == 0)
                objPoleMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objPoleMaster.geom);
            else
                objPoleMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objPoleMaster.system_id, EntityType.Pole.ToString());            
        }
		#endregion

		#endregion

		#region Manhole

		#region Add Manhole
		/// <summary> Add Manhole </summary>
		/// <param name="data">json data</param>
		/// <returns>Manhole Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		///<Created Date>11-Jan-2020</Created Date>
		public ApiResponse<ManholeMaster> AddManhole(ReqInput data)
		{
			var response = new ApiResponse<ManholeMaster>();
			try
			{
				ManholeMaster objRequestIn = ReqHelper.GetRequestData<ManholeMaster>(data);
				ManholeMaster objManholeMaster = GetManholeDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objManholeMaster, EntityType.Manhole.ToString());
				fillProjectSpecifications(objManholeMaster);
				BindManholeDropDown(objManholeMaster);
                BindManholeRoute(objManholeMaster);
                List<int> listI = new List<int>();
                foreach (var i in objManholeMaster.lstRouteInfo.Where(x => x.is_associated))
                {
                    listI.Add(i.cable_id);
                }
                objManholeMaster.selected_route_ids = listI;
                objManholeMaster.is_specification_allowed = new BLLayer().GetSpecificationAllowed(EntityType.Manhole.ToString()).ToString();
				objManholeMaster.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Manhole.ToString()).ToList();
				//Get the layer details to bind additional attributes Manhole
				var layerdetails = new BLLayer().getLayer(EntityType.Manhole.ToString());
				objManholeMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Manhole
				response.status = StatusCodes.OK.ToString();
				response.results = objManholeMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddManhole()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Save Manhole
		/// <summary> SaveManhole </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<ManholeMaster> SaveManhole(ReqInput data)
		{
			var response = new ApiResponse<ManholeMaster>();
			ManholeMaster objManholeMaster = ReqHelper.GetRequestData<ManholeMaster>(data);
			try
			{
				if (objManholeMaster.networkIdType == NetworkIdType.A.ToString() && objManholeMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Manhole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objManholeMaster.geom, parent_eType = objManholeMaster.pEntityType, parent_sysId = objManholeMaster.pSystemId });
					if (objManholeMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objManholeMaster = GetManholeDetail(objManholeMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objManholeMaster.manhole_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objManholeMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objManholeMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objManholeMaster.network_id = objNetworkCodeDetail.network_code;
					objManholeMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objManholeMaster.manhole_name))
				{
					objManholeMaster.manhole_name = objManholeMaster.network_id;
				}
                BLItemTemplate.Instance.BindItemDropdowns(objManholeMaster, EntityType.Manhole.ToString());
                for (int i = 0; i < objManholeMaster.lstAccessibility.Count; i++)
                {
                    if (objManholeMaster.lstAccessibility[i].key == "No")
                        objManholeMaster.accessibility = objManholeMaster.is_buried == true ? Convert.ToInt16(objManholeMaster.lstAccessibility[i].value) : objManholeMaster.accessibility;
                }
                this.Validate(objManholeMaster);
				if (ModelState.IsValid)
				{
					var isNew = objManholeMaster.system_id > 0 ? false : true;
					objManholeMaster.is_new_entity = (isNew && objManholeMaster.source_ref_id != "0" && objManholeMaster.source_ref_id != "");
					var resultItem = new BLManhole().SaveEntityManhole(objManholeMaster, objManholeMaster.user_id);                   
                    BindManholeRoute(objManholeMaster);                    
                    List<RouteInfo> objL = new List<RouteInfo>();
                    foreach (var itm in objManholeMaster.lstRouteInfo)
                    {
                        bool f = false;
						if (objManholeMaster.selected_route_ids != null)
						{
							foreach (var ids in objManholeMaster.selected_route_ids)
							{
								if (ids == itm.cable_id)
								{
									f = true;

								}
							}
						}
                        RouteInfo objS = new RouteInfo();
                        objS.entity_id = resultItem.system_id;
                        objS.entity_type = EntityType.Manhole.ToString();
                        objS.created_by = objManholeMaster.user_id.ToString();
                        objS.cable_id = itm.cable_id;
                        objS.is_associated = f;
                        objL.Add(objS);
                    }

                    var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objL), resultItem.system_id, EntityType.Manhole.ToString(), objManholeMaster.user_id);
                    BindManholeRoute(objManholeMaster);
                    List<int> listI = new List<int>();
                    foreach (var i in objManholeMaster.lstRouteInfo.Where(x => x.is_associated))
                    {
                        listI.Add(i.cable_id);
                    }
                    objManholeMaster.selected_route_ids = listI;
                    if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						string[] LayerName = { EntityType.Manhole.ToString() };
						if (objManholeMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objManholeMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objManholeMaster.objPM.status = ResponseStatus.OK.ToString();
							objManholeMaster.objPM.isNewEntity = isNew;
							objManholeMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = objManholeMaster.objPM.message;
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							BLLoopMangment.Instance.UpdateLoopDetails(objManholeMaster.system_id, EntityType.Manhole.ToString(), objManholeMaster.network_id, objManholeMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objManholeMaster.longitude + " " + objManholeMaster.latitude }, objManholeMaster.user_id);
							objManholeMaster.objPM.status = ResponseStatus.OK.ToString();
							objManholeMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = objManholeMaster.objPM.message;
							response.status = ResponseStatus.OK.ToString();
						}
						objManholeMaster.objPM = objManholeMaster.objPM;
						//save AT Status                        
						if (objManholeMaster.ATAcceptance != null && objManholeMaster.system_id > 0)
						{
							SaveATAcceptance(objManholeMaster.ATAcceptance, objManholeMaster.system_id, objManholeMaster.user_id);
						}
					}
					else
					{
						objManholeMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objManholeMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objManholeMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objManholeMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objManholeMaster.isDirectSave == true)
				{
					response.results = objManholeMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objManholeMaster, EntityType.Manhole.ToString());
                    fillProjectSpecifications(objManholeMaster);
					BindManholeDropDown(objManholeMaster);
					objManholeMaster.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Manhole.ToString()).ToList();
					//Get the layer details to bind additional attributes Manhole
					var layerdetails = new BLLayer().getLayer(EntityType.Manhole.ToString());
					objManholeMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Manhole
					response.results = objManholeMaster;

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveManhole()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion

		#region Bind Manhole Dropdown
		/// <summary>Bind Manhole DropDown </summary>
		/// <param name="objPoleMaster"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindManholeDropDown(ManholeMaster objManholeMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Manhole.ToString());
            objManholeMaster.listConstructionType = objDDL.Where(x => x.dropdown_type == DropDownType.Construction_Type.ToString()).ToList();
			objManholeMaster.lstManholeType = objDDL.Where(x => x.dropdown_type == DropDownType.Manhole_types.ToString()).ToList();
            objManholeMaster.MCGMWardIn = objDDL.Where(x => x.dropdown_type == DropDownType.MCGM_Ward.ToString()).ToList();
            objManholeMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objManholeMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList("");
			objManholeMaster.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			objManholeMaster.listaerialLocation = objDDL.Where(x => x.dropdown_type == DropDownType.Aerial_Location.ToString()).ToList();
          
            //objManholeMaster.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        }
        private void BindManholeRoute(ManholeMaster objManholeMaster)
		{
            if (objManholeMaster.system_id == 0)
                objManholeMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objManholeMaster.geom);
            else
                objManholeMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objManholeMaster.system_id, EntityType.Manhole.ToString());
        }
        #endregion

        #region Get Manhole Details
        /// <summary> Get Manhole </summary>
        /// <returns>Get Manhole Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        ///<Created Date>11-Jan-2020</Created Date>
        public ManholeMaster GetManholeDetail(ManholeMaster objManhole)
		{
			int id = objManhole.user_id;
			if (objManhole.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objManhole, GeometryType.Point.ToString(), objManhole.geom);
				//Fill Parent detail...              
				fillParentDetail(objManhole, new NetworkCodeIn() { eType = EntityType.Manhole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objManhole.geom }, objManhole.networkIdType);
				objManhole.longitude = Convert.ToDouble(objManhole.geom.Split(' ')[0]);
				objManhole.latitude = Convert.ToDouble(objManhole.geom.Split(' ')[1]);
				objManhole.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<ManholeTemplateMaster>(objManhole.user_id, EntityType.Manhole);
				MiscHelper.CopyMatchingProperties(objItem, objManhole);
				objManhole.other_info = null;   //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objManhole = new BLMisc().GetEntityDetailById<ManholeMaster>(objManhole.system_id, EntityType.Manhole, objManhole.user_id);
				//for additional-attributes
				objManhole.other_info = new BLManhole().GetOtherInfoManhole(objManhole.system_id);
				fillRegionProvAbbr(objManhole);
			}
			objManhole.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());

			return objManhole;
		}
		#endregion

		#endregion

		#region Tree

		#region Add Tree
		/// <summary> Add Tree </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Tree Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<TreeMaster> AddTree(ReqInput data)
		{
			var response = new ApiResponse<TreeMaster>();
			try
			{
				TreeMaster objRequestIn = ReqHelper.GetRequestData<TreeMaster>(data);
				TreeMaster objTreeMaster = GetTreeDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objTreeMaster, EntityType.Tree.ToString());
				fillProjectSpecifications(objTreeMaster);
				//Get the layer details to bind additional attributes Tree
				var layerdetails = new BLLayer().getLayer(EntityType.Tree.ToString());
				objTreeMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Tree
				response.status = StatusCodes.OK.ToString();
				response.results = objTreeMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddTree()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get Tree Detail
		/// <summary> Get Tree Detail </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Tree Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public TreeMaster GetTreeDetail(TreeMaster objTree)
		{
			int user_id = objTree.user_id;
			if (objTree.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objTree, GeometryType.Point.ToString(), objTree.geom);
				//Fill Parent detail...              
				fillParentDetail(objTree, new NetworkCodeIn() { eType = EntityType.Tree.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTree.geom }, objTree.networkIdType);
				objTree.longitude = Convert.ToDouble(objTree.geom.Split(' ')[0]);
				objTree.latitude = Convert.ToDouble(objTree.geom.Split(' ')[1]);
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<TreeTemplateMaster>(objTree.user_id, EntityType.Tree);
				MiscHelper.CopyMatchingProperties(objItem, objTree);
				objTree.other_info = null;  //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objTree = new BLMisc().GetEntityDetailById<TreeMaster>(objTree.system_id, EntityType.Tree, objTree.user_id);
				//for additional-attributes
				objTree.other_info = new BLTree().GetOtherInfoTree(objTree.system_id);
				fillRegionProvAbbr(objTree);
			}
			objTree.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objTree;
		}
		#endregion

		#region Save Tree
		/// <summary> SaveTree </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<TreeMaster> SaveTree(ReqInput data)
		{
			var response = new ApiResponse<TreeMaster>();
			TreeMaster objTreeMaster = ReqHelper.GetRequestData<TreeMaster>(data);
			try
			{
				if (objTreeMaster.networkIdType == NetworkIdType.A.ToString() && objTreeMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Tree.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTreeMaster.geom, parent_eType = objTreeMaster.pEntityType, parent_sysId = objTreeMaster.pSystemId });
					if (objTreeMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objTreeMaster = GetTreeDetail(objTreeMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objTreeMaster.tree_name = objNetworkCodeDetail.network_code;
					}
					//SET NETWORK CODE
					objTreeMaster.network_id = objNetworkCodeDetail.network_code;
					objTreeMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objTreeMaster.tree_name))
				{
					objTreeMaster.tree_name = objTreeMaster.network_id;
				}
				this.Validate(objTreeMaster);
				if (ModelState.IsValid)
				{
					var isNew = objTreeMaster.system_id > 0 ? false : true;
					objTreeMaster.is_new_entity = (isNew && objTreeMaster.source_ref_id != "0" && objTreeMaster.source_ref_id != "");
					var resultItem = new BLTree().SaveEntityTree(objTreeMaster, objTreeMaster.user_id);
					string[] LayerName = { EntityType.Tree.ToString() };
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						if (objTreeMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objTreeMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objTreeMaster.objPM.status = ResponseStatus.OK.ToString();
							objTreeMaster.objPM.isNewEntity = isNew;
							objTreeMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							BLLoopMangment.Instance.UpdateLoopDetails(objTreeMaster.system_id, "Tree", objTreeMaster.network_id, objTreeMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTreeMaster.longitude + " " + objTreeMaster.latitude }, objTreeMaster.user_id);
							objTreeMaster.objPM.status = ResponseStatus.OK.ToString();
							objTreeMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objTreeMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objTreeMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
					objTreeMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objTreeMaster.objPM.message = getFirstErrorFromModelState();
				}
				if (objTreeMaster.isDirectSave == true)
				{
					response.results = objTreeMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objTreeMaster, EntityType.Tree.ToString());
					fillProjectSpecifications(objTreeMaster);
					//Get the layer details to bind additional attributes Tree
					var layerdetails = new BLLayer().getLayer(EntityType.Tree.ToString());
					objTreeMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Tree
					response.results = objTreeMaster;
				}

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveTree()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#endregion

		#region Wallmount

		#region Add Wallmount
		/// <summary> Add Wallmount </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Wallmount Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<WallMountMaster> AddWallMount(ReqInput data)
		{
			var response = new ApiResponse<WallMountMaster>();
			try
			{
				WallMountMaster objRequestIn = ReqHelper.GetRequestData<WallMountMaster>(data);
				WallMountMaster objWallMountMaster = GetWallMountDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objWallMountMaster, EntityType.WallMount.ToString());
				BindWallMountDropDown(objWallMountMaster);
				fillProjectSpecifications(objWallMountMaster);
				//Get the layer details to bind additional attributes WallMount
				var layerdetails = new BLLayer().getLayer(EntityType.WallMount.ToString());
				objWallMountMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes WallMount
				response.status = StatusCodes.OK.ToString();
				response.results = objWallMountMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddWallMount()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind Wallmount Dropdown
		/// <summary> Bind Wallmount Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>Wallmount Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindWallMountDropDown(WallMountMaster objWallMountMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.WallMount.ToString());
			//objWallMountMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objWallMountMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objWallMountMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList("");
			objWallMountMaster.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//objWallMountMaster.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Get Wallmount Detail
		/// <summary> Get Wallmount Detail </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Wallmount Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public WallMountMaster GetWallMountDetail(WallMountMaster objWallMount)
		{
			int user_id = objWallMount.user_id;
			if (objWallMount.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objWallMount, GeometryType.Point.ToString(), objWallMount.geom);
				//Fill Parent detail...              
				fillParentDetail(objWallMount, new NetworkCodeIn() { eType = EntityType.WallMount.ToString(), gType = GeometryType.Point.ToString(), eGeom = objWallMount.geom }, objWallMount.networkIdType);
				objWallMount.longitude = Convert.ToDouble(objWallMount.geom.Split(' ')[0]);
				objWallMount.latitude = Convert.ToDouble(objWallMount.geom.Split(' ')[1]);
				objWallMount.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<WallMountTemplateMaster>(objWallMount.user_id, EntityType.WallMount);
				MiscHelper.CopyMatchingProperties(objItem, objWallMount);
				objWallMount.other_info = null; //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objWallMount = new BLMisc().GetEntityDetailById<WallMountMaster>(objWallMount.system_id, EntityType.WallMount, objWallMount.user_id);
				//for additional-attributes
				objWallMount.other_info = new BLWallMount().GetOtherInfoWallMount(objWallMount.system_id);
				fillRegionProvAbbr(objWallMount);
			}
			objWallMount.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objWallMount;
		}
		#endregion

		#region Save Wallmount
		/// <summary> SaveWallmount </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<WallMountMaster> SaveWallmount(ReqInput data)
		{
			var response = new ApiResponse<WallMountMaster>();
			WallMountMaster objWallMountMaster = ReqHelper.GetRequestData<WallMountMaster>(data);
			try
			{
				if (objWallMountMaster.networkIdType == NetworkIdType.A.ToString() && objWallMountMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.WallMount.ToString(), gType = GeometryType.Point.ToString(), eGeom = objWallMountMaster.geom, parent_eType = objWallMountMaster.pEntityType, parent_sysId = objWallMountMaster.pSystemId });
					if (objWallMountMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objWallMountMaster = GetWallMountDetail(objWallMountMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objWallMountMaster.wallmount_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objWallMountMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objWallMountMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objWallMountMaster.network_id = objNetworkCodeDetail.network_code;
					objWallMountMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objWallMountMaster.wallmount_name))
				{
					objWallMountMaster.wallmount_name = objWallMountMaster.network_id;
				}
				this.Validate(objWallMountMaster);
				if (ModelState.IsValid)
				{
					var isNew = objWallMountMaster.system_id > 0 ? false : true;
					objWallMountMaster.is_new_entity = (isNew && objWallMountMaster.source_ref_id != "0" && objWallMountMaster.source_ref_id != "");
					var resultItem = new BLWallMount().SaveEntityWallMount(objWallMountMaster, objWallMountMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.WallMount.ToString() };
						//Save Reference
						if (objWallMountMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objWallMountMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objWallMountMaster.objPM.status = ResponseStatus.OK.ToString();
							objWallMountMaster.objPM.isNewEntity = isNew;
							objWallMountMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
						}
						else
						{
							BLLoopMangment.Instance.UpdateLoopDetails(objWallMountMaster.system_id, "WallMount", objWallMountMaster.network_id, objWallMountMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objWallMountMaster.longitude + " " + objWallMountMaster.latitude }, objWallMountMaster.user_id);
							objWallMountMaster.objPM.status = ResponseStatus.OK.ToString();
							objWallMountMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
						objWallMountMaster.objPM = objWallMountMaster.objPM;
					}
					else
					{
						objWallMountMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objWallMountMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objWallMountMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objWallMountMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objWallMountMaster.isDirectSave == true)
				{
					response.results = objWallMountMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objWallMountMaster, EntityType.WallMount.ToString());
					BindWallMountDropDown(objWallMountMaster);
					fillProjectSpecifications(objWallMountMaster);
					//Get the layer details to bind additional attributes WallMount
					var layerdetails = new BLLayer().getLayer(EntityType.WallMount.ToString());
					objWallMountMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes WallMount
					response.results = objWallMountMaster;
				}

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveWallmount()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}


		#endregion
		#endregion

		#region Pod

		#region Pod Details
		/// <summary> Get Pod Details</summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Pod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public PODMaster GetPODDetail(PODMaster objPOD)
		{
			int user_id = objPOD.user_id;
			if (objPOD.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				objPOD.ownership_type = "Own";
				fillRegionProvinceDetail(objPOD, GeometryType.Point.ToString(), objPOD.geom);
				//Fill Parent detail...              
				fillParentDetail(objPOD, new NetworkCodeIn() { eType = EntityType.POD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPOD.geom }, objPOD.networkIdType);
				objPOD.longitude = Convert.ToDouble(objPOD.geom.Split(' ')[0]);
				objPOD.latitude = Convert.ToDouble(objPOD.geom.Split(' ')[1]);
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<PODTemplateMaster>(objPOD.user_id, EntityType.POD);
				MiscHelper.CopyMatchingProperties(objItem, objPOD);
				objPOD.other_info = null;   //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objPOD = new BLMisc().GetEntityDetailById<PODMaster>(objPOD.system_id, EntityType.POD, objPOD.user_id);
				//for additional-attributes
				objPOD.other_info = new BLPOD().GetOtherInfoPOD(objPOD.system_id);
				fillRegionProvAbbr(objPOD);
			}
			objPOD.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objPOD;
		}

		#endregion

		#region Add Pod
		/// <summary> Add Pod </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Pod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<PODMaster> AddPod(ReqInput data)
		{
			var response = new ApiResponse<PODMaster>();
			try
			{
				PODMaster objRequestIn = ReqHelper.GetRequestData<PODMaster>(data);
				PODMaster objPODMaster = GetPODDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objPODMaster, EntityType.POD.ToString());
				fillProjectSpecifications(objPODMaster);
				//if (objPODMaster.parent_system_id > 0)
				//{
				//    objPODMaster.objIspEntityMap.structure_id = objPODMaster.parent_system_id;
				//    objPODMaster.objIspEntityMap.AssociateStructure = objPODMaster.parent_system_id;
				//}
				BindPODDropDown(objPODMaster);
				//Get the layer details to bind additional attributes POD
				var layerdetails = new BLLayer().getLayer(EntityType.POD.ToString());
				objPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes POD
				response.status = StatusCodes.OK.ToString();
				response.results = objPODMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddPod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind Pod Dropdown
		/// <summary> Bind Pod Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>Pod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindPODDropDown(PODMaster objPOD)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objPOD.parent_system_id, objPOD.system_id, EntityType.POD.ToString());
			if (ispEntityMap != null)
			{
				objPOD.objIspEntityMap.id = ispEntityMap.id;
				objPOD.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objPOD.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objPOD.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objPOD.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}

			objPOD.objIspEntityMap.AssoType = objPOD.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objPOD.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objPOD.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objPOD.longitude + " " + objPOD.latitude);

			if (objPOD.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objPOD.objIspEntityMap.structure_id);
				objPOD.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objPOD.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objPOD.parent_entity_type == EntityType.UNIT.ToString())
				{
					objPOD.objIspEntityMap.unitId = objPOD.parent_system_id;
					//objPOD.objIspEntityMap.AssoType = "";
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.POD.ToString()).FirstOrDefault() != null)
			{
				objPOD.objIspEntityMap.isValidParent = true;
				objPOD.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objPOD.objIspEntityMap.structure_id, objPOD.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objPOD.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objPOD.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objPOD.objIspEntityMap.entity_type == null) { objPOD.objIspEntityMap.entity_type = EntityType.POD.ToString(); }
			//objPOD.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			var obj_DDL = new BLMisc().GetDropDownList(EntityType.POD.ToString());
			objPOD.listPODType = obj_DDL.Where(x => x.dropdown_type == DropDownType.POD_Type.ToString()).ToList();
			objPOD.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objPOD.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objDDL_ = new BLMisc().GetDropDownList("");
			objPOD.lstBOMSubCategory = objDDL_.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objPOD.lstServedByRing = objDDL_.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save Pod
		/// <summary> SavePod </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<PODMaster> SavePod(ReqInput data)
		{
			var response = new ApiResponse<PODMaster>();
			PODMaster objPODMaster = ReqHelper.GetRequestData<PODMaster>(data);
			try
			{
				if (objPODMaster.networkIdType == NetworkIdType.A.ToString() && objPODMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.POD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPODMaster.geom, parent_eType = objPODMaster.pEntityType, parent_sysId = objPODMaster.pSystemId });
					if (objPODMaster.isDirectSave == true)
					{
						if (objPODMaster.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
						{
							objPODMaster.objIspEntityMap.structure_id = objPODMaster.parent_system_id;
						}
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objPODMaster = GetPODDetail(objPODMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objPODMaster.pod_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objPODMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objPODMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objPODMaster.network_id = objNetworkCodeDetail.network_code;
					objPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;

				}
				if (string.IsNullOrEmpty(objPODMaster.pod_name))
				{
					objPODMaster.pod_name = objPODMaster.network_id;
				}
				this.Validate(objPODMaster);
				if (ModelState.IsValid)
				{

					var layer_title = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault().layer_title;
					objPODMaster.objIspEntityMap.structure_id = Convert.ToInt32(objPODMaster.objIspEntityMap.AssociateStructure);
					if (objPODMaster.objIspEntityMap.structure_id != 0)
					{
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objPODMaster.objIspEntityMap.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							objPODMaster.latitude = Convert.ToDouble(structureDetails.latitude);
							objPODMaster.longitude = Convert.ToDouble(structureDetails.longitude);
						}
					}
					objPODMaster.objIspEntityMap.shaft_id = objPODMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objPODMaster.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(objPODMaster.objIspEntityMap.AssoType))
					{
						objPODMaster.objIspEntityMap.shaft_id = 0; objPODMaster.objIspEntityMap.floor_id = 0;
					}
					if (objPODMaster.objIspEntityMap.structure_id == 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.POD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPODMaster.longitude + " " + objPODMaster.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							objPODMaster.parent_system_id = parentDetails.p_system_id;
							objPODMaster.parent_network_id = parentDetails.p_network_id;
							objPODMaster.parent_entity_type = parentDetails.p_entity_type;
						}
					}
					//if (objPODMaster.objIspEntityMap.structure_id != 0)
					//{
					//    objPODMaster.parent_system_id = Convert.ToInt32(objPODMaster.objIspEntityMap.structure_id);
					//    objPODMaster.parent_entity_type = EntityType.Structure.ToString();
					//    objPODMaster.latitude = objPODMaster.latitude;
					//    objPODMaster.longitude = objPODMaster.longitude;
					//}
					//else
					//{
					//    objPODMaster.parent_system_id = 0;
					//    objPODMaster.parent_entity_type = "Province";
					//}
					var isNew = objPODMaster.system_id > 0 ? false : true;
					objPODMaster.is_new_entity = (isNew && objPODMaster.source_ref_id != "0" && objPODMaster.source_ref_id != "");
					//objPODMaster.Source_ref_type = headerAttribute.Source;
					//objPODMaster.Source_ref_description = headerAttribute.Source_ref_description;
					//objPODMaster.Source_ref_id = headerAttribute.Source_ref_id;
					var resultItem = new BLPOD().SaveEntityPOD(objPODMaster, objPODMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						if (objPODMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objPODMaster.EntityReference, resultItem.system_id);
						}
						objPODMaster.extraAttributes.system_id = resultItem.system_id;
						objPODMaster.extraAttributes.entity_type = EntityType.POD.ToString();
						new BLAdditionalAttributes().SaveAttributes(objPODMaster.extraAttributes);
						if (isNew)
						{
							//// Save geometry
							//InputGeom geom = new InputGeom();
							//geom.systemId = resultItem.system_id;
							//geom.longLat = resultItem.geom;
							//geom.userId = Convert.ToInt32(Session["user_id"]);
							//geom.entityType = EntityType.POD.ToString();
							//geom.commonName = resultItem.network_id;
							//geom.geomType = GeometryType.Point.ToString();
							//if (resultItem.parent_entity_type == EntityType.Structure.ToString())
							//{
							//    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
							//}
							//string chkGeomInsert = BASaveEntityGeometry.Instance.SaveEntityGeometry(geom);

							objPODMaster.objPM.status = ResponseStatus.OK.ToString();
							objPODMaster.objPM.isNewEntity = isNew;
							objPODMaster.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, layer_title);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, layer_title);
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objPODMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objPODMaster.system_id, EntityType.POD.ToString(), objPODMaster.network_id, objPODMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPODMaster.longitude + " " + objPODMaster.latitude }, objPODMaster.user_id);
								objPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objPODMaster.objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
							}
						}
					}
					else
					{
						objPODMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objPODMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objPODMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objPODMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objPODMaster.isDirectSave == true)
				{
					response.results = objPODMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objPODMaster, EntityType.POD.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA              
					fillProjectSpecifications(objPODMaster);
					BindPODDropDown(objPODMaster);
					//Get the layer details to bind additional attributes POD
					var layerdetails = new BLLayer().getLayer(EntityType.POD.ToString());
					objPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes POD
					response.results = objPODMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SavePod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion

		#region Mpod

		#region Mpod Details
		/// <summary> Get Mpod Details</summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Mpod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public MPODMaster GetMPODDetail(MPODMaster objMPOD)
		{
			int id = objMPOD.user_id;
			if (objMPOD.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objMPOD, GeometryType.Point.ToString(), objMPOD.geom);
				//Fill Parent detail...              
				fillParentDetail(objMPOD, new NetworkCodeIn() { eType = EntityType.MPOD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPOD.geom }, objMPOD.networkIdType);
				objMPOD.longitude = Convert.ToDecimal(objMPOD.geom.Split(' ')[0]);
				objMPOD.latitude = Convert.ToDecimal(objMPOD.geom.Split(' ')[1]);
				objMPOD.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<MPODTemplateMaster>(objMPOD.user_id, EntityType.MPOD);
				MiscHelper.CopyMatchingProperties(objItem, objMPOD);
				objMPOD.other_info = null;  //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objMPOD = new BLMisc().GetEntityDetailById<MPODMaster>(objMPOD.system_id, EntityType.MPOD, objMPOD.user_id);
				//for additional-attributes
				objMPOD.other_info = new BLMPOD().GetOtherInfoMPOD(objMPOD.system_id);
				fillRegionProvAbbr(objMPOD);
			}
			objMPOD.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());
			return objMPOD;
		}

		#endregion

		#region Add Mpod
		/// <summary> Add Mpod </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Mpod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<MPODMaster> AddMpod(ReqInput data)
		{
			var response = new ApiResponse<MPODMaster>();
			try
			{
				MPODMaster objRequestIn = ReqHelper.GetRequestData<MPODMaster>(data);
				MPODMaster objMPODMaster = GetMPODDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objMPODMaster, EntityType.MPOD.ToString());
				fillProjectSpecifications(objMPODMaster);
				//if (objPODMaster.parent_system_id > 0)
				//{
				//    objPODMaster.objIspEntityMap.structure_id = objPODMaster.parent_system_id;
				//    objPODMaster.objIspEntityMap.AssociateStructure = objPODMaster.parent_system_id;
				//}
				BindMPODDropDown(objMPODMaster);
				//Get the layer details to bind additional attributes MPOD
				var layerdetails = new BLLayer().getLayer(EntityType.MPOD.ToString());
				objMPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes MPOD
				response.status = StatusCodes.OK.ToString();
				response.results = objMPODMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddMpod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind Pod Dropdown
		/// <summary> Bind Mpod Dropdown </summary>
		/// <param name="data">object</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindMPODDropDown(MPODMaster objMPOD)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objMPOD.parent_system_id, objMPOD.system_id, EntityType.MPOD.ToString());
			if (ispEntityMap != null)
			{
				objMPOD.objIspEntityMap.id = ispEntityMap.id;
				objMPOD.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objMPOD.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objMPOD.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objMPOD.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}

			objMPOD.objIspEntityMap.AssoType = objMPOD.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objMPOD.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objMPOD.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objMPOD.longitude + " " + objMPOD.latitude);
			if (objMPOD.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objMPOD.objIspEntityMap.structure_id);
				objMPOD.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objMPOD.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objMPOD.parent_entity_type == EntityType.UNIT.ToString())
				{
					objMPOD.objIspEntityMap.unitId = objMPOD.parent_system_id;
					//objMPOD.objIspEntityMap.AssoType = "";
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.MPOD.ToString()).FirstOrDefault() != null)
			{
				objMPOD.objIspEntityMap.isValidParent = true;
				objMPOD.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objMPOD.objIspEntityMap.structure_id, objMPOD.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = new BLLayer().GetLayerDetails().Where(m => m.layer_name.ToUpper() == EntityType.MPOD.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objMPOD.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objMPOD.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objMPOD.objIspEntityMap.entity_type == null) { objMPOD.objIspEntityMap.entity_type = EntityType.MPOD.ToString(); }
			objMPOD.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objMPOD.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList(EntityType.MPOD.ToString());
			objMPOD.listMPODType = obj_DDL.Where(x => x.dropdown_type == DropDownType.MPOD_Type.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objMPOD.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objMPOD.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save Mpod
		/// <summary> SaveMpod</summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<MPODMaster> SaveMpod(ReqInput data)
		{
			var response = new ApiResponse<MPODMaster>();
			MPODMaster objMPODMaster = ReqHelper.GetRequestData<MPODMaster>(data);
			try
			{
				if (objMPODMaster.networkIdType == NetworkIdType.A.ToString() && objMPODMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.MPOD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.geom, parent_eType = objMPODMaster.pEntityType, parent_sysId = objMPODMaster.pSystemId });
					if (objMPODMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objMPODMaster = GetMPODDetail(objMPODMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objMPODMaster.mpod_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objMPODMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objMPODMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objMPODMaster.network_id = objNetworkCodeDetail.network_code;
					objMPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;

				}
				if (string.IsNullOrEmpty(objMPODMaster.mpod_name))
				{
					objMPODMaster.mpod_name = objMPODMaster.network_id;
				}
				this.Validate(objMPODMaster);
				if (ModelState.IsValid)
				{

					objMPODMaster.objIspEntityMap.structure_id = Convert.ToInt32(objMPODMaster.objIspEntityMap.AssociateStructure);
					if (objMPODMaster.objIspEntityMap.structure_id != 0)
					{
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objMPODMaster.objIspEntityMap.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							objMPODMaster.latitude = Convert.ToDecimal(structureDetails.latitude);
							objMPODMaster.longitude = Convert.ToDecimal(structureDetails.longitude);
						}

					}
					objMPODMaster.objIspEntityMap.shaft_id = objMPODMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objMPODMaster.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(objMPODMaster.objIspEntityMap.AssoType))
					{
						objMPODMaster.objIspEntityMap.shaft_id = 0; objMPODMaster.objIspEntityMap.floor_id = 0;
					}
					if (objMPODMaster.objIspEntityMap.structure_id == 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.MPOD.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.longitude + " " + objMPODMaster.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							objMPODMaster.parent_system_id = parentDetails.p_system_id;
							objMPODMaster.parent_network_id = parentDetails.p_network_id;
							objMPODMaster.parent_entity_type = parentDetails.p_entity_type;
						}
					}
					//if (objMPODMaster.objIspEntityMap.structure_id != 0)
					//{
					//    objMPODMaster.parent_system_id = Convert.ToInt32(objMPODMaster.objIspEntityMap.structure_id);
					//    objMPODMaster.parent_entity_type = EntityType.Structure.ToString();
					//    objMPODMaster.latitude = objMPODMaster.latitude;
					//    objMPODMaster.longitude = objMPODMaster.longitude;
					//}
					//else
					//{
					//    objMPODMaster.parent_system_id = 0;
					//    objMPODMaster.parent_entity_type = "Province";
					//}
					var isNew = objMPODMaster.system_id > 0 ? false : true;
					objMPODMaster.is_new_entity = (isNew && objMPODMaster.source_ref_id != "0" && objMPODMaster.source_ref_id != "");
					//objMPODMaster.Source_ref_type = headerAttribute.Source;
					//objMPODMaster.Source_ref_description = headerAttribute.Source_ref_description;
					//objMPODMaster.Source_ref_id = headerAttribute.Source_ref_id;

					var resultItem = new BLMPOD().SaveEntityMPOD(objMPODMaster, objMPODMaster.user_id);
					//Save Reference
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (objMPODMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objMPODMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							string[] LayerName = { EntityType.MPOD.ToString() };
							objMPODMaster.objPM.status = ResponseStatus.OK.ToString();
							objMPODMaster.objPM.isNewEntity = isNew;
							objMPODMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objMPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objMPODMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
							}
							else
							{
								string[] LayerName = { EntityType.MPOD.ToString() };
								BLLoopMangment.Instance.UpdateLoopDetails(objMPODMaster.system_id, EntityType.MPOD.ToString(), objMPODMaster.network_id, objMPODMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.longitude + " " + objMPODMaster.latitude }, objMPODMaster.user_id);
								objMPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objMPODMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						objMPODMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objMPODMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objMPODMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objMPODMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objMPODMaster.isDirectSave == true)
				{
					response.status = ResponseStatus.OK.ToString();
					response.results = objMPODMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objMPODMaster, EntityType.MPOD.ToString());
					BindMPODDropDown(objMPODMaster);
					fillProjectSpecifications(objMPODMaster);
					//Get the layer details to bind additional attributes MPOD
					var layerdetails = new BLLayer().getLayer(EntityType.MPOD.ToString());
					objMPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes MPOD
					response.results = objMPODMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveMpod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion

		#region Splice Closure

		#region Splice Closure Detail
		/// <summary>/// GetSCDetail/// </summary>
		/// <param name="objSC"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public SCMaster GetSCDetail(SCMaster objSC)
		{
			int user_id = objSC.user_id;
			if (objSC.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objSC, GeometryType.Point.ToString(), objSC.geom);
				//Fill Parent detail...              
				fillParentDetail(objSC, new NetworkCodeIn() { eType = EntityType.SpliceClosure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSC.geom }, objSC.networkIdType);
				objSC.longitude = Convert.ToDouble(objSC.geom.Split(' ')[0]);
				objSC.latitude = Convert.ToDouble(objSC.geom.Split(' ')[1]);
				objSC.ownership_type = "Own";
				objSC.other_info = null;    //for additional-attributes
											// Item template binding
				if (objSC.isConvert)
				{
					var objItem = new BLVendorSpecification().getEntityTemplatebyPortNo(objSC.no_of_ports, EntityType.SpliceClosure.ToString(), objSC.vendor_id);
					objSC.vendor_id = objItem.vendor_id;
					objSC.specification = objItem.specification;
					objSC.subcategory1 = objItem.subcategory_1;
					objSC.subcategory2 = objItem.subcategory_2;
					objSC.subcategory3 = objItem.subcategory_3;
					objSC.no_of_port = objItem.no_of_port;
					objSC.category = objItem.category_reference;
					objSC.item_code = objItem.code;
				}
				else
				{
					var objItem = BLItemTemplate.Instance.GetTemplateDetail<SCTemplateMaster>(objSC.user_id, EntityType.SpliceClosure);
					MiscHelper.CopyMatchingProperties(objItem, objSC);
					objSC.no_of_port = objItem.no_of_ports;
				}
			}
			else
			{
				// Get entity detail by Id...
				objSC = new BLMisc().GetEntityDetailById<SCMaster>(objSC.system_id, EntityType.SpliceClosure, objSC.user_id);
				objSC.no_of_port = objSC.no_of_ports;
				//for additional-attributes
				objSC.other_info = new BLSC().GetOtherInfoSpliceClosure(objSC.system_id);
				fillRegionProvAbbr(objSC);
			}
			objSC.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objSC;
		}
		#endregion

		#region Bind Splice Closure Dropdown
		/// <summary>
		/// <param name="data">object</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindSpilceClosureDropdown(SCMaster objSCMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.SpliceClosure.ToString());
			//objSCMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objSCMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objSCMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objSCMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			objSCMaster.listaerialLocation = objDDL.Where(x => x.dropdown_type == DropDownType.Aerial_Location.ToString()).ToList();
            objSCMaster.listSCType = objDDL.Where(x => x.dropdown_type == DropDownType.Spliceclosure_type.ToString()).ToList();

            // objSCMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        }
        private void BindSpilceClosureRoute(SCMaster objSCMaster)
		{
            if (objSCMaster.system_id == 0)
                objSCMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objSCMaster.geom);
            else
                objSCMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objSCMaster.system_id, objSCMaster.entityType);
        }
		#endregion

		#region Add Splice Closure
		/// <summary> Add Splice Closure </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Splice Closure Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<SCMaster> AddSpliceClosure(ReqInput data)
		{
			var response = new ApiResponse<SCMaster>();
			try
			{
				SCMaster objRequestIn = ReqHelper.GetRequestData<SCMaster>(data);
				if (string.IsNullOrWhiteSpace(objRequestIn.geom) && objRequestIn.system_id == 0)
				{
					// get geom by parent id...
					objRequestIn.geom = GetPointTypeParentGeom(objRequestIn.pSystemId, objRequestIn.pEntityType);
				}
				SCMaster objSCMaster = GetSCDetail(objRequestIn);

				BLItemTemplate.Instance.BindItemDropdowns(objSCMaster, EntityType.SpliceClosure.ToString());
				BindSpilceClosureDropdown(objSCMaster);
				BindSpilceClosureRoute(objSCMaster);

                fillProjectSpecifications(objSCMaster);
                List<int> listI = new List<int>();
                foreach (var i in objSCMaster.lstRouteInfo.Where(x => x.is_associated))
                {
					listI.Add(i.cable_id);
                }
				objSCMaster.selected_route_ids = listI;
                new BLMisc().BindPortDetails(objSCMaster, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
				//Get the layer details to bind additional attributes SpliceClosure
				var layerdetails = new BLLayer().getLayer(EntityType.SpliceClosure.ToString());
				objSCMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes SpliceClosure
				response.status = StatusCodes.OK.ToString();
				response.results = objSCMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddSpliceClosure()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Save SpliceClosure
		/// <summary> SaveSpliceClosure</summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<SCMaster> SaveSpliceClosure(ReqInput data)
		{
			var response = new ApiResponse<SCMaster>();
			SCMaster objSCMaster = ReqHelper.GetRequestData<SCMaster>(data);
			try
			{
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objSCMaster.geom) && objSCMaster.system_id == 0)
				{
					objSCMaster.geom = GetPointTypeParentGeom(objSCMaster.pSystemId, objSCMaster.pEntityType);
				}

				ModelState.Clear();
				if (objSCMaster.networkIdType == NetworkIdType.A.ToString() && objSCMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.SpliceClosure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSCMaster.geom, parent_eType = objSCMaster.pEntityType, parent_sysId = objSCMaster.pSystemId });
					if (objSCMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objSCMaster = GetSCDetail(objSCMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objSCMaster.spliceclosure_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objSCMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objSCMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objSCMaster.network_id = objNetworkCodeDetail.network_code;
					objSCMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (objSCMaster.unitValue != null && objSCMaster.unitValue.Contains(":"))
				{
					objSCMaster.no_of_input_port = Convert.ToInt32(objSCMaster.unitValue.Split(':')[0]);
					objSCMaster.no_of_output_port = Convert.ToInt32(objSCMaster.unitValue.Split(':')[1]);
				}
				if (string.IsNullOrEmpty(objSCMaster.spliceclosure_name))
				{
					objSCMaster.spliceclosure_name = objSCMaster.network_id;
				}
                BLItemTemplate.Instance.BindItemDropdowns(objSCMaster, EntityType.SpliceClosure.ToString());
                for (int i = 0; i < objSCMaster.lstAccessibility.Count; i++)
                {
                    if (objSCMaster.lstAccessibility[i].key == "No")
                        objSCMaster.accessibility = objSCMaster.is_buried == true ? Convert.ToInt16(objSCMaster.lstAccessibility[i].value) : objSCMaster.accessibility;
                }
                this.Validate(objSCMaster);
				if (objSCMaster.pSystemId > 0 && !String.IsNullOrEmpty(objSCMaster.pNetworkId))
				{
					objSCMaster.parent_system_id = objSCMaster.pSystemId;
					objSCMaster.parent_network_id = objSCMaster.pNetworkId;
					objSCMaster.parent_entity_type = objSCMaster.pEntityType;
				}
				if (ModelState.IsValid)
				{
					var isNew = objSCMaster.system_id > 0 ? false : true;
					objSCMaster.is_new_entity = (isNew && objSCMaster.source_ref_id != "0" && objSCMaster.source_ref_id != "");
					var resultItem = new BLSC().SaveEntitySC(objSCMaster, objSCMaster.user_id);
                    BindSpilceClosureRoute(objSCMaster);
                    List<RouteInfo> objL = new List<RouteInfo>();
                    foreach (var itm in objSCMaster.lstRouteInfo)
					{
						bool f = false;
						if (objSCMaster.selected_route_ids != null)
						{
							foreach (var ids in objSCMaster.selected_route_ids)
							{
								if (ids == itm.cable_id)
								{
									f = true;

								}
							}
						}
                        RouteInfo objS = new RouteInfo();
                        objS.entity_id = resultItem.system_id;
                        objS.entity_type = EntityType.SpliceClosure.ToString();
                        objS.created_by = objSCMaster.user_id.ToString();
                        objS.cable_id = itm.cable_id;
						objS.is_associated = f;
                        objL.Add(objS);
                    }
				
					var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objL), resultItem.system_id, EntityType.SpliceClosure.ToString(), objSCMaster.user_id);
                    BindSpilceClosureRoute(objSCMaster);
                    fillProjectSpecifications(objSCMaster);
                    List<int> listI = new List<int>();
                    foreach (var i in objSCMaster.lstRouteInfo.Where(x => x.is_associated))
                    {
                        listI.Add(i.cable_id);
                    }
                    objSCMaster.selected_route_ids = listI;
                    if (resultItem.isConvert && string.IsNullOrEmpty(resultItem.objPM.message) && isNew)
					{
						string[] LayerName = { EntityType.CDB.ToString(), EntityType.SpliceClosure.ToString() };
						CDBMaster objCDB = new CDBMaster();
						objCDB = new BLMisc().GetEntityDetailById<CDBMaster>(objSCMaster.cdb_system_id, EntityType.CDB);
						//====  VALIDATE CONNECTION AND SPLICING===// 
						var result = new BLMisc().EntityConversion(EntityType.CDB.ToString(), objCDB.network_id, objCDB.system_id, EntityType.SpliceClosure.ToString(), resultItem.network_id, resultItem.system_id, objCDB.geom, objSCMaster.user_id);
						//if (response.status=="OK")
						//{
						objSCMaster.objPM.status = ResponseStatus.OK.ToString();
						objSCMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CDB_NET_FRM_007, ApplicationSettings.listLayerDetails, LayerName);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CDB_NET_FRM_007, ApplicationSettings.listLayerDetails, LayerName);
						if (objSCMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objSCMaster.EntityReference, resultItem.system_id);
						}
						
                    }
					//START SPLIT CABLE FEATURE BY ANTRA//
					var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.SpliceClosure.ToString().ToUpper()).FirstOrDefault();
					if (objSCMaster.split_cable_system_id > 0 && layerDetails.is_split_allowed == true && isNew)
					{
						SplitCable(objSCMaster.system_id, objSCMaster.split_cable_system_id, "SpliceClosure", objSCMaster.network_status, objSCMaster.user_id);
						string[] Layer_Name = { EntityType.Cable.ToString() };
						objSCMaster.objPM.status = ResponseStatus.OK.ToString();
						objSCMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						if (objSCMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objSCMaster.EntityReference, resultItem.system_id);
						}
					}
					// END //
					//Save Reference 
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.SpliceClosure.ToString() };

						if (objSCMaster.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(objSCMaster.lstSpliceTrayInfo, resultItem.system_id, objSCMaster.user_id, EntityType.SpliceTray.ToString(), EntityType.SpliceClosure.ToString());
						}
						if (objSCMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objSCMaster.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							objSCMaster.objPM.status = ResponseStatus.OK.ToString();
							objSCMaster.objPM.isNewEntity = isNew;
							objSCMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objSCMaster.objPM.status = ResponseStatus.OK.ToString();
								objSCMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;

							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objSCMaster.system_id, EntityType.SpliceClosure.ToString(), objSCMaster.network_id, objSCMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSCMaster.longitude + " " + objSCMaster.latitude }, objSCMaster.user_id);
								objSCMaster.objPM.status = ResponseStatus.OK.ToString();
								objSCMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}

					}
					//else
					//{
					//    objSCMaster.objPM.status = ResponseStatus.FAILED.ToString();
					//    objSCMaster.objPM.message = resultItem.objPM.message;
					//    response.error_message = resultItem.objPM.message;
					//    response.status = ResponseStatus.FAILED.ToString();
					//}
				}
				else
				{
					objSCMaster.objPM.status = ResponseStatus.FAILED.ToString();
					objSCMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();


				}
				if (objSCMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objSCMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objSCMaster, EntityType.SpliceClosure.ToString());
                    BindSpilceClosureDropdown(objSCMaster);
                    BindSpilceClosureRoute(objSCMaster);
                    // RETURN PARTIAL VIEW WITH MODEL DATA
                    fillProjectSpecifications(objSCMaster);
					new BLMisc().BindPortDetails(objSCMaster, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
					//Get the layer details to bind additional attributes SpliceClosure
					var layerdetails = new BLLayer().getLayer(EntityType.SpliceClosure.ToString());
					objSCMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes SpliceClosure
					response.results = objSCMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveSpliceClosure()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		private void AddSpliceTray(List<SpliceTrayInfo> objSCMaster, int system_id, int userId, string eType, string pEtype)
		{
			foreach (var item in objSCMaster)
			{

				if (item.system_id > 0)
				{
					item.modified_by = userId;
					item.modified_on = DateTime.Now;
				}
				else
				{
					var objSplice = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = eType, gType = GeometryType.Point.ToString(), parent_eType = pEtype, parent_sysId = system_id, eGeom = "" });
					item.parent_system_id = system_id;
					item.sequence_id = objSplice.sequence_id;
					item.network_id = objSplice.network_code;
					item.parent_entity_type = objSplice.parent_entity_type;
					item.parent_network_id = objSplice.parent_network_id;
					//item.network_name = EntityType.SpliceTray.ToString();
					item.created_by = userId;
					item.created_on = DateTime.Now;
				}
				if (item.no_of_ports > 0 && !String.IsNullOrEmpty(item.network_name.Trim()))
					BLSpliceTray.Instance.SaveSpliceTary(item);
			}
		}


		#endregion

		#endregion

		#region Cable
		#region Add Cable
		/// <summary> Add Cable </summary>
		/// <param name="data">SystemId,EntityType,NetworkId,userId</param>
		/// <returns>Cable Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<CableMaster> AddCable(ReqInput data)
		{
			var response = new ApiResponse<CableMaster>();
			try
			{
				LineEntityIn objIn = ReqHelper.GetRequestData<LineEntityIn>(data);
				CableMaster objCbl = new CableMaster();
				objCbl = GetCableDetail(objIn, objCbl);
				if (objIn.system_id == 0)
				{
					//Fill Location detail...    
					GetLineNetworkDetail(objCbl, objIn, EntityType.Cable.ToString(), false);

				}

				BLItemTemplate.Instance.BindItemDropdowns(objCbl, EntityType.Cable.ToString());
				BindCableDropDown(objCbl);
				objCbl.fiberCountIn = objCbl.total_core.ToString();
				fillProjectSpecifications(objCbl);
				new BLMisc().BindPortDetails(objCbl, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
				objCbl.unitValue = Convert.ToString(objCbl.total_core);
				objCbl.pSystemId = objIn.pSystemId;
				objCbl.pNetworkId = objIn.pNetworkId;
				objCbl.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
				//Get the layer details to bind additional attributes Cable
				var layerdetails = new BLLayer().getLayer(EntityType.Cable.ToString());
				objCbl.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Cable
				response.status = StatusCodes.OK.ToString();
				response.results = objCbl;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddCable()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region GetCableDetails
		public CableMaster GetCableDetail(LineEntityIn objIn, CableMaster objCbl)
		{
			if (objIn.system_id == 0)
			{
				objCbl.geom = objIn.geom;
				objCbl.cable_type = objIn.cableType;
				if (!string.IsNullOrEmpty(objIn.geom))
					objCbl.cable_measured_length = (float)new BLMisc().GetCableLength(objIn.geom);
				var extraLength = (ApplicationSettings.CableExtraLengthPercentage * objCbl.cable_measured_length) / 100 - objCbl.total_loop_length;
				objCbl.cable_calculated_length = Math.Round((Double)(objCbl.cable_measured_length + objCbl.total_loop_length + extraLength), 3);

				objCbl.networkIdType = objIn.networkIdType;
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objCbl, GeometryType.Line.ToString(), objIn.geom);
				var a = new BLMisc().GetEndPoints(objIn.geom);
				objCbl.a_latitude = a[0].a_latitude.Replace("POINT(", "").Replace(")", "");
				objCbl.b_longitude = a[0].b_longitude.Replace("POINT(", "").Replace(")", "");
				objCbl.a_region = a[0].tstart_region;
				objCbl.b_region = a[0].tEnd_region;
				objCbl.a_city = a[0].tstrat_province;
				objCbl.b_city = a[0].tEnd_province;
				if (ApplicationSettings.IsEntityNamePrefixAllow)
				{
					objCbl.cable_name = "R/OWN/" + a[0].province_abbr;
				}
				// set default value for ownership..
				objCbl.ownership_type = "Own";
				objCbl.isDirectSave = objIn.isDirectSave;
				objCbl.user_id = objIn.user_id;
				var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
				//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
				objCbl.bom_sub_category = objBOMDDL[0].dropdown_value;
				// objCbl.served_by_ring = objSubCatDDL[0].dropdown_value;

				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<CableTemplateMaster>(objIn.user_id, EntityType.Cable);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objCbl);
				objCbl.unitValue = objItem.total_core.ToString();
				objCbl.other_info = null;   //for additional-attributes

			}
			else
			{
				objCbl = new BLMisc().GetEntityDetailById<CableMaster>(objIn.system_id, EntityType.Cable, objIn.user_id);
				//Session["CableNetworkId"] = objCbl.network_id;
				//for additional-attributes
				objCbl.other_info = new BLCable().GetOtherInfoCable(objCbl.system_id);
				fillRegionProvAbbr(objCbl);
                var objCDB = BLCable.Instance.GetDetailsCDBAttribute(objCbl.system_id);
                if (objCDB != null)
                {
                    objCbl.LstCDBAttribute = objCDB;
                }
            }
			objCbl.lstUserModule = new BLLayer().GetUserModuleAbbrList(objIn.user_id, UserType.Web.ToString());
			return objCbl;
		}
		#endregion

		#region GetLineNetworkDetail
		private void GetLineNetworkDetail(dynamic objLib, LineEntityIn objIn, string enName, bool isAuto)
		{
			var startObj = new NetworkDtl();
			var endObj = new NetworkDtl();
			var start_network_id = "";
			var end_network_id = "";
			//if (objIn.lstTP != null && objIn.lstTP.Count > 0)
			//{
			//    startObj = objIn.lstTP[0];
			//    start_network_id = startObj.network_id;
			//}
			//if (objIn.lstTP != null && objIn.lstTP.Count > 1)
			//{
			//    endObj = objIn.lstTP[objIn.lstTP.Count() - 1];
			//    end_network_id = endObj.network_id;
			//}
			startObj = objIn.lstTP.Where(m => m.node_type == "start").FirstOrDefault();
			endObj = objIn.lstTP.Where(m => m.node_type == "end").FirstOrDefault();
			//fill parent detail....
			var networkCodeDetail = new BLMisc().GetLineNetworkCode(start_network_id, end_network_id, enName, objIn.geom, "OSP");

			if (!string.IsNullOrEmpty(networkCodeDetail.network_code))
			{
				if (objIn.networkIdType == NetworkIdType.M.ToString())
				{
					//FILL NETWORK CODE FORMAT FOR MANUAL
					objLib.network_id = networkCodeDetail.network_code;
				}
				else if (objIn.networkIdType == NetworkIdType.A.ToString() && isAuto)
				{
					objLib.network_id = networkCodeDetail.network_code;
				}
			}
			objLib.parent_entity_type = networkCodeDetail.parent_entity_type;
			objLib.parent_network_id = networkCodeDetail.parent_network_id;
			objLib.parent_system_id = networkCodeDetail.parent_system_id;
			if (startObj != null)
			{
				var entityName = new BLMisc().GetEntityName(startObj.network_name, startObj.system_id);
				objLib.a_entity_type = startObj.network_name;
				objLib.a_system_id = startObj.system_id;
                objLib.a_location_name = entityName;
                objLib.a_location = startObj.network_id;
				objLib.a_node_type = startObj.node_type;
				objLib.a_long_lat = startObj.actualLatLng;
				if (!string.IsNullOrEmpty(startObj.actualLatLng)) { objLib.geom = startObj.actualLatLng + "," + objLib.geom; }
			}
			if (endObj != null)
			{
                var entityName = new BLMisc().GetEntityName(endObj.network_name, endObj.system_id);
                objLib.b_entity_type = endObj.network_name;
				objLib.b_system_id = endObj.system_id;
                objLib.b_location_name = entityName;
                objLib.b_location = endObj.network_id;
				objLib.b_node_type = endObj.node_type;
				objLib.b_long_lat = endObj.actualLatLng;
				if (!string.IsNullOrEmpty(endObj.actualLatLng)) { objLib.geom = objLib.geom + "," + endObj.actualLatLng; }
			}
			objLib.sequence_id = networkCodeDetail.sequence_id;
		}
		#endregion

		#region BindCableDropDown
		private void BindCableDropDown(CableMaster objCableIn)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
			objCableIn.fiberCount = objDDL.Where(x => x.dropdown_type == DropDownType.Fiber_Count.ToString()).ToList();
			objCableIn.listcableCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
			objCableIn.listcableSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Subcategory.ToString()).ToList();
			objCableIn.listExecutionMethod = objDDL.Where(x => x.dropdown_type == DropDownType.Execution_Method.ToString()).ToList();
			objCableIn.listcableType = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Type.ToString()).ToList();
			objCableIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objCableIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            objCableIn.LstCDBAttribute.lstRoute = objDDL.Where(x => x.dropdown_type == DropDownType.Route_Type.ToString()).ToList();
            objCableIn.LstCDBAttribute.lstFiber = objDDL.Where(x => x.dropdown_type == DropDownType.Fiber_Type_LOV.ToString()).ToList();
            objCableIn.LstCDBAttribute.lstOperator = objDDL.Where(x => x.dropdown_type == DropDownType.Operator_Type_LOV.ToString()).ToList();
            objCableIn.listaerialLocation= objDDL.Where(x => x.dropdown_type == DropDownType.Aerial_Location.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objCableIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objCableIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
			objCableIn.listALocation = _objDDL.Where(x => x.dropdown_type == DropDownType.A_location.ToString()).ToList();
			objCableIn.listBLocation = _objDDL.Where(x => x.dropdown_type == DropDownType.A_location.ToString()).ToList();
		}

		#endregion

		#region Save Cable
		/// <summary> SaveCable</summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<CableMaster> SaveCable(ReqInput data)
		{
			var response = new ApiResponse<CableMaster>();
			CableMaster objCbl = ReqHelper.GetRequestData<CableMaster>(data);
			if (objCbl.total_core > 0)
			{
				objCbl.unitValue = Convert.ToString(objCbl.total_core);
			}
			try
			{
				/*....LMC INFORMATION is not included in API section!! */
				//if (objCbl.actionTab == "LMCInfoTab")
				//{
				//    objCbl.LMCCableInfo.ActionTab = objCbl.actionTab;
				//    return SaveLMCInfo(objCbl.LMCCableInfo);
				//}
				//else
				//{
				ModelState.Clear();
				bool isValid = true;
				int pSystemId = objCbl.pSystemId;
				string pNetworkId = objCbl.pNetworkId;
				if (objCbl.networkIdType == NetworkIdType.A.ToString() && objCbl.system_id == 0)
				{
					if (objCbl.isDirectSave == false)
					{
						objCbl.lstTP.Add(new NetworkDtl { system_id = objCbl.a_system_id, network_id = objCbl.a_location, network_name = objCbl.a_entity_type, node_type = objCbl.a_node_type, actualLatLng = objCbl.a_long_lat, entity_name= objCbl.a_location_name });
						objCbl.lstTP.Add(new NetworkDtl { system_id = objCbl.b_system_id, network_id = objCbl.b_location, network_name = objCbl.b_entity_type, node_type = objCbl.b_node_type, actualLatLng = objCbl.b_long_lat, entity_name= objCbl.b_location_name });
					}
					var objLineEntity = new LineEntityIn() { geom = objCbl.geom, system_id = objCbl.system_id, cableType = objCbl.cable_type, networkIdType = objCbl.networkIdType, lstTP = objCbl.lstTP, user_id = objCbl.user_id, isDirectSave = objCbl.isDirectSave };
					if (objCbl.isDirectSave == true)
					{

						objCbl = GetCableDetail(objLineEntity, objCbl);
						objCbl.duct_id = pSystemId;
						objCbl.pNetworkId = pNetworkId;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objCbl.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objCbl.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//GET AUTO NETWORK CODE...
					GetLineNetworkDetail(objCbl, objLineEntity, EntityType.Cable.ToString(), true);
					if (objCbl.isDirectSave == true)
						objCbl.cable_name = objCbl.network_id;
				}
				if (string.IsNullOrEmpty(objCbl.cable_name))
				{
					objCbl.cable_name = objCbl.network_id;
				}
				objCbl.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
				var startEndReading = objCbl.formInputSettings.Count > 0 ? objCbl.formInputSettings.Where(m => m.form_feature_name.ToLower() == formFeatureName.start_end_reading.ToString() && m.form_feature_type.ToLower() == formFeatureType.feature.ToString() && m.is_active == true).FirstOrDefault() : null;

				var multipleTubeCore = objCbl.no_of_core_per_tube * objCbl.no_of_tube;
				if (multipleTubeCore != Convert.ToInt32(objCbl.unitValue) || multipleTubeCore == 0)
				{
					objCbl.objPM.status = ResponseStatus.FAILED.ToString();
					objCbl.objPM.message = Resources.Resources.SI_OSP_GBL_JQ_FRM_012;
					response.error_message = Resources.Resources.SI_OSP_GBL_JQ_FRM_012;
					response.status = ResponseStatus.FAILED.ToString();
					isValid = false;
				}
				else if (objCbl.cable_measured_length == 0)
				{
					objCbl.objPM.status = ResponseStatus.FAILED.ToString();
					objCbl.objPM.message = Resources.Resources.SI_OSP_CAB_NET_FRM_066;
					response.error_message = Resources.Resources.SI_OSP_CAB_NET_FRM_066;
					response.status = ResponseStatus.FAILED.ToString();
					isValid = false;
				}
				if (!(string.IsNullOrEmpty(objCbl.unitValue)))
				{
					objCbl.total_core = Convert.ToInt32(objCbl.unitValue);
				}

				if (objCbl.isDirectSave)
				{
					if (startEndReading != null)
					{
						objCbl.start_reading = 0;
						objCbl.end_reading = objCbl.cable_calculated_length;
						//objCbl.cable_calculated_length = 0;
					}
				}
				if (string.IsNullOrEmpty(objCbl.pin_code))
					objCbl.pin_code = null;
				this.Validate(objCbl);
				if (ModelState.IsValid)
				{
					var isNew = objCbl.system_id > 0 ? false : true;
					objCbl.is_new_entity = (isNew && objCbl.source_ref_id != "0" && objCbl.source_ref_id != "");
					objCbl.duct_id = pSystemId;
					var resultItem = BLCable.Instance.SaveCable(objCbl, objCbl.user_id);
					//var insertTubeCore = BLCable.Instance.SetCableColorDetails(resultItem.system_id, resultItem.no_of_tube,resultItem.no_of_core_per_tube, resultItem.created_by);

					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{

						string[] LayerName = { EntityType.Cable.ToString() };

						if (objCbl.lstTubeCore != null)
						{
							BLCable.Instance.SaveTubeCoreColor(JsonConvert.SerializeObject(objCbl.lstTubeCore.objTube), JsonConvert.SerializeObject(objCbl.lstTubeCore.objCore), resultItem.system_id);
						}
						if (isNew)
						{
							objCbl.objPM.status = ResponseStatus.OK.ToString();
							objCbl.objPM.isNewEntity = isNew;
							objCbl.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objCbl.objPM.status = ResponseStatus.OK.ToString();
								objCbl.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);
								response.status = ResponseStatus.OK.ToString();
							}
							else
							{
								objCbl.objPM.status = ResponseStatus.OK.ToString();
								objCbl.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
							}
						}
						//Save Reference
						if (objCbl.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objCbl.EntityReference, resultItem.system_id);
						}
						//save AT Status                        
						if (objCbl.ATAcceptance != null && objCbl.system_id > 0)
						{
							SaveATAcceptance(objCbl.ATAcceptance, objCbl.system_id, objCbl.user_id);
						}
					}
					else
					{
						objCbl.objPM.status = ResponseStatus.FAILED.ToString();
						objCbl.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}

				}
				else
				{
					objCbl.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objCbl.objPM.message = isValid == true ? getFirstErrorFromModelState() : objCbl.objPM.message;
					response.error_message = isValid == true ? getFirstErrorFromModelState() : objCbl.objPM.message;
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objCbl.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objCbl;
					response.status = ResponseStatus.OK.ToString();

				}
				else
				{

					BLItemTemplate.Instance.BindItemDropdowns(objCbl, EntityType.Cable.ToString());
					BindCableDropDown(objCbl);
					new BLMisc().BindPortDetails(objCbl, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
					objCbl.unitValue = Convert.ToString(objCbl.total_core);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objCbl);
					//Get the layer details to bind additional attributes Cable
					var layerdetails = new BLLayer().getLayer(EntityType.Cable.ToString());
					objCbl.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Cable
					response.results = objCbl;
				}
				//}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveCable()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion

		#region DEPL API Created by Diksha Gupta
		[HttpPost]
		public ApiResponse<List<CableFiberDetail>> GetCableFiberDetail(ReqInput data)
		{
			var response = new ApiResponse<List<CableFiberDetail>>();
			CableFiberDetail objRequestIn = ReqHelper.GetRequestData<CableFiberDetail>(data);
			int cableId = objRequestIn.cable_id;

			List<CableFiberDetail> CableFiberDetail = BLCable.Instance.GetFiberDetailInfo(Convert.ToInt32(cableId));

			try
			{
				response.status = StatusCodes.OK.ToString();
				response.results = CableFiberDetail;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("GetCableFiberDetail()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}

		public ApiResponse<List<portStatusMaster>> GetFiberCorePortStatus()
		{
			var response = new ApiResponse<List<portStatusMaster>>();
			try
			{
				var listPortStatus = new BLPortStatus().getPortStatusFiber();
				response.status = StatusCodes.OK.ToString();
				response.results = listPortStatus.ToList();
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("GetFiberCorePortStatus()", "Splicing Controller", "", ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}

		[HttpPost]
		public ApiResponse<string> SaveCorePortStatus(ReqInput data)
		{
			var response = new ApiResponse<string>();
			try
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.WriteDebugLog("SaveCoreAPIRequest," + data.ToString());
				UpdateCorePortStatus objRequestIn = ReqHelper.GetRequestData<UpdateCorePortStatus>(data);

				var result = new BLOSPSplicing().UpdateCorePortStatus(objRequestIn);
				logHelper.WriteDebugLog("SaveCoreAPIRequest," + Convert.ToString(result.message));
				if (result.status)
				{
					response.results = "Core status is updated successfully.";
					response.status = StatusCodes.OK.ToString();
				}
				else
				{
					response.results = result.message;
					response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.WriteDebugLog("SaveCoreAPIRequest Error," + ex.StackTrace.ToString());
				logHelper.ApiLogWriter("SaveCorePortStatus()", "Library Controller", "", ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = "Error While Processing  Request.";
			}
			return response;
		}

		#endregion

		#region SaveLMCInfo
		public ApiResponse<CableMaster> SaveLMCInfo(LMCCableInfo objLMCInfo)
		{
			var response = new ApiResponse<CableMaster>();
			ModelState.Clear();
			this.Validate(objLMCInfo);
			if (ModelState.IsValid)
			{
				// GENERATE LMC ID
				if (objLMCInfo.system_id == 0)
				{
					var lmcDetails = BLLmcInfo.Instance.GetLMCId(objLMCInfo.lmc_type, objLMCInfo.lmc_standalone_redundant);
					objLMCInfo.lmc_id = lmcDetails.lmc_id;
				}
				var result = BLLmcInfo.Instance.SaveLMCInfo(objLMCInfo, objLMCInfo.user_id);
				if (string.IsNullOrEmpty(result.objPM.message))
				{
					if (objLMCInfo.objPM.isNewEntity)
					{
						objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_206;
						objLMCInfo.objPM.status = ResponseStatus.OK.ToString();
						response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_206;
						response.status = ResponseStatus.OK.ToString();
					}
					else
					{
						objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_207;
						objLMCInfo.objPM.status = ResponseStatus.OK.ToString();
						response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_207;
						response.status = ResponseStatus.OK.ToString();
					}
				}
				else
				{
					objLMCInfo.objPM.status = ResponseStatus.FAILED.ToString();
					objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_208;
					response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_208;
					response.status = ResponseStatus.FAILED.ToString();
				}

			}
			else
			{
				objLMCInfo.objPM.status = ResponseStatus.FAILED.ToString();
				objLMCInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_208;
				response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_208;
				response.status = ResponseStatus.FAILED.ToString();
			}
			BindLMCDropdownList(objLMCInfo);
			return response;
		}
		#endregion

		#region BindLMCDropdownList
		public void BindLMCDropdownList(LMCCableInfo objlmcInfo)
		{
			objlmcInfo.lstLMCType = new BLMisc().GetDropDownList("", DropDownType.LMC_TYPE.ToString());
			objlmcInfo.lstLMCCascadedStandalone = new BLMisc().GetDropDownList("", DropDownType.LMC_Cascaded_Standalone.ToString());
			objlmcInfo.lstRTNBuildingSiteTappingPoint = new BLMisc().GetDropDownList("", DropDownType.RTN_Building_Side_Tapping_Point.ToString());
			objlmcInfo.lstFiberCount = new BLMisc().GetDropDownList("", DropDownType.Fiber_Count.ToString());
			objlmcInfo.lstTerminationDetails = new BLMisc().GetDropDownList("", DropDownType.Termination_Details.ToString());
			objlmcInfo.lstLMCAttachment = new BLAttachment().getAttachmentDetails(objlmcInfo.system_id, "Cable", "Document", "LMCInfo");
			//converting file size
			foreach (var item in objlmcInfo.lstLMCAttachment)
			{
				item.file_size_converted = BytesToString(item.file_size);

			}
		}
		public static String BytesToString(long byteCount)
		{
			string[] suf = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
			if (byteCount == 0)
				return "0 " + suf[1];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
		}
		#endregion
		#endregion

		#region Splitter

		#region Splitter Detail
		/// <summary>/// GetSplitterDetail/// </summary>
		/// <param name="objSC"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public SplitterMaster GetSplitterDetail(SplitterMaster objSplitter)
		{
			int user_id = objSplitter.user_id;
			if (objSplitter.system_id == 0)
			{
				objSplitter.longitude = Convert.ToDouble(objSplitter.geom.Split(' ')[0]);
				objSplitter.latitude = Convert.ToDouble(objSplitter.geom.Split(' ')[1]);
				objSplitter.ownership_type = "Own";
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objSplitter, GeometryType.Point.ToString(), objSplitter.geom);
				//Fill Parent detail...              
				fillParentDetail(objSplitter, new NetworkCodeIn() { eType = EntityType.Splitter.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitter.geom, parent_eType = objSplitter.pEntityType, parent_sysId = objSplitter.pSystemId }, objSplitter.networkIdType);
				//Item template binding
				if (objSplitter.addSplitterInBox == 0)
				{
					var objItem = BLItemTemplate.Instance.GetTemplateDetail<SplitterTemplateMaster>(objSplitter.user_id, EntityType.Splitter);
					Utility.MiscHelper.CopyMatchingProperties(objItem, objSplitter);
				}
				objSplitter.other_info = null;  //for additional-attributes
			}
			else
			{
				objSplitter = new BLMisc().GetEntityDetailById<SplitterMaster>(objSplitter.system_id, EntityType.Splitter, objSplitter.user_id);
				//for additional-attributes
				objSplitter.other_info = new BLSplitter().GetOtherInfoSplitter(objSplitter.system_id);
				fillRegionProvAbbr(objSplitter);
			}
			objSplitter.formInputSettings = new BLFormInputSettings().getformInputSettings();
			objSplitter.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objSplitter;
		}
		#endregion

		#region Bind Splitter Dropdown
		/// <summary>
		/// <param name="data">object</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindSplitterDropDown(SplitterMaster objSplitterMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Splitter.ToString());
			//objSplitterMaster.lstSplRatio = objDDL.Where(x => x.dropdown_type == DropDownType.Splitter_Ratio.ToString()).ToList();
			new BLMisc().BindPortDetails(objSplitterMaster, EntityType.Splitter.ToString(), DropDownType.Splitter_Ratio.ToString());
			objSplitterMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objSplitterMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objSplitterMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objSplitterMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		private void BindSplitterRoute(SplitterMaster objSplitterMaster)
		{
            if (objSplitterMaster.system_id == 0)
                objSplitterMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objSplitterMaster.geom);
            else
                objSplitterMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objSplitterMaster.system_id, EntityType.Splitter.ToString());
        }
		#endregion

		#region Add Splitter 
		/// <summary> Add Splitter </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Splitter Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<SplitterMaster> AddSplitter(ReqInput data)
		{
			var response = new ApiResponse<SplitterMaster>();
			try
			{
				SplitterMaster objRequestIn = ReqHelper.GetRequestData<SplitterMaster>(data);
				if (string.IsNullOrWhiteSpace(objRequestIn.geom) && objRequestIn.system_id == 0)
				{
					// get geom by parent id...
					objRequestIn.geom = GetPointTypeParentGeom(objRequestIn.pSystemId, objRequestIn.pEntityType);
				}
				SplitterMaster objSplitterMaster = GetSplitterDetail(objRequestIn);
				objSplitterMaster.pSystemId = objRequestIn.pSystemId;
				objSplitterMaster.pEntityType = objRequestIn.pEntityType;
				BLItemTemplate.Instance.BindItemDropdowns(objSplitterMaster, EntityType.Splitter.ToString());
				BindSplitterDropDown(objSplitterMaster);
                BindSplitterRoute(objSplitterMaster);
                List<int> listI = new List<int>();
                foreach (var i in objSplitterMaster.lstRouteInfo.Where(x => x.is_associated))
                {
                    listI.Add(i.cable_id);
                }
                objSplitterMaster.selected_route_ids = listI;
                if (objSplitterMaster.pEntityType == EntityType.FDB.ToString() || objSplitterMaster.parent_entity_type == EntityType.FDB.ToString())
				{
					if (ApplicationSettings.splitterTypeForFat == 2)
					{
						objSplitterMaster.lstType = objSplitterMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Secondary.ToString()).ToList();
					}
					else if (ApplicationSettings.splitterTypeForFat == 1)
					{
						objSplitterMaster.lstType = objSplitterMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Primary.ToString()).ToList();
					}
					else
					{
						objSplitterMaster.lstType = objSplitterMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString()).ToList();
					}
				}
				if (objSplitterMaster.parent_entity_type == EntityType.BDB.ToString() || objSplitterMaster.pEntityType == EntityType.BDB.ToString())
				{
					if (ApplicationSettings.splitterTypeForFdc == 2)
					{
						objSplitterMaster.lstType = objSplitterMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Secondary.ToString()).ToList();
					}
					else if (ApplicationSettings.splitterTypeForFdc == 1)
					{
						objSplitterMaster.lstType = objSplitterMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Primary.ToString()).ToList();
					}
					else
					{
						objSplitterMaster.lstType = objSplitterMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString()).ToList();
					}
				}
				fillProjectSpecifications(objSplitterMaster);
				objSplitterMaster.unitValue = objSplitterMaster.splitter_ratio;
				//Get the layer details to bind additional attributes Splitter
				var layerdetails = new BLLayer().getLayer(EntityType.Splitter.ToString());
				objSplitterMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Splitter
				response.status = StatusCodes.OK.ToString();
				response.results = objSplitterMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddSplitter()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Save Splitter
		/// <summary> SaveSplitter</summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<SplitterMaster> SaveSplitter(ReqInput data)
		{
			var response = new ApiResponse<SplitterMaster>();
			SplitterMaster objSplitterMaster = ReqHelper.GetRequestData<SplitterMaster>(data);
			try
			{
				if (string.IsNullOrWhiteSpace(objSplitterMaster.geom) && objSplitterMaster.system_id == 0)
				{
					objSplitterMaster.geom = GetPointTypeParentGeom(objSplitterMaster.pSystemId, objSplitterMaster.pEntityType);
				}

				if (objSplitterMaster.networkIdType == NetworkIdType.A.ToString() && objSplitterMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Splitter.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSplitterMaster.geom, parent_eType = objSplitterMaster.pEntityType, parent_sysId = objSplitterMaster.pSystemId });
					if (objSplitterMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objSplitterMaster = GetSplitterDetail(objSplitterMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objSplitterMaster.splitter_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objSplitterMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objSplitterMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objSplitterMaster.network_id = objNetworkCodeDetail.network_code;
					objSplitterMaster.sequence_id = objNetworkCodeDetail.sequence_id;
					if (objSplitterMaster.parent_network_id == null)
					{
						objSplitterMaster.parent_network_id = objNetworkCodeDetail.parent_network_id;
					}
				}
				if (objSplitterMaster.unitValue != null && objSplitterMaster.unitValue.Contains(":"))
				{
					objSplitterMaster.splitter_ratio = objSplitterMaster.unitValue;
				}
				if (string.IsNullOrEmpty(objSplitterMaster.splitter_name))
				{
					objSplitterMaster.splitter_name = objSplitterMaster.network_id;
				}
				this.Validate(objSplitterMaster);
				if (ModelState.IsValid)
				{
					var isNew = objSplitterMaster.system_id > 0 ? false : true;
					objSplitterMaster.is_new_entity = (isNew && objSplitterMaster.source_ref_id != "0" && objSplitterMaster.source_ref_id != "");
					var resultItem = new BLSplitter().SaveSplitterEntity(objSplitterMaster, objSplitterMaster.user_id);
                    BindSplitterRoute(objSplitterMaster);
                    List<RouteInfo> objL = new List<RouteInfo>();
                    foreach (var itm in objSplitterMaster.lstRouteInfo)
                    {
                        bool f = false;
						if (objSplitterMaster.selected_route_ids != null)
						{
							foreach (var ids in objSplitterMaster.selected_route_ids)
							{
								if (ids == itm.cable_id)
								{
									f = true;

								}
							}
						}
                        RouteInfo objS = new RouteInfo();
                        objS.entity_id = resultItem.system_id;
                        objS.entity_type = EntityType.Splitter.ToString();
                        objS.created_by = objSplitterMaster.user_id.ToString();
                        objS.cable_id = itm.cable_id;
                        objS.is_associated = f;
                        objL.Add(objS);
                    }

                    var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objL), resultItem.system_id, EntityType.Splitter.ToString(), objSplitterMaster.user_id);
                    BindSplitterRoute(objSplitterMaster);
                    List<int> listI = new List<int>();
                    foreach (var i in objSplitterMaster.lstRouteInfo.Where(x => x.is_associated))
                    {
                        listI.Add(i.cable_id);
                    }
                    objSplitterMaster.selected_route_ids = listI;
                    if (String.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						if (objSplitterMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objSplitterMaster.EntityReference, resultItem.system_id);
						}
						string[] LayerName = { EntityType.Splitter.ToString() };
						if (isNew)
						{
							objSplitterMaster.objPM.status = ResponseStatus.OK.ToString();
							objSplitterMaster.objPM.isNewEntity = isNew;
							objSplitterMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);

						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objSplitterMaster.objPM.status = ResponseStatus.OK.ToString();
								objSplitterMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								objSplitterMaster.objPM.status = ResponseStatus.OK.ToString();
								objSplitterMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);

							}
						}
					}
					else
					{
						objSplitterMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objSplitterMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{

					objSplitterMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objSplitterMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();

				}
				if (objSplitterMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objSplitterMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objSplitterMaster, EntityType.Splitter.ToString());
					BindSplitterDropDown(objSplitterMaster);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objSplitterMaster);
					//Get the layer details to bind additional attributes Splitter
					var layerdetails = new BLLayer().getLayer(EntityType.Splitter.ToString());
					objSplitterMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Splitter
					response.results = objSplitterMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveSplitter()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}


		#endregion

		#endregion

		#region ADB

		#region ADB Detail
		/// <summary>/// GetADBDetail/// </summary>
		/// <param name="objSC"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ADBMaster GetADBDetail(ADBMaster objADB)
		{
			int user_id = objADB.user_id;
			if (objADB.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objADB, GeometryType.Point.ToString(), objADB.geom);
				//Fill Parent detail...              
				fillParentDetail(objADB, new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objADB.geom }, objADB.networkIdType);
				objADB.longitude = Convert.ToDouble(objADB.geom.Split(' ')[0]);
				objADB.latitude = Convert.ToDouble(objADB.geom.Split(' ')[1]);
				objADB.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<ADBTemplateMaster>(objADB.user_id, EntityType.ADB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objADB);
				objADB.other_info = null;   //for additional-attributes

			}
			else
			{
				// Get entity detail by Id...
				objADB = new BLMisc().GetEntityDetailById<ADBMaster>(objADB.system_id, EntityType.ADB, objADB.user_id);
				//for additional-attributes
				objADB.other_info = new BLADB().GetOtherInfoADB(objADB.system_id);
				fillRegionProvAbbr(objADB);
			}
			objADB.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objADB;
		}
		#endregion

		#region Bind ADB Dropdown
		/// <summary>
		/// <param name="data">object</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindADBDropDown(ADBMaster objADBMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString());
			// objADBMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objADBMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objADBMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objADBMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objADBMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Add ADB 
		/// <summary> Add ADB </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>ADB Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<ADBMaster> AddADB(ReqInput data)
		{
			var response = new ApiResponse<ADBMaster>();
			try
			{
				ADBMaster objRequestIn = ReqHelper.GetRequestData<ADBMaster>(data);
				if (string.IsNullOrWhiteSpace(objRequestIn.geom) && objRequestIn.system_id == 0)
				{
					// get geom by parent id...
					objRequestIn.geom = GetPointTypeParentGeom(objRequestIn.pSystemId, objRequestIn.pEntityType);
				}
				ADBMaster objADBMaster = GetADBDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objADBMaster, EntityType.ADB.ToString());
				BindADBDropDown(objADBMaster);
				fillProjectSpecifications(objADBMaster);
				new BLMisc().BindPortDetails(objADBMaster, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
				objADBMaster.pNetworkId = objRequestIn.pNetworkId;
				var objDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString(), DropDownType.Entity_Type.ToString());
				objADBMaster.lstEntityType = objDDL;

				//Get the layer details to bind additional attributes ADB
				var layerdetails = new BLLayer().getLayer(EntityType.ADB.ToString());
				objADBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes ADB
				response.status = StatusCodes.OK.ToString();
				response.results = objADBMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddADB()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Save ADB
		/// <summary> SaveADB</summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<ADBMaster> SaveADB(ReqInput data)
		{
			var response = new ApiResponse<ADBMaster>();
			ADBMaster objADBMaster = ReqHelper.GetRequestData<ADBMaster>(data);
			try
			{
				if (objADBMaster.networkIdType == NetworkIdType.A.ToString() && objADBMaster.system_id == 0)
				{   // get parent geometry 
					if (string.IsNullOrWhiteSpace(objADBMaster.geom) && objADBMaster.system_id == 0)
					{
						objADBMaster.geom = GetPointTypeParentGeom(objADBMaster.pSystemId, objADBMaster.pEntityType);
					}
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ADB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objADBMaster.geom, parent_eType = objADBMaster.pEntityType, parent_sysId = objADBMaster.pSystemId });
					if (objADBMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objADBMaster = GetADBDetail(objADBMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objADBMaster.adb_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//    var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objADBMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objADBMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objADBMaster.network_id = objNetworkCodeDetail.network_code;
					objADBMaster.sequence_id = objNetworkCodeDetail.sequence_id;

				}
				if (string.IsNullOrEmpty(objADBMaster.adb_name))
				{
					objADBMaster.adb_name = objADBMaster.network_id;
				}
				//ADBTemplateMaster objItem = new ADBTemplateMaster();

				this.Validate(objADBMaster);
                if (objADBMaster.pSystemId > 0 && !String.IsNullOrEmpty(objADBMaster.pNetworkId))
                {
                    objADBMaster.parent_system_id = objADBMaster.pSystemId;
                    objADBMaster.parent_network_id = objADBMaster.pNetworkId;
                    objADBMaster.parent_entity_type = objADBMaster.pEntityType;
                }
                if (ModelState.IsValid)
				{
					var isNew = objADBMaster.system_id > 0 ? false : true;
					objADBMaster.is_new_entity = (isNew && objADBMaster.source_ref_id != "0" && objADBMaster.source_ref_id != "");
					if (objADBMaster.unitValue != null && objADBMaster.unitValue.Contains(":"))
					{
						objADBMaster.no_of_input_port = Convert.ToInt32(objADBMaster.unitValue.Split(':')[0]);
						objADBMaster.no_of_output_port = Convert.ToInt32(objADBMaster.unitValue.Split(':')[1]);
					}
					var resultItem = new BLADB().SaveEntityADB(objADBMaster, objADBMaster.user_id);
					//START SPLIT CABLE FEATURE BY ANTRA//
					var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.ADB.ToString().ToUpper()).FirstOrDefault();
					if (objADBMaster.split_cable_system_id > 0 && layerDetails.is_split_allowed == true && isNew)
					{
						SplitCable(objADBMaster.system_id, objADBMaster.split_cable_system_id, "ADB", objADBMaster.network_status, objADBMaster.user_id);
						string[] Layer_Name = { EntityType.Cable.ToString() };
						objADBMaster.objPM.status = ResponseStatus.OK.ToString();
						objADBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						//Save Reference
						if (objADBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objADBMaster.EntityReference, resultItem.system_id);
						}
					}
					// END //
					//Save Reference
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.ADB.ToString() };
						if (objADBMaster.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(objADBMaster.lstSpliceTrayInfo, resultItem.system_id, objADBMaster.user_id, EntityType.SpliceTray.ToString(), EntityType.ADB.ToString());
						}
						if (objADBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objADBMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objADBMaster.objPM.status = ResponseStatus.OK.ToString();
							objADBMaster.objPM.isNewEntity = isNew;
							objADBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}


						else
						{
							if (resultItem.isPortConnected == true)
							{
								objADBMaster.objPM.status = ResponseStatus.OK.ToString();
								objADBMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objADBMaster.system_id, EntityType.ADB.ToString(), objADBMaster.network_id, objADBMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objADBMaster.longitude + " " + objADBMaster.latitude }, objADBMaster.user_id);
								objADBMaster.objPM.status = ResponseStatus.OK.ToString();
								objADBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					//else
					//{
					//    objADBMaster.objPM.status = ResponseStatus.FAILED.ToString();
					//    objADBMaster.objPM.message = resultItem.objPM.message;
					//    response.error_message = resultItem.objPM.message;
					//    response.status = ResponseStatus.FAILED.ToString();
					//}
				}
				else
				{
					objADBMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objADBMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objADBMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objADBMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objADBMaster, EntityType.ADB.ToString());
					new BLMisc().BindPortDetails(objADBMaster, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA
					BindADBDropDown(objADBMaster);
					fillProjectSpecifications(objADBMaster);
					//Get the layer details to bind additional attributes ADB
					var layerdetails = new BLLayer().getLayer(EntityType.ADB.ToString());
					objADBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes ADB
					response.results = objADBMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveADB()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion

		#endregion

		#region Area
		#region Add Area
		/// <summary> Add Area </summary>
		/// <param name="data">SystemId,networkIdType,geom</param>
		/// <returns>Area Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<Area> AddArea(ReqInput data)
		{
			var response = new ApiResponse<Area>();
			try
			{
				Area objRequestIn = ReqHelper.GetRequestData<Area>(data);
				Area objArea = GetAreaDetail(objRequestIn);
				response.status = StatusCodes.OK.ToString();
				response.results = objArea;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get Area Details
		/// <summary> GetAreaDetail</summary>
		/// <param >networkIdType,systemId,geom,userId</param>
		/// <returns>Area Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public Area GetAreaDetail(Area objArea)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Area.ToString());
			DropDownMaster drp = new DropDownMaster();
			//objDDL.Insert(0, new DropDownMaster { dropdown_key = "Select", dropdown_status = false, dropdown_type = DropDownType.Area_RFS.ToString(), dropdown_value = "0" });
			int user_id = objArea.user_id;
			if (objArea.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objArea, GeometryType.Polygon.ToString(), objArea.geom);
				//Fill Parent detail...              
				fillParentDetail(objArea, new NetworkCodeIn() { eType = EntityType.Area.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objArea.geom }, objArea.networkIdType);
			}
			else
			{
				// Get entity detail by Id...
				objArea = new BLMisc().GetEntityDetailById<Area>(objArea.system_id, EntityType.Area, objArea.user_id);
				fillRegionProvAbbr(objArea);
			}

			objArea.lstAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.Area_RFS.ToString()).ToList();
			objArea.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objArea;
		}
		#endregion

		#region Save Area
		/// <summary> SaveArea </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<Area> SaveArea(ReqInput data)
		{
			var response = new ApiResponse<Area>();
			Area objArea = ReqHelper.GetRequestData<Area>(data);
			try
			{
				ModelState.Clear();
				var objDDL = new BLMisc().GetDropDownList(EntityType.Area.ToString());
				DropDownMaster drp = new DropDownMaster();
				objArea.lstAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.Area_RFS.ToString()).ToList();
				if (objArea.networkIdType == NetworkIdType.A.ToString() && objArea.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Area.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objArea.geom });
					if (objArea.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objArea = GetAreaDetail(objArea);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objArea.area_name = objNetworkCodeDetail.network_code;
					}
					//SET NETWORK CODE
					objArea.network_id = objNetworkCodeDetail.network_code;
					objArea.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objArea.area_name))
				{
					objArea.area_name = objArea.network_id;
				}
				objArea.network_status = "P";
				this.Validate(objArea);
				if (ModelState.IsValid)
				{
					var isNew = objArea.system_id > 0 ? false : true;
					objArea.is_new_entity = (isNew && objArea.source_ref_id != "0" && objArea.source_ref_id != "");
					var resultItem = new BLArea().SaveArea(objArea, objArea.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.Area.ToString() };

						if (isNew)
						{
							objArea.objPM.status = ResponseStatus.OK.ToString();
							objArea.objPM.isNewEntity = isNew;
							objArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							objArea.objPM.status = ResponseStatus.OK.ToString();
							objArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
					}
					else
					{
						objArea.objPM.status = ResponseStatus.FAILED.ToString();
						objArea.objPM.message = objArea.objPM.message;
						response.error_message = objArea.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objArea.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objArea.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objArea.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objArea;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					response.results = objArea;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#endregion
		#region RestrictedArea
		#region Add RestrictedArea
		/// <summary> Add Restricted Area </summary>
		/// <param name="data">SystemId,networkIdType,geom</param>
		/// <returns>Restricted Area Details</returns>


		public ApiResponse<RestrictedArea> AddRestrictedArea(ReqInput data)
		{
			var response = new ApiResponse<RestrictedArea>();
			try
			{
				RestrictedArea objRequestIn = ReqHelper.GetRequestData<RestrictedArea>(data);
				RestrictedArea objRestrictedArea = GetRestrictedAreaDetail(objRequestIn);
				response.status = StatusCodes.OK.ToString(); 
				response.results = objRestrictedArea;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddRestrictedArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#region Get RestrictedArea Details
		/// <summary> GetRestrictedAreaDetail</summary>
		/// <param >networkIdType,systemId,geom,userId</param>
		/// <returns>RestrictedArea Details</returns>

		public RestrictedArea GetRestrictedAreaDetail(RestrictedArea objRestrictedArea)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Restricted_Area.ToString());
			DropDownMaster drp = new DropDownMaster();
			//objDDL.Insert(0, new DropDownMaster { dropdown_key = "Select", dropdown_status = false, dropdown_type = DropDownType.Area_RFS.ToString(), dropdown_value = "0" });

			if (objRestrictedArea.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objRestrictedArea, GeometryType.Polygon.ToString(), objRestrictedArea.geom);
				//Fill Parent detail...              
				fillParentDetail(objRestrictedArea, new NetworkCodeIn() { eType = EntityType.Restricted_Area.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objRestrictedArea.geom }, objRestrictedArea.networkIdType);
			}
			else
			{
				// Get entity detail by Id...
				objRestrictedArea = new BLMisc().GetEntityDetailById<RestrictedArea>(objRestrictedArea.system_id, EntityType.Restricted_Area, objRestrictedArea.user_id);
				fillRegionProvAbbr(objRestrictedArea);
			}

			objRestrictedArea.lstRestrictedAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.RestrictedArea_RFS.ToString()).ToList();
			objRestrictedArea.lstcategoryRFS = objDDL.Where(x => x.dropdown_type == DropDownType.Category.ToString()).ToList();
			objRestrictedArea.lstsubcategoryRFS = objDDL.Where(x => x.dropdown_type == DropDownType.SubCategory.ToString()).ToList();
			objRestrictedArea.lstQualificationType = objDDL.Where(x => x.dropdown_type == DropDownType.QualificationType.ToString()).ToList();
			objRestrictedArea.lstAllowedNetworkType= objDDL.Where(x => x.dropdown_type == DropDownType.AllowedNetworkType.ToString()).ToList();

            return objRestrictedArea;
		}
		#endregion
		#region Save RestrictedArea
		/// <summary> SaveRestrictedArea </summary>
		/// <param name="data">ReqInput</param>

		public ApiResponse<RestrictedArea> SaveRestrictedArea(ReqInput data)
		{
			var response = new ApiResponse<RestrictedArea>();
			RestrictedArea objRestrictedArea = ReqHelper.GetRequestData<RestrictedArea>(data);
			try
			{
				ModelState.Clear();
				var objDDL = new BLMisc().GetDropDownList(EntityType.Restricted_Area.ToString());
				DropDownMaster drp = new DropDownMaster();
				objRestrictedArea.lstRestrictedAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.RestrictedArea_RFS.ToString()).ToList();
				objRestrictedArea.lstcategoryRFS = objDDL.Where(x => x.dropdown_type == DropDownType.Category.ToString()).ToList();
				objRestrictedArea.lstsubcategoryRFS = objDDL.Where(x => x.dropdown_type == DropDownType.SubCategory.ToString()).ToList();
				objRestrictedArea.lstQualificationType = objDDL.Where(x => x.dropdown_type == DropDownType.QualificationType.ToString()).ToList();
				objRestrictedArea.lstAllowedNetworkType = objDDL.Where(x => x.dropdown_type == DropDownType.AllowedNetworkType.ToString()).ToList();

				if (objRestrictedArea.networkIdType == NetworkIdType.A.ToString() && objRestrictedArea.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Restricted_Area.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objRestrictedArea.geom });
					if (objRestrictedArea.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objRestrictedArea = GetRestrictedAreaDetail(objRestrictedArea);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objRestrictedArea.restricted_area_name = objNetworkCodeDetail.network_code;
					}
					else
					{
						var objNetworkStatus = new BLMisc().GetEntityDetailById<RestrictedArea>(objRestrictedArea.system_id, EntityType.Restricted_Area, objRestrictedArea.user_id);
						objRestrictedArea.network_status = objNetworkStatus.network_status;
					}
					//SET NETWORK CODE
					objRestrictedArea.parent_entity_type = objNetworkCodeDetail.parent_entity_type;
					objRestrictedArea.parent_network_id = objNetworkCodeDetail.parent_network_id;

					//SET NETWORK CODE
					objRestrictedArea.network_id = objNetworkCodeDetail.network_code;
					objRestrictedArea.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objRestrictedArea.restricted_area_name))
				{
					objRestrictedArea.restricted_area_name = objRestrictedArea.network_id;
				}
				this.Validate(objRestrictedArea);
				if (ModelState.IsValid)
				{
					var isNew = objRestrictedArea.system_id > 0 ? false : true;
					objRestrictedArea.is_new_entity = (isNew && objRestrictedArea.source_ref_id != "0" && objRestrictedArea.source_ref_id != "");
					objRestrictedArea.network_status = "P";
					var resultItem = new BLrestricted_area().SaveRestrictedArea(objRestrictedArea, objRestrictedArea.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.Restricted_Area.ToString() };

						if (isNew)
						{
							objRestrictedArea.objPM.status = ResponseStatus.OK.ToString();
							objRestrictedArea.objPM.isNewEntity = isNew;
							objRestrictedArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							objRestrictedArea.objPM.status = ResponseStatus.OK.ToString();
							objRestrictedArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
					}
					else
					{
						objRestrictedArea.objPM.status = ResponseStatus.FAILED.ToString();
						objRestrictedArea.objPM.message = objRestrictedArea.objPM.message;
						response.error_message = objRestrictedArea.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objRestrictedArea.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objRestrictedArea.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objRestrictedArea.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objRestrictedArea;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					response.results = objRestrictedArea;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveRestrictedArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion

		#region ONT 
		#region Add ONT
		/// <summary> Add ONT </summary>
		/// <param name="data">pEntityType,networkIdType,geom,systemId,pSystemId,userId</param>
		/// <returns>Cable Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<ONTMaster> AddONT(ReqInput data)
		{
			var response = new ApiResponse<ONTMaster>();
			try
			{
				ONTMaster objONT = ReqHelper.GetRequestData<ONTMaster>(data);
				if (string.IsNullOrWhiteSpace(objONT.geom) && objONT.system_id == 0)
				{
					// get geom by parent id...
					objONT.geom = GetPointTypeParentGeom(objONT.pSystemId, objONT.pEntityType);
				}
				ONTMaster objONTMaster = GetONTDetail(objONT);
				objONTMaster.pSystemId = objONT.pSystemId;
				objONTMaster.pEntityType = objONT.pEntityType;
				BLItemTemplate.Instance.BindItemDropdowns(objONTMaster, EntityType.ONT.ToString());

				fillProjectSpecifications(objONTMaster);
				//if (objONTMaster.pSystemId != 0 && objONTMaster.pEntityType != null && objONTMaster.pEntityType.ToLower() == EntityType.Structure.ToString().ToLower())
				//if (objONTMaster.parent_entity_type.ToLower() == EntityType.Structure.ToString().ToLower())
				//{
				//    objONTMaster.objIspEntityMap.structure_id = objONTMaster.parent_system_id;
				//    objONTMaster.objIspEntityMap.AssociateStructure = objONTMaster.parent_system_id;
				//}
				BindONTDropDown(objONTMaster);
				new BLMisc().BindPortDetails(objONTMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
				//Get the layer details to bind additional attributes ONT
				var layerdetails = new BLLayer().getLayer(EntityType.ONT.ToString());
				objONTMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes ONT
				response.status = StatusCodes.OK.ToString();
				response.results = objONTMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddONT()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get ONT Details
		/// <summary> GetONTDetail</summary>
		/// <param >pEntityType,networkIdType,geom,systemId,pSystemId,userId</param>
		/// <returns>ONT Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ONTMaster GetONTDetail(ONTMaster objONT)
		{
			if (objONT.objIspEntityMap.structure_id != 0)
			{
				objONT.parent_system_id = Convert.ToInt32(objONT.objIspEntityMap.structure_id);
				objONT.parent_entity_type = EntityType.Structure.ToString();
			}
			else
			{
				objONT.parent_system_id = 0;
				objONT.parent_entity_type = "Province";
			}
			if (objONT.system_id == 0)
			{
				objONT.longitude = Convert.ToDouble(objONT.geom.Split(' ')[0]);
				objONT.latitude = Convert.ToDouble(objONT.geom.Split(' ')[1]);
				objONT.ownership_type = "Own";

				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objONT, GeometryType.Point.ToString(), objONT.geom);
				//Fill Parent detail...              
				fillParentDetail(objONT, new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONT.geom, parent_eType = objONT.pEntityType, parent_sysId = objONT.pSystemId }, objONT.networkIdType);
				//Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<ONTTemplateMaster>(objONT.user_id, EntityType.ONT);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objONT);
				objONT.other_info = null;   //for additional-attributes
			}
			else
			{
				objONT = new BLMisc().GetEntityDetailById<ONTMaster>(objONT.system_id, EntityType.ONT, objONT.user_id);
				//for additional-attributes
				objONT.other_info = new BLONT().GetOtherInfoONT(objONT.system_id);
				fillRegionProvAbbr(objONT);
			}
			objONT.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");
			return objONT;
		}
		#endregion

		#region Bind ONT Dropdown
		/// <summary>BindONTDropDown </summary>
		/// <param name="objONTMaster"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindONTDropDown(ONTMaster objONT)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objONT.parent_system_id, objONT.system_id, EntityType.ONT.ToString());
			if (ispEntityMap != null)
			{
				objONT.objIspEntityMap.id = ispEntityMap.id;
				objONT.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objONT.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objONT.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objONT.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}
			objONT.objIspEntityMap.AssoType = objONT.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objONT.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objONT.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objONT.longitude + " " + objONT.latitude);
			if (objONT.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objONT.objIspEntityMap.structure_id);
				objONT.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objONT.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objONT.parent_entity_type == EntityType.UNIT.ToString())
				{
					objONT.objIspEntityMap.unitId = objONT.parent_system_id;
					//objONT.objIspEntityMap.AssoType = "";
					//objONT.objIspEntityMap.floor_id = 0;
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.ONT.ToString()).FirstOrDefault() != null)
			{
				objONT.objIspEntityMap.isValidParent = true;
				objONT.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objONT.objIspEntityMap.structure_id, objONT.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.ONT.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objONT.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objONT.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objONT.objIspEntityMap.entity_type == null) { objONT.objIspEntityMap.entity_type = EntityType.ONT.ToString(); }
			objONT.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objONT.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			objONT.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");
			var _objDDL = new BLMisc().GetDropDownList("");
			objONT.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objONT.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save ONT
		/// <summary> SaveONT </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<ONTMaster> SaveONT(ReqInput data)
		{
			var response = new ApiResponse<ONTMaster>();
			ONTMaster objONTMaster = ReqHelper.GetRequestData<ONTMaster>(data);
			try
			{
				ModelState.Clear();
				objONTMaster.cpe_type = string.IsNullOrEmpty(objONTMaster.cpe_type) ? "ONT" : objONTMaster.cpe_type;
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objONTMaster.geom) && objONTMaster.system_id == 0)
				{
					objONTMaster.geom = GetPointTypeParentGeom(objONTMaster.pSystemId, objONTMaster.pEntityType);
				}

				if (objONTMaster.networkIdType == NetworkIdType.A.ToString() && objONTMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONTMaster.geom, parent_eType = objONTMaster.pEntityType, parent_sysId = objONTMaster.pSystemId });
					if (objONTMaster.isDirectSave == true)
					{
						if (objONTMaster.pEntityType != null && objONTMaster.pEntityType.ToLower() == EntityType.Structure.ToString().ToLower())
						{
							objONTMaster.objIspEntityMap.structure_id = objONTMaster.pSystemId;
						}
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objONTMaster = GetONTDetail(objONTMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objONTMaster.ont_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//    var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objONTMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objONTMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objONTMaster.network_id = objNetworkCodeDetail.network_code;
					objONTMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objONTMaster.ont_name))
				{
					objONTMaster.ont_name = objONTMaster.network_id;
				}
				this.Validate(objONTMaster);
				if (ModelState.IsValid)
				{
					// var portinfo = new BLPortInfo().ChkPortEXist(objONTMaster.model);
					// if (portinfo)
					// {
					objONTMaster.objIspEntityMap.structure_id = Convert.ToInt32(objONTMaster.objIspEntityMap.AssociateStructure);
					objONTMaster.objIspEntityMap.shaft_id = objONTMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objONTMaster.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(objONTMaster.objIspEntityMap.AssoType))
					{
						objONTMaster.objIspEntityMap.shaft_id = 0; objONTMaster.objIspEntityMap.floor_id = 0;
					}
					if (objONTMaster.unitValue != null && objONTMaster.unitValue.Contains(":"))
					{
						objONTMaster.no_of_input_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[0]);
						objONTMaster.no_of_output_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[1]);
					}
					if (objONTMaster.objIspEntityMap.structure_id != 0)
					{
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objONTMaster.objIspEntityMap.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							objONTMaster.latitude = Convert.ToDouble(structureDetails.latitude);
							objONTMaster.longitude = Convert.ToDouble(structureDetails.longitude);
						}
					}
					if (objONTMaster.objIspEntityMap.structure_id == 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.ONT.ToString(), gType = GeometryType.Point.ToString(), eGeom = objONTMaster.longitude + " " + objONTMaster.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							objONTMaster.parent_system_id = parentDetails.p_system_id;
							objONTMaster.parent_network_id = parentDetails.p_network_id;
							objONTMaster.parent_entity_type = parentDetails.p_entity_type;
						}
					}
					var isNew = objONTMaster.system_id > 0 ? false : true;
					objONTMaster.is_new_entity = (isNew && objONTMaster.source_ref_id != "0" && objONTMaster.source_ref_id != "");
					var resultItem = new BLONT().SaveONTEntity(objONTMaster, objONTMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{


						string[] LayerName = { EntityType.ONT.ToString() };
						//Save Reference
						if (objONTMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objONTMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objONTMaster.objPM.status = ResponseStatus.OK.ToString();
							objONTMaster.objPM.isNewEntity = isNew;
							objONTMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objONTMaster.objPM.status = ResponseStatus.FAILED.ToString();
								objONTMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.FAILED.ToString();
							}
							else
							{
								objONTMaster.objPM.status = ResponseStatus.OK.ToString();
								objONTMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
							}
						}
					}
					else
					{
						objONTMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objONTMaster.objPM.message = objONTMaster.objPM.message;
						response.error_message = objONTMaster.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{

					objONTMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objONTMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objONTMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objONTMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objONTMaster, EntityType.ONT.ToString());
					BindONTDropDown(objONTMaster);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objONTMaster);
					new BLMisc().BindPortDetails(objONTMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
					//Get the layer details to bind additional attributes ONT
					var layerdetails = new BLLayer().getLayer(EntityType.ONT.ToString());
					objONTMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes ONT
					response.results = objONTMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveONT()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion

		#region FDB 
		#region Add FDB
		/// <summary> Add FDB </summary>
		/// <param name="data">pEntityType,networkIdType,geom,systemId,pSystemId,userId</param>
		/// <returns>Fdb Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<FDBInfo> AddFDB(ReqInput data)
		{
			var response = new ApiResponse<FDBInfo>();
			try
			{
				FDBInfo objFDB = ReqHelper.GetRequestData<FDBInfo>(data);
				if (string.IsNullOrWhiteSpace(objFDB.geom) && objFDB.system_id == 0)
				{
					// get geom by parent id...
					objFDB.geom = GetPointTypeParentGeom(objFDB.pSystemId, objFDB.pEntityType);
				}
				FDBInfo objFDBMaster = getFDBInfo(objFDB);
				//objFDBMaster.parent_system_id = pSystemId;
				//objFDBMaster.parent_entity_type = pEntityType;

				BLItemTemplate.Instance.BindItemDropdowns(objFDBMaster, EntityType.FDB.ToString());

				objFDBMaster.lstType = objFDBMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Secondary.ToString()).ToList();

				fillProjectSpecifications(objFDBMaster);
				if (objFDB.system_id == 0 && objFDB.pSystemId > 0)
				{
					objFDBMaster.objIspEntityMap.structure_id = objFDB.pSystemId;
					objFDBMaster.objIspEntityMap.AssociateStructure = objFDB.pSystemId;
				}
				BindFDBDropdown(objFDBMaster);
                BindFDBRoute(objFDBMaster);
                List<int> listI = new List<int>();
                foreach (var i in objFDBMaster.lstRouteInfo.Where(x => x.is_associated))
                {
                    listI.Add(i.cable_id);
                }
                objFDBMaster.selected_route_ids = listI;

                //Get the layer details to bind additional attributes FDB
                var layerdetails = new BLLayer().getLayer(EntityType.FDB.ToString());
				objFDBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes FDB

				response.status = StatusCodes.OK.ToString();
				response.results = objFDBMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddFDB()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get FDB Details
		/// <summary> GetFDBDetail</summary>
		/// <param >pEntityType,networkIdType,geom,systemId,pSystemId,userId</param>
		/// <returns>FDB Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public FDBInfo getFDBInfo(FDBInfo objFDB)
		{
			int id = objFDB.user_id;
			if (objFDB.system_id == 0)
			{
				objFDB.longitude = Convert.ToDouble(objFDB.geom.Split(' ')[0]);
				objFDB.latitude = Convert.ToDouble(objFDB.geom.Split(' ')[1]);
				objFDB.ownership_type = "Own";
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objFDB, GeometryType.Point.ToString(), objFDB.geom);
				//Fill Parent detail...              
				fillParentDetail(objFDB, new NetworkCodeIn() { eType = EntityType.FDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFDB.geom, parent_eType = objFDB.pEntityType, parent_sysId = objFDB.pSystemId }, objFDB.networkIdType);
				//Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(objFDB.user_id, EntityType.FDB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objFDB);
				objFDB.other_info = null;   //for additional-attributes
			}
			else
			{
				objFDB = new BLMisc().GetEntityDetailById<FDBInfo>(objFDB.system_id, EntityType.FDB, objFDB.user_id);
				//for additional-attributes
				objFDB.other_info = new BLISP().GetOtherInfoFDB(objFDB.system_id);
				fillRegionProvAbbr(objFDB);
			}
			objFDB.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());
			return objFDB;
		}
		#endregion

		#region Bind FDB Dropdown
		/// <summary>BindFDBDropDown </summary>
		/// <param name="objFDB"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindFDBDropdown(FDBInfo objFDB)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objFDB.parent_system_id, objFDB.system_id, EntityType.FDB.ToString());
			if (ispEntityMap != null && ispEntityMap.id > 0)
			{
				objFDB.objIspEntityMap.id = ispEntityMap.id;
				objFDB.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objFDB.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objFDB.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objFDB.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}
			objFDB.objIspEntityMap.AssoType = objFDB.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objFDB.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objFDB.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objFDB.longitude + " " + objFDB.latitude);
			if (objFDB.objIspEntityMap.structure_id > 0)
			{
				var shaftFloorList = new BLBDB().GetShaftFloorByStrucId(objFDB.objIspEntityMap.structure_id);
				objFDB.objIspEntityMap.lstShaft = shaftFloorList.Where(m => m.isshaft == true).ToList();
				objFDB.objIspEntityMap.lstFloor = shaftFloorList.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objFDB.parent_entity_type == EntityType.UNIT.ToString())
				{
					objFDB.objIspEntityMap.unitId = objFDB.parent_system_id;
					//objONT.objIspEntityMap.AssoType = "";
					//objONT.objIspEntityMap.floor_id = 0;
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.FDB.ToString()).FirstOrDefault() != null)
			{
				objFDB.objIspEntityMap.isValidParent = true;
				objFDB.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objFDB.objIspEntityMap.structure_id, objFDB.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objFDB.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objFDB.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			objFDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objFDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			new BLMisc().BindPortDetails(objFDB, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());

			var obj_DDL = new BLMisc().GetDropDownList("");
			objFDB.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objFDB.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		private void BindFDBRoute(FDBInfo objFDB)
		{
            if (objFDB.system_id == 0)
                objFDB.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objFDB.geom);
            else
                objFDB.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objFDB.system_id, EntityType.FDB.ToString());
        }
		#endregion

		#region Save FDB
		/// <summary> SaveFDB </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<FDBInfo> SaveFDB(ReqInput data)
		{
			var response = new ApiResponse<FDBInfo>();
			FDBInfo objFDBMaster = ReqHelper.GetRequestData<FDBInfo>(data);
			try
			{
				ModelState.Clear();
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objFDBMaster.geom) && objFDBMaster.system_id == 0)
				{
					objFDBMaster.geom = GetPointTypeParentGeom(objFDBMaster.pSystemId, objFDBMaster.pEntityType);
				}

				if (objFDBMaster.networkIdType == NetworkIdType.A.ToString() && objFDBMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn()
					{
						eType = EntityType.FDB.ToString(),
						gType = GeometryType.Point.ToString(),
						eGeom = objFDBMaster.geom,
						parent_eType = objFDBMaster.pEntityType,
						parent_sysId = objFDBMaster.pSystemId
					});
					if (objFDBMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objFDBMaster = getFDBInfo(objFDBMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objFDBMaster.fdb_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//   var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objFDBMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objFDBMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objFDBMaster.network_id = objNetworkCodeDetail.network_code;
					objFDBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objFDBMaster.fdb_name))
				{
					objFDBMaster.fdb_name = objFDBMaster.network_id;
				}
				this.Validate(objFDBMaster);
				if (objFDBMaster.pSystemId > 0 && !String.IsNullOrEmpty(objFDBMaster.pNetworkId))
				{
					objFDBMaster.parent_system_id = objFDBMaster.pSystemId;
					objFDBMaster.parent_network_id = objFDBMaster.pNetworkId;
					objFDBMaster.parent_entity_type = objFDBMaster.pEntityType;
				}
				if (ModelState.IsValid)
				{
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;
					objFDBMaster.objIspEntityMap.structure_id = Convert.ToInt32(objFDBMaster.objIspEntityMap.AssociateStructure);
					objFDBMaster.objIspEntityMap.shaft_id = objFDBMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objFDBMaster.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(objFDBMaster.objIspEntityMap.AssoType))
					{
						objFDBMaster.objIspEntityMap.shaft_id = 0; objFDBMaster.objIspEntityMap.floor_id = 0;
					}
					var isNew = objFDBMaster.system_id > 0 ? false : true;
					objFDBMaster.is_new_entity = (isNew && objFDBMaster.source_ref_id != "0" && objFDBMaster.source_ref_id != "");
					if (objFDBMaster.unitValue != null && objFDBMaster.unitValue.Contains(":"))
					{
						objFDBMaster.no_of_input_port = Convert.ToInt32(objFDBMaster.unitValue.Split(':')[0]);
						objFDBMaster.no_of_output_port = Convert.ToInt32(objFDBMaster.unitValue.Split(':')[1]);
					}
					if (objFDBMaster.objIspEntityMap.structure_id == 0 && objFDBMaster.system_id > 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.FDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFDBMaster.longitude + " " + objFDBMaster.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							objFDBMaster.parent_system_id = parentDetails.p_system_id;
							objFDBMaster.parent_network_id = parentDetails.p_network_id;
							objFDBMaster.parent_entity_type = parentDetails.p_entity_type;
						}

					}
					objFDBMaster.userId = objFDBMaster.user_id;

					objFDBMaster.barcode = objFDBMaster.barcode;

					var resultItem = BLISP.Instance.SaveFDBDetails(objFDBMaster);
                    BindFDBRoute(objFDBMaster);
                    List<RouteInfo> objL = new List<RouteInfo>();
                    foreach (var itm in objFDBMaster.lstRouteInfo)
                    {
                        bool f = false;
						if (objFDBMaster.selected_route_ids != null)
						{
							foreach (var ids in objFDBMaster.selected_route_ids)
							{
								if (ids == itm.cable_id)
								{
									f = true;

								}
							}
						}
                        RouteInfo objS = new RouteInfo();
                        objS.entity_id = resultItem.system_id;
                        objS.entity_type = EntityType.FDB.ToString();
                        objS.created_by = objFDBMaster.user_id.ToString();
                        objS.cable_id = itm.cable_id;
                        objS.is_associated = f;
                        objL.Add(objS);
                    }

                    var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objL), resultItem.system_id, EntityType.FDB.ToString(), objFDBMaster.user_id);
                    BindFDBRoute(objFDBMaster);
                    List<int> listI = new List<int>();
                    foreach (var i in objFDBMaster.lstRouteInfo.Where(x => x.is_associated))
                    {
                        listI.Add(i.cable_id);
                    }
                    objFDBMaster.selected_route_ids = listI;
                    if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						#region By default add the splitter in BOX -JIO BY ANTRA

						if (ApplicationSettings.isDefaultSplitterAllowed && string.IsNullOrEmpty(resultItem.modified_by.ToString()))
						{
							AddSplitterInBox(resultItem);
						}
						#endregion`

						if (objFDBMaster.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(objFDBMaster.lstSpliceTrayInfo, resultItem.system_id, objFDBMaster.user_id, EntityType.SpliceTray.ToString(), EntityType.FDB.ToString());
						}
						//Save Reference
						if (objFDBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objFDBMaster.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							objFDBMaster.objPM.status = ResponseStatus.OK.ToString();
							objFDBMaster.objPM.isNewEntity = isNew;
							objFDBMaster.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, layer_title);
							response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_005, layer_title);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objFDBMaster.objPM.status = ResponseStatus.OK.ToString();
								objFDBMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objFDBMaster.system_id, EntityType.FDB.ToString(), objFDBMaster.network_id, objFDBMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFDBMaster.longitude + " " + objFDBMaster.latitude }, objFDBMaster.user_id);
								objFDBMaster.objPM.status = ResponseStatus.OK.ToString();
								objFDBMaster.objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
								response.error_message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
								response.status = ResponseStatus.OK.ToString();
							}
						}
					}
					else
					{
						objFDBMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objFDBMaster.objPM.message = objFDBMaster.objPM.message;
						response.error_message = objFDBMaster.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objFDBMaster.objPM.status = ResponseStatus.FAILED.ToString();
					objFDBMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
				}
				if (objFDBMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objFDBMaster;
					response.status = ResponseStatus.OK.ToString();
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objFDBMaster, EntityType.FDB.ToString());
					BindFDBDropdown(objFDBMaster);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objFDBMaster);

					//Get the layer details to bind additional attributes FDB
					var layerdetails = new BLLayer().getLayer(EntityType.FDB.ToString());
					objFDBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes FDB

					response.results = objFDBMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveFDB()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion

		#region BDB
		#region Add BDB
		/// <summary> Add BDB </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>BDB Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<BDBMaster> AddBdb(ReqInput data)
		{
			var response = new ApiResponse<BDBMaster>();
			try
			{
				BDBMaster objRequestIn = ReqHelper.GetRequestData<BDBMaster>(data);
				if (string.IsNullOrWhiteSpace(objRequestIn.geom) && objRequestIn.system_id == 0)
				{
					// get geom by parent id...
					objRequestIn.geom = GetPointTypeParentGeom(objRequestIn.pSystemId, objRequestIn.pEntityType);
				}
				BDBMaster objBDBMaster = GetBDBDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objBDBMaster, EntityType.BDB.ToString());

				fillProjectSpecifications(objBDBMaster);
				if (objRequestIn.system_id == 0 && objRequestIn.pSystemId > 0)
				{
					objBDBMaster.objIspEntityMap.structure_id = objRequestIn.pSystemId;
					objBDBMaster.objIspEntityMap.AssociateStructure = objRequestIn.pSystemId;
				}

				BindBDBDropDown(objBDBMaster);
                BindBDBRoute(objBDBMaster);
                List<int> listI = new List<int>();
                foreach (var i in objBDBMaster.lstRouteInfo.Where(x => x.is_associated))
                {
                    listI.Add(i.cable_id);
                }
                objBDBMaster.selected_route_ids = listI;
                //Get the layer details to bind additional attributes BDB
                var layerdetails = new BLLayer().getLayer(EntityType.BDB.ToString());
				objBDBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes BDB

				response.status = StatusCodes.OK.ToString();
				response.results = objBDBMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddBdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get BDB Details
		/// <summary> GetBdbDetail</summary>
		/// <param >networkIdType,systemId,geom,userId</param>
		/// <returns>BDB Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public BDBMaster GetBDBDetail(BDBMaster objBDB)
		{
			int user_id = objBDB.user_id;
			if (objBDB.system_id == 0)
			{
				objBDB.longitude = Convert.ToDouble(objBDB.geom.Split(' ')[0]);
				objBDB.latitude = Convert.ToDouble(objBDB.geom.Split(' ')[1]);
				objBDB.ownership_type = "Own";
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objBDB, GeometryType.Point.ToString(), objBDB.geom);
				//Fill Parent detail...              
				fillParentDetail(objBDB, new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDB.geom, parent_eType = objBDB.pEntityType, parent_sysId = objBDB.pSystemId }, objBDB.networkIdType);
				//Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<BDBTemplateMaster>(objBDB.user_id, EntityType.BDB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objBDB);

				objBDB.other_info = null; //for additional-attributes
				objBDB.lstUserModule = new BLLayer().GetUserModuleAbbrList(objBDB.user_id, UserType.Web.ToString());
				////copy pin code and address from building itself for BDB box..
				//if (pEntityType.ToLower() == EntityType.Structure.ToString().ToLower())
				//{
				//    var objStructureDetail = new BLMisc().GetEntityDetailById<StructureMaster>(pSystemId, EntityType.Structure);
				//    objBDB.pincode=objStructureDetail
				//}
				//else if(pEntityType.ToLower() == EntityType.Building.ToString().ToLower())
				//{
				//    var objStructureDetail = new BLMisc().GetEntityDetailById<BuildingMaster>(pSystemId, EntityType.Building);
				//}
			}
			else
			{
				objBDB = new BLMisc().GetEntityDetailById<BDBMaster>(objBDB.system_id, EntityType.BDB, objBDB.user_id);
				//for additional-attributes
				objBDB.other_info = new BLBDB().GetOtherInfoBDB(objBDB.system_id);
				fillRegionProvAbbr(objBDB);
			}
			objBDB.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objBDB;
		}
		#endregion

		#region Save BDB
		/// <summary> SaveBdb </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<BDBMaster> SaveBdb(ReqInput data)
		{
			var response = new ApiResponse<BDBMaster>();
			BDBMaster objBDBMaster = ReqHelper.GetRequestData<BDBMaster>(data);
			try
			{
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objBDBMaster.geom) && objBDBMaster.system_id == 0)
				{
					objBDBMaster.geom = GetPointTypeParentGeom(objBDBMaster.pSystemId, objBDBMaster.pEntityType);
				}

				if (objBDBMaster.networkIdType == NetworkIdType.A.ToString() && objBDBMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDBMaster.geom, parent_eType = objBDBMaster.pEntityType, parent_sysId = objBDBMaster.pSystemId });
					if (objBDBMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objBDBMaster = GetBDBDetail(objBDBMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objBDBMaster.bdb_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objBDBMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objBDBMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objBDBMaster.network_id = objNetworkCodeDetail.network_code;
					objBDBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objBDBMaster.bdb_name))
				{
					objBDBMaster.bdb_name = objBDBMaster.network_id;
				}
				this.Validate(objBDBMaster);
				if (objBDBMaster.pSystemId > 0 && !String.IsNullOrEmpty(objBDBMaster.pNetworkId))
				{
					objBDBMaster.parent_system_id = objBDBMaster.pSystemId;
					objBDBMaster.parent_network_id = objBDBMaster.pNetworkId;
					objBDBMaster.parent_entity_type = objBDBMaster.pEntityType;
				}
				if (ModelState.IsValid)
				{
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault().layer_title;
					objBDBMaster.objIspEntityMap.structure_id = Convert.ToInt32(objBDBMaster.objIspEntityMap.AssociateStructure);
					objBDBMaster.objIspEntityMap.shaft_id = objBDBMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objBDBMaster.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(objBDBMaster.objIspEntityMap.AssoType))
					{
						objBDBMaster.objIspEntityMap.shaft_id = 0; objBDBMaster.objIspEntityMap.floor_id = 0;
					}
					var isNew = objBDBMaster.system_id > 0 ? false : true;
					objBDBMaster.is_new_entity = (isNew && objBDBMaster.source_ref_id != "0" && objBDBMaster.source_ref_id != "");
					if (objBDBMaster.unitValue != null && objBDBMaster.unitValue.Contains(":"))
					{
						objBDBMaster.no_of_input_port = Convert.ToInt32(objBDBMaster.unitValue.Split(':')[0]);
						objBDBMaster.no_of_output_port = Convert.ToInt32(objBDBMaster.unitValue.Split(':')[1]);
					}
					if (objBDBMaster.objIspEntityMap.structure_id == 0 && objBDBMaster.system_id > 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDBMaster.longitude + " " + objBDBMaster.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							objBDBMaster.parent_system_id = parentDetails.p_system_id;
							objBDBMaster.parent_network_id = parentDetails.p_network_id;
							objBDBMaster.parent_entity_type = parentDetails.p_entity_type;
						}
					}
					var resultItem = new BLBDB().SaveEntityBDB(objBDBMaster, objBDBMaster.user_id);
                    BindBDBRoute(objBDBMaster);
                    List<RouteInfo> objL = new List<RouteInfo>();
                    foreach (var itm in objBDBMaster.lstRouteInfo)
                    {
                        bool f = false;
						if (objBDBMaster.selected_route_ids != null)
						{
							foreach (var ids in objBDBMaster.selected_route_ids)
							{
								if (ids == itm.cable_id)
								{
									f = true;

								}
							}
						}
                        RouteInfo objS = new RouteInfo();
                        objS.entity_id = resultItem.system_id;
                        objS.entity_type = EntityType.BDB.ToString();
                        objS.created_by = objBDBMaster.user_id.ToString();
                        objS.cable_id = itm.cable_id;
                        objS.is_associated = f;
                        objL.Add(objS);
                    }

                    var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objL), resultItem.system_id, EntityType.BDB.ToString(), objBDBMaster.user_id);

                    BindBDBRoute(objBDBMaster);
                    List<int> listI = new List<int>();
                    foreach (var i in objBDBMaster.lstRouteInfo.Where(x => x.is_associated))
                    {
                        listI.Add(i.cable_id);
                    }
                    objBDBMaster.selected_route_ids = listI;
                    //START SPLIT CABLE FEATURE BY ANTRA//
                    var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault();
					if (objBDBMaster.split_cable_system_id > 0 && layerDetails.is_split_allowed == true && isNew)
					{
						SplitCable(objBDBMaster.system_id, objBDBMaster.split_cable_system_id, "BDB", objBDBMaster.network_status, objBDBMaster.user_id);
						string[] Layer_Name = { EntityType.Cable.ToString() };
						objBDBMaster.objPM.status = ResponseStatus.OK.ToString();
						objBDBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						if (objBDBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objBDBMaster.EntityReference, resultItem.system_id);
						}
					}
					// END //
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						#region By default add the splitter in BOX -JIO BY ANTRA

						if (ApplicationSettings.isDefaultSplitterAllowed && string.IsNullOrEmpty(resultItem.modified_by.ToString()))
						{
							AddSplitterInBox(resultItem);
						}
						#endregion`

						if (objBDBMaster.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(objBDBMaster.lstSpliceTrayInfo, resultItem.system_id, objBDBMaster.user_id, EntityType.SpliceTray.ToString(), EntityType.BDB.ToString());
						}
						//Save Reference
						if (objBDBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objBDBMaster.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							objBDBMaster.objPM.status = ResponseStatus.OK.ToString();
							objBDBMaster.objPM.isNewEntity = isNew;
							objBDBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, EntityType.BDB.ToString()); ;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, EntityType.BDB.ToString()); ;
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objBDBMaster.objPM.status = ResponseStatus.OK.ToString();
								objBDBMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objBDBMaster.system_id, EntityType.BDB.ToString(), objBDBMaster.network_id, objBDBMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDBMaster.longitude + " " + objBDBMaster.latitude }, objBDBMaster.user_id);
								objBDBMaster.objPM.status = ResponseStatus.OK.ToString();
								objBDBMaster.objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
							}
						}
					}
					//else
					//{
					//    objBDBMaster.objPM.status = ResponseStatus.FAILED.ToString();
					//    objBDBMaster.objPM.message = resultItem.objPM.message;
					//    response.error_message = resultItem.objPM.message;
					//    response.status = ResponseStatus.FAILED.ToString();
					//}
				}
				else
				{
					objBDBMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objBDBMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objBDBMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					//objBDBMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.status = ResponseStatus.OK.ToString();
					response.results = objBDBMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objBDBMaster, EntityType.BDB.ToString());
					BindBDBDropDown(objBDBMaster);
					fillProjectSpecifications(objBDBMaster);
					//Get the layer details to bind additional attributes BDB
					var layerdetails = new BLLayer().getLayer(EntityType.BDB.ToString());
					objBDBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes BDB
					response.results = objBDBMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveBdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind BDB Dropdown
		/// <summary>BindBDBDropDown </summary>
		/// <param name="objBDB"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindBDBDropDown(BDBMaster objBDB)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objBDB.parent_system_id, objBDB.system_id, EntityType.BDB.ToString());
			if (ispEntityMap != null && ispEntityMap.id > 0)
			{
				objBDB.objIspEntityMap.id = ispEntityMap.id;
				objBDB.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objBDB.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objBDB.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objBDB.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}
			objBDB.objIspEntityMap.AssoType = objBDB.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objBDB.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objBDB.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objBDB.longitude + " " + objBDB.latitude);
			if (objBDB.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.objIspEntityMap.structure_id);
				objBDB.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objBDB.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objBDB.parent_entity_type == EntityType.UNIT.ToString())
				{
					objBDB.objIspEntityMap.unitId = objBDB.parent_system_id;
					//objONT.objIspEntityMap.AssoType = "";
					//objONT.objIspEntityMap.floor_id = 0;
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.BDB.ToString()).FirstOrDefault() != null)
			{
				objBDB.objIspEntityMap.isValidParent = true;
				objBDB.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objBDB.objIspEntityMap.structure_id, objBDB.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objBDB.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objBDB.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objBDB.objIspEntityMap.entity_type == null) { objBDB.objIspEntityMap.entity_type = EntityType.BDB.ToString(); }
			new BLMisc().BindPortDetails(objBDB, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
			var entityTypeDDL = new BLMisc().GetDropDownList(EntityType.BDB.ToString(), DropDownType.Entity_Type.ToString());
			objBDB.lstEntityType = entityTypeDDL;
			objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objBDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList("");
			objBDB.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//   objBDB.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		private void BindBDBRoute(BDBMaster objBDB)
		{
            if (objBDB.system_id == 0)
                objBDB.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objBDB.geom);
            else
                objBDB.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objBDB.system_id, EntityType.BDB.ToString());
        }
		#endregion

		#endregion

		#region CDB
		#region Add CDB
		/// <summary> Add CDB </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>CDB Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<CDBMaster> AddCdb(ReqInput data)
		{
			var response = new ApiResponse<CDBMaster>();
			try
			{
				CDBMaster objRequestIn = ReqHelper.GetRequestData<CDBMaster>(data);
				if (string.IsNullOrWhiteSpace(objRequestIn.geom) && objRequestIn.system_id == 0)
				{
					// get geom by parent id...
					objRequestIn.geom = GetPointTypeParentGeom(objRequestIn.pSystemId, objRequestIn.pEntityType);
				}
				CDBMaster objCDBMaster = GetCDBDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objCDBMaster, EntityType.CDB.ToString());
				new BLMisc().BindPortDetails(objCDBMaster, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
				BindCDBDropDown(objCDBMaster);
				fillProjectSpecifications(objCDBMaster);
				objCDBMaster.pNetworkId = objRequestIn.pNetworkId;
				var entityTypeDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString(), DropDownType.Entity_Type.ToString());
				objCDBMaster.lstEntityType = entityTypeDDL;
				//Get the layer details to bind additional attributes CDB
				var layerdetails = new BLLayer().getLayer(EntityType.CDB.ToString());
				objCDBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes CDB  
				response.status = StatusCodes.OK.ToString();
				response.results = objCDBMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddCdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get CDB Details
		/// <summary> GetCdbDetail</summary>
		/// <param >networkIdType,systemId,geom,userId</param>
		/// <returns>CDB Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public CDBMaster GetCDBDetail(CDBMaster objCDB)
		{
			int user_id = objCDB.user_id;
			if (objCDB.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objCDB, GeometryType.Point.ToString(), objCDB.geom);
				//Fill Parent detail...              
				fillParentDetail(objCDB, new NetworkCodeIn() { eType = EntityType.CDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCDB.geom }, objCDB.networkIdType);
				objCDB.longitude = Convert.ToDouble(objCDB.geom.Split(' ')[0]);
				objCDB.latitude = Convert.ToDouble(objCDB.geom.Split(' ')[1]);
				objCDB.ownership_type = "Own";
				objCDB.other_info = null; //for additional-attributes
										  // Item template binding
				if (objCDB.isConvert)
				{
					objCDB.is_servingdb = true;
					//var objItem = new BLCDBItemMaster().getCDBTemplatebyPortNo(no_of_ports, EntityType.CDB.ToString(), vendor_id); 
					var objItem = new BLVendorSpecification().getEntityTemplatebyPortNo(objCDB.no_of_ports, EntityType.CDB.ToString(), objCDB.vendor_id);
					objCDB.entity_category = "Primary";
					objCDB.vendor_id = objItem.vendor_id;
					objCDB.specification = objItem.specification;
					objCDB.subcategory1 = objItem.subcategory_1;
					objCDB.subcategory2 = objItem.subcategory_2;
					objCDB.subcategory3 = objItem.subcategory_3;
					objCDB.no_of_port = objItem.no_of_port;
					objCDB.category = objItem.category_reference;
					objCDB.item_code = objItem.code;
					//var objItem = BLItemTemplate.Instance.GetTemplateDetail<CDBTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.CDB);
					//Utility.MiscHelper.CopyMatchingProperties(objItem, objCDB);
				}
				else
				{
					var objItem = BLItemTemplate.Instance.GetTemplateDetail<CDBTemplateMaster>(objCDB.user_id, EntityType.CDB);
					Utility.MiscHelper.CopyMatchingProperties(objItem, objCDB);
					fillRegionProvAbbr(objCDB);
				}
			}
			else
			{
				// Get entity detail by Id...
				objCDB = new BLMisc().GetEntityDetailById<CDBMaster>(objCDB.system_id, EntityType.CDB, objCDB.user_id);
				//For additional-attributes
				objCDB.other_info = new BLCDB().GetOtherInfoCDB(objCDB.system_id);
			}
			objCDB.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objCDB;
		}

		#endregion

		#region Save CDB
		/// <summary> SaveCdb </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<CDBMaster> SaveCdb(ReqInput data)
		{
			var response = new ApiResponse<CDBMaster>();
			CDBMaster objCDBMaster = ReqHelper.GetRequestData<CDBMaster>(data);
			try
			{
				if (objCDBMaster.networkIdType == NetworkIdType.A.ToString() && objCDBMaster.system_id == 0)
				{// get parent geometry 
					if (string.IsNullOrWhiteSpace(objCDBMaster.geom) && objCDBMaster.system_id == 0)
					{
						objCDBMaster.geom = GetPointTypeParentGeom(objCDBMaster.pSystemId, objCDBMaster.pEntityType);
					}
                    //GET AUTO NETWORK CODE...
                    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.CDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCDBMaster.geom, parent_eType = objCDBMaster.pEntityType, parent_sysId = objCDBMaster.pSystemId });
                    if (objCDBMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objCDBMaster = GetCDBDetail(objCDBMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objCDBMaster.cdb_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objCDBMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objCDBMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objCDBMaster.network_id = objNetworkCodeDetail.network_code;
					objCDBMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				//CDBTemplateMaster objItem = new CDBTemplateMaster();
				if (string.IsNullOrEmpty(objCDBMaster.cdb_name))
				{
					objCDBMaster.cdb_name = objCDBMaster.network_id;
				}
                if (objCDBMaster.pSystemId > 0 && !String.IsNullOrEmpty(objCDBMaster.pNetworkId))
                {
                    objCDBMaster.parent_system_id = objCDBMaster.pSystemId;
                    objCDBMaster.parent_network_id = objCDBMaster.pNetworkId;
                    objCDBMaster.parent_entity_type = objCDBMaster.pEntityType;
                }
                this.Validate(objCDBMaster);
				if (ModelState.IsValid)
				{
					var isNew = objCDBMaster.system_id > 0 ? false : true;

					if (objCDBMaster.unitValue != null && objCDBMaster.unitValue.Contains(":"))
					{
						objCDBMaster.no_of_input_port = Convert.ToInt32(objCDBMaster.unitValue.Split(':')[0]);
						objCDBMaster.no_of_output_port = Convert.ToInt32(objCDBMaster.unitValue.Split(':')[1]);
					}
					var resultItem = new BLCDB().SaveEntityCDB(objCDBMaster, objCDBMaster.user_id);
					if (resultItem.isConvert && string.IsNullOrEmpty(resultItem.objPM.message) && isNew)
					{
						string[] LayerName = { EntityType.SpliceClosure.ToString(), EntityType.CDB.ToString() };
						SCMaster objSC = new SCMaster();
						objSC = new BLMisc().GetEntityDetailById<SCMaster>(objCDBMaster.sc_system_id, EntityType.SpliceClosure);
						//====  VALIDATE CONNECTION AND SPLICING===// 
						var responseData = new BLMisc().EntityConversion(EntityType.SpliceClosure.ToString(), objSC.network_id, objSC.system_id, resultItem.entityType, resultItem.network_id, resultItem.system_id, objSC.geom, objSC.user_id);
						objCDBMaster.objPM.status = ResponseStatus.OK.ToString();
						objCDBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SC_NET_FRM_021, ApplicationSettings.listLayerDetails, LayerName);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SC_NET_FRM_021, ApplicationSettings.listLayerDetails, LayerName);
						//Save Reference
						if (objCDBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objCDBMaster.EntityReference, resultItem.system_id);
						}

					}
					//START SPLIT CABLE FEATURE BY ANTRA//
					var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.CDB.ToString().ToUpper()).FirstOrDefault();
					if (objCDBMaster.split_cable_system_id > 0 && layerDetails.is_split_allowed == true && isNew)
					{
						SplitCable(objCDBMaster.system_id, objCDBMaster.split_cable_system_id, "CDB", objCDBMaster.network_status, objCDBMaster.user_id);
						string[] Layer_Name = { EntityType.Cable.ToString() };
						objCDBMaster.objPM.status = ResponseStatus.OK.ToString();
						objCDBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						//Save Reference
						if (objCDBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objCDBMaster.EntityReference, resultItem.system_id);
						}
					}
					// END //
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.CDB.ToString() };
						//Save Reference
						if (objCDBMaster.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(objCDBMaster.lstSpliceTrayInfo, resultItem.system_id, objCDBMaster.user_id, EntityType.SpliceTray.ToString(), EntityType.CDB.ToString());
						}
						if (objCDBMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objCDBMaster.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							objCDBMaster.objPM.status = ResponseStatus.OK.ToString();
							objCDBMaster.objPM.isNewEntity = isNew;
							objCDBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objCDBMaster.objPM.status = ResponseStatus.OK.ToString();
								objCDBMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objCDBMaster.system_id, EntityType.CDB.ToString(), objCDBMaster.network_id, objCDBMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCDBMaster.longitude + " " + objCDBMaster.latitude }, objCDBMaster.user_id);
								objCDBMaster.objPM.status = ResponseStatus.OK.ToString();
								objCDBMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}

						}
					}
					//else
					//{
					//    objCDBMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					//    objCDBMaster.objPM.message = objCDBMaster.objPM.message;
					//    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					//    response.error_message = objCDBMaster.objPM.message;
					//}
				}
				else
				{

					objCDBMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objCDBMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();

				}
				if (objCDBMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objCDBMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objCDBMaster, EntityType.CDB.ToString());
					new BLMisc().BindPortDetails(objCDBMaster, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
					BindCDBDropDown(objCDBMaster);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objCDBMaster);
					//Get the layer details to bind additional attributes CDB
					var layerdetails = new BLLayer().getLayer(EntityType.CDB.ToString());
					objCDBMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes CDB  
					response.results = objCDBMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveCdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind CDB Dropdown
		/// <summary>BindCDBDropDown </summary>
		/// <param name="objCDBMaster"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindCDBDropDown(CDBMaster objCDBMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString());
			//objCDBMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objCDBMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objCDBMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objCDBMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objCDBMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#endregion

		#region Loop

		#region Loop Managment Detail
		/// <summary>GetLoopMangmentDetail</summary>
		/// <param name="data">objloop</param>
		/// <returns>Loop Managment Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<List<NELoopDetails>> GetLoopMangmentDetail(ReqInput data)
		{
			var response = new ApiResponse<List<NELoopDetails>>();
			try
			{
				NELoopDetails objRequestIn = ReqHelper.GetRequestData<NELoopDetails>(data);
				List<NELoopDetails> lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(objRequestIn.latitude, objRequestIn.longitude, objRequestIn.system_id, objRequestIn.associated_System_Type, objRequestIn.structure_id);
				response.status = StatusCodes.OK.ToString();
				response.results = lstLoopMangment.OrderByDescending(m => m.loop_length).ToList();
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("GetLoopMangmentDetail()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Cable Loop  Detail
		/// <summary>GetLoopDetailsForCable</summary>
		/// <param name="data">objloop</param>
		/// <returns>Cable Loop  Detail</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<List<NELoopDetails>> GetLoopDetailsForCable(ReqInput data)
		{
			var response = new ApiResponse<List<NELoopDetails>>();
			try
			{
				NELoopDetails objRequestIn = ReqHelper.GetRequestData<NELoopDetails>(data);
				List<NELoopDetails> lstLoopMangment = BLLoopMangment.Instance.GetLoopDetailsForCable(objRequestIn.cable_system_id);
				response.status = StatusCodes.OK.ToString();
				response.results = lstLoopMangment;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("GetLoopDetailsForCable()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#endregion

		#region Add DUCT
		/// <summary> Add DUCT </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>DUCT Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<DuctMaster> AddDuct(ReqInput data)
		{
			var response = new ApiResponse<DuctMaster>();
			try
			{
				LineEntityIn objIn = ReqHelper.GetRequestData<LineEntityIn>(data);
				DuctMaster objDuct = GetDuctDetail(objIn);
				if (objIn.system_id == 0)
				{
					//Fill Location detail...    
					GetLineNetworkDetail(objDuct, objIn, EntityType.Duct.ToString(), false);
				}
				BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
				fillProjectSpecifications(objDuct);
				BindDuctDropDown(objDuct);
				objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
				//Get the layer details to bind additional attributes Duct
				var layerdetails = new BLLayer().getLayer(EntityType.Duct.ToString());
				objDuct.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Duct
				response.status = StatusCodes.OK.ToString();
				response.results = objDuct;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddDuct()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
        #endregion
        #region DUCT
        #region Add Microduct
        /// <summary> Add DUCT </summary>
        /// <param name="data">networkIdType,systemId,geom,userId</param>
        /// <returns>DUCT Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>

        public ApiResponse<MicroductMaster> AddMicroduct(ReqInput data)
        {
            var response = new ApiResponse<MicroductMaster>();
            try
            {
                LineEntityIn objIn = ReqHelper.GetRequestData<LineEntityIn>(data);
                MicroductMaster objDuct = GetMicroductDetail(objIn);
                if (objIn.system_id == 0)
                {
                    //Fill Location detail...    
                    GetLineNetworkDetail(objDuct, objIn, EntityType.Microduct.ToString(), false);
                }
                BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Microduct.ToString());
                fillProjectSpecifications(objDuct);
                BindMicroductDropDown(objDuct);
                objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Microduct.ToString()).ToList();
                //Get the layer details to bind additional attributes Duct
                var layerdetails = new BLLayer().getLayer(EntityType.Microduct.ToString());
                objDuct.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                //End for additional attributes Duct
                response.status = StatusCodes.OK.ToString();
                response.results = objDuct;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("AddDuct()", "Library Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        public ApiResponse<GipipeMaster> AddGipipe(ReqInput data)
		{
			var response = new ApiResponse<GipipeMaster>();
			try
			{
				LineEntityIn objIn = ReqHelper.GetRequestData<LineEntityIn>(data);
				GipipeMaster objGipipe = GetGipipeDetail(objIn);
				if (objIn.system_id == 0)
				{
					//Fill Location detail...    
					GetLineNetworkDetail(objGipipe, objIn, EntityType.Gipipe.ToString(), false);
				}
				BLItemTemplate.Instance.BindItemDropdowns(objGipipe, EntityType.Gipipe.ToString());
				fillProjectSpecifications(objGipipe);
				BindGipipeDropDown(objGipipe);
				objGipipe.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Gipipe.ToString()).ToList();
				//Get the layer details to bind additional attributes Duct
				var layerdetails = new BLLayer().getLayer(EntityType.Gipipe.ToString());
				objGipipe.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Duct
				response.status = StatusCodes.OK.ToString();
				response.results = objGipipe;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddGipipe()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#region Get DUCT Details
		/// <summary> GetDuctDetail</summary>
		/// <param >objIn</param>
		/// <returns>DUCT Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public DuctMaster GetDuctDetail(LineEntityIn objIn)
		{
			DuctMaster objDuct = new DuctMaster();
			int id = objIn.user_id;
			if (objIn.system_id == 0)
			{
				objDuct.geom = objIn.geom;
				if (!string.IsNullOrEmpty(objIn.geom))
					objDuct.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

				objDuct.manual_length = objDuct.calculated_length;
				objDuct.networkIdType = objIn.networkIdType;
				objDuct.ownership_type = "Own";
				objDuct.isDirectSave = objIn.isDirectSave;
				objDuct.user_id = objIn.user_id;
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objDuct, GeometryType.Line.ToString(), objIn.geom);
				var a = new BLMisc().GetEndPoints(objIn.geom);
				objDuct.a_latitude = a[0].a_latitude.Replace("POINT(", "").Replace(")", "");
				objDuct.b_longitude = a[0].b_longitude.Replace("POINT(", "").Replace(")", "");
				objDuct.a_region = a[0].tstart_region;
				objDuct.b_region = a[0].tEnd_region;
				objDuct.a_city = a[0].tstrat_province;
				objDuct.b_city = a[0].tEnd_province;
				if (ApplicationSettings.IsEntityNamePrefixAllow)
				{
					objDuct.duct_name = "R/OWN/" + a[0].province_abbr;
				}
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<DuctTemplateMaster>(objDuct.user_id, EntityType.Duct);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objDuct);
				objDuct.other_info = null;  //for additional-attributes
			}
			else
			{
				objDuct = new BLMisc().GetEntityDetailById<DuctMaster>(objIn.system_id, EntityType.Duct, objIn.user_id);
				//for additional-attributes
				objDuct.other_info = new BLDuct().GetOtherInfoDuct(objDuct.system_id);
				fillRegionProvAbbr(objDuct);
			}
			objDuct.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());

			return objDuct;
		}
        #endregion
        #region Get Microduct Details
        /// <summary> GetMicroductDetail</summary>
        /// <param >objIn</param>
        /// <returns>Microduct Details</returns>
        /// <CreatedBy>Arabind</CreatedBy>
        public MicroductMaster GetMicroductDetail(LineEntityIn objIn)
        {
            MicroductMaster objMicroduct = new MicroductMaster();
            int id = objIn.user_id;
            if (objIn.system_id == 0)
            {
                objMicroduct.geom = objIn.geom;
                if (!string.IsNullOrEmpty(objIn.geom))
                    objMicroduct.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

                objMicroduct.manual_length = objMicroduct.calculated_length;
                objMicroduct.networkIdType = objIn.networkIdType;
                objMicroduct.ownership_type = "Own";
                objMicroduct.isDirectSave = objIn.isDirectSave;
                objMicroduct.user_id = objIn.user_id;
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objMicroduct, GeometryType.Line.ToString(), objIn.geom);
                var a = new BLMisc().GetEndPoints(objIn.geom);
                objMicroduct.a_latitude = a[0].a_latitude.Replace("POINT(", "").Replace(")", "");
                objMicroduct.b_longitude = a[0].b_longitude.Replace("POINT(", "").Replace(")", "");
                objMicroduct.a_region = a[0].tstart_region;
                objMicroduct.b_region = a[0].tEnd_region;
                objMicroduct.a_city = a[0].tstrat_province;
                objMicroduct.b_city = a[0].tEnd_province;
                if (ApplicationSettings.IsEntityNamePrefixAllow)
                {
                    objMicroduct.microduct_name = "R/OWN/" + a[0].province_abbr;
                }
                // Item template binding
                var objItem = BLItemTemplate.Instance.GetTemplateDetail<MicroductTemplateMaster>(objMicroduct.user_id, EntityType.Microduct);
                Utility.MiscHelper.CopyMatchingProperties(objItem, objMicroduct);
                objMicroduct.other_info = null;  //for additional-attributes
            }
            else
            {
                objMicroduct = new BLMisc().GetEntityDetailById<MicroductMaster>(objIn.system_id, EntityType.Microduct, objIn.user_id);
                //for additional-attributes
                objMicroduct.other_info = new BLMicroduct().GetOtherInfoMicroduct(objMicroduct.system_id);
                fillRegionProvAbbr(objMicroduct);
            }
            objMicroduct.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());

            return objMicroduct;
        }
        #endregion
        #region Get GIPIPE Details
        public GipipeMaster GetGipipeDetail(LineEntityIn objIn)
		{
			GipipeMaster objGipipe = new GipipeMaster();
			int id = objIn.user_id;
			if (objIn.system_id == 0)
			{
				objGipipe.geom = objIn.geom;
				if (!string.IsNullOrEmpty(objIn.geom))
					objGipipe.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

				objGipipe.manual_length = objGipipe.calculated_length;
				objGipipe.networkIdType = objIn.networkIdType;
				objGipipe.ownership_type = "Own";
				objGipipe.isDirectSave = objIn.isDirectSave;
				objGipipe.user_id = objIn.user_id;
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objGipipe, GeometryType.Line.ToString(), objIn.geom);
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<GipipeTemplateMaster>(objGipipe.user_id, EntityType.Gipipe);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objGipipe);
				objGipipe.other_info = null;  //for additional-attributes
			}
			else
			{
				objGipipe = new BLMisc().GetEntityDetailById<GipipeMaster>(objIn.system_id, EntityType.Gipipe, objIn.user_id);
				//for additional-attributes
				objGipipe.other_info = new BLGipipe().GetOtherInfoGipipe(objGipipe.system_id);
				fillRegionProvAbbr(objGipipe);
			}
			objGipipe.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());

			return objGipipe;
		}


        #endregion

        #region Save DUCT
        /// <summary> SaveCdb </summary>
        /// <param name="data">ReqInput</param>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        public ApiResponse<DuctMaster> SaveDuct(ReqInput data)
        {
            var response = new ApiResponse<DuctMaster>();
            var ductOffsetVal = Convert.ToDecimal(0.00001) * Convert.ToDecimal(ApplicationSettings.DuctOffset);
            DuctMaster objnewDuct = ReqHelper.GetRequestData<DuctMaster>(data);
            // get duct count for already added duct in trench loaction
            int CheckductCount = BLDuct.Instance.getDuctCount(objnewDuct.pSystemId);
            try
            {
                var OffsetDir = "leftOffset";
                decimal leftdistance = ductOffsetVal;
                decimal rightdistance = ductOffsetVal;
                string finalDistance;
                int ductCount = objnewDuct.system_id > 0 ? ductCount = 1 : objnewDuct.duct_count == 0 ? 1 : objnewDuct.duct_count;
                // set left or right distance that already existed duct for same the trench loaction
                if (CheckductCount > 0)
                {
                    if (CheckductCount % 2 == 0)
                    {
                        rightdistance = ductOffsetVal * ((CheckductCount / 2));
                        leftdistance = ductOffsetVal + rightdistance;
                        OffsetDir = "rightOffset";
                    }
                    else
                    {
                        leftdistance = ductOffsetVal * ((CheckductCount / 2) + 1);
                        rightdistance = leftdistance;
                        OffsetDir = "leftOffset";
                    }
                }
                for (int i = 1; i <= ductCount; i++)
                {
                    // set final distance for left or right offset
                    finalDistance = OffsetDir == "leftOffset" ? "-" + leftdistance.ToString() : rightdistance.ToString();
                    DuctMaster objDuct = ReqHelper.GetRequestData<DuctMaster>(data);
                    if (objDuct.networkIdType == NetworkIdType.A.ToString() && objDuct.system_id == 0)
                    {
                        if (objDuct.isDirectSave == false)
                        {
                            objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.a_system_id, network_id = objDuct.a_location, network_name = objDuct.a_entity_type });
                            objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.b_system_id, network_id = objDuct.b_location, network_name = objDuct.b_entity_type });
                        }
                        var objLineEntity = new LineEntityIn() { geom = objDuct.geom, systemId = objDuct.system_id, networkIdType = objDuct.networkIdType, lstTP = objDuct.lstTP, user_id = objDuct.user_id, isDirectSave = objDuct.isDirectSave };
                        if (objDuct.isDirectSave == true)
                        {
                            //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                            int trenchId = objDuct.pSystemId;
                            string pentityType = objDuct.pEntityType;
                            objDuct = GetDuctDetail(objLineEntity);
                            objDuct.pSystemId = trenchId;//it's due to pSystemId lost when call above function i.e. GetDuctDetail.
                            objDuct.trench_id = objDuct.pSystemId;
                            objDuct.pEntityType = pentityType;
                            var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                            // var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
                            objDuct.bom_sub_category = objBOMDDL[0].dropdown_value;
                            //objDuct.served_by_ring = objSubCatDDL[0].dropdown_value;
                        }
                        GetLineNetworkDetail(objDuct, objLineEntity, EntityType.Duct.ToString(), true);
                        if (objDuct.isDirectSave == true)
                            objDuct.duct_name = objDuct.network_id;
                    }
                    if (string.IsNullOrEmpty(objDuct.duct_name))
                    {
                        objDuct.duct_name = objDuct.network_id;
                    }
                    this.Validate(objDuct);
                    if (ModelState.IsValid)
                    {

                        var isNew = objDuct.system_id > 0 ? false : true;
                        objDuct.is_new_entity = (isNew && objDuct.source_ref_id != "0" && objDuct.source_ref_id != "");
                        objDuct.trench_id = objDuct.pSystemId;
                        var resultItem = BLDuct.Instance.SaveDuct(objDuct, objDuct.user_id);
                        // update duct geometry                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
                        if (i != ductCount || CheckductCount > 0)
                        {
                            if (objnewDuct.system_id == 0)
                            {
                                BASaveEntityGeometry.Instance.UpdateDuctLocation(resultItem.system_id, finalDistance, OffsetDir);
                            }
                        }
                        // update duct color code 
                        if (objnewDuct.system_id == 0 && string.IsNullOrEmpty(objnewDuct.duct_color))
                        {
                            BASaveEntityGeometry.Instance.UpdateDuctColorCode(resultItem.system_id, objDuct.trench_id,i);
                        }
                        if (string.IsNullOrEmpty(resultItem.objPM.message))
                        {
                            string[] LayerName = { EntityType.Duct.ToString() };

                            //Save Reference
                            if (objDuct.EntityReference != null && resultItem.system_id > 0)
                            {
                                SaveReference(objDuct.EntityReference, resultItem.system_id);
                            }
                            if (isNew)
                            {
                                objDuct.objPM.status = ResponseStatus.OK.ToString();
                                objDuct.objPM.isNewEntity = isNew;
                                objDuct.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                                response.status = ResponseStatus.OK.ToString();
                                response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                            }
                            else
                            {
                                objDuct.objPM.status = ResponseStatus.OK.ToString();
                                objDuct.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                                BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
                                response.status = ResponseStatus.OK.ToString();
                                response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                            }
                        }
                        else
                        {
                            objDuct.objPM.status = ResponseStatus.FAILED.ToString();
                            objDuct.objPM.message = resultItem.objPM.message;
                            response.error_message = resultItem.objPM.message;
                            response.status = ResponseStatus.FAILED.ToString();
                        }

                        //save AT Status                        
                        if (objDuct.ATAcceptance != null && objDuct.system_id > 0)
                        {
                            SaveATAcceptance(objDuct.ATAcceptance, objDuct.system_id, objDuct.user_id);
                        }

                    }
                    else
                    {
                        objDuct.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        objDuct.objPM.message = getFirstErrorFromModelState();
                        response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        response.error_message = getFirstErrorFromModelState();
                    }
                    if (objDuct.isDirectSave == true)
                    {
                        //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                        objDuct.objPM.status = ResponseStatus.OK.ToString();
                        response.status = ResponseStatus.OK.ToString();
                        response.results = objDuct;
                    }
                    else
                    {
                        BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Duct.ToString());
                        // RETURN PARTIAL VIEW WITH MODEL DATA
                        fillProjectSpecifications(objDuct);
                        BindDuctDropDown(objDuct);
                        objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
                        //Get the layer details to bind additional attributes Duct
                        var layerdetails = new BLLayer().getLayer(EntityType.Duct.ToString());
                        objDuct.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                        //End for additional attributes Duct
                        response.results = objDuct;
                    }
                    if (OffsetDir == "leftOffset")
                    {
                        leftdistance = leftdistance + ductOffsetVal;
                        OffsetDir = "rightOffset";
                    }
                    else
                    {
                        rightdistance = rightdistance + ductOffsetVal;
                        OffsetDir = "leftOffset";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuct()", "Library Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        #region Save Microduct
        /// <summary> Microduct </summary>
        /// <param name="data">ReqInput</param>
        /// <CreatedBy>Arabind</CreatedBy>
        public ApiResponse<MicroductMaster> SaveMicroduct(ReqInput data)
        {
            var response = new ApiResponse<MicroductMaster>();
            var ductOffsetVal = Convert.ToDecimal(0.00001) * Convert.ToDecimal(ApplicationSettings.DuctOffset);
            MicroductMaster objnewDuct = ReqHelper.GetRequestData<MicroductMaster>(data);
            // get duct count for already added duct in trench loaction
            int CheckductCount = BLMicroduct.Instance.getMicroductCount(objnewDuct.pSystemId);
            try
            {
                var OffsetDir = "leftOffset";
                decimal leftdistance = ductOffsetVal;
                decimal rightdistance = ductOffsetVal;
                string finalDistance;
                int ductCount = objnewDuct.system_id > 0 ? ductCount = 1 : objnewDuct.microduct_count == 0 ? 1 : objnewDuct.microduct_count;
                // set left or right distance that already existed duct for same the trench loaction
                if (CheckductCount > 0)
                {
                    if (CheckductCount % 2 == 0)
                    {
                        rightdistance = ductOffsetVal * ((CheckductCount / 2));
                        leftdistance = ductOffsetVal + rightdistance;
                        OffsetDir = "rightOffset";
                    }
                    else
                    {
                        leftdistance = ductOffsetVal * ((CheckductCount / 2) + 1);
                        rightdistance = leftdistance;
                        OffsetDir = "leftOffset";
                    }
                }
                for (int i = 1; i <= ductCount; i++)
                {
                    // set final distance for left or right offset
                    finalDistance = OffsetDir == "leftOffset" ? "-" + leftdistance.ToString() : rightdistance.ToString();
                    MicroductMaster objDuct = ReqHelper.GetRequestData<MicroductMaster>(data);
                    if (objDuct.networkIdType == NetworkIdType.A.ToString() && objDuct.system_id == 0)
                    {
                        if (objDuct.isDirectSave == false)
                        {
                            objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.a_system_id, network_id = objDuct.a_location, network_name = objDuct.a_entity_type });
                            objDuct.lstTP.Add(new NetworkDtl { system_id = objDuct.b_system_id, network_id = objDuct.b_location, network_name = objDuct.b_entity_type });
                        }
                        var objLineEntity = new LineEntityIn() { geom = objDuct.geom, systemId = objDuct.system_id, networkIdType = objDuct.networkIdType, lstTP = objDuct.lstTP, user_id = objDuct.user_id, isDirectSave = objDuct.isDirectSave };
                        if (objDuct.isDirectSave == true)
                        {
                            //GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
                            int trenchId = objDuct.pSystemId;
                            string pentityType = objDuct.pEntityType;
                            objDuct = GetMicroductDetail(objLineEntity);
                            objDuct.pSystemId = trenchId;//it's due to pSystemId lost when call above function i.e. GetDuctDetail.
                            objDuct.trench_id = objDuct.pSystemId;
                            objDuct.pEntityType = pentityType;
                            var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
                            // var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
                            objDuct.bom_sub_category = objBOMDDL[0].dropdown_value;
                            //objDuct.served_by_ring = objSubCatDDL[0].dropdown_value;
                        }
                        GetLineNetworkDetail(objDuct, objLineEntity, EntityType.Microduct.ToString(), true);
                        if (objDuct.isDirectSave == true)
                            objDuct.microduct_name = objDuct.network_id;
                    }
                    if (string.IsNullOrEmpty(objDuct.microduct_name))
                    {
                        objDuct.microduct_name = objDuct.network_id;
                    }
                    this.Validate(objDuct);
                    if (ModelState.IsValid)
                    {

                        var isNew = objDuct.system_id > 0 ? false : true;
                        objDuct.is_new_entity = (isNew && objDuct.source_ref_id != "0" && objDuct.source_ref_id != "");
                        objDuct.trench_id = objDuct.pSystemId;
                        var resultItem = BLMicroduct.Instance.Save(objDuct, objDuct.user_id);
                        // update duct geometry                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
                        if (i != ductCount || CheckductCount > 0)
                        {
                            if (objnewDuct.system_id == 0)
                            {
                                BASaveEntityGeometry.Instance.UpdateMicroductLocation(resultItem.system_id, finalDistance, OffsetDir);
                            }
                        }
                        // update duct color code 
                        if (objnewDuct.system_id == 0 && string.IsNullOrEmpty(objnewDuct.microduct_color))
                        {
                            BASaveEntityGeometry.Instance.UpdateMicroductColorCode(resultItem.system_id, objDuct.trench_id, i);
                        }
                        if (string.IsNullOrEmpty(resultItem.objPM.message))
                        {
                            string[] LayerName = { EntityType.Microduct.ToString() };

                            //Save Reference
                            if (objDuct.EntityReference != null && resultItem.system_id > 0)
                            {
                                SaveReference(objDuct.EntityReference, resultItem.system_id);
                            }
                            if (isNew)
                            {
                                objDuct.objPM.status = ResponseStatus.OK.ToString();
                                objDuct.objPM.isNewEntity = isNew;
                                objDuct.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                                response.status = ResponseStatus.OK.ToString();
                                response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                            }
                            else
                            {
                                objDuct.objPM.status = ResponseStatus.OK.ToString();
                                objDuct.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                                BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Microduct.ToString());
                                response.status = ResponseStatus.OK.ToString();
                                response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                            }
                        }
                        else
                        {
                            objDuct.objPM.status = ResponseStatus.FAILED.ToString();
                            objDuct.objPM.message = resultItem.objPM.message;
                            response.error_message = resultItem.objPM.message;
                            response.status = ResponseStatus.FAILED.ToString();
                        }

                        //save AT Status                        
                        if (objDuct.ATAcceptance != null && objDuct.system_id > 0)
                        {
                            SaveATAcceptance(objDuct.ATAcceptance, objDuct.system_id, objDuct.user_id);
                        }

                    }
                    else
                    {
                        objDuct.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        objDuct.objPM.message = getFirstErrorFromModelState();
                        response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        response.error_message = getFirstErrorFromModelState();
                    }
                    if (objDuct.isDirectSave == true)
                    {
                        //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                        objDuct.objPM.status = ResponseStatus.OK.ToString();
                        response.status = ResponseStatus.OK.ToString();
                        response.results = objDuct;
                    }
                    else
                    {
                        BLItemTemplate.Instance.BindItemDropdowns(objDuct, EntityType.Microduct.ToString());
                        // RETURN PARTIAL VIEW WITH MODEL DATA
                        fillProjectSpecifications(objDuct);
                        BindMicroductDropDown(objDuct);
                        objDuct.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Microduct.ToString()).ToList();
                        //Get the layer details to bind additional attributes Duct
                        var layerdetails = new BLLayer().getLayer(EntityType.Microduct.ToString());
                        objDuct.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                        //End for additional attributes Duct
                        response.results = objDuct;
                    }
                    if (OffsetDir == "leftOffset")
                    {
                        leftdistance = leftdistance + ductOffsetVal;
                        OffsetDir = "rightOffset";
                    }
                    else
                    {
                        rightdistance = rightdistance + ductOffsetVal;
                        OffsetDir = "leftOffset";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuct()", "Library Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion
        public ApiResponse<GipipeMaster> SaveGipipe(ReqInput data)
		{
			var response = new ApiResponse<GipipeMaster>();
			GipipeMaster objGipipe = ReqHelper.GetRequestData<GipipeMaster>(data);
			try
			{
				if (objGipipe.networkIdType == NetworkIdType.A.ToString() && objGipipe.system_id == 0)
				{
					if (objGipipe.isDirectSave == false)
					{
						objGipipe.lstTP.Add(new NetworkDtl { system_id = objGipipe.a_system_id, network_id = objGipipe.a_location, network_name = objGipipe.a_entity_type });
						objGipipe.lstTP.Add(new NetworkDtl { system_id = objGipipe.b_system_id, network_id = objGipipe.b_location, network_name = objGipipe.b_entity_type });
					}
					var objLineEntity = new LineEntityIn() { geom = objGipipe.geom, systemId = objGipipe.system_id, networkIdType = objGipipe.networkIdType, lstTP = objGipipe.lstTP, user_id = objGipipe.user_id, isDirectSave = objGipipe.isDirectSave };
					if (objGipipe.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						int trenchId = objGipipe.pSystemId;
						string pentityType = objGipipe.pEntityType;
						objGipipe = GetGipipeDetail(objLineEntity);
						objGipipe.pSystemId = trenchId;//it's due to pSystemId lost when call above function i.e. GetDuctDetail.
						objGipipe.trench_id = objGipipe.pSystemId;
						objGipipe.pEntityType = pentityType;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objGipipe.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objDuct.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					GetLineNetworkDetail(objGipipe, objLineEntity, EntityType.Gipipe.ToString(), true);
					if (objGipipe.isDirectSave == true)
						objGipipe.gipipe_name = objGipipe.network_id;
				}
				if (string.IsNullOrEmpty(objGipipe.gipipe_name))
				{
					objGipipe.gipipe_name = objGipipe.network_id;
				}
				this.Validate(objGipipe);
				if (ModelState.IsValid)
				{

					var isNew = objGipipe.system_id > 0 ? false : true;

					objGipipe.trench_id = objGipipe.pSystemId;
					var resultItem = BLGipipe.Instance.SaveGipipe(objGipipe, objGipipe.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.Gipipe.ToString() };

						//Save Reference
						if (objGipipe.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objGipipe.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objGipipe.objPM.status = ResponseStatus.OK.ToString();
							objGipipe.objPM.isNewEntity = isNew;
							objGipipe.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							objGipipe.objPM.status = ResponseStatus.OK.ToString();
							objGipipe.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							BLItemTemplate.Instance.BindItemDropdowns(objGipipe, EntityType.Gipipe.ToString());
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objGipipe.objPM.status = ResponseStatus.FAILED.ToString();
						objGipipe.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}

					//save AT Status                        
					if (objGipipe.ATAcceptance != null && objGipipe.system_id > 0)
					{
						SaveATAcceptance(objGipipe.ATAcceptance, objGipipe.system_id, objGipipe.user_id);
					}

				}
				else
				{
					objGipipe.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objGipipe.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objGipipe.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					objGipipe.objPM.status = ResponseStatus.OK.ToString();
					response.status = ResponseStatus.OK.ToString();
					response.results = objGipipe;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objGipipe, EntityType.Gipipe.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objGipipe);
					BindGipipeDropDown(objGipipe);
					objGipipe.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Gipipe.ToString()).ToList();
					//Get the layer details to bind additional attributes Duct
					var layerdetails = new BLLayer().getLayer(EntityType.Gipipe.ToString());
					objGipipe.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Duct
					response.results = objGipipe;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveGipipe()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#region Bind DUCT Dropdown
		/// <summary>BindDuctDropDown </summary>
		/// <param name="objDuctIn"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindDuctDropDown(DuctMaster objDuctIn)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Duct.ToString());
			objDuctIn.NoofDuctsCreated = objDDL.Where(x => x.dropdown_type == DropDownType.No_of_Ducts_Created.ToString()).ToList();
			objDuctIn.DuctTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Type.ToString()).ToList();
			objDuctIn.DuctCount = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Count.ToString()).ToList();
			objDuctIn.DuctColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Color.ToString()).ToList();
			objDuctIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objDuctIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objDuctIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objDuctIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
			objDuctIn.listALocation = _objDDL.Where(x => x.dropdown_type == DropDownType.A_location.ToString()).ToList();
			objDuctIn.listBLocation = _objDDL.Where(x => x.dropdown_type == DropDownType.A_location.ToString()).ToList();


		}
        private void BindMicroductDropDown(MicroductMaster objMicroductIn)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Microduct.ToString());
            objMicroductIn.NoofMicroductsCreated = objDDL.Where(x => x.dropdown_type == DropDownType.No_of_Ducts_Created.ToString()).ToList();
            objMicroductIn.MicroductTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Type.ToString()).ToList();
            objMicroductIn.MicroductCount = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Count.ToString()).ToList();
            objMicroductIn.MicroductColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Duct_Color.ToString()).ToList();
            objMicroductIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
            objMicroductIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
            objMicroductIn.lstNoOfWays = objDDL.Where(x => x.dropdown_type == DropDownType.Number_of_Ways.ToString()).ToList();
            objMicroductIn.lstInternalDiameter = objDDL.Where(x => x.dropdown_type == DropDownType.Internal_Diameter.ToString()).ToList();
            objMicroductIn.lstExternalDiameter = objDDL.Where(x => x.dropdown_type == DropDownType.External_Diameter.ToString()).ToList();
            objMicroductIn.lstMaterialType = objDDL.Where(x => x.dropdown_type == DropDownType.Material_Type.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
            objMicroductIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
            objMicroductIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
            

        }

        #endregion

        #endregion
        private void BindGipipeDropDown(GipipeMaster objGipipeIn)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Gipipe.ToString());
			objGipipeIn.GipipeTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Gipipe_Type.ToString()).ToList();
			objGipipeIn.GipipeColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Gipipe_Color.ToString()).ToList();
			objGipipeIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objGipipeIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objGipipeIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objDuctIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}

		#region CONDUIT
		#region Add CONDUIT
		/// <summary> Add CONDUIT </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>CONDUIT Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<ConduitMaster> AddConduit(ReqInput data)
		{
			var response = new ApiResponse<ConduitMaster>();
			try
			{
				LineEntityIn objIn = ReqHelper.GetRequestData<LineEntityIn>(data);
				ConduitMaster objConduit = GetConduitDetail(objIn);
				if (objIn.system_id == 0)
				{
					//Fill Location detail...    
					GetLineNetworkDetail(objConduit, objIn, EntityType.Conduit.ToString(), false);
				}
				BLItemTemplate.Instance.BindItemDropdowns(objConduit, EntityType.Conduit.ToString());
				fillProjectSpecifications(objConduit);
				BindConduitDropDown(objConduit);
				objConduit.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Conduit.ToString()).ToList();
				//Get the layer details to bind additional attributes Conduit
				var layerdetails = new BLLayer().getLayer(EntityType.Conduit.ToString());
				objConduit.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Conduit
				response.status = StatusCodes.OK.ToString();
				response.results = objConduit;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddConduit()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get Conduit Details
		/// <summary> GetConduitDetail</summary>
		/// <param >objIn</param>
		/// <returns>Conduit Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ConduitMaster GetConduitDetail(LineEntityIn objIn)
		{
			ConduitMaster objConduit = new ConduitMaster();
			if (objIn.system_id == 0)
			{
				objConduit.geom = objIn.geom;
				if (!string.IsNullOrEmpty(objIn.geom))
					objConduit.calculated_length = Math.Round((double)new BLMisc().GetCableLength(objIn.geom), 3);

				objConduit.manual_length = objConduit.calculated_length;
				objConduit.networkIdType = objIn.networkIdType;
				objConduit.ownership_type = "Own";
				objConduit.isDirectSave = objIn.isDirectSave;
				objConduit.user_id = objIn.user_id;
				objConduit.pEntityType = objIn.pEntityType;
				objConduit.pSystemId = objIn.pSystemId;
				objConduit.pNetworkId = objIn.pNetworkId;
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objConduit, GeometryType.Line.ToString(), objIn.geom);
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<ConduitTemplateMaster>(objConduit.user_id, EntityType.Conduit);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objConduit);
				objConduit.other_info = null;   //for additional-attributes
			}
			else
			{
				objConduit = new BLMisc().GetEntityDetailById<ConduitMaster>(objIn.system_id, EntityType.Conduit, objConduit.user_id);
				//for additional-attributes
				objConduit.other_info = BLConduit.Instance.GetOtherInfoConduit(objConduit.system_id);
				fillRegionProvAbbr(objConduit);
			}
			objConduit.lstUserModule = new BLLayer().GetUserModuleAbbrList(objIn.user_id, UserType.Web.ToString());
			return objConduit;
		}

		#endregion

		#region Save DUCT
		/// <summary> SaveCdb </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<ConduitMaster> SaveConduit(ReqInput data)
		{
			var response = new ApiResponse<ConduitMaster>();
			ConduitMaster objConduit = ReqHelper.GetRequestData<ConduitMaster>(data);
			try
			{
				if (objConduit.networkIdType == NetworkIdType.A.ToString() && objConduit.system_id == 0)
				{
					if (objConduit.isDirectSave == false)
					{
						objConduit.lstTP.Add(new NetworkDtl { system_id = objConduit.a_system_id, network_id = objConduit.a_location, network_name = objConduit.a_entity_type });
						objConduit.lstTP.Add(new NetworkDtl { system_id = objConduit.b_system_id, network_id = objConduit.b_location, network_name = objConduit.b_entity_type });
					}
					var objLineEntity = new LineEntityIn() { geom = objConduit.geom, systemId = objConduit.system_id, networkIdType = objConduit.networkIdType, lstTP = objConduit.lstTP, user_id = objConduit.user_id, isDirectSave = objConduit.isDirectSave, pEntityType = objConduit.pEntityType, pSystemId = objConduit.pSystemId, pNetworkId = objConduit.pNetworkId };
					if (objConduit.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objConduit = GetConduitDetail(objLineEntity);
						objConduit.trench_id = objConduit.pSystemId;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objConduit.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objConduit.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					GetLineNetworkDetail(objConduit, objLineEntity, EntityType.Conduit.ToString(), true);
					if (objConduit.isDirectSave == true)
						objConduit.network_name = objConduit.network_id;
				}
				if (string.IsNullOrEmpty(objConduit.network_name))
				{
					objConduit.network_name = objConduit.network_id;
				}
				this.Validate(objConduit);
				if (ModelState.IsValid)
				{

					var isNew = objConduit.system_id > 0 ? false : true;
					objConduit.trench_id = objConduit.pSystemId;
					var resultItem = BLConduit.Instance.SaveConduit(objConduit, objConduit.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.Conduit.ToString() };

						//Save Reference
						if (objConduit.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objConduit.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objConduit.objPM.status = ResponseStatus.OK.ToString();
							objConduit.objPM.isNewEntity = isNew;
							objConduit.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							objConduit.objPM.status = ResponseStatus.OK.ToString();
							objConduit.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							BLItemTemplate.Instance.BindItemDropdowns(objConduit, EntityType.Conduit.ToString());
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objConduit.objPM.status = ResponseStatus.FAILED.ToString();
						objConduit.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}

					//save AT Status                        
					if (objConduit.ATAcceptance != null && objConduit.system_id > 0)
					{
						SaveATAcceptance(objConduit.ATAcceptance, objConduit.system_id, objConduit.user_id);
					}

				}
				else
				{
					objConduit.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objConduit.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objConduit.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					objConduit.objPM.status = ResponseStatus.OK.ToString();
					response.status = ResponseStatus.OK.ToString();
					response.results = objConduit;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objConduit, EntityType.Conduit.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objConduit);
					BindConduitDropDown(objConduit);
					objConduit.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Conduit.ToString()).ToList();
					//Get the layer details to bind additional attributes Conduit
					var layerdetails = new BLLayer().getLayer(EntityType.Conduit.ToString());
					objConduit.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Conduit
					response.results = objConduit;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveConduit()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind Conduit Dropdown
		/// <summary>BindConduitDropDown </summary>
		/// <param name="objConduitIn"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindConduitDropDown(ConduitMaster objConduitIn)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Conduit.ToString());
			objConduitIn.ConduitTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.conduit_Type.ToString()).ToList();
			objConduitIn.ConduitColorIn = objDDL.Where(x => x.dropdown_type == DropDownType.Conduit_Color.ToString()).ToList();
			objConduitIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objConduitIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objConduitIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//objConduitIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#endregion

		#region TRENCH
		#region Add Trench
		/// <summary> Add Trench </summary>
		/// <param name="data">LineEntityIn</param>
		/// <returns>Trench Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<TrenchMaster> AddTrench(ReqInput data)
		{
			var response = new ApiResponse<TrenchMaster>();
			try
			{
				LineEntityIn objIn = ReqHelper.GetRequestData<LineEntityIn>(data);
				TrenchMaster objTrench = GetTrenchDetail(objIn);
				if (objIn.system_id == 0)
				{
					//Fill Location detail...    
					GetLineNetworkDetail(objTrench, objIn, EntityType.Trench.ToString(), false);
				}

				BLItemTemplate.Instance.BindItemDropdowns(objTrench, EntityType.Trench.ToString());
				BindTrenchDropDown(objTrench);
				fillProjectSpecifications(objTrench);
				objTrench.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Trench.ToString()).ToList();
				//Get the layer details to bind additional attributes Trench
				var layerdetails = new BLLayer().getLayer(EntityType.Trench.ToString());
				objTrench.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Trench
				response.status = StatusCodes.OK.ToString();
				response.results = objTrench;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddTrench()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get Trench Details
		/// <summary> GetDuctDetail</summary>
		/// <param >objIn</param>
		/// <returns>Trench Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public TrenchMaster GetTrenchDetail(LineEntityIn objIn)
		{
			TrenchMaster objTrench = new TrenchMaster();
			if (objIn.system_id == 0)
			{
				objTrench.geom = objIn.geom;
				if (!string.IsNullOrEmpty(objIn.geom))
					objTrench.trench_length = (double)new BLMisc().GetCableLength(objIn.geom);

				objTrench.networkIdType = objIn.networkIdType;
				objTrench.ownership_type = "Own";
				objTrench.isDirectSave = objIn.isDirectSave;
				objTrench.user_id = objIn.user_id;
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objTrench, GeometryType.Line.ToString(), objIn.geom);
				var a = new BLMisc().GetEndPoints(objIn.geom);
				objTrench.a_latitude = a[0].a_latitude.Replace("POINT(", "").Replace(")", "");
				objTrench.b_longitude = a[0].b_longitude.Replace("POINT(", "").Replace(")", "");
				objTrench.a_region = a[0].tstart_region;
				objTrench.b_region = a[0].tEnd_region;
				objTrench.a_city = a[0].tstrat_province;
				objTrench.b_city = a[0].tEnd_province;
				if (ApplicationSettings.IsEntityNamePrefixAllow)
				{
					objTrench.trench_name = "R/OWN/" + a[0].province_abbr;
				}
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<TrenchTemplateMaster>(objTrench.user_id, EntityType.Trench);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objTrench);
				objTrench.other_info = null;    //for additional-attributes
			}
			else
			{
				objTrench = new BLMisc().GetEntityDetailById<TrenchMaster>(objIn.system_id, EntityType.Trench, objIn.user_id);
				//for additional-attributes
				objTrench.other_info = new BLTrench().GetOtherInfoTrench(objTrench.system_id);
				fillRegionProvAbbr(objTrench);
			}
			objTrench.lstUserModule = new BLLayer().GetUserModuleAbbrList(objIn.user_id, UserType.Web.ToString());
			return objTrench;
		}

		#endregion

		#region Save Trench
		/// <summary> SaveTrench </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<TrenchMaster> SaveTrench(ReqInput data)
		{
			var response = new ApiResponse<TrenchMaster>();
			TrenchMaster objTrench = ReqHelper.GetRequestData<TrenchMaster>(data);
			try
			{
				if (objTrench.networkIdType == NetworkIdType.A.ToString() && objTrench.system_id == 0)
				{
					if (objTrench.isDirectSave == false)
					{
						objTrench.lstTP.Add(new NetworkDtl { system_id = objTrench.a_system_id, network_id = objTrench.a_location, network_name = objTrench.a_entity_type });
						objTrench.lstTP.Add(new NetworkDtl { system_id = objTrench.b_system_id, network_id = objTrench.b_location, network_name = objTrench.b_entity_type });
					}
					var objLineEntity = new LineEntityIn() { geom = objTrench.geom, systemId = objTrench.system_id, networkIdType = objTrench.networkIdType, lstTP = objTrench.lstTP, user_id = objTrench.user_id, isDirectSave = objTrench.isDirectSave };
					if (objTrench.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objTrench = GetTrenchDetail(objLineEntity);
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						//var objServngTypDDL = new BLMisc().GetDropDownList(EntityType.Trench.ToString(), "trench_serving_type");

						objTrench.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objTrench.served_by_ring = objSubCatDDL[0].dropdown_value;
						//objTrench.trench_serving_type = objServngTypDDL[0].dropdown_value;
					}
					GetLineNetworkDetail(objTrench, objLineEntity, EntityType.Trench.ToString(), true);
					if (objTrench.isDirectSave == true)
						objTrench.trench_name = objTrench.network_id;
				}
				if (string.IsNullOrEmpty(objTrench.trench_name))
				{
					objTrench.trench_name = objTrench.network_id;
				}
				this.Validate(objTrench);
				if (ModelState.IsValid)
				{
					var isNew = objTrench.system_id > 0 ? false : true;
					objTrench.is_new_entity = (isNew && objTrench.source_ref_id != "0" && objTrench.source_ref_id != "");
					var resultItem = BLTrench.Instance.SaveTrench(objTrench, objTrench.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.Trench.ToString() };
						//Save Reference
						if (objTrench.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objTrench.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objTrench.objPM.status = ResponseStatus.OK.ToString();
							objTrench.objPM.isNewEntity = isNew;
							objTrench.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							objTrench.objPM.status = ResponseStatus.OK.ToString();
							objTrench.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objTrench.objPM.status = ResponseStatus.FAILED.ToString();
						objTrench.objPM.message = objTrench.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = objTrench.objPM.message;
					}
					//save AT Status                        
					if (objTrench.ATAcceptance != null && objTrench.system_id > 0)
					{
						SaveATAcceptance(objTrench.ATAcceptance, objTrench.system_id, objTrench.user_id);
					}
                    if (objTrench.ExecutionMethod != null && objTrench.system_id > 0)
                    {
						objTrench.ExecutionMethod.listExecutionRecords.ForEach(record =>
						{
							record.system_id = objTrench.system_id;
							record.entity_type = EntityType.Trench.ToString();
						});



						trenchExecutionList objExMethodStatus = new trenchExecutionList();
						objExMethodStatus.listExecutionRecords = BLExecution.Instance.GetExecutionStatus(objTrench.system_id, EntityType.Trench.ToString());//FillATAcceptance(entityId, entityType);
						if(objExMethodStatus.listExecutionRecords.Count> 0)
                        {
							var output = BLExecution.Instance.DeleteExecutionStatus(objTrench.system_id, EntityType.Trench.ToString());
						}                      

						SaveExecutionmethod(objTrench.ExecutionMethod, objTrench.system_id, objTrench.user_id);
                    }
                }
				else
				{
					objTrench.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objTrench.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();

				}
				if (objTrench.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objTrench;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objTrench, EntityType.Trench.ToString());
					BindTrenchDropDown(objTrench);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objTrench);
					objTrench.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Trench.ToString()).ToList();
					//Get the layer details to bind additional attributes Trench
					var layerdetails = new BLLayer().getLayer(EntityType.Trench.ToString());
					objTrench.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Trench
					response.results = objTrench;
				}

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveTrench()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind TRENCH Dropdown
		/// <summary>BindTrenchDropDown </summary>
		/// <param name="objTrenchIn"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindTrenchDropDown(TrenchMaster objTrenchIn)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Trench.ToString());
			objTrenchIn.trenchTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Trench_Type.ToString()).ToList();
			objTrenchIn.MCGMWardIn = objDDL.Where(x => x.dropdown_type == DropDownType.MCGM_Ward.ToString()).ToList();
			objTrenchIn.StrataTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Strata_Type.ToString()).ToList();
			objTrenchIn.SurfaceTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Surface_Type.ToString()).ToList();
			objTrenchIn.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objTrenchIn.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objTrenchIn.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objTrenchIn.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
			objTrenchIn.lstTrenchServingType = objDDL.Where(x => x.dropdown_type == DropDownType.trench_serving_type.ToString()).ToList();
			//objTrenchIn.ExecutionMethodsIn = objDDL.Where(x => x.dropdown_type == DropDownType.execution_method.ToString()).ToList();
			objTrenchIn.listALocation = _objDDL.Where(x => x.dropdown_type == DropDownType.A_location.ToString()).ToList();
			objTrenchIn.listBLocation = _objDDL.Where(x => x.dropdown_type == DropDownType.A_location.ToString()).ToList();

		}
        #endregion

        #endregion

        #region FMS
        #region Add FMS
        /// <summary> Add FMS </summary>
        /// <param name="data">networkIdType,systemId,geom,userId</param>
        /// <returns>FMS Details</returns>
        /// <CreatedBy>Antra Mathurf</CreatedBy>

        public ApiResponse<FMSMaster> AddFMS(ReqInput data)
		{
			var response = new ApiResponse<FMSMaster>();
			try
			{
				FMSMaster objFMS = ReqHelper.GetRequestData<FMSMaster>(data);
				if (string.IsNullOrWhiteSpace(objFMS.geom) && objFMS.system_id == 0)
				{
					// get geom by parent id...
					objFMS.geom = GetPointTypeParentGeom(objFMS.pSystemId, objFMS.pEntityType);
				}
				FMSMaster objFMSMaster = GetFMSDetail(objFMS);

				BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
				BindFMSDropDown(objFMSMaster);
                BindFMSRoute(objFMSMaster);
                List<int> listI = new List<int>();
                foreach (var i in objFMSMaster.lstRouteInfo.Where(x => x.is_associated))
                {
                    listI.Add(i.cable_id);
                }
                objFMSMaster.selected_route_ids = listI;
                fillProjectSpecifications(objFMSMaster);
				new BLMisc().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
				objFMSMaster.pNetworkId = objFMS.pNetworkId;
				objFMSMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.FMS.ToString()).ToList();
				//Get the layer details to bind additional attributes FMS
				var layerdetails = new BLLayer().getLayer(EntityType.FMS.ToString());
				objFMSMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes FMS
				response.status = StatusCodes.OK.ToString();
				response.results = objFMSMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddFMS()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get FMS Details
		/// <summary> GetFMSDetail</summary>
		/// <param >networkIdType,pEntityType,geom,userId,pSystemId</param>
		/// <returns>FMS Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public FMSMaster GetFMSDetail(FMSMaster objFMS)
		{
			int id = objFMS.user_id;
			if (objFMS.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objFMS, GeometryType.Point.ToString(), objFMS.geom);
				//Fill Parent detail...              
				fillParentDetail(objFMS, new NetworkCodeIn() { eType = EntityType.FMS.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFMS.geom, parent_sysId = objFMS.pSystemId, parent_eType = objFMS.pEntityType }, objFMS.networkIdType);
				objFMS.longitude = Convert.ToDouble(objFMS.geom.Split(' ')[0]);
				objFMS.latitude = Convert.ToDouble(objFMS.geom.Split(' ')[1]);
				objFMS.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<FMSTemplateMaster>(objFMS.user_id, EntityType.FMS);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objFMS);
				objFMS.other_info = null;   //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objFMS = new BLMisc().GetEntityDetailById<FMSMaster>(objFMS.system_id, EntityType.FMS, objFMS.user_id);
				//for additional-attributes
				objFMS.other_info = new BLFMS().GetOtherInfoFMS(objFMS.system_id);
				fillRegionProvAbbr(objFMS);

			}
			objFMS.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());

			return objFMS;
		}

		#endregion

		#region Save FMS
		/// <summary> SaveFMS </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<FMSMaster> SaveFMS(ReqInput data)
		{
			var response = new ApiResponse<FMSMaster>();
			FMSMaster objFMSMaster = ReqHelper.GetRequestData<FMSMaster>(data);
			try
			{
				int pSystemId = objFMSMaster.pSystemId;
				string pEntitytype = objFMSMaster.pEntityType;
				string pNetworkId = objFMSMaster.pNetworkId;
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objFMSMaster.geom) && objFMSMaster.system_id == 0)
				{
					objFMSMaster.geom = GetPointTypeParentGeom(objFMSMaster.pSystemId, objFMSMaster.pEntityType);
				}

				ModelState.Clear();
				if (objFMSMaster.networkIdType == NetworkIdType.A.ToString() && objFMSMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.FMS.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFMSMaster.geom, parent_sysId = pSystemId, parent_eType = pEntitytype });
					if (objFMSMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objFMSMaster = GetFMSDetail(objFMSMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objFMSMaster.fms_name = objNetworkCodeDetail.network_code;
						objFMSMaster.pSystemId = pSystemId;
						objFMSMaster.pEntityType = pEntitytype;
						objFMSMaster.pNetworkId = pNetworkId;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objFMSMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objFMSMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objFMSMaster.network_id = objNetworkCodeDetail.network_code;
					objFMSMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (objFMSMaster.unitValue != null && objFMSMaster.unitValue.Contains(":"))
				{
					objFMSMaster.no_of_input_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[0]);
					objFMSMaster.no_of_output_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[1]);
				}
				if (string.IsNullOrEmpty(objFMSMaster.fms_name))
				{
					objFMSMaster.fms_name = objFMSMaster.network_id;
				}
				objFMSMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.FMS.ToString()).ToList();
				this.Validate(objFMSMaster);
				if (ModelState.IsValid)
				{
					var isNew = objFMSMaster.system_id > 0 ? false : true;
					objFMSMaster.is_new_entity = (isNew && objFMSMaster.source_ref_id != "0" && objFMSMaster.source_ref_id != "");
					var resultItem = new BLFMS().SaveEntityFMS(objFMSMaster, objFMSMaster.user_id);
                    BindFMSRoute(objFMSMaster);
                    List<RouteInfo> objL = new List<RouteInfo>();
                    foreach (var itm in objFMSMaster.lstRouteInfo)
                    {
                        bool f = false;
						if (objFMSMaster.selected_route_ids != null)
						{
							foreach (var ids in objFMSMaster.selected_route_ids)
							{
								if (ids == itm.cable_id)
								{
									f = true;

								}
							}
						}
                        RouteInfo objS = new RouteInfo();
                        objS.entity_id = resultItem.system_id;
                        objS.entity_type = EntityType.FMS.ToString();
                        objS.created_by = objFMSMaster.user_id.ToString();
                        objS.cable_id = itm.cable_id;
                        objS.is_associated = f;
                        objL.Add(objS);
                    }

                    var res = new BLMisc().saveRouteAssocition(JsonConvert.SerializeObject(objL), resultItem.system_id, EntityType.FMS.ToString(), objFMSMaster.user_id);
                    BindFMSRoute(objFMSMaster);
                    List<int> listI = new List<int>();
                    foreach (var i in objFMSMaster.lstRouteInfo.Where(x => x.is_associated))
                    {
                        listI.Add(i.cable_id);
                    }
                    objFMSMaster.selected_route_ids = listI;
                    //START SPLIT CABLE FEATURE BY ANTRA//
                    var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.FMS.ToString().ToUpper()).FirstOrDefault();
					if (objFMSMaster.split_cable_system_id > 0 && layerDetails.is_split_allowed == true && isNew)
					{
						SplitCable(objFMSMaster.system_id, objFMSMaster.split_cable_system_id, "FMS", objFMSMaster.network_status, objFMSMaster.user_id);
						string[] Layer_Name = { EntityType.Cable.ToString() };
						objFMSMaster.objPM.status = ResponseStatus.OK.ToString();
						objFMSMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
					}
					// END //
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.FMS.ToString() };

						if (isNew)
						{
							objFMSMaster.objPM.status = ResponseStatus.OK.ToString();
							objFMSMaster.objPM.isNewEntity = isNew;
							objFMSMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objFMSMaster.objPM.status = ResponseStatus.OK.ToString();
								objFMSMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
							}
							else
							{
								objFMSMaster.objPM.status = ResponseStatus.OK.ToString();
								objFMSMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					//else
					//{
					//    objFMSMaster.objPM.status = ResponseStatus.FAILED.ToString();
					//    objFMSMaster.objPM.message = objFMSMaster.objPM.message;
					//    response.status = ResponseStatus.FAILED.ToString();
					//    response.error_message = objFMSMaster.objPM.message;
					//}
				}
				else
				{
					objFMSMaster.objPM.status = ResponseStatus.FAILED.ToString();
					objFMSMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objFMSMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objFMSMaster;

				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
					BindFMSDropDown(objFMSMaster);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objFMSMaster);
					new BLMisc().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
					//Get the layer details to bind additional attributes FMS
					var layerdetails = new BLLayer().getLayer(EntityType.FMS.ToString());
					objFMSMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes FMS
					response.results = objFMSMaster;

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveFMS()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind FMS Dropdown
		/// <summary>BindFMSDropDown </summary>
		/// <param name="objFMSMaster"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindFMSDropDown(FMSMaster objFMSMaster)
		{
			objFMSMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objFMSMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objDDL = new BLMisc().GetDropDownList("");
			objFMSMaster.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objFMSMaster.lstServedByRing = objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
			objFMSMaster.listILocationCode = objDDL.Where(x => x.dropdown_type == DropDownType.A_location.ToString()).ToList();
			objFMSMaster.listFMSType = objDDL.Where(x => x.dropdown_type == DropDownType.FMS_Type.ToString()).ToList();
		}
        private void BindFMSRoute(FMSMaster objFMSMaster)
        {
            if (objFMSMaster.system_id == 0)
                objFMSMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objFMSMaster.geom);
            else
                objFMSMaster.lstRouteInfo = new BLMisc().getRouteEntityInLineBuffer(objFMSMaster.system_id, EntityType.FMS.ToString());
        }
        #endregion

        #endregion

        #region OpticalRepeater
        #region Add OpticalRepeater
        /// <summary> Add OpticalRepeater </summary>
        /// <returns>OpticalRepeater Details</returns>
        /// <CreatedBy>Tapish</CreatedBy>

        public ApiResponse<OpticalRepeaterInfo> AddOpticalRepeater(ReqInput data)
		{
			var response = new ApiResponse<OpticalRepeaterInfo>();
			try
			{
				OpticalRepeaterInfo objOpticalRepeater = ReqHelper.GetRequestData<OpticalRepeaterInfo>(data);
				if (string.IsNullOrWhiteSpace(objOpticalRepeater.geom) && objOpticalRepeater.system_id == 0)
				{
					// get geom by parent id...
					objOpticalRepeater.geom = GetPointTypeParentGeom(objOpticalRepeater.pSystemId, objOpticalRepeater.pEntityType);
				}
				OpticalRepeaterInfo model = getOpticalRepeaterInfo(objOpticalRepeater);
				if (objOpticalRepeater.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objOpticalRepeater.system_id, EntityType.OpticalRepeater.ToString());
					//model.objIspEntityMap.floor_id = ispMapping.floor_id;
					//model.objIspEntityMap.structure_id = ispMapping.structure_id;
					// model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				}
				else
				{
					model.objIspEntityMap.floor_id = objOpticalRepeater.objIspEntityMap.floor_id;
					model.objIspEntityMap.structure_id = objOpticalRepeater.objIspEntityMap.structure_id;
					model.objIspEntityMap.shaft_id = objOpticalRepeater.objIspEntityMap.shaft_id;
				}
				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.OpticalRepeater.ToString());
				BindOpticalRepeaterDropDown(model);
				new BLMisc().BindPortDetails(model, EntityType.OpticalRepeater.ToString(), DropDownType.OpticalRepeater_Port_Ratio.ToString());
				fillProjectSpecifications(model);
				//Get the layer details to bind additional attributes OpticalRepeater
				var layerdetails = new BLLayer().getLayer(EntityType.OpticalRepeater.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes OpticalRepeater
				response.status = StatusCodes.OK.ToString();
				response.results = model;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddOpticalRepeater()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get OpticalRepeater Details
		/// <summary> GetOpticalRepeaterDetail</summary>
		/// <param >networkIdType,templateId,geom,userId,SystemId</param>
		/// <returns>FMS Details</returns>
		/// <CreatedBy>Tapish</CreatedBy>
		public OpticalRepeaterInfo getOpticalRepeaterInfo(OpticalRepeaterInfo objOpticalRepeater)
		{
			int id = objOpticalRepeater.user_id;
			if (objOpticalRepeater.system_id != 0)
			{
				objOpticalRepeater = new BLMisc().GetEntityDetailById<OpticalRepeaterInfo>(objOpticalRepeater.system_id, EntityType.OpticalRepeater, objOpticalRepeater.user_id);
				//for additional-attributes
				objOpticalRepeater.other_info = new BLISP().GetOtherInfoOpticalRepeater(objOpticalRepeater.system_id);
				fillRegionProvAbbr(objOpticalRepeater);
			}
			else
			{
				objOpticalRepeater.longitude = Convert.ToDouble(objOpticalRepeater.geom.Split(' ')[0]);
				objOpticalRepeater.latitude = Convert.ToDouble(objOpticalRepeater.geom.Split(' ')[1]);
				objOpticalRepeater.ownership_type = "Own";
				fillRegionProvinceDetail(objOpticalRepeater, GeometryType.Point.ToString(), objOpticalRepeater.geom);
				fillParentDetail(objOpticalRepeater, new NetworkCodeIn() { eType = EntityType.OpticalRepeater.ToString(), gType = GeometryType.Point.ToString(), eGeom = objOpticalRepeater.geom, parent_eType = objOpticalRepeater.pEntityType, parent_sysId = objOpticalRepeater.pSystemId }, objOpticalRepeater.networkIdType);
				// for Manual network id type 
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<OpticalRepeaterTemplateMaster>(objOpticalRepeater.user_id, EntityType.OpticalRepeater);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objOpticalRepeater);
				objOpticalRepeater.other_info = null;   //for additional-attributes
			}
			objOpticalRepeater.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());

			return objOpticalRepeater;
		}

		#endregion

		#region Save OpticalRepeater
		/// <summary> SaveOpticalRepeater </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Tapish</CreatedBy>
		public ApiResponse<OpticalRepeaterInfo> SaveOpticalRepeater(ReqInput data)
		{
			var response = new ApiResponse<OpticalRepeaterInfo>();
			OpticalRepeaterInfo model = ReqHelper.GetRequestData<OpticalRepeaterInfo>(data);
			try
			{
				int pSystemId = model.pSystemId;
				string pEntitytype = model.pEntityType;
				string pNetworkId = model.pNetworkId;
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(model.geom) && model.system_id == 0)
				{
					model.geom = GetPointTypeParentGeom(model.pSystemId, model.pEntityType);
				}
				ModelState.Clear();
				model.userId = model.user_id;
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					var objISPNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.OpticalRepeater.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = pEntitytype, parent_sysId = pSystemId });
					model.longitude = Convert.ToDouble(model.geom.Split(' ')[0]);
					model.latitude = Convert.ToDouble(model.geom.Split(' ')[1]);
					if (model.isDirectSave == true)
					{
						model = getOpticalRepeaterInfo(model);
						model.amplifier_wavelength = 1530;
						model.signal_boost_value = 12;
						model.opticalrepeater_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						//  model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;
				}
				if (string.IsNullOrEmpty(model.opticalrepeater_name))
				{
					model.opticalrepeater_name = model.network_id;
				}
				this.Validate(model);
				if (ModelState.IsValid)
				{
					model.objIspEntityMap.structure_id = Convert.ToInt32(model.objIspEntityMap.AssociateStructure);
					model.objIspEntityMap.shaft_id = model.objIspEntityMap.AssoType == "Floor" ? 0 : model.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(model.objIspEntityMap.AssoType))
					{
						model.objIspEntityMap.shaft_id = 0; model.objIspEntityMap.floor_id = 0;
					}
					bool isNew = model.system_id == 0 ? true : false;
					if (model.unitValue != null && model.unitValue.Contains(":"))
					{
						model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
						model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
					}
					if (model.objIspEntityMap.structure_id == 0 && model.parent_system_id == 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.OpticalRepeater.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							model.parent_system_id = parentDetails.p_system_id;
							model.parent_network_id = parentDetails.p_network_id;
							model.parent_entity_type = parentDetails.p_entity_type;
						}
					}
					var result = new BLISP().SaveOpticalRepeaterDetails(model, model.user_id);
					if (string.IsNullOrEmpty(result.objPM.message))
					{
						string[] LayerName = { EntityType.OpticalRepeater.ToString() };
						if (model.EntityReference != null && result.system_id > 0)
						{
							SaveReference(model.EntityReference, result.system_id);
						}
						if (isNew)
						{
							model.objPM.status = ResponseStatus.OK.ToString(); ;
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}

						else
						{
							if (result.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
							}
							else
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						model.objPM.status = ResponseStatus.FAILED.ToString();
						model.objPM.message = model.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = model.objPM.message;
					}
				}
				else
				{
					model.objPM.status = ResponseStatus.FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.OpticalRepeater.ToString());
					BindOpticalRepeaterDropDown(model);
					new BLMisc().BindPortDetails(model, EntityType.OpticalRepeater.ToString(), DropDownType.OpticalRepeater_Port_Ratio.ToString());
					fillProjectSpecifications(model);
					//Get the layer details to bind additional attributes OpticalRepeater
					var layerdetails = new BLLayer().getLayer(EntityType.OpticalRepeater.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes OpticalRepeater
					model.entityType = EntityType.OpticalRepeater.ToString();
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveOpticalRepeater()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind OpticalRepeater Dropdown
		/// <summary>BindOpticalRepeaterDropDown </summary>
		/// <param name="objOpticalRepeaterMaster"></param>
		/// <CreatedBy>Tapish</CreatedBy>
		private void BindOpticalRepeaterDropDown(OpticalRepeaterInfo objOpticalRepeater)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objOpticalRepeater.parent_system_id, objOpticalRepeater.system_id, EntityType.OpticalRepeater.ToString());
			if (ispEntityMap != null)
			{
				objOpticalRepeater.objIspEntityMap.id = ispEntityMap.id;
				objOpticalRepeater.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objOpticalRepeater.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objOpticalRepeater.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objOpticalRepeater.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}
			objOpticalRepeater.objIspEntityMap.AssoType = objOpticalRepeater.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objOpticalRepeater.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objOpticalRepeater.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objOpticalRepeater.longitude + " " + objOpticalRepeater.latitude);
			if (objOpticalRepeater.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objOpticalRepeater.objIspEntityMap.structure_id);
				objOpticalRepeater.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objOpticalRepeater.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objOpticalRepeater.parent_entity_type == EntityType.UNIT.ToString())
				{
					objOpticalRepeater.objIspEntityMap.unitId = objOpticalRepeater.parent_system_id;
					//objONT.objIspEntityMap.AssoType = "";
					//objONT.objIspEntityMap.floor_id = 0;
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.HTB.ToString()).FirstOrDefault() != null)
			{
				objOpticalRepeater.objIspEntityMap.isValidParent = true;
				objOpticalRepeater.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objOpticalRepeater.objIspEntityMap.structure_id, objOpticalRepeater.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.OpticalRepeater.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objOpticalRepeater.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objOpticalRepeater.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objOpticalRepeater.objIspEntityMap.entity_type == null) { objOpticalRepeater.objIspEntityMap.entity_type = EntityType.OpticalRepeater.ToString(); }
			//objHTB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objOpticalRepeater.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objOpticalRepeater.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objOpticalDDL = new BLMisc().GetDropDownList(EntityType.OpticalRepeater.ToString());
			objOpticalRepeater.lstAmplifierType = objOpticalDDL.Where(x => x.dropdown_type == DropDownType.Amplifier_Type.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objOpticalRepeater.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objOpticalRepeater.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#endregion

		#region HTB
		#region Add HTB
		/// <summary> Add HTB </summary>
		/// <returns>HTB Details</returns>
		/// <CreatedBy>Antra Mathurf</CreatedBy>

		public ApiResponse<HTBInfo> AddHTB(ReqInput data)
		{
			var response = new ApiResponse<HTBInfo>();
			try
			{
				HTBInfo objHTB = ReqHelper.GetRequestData<HTBInfo>(data);
				HTBInfo model = getHTBInfo(objHTB);
				if (objHTB.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objHTB.system_id, EntityType.HTB.ToString());
					//model.objIspEntityMap.floor_id = ispMapping.floor_id;
					//model.objIspEntityMap.structure_id = ispMapping.structure_id;
					// model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				}
				else
				{
					model.objIspEntityMap.floor_id = objHTB.objIspEntityMap.floor_id;
					model.objIspEntityMap.structure_id = objHTB.objIspEntityMap.structure_id;
					model.objIspEntityMap.shaft_id = objHTB.objIspEntityMap.shaft_id;
				}
				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
				BindHTBDropDown(model);
				new BLMisc().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
				fillProjectSpecifications(model);
				//Get the layer details to bind additional attributes HTB
				var layerdetails = new BLLayer().getLayer(EntityType.HTB.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes HTB
				response.status = StatusCodes.OK.ToString();
				response.results = model;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddHTB()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get HTB Details
		/// <summary> GetHTBDetail</summary>
		/// <param >networkIdType,templateId,geom,userId,SystemId</param>
		/// <returns>FMS Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public HTBInfo getHTBInfo(HTBInfo objHTB)
		{
			int id = objHTB.user_id;
			if (objHTB.system_id != 0)
			{
				// objHTB = BLISP.Instance.getHTBDetails(systemId);
				objHTB = new BLMisc().GetEntityDetailById<HTBInfo>(objHTB.system_id, EntityType.HTB, objHTB.user_id);
				//for additional-attributes
				objHTB.other_info = new BLISP().GetOtherInfoHTB(objHTB.system_id);
				fillRegionProvAbbr(objHTB);
			}
			else
			{
				//if (networkIdType == NetworkIdType.M.ToString())
				//{
				if (string.IsNullOrWhiteSpace(objHTB.geom) && objHTB.system_id == 0)
				{
					// get geom by parent id...
					objHTB.geom = GetPointTypeParentGeom(objHTB.pSystemId, objHTB.pEntityType);
				}
				objHTB.longitude = Convert.ToDouble(objHTB.geom.Split(' ')[0]);
				objHTB.latitude = Convert.ToDouble(objHTB.geom.Split(' ')[1]);
				objHTB.ownership_type = "Own";
				fillRegionProvinceDetail(objHTB, GeometryType.Point.ToString(), objHTB.geom);
				fillParentDetail(objHTB, new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHTB.geom, parent_eType = objHTB.pEntityType, parent_sysId = objHTB.pSystemId }, objHTB.networkIdType);
				// for Manual network id type 

				var objItem = BLItemTemplate.Instance.GetTemplateDetail<HTBTemplateMaster>(objHTB.user_id, EntityType.HTB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objHTB);
				//var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHTB.geom, parent_eType = "", parent_sysId = 0 }, objHTB.networkIdType);
				// objHTB.network_id = ISPNetworkCodeDetail.network_code;
				objHTB.other_info = null;   //for additional-attributes
			}
			objHTB.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());

			return objHTB;
		}

		#endregion

		#region Save HTB
		/// <summary> SaveHTB </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<HTBInfo> SaveHTB(ReqInput data)
		{
			var response = new ApiResponse<HTBInfo>();
			HTBInfo model = ReqHelper.GetRequestData<HTBInfo>(data);
			try
			{
				ModelState.Clear();
				// int structure_id = model.objIspEntityMap.structure_id;
				// int? floor_id = model.objIspEntityMap.floor_id;
				//int? shaft_id = model.objIspEntityMap.shaft_id;

				int pSystemId = model.pSystemId;
				string pEntitytype = model.pEntityType;
				string pNetworkId = model.pNetworkId;
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(model.geom) && model.system_id == 0)
				{
					model.geom = GetPointTypeParentGeom(model.pSystemId, model.pEntityType);

				}


				model.userId = model.user_id;
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE... new NetworkCodeIn() { eType = EntityType.BDB.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBDB.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType)
					//var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 });
					var objISPNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = pEntitytype, parent_sysId = pSystemId });
					model.longitude = Convert.ToDouble(model.geom.Split(' ')[0]);
					model.latitude = Convert.ToDouble(model.geom.Split(' ')[1]);
					//NEW ENTITY->Fill Region and Province Detail..
					// fillRegionProvinceDetail(model, GeometryType.Point.ToString(), model.geom);

					// fillParentDetail(model, new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.geom, parent_eType = "", parent_sysId = 0 }, networkIdType);


					if (model.isDirectSave == true)
					{
						//objONTMaster.pSystemId, objONTMaster.pEntityType, objONTMaster.networkIdType, objONTMaster.system_id, objONTMaster.geom
						model = getHTBInfo(model);
						// model.objIspEntityMap.floor_id = floor_id;
						// model.objIspEntityMap.structure_id = structure_id;
						// model.objIspEntityMap.shaft_id = shaft_id;
						model.htb_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						//  model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;

				}

				//var structureDetails = new BLISP().GetStructureById(structure_id);
				//if (structureDetails != null)
				//{
				//    model.region_id = structureDetails.First().region_id;
				//    model.province_id = structureDetails.First().province_id;
				//    model.latitude = structureDetails.First().latitude;
				//    model.longitude = structureDetails.First().longitude;
				//}
				if (string.IsNullOrEmpty(model.htb_name))
				{
					model.htb_name = model.network_id;
				}
				this.Validate(model);
				if (ModelState.IsValid)
				{
					model.objIspEntityMap.structure_id = Convert.ToInt32(model.objIspEntityMap.AssociateStructure);
					model.objIspEntityMap.shaft_id = model.objIspEntityMap.AssoType == "Floor" ? 0 : model.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(model.objIspEntityMap.AssoType))
					{
						model.objIspEntityMap.shaft_id = 0; model.objIspEntityMap.floor_id = 0;
					}
					bool isNew = model.system_id == 0 ? true : false;
					if (model.unitValue != null && model.unitValue.Contains(":"))
					{
						model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
						model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
					}
					if (model.objIspEntityMap.structure_id == 0 && model.parent_system_id == 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.HTB.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							model.parent_system_id = parentDetails.p_system_id;
							model.parent_network_id = parentDetails.p_network_id;
							model.parent_entity_type = parentDetails.p_entity_type;
						}
					}
					var result = new BLISP().SaveHTBDetails(model, model.user_id);
					if (string.IsNullOrEmpty(result.objPM.message))
					{
						string[] LayerName = { EntityType.HTB.ToString() };
						if (model.EntityReference != null && result.system_id > 0)
						{
							SaveReference(model.EntityReference, result.system_id);
						}
						if (isNew)
						{
							model.objPM.status = ResponseStatus.OK.ToString(); ;
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}

						else
						{
							if (result.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
							}
							else
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						model.objPM.status = ResponseStatus.FAILED.ToString();
						model.objPM.message = model.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = model.objPM.message;
					}
				}
				else
				{
					model.objPM.status = ResponseStatus.FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
					BindHTBDropDown(model);
					new BLMisc().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
					fillProjectSpecifications(model);
					//Get the layer details to bind additional attributes HTB
					var layerdetails = new BLLayer().getLayer(EntityType.HTB.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes HTB
					model.entityType = EntityType.HTB.ToString();
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveHTB()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind HTB Dropdown
		/// <summary>BindHTBDropDown </summary>
		/// <param name="objHTBMaster"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindHTBDropDown(HTBInfo objHTB)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objHTB.parent_system_id, objHTB.system_id, EntityType.HTB.ToString());
			if (ispEntityMap != null)
			{
				objHTB.objIspEntityMap.id = ispEntityMap.id;
				objHTB.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objHTB.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objHTB.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objHTB.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}
			objHTB.objIspEntityMap.AssoType = objHTB.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objHTB.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objHTB.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objHTB.longitude + " " + objHTB.latitude);
			if (objHTB.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objHTB.objIspEntityMap.structure_id);
				objHTB.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objHTB.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objHTB.parent_entity_type == EntityType.UNIT.ToString())
				{
					objHTB.objIspEntityMap.unitId = objHTB.parent_system_id;
					//objONT.objIspEntityMap.AssoType = "";
					//objONT.objIspEntityMap.floor_id = 0;
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.HTB.ToString()).FirstOrDefault() != null)
			{
				objHTB.objIspEntityMap.isValidParent = true;
				objHTB.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objHTB.objIspEntityMap.structure_id, objHTB.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.HTB.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objHTB.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objHTB.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objHTB.objIspEntityMap.entity_type == null) { objHTB.objIspEntityMap.entity_type = EntityType.HTB.ToString(); }
			//objHTB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objHTB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objHTB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList("");
			objHTB.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objHTB.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#endregion

		#region Building
		#region Add Building
		/// <summary> Add Building </summary>
		/// <returns>Building Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<BuildingMaster> AddBuilding(ReqInput data)
		{
			var response = new ApiResponse<BuildingMaster>();
			try
			{
				BuildingMaster objBuilding = ReqHelper.GetRequestData<BuildingMaster>(data);
				BuildingMaster obj = GetBuildingDetail(objBuilding);
				obj.CJParent_system_id = objBuilding.system_id;
				obj.CJParent_entity_type = EntityType.Structure.ToString();
				obj.role_id = objBuilding.role_id;
				obj.lstUserModule = objBuilding.lstUserModule;
				//Bind dropdowns..
				BindBuildingDropDown(obj);
				//Get the layer details to bind additional attributes Building
				var layerdetails = new BLLayer().getLayer(EntityType.Building.ToString());
				obj.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Cable
				if (obj.childModel != "")
					obj.childModel = objBuilding.childModel;
				response.status = StatusCodes.OK.ToString();
				response.results = obj;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddBuilding()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get Building Details
		/// <summary> GetBuildingDetail</summary>
		/// <param >networkIdType,geom,userId,SystemId</param>
		/// <returns>Building Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public BuildingMaster GetBuildingDetail(BuildingMaster objBM)
		{
			int user_id = objBM.user_id;
			if (objBM.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objBM, GeometryType.Point.ToString(), objBM.geom);
				//Fill Parent detail...              
				fillParentDetail(objBM, new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBM.geom }, objBM.networkIdType);

				// fill latlong values
				string[] lnglat = objBM.geom.Split(new string[] { " " }, StringSplitOptions.None);
				objBM.latitude = Convert.ToDouble(lnglat[1].ToString());
				objBM.longitude = Convert.ToDouble(lnglat[0].ToString());
				//fill survey area detail   
				var objSurveyArea = BLBuilding.Instance.GetSurveyAreaExist(objBM.geom, GeometryType.Point.ToString());
				var objLocality = BLBuilding.Instance.GetBuildingLocality(objBM.longitude, objBM.latitude);
				if (objLocality != null)
				{
					objBM.locality = objLocality.locality;
					objBM.sub_locality = objLocality.sub_locality;
				}
				if (objSurveyArea != null)
				{
					objBM.surveyarea_id = objSurveyArea.system_id;
					objBM.surveyarea_name = objSurveyArea.surveyarea_name;
				}
				objBM.created_on = DateTimeHelper.Now;
			}
			else
			{
				// Get entity detail by Id...
				objBM = new BLMisc().GetEntityDetailById<BuildingMaster>(objBM.system_id, EntityType.Building, objBM.user_id);
				objBM.createDate = Utility.MiscHelper.FormatDate(objBM.created_on.ToString());
				if (!string.IsNullOrEmpty(objBM.rfs_date.ToString()))
					objBM.rfsSetDate = Utility.MiscHelper.FormatDate(objBM.rfs_date.ToString());
				objBM.other_info = BLBuilding.Instance.GetOtherInfoBuilding(objBM.system_id);
				fillRegionProvAbbr(objBM);
			}
			objBM.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objBM;
		}

		#endregion

		#region Save Building
		/// <summary> SaveBuilding </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<BuildingMaster> SaveBuilding(ReqInput data)
		{
			var response = new ApiResponse<BuildingMaster>();
			BuildingMaster objBuilding = ReqHelper.GetRequestData<BuildingMaster>(data);
			var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.Building.ToString().ToUpper()).FirstOrDefault() : null;
			try
			{
				ModelState.Clear();
				PageMessage objPM = new PageMessage();
				if (objBuilding.system_id == 0)
				{ objBuilding.btnaction = "Save"; }
				else if (string.IsNullOrEmpty(objBuilding.btnaction)) { objBuilding.btnaction = "Update"; }
				if (objBuilding.networkIdType == NetworkIdType.A.ToString() && objBuilding.system_id == 0)
				{
					if (ApplicationSettings.IsbldApprovedEnabled)
					{
						objBuilding.IsbldApprovedStatus = true;
						objBuilding.building_status = BuildingStatus.Approved.ToString();
					};
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBuilding.geom });
					if (objBuilding.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objBuilding = GetBuildingDetail(objBuilding);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS HERE
						//CURRENTLY NO VALUE IS INITIALIZED THERE AS BUILDING CAN NOT SAVED DIRECTLY
						// IF IN FUTURE WE NEED TO SAVE IT DIRECTLY THEN WE WOULD HAVE TO CREATE  A TEMPLATE FOR SAME
						// AND THEN WILL HAVE TO FILL ALL REQUIRED FILEDS VALUES FROM TEMPLATE
						objBuilding.btnaction = "Save";
					}
					//SET NETWORK CODE
					objBuilding.network_id = objNetworkCodeDetail.network_code;
					objBuilding.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objBuilding.building_name))
				{
					objBuilding.building_name = objBuilding.network_id;
				}
				this.Validate(objBuilding);
				if (ModelState.IsValid)
				{
					BuildingAction bldAction = (BuildingAction)Enum.Parse(typeof(BuildingAction), objBuilding.btnaction);
					var isNew = objBuilding.system_id > 0 ? false : true;
					objBuilding.is_new_entity = (isNew && objBuilding.source_ref_id != "0" && objBuilding.source_ref_id != "");
					objBuilding.userid = objBuilding.user_id;
					objBuilding.bldAction = bldAction;
					if (!string.IsNullOrEmpty(objBuilding.rfsSetDate))
						objBuilding.rfs_date = Convert.ToDateTime(objBuilding.rfsSetDate);

					var result = BLBuilding.Instance.SaveBuilding(objBuilding, NetworkStatus.P);
					if (string.IsNullOrEmpty(result.pageMsg.message))
					{

						if (result.system_id != 0)
						{
							string[] LayerName = { EntityType.Building.ToString() };
							switch (bldAction)
							{
								case BuildingAction.Save:
									objPM.isNewEntity = true;
									objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);//Resources.Resources.SI_OSP_BUL_NET_FRM_002;
									response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);//Resources.Resources.SI_OSP_BUL_NET_FRM_002;
									break;
								case BuildingAction.Update:
									objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);//Resources.Resources.SI_OSP_BUL_NET_FRM_005;
									response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName); //Resources.Resources.SI_OSP_BUL_NET_FRM_005;
									break;
								case BuildingAction.Reject:
									objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_006;
									response.error_message = Resources.Resources.SI_OSP_BUL_NET_FRM_006;
									break;
								case BuildingAction.Approve:
									objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_007;
									response.error_message = Resources.Resources.SI_OSP_BUL_NET_FRM_007;
									break;

							}

							if (!string.IsNullOrEmpty(result.building_status) && result.building_status.ToLower() == "approved" && objBuilding.btnaction.ToLower() != "approve" && objBuilding.objVSATDetails.is_vsat_updated && layerDetail.is_vsat_enabled)
							{
								objBuilding.entityType = layerDetail.layer_name;
								SaveVsat(objBuilding);
							}


							// For default structure
							if ((bldAction == BuildingAction.Approve) || (bldAction == BuildingAction.Save && objBuilding.IsbldApprovedStatus))
							//if (bldAction == BuildingAction.Approve && objBuilding.tenancy.ToLower() == "single")
							{
								var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Structure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objBuilding.geom, parent_eType = EntityType.Building.ToString(), parent_sysId = objBuilding.system_id });

								StructureMaster objStructure = new StructureMaster();

								objStructure.userid = objBuilding.user_id;
								objStructure.system_id = 0;
								objStructure.parent_system_id = objNetworkCodeDetail.parent_system_id;
								objStructure.building_id = objNetworkCodeDetail.parent_system_id;
								objStructure.parent_entity_type = objNetworkCodeDetail.parent_entity_type;
								objStructure.parent_network_id = objNetworkCodeDetail.parent_network_id;
								objStructure.sequence_id = objNetworkCodeDetail.sequence_id;
								objStructure.network_id = objNetworkCodeDetail.network_code;
								//objStructure.no_of_floor = objBuilding.no_of_floor == 0 ? 1 : objBuilding.no_of_floor;
								objStructure.no_of_floor = 1;// No_of_floor will be 1 for default structure.
								objStructure.no_of_flat = objStructure.no_of_floor;//objBuilding.no_of_flat;
								objStructure.business_pass = objBuilding.business_pass == 0 ? 0 : 1;
								objStructure.home_pass = objBuilding.home_pass == 0 ? 0 : 1;
								objStructure.no_of_shaft = 2;
								objStructure.no_of_occupants = 1;
								objStructure.structure_name = "Tower-1";
								objStructure.geom = objBuilding.longitude.ToString() + " " + objBuilding.latitude.ToString();
								objStructure.longitude = objBuilding.longitude;
								objStructure.latitude = objBuilding.latitude;
								objStructure.region_id = objBuilding.region_id;
								objStructure.province_id = objBuilding.province_id;
								objStructure.isDefault = true;
								//if (objStructure.no_of_shaft > 0)
								//{
								//    StructureShaftInfo objShaft = new StructureShaftInfo();
								//    for (int i = 0; i < objStructure.no_of_shaft; i++)
								//    {
								//        objShaft.is_virtual = false;
								//        objShaft.shaft_name = "Shaft_" + (i + 1);
								//        objShaft.shaft_position = "left";
								//        objStructure.lstShaftInfo.Add(objShaft);
								//    }

								//}
								if (objStructure.no_of_floor > 0)
								{

									for (int i = 0; i < objStructure.no_of_floor; i++)
									{
										StructureFloorInfo objFloor = new StructureFloorInfo();
										objFloor.no_of_units = 1;
										objFloor.floor_name = "floor_" + (i + 1);
										objFloor.height = Convert.ToInt32(ApplicationSettings.DefaultFloorHeight);
										objFloor.width = Convert.ToInt32(ApplicationSettings.DefaultFloorWidth);
										objFloor.length = Convert.ToInt32(ApplicationSettings.DefaultFloorLength);
										objStructure.lstFloorInfo.Add(objFloor);
									}
								}

								var resultStruct = BLStructure.Instance.SaveStructure(objStructure, NetworkStatus.P.ToString());

							}
							if (bldAction == BuildingAction.Save)
								//save AT Status                        
								if (objBuilding.ATAcceptance != null && objBuilding.system_id > 0)
								{
									SaveATAcceptance(objBuilding.ATAcceptance, objBuilding.system_id, objBuilding.user_id);
								}

							//var isnew = objStructure.system_id == 0 ? true : false;
							//var result = BLStructure.Instance.SaveStructure(objStructure, NetworkStatus.P.ToString());
							//if (result.system_id != 0)
							//{
							//    if (isnew)
							//    {
							//        objPM.status = ResponseStatus.OK.ToString();
							//        objPM.isNewEntity = isnew;
							//        objPM.message = "Structure  Saved successfully !";
							//    }
							//    else
							//    {
							//        objPM.status = ResponseStatus.OK.ToString();
							//        objPM.message = "Structure Updated successfully !";
							//    }
							//    objStructure = result;
							//}
							//else
							//{
							//    objPM.status = ResponseStatus.FAILED.ToString();
							//    objPM.message = "Error while saving structure detail!";
							//}
							//For Save VSAT 

							objPM.status = ResponseStatus.OK.ToString();
							response.status = ResponseStatus.OK.ToString();
							objBuilding = result;
						}
						else
						{
							objPM.status = ResponseStatus.FAILED.ToString();
							objPM.message = Resources.Resources.SI_OSP_BUL_NET_FRM_008;
							response.status = ResponseStatus.FAILED.ToString();
							response.error_message = Resources.Resources.SI_OSP_BUL_NET_FRM_008;
						}
						objBuilding.pageMsg = objPM;
					}
					else
					{
						objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
						objPM.message = result.pageMsg.message;
						response.error_message = result.pageMsg.message;
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
						objBuilding.pageMsg = objPM;
					}
				}
				else
				{
					objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objBuilding.pageMsg = objPM;

				}


				if (objBuilding.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE 
					response.status = ResponseStatus.OK.ToString();
					response.results = objBuilding;
				}
				else
				{
					// RETURN PARTIAL VIEW WITH MODEL DATA
					// fill dropdowns
					BindBuildingDropDown(objBuilding);
					var layerdetails = new BLLayer().getLayer(EntityType.Building.ToString());
					objBuilding.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					objBuilding.createDate = Utility.MiscHelper.FormatDate(objBuilding.created_on.ToString());

					if (!string.IsNullOrEmpty(objBuilding.rfs_date.ToString()))
						objBuilding.rfsSetDate = Utility.MiscHelper.FormatDate(objBuilding.rfs_date.ToString());
					response.results = objBuilding;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveBuilding()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Bind Building Dropdown
		/// <summary>BindBuildingDropDown </summary>
		/// <param name="objBuilding"></param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindBuildingDropDown(BuildingMaster objBuilding)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Building.ToString());
			objBuilding.lstTenancy = objDDL.Where(x => x.dropdown_type == DropDownType.Tenancy.ToString()).ToList();
			objBuilding.lstCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Category.ToString()).ToList();
			objBuilding.lstRFSStatus = objDDL.Where(x => x.dropdown_type == DropDownType.RFS_Status.ToString()).ToList();
			objBuilding.lstMedia = objDDL.Where(x => x.dropdown_type == DropDownType.Media.ToString()).ToList();
			objBuilding.lstBuildingType = objDDL.Where(x => x.dropdown_type == DropDownType.Building_Type.ToString()).ToList();
			objBuilding.lstSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.SubCategory.ToString()).ToList();
		}
		#endregion

		#endregion

		#region Structure
		#region Add Structure
		/// <summary> Add Structure </summary>
		/// <returns>Structure Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<StructureMaster> AddStructure(ReqInput data)
		{
			var response = new ApiResponse<StructureMaster>();
			try
			{
				StructureMaster obj = ReqHelper.GetRequestData<StructureMaster>(data);
				StructureMaster objStruc = GetStructureDetail(obj);
				response.status = StatusCodes.OK.ToString();
				response.results = objStruc;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddStructure()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get Structure Details
		/// <summary> GetStructureDetail</summary>
		/// <param >networkIdType,geom,building_id,userId,SystemId</param>
		/// <returns>Building Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public StructureMaster GetStructureDetail(StructureMaster objStructure)
		{
			int user_id = objStructure.user_id;
			if (objStructure.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objStructure, GeometryType.Point.ToString(), objStructure.geom);
				//Fill Parent detail...              
				fillParentDetail(objStructure, new NetworkCodeIn() { eType = EntityType.Structure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objStructure.geom, parent_eType = EntityType.Building.ToString(), parent_sysId = objStructure.building_id }, objStructure.networkIdType);
				objStructure.created_on = DateTimeHelper.Now;
				objStructure.building_id = objStructure.building_id;
				// fill latlong values
				string[] lnglat = objStructure.geom.Split(new string[] { " " }, StringSplitOptions.None);
				objStructure.latitude = Convert.ToDouble(lnglat[1].ToString());
				objStructure.longitude = Convert.ToDouble(lnglat[0].ToString());
				List<StructureShaftInfo> shaftList = new List<StructureShaftInfo>();
				shaftList.Add(new StructureShaftInfo { is_virtual = true, shaft_name = "Shaft_1", shaft_position = "left" });
				shaftList.Add(new StructureShaftInfo { is_virtual = true, shaft_name = "Shaft_2", shaft_position = "right" });
				objStructure.lstShaftInfo = shaftList;
				objStructure.no_of_shaft = 2;
			}
			else
			{

				objStructure = new BLMisc().GetEntityDetailById<StructureMaster>(objStructure.system_id, EntityType.Structure, objStructure.user_id);
				objStructure.lstShaftInfo = BLShaft.Instance.GetShaftByBld(objStructure.system_id);
				objStructure.lstFloorInfo = BLFloor.Instance.GetFloorByBld(objStructure.system_id);
				objStructure.geom = objStructure.longitude.ToString() + " " + objStructure.latitude.ToString();
				fillRegionProvAbbr(objStructure);
			}
			objStructure.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objStructure;
		}

		#endregion

		#region Save Structure
		/// <summary> SaveStructure </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<StructureMaster> SaveStructure(ReqInput data)
		{
			var response = new ApiResponse<StructureMaster>();
			StructureMaster objStructure = ReqHelper.GetRequestData<StructureMaster>(data);
			try
			{
				/*....SITE INFORMATION is not included in API section!! */
				//if (objStructure.actionTab == "SiteInfoTab")
				//{
				//    objStructure.SiteInformation.ActionTab = objStructure.actionTab;
				//    return SaveSiteInfo(objStructure.SiteInformation, objStructure.user_id);
				//}
				//else
				//{
				bool shaftWithRiser = false;
				DbMessage resp = new DbMessage();
				ModelState.Clear();
				var strGeom = objStructure.geom;
				if (objStructure.networkIdType == NetworkIdType.A.ToString() && objStructure.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Structure.ToString(), gType = GeometryType.Point.ToString(), eGeom = objStructure.geom, parent_eType = EntityType.Building.ToString(), parent_sysId = objStructure.building_id });
					if (objStructure.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objStructure = GetStructureDetail(objStructure);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS HERE
						//CURRENTLY NO VALUE IS INITIALIZED THERE AS STRUCTURE CAN NOT SAVED DIRECTLY
						// IF IN FUTURE WE NEED TO SAVE IT DIRECTLY THEN WE WOULD HAVE TO CREATE  A TEMPLATE FOR SAME
						// AND THEN WILL HAVE TO FILL ALL REQUIRED FILEDS VALUES FROM TEMPLATE
					}
					//SET NETWORK CODE
					objStructure.network_id = objNetworkCodeDetail.network_code;
					objStructure.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objStructure.structure_name))
				{
					objStructure.structure_name = objStructure.network_id;
				}
				this.Validate(objStructure);
				if (ModelState.IsValid)
				{
					objStructure.userid = objStructure.user_id;
					var isnew = objStructure.system_id == 0 ? true : false;
					var shaftList = objStructure.lstShaftInfo.Select(m => new { system_id = m.system_id, shaft_position = m.shaft_position });
					//string cable = objStructure.parent_network_id+":"+
					//if (objStructure.system_id > 0)
					//{
					//    response = BLCable.Instance.isIspLineExists(JsonConvert.SerializeObject(shaftList), objStructure.system_id, objStructure.lstFloorInfo.Count(), objStructure.lstShaftInfo.Count());
					//}
					if (!resp.status)
					{
						for (int i = 0; i < objStructure.lstShaftInfo.Count; i++)
						{
							if (objStructure.lstShaftInfo[i].with_riser)
							{ shaftWithRiser = true; }

							if (objStructure.lstShaftInfo[i].length == 0)
								objStructure.lstShaftInfo[i].length = ApplicationSettings.Shaft_Length_Mtr;
							if (objStructure.lstShaftInfo[i].width == 0)
								objStructure.lstShaftInfo[i].width = ApplicationSettings.Shaft_Width_Mtr;
						}
						var formSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Structure.ToString()).ToList();
						var withRiser = formSettings.Count > 0 ? formSettings.Where(m => m.form_feature_name.ToLower() == formFeatureName.with_riser.ToString() && m.form_feature_type.ToLower() == formFeatureType.feature.ToString() && m.is_active == true).FirstOrDefault() : null;
						if (withRiser != null && shaftWithRiser)
						{
							var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(objStructure.user_id, EntityType.FDB);
							if (objItem.specification == "" || objItem.specification == null)
							{
								objStructure.pageMsg.status = ResponseStatus.FAILED.ToString();
								objStructure.pageMsg.message = Resources.Resources.SI_OSP_STR_NET_FRM_045;
								response.error_message = Resources.Resources.SI_OSP_STR_NET_FRM_045;
								response.status = ResponseStatus.FAILED.ToString();
								response.results = objStructure;
							}
						}
						for (int i = 0; i < objStructure.lstFloorInfo.Count; i++)
						{
							if (objStructure.lstFloorInfo[i].length == 0)
								objStructure.lstFloorInfo[i].length = ApplicationSettings.DefaultFloorLength;
							if (objStructure.lstFloorInfo[i].width == 0)
								objStructure.lstFloorInfo[i].width = ApplicationSettings.DefaultFloorWidth;
							if (objStructure.lstFloorInfo[i].height == 0)
								objStructure.lstFloorInfo[i].height = ApplicationSettings.DefaultFloorHeight;

							//if (objStructure.lstFloorInfo[i].length == 0)
							//    objStructure.lstFloorInfo[i].length = ApplicationSettings.Floor_Length_Mtr;
							//if (objStructure.lstFloorInfo[i].width == 0)
							//    objStructure.lstFloorInfo[i].width = ApplicationSettings.Floor_Width_Mtr;
							//if (objStructure.lstFloorInfo[i].height == 0)
							//    objStructure.lstFloorInfo[i].height = ApplicationSettings.Floor_Height_Mtr;
						}

						//add validationn for updating the structure depended on shaft and floor
						var shaftName = BLStructure.Instance.CheckEntityAssociation(objStructure);
						entityATStatusAtachmentsList objATA = objStructure.ATAcceptance;
						//SiteInfo objSite = objStructure.SiteInformation;
						objStructure = BLStructure.Instance.SaveStructure(objStructure, NetworkStatus.P.ToString());
						if (string.IsNullOrEmpty(objStructure.pageMsg.message))
						{
							string[] LayerName = { EntityType.Structure.ToString() };
							if (objStructure.system_id != 0)
							{
								if (isnew)
								{
									objStructure.pageMsg.status = ResponseStatus.OK.ToString();
									objStructure.pageMsg.isNewEntity = isnew;
									objStructure.pageMsg.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
									response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
									response.status = ResponseStatus.OK.ToString();
								}
								else
								{
									objStructure.pageMsg.status = ResponseStatus.OK.ToString();
									objStructure.pageMsg.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
									response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
									response.status = ResponseStatus.OK.ToString();
								}
								objStructure.geom = strGeom;
							}
							else
							{
								objStructure.pageMsg.status = ResponseStatus.FAILED.ToString();
								objStructure.pageMsg.message = Resources.Resources.SI_OSP_STR_NET_FRM_044;
								response.error_message = Resources.Resources.SI_OSP_STR_NET_FRM_044;
								response.status = ResponseStatus.FAILED.ToString();
							}
							if (shaftName != "")
							{

								objStructure.pageMsg.message += "<strong>" + Resources.Resources.SI_OSP_STR_NET_FRM_046 + " " + "[" + shaftName + "]" + " " + Resources.Resources.SI_OSP_STR_NET_FRM_047 + "</strong>";
								//objPM.message += "<strong> FDB is associated with other entity,So you can not set the shaft[" + shaftName + "] without riser!</strong>";
								//objPM.message += "<br/><table class='shaftLstInfo' cellpadding='0' cellspacing='0' style='font-size:smaller;text-align:center;'><thead><tr><th>Shaft Name</th></tr></thead><tbody>" + shaftName + "</tbody></table>";
							}
							if (objATA != null && objStructure.system_id > 0)
							{
								SaveATAcceptance(objATA, objStructure.system_id, objStructure.user_id);
							}
							//if (objSite != null && objStructure.system_id > 0)
							//{
							//    SaveSiteInfo(objSite, objStructure.system_id,EntityType.Structure.ToString(),objStructure.network_id);
							//}
						}
						else
						{
							objStructure.pageMsg.status = ResponseStatus.VALIDATION_FAILED.ToString();
							response.error_message = objStructure.pageMsg.message;
							response.status = ResponseStatus.VALIDATION_FAILED.ToString();
						}
					}
					else
					{
						objStructure.pageMsg.status = ResponseStatus.FAILED.ToString();
						objStructure.pageMsg.message = resp.message;
						response.error_message = resp.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objStructure.pageMsg.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objStructure.pageMsg.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();

				}
				if (objStructure.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objStructure;
				}
				else
				{
					if (objStructure.system_id != 0 && string.IsNullOrEmpty(objStructure.pageMsg.message))
					{
						objStructure.lstShaftInfo = BLShaft.Instance.GetShaftByBld(objStructure.system_id);
						objStructure.lstFloorInfo = BLFloor.Instance.GetFloorByBld(objStructure.system_id);
					}
					// RETURN PARTIAL VIEW WITH MODEL DATA
					response.results = objStructure;
				}
				//}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveStructure()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region SaveSiteInfo
		public ApiResponse<StructureMaster> SaveSiteInfo(SiteInfo objSiteInfo, int userId)
		{
			var response = new ApiResponse<StructureMaster>();
			ModelState.Clear();
			this.Validate(objSiteInfo);
			if (ModelState.IsValid)
			{
				var result = BLSiteInfo.Instance.SaveSiteInfo(objSiteInfo, userId);
				if (string.IsNullOrEmpty(result.objPM.message))
				{
					if (objSiteInfo.objPM.isNewEntity)
					{
						objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_203;
						objSiteInfo.objPM.status = ResponseStatus.OK.ToString();
						response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_203;
						response.status = ResponseStatus.OK.ToString();
					}
					else
					{
						objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_204;
						objSiteInfo.objPM.status = ResponseStatus.OK.ToString();
						response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_204;
						response.status = ResponseStatus.OK.ToString();
					}
				}
				else
				{
					objSiteInfo.objPM.status = ResponseStatus.FAILED.ToString();
					objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_205;
					response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_205;
					response.status = ResponseStatus.FAILED.ToString();
				}

			}
			else
			{
				objSiteInfo.objPM.status = ResponseStatus.FAILED.ToString();
				response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_203;
				response.status = ResponseStatus.FAILED.ToString();
				objSiteInfo.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_203;
			}
			BindSiteDropDown(objSiteInfo);
			return response;
		}
		#endregion

		#region BindSiteInfo
		private void BindSiteDropDown(SiteInfo objsiteInfo)
		{
			objsiteInfo.lstLMCType = new BLMisc().GetDropDownList("", DropDownType.LMC_TYPE.ToString());
			objsiteInfo.lstSITEType = new BLMisc().GetDropDownList("", DropDownType.SITE_TYPE.ToString());
			objsiteInfo.lstStructureType = new BLMisc().GetDropDownList("", DropDownType.Structure_Type.ToString());
			objsiteInfo.lstStructureSize = new BLMisc().GetDropDownList("", DropDownType.Structure_Size.ToString());
			objsiteInfo.lstSiteCircle = new BLSiteCircle().GetCircleList();

		}
		#endregion
		#endregion

		#region CSA
		#region Add CSA
		/// <summary> Add CSA </summary>
		/// <returns>CSA Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<CSA> AddCSA(ReqInput data)
		{
			var response = new ApiResponse<CSA>();
			try
			{
				CSA obj = ReqHelper.GetRequestData<CSA>(data);
				if (string.IsNullOrWhiteSpace(obj.geom) && obj.system_id == 0)
				{
					obj.geom = new BLMisc().getEntityGeom(obj.pSystemId, obj.pEntityType);
				}
				CSA objCsa = GetCSADetail(obj);
				objCsa.lstCsaRFS = new BLMisc().GetDropDownList(EntityType.CSA.ToString(), DropDownType.CSA_RFS.ToString());
				response.status = StatusCodes.OK.ToString();
				response.results = objCsa;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddCSA()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get CSA Details
		/// <summary> GetCSADetail</summary>
		/// <param >networkIdType,geom,userId,SystemId</param>
		/// <returns>CSA Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public CSA GetCSADetail(CSA objCsa)
		{
			int user_id = objCsa.user_id;
			if (objCsa.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objCsa, GeometryType.Polygon.ToString(), objCsa.geom);
				//Fill Parent detail...              
				fillParentDetail(objCsa, new NetworkCodeIn() { eType = EntityType.CSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objCsa.geom }, objCsa.networkIdType);

				//objCsa.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();

			}
			else
			{
				// Get entity detail by Id...
				objCsa = new BLMisc().GetEntityDetailById<CSA>(objCsa.system_id, EntityType.CSA, objCsa.user_id);
				fillRegionProvAbbr(objCsa);
			}
			objCsa.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objCsa;
		}

		#endregion

		#region Save CSA
		/// <summary> SaveCSA </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<CSA> SaveCSA(ReqInput data)
		{
			var response = new ApiResponse<CSA>();
			CSA objCsa = ReqHelper.GetRequestData<CSA>(data);

			try
			{
				ModelState.Clear();
				var CsaDetail = new BLMisc().GetEntityDetailById<CSA>(objCsa.system_id, EntityType.CSA, objCsa.user_id);
				if (objCsa.networkIdType == NetworkIdType.A.ToString() && objCsa.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.CSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objCsa.geom });
					if (objCsa.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objCsa = GetCSADetail(objCsa);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objCsa.csa_name = objNetworkCodeDetail.network_code;
					}
					//SET NETWORK CODE
					objCsa.network_id = objNetworkCodeDetail.network_code;
					objCsa.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				objCsa.lstCsaRFS = new BLMisc().GetDropDownList(EntityType.CSA.ToString(), DropDownType.CSA_RFS.ToString());
				//  objCsa.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
				if (string.IsNullOrEmpty(objCsa.csa_name))
				{
					objCsa.csa_name = objCsa.network_id;
				}
				objCsa.network_status = "P";
				this.Validate(objCsa);
				if (ModelState.IsValid)
				{
					DbMessage msg = new DbMessage();
					var isNew = objCsa.system_id > 0 ? false : true;
					objCsa.is_new_entity = (isNew && objCsa.source_ref_id != "0" && objCsa.source_ref_id != "");

					var resultItem = new BLCsa().SaveCSA(objCsa, objCsa.user_id);

					msg = new BLCsa().CalculateHomePasses(resultItem.system_id);

					//var result = VirtualBuilding(resultItem);
					//if (string.IsNullOrEmpty(result.objPM.message))
					//{

					string[] LayerName = { EntityType.CSA.ToString() };
					if (isNew)
					{
						objCsa.objPM.status = ResponseStatus.OK.ToString();
						objCsa.objPM.isNewEntity = isNew;
						objCsa.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
					}
					else
					{
						objCsa.objPM.status = ResponseStatus.OK.ToString();
						objCsa.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
					}


				}
				else
				{
					objCsa.objPM.status = ResponseStatus.FAILED.ToString();
					objCsa.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
					response.results = objCsa;
				}
				if (objCsa.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objCsa;

				}
				else
				{
					// RETURN PARTIAL VIEW WITH MODEL DATA
					response.results = objCsa;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveCSA()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion
		#endregion

		#region DSA
		#region Add DSA
		/// <summary> Add DSA </summary>
		/// <returns>DSA Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<DSA> AddDSA(ReqInput data)
		{
			var response = new ApiResponse<DSA>();
			try
			{
				DSA obj = ReqHelper.GetRequestData<DSA>(data);
				DSA objDsa = GetDSADetail(obj);
				response.status = StatusCodes.OK.ToString();
				response.results = objDsa;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddDSA()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get DSA Details
		/// <summary> GetDSADetail</summary>
		/// <param >networkIdType,geom,userId,SystemId</param>
		/// <returns>DSA Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public DSA GetDSADetail(DSA objDSA)
		{
			int user_id = objDSA.user_id;
			if (objDSA.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objDSA, GeometryType.Polygon.ToString(), objDSA.geom);
				//Fill Parent detail...              
				fillParentDetail(objDSA, new NetworkCodeIn() { eType = EntityType.DSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objDSA.geom }, objDSA.networkIdType);
				var obj_DDL = new BLMisc().GetDropDownList("");
				objDSA.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
			}
			else
			{
				// Get entity detail by Id...
				objDSA = new BLMisc().GetEntityDetailById<DSA>(objDSA.system_id, EntityType.DSA, objDSA.user_id);
				var obj_DDL = new BLMisc().GetDropDownList("");
				objDSA.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
				fillRegionProvAbbr(objDSA);
			}
			objDSA.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objDSA;
		}

		#endregion

		#region Save DSA
		/// <summary> SaveDSA </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<DSA> SaveDSA(ReqInput data)
		{
			var response = new ApiResponse<DSA>();
			DSA objDsa = ReqHelper.GetRequestData<DSA>(data);
			try
			{
				ModelState.Clear();
				var objDDL = new BLMisc().GetDropDownList(EntityType.DSA.ToString());
				var obj_DDL = new BLMisc().GetDropDownList("");
				if (objDsa.networkIdType == NetworkIdType.A.ToString() && objDsa.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.DSA.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objDsa.geom });
					if (objDsa.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objDsa = GetDSADetail(objDsa);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objDsa.dsa_name = objNetworkCodeDetail.network_code;
						objDsa.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
						objDsa.served_by_ring = objDsa.lstServedByRing[0].dropdown_value;
					}
					//SET NETWORK CODE
					objDsa.network_id = objNetworkCodeDetail.network_code;
					objDsa.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objDsa.dsa_name))
				{
					objDsa.dsa_name = objDsa.network_id;
				}
				//var obj_DDL = new BLMisc().GetDropDownList("");
				objDsa.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
				objDsa.network_status = "P";
				this.Validate(objDsa);
				if (ModelState.IsValid)
				{
					var isNew = objDsa.system_id > 0 ? false : true;
					objDsa.is_new_entity = (isNew && objDsa.source_ref_id != "0" && objDsa.source_ref_id != "");
					var resultItem = new BLDsa().SaveDSA(objDsa, objDsa.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.DSA.ToString() };

						if (isNew)
						{
							objDsa.objPM.status = ResponseStatus.OK.ToString();
							objDsa.objPM.isNewEntity = isNew;
							objDsa.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							objDsa.objPM.status = ResponseStatus.OK.ToString();
							objDsa.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objDsa.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
						objDsa.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					}
				}
				else
				{
					objDsa.objPM.status = ResponseStatus.FAILED.ToString();
					objDsa.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objDsa.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objDsa;
				}
				else
				{
					// RETURN PARTIAL VIEW WITH MODEL DATA
					response.results = objDsa;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveDSA()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion
		#endregion

		#region SubArea
		#region Add SubArea
		/// <summary> Add SubArea </summary>
		/// <returns>SubArea Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<SubArea> AddSubArea(ReqInput data)
		{
			var response = new ApiResponse<SubArea>();
			try
			{
				SubArea obj = ReqHelper.GetRequestData<SubArea>(data);
				if (string.IsNullOrWhiteSpace(obj.geom) && obj.system_id == 0)
				{
					obj.geom = new BLMisc().getEntityGeom(obj.pSystemId, obj.pEntityType);
				}
				SubArea objSubArea = GetSubAreaDetail(obj);
				response.status = StatusCodes.OK.ToString();
				response.results = objSubArea;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddSubArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get SubArea Details
		/// <summary> GetSubAreaDetail</summary>
		/// <param >networkIdType,geom,userId,SystemId</param>
		/// <returns>SubArea Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public SubArea GetSubAreaDetail(SubArea objSubArea)
		{
			int user_id = objSubArea.user_id;
			var objDDL = new BLMisc().GetDropDownList(EntityType.SubArea.ToString());
			int id = objSubArea.user_id;
			if (objSubArea.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objSubArea, GeometryType.Polygon.ToString(), objSubArea.geom);
				//Fill Parent detail...              
				//fillParentDetail(objSubArea, new NetworkCodeIn() { eType = EntityType.SubArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSubArea.geom }, objSubArea.networkIdType);
				////krishna
				fillParentDetail(objSubArea, new NetworkCodeIn() { eType = EntityType.SubArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSubArea.geom, parent_eType = objSubArea.pEntityType, parent_sysId = objSubArea.pSystemId }, objSubArea.networkIdType);

			}
			else
			{
				// Get entity detail by Id...
				objSubArea = new BLMisc().GetEntityDetailById<SubArea>(objSubArea.system_id, EntityType.SubArea, objSubArea.user_id);
				fillRegionProvAbbr(objSubArea);

			}

			objSubArea.lstSubAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.SubArea_RFS.ToString()).ToList();
			objSubArea.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objSubArea;
		}

		#endregion

		#region Save SubArea
		/// <summary> SaveSubArea </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<SubArea> SaveSubArea(ReqInput data)
		{
			var response = new ApiResponse<SubArea>();
			SubArea objSubArea = ReqHelper.GetRequestData<SubArea>(data);
			try
			{
				ModelState.Clear();
				BuildingMaster objBuilding = new BuildingMaster();
				var objDDL = new BLMisc().GetDropDownList(EntityType.Area.ToString());
				objSubArea.lstSubAreaRFS = objDDL.Where(x => x.dropdown_type == DropDownType.SubArea_RFS.ToString()).ToList();
				if (objSubArea.networkIdType == NetworkIdType.A.ToString() && objSubArea.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.SubArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSubArea.geom, parent_eType = objSubArea.parent_entity_type, parent_sysId = objSubArea.parent_system_id });
					if (objSubArea.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objSubArea = GetSubAreaDetail(objSubArea);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objSubArea.subarea_name = objNetworkCodeDetail.network_code;
					}
					//SET NETWORK CODE
					objSubArea.network_id = objNetworkCodeDetail.network_code;
					objSubArea.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objSubArea.subarea_name))
				{
					objSubArea.subarea_name = objSubArea.network_id;
				}
				objSubArea.network_status = "P";
				this.Validate(objSubArea);
				if (ModelState.IsValid)
				{
					var isNew = objSubArea.system_id > 0 ? false : true;
					objSubArea.is_new_entity = (isNew && objSubArea.source_ref_id != "0" && objSubArea.source_ref_id != "");
					var resultItem = new BLSubArea().SaveSubArea(objSubArea, objSubArea.user_id);
					var result = VirtualBuilding(resultItem);
					if (string.IsNullOrEmpty(result.objPM.message))
					{
						string[] LayerName = { EntityType.SubArea.ToString() };

						if (isNew)
						{
							objSubArea.objPM.status = ResponseStatus.OK.ToString();
							objSubArea.objPM.isNewEntity = isNew;
							objSubArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							objSubArea.objPM.status = ResponseStatus.OK.ToString();
							objSubArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objSubArea.objPM.status = ResponseStatus.FAILED.ToString();
						objSubArea.objPM.message = getFirstErrorFromModelState();
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = getFirstErrorFromModelState();


					}
				}
				else
				{
					objSubArea.objPM.status = ResponseStatus.FAILED.ToString();
					objSubArea.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objSubArea.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objSubArea;
				}
				else
				{
					// RETURN PARTIAL VIEW WITH MODEL DATA
					response.results = objSubArea;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveSubArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region VirualBuilding
		/// <param name="data">objSubArea</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public SubArea VirtualBuilding(SubArea objSubArea)
		{
			BuildingMaster objBuilding = new BuildingMaster();
			EditGeomIn geomObj = new EditGeomIn();
			//GET SUBAREA GEOMETRY, REQUIRE TO UPDATE THE VIRTUAL BUILDING GEOM ASSOCIATED WITH SUBAREA.
			var geom = new BLMisc().getEntityGeom(objSubArea.system_id, EntityType.SubArea.ToString());
			if ((objSubArea.subarea_rfs == "A-RFS" || objSubArea.subarea_rfs == "A-RFS Type1" || objSubArea.subarea_rfs == "A-RFS Type2") && string.IsNullOrWhiteSpace(objSubArea.building_code))
			{
				var buildingNetworkCode = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Building.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = geom });
				var dropdownlist = new BLMisc().GetDropDownList(EntityType.Building.ToString());

				var objCategory = dropdownlist.Where(x => x.dropdown_key.ToUpper() == "RESIDENTIAL" && x.dropdown_type == DropDownType.Category.ToString()).FirstOrDefault();
				objBuilding.category = objCategory != null ? objCategory.dropdown_value : "";

				var objTenancy = dropdownlist.Where(x => x.dropdown_key.ToUpper() == "MULTIPLE" && x.dropdown_type == DropDownType.Tenancy.ToString()).FirstOrDefault();
				objBuilding.tenancy = objTenancy != null ? objTenancy.dropdown_value : "";

				objSubArea.building_code = buildingNetworkCode.network_code;
				objBuilding.network_id = buildingNetworkCode.network_code;
				objBuilding.sequence_id = buildingNetworkCode.sequence_id;
				objBuilding.region_id = objSubArea.region_id;
				objBuilding.province_id = objSubArea.province_id;
				objBuilding.geom = geom;
				objBuilding.region_name = objSubArea.region_name;
				objBuilding.province_name = objSubArea.province_name;
				objBuilding.parent_entity_type = buildingNetworkCode.parent_entity_type;
				objBuilding.parent_system_id = buildingNetworkCode.parent_system_id;
				objBuilding.parent_network_id = buildingNetworkCode.parent_network_id;
				objBuilding.building_name = "Default Building";
				objBuilding.business_pass = 9999;
				objBuilding.home_pass = 9999;
				objBuilding.total_tower = 9999;
				objBuilding.rfs_status = objSubArea.subarea_rfs;
				objBuilding.rfs_date = DateTimeHelper.Now;
				objBuilding.is_virtual = true;
				objBuilding.building_status = BuildingStatus.Approved.ToString();
				objBuilding.userid = objSubArea.user_id;
				var result = BLBuilding.Instance.SaveBuilding(objBuilding, NetworkStatus.P);

				// UPDATE BUILDING CODE AND SYSTEM ID INTO SUBAREA
				objSubArea.building_system_id = result.system_id;
				objSubArea.geom = result.geom;
				var resultItem = new BLSubArea().UpdateSubAreaBuildingCode(objSubArea, objSubArea.user_id);

				// DEFAULT GEOMETRY FOR BUILDING IS ALREADY INSERTING THROUGH TRIGGER.
				// BELOW SECTION WILL UPDATE BUILDING GEOM AS SUBAREA GEOMETRY.
				geomObj.systemId = result.system_id;
				geomObj.longLat = objSubArea.geom;
				geomObj.userId = objSubArea.user_id;
				geomObj.entityType = EntityType.Building.ToString();
				geomObj.geomType = GeometryType.Polygon.ToString();
				var updateGeom = BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
				return objSubArea;
			}
			else if ((objSubArea.subarea_rfs == "A-RFS" || objSubArea.subarea_rfs == "A-RFS Type1" || objSubArea.subarea_rfs == "A-RFS Type2") && !string.IsNullOrWhiteSpace(objSubArea.building_code))
			{
				//UPDATE BUILDING RFS STATUS ONLY
				var result = BLBuilding.Instance.UpdateBuildingRFSType(objSubArea.subarea_rfs, objSubArea.building_system_id, objSubArea.user_id);

			}
			return objSubArea;
		}
		#endregion

		#endregion

		#region Coupler
		#region Add Coupler
		/// <summary> Add Coupler </summary>
		/// <returns>Coupler Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<CouplerMaster> AddCoupler(ReqInput data)
		{
			var response = new ApiResponse<CouplerMaster>();
			try
			{
				CouplerMaster obj = ReqHelper.GetRequestData<CouplerMaster>(data);
				CouplerMaster objCouplerMaster = GetCouplerDetail(obj);
				BLItemTemplate.Instance.BindItemDropdowns(objCouplerMaster, EntityType.Coupler.ToString());
				fillProjectSpecifications(objCouplerMaster);
				BindCouplerDropDown(objCouplerMaster);
				objCouplerMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Coupler.ToString()).ToList();
				//Get the layer details to bind additional attributes Coupler
				var layerdetails = new BLLayer().getLayer(EntityType.Coupler.ToString());
				objCouplerMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Coupler
				response.status = StatusCodes.OK.ToString();
				response.results = objCouplerMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddCoupler()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get Coupler Details
		/// <summary> GetCouplerDetail</summary>
		/// <param >networkIdType,geom,userId,SystemId</param>
		/// <returns>Coupler Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public CouplerMaster GetCouplerDetail(CouplerMaster objCoupler)
		{
			int user_id = objCoupler.user_id;
			if (objCoupler.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objCoupler, GeometryType.Point.ToString(), objCoupler.geom);
				//Fill Parent detail...              
				fillParentDetail(objCoupler, new NetworkCodeIn() { eType = EntityType.Coupler.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCoupler.geom }, objCoupler.networkIdType);
				objCoupler.longitude = Convert.ToDouble(objCoupler.geom.Split(' ')[0]);
				objCoupler.latitude = Convert.ToDouble(objCoupler.geom.Split(' ')[1]);
				objCoupler.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<CouplerTemplateMaster>(objCoupler.user_id, EntityType.Coupler);
				MiscHelper.CopyMatchingProperties(objItem, objCoupler);
				objCoupler.other_info = null;  //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objCoupler = new BLMisc().GetEntityDetailById<CouplerMaster>(objCoupler.system_id, EntityType.Coupler, objCoupler.user_id);
				//for additional-attributes
				objCoupler.other_info = new BLCoupler().GetOtherInfoCoupler(objCoupler.system_id);
				fillRegionProvAbbr(objCoupler);
			}
			objCoupler.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());

			return objCoupler;
		}

		#endregion

		#region Save Coupler
		/// <summary> SaveCoupler </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<CouplerMaster> SaveCoupler(ReqInput data)
		{
			var response = new ApiResponse<CouplerMaster>();
			CouplerMaster objCouplerMaster = ReqHelper.GetRequestData<CouplerMaster>(data);
			try
			{
				ModelState.Clear();
				if (objCouplerMaster.networkIdType == NetworkIdType.A.ToString() && objCouplerMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Coupler.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCouplerMaster.geom });
					if (objCouplerMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objCouplerMaster = GetCouplerDetail(objCouplerMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objCouplerMaster.coupler_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objCouplerMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//  objCouplerMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objCouplerMaster.network_id = objNetworkCodeDetail.network_code;
					objCouplerMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objCouplerMaster.coupler_name))
				{
					objCouplerMaster.coupler_name = objCouplerMaster.network_id;
				}
				this.Validate(objCouplerMaster);
				if (ModelState.IsValid)
				{
					var isNew = objCouplerMaster.system_id > 0 ? false : true;
					var resultItem = new BLCoupler().SaveEntityCoupler(objCouplerMaster, objCouplerMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						if (objCouplerMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objCouplerMaster.EntityReference, resultItem.system_id);
						}
						string[] LayerName = { EntityType.Coupler.ToString() };
						if (isNew)
						{
							objCouplerMaster.objPM.status = ResponseStatus.OK.ToString();
							objCouplerMaster.objPM.isNewEntity = isNew;
							objCouplerMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{

							objCouplerMaster.objPM.status = ResponseStatus.OK.ToString();
							objCouplerMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objCouplerMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objCouplerMaster.objPM.message = getFirstErrorFromModelState();
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
						response.error_message = getFirstErrorFromModelState();
						response.results = objCouplerMaster;
					}
				}
				else
				{
					objCouplerMaster.objPM.status = ResponseStatus.FAILED.ToString();
					objCouplerMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objCouplerMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objCouplerMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objCouplerMaster, EntityType.Coupler.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objCouplerMaster);
					BindCouplerDropDown(objCouplerMaster);
					//Get the layer details to bind additional attributes Coupler
					var layerdetails = new BLLayer().getLayer(EntityType.Coupler.ToString());
					objCouplerMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Coupler
					objCouplerMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Coupler.ToString()).ToList();
					response.results = objCouplerMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveCoupler()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region BindCouplerDropDown
		/// <param name="data">objCouplerMaster</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public void BindCouplerDropDown(CouplerMaster objCouplerMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Coupler.ToString());
			objCouplerMaster.listCouplerType = objDDL.Where(x => x.dropdown_type == DropDownType.Coupler.ToString()).ToList();
			objCouplerMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objCouplerMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objCouplerMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objCouplerMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#endregion

		#region ISP Pod

		#region ISP Pod Details
		/// <summary> Get Pod Details</summary>
		/// <param name="data">objPOD,ModelInfo</param>
		/// <returns>ISP Pod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public PODMaster GetISPPODDetail(PODMaster objPOD)
		{
			if (objPOD.system_id == 0)
			{
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objPOD.objIspEntityMap.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objPOD, structureDetails, GeometryType.Point.ToString());
				objPOD.ownership_type = "Own";
				//Fill Parent detail...              
				fillISPParentDetail(objPOD, new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), parent_networkId = objPOD.pNetworkId, eType = EntityType.POD.ToString(), structureId = objPOD.objIspEntityMap.structure_id }, objPOD.networkIdType, objPOD.pSystemId, objPOD.pEntityType, objPOD.pNetworkId);
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<PODTemplateMaster>(objPOD.user_id, EntityType.POD);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objPOD);
				objPOD.address = BLStructure.Instance.getBuildingAddress(objPOD.objIspEntityMap.structure_id);
				objPOD.other_info = null;   //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objPOD = new BLMisc().GetEntityDetailById<PODMaster>(objPOD.system_id, EntityType.POD, objPOD.user_id);
				var ispMapping = new BLISP().getMappingByEntityId(objPOD.system_id, EntityType.POD.ToString());
				if (ispMapping != null)
				{
					objPOD.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					objPOD.objIspEntityMap.floor_id = ispMapping.floor_id;
					objPOD.objIspEntityMap.structure_id = ispMapping.structure_id;
				}
				//for additional-attributes
				objPOD.other_info = new BLPOD().GetOtherInfoPOD(objPOD.system_id);
				fillRegionProvAbbr(objPOD);
			}
			return objPOD;
		}

		#endregion

		#region Add ISP Pod
		/// <summary> Add ISP Pod </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Wallmount Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<PODMaster> AddISPPod(ReqInput data)
		{
			var response = new ApiResponse<PODMaster>();
			try
			{
				PODMaster objRequestIn = ReqHelper.GetRequestData<PODMaster>(data);
				PODMaster objPODMaster = GetISPPODDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objPODMaster, EntityType.POD.ToString());
				fillProjectSpecifications(objPODMaster);
				BindISPPODDropDown(objPODMaster);
				//Get the layer details to bind additional attributes POD
				var layerdetails = new BLLayer().getLayer(EntityType.POD.ToString());
				objPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes POD
				response.status = StatusCodes.OK.ToString();
				response.results = objPODMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPPod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP Pod Dropdown
		/// <summary> Bind ISP Pod Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP Pod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindISPPODDropDown(PODMaster objPODMaster)
		{
			objPODMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objPODMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objDDL = new BLMisc().GetDropDownList("");
			objPODMaster.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objPODMaster.lstServedByRing = objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save ISP Pod
		/// <summary> SaveISPPod </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<PODMaster> SaveISPPod(ReqInput data)
		{
			var response = new ApiResponse<PODMaster>();
			PODMaster objPODMaster = ReqHelper.GetRequestData<PODMaster>(data);
			try
			{
				if (objPODMaster.networkIdType == NetworkIdType.A.ToString() && objPODMaster.system_id == 0)
				{
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objPODMaster.objIspEntityMap.structure_id, EntityType.Structure);
					//GET AUTO NETWORK CODE...               
					var objNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), eType = EntityType.POD.ToString(), structureId = objPODMaster.objIspEntityMap.structure_id });
					if (objPODMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objPODMaster = GetISPPODDetail(objPODMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objPODMaster.pod_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objPODMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objPODMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objPODMaster.network_id = objNetworkCodeDetail.network_code;
					objPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;

				}
				this.Validate(objPODMaster);
				if (ModelState.IsValid)
				{

					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault().layer_title;
					var isNew = objPODMaster.system_id > 0 ? false : true;
					var resultItem = new BLPOD().SaveEntityPOD(objPODMaster, objPODMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						if (objPODMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objPODMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objPODMaster.objPM.status = ResponseStatus.OK.ToString();
							objPODMaster.objPM.isNewEntity = isNew;
							objPODMaster.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, layer_title);
							objPODMaster.objPM.systemId = resultItem.system_id;
							objPODMaster.objPM.entityType = resultItem.entityType;
							objPODMaster.objPM.structureId = objPODMaster.objIspEntityMap.structure_id;
							objPODMaster.objPM.shaftId = objPODMaster.objIspEntityMap.shaft_id ?? 0;
							objPODMaster.objPM.floorId = objPODMaster.objIspEntityMap.floor_id ?? 0;
							objPODMaster.objPM.pSystemId = objPODMaster.parent_system_id;
							objPODMaster.objPM.pEntityType = objPODMaster.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, layer_title);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objPODMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objPODMaster.system_id, EntityType.POD.ToString(), objPODMaster.network_id, objPODMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPODMaster.longitude + " " + objPODMaster.latitude }, objPODMaster.user_id);
								objPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objPODMaster.objPM.message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = string.Format(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, layer_title);
							}
						}

					}
					else
					{
						objPODMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objPODMaster.objPM.message = objPODMaster.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = objPODMaster.objPM.message;
					}
				}
				else
				{
					objPODMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objPODMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objPODMaster.isDirectSave == true)
				{
					response.results = objPODMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objPODMaster, EntityType.POD.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPPODDropDown(objPODMaster);
					fillProjectSpecifications(objPODMaster);
					//Get the layer details to bind additional attributes POD
					var layerdetails = new BLLayer().getLayer(EntityType.POD.ToString());
					objPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes POD
					response.results = objPODMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPPod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion

		#region ISP Mpod

		#region ISP Mpod Details
		/// <summary> Get Mpod Details</summary>
		/// <param name="data">objMPOD,ModelInfo</param>
		/// <returns>Mpod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public MPODMaster GetISPMPODDetail(MPODMaster objMPOD)
		{
			if (objMPOD.system_id == 0)
			{
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objMPOD.objIspEntityMap.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objMPOD, structureDetails, GeometryType.Point.ToString());
				//Fill Parent detail...              
				fillISPParentDetail(objMPOD, new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), parent_networkId = objMPOD.pNetworkId, eType = EntityType.MPOD.ToString(), structureId = objMPOD.objIspEntityMap.structure_id }, objMPOD.networkIdType, objMPOD.pSystemId, objMPOD.pEntityType, objMPOD.pNetworkId);
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<MPODTemplateMaster>(objMPOD.user_id, EntityType.MPOD);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objMPOD);
				objMPOD.address = BLStructure.Instance.getBuildingAddress(objMPOD.objIspEntityMap.structure_id);
				objMPOD.ownership_type = "Own";
				objMPOD.geom = structureDetails.longitude + " " + structureDetails.latitude;
				objMPOD.other_info = null;  //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objMPOD = new BLMisc().GetEntityDetailById<MPODMaster>(objMPOD.system_id, EntityType.MPOD, objMPOD.user_id);
				objMPOD.geom = objMPOD.longitude + " " + objMPOD.latitude;
				var ispMapping = new BLISP().getMappingByEntityId(objMPOD.system_id, EntityType.MPOD.ToString());
				if (ispMapping != null)
				{
					objMPOD.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					objMPOD.objIspEntityMap.floor_id = ispMapping.floor_id;
					objMPOD.objIspEntityMap.structure_id = ispMapping.structure_id;
				}
				//for additional-attributes
				objMPOD.other_info = new BLMPOD().GetOtherInfoMPOD(objMPOD.system_id);
				fillRegionProvAbbr(objMPOD);
			}
			return objMPOD;
		}

		#endregion

		#region Add ISP Mpod
		/// <summary> Add ISP Mpod </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Wallmount Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<MPODMaster> AddISPMpod(ReqInput data)
		{
			var response = new ApiResponse<MPODMaster>();
			try
			{
				MPODMaster objRequestIn = ReqHelper.GetRequestData<MPODMaster>(data);

				MPODMaster objMPODMaster = GetISPMPODDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objMPODMaster, EntityType.MPOD.ToString());
				fillProjectSpecifications(objMPODMaster);
				BindISPMPODDropDown(objMPODMaster);
				//Get the layer details to bind additional attributes MPOD
				var layerdetails = new BLLayer().getLayer(EntityType.MPOD.ToString());
				objMPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes MPOD
				response.status = StatusCodes.OK.ToString();
				response.results = objMPODMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPMpod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP Mpod Dropdown
		/// <summary> Bind ISP Mpod Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP Mpod Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindISPMPODDropDown(MPODMaster objMPODMaster)
		{
			objMPODMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objMPODMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objDDL = new BLMisc().GetDropDownList("");
			objMPODMaster.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objMPODMaster.lstServedByRing = objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save ISP Mpod
		/// <summary> SaveISPMpod </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<MPODMaster> SaveISPMpod(ReqInput data)
		{
			var response = new ApiResponse<MPODMaster>();
			MPODMaster objMPODMaster = ReqHelper.GetRequestData<MPODMaster>(data);
			try
			{
				if (objMPODMaster.networkIdType == NetworkIdType.A.ToString() && objMPODMaster.system_id == 0)
				{
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objMPODMaster.objIspEntityMap.structure_id, EntityType.Structure);
					//GET AUTO NETWORK CODE...               
					var objNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), eType = EntityType.MPOD.ToString(), structureId = objMPODMaster.objIspEntityMap.structure_id });
					if (objMPODMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objMPODMaster = GetISPMPODDetail(objMPODMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objMPODMaster.mpod_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objMPODMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objMPODMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objMPODMaster.network_id = objNetworkCodeDetail.network_code;
					objMPODMaster.sequence_id = objNetworkCodeDetail.sequence_id;

				}
				this.Validate(objMPODMaster);
				if (ModelState.IsValid)
				{
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.MPOD.ToString().ToUpper()).FirstOrDefault().layer_title;
					var isNew = objMPODMaster.system_id > 0 ? false : true;
					var resultItem = new BLMPOD().SaveEntityMPOD(objMPODMaster, objMPODMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						string[] LayerName = { EntityType.MPOD.ToString() };
						if (objMPODMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objMPODMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objMPODMaster.objPM.status = ResponseStatus.OK.ToString();
							objMPODMaster.objPM.isNewEntity = isNew;
							objMPODMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							objMPODMaster.objPM.systemId = resultItem.system_id;
							objMPODMaster.objPM.entityType = resultItem.entityType;
							objMPODMaster.objPM.structureId = objMPODMaster.objIspEntityMap.structure_id;
							objMPODMaster.objPM.shaftId = objMPODMaster.objIspEntityMap.shaft_id ?? 0;
							objMPODMaster.objPM.floorId = objMPODMaster.objIspEntityMap.floor_id ?? 0;
							objMPODMaster.objPM.pSystemId = objMPODMaster.parent_system_id;
							objMPODMaster.objPM.pEntityType = objMPODMaster.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objMPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objMPODMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objMPODMaster.system_id, EntityType.MPOD.ToString(), objMPODMaster.network_id, objMPODMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objMPODMaster.longitude + " " + objMPODMaster.latitude }, objMPODMaster.user_id);
								objMPODMaster.objPM.status = ResponseStatus.OK.ToString();
								objMPODMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName); ;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName); ;
							}
						}
					}
					else
					{
						objMPODMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objMPODMaster.objPM.message = objMPODMaster.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = objMPODMaster.objPM.message;
					}
				}
				else
				{
					objMPODMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objMPODMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objMPODMaster.isDirectSave == true)
				{
					response.results = objMPODMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objMPODMaster, EntityType.MPOD.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPMPODDropDown(objMPODMaster);
					fillProjectSpecifications(objMPODMaster);
					//Get the layer details to bind additional attributes MPOD
					var layerdetails = new BLLayer().getLayer(EntityType.MPOD.ToString());
					objMPODMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes MPOD
					response.results = objMPODMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPMpod()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion

		#region ISP ONT 
		#region Add ISP ONT
		/// <summary> Add ISP ONT </summary>
		/// <param name="data">ReqInput</param>
		/// <returns>ONT Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<ONTMaster> AddISPONT(ReqInput data)
		{
			var response = new ApiResponse<ONTMaster>();
			try
			{
				ONTMaster objONT = ReqHelper.GetRequestData<ONTMaster>(data);
				ONTMaster objONTMaster = GetISPONTDetail(objONT);
				BLItemTemplate.Instance.BindItemDropdowns(objONTMaster, EntityType.ONT.ToString());
				//BindONTDropDown(objONTMaster);
				BindISPONT_Dropdown(objONTMaster);
				fillProjectSpecifications(objONTMaster);
				//if (systemId != 0)
				//{
				//    var ispMapping = new BLISP().getMappingByEntityId(systemId, EntityType.ONT.ToString());
				//    objONTMaster.objIspEntityMap.floor_id = ispMapping.floor_id;
				//    objONTMaster.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				//    objONTMaster.objIspEntityMap.structure_id = ispMapping.structure_id;
				//}
				//else
				//{
				//    var structureDetails = new BLISP().GetStructureById(ModelInfo.structureid);
				//    if (structureDetails != null && structureDetails.Count > 0)
				//    {
				//        objONTMaster.region_id = structureDetails.First().region_id;
				//        objONTMaster.province_id = structureDetails.First().province_id;
				//        objONTMaster.latitude = structureDetails.First().latitude;
				//        objONTMaster.longitude = structureDetails.First().longitude;
				//        objONTMaster.parent_network_id = structureDetails.First().network_id;
				//    }
				//    objONTMaster.objIspEntityMap.floor_id = ModelInfo.floorid;
				//    objONTMaster.objIspEntityMap.shaft_id = ModelInfo.shaftid;
				//    objONTMaster.objIspEntityMap.structure_id = ModelInfo.structureid;
				//}
				new BLMisc().BindPortDetails(objONTMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
				//Get the layer details to bind additional attributes ONT
				var layerdetails = new BLLayer().getLayer(EntityType.ONT.ToString());
				objONTMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes ONT
				response.status = StatusCodes.OK.ToString();
				response.results = objONTMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPONT()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get ISP ONT Details
		/// <summary> GetISPONTDetail</summary>
		/// <param >pEntityType,networkIdType,geom,systemId,pSystemId,userId</param>
		/// <returns>ISPONT Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ONTMaster GetISPONTDetail(ONTMaster objONT)
		{
			if (objONT.system_id == 0)
			{
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objONT.objIspEntityMap.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objONT, structureDetails, GeometryType.Point.ToString());
				//Fill Parent detail...              
				fillISPParentDetail(objONT, new ISPNetworkCodeIn() { parent_sysId = objONT.pSystemId, parent_eType = objONT.pEntityType, eType = EntityType.ONT.ToString(), structureId = objONT.structure_id }, objONT.networkIdType, objONT.pSystemId, objONT.pEntityType, objONT.pNetworkId);
				//Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<ONTTemplateMaster>(objONT.user_id, EntityType.ONT);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objONT);
				objONT.ownership_type = "Own";
				objONT.geom = structureDetails.longitude + " " + structureDetails.latitude;
				objONT.other_info = null;   //for additional-attributes
			}
			else
			{
				objONT = new BLMisc().GetEntityDetailById<ONTMaster>(objONT.system_id, EntityType.ONT, objONT.user_id);
				var ispMapping = new BLISP().getMappingByEntityId(objONT.system_id, EntityType.ONT.ToString());
				if (ispMapping != null && ispMapping.id > 0)
				{
					objONT.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					objONT.objIspEntityMap.floor_id = ispMapping.floor_id;
					objONT.objIspEntityMap.structure_id = ispMapping.structure_id;
				}
				objONT.geom = objONT.longitude + " " + objONT.latitude;
				//for additional-attributes
				objONT.other_info = new BLONT().GetOtherInfoONT(objONT.system_id);
				fillRegionProvAbbr(objONT);
			}
			objONT.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");
			return objONT;
		}
		#endregion

		#region Bind ISP ONT Dropdown
		/// <summary>BindISPONTDropDown </summary>
		/// <param name="objONTMaster"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindISPONT_Dropdown(ONTMaster ObjONT)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.FMS.ToString());
			ObjONT.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			ObjONT.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			ObjONT.lstCpeType = new BLMisc().GetDropDownList("", "CPE TYPE");
			var _objDDL = new BLMisc().GetDropDownList("");
			ObjONT.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
		}
		#endregion

		#region Save ISP ONT
		/// <summary> SaveISPONT </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<ONTMaster> SaveISPONT(ReqInput data)
		{
			var response = new ApiResponse<ONTMaster>();
			ONTMaster objONTMaster = ReqHelper.GetRequestData<ONTMaster>(data);
			try
			{
				if (objONTMaster.networkIdType == NetworkIdType.A.ToString() && objONTMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objONTMaster.pSystemId, parent_eType = objONTMaster.pEntityType, eType = EntityType.ONT.ToString(), structureId = objONTMaster.structure_id });
					if (objONTMaster.isDirectSave == true)
					{
						objONTMaster = GetISPONTDetail(objONTMaster);
						//modelMaster.objIspEntityMap.structure_id = structure_id;
						//modelMaster.objIspEntityMap.floor_id = floor_id;
						//modelMaster.objIspEntityMap.shaft_id = shaft_id;
						objONTMaster.ont_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objONTMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objONTMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
						//var structureDetails = new BLISP().GetStructureById(structure_id);
						//if (structureDetails != null && structureDetails.Count > 0)
						//{
						//    modelMaster.region_id = structureDetails.First().region_id;
						//    modelMaster.province_id = structureDetails.First().province_id;
						//   modelMaster.latitude = structureDetails.First().latitude;
						//   modelMaster.longitude = structureDetails.First().longitude;
						//   modelMaster.parent_network_id = structureDetails.First().network_id;
						//}
					}
					objONTMaster.network_id = objISPNetworkCode.network_code;
					objONTMaster.sequence_id = objISPNetworkCode.sequence_id;

				}
				this.Validate(objONTMaster);
				if (ModelState.IsValid)
				{
					bool isNew = objONTMaster.system_id == 0 ? true : false;
					if (objONTMaster.unitValue != null && objONTMaster.unitValue.Contains(":"))
					{
						objONTMaster.no_of_input_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[0]);
						objONTMaster.no_of_output_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[1]);
					}
					var resultItem = new BLONT().SaveONTEntity(objONTMaster, objONTMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.ONT.ToString() };
						if (isNew)
						{

							objONTMaster.objPM.status = ResponseStatus.OK.ToString(); ;
							objONTMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							objONTMaster.objPM.systemId = resultItem.system_id;
							objONTMaster.objPM.entityType = EntityType.ONT.ToString();
							objONTMaster.objPM.NetworkId = resultItem.network_id;
							objONTMaster.objPM.structureId = objONTMaster.objIspEntityMap.structure_id;
							objONTMaster.objPM.shaftId = objONTMaster.objIspEntityMap.shaft_id ?? 0;
							objONTMaster.objPM.floorId = objONTMaster.objIspEntityMap.floor_id ?? 0;
							objONTMaster.objPM.pSystemId = objONTMaster.parent_system_id;
							objONTMaster.objPM.pEntityType = objONTMaster.parent_entity_type;
							response.status = ResponseStatus.OK.ToString(); ;
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);

						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objONTMaster.objPM.status = ResponseStatus.OK.ToString();
								objONTMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString(); ;
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								objONTMaster.objPM.status = ResponseStatus.OK.ToString();
								objONTMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "ONT updated successfully.";
								response.status = ResponseStatus.OK.ToString(); ;
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "ONT updated successfully.";
							}
						}
					}
					else
					{
						objONTMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objONTMaster.objPM.message = objONTMaster.objPM.message;
						response.error_message = objONTMaster.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{

					objONTMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objONTMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objONTMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objONTMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objONTMaster, EntityType.ONT.ToString());
					BindONTDropDown(objONTMaster);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objONTMaster);
					new BLMisc().BindPortDetails(objONTMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
					//Get the layer details to bind additional attributes ONT
					var layerdetails = new BLLayer().getLayer(EntityType.ONT.ToString());
					objONTMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes ONT
					response.results = objONTMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPONT()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion


		#region ISP OpticalRepeater

		#region ISP OpticalRepeater Details
		/// <summary> Get OpticalRepeater Details</summary>
		/// <param name="data">objOpticalRepeater,ModelInfo</param>
		/// <returns>OpticalRepeater Details</returns>
		/// <CreatedBy>Tapish</CreatedBy>
		public OpticalRepeaterInfo GetISPOpticalRepeaterDetail(OpticalRepeaterInfo objOpticalRepeater)
		{
			if (objOpticalRepeater.system_id != 0)
			{
				objOpticalRepeater = new BLMisc().GetEntityDetailById<OpticalRepeaterInfo>(objOpticalRepeater.system_id, EntityType.OpticalRepeater, objOpticalRepeater.user_id);
				objOpticalRepeater.geom = objOpticalRepeater.longitude + " " + objOpticalRepeater.latitude;
				//for additional-attributes
				objOpticalRepeater.other_info = new BLISP().GetOtherInfoOpticalRepeater(objOpticalRepeater.system_id);
				fillRegionProvAbbr(objOpticalRepeater);
			}
			else
			{
				if (objOpticalRepeater.networkIdType == NetworkIdType.M.ToString())
				{
					// for Manual network id type 
					var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objOpticalRepeater.parent_system_id, parent_eType = objOpticalRepeater.parent_entity_type, eType = EntityType.OpticalRepeater.ToString(), structureId = objOpticalRepeater.objIspEntityMap.structure_id });
					objOpticalRepeater.network_id = ISPNetworkCodeDetail.network_code;
				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objOpticalRepeater.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objOpticalRepeater, structureDetails, GeometryType.Point.ToString());
				objOpticalRepeater.ownership_type = "Own";
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<OpticalRepeaterTemplateMaster>(objOpticalRepeater.user_id, EntityType.OpticalRepeater);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objOpticalRepeater);
				objOpticalRepeater.other_info = null;   //for additional-attributes                
			}
			return objOpticalRepeater;
		}

		#endregion

		#region Add ISP OpticalRepeater
		/// <summary> Add ISP OpticalRepeater </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>OpticalRepeater Details</returns>
		/// <CreatedBy>Tapish</CreatedBy>

		public ApiResponse<OpticalRepeaterInfo> AddISPOpticalRepeater(ReqInput data)
		{
			var response = new ApiResponse<OpticalRepeaterInfo>();
			try
			{
				OpticalRepeaterInfo objRequestIn = ReqHelper.GetRequestData<OpticalRepeaterInfo>(data);
				OpticalRepeaterInfo model = GetISPOpticalRepeaterDetail(objRequestIn);
				if (objRequestIn.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objRequestIn.system_id, EntityType.OpticalRepeater.ToString());
					model.objIspEntityMap.floor_id = ispMapping.floor_id;
					model.objIspEntityMap.structure_id = ispMapping.structure_id;
					model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				}

				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.OpticalRepeater.ToString());
				new BLMisc().BindPortDetails(model, EntityType.OpticalRepeater.ToString(), DropDownType.OpticalRepeater_Port_Ratio.ToString());
				BindISPOpticalRepeaterDropDown(model);
				fillProjectSpecifications(model);
				//Get the layer details to bind additional attributes OpticalRepeater
				var layerdetails = new BLLayer().getLayer(EntityType.OpticalRepeater.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes OpticalRepeater
				response.status = StatusCodes.OK.ToString();
				response.results = model;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPOpticalRepeater()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP OpticalRepeater Dropdown
		/// <summary> Bind ISP OpticalRepeater Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP OpticalRepeater Details</returns>
		/// <CreatedBy>Tapish</CreatedBy>
		private void BindISPOpticalRepeaterDropDown(OpticalRepeaterInfo objBDB)
		{
			var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.parent_system_id);
			//objBDB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objBDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objOpticalDDL = new BLMisc().GetDropDownList(EntityType.OpticalRepeater.ToString());
			objBDB.lstAmplifierType = objOpticalDDL.Where(x => x.dropdown_type == DropDownType.Amplifier_Type.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objBDB.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objBDB.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save ISP OpticalRepeater
		/// <summary> SaveISPOpticalRepeater </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Tapish</CreatedBy>
		public ApiResponse<OpticalRepeaterInfo> SaveISPOpticalRepeater(ReqInput data)
		{
			var response = new ApiResponse<OpticalRepeaterInfo>();
			OpticalRepeaterInfo model = ReqHelper.GetRequestData<OpticalRepeaterInfo>(data);
			try
			{
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.pSystemId, parent_eType = model.pEntityType, parent_networkId = model.pNetworkId, eType = EntityType.OpticalRepeater.ToString(), structureId = model.structure_id });
					if (model.isDirectSave == true)
					{
						model = GetISPOpticalRepeaterDetail(model);
						model.opticalrepeater_name = objISPNetworkCode.network_code;
						model.parent_system_id = model.pSystemId;
						model.parent_entity_type = model.pEntityType;
						model.parent_network_id = model.pNetworkId;
						model.opticalrepeater_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						//  model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;

				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure);
				if (structureDetails != null)
				{
					model.region_id = structureDetails.region_id;
					model.province_id = structureDetails.province_id;
					model.latitude = structureDetails.latitude;
					model.longitude = structureDetails.longitude;
				}

				this.Validate(model);
				if (ModelState.IsValid)
				{
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.OpticalRepeater.ToString().ToUpper()).FirstOrDefault().layer_title;
					bool isNew = model.system_id == 0 ? true : false;
					if (model.unitValue != null && model.unitValue.Contains(":"))
					{
						model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
						model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
					}
					var result = new BLISP().SaveOpticalRepeaterDetails(model, model.user_id);
					if (string.IsNullOrEmpty(result.objPM.message))
					{

						if (isNew)
						{
							string[] LayerName = { EntityType.OpticalRepeater.ToString() };
							model.objPM.status = ResponseStatus.OK.ToString();
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "HTB saved successfully.";
							model.objPM.systemId = result.system_id;
							model.objPM.entityType = EntityType.OpticalRepeater.ToString();
							model.objPM.NetworkId = result.network_id;
							model.objPM.structureId = model.objIspEntityMap.structure_id;
							model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
							model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
							model.objPM.pSystemId = model.parent_system_id;
							model.objPM.pEntityType = model.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "HTB saved successfully.";
						}
						else
						{
							if (result.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
							}
							else
							{
								string[] LayerName = { EntityType.OpticalRepeater.ToString() };
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						model.objPM.status = ResponseStatus.FAILED.ToString();
						model.objPM.message = model.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = model.objPM.message;
					}
				}
				else
				{
					model.objPM.status = ResponseStatus.FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.OpticalRepeater.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPOpticalRepeaterDropDown(model);
					fillProjectSpecifications(model);
					//Get the layer details to bind additional attributes OpticalRepeater
					var layerdetails = new BLLayer().getLayer(EntityType.OpticalRepeater.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes OpticalRepeater
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPOpticalRepeater()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion


		#region ISP Htb

		#region ISP Htb Details
		/// <summary> Get Htb Details</summary>
		/// <param name="data">objHTB,ModelInfo</param>
		/// <returns>Htb Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public HTBInfo GetISPHtbDetail(HTBInfo objHTB)
		{
			if (objHTB.system_id != 0)
			{
				objHTB = new BLMisc().GetEntityDetailById<HTBInfo>(objHTB.system_id, EntityType.HTB, objHTB.user_id);
				objHTB.geom = objHTB.longitude + " " + objHTB.latitude;
				//for additional-attributes
				objHTB.other_info = new BLISP().GetOtherInfoHTB(objHTB.system_id);
				fillRegionProvAbbr(objHTB);
			}
			else
			{
				if (objHTB.networkIdType == NetworkIdType.M.ToString())
				{
					// for Manual network id type 
					var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objHTB.parent_system_id, parent_eType = objHTB.parent_entity_type, eType = EntityType.HTB.ToString(), structureId = objHTB.objIspEntityMap.structure_id });
					objHTB.network_id = ISPNetworkCodeDetail.network_code;
				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objHTB.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objHTB, structureDetails, GeometryType.Point.ToString());
				objHTB.ownership_type = "Own";
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<HTBTemplateMaster>(objHTB.user_id, EntityType.HTB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objHTB);
				objHTB.other_info = null;   //for additional-attributes
			}
			return objHTB;
		}

		#endregion

		#region Add ISP Htb
		/// <summary> Add ISP Htb </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Htb Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<HTBInfo> AddISPHtb(ReqInput data)
		{
			var response = new ApiResponse<HTBInfo>();
			try
			{
				HTBInfo objRequestIn = ReqHelper.GetRequestData<HTBInfo>(data);
				HTBInfo model = GetISPHtbDetail(objRequestIn);
				if (objRequestIn.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objRequestIn.system_id, EntityType.HTB.ToString());
					model.objIspEntityMap.floor_id = ispMapping.floor_id;
					model.objIspEntityMap.structure_id = ispMapping.structure_id;
					model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				}

				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
				new BLMisc().BindPortDetails(model, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
				BindISPHTBDropDown(model);
				fillProjectSpecifications(model);
				//Get the layer details to bind additional attributes HTB
				var layerdetails = new BLLayer().getLayer(EntityType.HTB.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes HTB
				response.status = StatusCodes.OK.ToString();
				response.results = model;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPHtb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP Htb Dropdown
		/// <summary> Bind ISP Htb Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP Htb Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindISPHTBDropDown(HTBInfo objBDB)
		{
			var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.parent_system_id);
			//objBDB.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objBDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList("");
			objBDB.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objBDB.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save ISP Htb
		/// <summary> SaveISPHtb </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<HTBInfo> SaveISPHtb(ReqInput data)
		{
			var response = new ApiResponse<HTBInfo>();
			HTBInfo model = ReqHelper.GetRequestData<HTBInfo>(data);
			try
			{
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.HTB.ToString(), structureId = model.structure_id });
					if (model.isDirectSave == true)
					{
						model = GetISPHtbDetail(model);
						model.htb_name = objISPNetworkCode.network_code;
						model.parent_system_id = model.pSystemId;
						model.parent_entity_type = model.pEntityType;
						model.parent_network_id = model.pNetworkId;
						model.htb_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						// model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;

				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure);
				if (structureDetails != null)
				{
					model.region_id = structureDetails.region_id;
					model.province_id = structureDetails.province_id;
					model.region_name = structureDetails.region_name;
					model.province_name = structureDetails.province_name;
					model.latitude = structureDetails.latitude;
					model.longitude = structureDetails.longitude;
					model.geom = structureDetails.longitude + " " + structureDetails.latitude;
				}

				this.Validate(model);
				if (ModelState.IsValid)
				{
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.HTB.ToString().ToUpper()).FirstOrDefault().layer_title;
					bool isNew = model.system_id == 0 ? true : false;
					if (model.unitValue != null && model.unitValue.Contains(":"))
					{
						model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
						model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
					}
					var result = new BLISP().SaveHTBDetails(model, model.user_id);
					if (string.IsNullOrEmpty(result.objPM.message))
					{

						if (isNew)
						{
							string[] LayerName = { EntityType.HTB.ToString() };
							model.objPM.status = ResponseStatus.OK.ToString();
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "HTB saved successfully.";
							model.objPM.systemId = result.system_id;
							model.objPM.entityType = EntityType.HTB.ToString();
							model.objPM.NetworkId = result.network_id;
							model.objPM.structureId = model.objIspEntityMap.structure_id;
							model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
							model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
							model.objPM.pSystemId = model.parent_system_id;
							model.objPM.pEntityType = model.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "HTB saved successfully.";
						}
						else
						{
							if (result.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(result.message);//result.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(model.system_id, EntityType.HTB.ToString(), model.network_id, model.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude }, model.user_id);
								string[] LayerName = { layer_title };
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						model.objPM.status = ResponseStatus.FAILED.ToString();
						model.objPM.message = model.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = model.objPM.message;
					}
				}
				else
				{
					model.objPM.status = ResponseStatus.FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.HTB.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPHTBDropDown(model);
					fillProjectSpecifications(model);
					//Get the layer details to bind additional attributes HTB
					var layerdetails = new BLLayer().getLayer(EntityType.HTB.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes HTB
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPHtb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion

		#region ISP Fdb

		#region ISP Fdb Details
		/// <summary> Get Fdb Details</summary>
		/// <param name="data">objFDB,ModelInfo</param>
		/// <returns>ISP Fdb Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public FDBInfo GetISPFDBDetail(FDBInfo objFDB)
		{
			objFDB.parent_entity_type = EntityType.Structure.ToString();
			objFDB.parent_system_id = objFDB.structure_id;
			if (objFDB.system_id != 0)
			{
				objFDB = new BLMisc().GetEntityDetailById<FDBInfo>(objFDB.system_id, EntityType.FDB, objFDB.user_id);
				objFDB.geom = objFDB.longitude + " " + objFDB.latitude;
				//for additional-attributes
				objFDB.other_info = new BLISP().GetOtherInfoFDB(objFDB.system_id);
				fillRegionProvAbbr(objFDB);
			}
			else
			{
				if (objFDB.networkIdType == NetworkIdType.M.ToString())
				{
					// for Manual network id type 
					var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objFDB.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = objFDB.structure_id });
					objFDB.network_id = ISPNetworkCodeDetail.network_code;
				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objFDB.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objFDB, structureDetails, GeometryType.Point.ToString());
				objFDB.ownership_type = "Own";
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplateMaster>(objFDB.user_id, EntityType.FDB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objFDB);
				objFDB.other_info = null;   //for additional-attributes

			}
			BLItemTemplate.Instance.BindItemDropdowns(objFDB, EntityType.FDB.ToString());
			fillProjectSpecifications(objFDB);
			return objFDB;
		}

		#endregion

		#region Add ISP Fdb
		/// <summary> Add ISP Fdb </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Fdb Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<FDBInfo> AddISPFdb(ReqInput data)
		{
			var response = new ApiResponse<FDBInfo>();
			try
			{
				FDBInfo objRequestIn = ReqHelper.GetRequestData<FDBInfo>(data);

				FDBInfo objFDB = GetISPFDBDetail(objRequestIn);
				if (objRequestIn.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objRequestIn.system_id, EntityType.FDB.ToString());
					objFDB.objIspEntityMap.floor_id = ispMapping.floor_id;
					objFDB.objIspEntityMap.structure_id = ispMapping.structure_id;
					objFDB.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				}
				else
				{
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objRequestIn.structure_id, EntityType.Structure);
					objFDB.geom = structureDetails.longitude + " " + structureDetails.latitude;
				}
				new BLMisc().BindPortDetails(objFDB, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
				BindISPFDBDropdown(objFDB);
				//Get the layer details to bind additional attributes FDB
				var layerdetails = new BLLayer().getLayer(EntityType.FDB.ToString());
				objFDB.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes FDB
				response.status = StatusCodes.OK.ToString();
				response.results = objFDB;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPFdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP FDB Dropdown
		/// <summary> Bind ISP FDB Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP FDB Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindISPFDBDropdown(FDBInfo objFDB)
		{
			objFDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objFDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objDDL = new BLMisc().GetDropDownList("");
			objFDB.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objFDB.lstServedByRing = objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save ISP Fdb
		/// <summary> SaveISPFdb </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<FDBInfo> SaveISPFdb(ReqInput data)
		{
			var response = new ApiResponse<FDBInfo>();
			FDBInfo model = ReqHelper.GetRequestData<FDBInfo>(data);
			try
			{
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.objIspEntityMap.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.FDB.ToString(), structureId = model.objIspEntityMap.structure_id });
					if (model.isDirectSave == true)
					{
						model = GetISPFDBDetail(model);
						model.fdb_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//    var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						//  model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;

				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure);
				if (structureDetails != null)
				{
					model.region_id = structureDetails.region_id;
					model.province_id = structureDetails.province_id;
					model.region_name = structureDetails.region_name;
					model.province_name = structureDetails.province_name;
					model.latitude = structureDetails.latitude;
					model.longitude = structureDetails.longitude;
				}
				this.Validate(model);
				if (ModelState.IsValid)
				{
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;

					bool isNew = model.system_id == 0 ? true : false;
					if (model.unitValue != null && model.unitValue.Contains(":"))
					{
						model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
						model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
					}
					model.userId = model.user_id;
					var resultItem = new BLISP().SaveFDBDetails(model);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (model.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(model.lstSpliceTrayInfo, resultItem.system_id, model.user_id, EntityType.SpliceTray.ToString(), EntityType.FDB.ToString());
						}

						//Save Reference
						if (model.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(model.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							string[] LayerName = { EntityType.FDB.ToString() };
							model.objPM.status = ResponseStatus.OK.ToString(); ;
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							model.objPM.systemId = resultItem.system_id;
							model.objPM.entityType = EntityType.FDB.ToString();
							model.objPM.NetworkId = resultItem.network_id;
							model.objPM.structureId = model.objIspEntityMap.structure_id;
							model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
							model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
							model.objPM.pSystemId = model.parent_system_id;
							model.objPM.pEntityType = model.parent_entity_type;
							response.status = ResponseStatus.OK.ToString(); ;
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = ResponseStatus.OK.ToString(); ;
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(model.system_id, EntityType.FDB.ToString(), model.network_id, model.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude }, model.user_id);
								string[] LayerName = { EntityType.FDB.ToString() };
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString(); ;
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						model.objPM.status = ResponseStatus.FAILED.ToString();
						model.objPM.message = model.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = model.objPM.message;
					}
				}
				else
				{
					model.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.FDB.ToString());
					BindISPFDBDropdown(model);
					fillProjectSpecifications(model);
					new BLMisc().BindPortDetails(model, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
					//Get the layer details to bind additional attributes FDB
					var layerdetails = new BLLayer().getLayer(EntityType.FDB.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes FDB
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPFdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion

		#region ISP Bdb

		#region ISP Bdb Details
		/// <summary> Get Bdb Details</summary>
		/// <param name="data">objPOD,ModelInfo</param>
		/// <returns>ISP Bdb Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public BDBMaster GetISPBdbDetail(BDBMaster objBDB)
		{
			objBDB.parent_entity_type = EntityType.Structure.ToString();
			objBDB.parent_system_id = objBDB.structure_id;
			if (objBDB.system_id != 0)
			{
				objBDB = new BLMisc().GetEntityDetailById<BDBMaster>(objBDB.system_id, EntityType.BDB, objBDB.user_id);
				objBDB.geom = objBDB.longitude + " " + objBDB.latitude;
				//for additional-attributes
				objBDB.other_info = new BLBDB().GetOtherInfoBDB(objBDB.system_id);
				fillRegionProvAbbr(objBDB);
			}
			else
			{
				if (objBDB.networkIdType == NetworkIdType.M.ToString())
				{
					// for Manual network id type 
					var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objBDB.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.BDB.ToString(), structureId = objBDB.structure_id });
					objBDB.network_id = ISPNetworkCodeDetail.network_code;
				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objBDB.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objBDB, structureDetails, GeometryType.Point.ToString());
				objBDB.ownership_type = "Own";
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<BDBTemplateMaster>(objBDB.user_id, EntityType.BDB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objBDB);
				objBDB.address = BLStructure.Instance.getBuildingAddress(objBDB.structure_id);
				objBDB.other_info = null; //for additional-attributes
			}
			return objBDB;
		}

		#endregion

		#region Add ISP Bdb
		/// <summary> Add ISP Bdb </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>Wallmount Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<BDBMaster> AddISPBdb(ReqInput data)
		{
			var response = new ApiResponse<BDBMaster>();
			try
			{
				BDBMaster objRequestIn = ReqHelper.GetRequestData<BDBMaster>(data);
				ElementInfo objElementReqIn = ReqHelper.GetRequestData<ElementInfo>(data);
				BDBMaster model = GetISPBdbDetail(objRequestIn);
				if (objRequestIn.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objRequestIn.system_id, EntityType.BDB.ToString());
					model.objIspEntityMap.floor_id = ispMapping.floor_id;
					model.parent_system_id = ispMapping.structure_id;
					model.objIspEntityMap.structure_id = ispMapping.structure_id;
					model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				}
				else
				{
					model.objIspEntityMap.floor_id = objRequestIn.objIspEntityMap.floor_id;
					model.parent_system_id = objRequestIn.system_id;
					model.objIspEntityMap.structure_id = objRequestIn.structure_id;
					model.objIspEntityMap.shaft_id = objRequestIn.objIspEntityMap.shaft_id;
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objRequestIn.structure_id, EntityType.Structure);
					if (structureDetails != null)
					{
						model.region_id = structureDetails.region_id;
						model.province_id = structureDetails.province_id;
						model.region_name = structureDetails.region_name;
						model.province_name = structureDetails.province_name;
						model.latitude = structureDetails.latitude;
						model.longitude = structureDetails.longitude;
						model.parent_network_id = structureDetails.network_id;
						model.geom = structureDetails.longitude + " " + structureDetails.latitude;
					}
				}
				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.BDB.ToString());
				BindISPBDBDropDown(model);
				fillProjectSpecifications(model);
				new BLMisc().BindPortDetails(model, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
				//Get the layer details to bind additional attributes BDB
				var layerdetails = new BLLayer().getLayer(EntityType.BDB.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes BDB
				response.status = StatusCodes.OK.ToString();
				response.results = model;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPBdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP Bdb Dropdown
		/// <summary> Bind ISP Bdb Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP Bdb Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindISPBDBDropDown(BDBMaster objBDB)
		{
			var objDDL = new BLBDB().GetShaftFloorByStrucId(objBDB.parent_system_id);
			var objTypDDL = new BLMisc().GetDropDownList(EntityType.BDB.ToString(), DropDownType.Entity_Type.ToString());
			objBDB.lstEntityType = objTypDDL;
			objBDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objBDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objBDB.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();

		}
		#endregion

		#region Save ISP Bdb
		/// <summary> SaveISPBdb </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<BDBMaster> SaveISPBdb(ReqInput data)
		{
			var response = new ApiResponse<BDBMaster>();
			BDBMaster model = ReqHelper.GetRequestData<BDBMaster>(data);
			try
			{
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.BDB.ToString(), structureId = model.structure_id });
					if (model.isDirectSave == true)
					{
						model = GetISPBdbDetail(model);
						model.bdb_name = objISPNetworkCode.network_code;
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							model.region_id = structureDetails.region_id;
							model.province_id = structureDetails.province_id;
							model.region_name = structureDetails.region_name;
							model.province_name = structureDetails.province_name;
							model.latitude = structureDetails.latitude;
							model.longitude = structureDetails.longitude;
							model.parent_network_id = structureDetails.network_id;
						}
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//   var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						// model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;
				}

				if (model.unitValue != null && model.unitValue.Contains(":"))
				{
					model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
					model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
				}
				this.Validate(model);
				if (ModelState.IsValid)
				{

					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault().layer_title;
					bool isNew = model.system_id == 0 ? true : false;
					var resultItem = new BLBDB().SaveEntityBDB(model, model.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (model.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(model.lstSpliceTrayInfo, resultItem.system_id, model.user_id, EntityType.SpliceTray.ToString(), EntityType.BDB.ToString());
						}
						//Save Reference
						if (model.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(model.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							string[] LayerName = { EntityType.BDB.ToString() };
							model.objPM.status = ResponseStatus.OK.ToString();
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							model.objPM.systemId = resultItem.system_id;
							model.objPM.entityType = EntityType.BDB.ToString();
							model.objPM.NetworkId = resultItem.network_id;
							model.objPM.structureId = model.objIspEntityMap.structure_id;
							model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
							model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
							model.objPM.pSystemId = model.parent_system_id;
							model.objPM.pEntityType = model.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(model.system_id, EntityType.BDB.ToString(), model.network_id, model.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude }, model.user_id);
								string[] LayerName = { EntityType.BDB.ToString() };
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						model.objPM.status = ResponseStatus.FAILED.ToString();
						model.objPM.message = model.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = model.objPM.message;
					}

				}
				else
				{
					model.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.BDB.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPBDBDropDown(model);
					fillProjectSpecifications(model);
					//Get the layer details to bind additional attributes BDB
					var layerdetails = new BLLayer().getLayer(EntityType.BDB.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes BDB
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPBdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}


		#endregion
		#endregion

		#region ISP SpliceClosure

		#region ISP SpliceClosure Details
		/// <summary> Get SpliceClosure Details</summary>
		/// <param name="data">objPOD,ModelInfo</param>
		/// <returns>ISP SpliceClosure Details</returns>
		/// <CreatedBy>Rahul Tyagi</CreatedBy>
		public SCMaster GetISPSpliceClosureDetail(SCMaster objSC)
		{
			objSC.parent_entity_type = EntityType.Structure.ToString();
			objSC.parent_system_id = objSC.structure_id;
			if (objSC.system_id != 0)
			{
				objSC = new BLMisc().GetEntityDetailById<SCMaster>(objSC.system_id, EntityType.SpliceClosure, objSC.user_id);
				objSC.geom = objSC.longitude + " " + objSC.latitude;
				objSC.no_of_port = objSC.no_of_ports;
				//for additional-attributes
				objSC.other_info = new BLSC().GetOtherInfoSpliceClosure(objSC.system_id);
				fillRegionProvAbbr(objSC);
			}
			else
			{
				if (objSC.isConvert)
				{
					var objItem = new BLVendorSpecification().getEntityTemplatebyPortNo(objSC.no_of_ports, EntityType.SpliceClosure.ToString(), objSC.vendor_id);
					objSC.vendor_id = objItem.vendor_id;
					objSC.specification = objItem.specification;
					objSC.subcategory1 = objItem.subcategory_1;
					objSC.subcategory2 = objItem.subcategory_2;
					objSC.subcategory3 = objItem.subcategory_3;
					objSC.no_of_port = objItem.no_of_port;
					objSC.category = objItem.category_reference;
					objSC.item_code = objItem.code;
					//isp_entity_mapping details..
					var IspMapDetails = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(objSC.cdb_system_id, EntityType.CDB.ToString());
					objSC.objIspEntityMap.structure_id = IspMapDetails.structure_id;
					objSC.objIspEntityMap.floor_id = IspMapDetails.floor_id;
					objSC.objIspEntityMap.shaft_id = IspMapDetails.shaft_id;
					objSC.pEntityType = objSC.parent_entity_type;
					objSC.pSystemId = objSC.parent_system_id;
				}
				if (objSC.networkIdType == NetworkIdType.M.ToString())
				{
					// for Manual network id type 
					var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objSC.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.SpliceClosure.ToString(), structureId = objSC.structure_id });
					objSC.network_id = ISPNetworkCodeDetail.network_code;
				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objSC.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objSC, structureDetails, GeometryType.Point.ToString());
				if (!objSC.isConvert)
				{
					var obj_Item = BLItemTemplate.Instance.GetTemplateDetail<SCTemplateMaster>(objSC.user_id, EntityType.SpliceClosure);
					Utility.MiscHelper.CopyMatchingProperties(obj_Item, objSC);
					objSC.no_of_port = obj_Item.no_of_ports;

				}
				objSC.ownership_type = "Own";
				objSC.address = BLStructure.Instance.getBuildingAddress(objSC.structure_id);
				objSC.other_info = null;    //for additional-attributes
			}
			return objSC;
		}

		#endregion

		#region Add ISP SpliceClosure
		/// <summary> Add ISP SpliceClosure</summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns></returns>
		/// <CreatedBy>Rahul Tyagi</CreatedBy>

		public ApiResponse<SCMaster> AddISPSpliceClosure(ReqInput data)
		{
			var response = new ApiResponse<SCMaster>();
			try
			{
				SCMaster objRequestIn = ReqHelper.GetRequestData<SCMaster>(data);
				ElementInfo objElementReqIn = ReqHelper.GetRequestData<ElementInfo>(data);
				SCMaster model = GetISPSpliceClosureDetail(objRequestIn);
				if (objRequestIn.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objRequestIn.system_id, EntityType.SpliceClosure.ToString());
					model.objIspEntityMap.floor_id = ispMapping.floor_id;
					model.parent_system_id = ispMapping.structure_id;
					model.objIspEntityMap.structure_id = ispMapping.structure_id;
					model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objRequestIn.structure_id, EntityType.Structure);
					if (structureDetails != null)
					{
						model.region_id = structureDetails.region_id;
						model.province_id = structureDetails.province_id;
						model.region_name = structureDetails.region_name;
						model.province_name = structureDetails.province_name;
					}
				}
				else
				{
					model.objIspEntityMap.floor_id = objRequestIn.objIspEntityMap.floor_id;
					model.parent_system_id = objRequestIn.system_id;
					model.objIspEntityMap.structure_id = objRequestIn.structure_id;
					model.objIspEntityMap.shaft_id = objRequestIn.objIspEntityMap.shaft_id;
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objRequestIn.structure_id, EntityType.Structure);
					if (structureDetails != null)
					{
						model.region_id = structureDetails.region_id;
						model.province_id = structureDetails.province_id;
						model.region_name = structureDetails.region_name;
						model.province_name = structureDetails.province_name;
						model.latitude = structureDetails.latitude;
						model.longitude = structureDetails.longitude;
						model.parent_network_id = structureDetails.network_id;
						model.geom = structureDetails.longitude + " " + structureDetails.latitude;
					}
				}
				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.SpliceClosure.ToString());
				BindISPSCDropDown(model);
				fillProjectSpecifications(model);
				//Get the layer details to bind additional attributes SpliceClosure
				var layerdetails = new BLLayer().getLayer(EntityType.SpliceClosure.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes SpliceClosure
				new BLMisc().BindPortDetails(model, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
				response.status = StatusCodes.OK.ToString();
				response.results = model;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPSC()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP SC Dropdown
		/// <summary> Bind ISP SC Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP SC Details</returns>
		/// <CreatedBy>Rahul Tyagi</CreatedBy>
		private void BindISPSCDropDown(SCMaster objSC)
		{
			var objDDL = new BLBDB().GetShaftFloorByStrucId(objSC.parent_system_id);
			var objTypDDL = new BLMisc().GetDropDownList(EntityType.SpliceClosure.ToString(), DropDownType.Entity_Type.ToString());
			objSC.lstEntityType = objTypDDL;
			objSC.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objSC.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objSC.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//   objSC.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();

		}
		#endregion

		#region Save ISP SC
		/// <summary> SaveISPSC </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Rahul Tyagi</CreatedBy>
		public ApiResponse<SCMaster> SaveISPSC(ReqInput data)
		{
			var response = new ApiResponse<SCMaster>();
			SCMaster model = ReqHelper.GetRequestData<SCMaster>(data);
			try
			{
				ModelState.Clear();
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.SpliceClosure.ToString(), structureId = model.structure_id });
					if (model.isDirectSave == true)
					{
						model = GetISPSpliceClosureDetail(model);
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							model.region_id = structureDetails.region_id;
							model.province_id = structureDetails.province_id;
							model.region_name = structureDetails.region_name;
							model.province_name = structureDetails.province_name;
							model.latitude = structureDetails.latitude;
							model.longitude = structureDetails.longitude;
							model.parent_network_id = structureDetails.network_id;
							model.pNetworkId = model.parent_network_id;
						}
						model.spliceclosure_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						// model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;

				}

				if (model.unitValue != null && model.unitValue.Contains(":"))
				{
					model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
					model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
				}
				this.Validate(model);
				if (ModelState.IsValid)
				{

					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.SpliceClosure.ToString().ToUpper()).FirstOrDefault().layer_title;
					bool isNew = model.system_id == 0 ? true : false;
					var resultItem = new BLSC().SaveEntitySC(model, model.user_id);
					if (resultItem.isConvert && string.IsNullOrEmpty(resultItem.objPM.message) && isNew)
					{
						string[] LayerName = { EntityType.CDB.ToString(), EntityType.SpliceClosure.ToString() };
						CDBMaster objCDB = new CDBMaster();
						objCDB = new BLMisc().GetEntityDetailById<CDBMaster>(model.cdb_system_id, EntityType.CDB);
						//====  VALIDATE CONNECTION AND SPLICING===// 
						var result = new BLMisc().EntityConversion(EntityType.CDB.ToString(), objCDB.network_id, objCDB.system_id, EntityType.SpliceClosure.ToString(), resultItem.network_id, resultItem.system_id, objCDB.geom, model.user_id);
						//if (response.status=="OK")
						//{
						model.objPM.status = ResponseStatus.OK.ToString();
						model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CDB_NET_FRM_007, ApplicationSettings.listLayerDetails, LayerName);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CDB_NET_FRM_007, ApplicationSettings.listLayerDetails, LayerName);
						if (model.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(model.EntityReference, resultItem.system_id);
						}
						// }
					}
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						if (model.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(model.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							string[] LayerName = { EntityType.SpliceClosure.ToString() };
							model.objPM.status = ResponseStatus.OK.ToString();
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							model.objPM.systemId = resultItem.system_id;
							model.objPM.entityType = EntityType.SpliceClosure.ToString();
							model.objPM.NetworkId = resultItem.network_id;
							model.objPM.structureId = model.objIspEntityMap.structure_id;
							model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
							model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
							model.objPM.pSystemId = model.parent_system_id;
							model.objPM.pEntityType = model.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(model.system_id, EntityType.SpliceClosure.ToString(), model.network_id, model.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude }, model.user_id);
								string[] LayerName = { EntityType.SpliceClosure.ToString() };
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}

				}
				else
				{
					model.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.SpliceClosure.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPSCDropDown(model);
					fillProjectSpecifications(model);
					//Get the layer details to bind additional attributes SpliceClosure
					var layerdetails = new BLLayer().getLayer(EntityType.SpliceClosure.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes SpliceClosure
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPSC()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}


		#endregion
		#endregion

		#region ISP Adb

		#region ISP Adb Details
		/// <summary> Get Adb Details</summary>
		/// <param name="data">objPOD,ModelInfo</param>
		/// <returns>ISP Adb Details</returns>
		/// <CreatedBy></CreatedBy>
		public ADBMaster GetISPAdbDetail(ADBMaster objADB)
		{
			objADB.parent_entity_type = EntityType.Structure.ToString();
			objADB.parent_system_id = objADB.structure_id;
			if (objADB.system_id != 0)
			{
				objADB = new BLMisc().GetEntityDetailById<ADBMaster>(objADB.system_id, EntityType.ADB, objADB.user_id);
				objADB.geom = objADB.longitude + " " + objADB.latitude;
				//for additional-attributes
				objADB.other_info = new BLADB().GetOtherInfoADB(objADB.system_id);
				fillRegionProvAbbr(objADB);
			}
			else
			{
				if (objADB.networkIdType == NetworkIdType.M.ToString())
				{
					// for Manual network id type 
					var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = objADB.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.ADB.ToString(), structureId = objADB.structure_id });
					objADB.network_id = ISPNetworkCodeDetail.network_code;
				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objADB.objIspEntityMap.structure_id, EntityType.Structure);
				fillISPRegionProvinceDetail(objADB, structureDetails, GeometryType.Point.ToString());
				objADB.ownership_type = "Own";
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<ADBTemplateMaster>(objADB.user_id, EntityType.ADB);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objADB);
				objADB.address = BLStructure.Instance.getBuildingAddress(objADB.structure_id);
				objADB.other_info = null; //for additional-attributes
			}
			return objADB;
		}

		#endregion

		#region Add ISP Adb
		/// <summary> Add ISP Adb </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns></returns>
		/// <CreatedBy></CreatedBy>

		public ApiResponse<ADBMaster> AddISPAdb(ReqInput data)
		{
			var response = new ApiResponse<ADBMaster>();
			try
			{
				ADBMaster objRequestIn = ReqHelper.GetRequestData<ADBMaster>(data);
				ElementInfo objElementReqIn = ReqHelper.GetRequestData<ElementInfo>(data);
				ADBMaster model = GetISPAdbDetail(objRequestIn);
				if (objRequestIn.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objRequestIn.system_id, EntityType.ADB.ToString());
					model.objIspEntityMap.floor_id = ispMapping.floor_id;
					model.parent_system_id = ispMapping.structure_id;
					model.objIspEntityMap.structure_id = ispMapping.structure_id;
					model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
				}
				else
				{
					model.objIspEntityMap.floor_id = objRequestIn.objIspEntityMap.floor_id;
					model.parent_system_id = (model.parent_system_id == 0 ? objRequestIn.system_id : model.parent_system_id);
					model.objIspEntityMap.structure_id = objRequestIn.structure_id;
					model.objIspEntityMap.shaft_id = objRequestIn.objIspEntityMap.shaft_id;
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objRequestIn.structure_id, EntityType.Structure);
					if (structureDetails != null)
					{
						model.region_id = structureDetails.region_id;
						model.province_id = structureDetails.province_id;
						model.region_name = structureDetails.region_name;
						model.province_name = structureDetails.province_name;
						model.latitude = structureDetails.latitude;
						model.longitude = structureDetails.longitude;
						model.parent_network_id = structureDetails.network_id;
						model.geom = structureDetails.longitude + " " + structureDetails.latitude;
					}
				}
				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.ADB.ToString());
				BindISPADBDropDown(model);
				fillProjectSpecifications(model);

				//Get the layer details to bind additional attributes ADB
				var layerdetails = new BLLayer().getLayer(EntityType.ADB.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes ADB
				new BLMisc().BindPortDetails(model, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
				response.status = StatusCodes.OK.ToString();
				response.results = model;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPAdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP Adb Dropdown
		/// <summary> Bind ISP Adb Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP Adb Details</returns>
		/// <CreatedBy></CreatedBy>
		private void BindISPADBDropDown(ADBMaster objADB)
		{
			var objDDL = new BLBDB().GetShaftFloorByStrucId(objADB.parent_system_id);
			var objTypDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString(), DropDownType.Entity_Type.ToString());
			objADB.lstEntityType = objTypDDL;
			objADB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objADB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objADB.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();

		}
		#endregion

		#region Save ISP Adb
		/// <summary> SaveISPAdb </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy></CreatedBy>
		public ApiResponse<ADBMaster> SaveISPAdb(ReqInput data)
		{
			var response = new ApiResponse<ADBMaster>();
			ADBMaster model = ReqHelper.GetRequestData<ADBMaster>(data);
			try
			{
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.ADB.ToString(), structureId = model.structure_id });
					if (model.isDirectSave == true)
					{
						model = GetISPAdbDetail(model);
						model.adb_name = objISPNetworkCode.network_code;
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							model.region_id = structureDetails.region_id;
							model.province_id = structureDetails.province_id;
							model.region_name = structureDetails.region_name;
							model.province_name = structureDetails.province_name;
							model.latitude = structureDetails.latitude;
							model.longitude = structureDetails.longitude;
							model.parent_network_id = structureDetails.network_id;
						}
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						//model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;
				}

				if (model.unitValue != null && model.unitValue.Contains(":"))
				{
					model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
					model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
				}
				this.Validate(model);
				if (ModelState.IsValid)
				{

					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.ADB.ToString().ToUpper()).FirstOrDefault().layer_title;
					bool isNew = model.system_id == 0 ? true : false;
					var resultItem = new BLADB().SaveEntityADB(model, model.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (model.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(model.lstSpliceTrayInfo, resultItem.system_id, model.user_id, EntityType.SpliceTray.ToString(), EntityType.ADB.ToString());
						}
						//Save Reference
						if (model.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(model.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							string[] LayerName = { EntityType.ADB.ToString() };
							model.objPM.status = ResponseStatus.OK.ToString();
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							model.objPM.systemId = resultItem.system_id;
							model.objPM.entityType = EntityType.ADB.ToString();
							model.objPM.NetworkId = resultItem.network_id;
							model.objPM.structureId = model.objIspEntityMap.structure_id;
							model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
							model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
							model.objPM.pSystemId = model.parent_system_id;
							model.objPM.pEntityType = model.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(model.system_id, EntityType.ADB.ToString(), model.network_id, model.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude }, model.user_id);
								string[] LayerName = { EntityType.ADB.ToString() };
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						model.objPM.status = ResponseStatus.FAILED.ToString();
						model.objPM.message = model.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = model.objPM.message;
					}

				}
				else
				{
					model.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.ADB.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPADBDropDown(model);
					fillProjectSpecifications(model);

					//Get the layer details to bind additional attributes ADB
					var layerdetails = new BLLayer().getLayer(EntityType.ADB.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes ADB
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPAdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}


		#endregion
		#endregion
		#region ISP Cdb

		#region ISP Cdb Details
		/// <summary> Get Cdb Details</summary>
		/// <param name="data">objPOD,ModelInfo</param>
		/// <returns>ISP Cdb Details</returns>
		/// <CreatedBy></CreatedBy>
		public CDBMaster GetISPCdbDetail(CDBMaster obCDB)
		{
			obCDB.parent_entity_type = EntityType.Structure.ToString();
			obCDB.parent_system_id = obCDB.structure_id;
			if (obCDB.system_id != 0)
			{
				obCDB = new BLMisc().GetEntityDetailById<CDBMaster>(obCDB.system_id, EntityType.CDB, obCDB.user_id);
				obCDB.geom = obCDB.longitude + " " + obCDB.latitude;
				//For additional-attributes
				obCDB.other_info = new BLCDB().GetOtherInfoCDB(obCDB.system_id);
				fillRegionProvAbbr(obCDB);
			}
			else
			{

				if (obCDB.isConvert)
				{
					obCDB.is_servingdb = true;
					var obj_Item = new BLVendorSpecification().getEntityTemplatebyPortNo(obCDB.no_of_ports, EntityType.CDB.ToString(), obCDB.vendor_id);
					obCDB.entity_category = "Primary";
					obCDB.vendor_id = obj_Item.vendor_id;
					obCDB.specification = obj_Item.specification;
					obCDB.subcategory1 = obj_Item.subcategory_1;
					obCDB.subcategory2 = obj_Item.subcategory_2;
					obCDB.subcategory3 = obj_Item.subcategory_3;
					obCDB.no_of_port = obj_Item.no_of_port;
					obCDB.category = obj_Item.category_reference;
					obCDB.item_code = obj_Item.code;
					//isp_entity_mapping details..
					var IspMapDetails = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(obCDB.sc_system_id, EntityType.SpliceClosure.ToString());
					obCDB.objIspEntityMap.structure_id = IspMapDetails.structure_id;
					obCDB.objIspEntityMap.floor_id = IspMapDetails.floor_id;
					obCDB.objIspEntityMap.shaft_id = IspMapDetails.shaft_id;
				}
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(obCDB.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(obCDB, structureDetails, GeometryType.Point.ToString());
				if (obCDB.networkIdType == NetworkIdType.M.ToString())
				{
					// for Manual network id type 
					var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = obCDB.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.CDB.ToString(), structureId = obCDB.structure_id });
					obCDB.network_id = ISPNetworkCodeDetail.network_code;
				}

				if (!obCDB.isConvert)
				{
					var objItem = BLItemTemplate.Instance.GetTemplateDetail<CDBTemplateMaster>(obCDB.user_id, EntityType.CDB);
					Utility.MiscHelper.CopyMatchingProperties(objItem, obCDB);
				}
				obCDB.ownership_type = "Own";
				obCDB.address = BLStructure.Instance.getBuildingAddress(obCDB.structure_id);
				obCDB.other_info = null;//for additional-attributes
			}
			return obCDB;
		}

		#endregion

		#region Add ISP Cdb
		/// <summary> Add ISP Cdb </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns></returns>
		/// <CreatedBy></CreatedBy>

		public ApiResponse<CDBMaster> AddISPCdb(ReqInput data)
		{
			var response = new ApiResponse<CDBMaster>();
			try
			{
				CDBMaster objRequestIn = ReqHelper.GetRequestData<CDBMaster>(data);
				ElementInfo objElementReqIn = ReqHelper.GetRequestData<ElementInfo>(data);
				CDBMaster model = GetISPCdbDetail(objRequestIn);
				if (objRequestIn.system_id != 0)
				{
					var ispMapping = new BLISP().getMappingByEntityId(objRequestIn.system_id, EntityType.CDB.ToString());
					model.objIspEntityMap.floor_id = ispMapping.floor_id;
					model.parent_system_id = ispMapping.structure_id;
					model.objIspEntityMap.structure_id = ispMapping.structure_id;
					model.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objRequestIn.structure_id, EntityType.Structure);
					if (structureDetails != null)
					{
						model.region_id = structureDetails.region_id;
						model.province_id = structureDetails.province_id;
						model.region_name = structureDetails.region_name;
						model.province_name = structureDetails.province_name;
					}
				}
				else
				{
					model.objIspEntityMap.floor_id = objRequestIn.objIspEntityMap.floor_id;
					model.parent_system_id = (model.parent_system_id == 0 ? objRequestIn.system_id : model.parent_system_id);
					model.objIspEntityMap.structure_id = objRequestIn.structure_id;
					model.objIspEntityMap.shaft_id = objRequestIn.objIspEntityMap.shaft_id;
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objRequestIn.structure_id, EntityType.Structure);
					if (structureDetails != null)
					{
						model.region_id = structureDetails.region_id;
						model.province_id = structureDetails.province_id;
						model.region_name = structureDetails.region_name;
						model.province_name = structureDetails.province_name;
						model.latitude = structureDetails.latitude;
						model.longitude = structureDetails.longitude;
						model.parent_network_id = structureDetails.network_id;
						model.geom = structureDetails.longitude + " " + structureDetails.latitude;
					}
				}
				BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.CDB.ToString());
				BindISPCDBDropDown(model);
				fillProjectSpecifications(model);
				new BLMisc().BindPortDetails(model, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
				//Get the layer details to bind additional attributes CDB
				var layerdetails = new BLLayer().getLayer(EntityType.CDB.ToString());
				model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes CDB
				response.status = StatusCodes.OK.ToString();
				response.results = model;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPCdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP Cdb Dropdown
		/// <summary> Bind ISP Cdb Dropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP Cdb Details</returns>
		/// <CreatedBy></CreatedBy>
		private void BindISPCDBDropDown(CDBMaster objCDB)
		{
			var objDDL = new BLBDB().GetShaftFloorByStrucId(objCDB.parent_system_id);
			var objTypDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString(), DropDownType.Entity_Type.ToString());
			objCDB.lstEntityType = objTypDDL;
			objCDB.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objCDB.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objCDB.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//    objCDB.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();

		}
		#endregion

		#region Save ISP Cdb
		/// <summary> SaveISPCdb </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy></CreatedBy>
		public ApiResponse<CDBMaster> SaveISPCdb(ReqInput data)
		{
			var response = new ApiResponse<CDBMaster>();
			CDBMaster model = ReqHelper.GetRequestData<CDBMaster>(data);
			try
			{
				if (model.networkIdType == NetworkIdType.A.ToString() && model.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = model.structure_id, parent_eType = EntityType.Structure.ToString(), eType = EntityType.CDB.ToString(), structureId = model.structure_id });
					if (model.isDirectSave == true)
					{
						model = GetISPCdbDetail(model);
						model.cdb_name = objISPNetworkCode.network_code;
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(model.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							model.region_id = structureDetails.region_id;
							model.province_id = structureDetails.province_id;
							model.region_name = structureDetails.region_name;
							model.province_name = structureDetails.province_name;
							model.latitude = structureDetails.latitude;
							model.longitude = structureDetails.longitude;
							model.parent_network_id = structureDetails.network_id;
						}
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						model.bom_sub_category = objBOMDDL[0].dropdown_value;
						// model.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					model.network_id = objISPNetworkCode.network_code;
					model.sequence_id = objISPNetworkCode.sequence_id;
				}
				if (model.isConvert)
				{
					//isp_entity_mapping details..
					var IspMapDetails = BLIspEntityMapping.Instance.GetStructureFloorbyEntityId(model.sc_system_id, EntityType.SpliceClosure.ToString());
					model.objIspEntityMap.structure_id = IspMapDetails.structure_id;
					model.objIspEntityMap.floor_id = IspMapDetails.floor_id;
					model.objIspEntityMap.shaft_id = IspMapDetails.shaft_id;
				}

				if (model.unitValue != null && model.unitValue.Contains(":"))
				{
					model.no_of_input_port = Convert.ToInt32(model.unitValue.Split(':')[0]);
					model.no_of_output_port = Convert.ToInt32(model.unitValue.Split(':')[1]);
				}
				this.Validate(model);
				if (ModelState.IsValid)
				{

					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.CDB.ToString().ToUpper()).FirstOrDefault().layer_title;
					bool isNew = model.system_id == 0 ? true : false;
					var resultItem = new BLCDB().SaveEntityCDB(model, model.user_id);
					if (resultItem.isConvert && string.IsNullOrEmpty(resultItem.objPM.message) && isNew)
					{
						string[] LayerName = { EntityType.SpliceClosure.ToString(), EntityType.CDB.ToString() };
						SCMaster objSC = new SCMaster();
						objSC = new BLMisc().GetEntityDetailById<SCMaster>(model.sc_system_id, EntityType.SpliceClosure);
						//====  VALIDATE CONNECTION AND SPLICING===// 
						var responseData = new BLMisc().EntityConversion(EntityType.SpliceClosure.ToString(), objSC.network_id, objSC.system_id, resultItem.entityType, resultItem.network_id, resultItem.system_id, objSC.geom, objSC.user_id);
						model.objPM.status = ResponseStatus.OK.ToString();
						model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SC_NET_FRM_021, ApplicationSettings.listLayerDetails, LayerName);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_SC_NET_FRM_021, ApplicationSettings.listLayerDetails, LayerName);
						//Save Reference
						if (model.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(model.EntityReference, resultItem.system_id);
						}

					}
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (model.lstSpliceTrayInfo != null && resultItem.system_id > 0)
						{
							AddSpliceTray(model.lstSpliceTrayInfo, resultItem.system_id, model.user_id, EntityType.SpliceTray.ToString(), EntityType.CDB.ToString());
						}
						//Save Reference
						if (model.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(model.EntityReference, resultItem.system_id);
						}

						if (isNew)
						{
							string[] LayerName = { EntityType.CDB.ToString() };
							model.objPM.status = ResponseStatus.OK.ToString();
							model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							model.objPM.systemId = resultItem.system_id;
							model.objPM.entityType = EntityType.ADB.ToString();
							model.objPM.NetworkId = resultItem.network_id;
							model.objPM.structureId = model.objIspEntityMap.structure_id;
							model.objPM.shaftId = model.objIspEntityMap.shaft_id ?? 0;
							model.objPM.floorId = model.objIspEntityMap.floor_id ?? 0;
							model.objPM.pSystemId = model.parent_system_id;
							model.objPM.pEntityType = model.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(model.system_id, EntityType.CDB.ToString(), model.network_id, model.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = model.longitude + " " + model.latitude }, model.user_id);
								string[] LayerName = { EntityType.CDB.ToString() };
								model.objPM.status = ResponseStatus.OK.ToString();
								model.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
				}
				else
				{
					model.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					model.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (model.isDirectSave == true)
				{
					response.results = model;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(model, EntityType.CDB.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPCDBDropDown(model);
					fillProjectSpecifications(model);
					//Get the layer details to bind additional attributes CDB
					var layerdetails = new BLLayer().getLayer(EntityType.CDB.ToString());
					model.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes CDB
					response.results = model;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPCdb()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}


		#endregion
		#endregion

		#region Competitor
		#region Add Competitor
		/// <summary> Add Competitor </summary>
		/// <returns>Competitor Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<Competitor> AddCompetitor(ReqInput data)
		{
			var response = new ApiResponse<Competitor>();
			try
			{
				Competitor obj = ReqHelper.GetRequestData<Competitor>(data);
				Competitor objCompetitor = GetCompetitorDetail(obj);
				BindCompetitorIcons(objCompetitor);
				//Get the layer details to bind additional attributes Competitor
				var layerdetails = new BLLayer().getLayer(EntityType.Competitor.ToString());
				objCompetitor.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes Competitor
				response.status = StatusCodes.OK.ToString();
				response.results = objCompetitor;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddCompetitor()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get Competitor Details
		/// <summary> GetCompetitorDetail</summary>
		/// <param >networkIdType,geom,userId,SystemId</param>
		/// <returns>Competitor Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public Competitor GetCompetitorDetail(Competitor objCompetitor)
		{
			int user_id = objCompetitor.user_id;
			if (objCompetitor.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objCompetitor, GeometryType.Point.ToString(), objCompetitor.geom);
				//Fill Parent detail...              
				fillParentDetail(objCompetitor, new NetworkCodeIn() { eType = EntityType.Competitor.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCompetitor.geom }, objCompetitor.networkIdType);
				// fill latlong values
				string[] lnglat = objCompetitor.geom.Split(new string[] { " " }, StringSplitOptions.None);
				objCompetitor.latitude = Convert.ToDouble(lnglat[1].ToString());
				objCompetitor.longitude = Convert.ToDouble(lnglat[0].ToString());
				objCompetitor.other_info = null; //for additional-attributes
			}
			else
			{
				// Get entity detail by Id...
				objCompetitor = new BLMisc().GetEntityDetailById<Competitor>(objCompetitor.system_id, EntityType.Competitor, objCompetitor.user_id);
				//for additional-attributes
				objCompetitor.other_info = new BLCompetitor().GetOtherInfoCompetitor(objCompetitor.system_id);
				fillRegionProvAbbr(objCompetitor);
			}
			objCompetitor.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objCompetitor;
		}

		#endregion

		#region Save Competitor
		/// <summary> SaveCompetitor </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<Competitor> SaveCompetitor(ReqInput data)
		{
			var response = new ApiResponse<Competitor>();
			Competitor objCompetitor = ReqHelper.GetRequestData<Competitor>(data);
			try
			{
				ModelState.Clear();
				if (objCompetitor.networkIdType == NetworkIdType.A.ToString() && objCompetitor.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Competitor.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCompetitor.geom });
					if (objCompetitor.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objCompetitor = GetCompetitorDetail(objCompetitor);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objCompetitor.name = objNetworkCodeDetail.network_code;
					}
					//SET NETWORK CODE
					objCompetitor.network_id = objNetworkCodeDetail.network_code;
					objCompetitor.sequence_id = objNetworkCodeDetail.sequence_id;
					// fill latlong values
					string[] lnglat = objCompetitor.geom.Split(new string[] { " " }, StringSplitOptions.None);
					objCompetitor.latitude = Convert.ToDouble(lnglat[1].ToString());
					objCompetitor.longitude = Convert.ToDouble(lnglat[0].ToString());
				}
				if (string.IsNullOrEmpty(objCompetitor.name))
				{
					objCompetitor.name = objCompetitor.network_id;
				}
				this.Validate(objCompetitor);
				if (ModelState.IsValid)
				{
					string[] LayerName = { EntityType.Competitor.ToString() };
					var isNew = objCompetitor.system_id > 0 ? false : true;
					var resultItem = new BLCompetitor().SaveCompetitor(objCompetitor, objCompetitor.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (isNew)
						{
							objCompetitor.objPM.status = ResponseStatus.OK.ToString();
							objCompetitor.objPM.isNewEntity = isNew;
							objCompetitor.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							objCompetitor.objPM.status = ResponseStatus.OK.ToString();
							objCompetitor.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objCompetitor.objPM.status = ResponseStatus.FAILED.ToString();
						objCompetitor.objPM.message = getFirstErrorFromModelState();
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
						response.error_message = getFirstErrorFromModelState();
						response.results = objCompetitor;
					}
				}
				else
				{
					objCompetitor.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
					response.results = objCompetitor;
				}
				if (objCompetitor.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objCompetitor;
				}
				else
				{
					BindCompetitorIcons(objCompetitor);
					//Get the layer details to bind additional attributes Competitor
					var layerdetails = new BLLayer().getLayer(EntityType.Competitor.ToString());
					objCompetitor.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes Competitor
					// RETURN PARTIAL VIEW WITH MODEL DATA
					response.results = objCompetitor;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveCompetitor()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region BindCompetitorIcons
		/// <param name="data">objCompetitor</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindCompetitorIcons(Competitor objCompetitor)
		{
			/// GET COMPETITOR IMAGES FROM FOLDER AND BIND---
			var CompetitorImagePath = ConfigurationManager.AppSettings["CompetitorImagePath"];
			var CompPath = System.Web.Hosting.HostingEnvironment.MapPath(CompetitorImagePath);
			var imageFiles = Directory.GetFiles(CompPath);
			foreach (var item in imageFiles)
			{
				var iconName = Path.GetFileName(item);
				objCompetitor.lstIcons.Add(iconName.ToString());
			}
			// END
		}
		#endregion

		#endregion

		#region Fault
		#region Add Fault
		/// <summary> Add Fault </summary>
		/// <returns>Fault Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<Fault> AddFault(ReqInput data)
		{
			var response = new ApiResponse<Fault>();
			try
			{
				Fault objFaultStatusVeiwModel = ReqHelper.GetRequestData<Fault>(data);
				if (string.IsNullOrWhiteSpace(objFaultStatusVeiwModel.geom) && objFaultStatusVeiwModel.system_id == 0)
				{
					objFaultStatusVeiwModel.geom = new BLMisc().getEntityGeom(objFaultStatusVeiwModel.pSystemId, objFaultStatusVeiwModel.pEntityType);
				}
				objFaultStatusVeiwModel = GetFaultDetail(objFaultStatusVeiwModel);
				objFaultStatusVeiwModel.objFaultStatusHistory = GetFaultStatusDetail(objFaultStatusVeiwModel.system_id, objFaultStatusVeiwModel.user_name);
				BindFaultDropDown(objFaultStatusVeiwModel);
				response.status = StatusCodes.OK.ToString();
				response.results = objFaultStatusVeiwModel;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddFault()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region Get Fault Details
		/// <summary> GetFaultDetail</summary>
		/// <param >networkIdType,geom,userId,SystemId</param>
		/// <returns>Fault Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public Fault GetFaultDetail(Fault objFault)
		{
			int id = objFault.user_id;
			if (objFault.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objFault, GeometryType.Point.ToString(), objFault.geom);
				//Fill Parent detail...              
				fillParentDetail(objFault, new NetworkCodeIn() { eType = EntityType.Fault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFault.geom }, objFault.networkIdType);
				string[] lnglat = objFault.geom.Split(new string[] { " " }, StringSplitOptions.None);
				objFault.latitude = Convert.ToDouble(lnglat[1].ToString());
				objFault.longitude = Convert.ToDouble(lnglat[0].ToString());
			}
			else
			{
				// Get entity detail by Id...
				objFault = new BLMisc().GetEntityDetailById<Fault>(objFault.system_id, EntityType.Fault, objFault.user_id);
				//  objFault = new BLMisc().GetEntityDetailById<Fault>(systemId, EntityType.Fault);
				fillRegionProvAbbr(objFault);
			}
			objFault.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());
			return objFault;
		}

		#endregion

		#region GetFaultStatusDetail

		public FaultStatusHistory GetFaultStatusDetail(int faultSystemId, string user_name)
		{
			FaultStatusHistory objFault = new FaultStatusHistory();
			if (faultSystemId == 0)
			{
				objFault.updated_by = user_name;
			}
			else
			{
				// Get entity detail by Id... 
				objFault = BLFaultStatusHistory.Instance.GetFaultStatusHistoryById(faultSystemId);
			}

			return objFault;
		}
		#endregion

		#region Save Fault
		/// <summary> SaveFault </summary>
		/// <param name="data">ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<Fault> SaveFault(ReqInput data)
		{
			var response = new ApiResponse<Fault>();
			Fault objFaultStatusViewModel = ReqHelper.GetRequestData<Fault>(data);
			try
			{
				ModelState.Clear();
				if (objFaultStatusViewModel.networkIdType == NetworkIdType.A.ToString() && objFaultStatusViewModel.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Fault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFaultStatusViewModel.geom });
					if (objFaultStatusViewModel.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objFaultStatusViewModel = GetFaultDetail(objFaultStatusViewModel);

					}
					//SET NETWORK CODE
					objFaultStatusViewModel.network_id = objNetworkCodeDetail.network_code;
					objFaultStatusViewModel.sequence_id = objNetworkCodeDetail.sequence_id;
					objFaultStatusViewModel.parent_network_id = objNetworkCodeDetail.parent_network_id;
					if (objFaultStatusViewModel.select_entity == "")
					  objFaultStatusViewModel.select_entity = "0"; 
				}
				this.Validate(objFaultStatusViewModel);
				if (ModelState.IsValid)
				{
					// GENERATE FAULT ID
					if (objFaultStatusViewModel.system_id == 0)
					{
						var faultDetails = BLFault.Instance.GetFaultID(objFaultStatusViewModel.parent_network_id);
						objFaultStatusViewModel.fault_id = faultDetails.fault_id;
					}
					objFaultStatusViewModel.fault_status = objFaultStatusViewModel.objFaultStatusHistory.fault_status;
					var isNew = objFaultStatusViewModel.system_id > 0 ? false : true;
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.Fault.ToString().ToUpper()).FirstOrDefault().layer_title;
					//var selected_Entity_GeomType = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objFaultStatusViewModel.objFault.fault_entity_type.ToUpper()).FirstOrDefault().geom_type;
					var latlong = new BLMisc().GetFaultEntityGeomInfo(objFaultStatusViewModel.fault_entity_system_id, EntityType.Fault.ToString(), objFaultStatusViewModel.latitude, objFaultStatusViewModel.longitude);
					if (latlong.entityGeom != "" && latlong.entityGeom != null)
					{
						string[] lnglat = latlong.entityGeom.Split(new string[] { " " }, StringSplitOptions.None);
						objFaultStatusViewModel.latitude = Convert.ToDouble(lnglat[1].ToString());
						objFaultStatusViewModel.longitude = Convert.ToDouble(lnglat[0].ToString());
					}
					var result = BLFault.Instance.SaveFault(objFaultStatusViewModel, objFaultStatusViewModel.user_id);
					if (string.IsNullOrEmpty(result.objPM.message))
					{
						objFaultStatusViewModel.objFaultStatusHistory.fault_system_id = result.system_id;
						var objStatus = BLFaultStatusHistory.Instance.SaveFaultStatusHistory(objFaultStatusViewModel.objFaultStatusHistory, objFaultStatusViewModel.user_id);
						string[] LayerName = { layer_title };
						if (isNew)
						{
							objFaultStatusViewModel.objPM.status = ResponseStatus.OK.ToString();
							objFaultStatusViewModel.objPM.isNewEntity = isNew;
							objFaultStatusViewModel.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							objFaultStatusViewModel.objPM.status = ResponseStatus.OK.ToString();
							objFaultStatusViewModel.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
						}
					}
					else
					{
						objFaultStatusViewModel.objPM.status = ResponseStatus.FAILED.ToString();
						objFaultStatusViewModel.objPM.message = getFirstErrorFromModelState();
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = getFirstErrorFromModelState();
					}
				}
				else
				{
					objFaultStatusViewModel.objPM.status = ResponseStatus.FAILED.ToString();
					objFaultStatusViewModel.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objFaultStatusViewModel.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objFaultStatusViewModel;
				}
				else
				{
					// RETURN PARTIAL VIEW WITH MODEL DATA
					// fill dropdowns
					BindFaultDropDown(objFaultStatusViewModel);
					//objFaultStatusViewModel.objFault.created_on = Utility.MiscHelper.FormatDate(objFaultStatusViewModel.objFault.created_on.ToString());
					response.results = objFaultStatusViewModel;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveFault()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message;
			}
			return response;
		}
		#endregion

		#region BindFaultDropDown
		/// <param name="data">objFaultViewModel</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindFaultDropDown(Fault objFaultViewModel)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Fault.ToString());
			objFaultViewModel.lstTicketType = objDDL.Where(x => x.dropdown_type == DropDownType.Fault_Ticket_Type.ToString()).ToList();
			objFaultViewModel.objFaultStatusHistory.lstFaultStatus = objDDL.Where(x => x.dropdown_type == DropDownType.Fault_Status.ToString()).ToList();
			objFaultViewModel.lstEntitiesNearbyFault = new BLMisc().getEntitiesNearbyFault(objFaultViewModel.latitude, objFaultViewModel.longitude);
			objFaultViewModel.lstBusinessType = objDDL.Where(x => x.dropdown_type == DropDownType.Business_Type.ToString()).ToList();
			//objFault.objFaultStatusHistory.lstFaultStatus = abc;
			//objFault.lstRFSStatus = objDDL.Where(x => x.dropdown_type == DropDownType.RFS_Status.ToString()).ToList();
			//objFault.lstMedia = objDDL.Where(x => x.dropdown_type == DropDownType.Media.ToString()).ToList();
			//objFault.lstBuildingType = objDDL.Where(x => x.dropdown_type == DropDownType.Building_Type.ToString()).ToList();
			//objFault.lstSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.SubCategory.ToString()).ToList();
		}
		#endregion

		#endregion

		#region Save AT Acceptance
		/// <summary>Save ATA cceptance </summary>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		///<Created Date>11-Jan-2020</Created Date>
		private void SaveATAcceptance(entityATStatusAtachmentsList objList, int system_id, int userId)
		{
			BLATAcceptance.Instance.SaveATAcceptance(objList.listAtStatusRecords, system_id, userId);

		}
        private void SaveExecutionmethod(trenchExecutionList objList, int system_id, int userId)
        {
            BLExecution.Instance.SaveExecutionmethod(objList.listExecutionRecords, system_id, userId);
        }


		#endregion
		#region Save VSAT 
		/// <summary>Save Save VSAT  </summary>
		/// <CreatedBy>Rajesh Kumar</CreatedBy>
		///<Created Date>11-Jan-2020</Created Date>
		private void SaveVsat(BuildingMaster objBM)
		{
			var geom = objBM.longitude + " " + objBM.latitude;
			PageMessage objMsg = new PageMessage();
			var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.VSAT.ToString(), gType = GeometryType.Point.ToString(), parent_sysId = objBM.system_id, parent_eType = EntityType.Building.ToString(), eGeom = geom });
			objBM.objVSATDetails.created_by = objBM.user_id;
			objBM.objVSATDetails.network_id = objNetworkCodeDetail.network_code;
			objBM.objVSATDetails.parent_system_id = objBM.system_id;
			objBM.objVSATDetails.parent_entity_type = objBM.entityType;
			objBM.objVSATDetails.parent_network_id = objBM.network_id;
			objBM.objVSATDetails.region_id = objBM.region_id;
			objBM.objVSATDetails.province_id = objBM.province_id;
			objBM.objVSATDetails.latitude = objBM.latitude;
			objBM.objVSATDetails.longitude = objBM.longitude;
			var status = new BLMisc().SaveVsat(objBM.objVSATDetails, objBM.system_id);

		}

		#endregion

		#region Fill Project Specifications Details
		/// <summary>fillProjectSpecifications in modal </summary>
		/// <param>objLib</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void fillProjectSpecifications(dynamic objLib)
		{
			//"P" we need to pass this value as dynamically as network stage selection
			objLib.lstBindProjectCode = new BLProject().getProjectCodeDetails("P");
			objLib.lstBindPlanningCode = new BLProject().getPlanningDetailByIdList(Convert.ToInt32(objLib.project_id ?? 0));
			objLib.lstBindWorkorderCode = new BLProject().getWorkorderDetailByIdList(Convert.ToInt32(objLib.planning_id ?? 0));
			objLib.lstBindPurposeCode = new BLProject().getPurposeDetailByIdList(Convert.ToInt32(objLib.workorder_id ?? 0));
		}
		#endregion

		#region fill Region Province Detail
		/// <summary> fillRegionProvinceDetail </summary>
		/// <param>objEntityModel,enType,geom</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void fillRegionProvinceDetail(dynamic objEntityModel, string enType, string geom)
		{
			List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
			objRegionProvince = BLBuilding.Instance.GetRegionProvince(geom, enType);
			if (objRegionProvince != null && objRegionProvince.Count > 0)
			{
				objEntityModel.region_id = objRegionProvince[0].region_id;
				objEntityModel.province_id = objRegionProvince[0].province_id;
				objEntityModel.region_name = objRegionProvince[0].region_name;
				objEntityModel.province_name = objRegionProvince[0].province_name;
			}
			List<InGeographicDetails> obj = new List<InGeographicDetails>();
			obj = BLBuilding.Instance.GetGeographicDetails(geom, enType);
			try
			{
				if (obj != null && obj.Count > 0)
				{
					foreach (var item in obj)
					{
						if (string.IsNullOrEmpty(objEntityModel.area_id))
						{
							if (item.entity_type.ToUpper() == EntityType.Area.ToString().ToUpper())
							{
								objEntityModel.area_id = item.entity_network_id;
							}
						}
						if (item.entity_type.ToUpper() == EntityType.SubArea.ToString().ToUpper())
						{
							objEntityModel.subarea_id = item.entity_network_id;
						}
						if (item.entity_type.ToUpper() == EntityType.DSA.ToString().ToUpper())
						{
							objEntityModel.dsa_id = item.entity_network_id;
						}
						if (item.entity_type.ToUpper() == EntityType.CSA.ToString().ToUpper())
						{
							objEntityModel.csa_id = item.entity_network_id;
						}
					}
					objEntityModel.region_abbreviation = obj[0].region_abbreviation;
					objEntityModel.province_abbreviation = obj[0].province_abbreviation;
				}
			}
			catch (Exception ex)
			{
				string exception = ex.Message;
			}
		}
		#endregion

		#region Fill Parent Detail
		/// <summary> fillParentDetail </summary>
		/// <param>networkIdType,objLib,objIn</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void fillParentDetail(dynamic objLib, NetworkCodeIn objIn, string networkIdType)
		{
			//fill parent detail....
			var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
			if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
			{
				if (networkIdType == NetworkIdType.M.ToString())
				{
					//FILL NETWORK CODE FORMAT FOR MANUAL
					objLib.network_id = networkCodeDetail.network_code;
				}
				objLib.parent_entity_type = networkCodeDetail.parent_entity_type;
				objLib.parent_network_id = networkCodeDetail.parent_network_id;
				objLib.parent_system_id = networkCodeDetail.parent_system_id;
                objLib.sequence_id = networkCodeDetail.sequence_id;
            }
		}
		#endregion

		#region Save Reference
		/// <summary> SaveReference </summary>
		/// <param>entityReference , system_id</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void SaveReference(EntityReference entityReference, int system_id)
		{
			BLReference.Instance.SaveReference(entityReference, system_id);
		}
		#endregion

		#region Error Handling
		/// <summary> Error Handling </summary>
		/// <returns>Error Message</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public string getFirstErrorFromModelState()
		{
			var Error = ModelState.Values.Select(e => e.Errors).FirstOrDefault();
			if (Error != null)
				return Error[0].ErrorMessage;
			else
				return "";
		}
		#endregion

		# region GetPointTypeParentGeom
		/// <summary> GetPointTypeParentGeom </summary>
		/// <returns>geom</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private string GetPointTypeParentGeom(int pSystemId, string pEntityType)
		{
			string geom = "";
			//get parent detail..
			if (pSystemId > 0 && !string.IsNullOrEmpty(pEntityType))
			{
				var dicParentEntityDetail = new BLMisc().GetEntityDetailById<Dictionary<string, string>>(pSystemId, (EntityType)Enum.Parse(typeof(EntityType), pEntityType));
				if (dicParentEntityDetail != null)
				{
					//set geometry value as parent..
					geom = dicParentEntityDetail["longitude"] + " " + dicParentEntityDetail["latitude"];
				}
				return geom;
			}
			else
			{
				return "Parent system id or parent entity type required";
			}
		}
		#endregion

		#region fill ISP Region Province Detail
		/// <summary> fillISPRegionProvinceDetail </summary>
		/// <param>objEntityModel,enType,geom</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void fillISPRegionProvinceDetail(dynamic objEntityModel, StructureMaster structureDetails, string geomType)
		{
			if (structureDetails != null)
			{
				objEntityModel.region_id = structureDetails.region_id;
				objEntityModel.province_id = structureDetails.province_id;
				System.Reflection.PropertyInfo pi = objEntityModel.GetType().GetProperty("latitude");
				Type t = pi.PropertyType;
				if (t.FullName == "System.Decimal")
				{
					objEntityModel.latitude = Convert.ToDecimal(structureDetails.latitude);
					objEntityModel.longitude = Convert.ToDecimal(structureDetails.longitude);
				}
				else
				{
					objEntityModel.latitude = structureDetails.latitude;
					objEntityModel.longitude = structureDetails.longitude;
				}

				objEntityModel.region_name = structureDetails.region_name;
				objEntityModel.province_name = structureDetails.province_name;
			}
			var geom = structureDetails.longitude + " " + structureDetails.latitude;
			List<InGeographicDetails> obj = new List<InGeographicDetails>();
			obj = BLBuilding.Instance.GetGeographicDetails(geom, geomType);
			try
			{
				if (obj != null && obj.Count > 0)
				{
					foreach (var item in obj)
					{
						if (string.IsNullOrEmpty(objEntityModel.area_id))
						{
							if (item.entity_type.ToUpper() == EntityType.Area.ToString().ToUpper())
							{
								objEntityModel.area_id = item.entity_network_id;
							}
						}
						if (item.entity_type.ToUpper() == EntityType.SubArea.ToString().ToUpper())
						{
							objEntityModel.subarea_id = item.entity_network_id;
						}
						if (item.entity_type.ToUpper() == EntityType.DSA.ToString().ToUpper())
						{
							objEntityModel.dsa_id = item.entity_network_id;
						}
						if (item.entity_type.ToUpper() == EntityType.CSA.ToString().ToUpper())
						{
							objEntityModel.csa_id = item.entity_network_id;
						}
					}
					objEntityModel.region_abbreviation = obj[0].region_abbreviation;
					objEntityModel.province_abbreviation = obj[0].province_abbreviation;
				}
			}
			catch (Exception ex)
			{
				string exception = ex.Message;
			}
		}
		#endregion

		#region Fill ISP Parent Detail
		/// <summary> fillISPParentDetail </summary>
		/// <param>networkIdType,objLib,objIn</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void fillISPParentDetail(dynamic objLib, ISPNetworkCodeIn objIn, string networkIdType, int pSystemId, string pEntityType, string pNetworkid)
		{
			var networkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(objIn);
			if (string.IsNullOrEmpty(networkCodeDetail.err_msg))
			{
				if (networkIdType == NetworkIdType.M.ToString())
				{
					//FILL NETWORK CODE FORMAT FOR MANUAL
					objLib.network_id = networkCodeDetail.network_code;
				}
				objLib.parent_entity_type = pEntityType;
				objLib.parent_network_id = pNetworkid;
				objLib.parent_system_id = pSystemId;
				objLib.pEntityType = pEntityType;
				objLib.pNetworkId = pNetworkid;
				objLib.pSystemId = pSystemId;
			}
		}
		#endregion

		#region Province

		#region Add Province
		/// <summary> Add Province </summary>
		/// <param name="data">obj</param>
		/// <returns>Province Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<UpdateGeomtaryValue> AddProvince(ReqInput data)
		{
			var response = new ApiResponse<UpdateGeomtaryValue>();
			try
			{
				UpdateGeomtaryValue objRegionProvince = ReqHelper.GetRequestData<UpdateGeomtaryValue>(data);
				if (objRegionProvince.lstUpdateRegionProvince[0].id == 0)
				{
					var objRegPro = BLRegionProvince.Instance.GetRegionDetailbyProvinceGeom(objRegionProvince.lstUpdateRegionProvince[0].geomtext);
					if (objRegPro != null)
					{
						objRegionProvince.lstUpdateRegionProvince.Add(new UpdateGeomtaryProperties());
						objRegionProvince.lstUpdateRegionProvince[0].region_name = objRegPro[0].region_name;
						objRegionProvince.lstUpdateRegionProvince[0].country_name = objRegPro[0].country_name;
					}
				}
				if (Convert.ToInt32(objRegionProvince.lstUpdateRegionProvince[0].existing_id) > 0)
				{
					var provinceList = GetRegionProvinceDetailForExport("", objRegionProvince.lstUpdateRegionProvince[0].boundarytype);
					provinceList = provinceList.AsEnumerable().Where(x => x.Field<int>("Id") == Convert.ToInt32(objRegionProvince.lstUpdateRegionProvince[0].existing_id)).CopyToDataTable();
					objRegionProvince.lstUpdateRegionProvince[0].existing_id = Convert.ToInt32(provinceList.Rows[0]["Id"]);
					objRegionProvince.lstUpdateRegionProvince[0].region_name = provinceList.Rows[0]["Region_name"].ToString();
					objRegionProvince.lstUpdateRegionProvince[0].province_name = provinceList.Rows[0]["province_name"].ToString();
					objRegionProvince.lstUpdateRegionProvince[0].province_abbreviation = provinceList.Rows[0]["province_abbreviation"].ToString();
					objRegionProvince.lstUpdateRegionProvince[0].country_name = provinceList.Rows[0]["country_name"].ToString();
					objRegionProvince.is_active = Convert.ToBoolean(provinceList.Rows[0]["is_active"]);
				}
				response.status = StatusCodes.OK.ToString();
				response.results = objRegionProvince;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddProvince()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion


		#region Save Province
		/// <summary> Save Province </summary>
		/// <param name="data">obj</param>
		/// <returns>Save Province</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>

		public ApiResponse<UpdateGeomtaryValue> SaveProvince(ReqInput data)
		{
			var response = new ApiResponse<UpdateGeomtaryValue>();
			try
			{
				UpdateGeomtaryValue objRegionProvince = ReqHelper.GetRequestData<UpdateGeomtaryValue>(data);
				List<Status> message = new List<Status>();
				if (objRegionProvince.lstUpdateRegionProvince[0].id > 0)
				{
					//objRegionProvince.lstUpdateRegionProvince[0].region_name = "";
					//objRegionProvince.lstUpdateRegionProvince[0].region_abbreviation = "";
					//objRegionProvince.lstUpdateRegionProvince[0].province_name = "";
					//objRegionProvince.lstUpdateRegionProvince[0].province_abbreviation = "";
					//objRegionProvince.lstUpdateRegionProvince[0].shapefilepath = "";
					objRegionProvince.lstUpdateRegionProvince[0].created_by = objRegionProvince.user_id;
					objRegionProvince.lstUpdateRegionProvince[0].geomtext = objRegionProvince.lstUpdateRegionProvince[0].geomtext;
					// objRegionProvince.lstUpdateRegionProvince[0].country_name = "";
					objRegionProvince.lstUpdateRegionProvince[0].boundarytype = objRegionProvince.lstUpdateRegionProvince[0].boundarytype;
					objRegionProvince.lstUpdateRegionProvince[0].existing_id = objRegionProvince.lstUpdateRegionProvince[0].id;
					objRegionProvince.lstUpdateRegionProvince[0].entryStatus = "EDIT";
				}
				else
				{
					objRegionProvince.lstUpdateRegionProvince[0].region_abbreviation = "";
					objRegionProvince.lstUpdateRegionProvince[0].shapefilepath = "";
					objRegionProvince.lstUpdateRegionProvince[0].created_by = objRegionProvince.user_id;
					objRegionProvince.lstUpdateRegionProvince[0].geomtext = objRegionProvince.lstUpdateRegionProvince[0].geomtext == null ? "" : objRegionProvince.lstUpdateRegionProvince[0].geomtext;
					objRegionProvince.lstUpdateRegionProvince[0].entryStatus = objRegionProvince.lstUpdateRegionProvince[0].entryStatus == "" ? "NEW" : objRegionProvince.lstUpdateRegionProvince[0].entryStatus;
					objRegionProvince.lstUpdateRegionProvince[0].is_active = objRegionProvince.is_active;
				}
				message = BLRegionProvince.Instance.SaveRegionProvinceGeomatery(objRegionProvince.lstUpdateRegionProvince[0]);

				if (message[0].status.ToLower().Contains("exception") || message[0].status.ToLower().Contains("failed"))
				{
					objRegionProvince.pageMsg.status = ResponseStatus.FAILED.ToString();
					objRegionProvince.pageMsg.message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);//message[0].message;
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);//message[0].message;
				}
				else
				{
					objRegionProvince.pageMsg.status = ResponseStatus.OK.ToString();
					objRegionProvince.pageMsg.message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);
					response.status = ResponseStatus.OK.ToString();
					response.error_message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);
				}
				response.results = objRegionProvince;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveProvince()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Delete Province
		/// <summary> Delete Province </summary>
		/// <param name="data">obj</param>
		/// <returns>Delete Province</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<UpdateGeomtaryValue> DeleteProvince(ReqInput data)
		{
			var response = new ApiResponse<UpdateGeomtaryValue>();
			try
			{
				UpdateGeomtaryValue objRegionProvince = ReqHelper.GetRequestData<UpdateGeomtaryValue>(data);
				List<Status> message = new List<Status>();
				message = new BLRegionProvince().DeleteRegionProvince(objRegionProvince.lstUpdateRegionProvince[0].id, objRegionProvince.lstUpdateRegionProvince[0].boundarytype, objRegionProvince.user_id);
				if (message[0].status.ToLower().Contains("exception") || message[0].status.ToLower().Contains("failed"))
				{
					objRegionProvince.pageMsg.status = ResponseStatus.FAILED.ToString();
					objRegionProvince.pageMsg.message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);//message[0].message;
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);
				}
				else
				{
					objRegionProvince.pageMsg.status = ResponseStatus.OK.ToString();
					objRegionProvince.pageMsg.message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);//message[0].message;
					response.status = ResponseStatus.OK.ToString();
					response.error_message = BLConvertMLanguage.MultilingualMessageConvert(message[0].message);
				}
				response.results = objRegionProvince;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("DeleteProvince()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get Region Province Export Report
		/// <summary> GetRegionProvinceDetailForExport </summary>
		/// <param name="data">obj</param>
		/// <returns>Region Province Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public DataTable GetRegionProvinceDetailForExport(string Filter, string ReportType)
		{
			ViewRegionProvinces objViewProvince = new ViewRegionProvinces();
			objViewProvince.lstViewRegionProvinceDetails = new BLRegionProvince().GetRegionProvinceList(objViewProvince.objGridAttributes, ReportType, Filter);

			DataTable dt = new DataTable();
			dt = MiscHelper.ListToDataTable(objViewProvince.lstViewRegionProvinceDetails);
			dt.TableName = "RegionProvinceDetails";
			return dt;
		}
		#endregion


		#endregion

		#region EntityEditInfo
		public ApiResponse<EntityInfo> EditEntityInfo(ReqInput data, HeaderAttributes headerAttribute, string entityAction)
		{
			var response = new ApiResponse<EntityInfo>();
			EntityInfo objEntityInfo = ReqHelper.GetRequestData<EntityInfo>(data);
			objEntityInfo.entity_action = entityAction;
			objEntityInfo.attribute_info = data.data;
			objEntityInfo.entity_type = headerAttribute.entity_type;
			objEntityInfo.source_ref_id = headerAttribute.source_ref_id;
			objEntityInfo.source_ref_type = headerAttribute.source_ref_type.ToUpper();
			try
			{
				if (objEntityInfo.system_id != 0)
				{
					var result = new BLNetworkTicket().EditEntityInfo(objEntityInfo);
					if (result.status)
					{
						var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == headerAttribute.entity_type.ToUpper()).FirstOrDefault() : null;

						response.error_message = string.Format(BLConvertMLanguage.MultilingualMessageConvert(Resources.Resources.SI_OSP_GBL_GBL_GBL_064), layerDetail.layer_title.ToUpper());
						response.status = ResponseStatus.OK.ToString();
						response.results = objEntityInfo;
					}
					else
					{
						response.error_message = Resources.Resources.SI_OSP_GBL_NET_FRM_320;
						response.status = ResponseStatus.OK.ToString();
					}

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("EntityEditInfo()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region ISP FMS

		#region ISP FMS Details
		/// <summary> Get FMS Details</summary>
		/// <param name="data">objFMS,ModelInfo</param>
		/// <returns>ISP FMS Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public FMSMaster GetISPFMSDetail(FMSMaster objFMS)
		{
			if (objFMS.system_id != 0)
			{
				objFMS = new BLMisc().GetEntityDetailById<FMSMaster>(objFMS.system_id, EntityType.FMS, objFMS.user_id);
				objFMS.no_of_port = objFMS.no_of_port;
				objFMS.geom = objFMS.longitude + " " + objFMS.latitude;
				var ispMapping = new BLISP().getMappingByEntityId(objFMS.system_id, EntityType.FMS.ToString());
				if (ispMapping != null && ispMapping.id > 0)
				{
					objFMS.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					objFMS.objIspEntityMap.floor_id = ispMapping.floor_id;
					objFMS.objIspEntityMap.structure_id = ispMapping.structure_id;
				}
				//for additional-attributes
				objFMS.other_info = new BLFMS().GetOtherInfoFMS(objFMS.system_id);
				fillRegionProvAbbr(objFMS);
			}
			else
			{
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objFMS.objIspEntityMap.structure_id, EntityType.Structure);
				objFMS.geom = structureDetails.longitude + " " + structureDetails.latitude;
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objFMS, structureDetails, GeometryType.Point.ToString());
				//Fill Parent detail...              
				fillISPParentDetail(objFMS, new ISPNetworkCodeIn() { parent_sysId = objFMS.pSystemId, parent_eType = objFMS.pEntityType, parent_networkId = objFMS.pNetworkId, eType = EntityType.FMS.ToString(), structureId = objFMS.objIspEntityMap.structure_id }, objFMS.networkIdType, objFMS.pSystemId, objFMS.pEntityType, objFMS.pNetworkId);
				//if (networkIdType == NetworkIdType.M.ToString())
				//{
				//    // for Manual network id type 
				//    var ISPNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail();
				//    objFMS.network_id = ISPNetworkCodeDetail.network_code;
				//}
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<FMSTemplateMaster>(objFMS.user_id, EntityType.FMS);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objFMS);
				objFMS.no_of_port = objItem.no_of_port;
				//objFMS.objIspEntityMap.shaft_id = objFMS.objIspEntityMap.shaftid;
				//objFMS.objIspEntityMap.floor_id = ModelInfo.floorid;
				//objFMS.objIspEntityMap.structure_id = ModelInfo.structureid;
				objFMS.address = BLStructure.Instance.getBuildingAddress(objFMS.objIspEntityMap.structure_id);
				objFMS.ownership_type = "Own";
				objFMS.other_info = null;   //for additional-attributes
			}
			return objFMS;
		}

		#endregion

		#region Add ISP FMS
		/// <summary> Add ISP FMS </summary>
		/// <param name="data">networkIdType,systemId,geom,userId</param>
		/// <returns>FMS Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>

		public ApiResponse<FMSMaster> AddISPFMS(ReqInput data)
		{
			var response = new ApiResponse<FMSMaster>();
			try
			{
				FMSMaster objRequestIn = ReqHelper.GetRequestData<FMSMaster>(data);
				FMSMaster objFMSMaster = GetISPFMSDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
				BindISPFMSDropdown(objFMSMaster);
				fillProjectSpecifications(objFMSMaster);
				new BLMisc().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
				//Get the layer details to bind additional attributes FMS
				var layerdetails = new BLLayer().getLayer(EntityType.FMS.ToString());
				objFMSMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End for additional attributes FMS
				response.status = StatusCodes.OK.ToString();
				response.results = objFMSMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPFMS()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP FMS Dropdown
		/// <summary> BindISPFMSDropdown </summary>
		/// <param name="data">object</param>
		/// <returns>ISP FMS Details</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void BindISPFMSDropdown(FMSMaster objFMS)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.FMS.ToString());
			objFMS.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objFMS.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objFMS.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objFMS.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save ISP FMS
		/// <summary> SaveISPFMS </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		public ApiResponse<FMSMaster> SaveISPFMS(ReqInput data)
		{
			var response = new ApiResponse<FMSMaster>();
			FMSMaster objFMSMaster = ReqHelper.GetRequestData<FMSMaster>(data);
			try
			{
				string pNetworkId = objFMSMaster.pNetworkId;
				ModelState.Clear();
				int structure_id = objFMSMaster.objIspEntityMap.structure_id;
				int pSystemId = objFMSMaster.pSystemId;
				string pEntityType = objFMSMaster.pEntityType;
				int floor_id = objFMSMaster.objIspEntityMap.floor_id ?? 0;
				int shaft_id = objFMSMaster.objIspEntityMap.shaft_id ?? 0;
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objFMSMaster.structure_id, EntityType.Structure);
				//if (structureDetails != null)
				//{
				//    objFMSMaster.province_id = structureDetails.First().province_id;
				//}
				if (objFMSMaster.networkIdType == NetworkIdType.A.ToString() && objFMSMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = pSystemId, parent_eType = pEntityType, eType = EntityType.FMS.ToString(), structureId = objFMSMaster.objIspEntityMap.structure_id });
					if (objFMSMaster.isDirectSave == true)
					{
						objFMSMaster = GetISPFMSDetail(objFMSMaster);
						//objFMSMaster.objIspEntityMap.structure_id = structure_id;
						//objFMSMaster.parent_system_id = pSystemId;
						//objFMSMaster.parent_entity_type = pEntityType;
						objFMSMaster.fms_name = objISPNetworkCode.network_code;
						//objFMSMaster.pSystemId = pSystemId;
						//objFMSMaster.pEntityType = pEntityType;
						//objFMSMaster.pNetworkId = pNetworkId;
						//objFMSMaster.objIspEntityMap.floor_id = floor_id;
						//objFMSMaster.objIspEntityMap.shaft_id = shaft_id;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objFMSMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objFMSMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					objFMSMaster.network_id = objISPNetworkCode.network_code;
					objFMSMaster.sequence_id = objISPNetworkCode.sequence_id;

				}
				if (structureDetails != null)
				{
					objFMSMaster.region_id = structureDetails.region_id;
					objFMSMaster.province_id = structureDetails.province_id;
					objFMSMaster.region_name = structureDetails.region_name;
					objFMSMaster.province_name = structureDetails.province_name;
					objFMSMaster.latitude = structureDetails.latitude;
					objFMSMaster.longitude = structureDetails.longitude;
				}
				if (objFMSMaster.unitValue != null && objFMSMaster.unitValue.Contains(":"))
				{
					objFMSMaster.no_of_input_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[0]);
					objFMSMaster.no_of_output_port = Convert.ToInt32(objFMSMaster.unitValue.Split(':')[1]);
				}
				this.Validate(objFMSMaster);
				if (ModelState.IsValid)
				{
					bool isNew = objFMSMaster.system_id == 0 ? true : false;
					var resultItem = new BLFMS().SaveEntityFMS(objFMSMaster, objFMSMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.FMS.ToString() };
						if (isNew)
						{
							objFMSMaster.objPM.status = ResponseStatus.OK.ToString(); ;
							objFMSMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "FMS saved successfully.";
							objFMSMaster.objPM.systemId = resultItem.system_id;
							objFMSMaster.objPM.entityType = EntityType.FMS.ToString();
							objFMSMaster.objPM.NetworkId = resultItem.network_id;
							objFMSMaster.objPM.structureId = objFMSMaster.objIspEntityMap.structure_id;
							objFMSMaster.objPM.shaftId = objFMSMaster.objIspEntityMap.shaft_id ?? 0;
							objFMSMaster.objPM.floorId = objFMSMaster.objIspEntityMap.floor_id ?? 0;
							objFMSMaster.objPM.pSystemId = objFMSMaster.parent_system_id;
							objFMSMaster.objPM.pEntityType = objFMSMaster.parent_entity_type;
							response.status = StatusCodes.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);// "FMS saved successfully.";
							response.results = objFMSMaster;
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objFMSMaster.objPM.status = ResponseStatus.OK.ToString();
								objFMSMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = StatusCodes.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);
								response.results = objFMSMaster;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objFMSMaster.system_id, EntityType.FMS.ToString(), objFMSMaster.network_id, objFMSMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objFMSMaster.longitude + " " + objFMSMaster.latitude }, objFMSMaster.user_id);
								objFMSMaster.objPM.status = ResponseStatus.OK.ToString(); ;
								objFMSMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "FMS updated successfully.";
								response.status = StatusCodes.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "FMS updated successfully.";
								response.results = objFMSMaster;
							}
						}
					}
					else
					{
						objFMSMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objFMSMaster.objPM.message = getFirstErrorFromModelState();
						response.status = StatusCodes.FAILED.ToString();
						response.error_message = getFirstErrorFromModelState();
						response.results = objFMSMaster;
					}
				}
				else
				{
					objFMSMaster.objPM.status = ResponseStatus.FAILED.ToString();
					objFMSMaster.objPM.message = getFirstErrorFromModelState();
					response.status = StatusCodes.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
					response.results = objFMSMaster;
				}
				objFMSMaster.entityType = EntityType.FMS.ToString();
				if (objFMSMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = StatusCodes.OK.ToString();
					response.results = objFMSMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objFMSMaster, EntityType.FMS.ToString());
					BindISPFMSDropdown(objFMSMaster);
					fillProjectSpecifications(objFMSMaster);
					new BLMisc().BindPortDetails(objFMSMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
					objFMSMaster.pNetworkId = pNetworkId;
					//Get the layer details to bind additional attributes FMS
					var layerdetails = new BLLayer().getLayer(EntityType.FMS.ToString());
					objFMSMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					//End for additional attributes FMS
					response.results = objFMSMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPFMS()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion

		//cabinet shazia 
		#region Cabinet

		#region Cabinet Details
		public CabinetMaster GetCabinetDetail(CabinetMaster objCabinet)
		{
			int user_id = objCabinet.user_id;
			if (objCabinet.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objCabinet, GeometryType.Point.ToString(), objCabinet.geom);
				//Fill Parent detail...              
				fillParentDetail(objCabinet, new NetworkCodeIn() { eType = EntityType.Cabinet.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCabinet.geom }, objCabinet.networkIdType);
				objCabinet.longitude = Convert.ToDecimal(objCabinet.geom.Split(' ')[0]);
				objCabinet.latitude = Convert.ToDecimal(objCabinet.geom.Split(' ')[1]);
				objCabinet.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<CabinetTemplateMaster>(objCabinet.user_id, EntityType.Cabinet);
				MiscHelper.CopyMatchingProperties(objItem, objCabinet);
			}
			else
			{
				// Get entity detail by Id...
				objCabinet = new BLMisc().GetEntityDetailById<CabinetMaster>(objCabinet.system_id, EntityType.Cabinet, objCabinet.user_id);
				fillRegionProvAbbr(objCabinet);
			}
			objCabinet.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objCabinet;
		}
		#endregion

		#region Add Cabinet

		public ApiResponse<CabinetMaster> AddCabinet(ReqInput data)
		{
			var response = new ApiResponse<CabinetMaster>();
			try
			{
				CabinetMaster objRequestIn = ReqHelper.GetRequestData<CabinetMaster>(data);
				CabinetMaster objCabinetMaster = GetCabinetDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objCabinetMaster, EntityType.Cabinet.ToString());
				fillProjectSpecifications(objCabinetMaster);
				BindCabinetDropDown(objCabinetMaster);
				response.status = StatusCodes.OK.ToString();
				response.results = objCabinetMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddCabinet()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind Cabinet Dropdown
		private void BindCabinetDropDown(CabinetMaster objCabinet)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objCabinet.parent_system_id, objCabinet.system_id, EntityType.Cabinet.ToString());
			if (ispEntityMap != null)
			{
				objCabinet.objIspEntityMap.id = ispEntityMap.id;
				objCabinet.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objCabinet.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objCabinet.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objCabinet.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}

			objCabinet.objIspEntityMap.AssoType = objCabinet.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objCabinet.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objCabinet.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objCabinet.longitude + " " + objCabinet.latitude);
			if (objCabinet.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objCabinet.objIspEntityMap.structure_id);
				objCabinet.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objCabinet.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objCabinet.parent_entity_type == EntityType.UNIT.ToString())
				{
					objCabinet.objIspEntityMap.unitId = objCabinet.parent_system_id;
					//objCabinet.objIspEntityMap.AssoType = "";
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.Cabinet.ToString()).FirstOrDefault() != null)
			{
				objCabinet.objIspEntityMap.isValidParent = true;
				objCabinet.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objCabinet.objIspEntityMap.structure_id, objCabinet.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = new BLLayer().GetLayerDetails().Where(m => m.layer_name.ToUpper() == EntityType.Cabinet.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objCabinet.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objCabinet.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objCabinet.objIspEntityMap.entity_type == null) { objCabinet.objIspEntityMap.entity_type = EntityType.Cabinet.ToString(); }
			objCabinet.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objCabinet.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList(EntityType.Cabinet.ToString());
			objCabinet.listCabinetType = obj_DDL.Where(x => x.dropdown_type == DropDownType.Cabinet_Type.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objCabinet.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objCabinet.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save Cabinet
		public ApiResponse<CabinetMaster> SaveCabinet(ReqInput data)
		{
			var response = new ApiResponse<CabinetMaster>();
			CabinetMaster objCabinetMaster = ReqHelper.GetRequestData<CabinetMaster>(data);
			try
			{
				if (objCabinetMaster.networkIdType == NetworkIdType.A.ToString() && objCabinetMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Cabinet.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCabinetMaster.geom, parent_eType = objCabinetMaster.pEntityType, parent_sysId = objCabinetMaster.pSystemId });
					if (objCabinetMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objCabinetMaster = GetCabinetDetail(objCabinetMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objCabinetMaster.cabinet_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//   var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objCabinetMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objCabinetMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objCabinetMaster.network_id = objNetworkCodeDetail.network_code;
					objCabinetMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objCabinetMaster.cabinet_name))
				{
					objCabinetMaster.cabinet_name = objCabinetMaster.network_id;
				}
				this.Validate(objCabinetMaster);
				if (ModelState.IsValid)
				{

					objCabinetMaster.objIspEntityMap.structure_id = Convert.ToInt32(objCabinetMaster.objIspEntityMap.AssociateStructure);
					if (objCabinetMaster.objIspEntityMap.structure_id != 0)
					{
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objCabinetMaster.objIspEntityMap.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							objCabinetMaster.latitude = Convert.ToDecimal(structureDetails.latitude);
							objCabinetMaster.longitude = Convert.ToDecimal(structureDetails.longitude);
						}

					}
					objCabinetMaster.objIspEntityMap.shaft_id = objCabinetMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objCabinetMaster.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(objCabinetMaster.objIspEntityMap.AssoType))
					{
						objCabinetMaster.objIspEntityMap.shaft_id = 0; objCabinetMaster.objIspEntityMap.floor_id = 0;
					}
					if (objCabinetMaster.objIspEntityMap.structure_id == 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.Cabinet.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCabinetMaster.longitude + " " + objCabinetMaster.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							objCabinetMaster.parent_system_id = parentDetails.p_system_id;
							objCabinetMaster.parent_network_id = parentDetails.p_network_id;
							objCabinetMaster.parent_entity_type = parentDetails.p_entity_type;
						}
					}

					var isNew = objCabinetMaster.system_id > 0 ? false : true;
					objCabinetMaster.is_new_entity = (isNew && objCabinetMaster.source_ref_id != "0" && objCabinetMaster.source_ref_id != "");

					var resultItem = new BLCabinet().SaveEntityCabinet(objCabinetMaster, objCabinetMaster.user_id);
					//Save Reference
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (objCabinetMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objCabinetMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							string[] LayerName = { EntityType.Cabinet.ToString() };
							objCabinetMaster.objPM.status = ResponseStatus.OK.ToString();
							objCabinetMaster.objPM.isNewEntity = isNew;
							objCabinetMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objCabinetMaster.objPM.status = ResponseStatus.OK.ToString();
								objCabinetMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
							}
							else
							{
								string[] LayerName = { EntityType.Cabinet.ToString() };
								BLLoopMangment.Instance.UpdateLoopDetails(objCabinetMaster.system_id, EntityType.Cabinet.ToString(), objCabinetMaster.network_id, objCabinetMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCabinetMaster.longitude + " " + objCabinetMaster.latitude }, objCabinetMaster.user_id);
								objCabinetMaster.objPM.status = ResponseStatus.OK.ToString();
								objCabinetMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						objCabinetMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objCabinetMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objCabinetMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objCabinetMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objCabinetMaster.isDirectSave == true)
				{
					response.status = ResponseStatus.OK.ToString();
					response.results = objCabinetMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objCabinetMaster, EntityType.Cabinet.ToString());
					BindCabinetDropDown(objCabinetMaster);
					fillProjectSpecifications(objCabinetMaster);
					response.results = objCabinetMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveCabinet()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion
		#region ISP Cabinet

		#region ISP Cabinet Details

		public CabinetMaster GetISPCabinetDetail(CabinetMaster objCabinet)
		{
			if (objCabinet.system_id == 0)
			{
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objCabinet.objIspEntityMap.structure_id, EntityType.Structure);
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objCabinet, structureDetails, GeometryType.Point.ToString());
				//Fill Parent detail...              
				fillISPParentDetail(objCabinet, new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), parent_networkId = objCabinet.pNetworkId, eType = EntityType.Cabinet.ToString(), structureId = objCabinet.objIspEntityMap.structure_id }, objCabinet.networkIdType, objCabinet.pSystemId, objCabinet.pEntityType, objCabinet.pNetworkId);
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<CabinetTemplateMaster>(objCabinet.user_id, EntityType.Cabinet);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objCabinet);
				objCabinet.address = BLStructure.Instance.getBuildingAddress(objCabinet.objIspEntityMap.structure_id);
				objCabinet.ownership_type = "Own";
			}
			else
			{
				// Get entity detail by Id...
				objCabinet = new BLMisc().GetEntityDetailById<CabinetMaster>(objCabinet.system_id, EntityType.Cabinet, objCabinet.user_id);
				var ispMapping = new BLISP().getMappingByEntityId(objCabinet.system_id, EntityType.Cabinet.ToString());
				if (ispMapping != null)
				{
					objCabinet.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					objCabinet.objIspEntityMap.floor_id = ispMapping.floor_id;
					objCabinet.objIspEntityMap.structure_id = ispMapping.structure_id;
				}
				fillRegionProvAbbr(objCabinet);
			}
			return objCabinet;
		}

		#endregion

		#region Add ISP Cabinet

		public ApiResponse<CabinetMaster> AddISPCabinet(ReqInput data)
		{
			var response = new ApiResponse<CabinetMaster>();
			try
			{
				CabinetMaster objRequestIn = ReqHelper.GetRequestData<CabinetMaster>(data);

				CabinetMaster objCabinetMaster = GetISPCabinetDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objCabinetMaster, EntityType.Cabinet.ToString());
				fillProjectSpecifications(objCabinetMaster);
				BindISPCabinetDropDown(objCabinetMaster);
				response.status = StatusCodes.OK.ToString();
				response.results = objCabinetMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPCabinet()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP Cabinet Dropdown

		private void BindISPCabinetDropDown(CabinetMaster objCabinetMaster)
		{
			objCabinetMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objCabinetMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var objDDL = new BLMisc().GetDropDownList("");
			objCabinetMaster.lstBOMSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objCabinetMaster.lstServedByRing = objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Save Cabinet ISP 

		public ApiResponse<CabinetMaster> SaveISPCabinet(ReqInput data)
		{
			var response = new ApiResponse<CabinetMaster>();
			CabinetMaster objCabinetMaster = ReqHelper.GetRequestData<CabinetMaster>(data);
			try
			{
				if (objCabinetMaster.networkIdType == NetworkIdType.A.ToString() && objCabinetMaster.system_id == 0)
				{
					var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objCabinetMaster.objIspEntityMap.structure_id, EntityType.Structure);
					//GET AUTO NETWORK CODE...               
					var objNetworkCodeDetail = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = structureDetails.province_id, parent_eType = Models.Admin.BoundaryType.Province.ToString(), eType = EntityType.Cabinet.ToString(), structureId = objCabinetMaster.objIspEntityMap.structure_id });
					if (objCabinetMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objCabinetMaster = GetISPCabinetDetail(objCabinetMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objCabinetMaster.cabinet_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objCabinetMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//  objCabinetMaster.served_by_ring = objSubCatDDL[0].dropdown_value;

					}
					//SET NETWORK CODE
					objCabinetMaster.network_id = objNetworkCodeDetail.network_code;
					objCabinetMaster.sequence_id = objNetworkCodeDetail.sequence_id;

				}
				this.Validate(objCabinetMaster);
				if (ModelState.IsValid)
				{
					var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.Cabinet.ToString().ToUpper()).FirstOrDefault().layer_title;
					var isNew = objCabinetMaster.system_id > 0 ? false : true;
					var resultItem = new BLCabinet().SaveEntityCabinet(objCabinetMaster, objCabinetMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						string[] LayerName = { EntityType.Cabinet.ToString() };
						if (objCabinetMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objCabinetMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objCabinetMaster.objPM.status = ResponseStatus.OK.ToString();
							objCabinetMaster.objPM.isNewEntity = isNew;
							objCabinetMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							objCabinetMaster.objPM.systemId = resultItem.system_id;
							objCabinetMaster.objPM.entityType = resultItem.entityType;
							objCabinetMaster.objPM.structureId = objCabinetMaster.objIspEntityMap.structure_id;
							objCabinetMaster.objPM.shaftId = objCabinetMaster.objIspEntityMap.shaft_id ?? 0;
							objCabinetMaster.objPM.floorId = objCabinetMaster.objIspEntityMap.floor_id ?? 0;
							objCabinetMaster.objPM.pSystemId = objCabinetMaster.parent_system_id;
							objCabinetMaster.objPM.pEntityType = objCabinetMaster.parent_entity_type;
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objCabinetMaster.objPM.status = ResponseStatus.OK.ToString();
								objCabinetMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);//resultItem.message;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objCabinetMaster.system_id, EntityType.Cabinet.ToString(), objCabinetMaster.network_id, objCabinetMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objCabinetMaster.longitude + " " + objCabinetMaster.latitude }, objCabinetMaster.user_id);
								objCabinetMaster.objPM.status = ResponseStatus.OK.ToString();
								objCabinetMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName); ;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName); ;
							}
						}
					}
					else
					{
						objCabinetMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objCabinetMaster.objPM.message = objCabinetMaster.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = objCabinetMaster.objPM.message;
					}
				}
				else
				{
					objCabinetMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objCabinetMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objCabinetMaster.isDirectSave == true)
				{
					response.results = objCabinetMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objCabinetMaster, EntityType.Cabinet.ToString());
					// RETURN PARTIAL VIEW WITH MODEL DATA  
					BindISPCabinetDropDown(objCabinetMaster);
					fillProjectSpecifications(objCabinetMaster);
					response.results = objCabinetMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPCabinet()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion
		#endregion


		//cabinet shazia end  

		//vault shazia 
		#region Vault

		#region Vault Details
		public VaultMaster GetVaultDetail(VaultMaster objVault)
		{
			int user_id = objVault.user_id;
			if (objVault.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objVault, GeometryType.Point.ToString(), objVault.geom);
				//Fill Parent detail...              
				fillParentDetail(objVault, new NetworkCodeIn() { eType = EntityType.Vault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objVault.geom }, objVault.networkIdType);
				objVault.longitude = Convert.ToDecimal(objVault.geom.Split(' ')[0]);
				objVault.latitude = Convert.ToDecimal(objVault.geom.Split(' ')[1]);
				objVault.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<VaultTemplateMaster>(objVault.user_id, EntityType.Vault);
				MiscHelper.CopyMatchingProperties(objItem, objVault);
			}
			else
			{
				// Get entity detail by Id...
				objVault = new BLMisc().GetEntityDetailById<VaultMaster>(objVault.system_id, EntityType.Vault, objVault.user_id);
				fillRegionProvAbbr(objVault);
			}
			objVault.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objVault;
		}
		#endregion

		#region Add Vault

		public ApiResponse<VaultMaster> AddVault(ReqInput data)
		{
			var response = new ApiResponse<VaultMaster>();
			try
			{
				VaultMaster objRequestIn = ReqHelper.GetRequestData<VaultMaster>(data);
				if (string.IsNullOrWhiteSpace(objRequestIn.geom) && objRequestIn.system_id == 0)
				{

					objRequestIn.geom = GetPointTypeParentGeom(objRequestIn.pSystemId, objRequestIn.pEntityType);
				}
				VaultMaster objVaultMaster = GetVaultDetail(objRequestIn);
				objVaultMaster.pSystemId = objRequestIn.pSystemId;
				objVaultMaster.pEntityType = objRequestIn.pEntityType;
				BLItemTemplate.Instance.BindItemDropdowns(objVaultMaster, EntityType.Vault.ToString());
				fillProjectSpecifications(objVaultMaster);
				BindVaultDropDown(objVaultMaster);
				response.status = StatusCodes.OK.ToString();
				response.results = objVaultMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddVault()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind Vault Dropdown
		private void BindVaultDropDown(VaultMaster objVault)
		{
			var ispEntityMap = BLIspEntityMapping.Instance.GetIspEntityMapByStrucId(objVault.parent_system_id, objVault.system_id, EntityType.Vault.ToString());
			if (ispEntityMap != null)
			{
				objVault.objIspEntityMap.id = ispEntityMap.id;
				objVault.objIspEntityMap.floor_id = ispEntityMap.floor_id;
				objVault.objIspEntityMap.shaft_id = ispEntityMap.shaft_id;
				objVault.objIspEntityMap.structure_id = ispEntityMap.structure_id;
				objVault.objIspEntityMap.AssociateStructure = ispEntityMap.structure_id;
			}

			objVault.objIspEntityMap.AssoType = objVault.objIspEntityMap.shaft_id > 0 ? "Shaft" : (objVault.objIspEntityMap.floor_id > 0 ? "Floor" : "");
			objVault.objIspEntityMap.lstStructure = BLStructure.Instance.getStructureByBuffer(objVault.longitude + " " + objVault.latitude);
			if (objVault.objIspEntityMap.structure_id > 0)
			{
				var objDDL = new BLBDB().GetShaftFloorByStrucId(objVault.objIspEntityMap.structure_id);
				objVault.objIspEntityMap.lstShaft = objDDL.Where(m => m.isshaft == true).ToList();
				objVault.objIspEntityMap.lstFloor = objDDL.Where(m => m.isshaft == false).OrderByDescending(m => m.systemid).ToList();

				if (objVault.parent_entity_type == EntityType.UNIT.ToString())
				{
					objVault.objIspEntityMap.unitId = objVault.parent_system_id;
					//objVault.objIspEntityMap.AssoType = "";
				}
			}
			var layerMappings = new BLLayer().getLayerMapping(EntityType.UNIT.ToString());
			if (layerMappings.Count > 0 && layerMappings.Where(m => m.child_layer_name == EntityType.Vault.ToString()).FirstOrDefault() != null)
			{
				objVault.objIspEntityMap.isValidParent = true;
				objVault.objIspEntityMap.UnitList = BLISP.Instance.getAllParentInFloor(objVault.objIspEntityMap.structure_id, objVault.objIspEntityMap.floor_id ?? 0, EntityType.UNIT.ToString());
			}
			var layerDetails = new BLLayer().GetLayerDetails().Where(m => m.layer_name.ToUpper() == EntityType.Vault.ToString().ToUpper()).FirstOrDefault();
			if (layerDetails != null)
			{
				objVault.objIspEntityMap.isShaftElement = layerDetails.is_shaft_element;
				objVault.objIspEntityMap.isFloorElement = layerDetails.is_floor_element;
			}
			if (objVault.objIspEntityMap.entity_type == null) { objVault.objIspEntityMap.entity_type = EntityType.Vault.ToString(); }
			objVault.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objVault.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList(EntityType.Vault.ToString());
			objVault.listVaultType = obj_DDL.Where(x => x.dropdown_type == DropDownType.Vault_Type.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objVault.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//   objVault.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();

		}
		#endregion

		#region Save Vault
		public ApiResponse<VaultMaster> SaveVault(ReqInput data)
		{
			var response = new ApiResponse<VaultMaster>();
			VaultMaster objVaultMaster = ReqHelper.GetRequestData<VaultMaster>(data);
			try
			{
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objVaultMaster.geom) && objVaultMaster.system_id == 0)
				{
					objVaultMaster.geom = GetPointTypeParentGeom(objVaultMaster.pSystemId, objVaultMaster.pEntityType);
				}
				ModelState.Clear();
				if (objVaultMaster.networkIdType == NetworkIdType.A.ToString() && objVaultMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Vault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objVaultMaster.geom, parent_eType = objVaultMaster.pEntityType, parent_sysId = objVaultMaster.pSystemId });
					if (objVaultMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objVaultMaster = GetVaultDetail(objVaultMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objVaultMaster.vault_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//   var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objVaultMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						//objVaultMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objVaultMaster.network_id = objNetworkCodeDetail.network_code;
					objVaultMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objVaultMaster.vault_name))
				{
					objVaultMaster.vault_name = objVaultMaster.network_id;
				}
				this.Validate(objVaultMaster);
				if (ModelState.IsValid)
				{

					objVaultMaster.objIspEntityMap.structure_id = Convert.ToInt32(objVaultMaster.objIspEntityMap.AssociateStructure);
					if (objVaultMaster.objIspEntityMap.structure_id != 0)
					{
						var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objVaultMaster.objIspEntityMap.structure_id, EntityType.Structure);
						if (structureDetails != null)
						{
							objVaultMaster.latitude = Convert.ToDecimal(structureDetails.latitude);
							objVaultMaster.longitude = Convert.ToDecimal(structureDetails.longitude);
						}


					}
					objVaultMaster.objIspEntityMap.shaft_id = objVaultMaster.objIspEntityMap.AssoType == "Floor" ? 0 : objVaultMaster.objIspEntityMap.shaft_id;
					if (string.IsNullOrEmpty(objVaultMaster.objIspEntityMap.AssoType))
					{
						objVaultMaster.objIspEntityMap.shaft_id = 0; objVaultMaster.objIspEntityMap.floor_id = 0;
					}
					if (objVaultMaster.objIspEntityMap.structure_id == 0)
					{
						var objIn = new NetworkCodeIn() { eType = EntityType.Vault.ToString(), gType = GeometryType.Point.ToString(), eGeom = objVaultMaster.longitude + " " + objVaultMaster.latitude };
						var parentDetails = new BLMisc().getParentInfo(objIn);
						if (parentDetails != null)
						{
							objVaultMaster.parent_system_id = parentDetails.p_system_id;
							objVaultMaster.parent_network_id = parentDetails.p_network_id;
							objVaultMaster.parent_entity_type = parentDetails.p_entity_type;
						}
					}

					var isNew = objVaultMaster.system_id > 0 ? false : true;

					var resultItem = new BLVault().SaveEntityVault(objVaultMaster, objVaultMaster.user_id);
					//Save Reference
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						if (objVaultMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objVaultMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							string[] LayerName = { EntityType.Vault.ToString() };
							objVaultMaster.objPM.status = ResponseStatus.OK.ToString();
							objVaultMaster.objPM.isNewEntity = isNew;
							objVaultMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							//response.status = ResponseStatus.OK.ToString();
							//response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected)
							{
								objVaultMaster.objPM.status = ResponseStatus.OK.ToString();
								objVaultMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
							}
							else
							{
								string[] LayerName = { EntityType.Vault.ToString() };
								BLLoopMangment.Instance.UpdateLoopDetails(objVaultMaster.system_id, EntityType.Vault.ToString(), objVaultMaster.network_id, objVaultMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objVaultMaster.longitude + " " + objVaultMaster.latitude }, objVaultMaster.user_id);
								objVaultMaster.objPM.status = ResponseStatus.OK.ToString();
								objVaultMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								//response.status = ResponseStatus.OK.ToString();
								//response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					else
					{
						objVaultMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objVaultMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objVaultMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objVaultMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objVaultMaster.isDirectSave == true)
				{
					response.status = ResponseStatus.OK.ToString();
					response.results = objVaultMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objVaultMaster, EntityType.Vault.ToString());
					BindVaultDropDown(objVaultMaster);
					fillProjectSpecifications(objVaultMaster);
					response.results = objVaultMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveVault()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion

		//vault shazia end        

		#region Loop

		#region Loop Details
		public NELoopDetails GetLoopDetail(NELoopDetails objLoop)
		{
			if (objLoop.system_id == 0)
			{
				objLoop.longitude = Convert.ToDouble(objLoop.geom.Split(' ')[0]);
				objLoop.latitude = Convert.ToDouble(objLoop.geom.Split(' ')[1]);
				objLoop.lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(objLoop.longitude, objLoop.latitude, objLoop.associated_system_id, "Cable", objLoop.structure_id);
				//objLoop.networkIdType = networkIdType;

				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objLoop, GeometryType.Point.ToString(), objLoop.geom);
				//Fill Parent detail...              
				fillParentDetail(objLoop, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objLoop.geom }, objLoop.networkIdType);
			}
			else
			{
				// Get entity detail by Id...
				objLoop = new BLMisc().GetEntityDetailById<NELoopDetails>(objLoop.system_id, EntityType.Loop, objLoop.user_id);
				objLoop.lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(objLoop.longitude, objLoop.latitude, objLoop.associated_system_id, objLoop.associated_entity_type, objLoop.structure_id);
				CableMaster obj = new CableMaster();
				int cableId = Convert.ToInt32(objLoop.cable_id);
				obj = new BLCable().GetCableNameAndLengthForLoop(cableId);
				objLoop.cable_name = obj.cable_name;
				objLoop.cable_calculated_length = obj.cable_calculated_length;
				objLoop.total_loop_count = obj.total_loop_count;
				objLoop.available_calculated_length = (obj.cable_calculated_length - obj.total_loop_length);
				objLoop.total_loop_length = obj.total_loop_length;
				objLoop.associated_network_id = obj.network_id;
				fillRegionProvAbbr(objLoop);
			}
			return objLoop;
		}
		#endregion

		#region Add Loop

		public ApiResponse<NELoopDetails> AddLoop(ReqInput data)
		{
			var response = new ApiResponse<NELoopDetails>();
			try
			{
				NELoopDetails objRequestIn = ReqHelper.GetRequestData<NELoopDetails>(data);
				NELoopDetails objLoop = GetLoopDetail(objRequestIn);
				objLoop.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
				response.status = StatusCodes.OK.ToString();
				response.results = objLoop;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddLoop()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Save Loop
		public ApiResponse<NELoopDetails> SaveLoop(ReqInput data)
		{
			var response = new ApiResponse<NELoopDetails>();
			NELoopDetails objLoop = ReqHelper.GetRequestData<NELoopDetails>(data);
			try
			{
				ModelState.Clear();
				if (objLoop.system_id == 0)
				{
					objLoop.created_by = objLoop.user_id;
					objLoop.created_on = DateTimeHelper.Now;
					objLoop.associated_entity_type = "Cable";
					objLoop.longitude = Convert.ToDouble(objLoop.geom.Split(' ')[0]);
					objLoop.latitude = Convert.ToDouble(objLoop.geom.Split(' ')[1]);
					objLoop.associated_system_id = Convert.ToInt32(objLoop.cable_id);
					objLoop.cable_system_id = Convert.ToInt32(objLoop.cable_id);
					NetworkCodeIn objIn = new NetworkCodeIn();
					objIn.eType = "Loop"; objIn.eGeom = objLoop.geom; objIn.gType = "Point";
					var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
					objLoop.parent_entity_type = networkCodeDetail.parent_entity_type;
					objLoop.parent_network_id = networkCodeDetail.parent_network_id;
					objLoop.parent_system_id = networkCodeDetail.parent_system_id;
					objLoop.network_id = networkCodeDetail.network_code;
					objLoop.sequence_id = networkCodeDetail.sequence_id;
					
				}
				else
				{
					objLoop.modified_by = objLoop.user_id;
					objLoop.modified_on = DateTimeHelper.Now;
				}
				objLoop.objPM = null;
				objLoop.lstLoopMangment = null;
				this.Validate(objLoop);
				if (ModelState.IsValid)
				{
					var isNew = objLoop.system_id > 0 ? false : true;
					var resultItem = new BLLoop().SaveEntityLoop(JsonConvert.SerializeObject(objLoop), objLoop.province_id);
                    objLoop.system_id = resultItem.systemId;
					objLoop.objPM = new PageMessage();
					string[] LayerName = { EntityType.Loop.ToString() };
					if (resultItem.systemId > 0)
					{
						if (isNew)
						{
							objLoop.objPM.status = ResponseStatus.OK.ToString();
							objLoop.objPM.isNewEntity = isNew;
							objLoop.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							objLoop.objPM.status = ResponseStatus.OK.ToString();
							objLoop.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
					}
					else
					{
						objLoop.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
						objLoop.objPM.message = resultItem.message;
						response.error_message = resultItem.message;
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					}

				}
				else
				{
					objLoop.objPM.status = ResponseStatus.FAILED.ToString();
					objLoop.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objLoop.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objLoop;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					objLoop.lstLoopMangment = BLLoopMangment.Instance.GetLoopDetails(objLoop.longitude, objLoop.latitude, objLoop.associated_system_id, "Cable", objLoop.structure_id);
					objLoop.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
					response.results = objLoop;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveLoop()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion

		#region SPLIT CABLE
		/// <summary> SplitCable </summary>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		private void SplitCable(int system_id, int split_cable_system_id, string entityType, string network_status, int user_id)
		{
			SplitCable model = new SplitCable();
			int cableOneSystemid = 0;
			int cableTwoSystemid = 0;
			var cableDetail = new BLMisc().GetEntityDetailById<CableMaster>(split_cable_system_id, EntityType.Cable);
			model.old_cable_a_entity_type = cableDetail.a_entity_type;
			model.old_cable_a_system_id = cableDetail.a_system_id;
			model.old_cable_a_location = cableDetail.a_location;
			model.old_cable_b_entity_type = cableDetail.b_entity_type;
			model.old_cable_b_system_id = cableDetail.b_system_id;
			model.old_cable_b_location = cableDetail.b_location;
			model.parent_system_id = cableDetail.parent_system_id;
			model.parent_entity_type = cableDetail.parent_entity_type;
			model.parent_network_id = cableDetail.parent_network_id;

			var SplitCablesEntity = new BLMisc().getSplitCableEntity(system_id, entityType, split_cable_system_id, EntityType.Cable.ToString());
			var networkId = cableDetail.network_id;
			var firstCableNetworkId = cableDetail.network_id + "_01";

			var cableobjCable1 = getCableObject(1, model, cableDetail, SplitCablesEntity.cable1_a_location, cableDetail.network_id, Convert.ToSingle(SplitCablesEntity.cable1Length), Convert.ToSingle(SplitCablesEntity.cable1CalculatedLength), firstCableNetworkId, firstCableNetworkId, SplitCablesEntity.geom_cable1);
			cableobjCable1.a_system_id = SplitCablesEntity.cable1_a_system_id ?? 0;
			cableobjCable1.a_entity_type = SplitCablesEntity.cable1_a_entity_type;
			cableobjCable1.a_location = model.cable_one_a_location;

			cableobjCable1.b_system_id = SplitCablesEntity.cable1_b_system_id ?? 0;
			cableobjCable1.b_entity_type = SplitCablesEntity.cable1_b_entity_type;
			cableobjCable1.b_location = model.cable_one_b_location;
			cableobjCable1.bom_sub_category = "Proposed";
			BLCable.Instance.SaveCable(cableobjCable1, user_id);
			//SaveCable(cableobjCable1, "CableInfo", false);
			//2ndCable//
			cableOneSystemid = cableobjCable1.system_id;
			string[] geomObjLongLat = SplitCablesEntity.geom_cable2.Split(',');
			EditGeomIn geomObj = new EditGeomIn();
			geomObj.entityType = entityType;
			geomObj.geomType = "Point";
			geomObj.isExisting = true;
			geomObj.longLat = geomObjLongLat[0];
			geomObj.systemId = system_id;
			geomObj.networkStatus = network_status;
			geomObj.userId = user_id;
			BASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);

			var secondCableNetworkId = networkId + "_02";
			var cableobjCable2 = getCableObject(2, model, cableDetail, cableDetail.network_id, SplitCablesEntity.cable2_b_location, Convert.ToSingle(SplitCablesEntity.cable2Length), Convert.ToSingle(SplitCablesEntity.cable2CalculatedLength), secondCableNetworkId, secondCableNetworkId, SplitCablesEntity.geom_cable2);
			cableobjCable2.a_system_id = SplitCablesEntity.cable2_a_system_id ?? 0;
			cableobjCable2.a_entity_type = SplitCablesEntity.cable2_a_entity_type;
			cableobjCable2.a_location = model.cable_two_a_location;

			cableobjCable2.b_system_id = SplitCablesEntity.cable2_b_system_id ?? 0;
			cableobjCable2.b_entity_type = SplitCablesEntity.cable2_b_entity_type;
			cableobjCable2.b_location = model.cable_two_b_location;
			cableobjCable2.bom_sub_category = "Proposed";
			BLCable.Instance.SaveCable(cableobjCable2, user_id);
			cableTwoSystemid = cableobjCable2.system_id;
			BLCable.Instance.SetConnectionWithSplitCable(firstCableNetworkId, secondCableNetworkId, split_cable_system_id, system_id, cableDetail.network_id, entityType, user_id, model.splicing_source);
			// associate split cables
			new BLMisc().AssociateSplitEntities(cableOneSystemid, cableTwoSystemid, firstCableNetworkId, secondCableNetworkId, EntityType.Cable.ToString(), split_cable_system_id);
			// Delete parent cable
			new BLMisc().deleteParentSplitEntity(split_cable_system_id, EntityType.Cable.ToString());
		}

		public CableMaster getCableObject(int cableNo, SplitCable splitModle, CableMaster CableMaster, string cable_a_location, string cable_b_location, float? cable_measured_length, float? cable_calculated_length, string cable_name, string cable_network_id, string geom)
		{
			CableMaster newObj = new CableMaster();

			CableMaster.unitValue = Convert.ToString(CableMaster.total_core);
			CableMaster.system_id = 0;

			if (cableNo == 1)
			{
				//CableMaster.a_entity_type = splitModle.old_cable_a_entity_type;
				//CableMaster.a_system_id = splitModle.old_cable_a_system_id;
				//CableMaster.a_location = splitModle.old_cable_a_location;

				//CableMaster.b_entity_type = splitModle.split_entity_type;
				//CableMaster.b_system_id = splitModle.split_entity_system_id;
				//CableMaster.b_location = splitModle.split_entity_networkId;

				CableMaster.start_reading = splitModle.cable_one_start_reading;
				CableMaster.end_reading = splitModle.cable_one_end_reading;

			}
			else
			{
				// CableMaster.a_entity_type = splitModle.split_entity_type;
				//CableMaster.a_system_id = splitModle.split_entity_system_id;
				//CableMaster.a_location = splitModle.split_entity_networkId;

				//CableMaster.b_entity_type = splitModle.old_cable_b_entity_type;
				// CableMaster.b_system_id = splitModle.old_cable_b_system_id;
				//CableMaster.b_location = splitModle.old_cable_b_location;

				CableMaster.start_reading = splitModle.cable_two_start_reading;
				CableMaster.end_reading = splitModle.cable_two_end_reading;
			}

			var extraLengthForCable = (ApplicationSettings.CableExtraLengthPercentage * cable_measured_length) / 100;
			var cbl_calculated_length = Math.Round((double)((cable_measured_length) + extraLengthForCable), 3);

			CableMaster.cable_calculated_length = (float)cbl_calculated_length;
			CableMaster.cable_measured_length = (float)cable_measured_length;
			CableMaster.cable_name = cable_name;
			CableMaster.network_id = cable_network_id;
			CableMaster.geom = geom;

			return CableMaster;
		}
		#endregion

		#region Tower

		public TowerMaster GetTowerDetail(TowerMaster objTower)
		{
			if (objTower.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objTower, GeometryType.Point.ToString(), objTower.geom);
				//Fill Parent detail...              
				fillParentDetail(objTower, new NetworkCodeIn() { eType = EntityType.Tower.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTower.geom, parent_eType = objTower.pEntityType, parent_sysId = objTower.pSystemId }, objTower.networkIdType);
				objTower.longitude = Convert.ToDouble(objTower.geom.Split(' ')[0]);
				objTower.latitude = Convert.ToDouble(objTower.geom.Split(' ')[1]);
				objTower.ownership_type = "Own";
				// Item template binding
				var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.Antenna.ToString().ToUpper()).FirstOrDefault();
				if (layerDetails.is_template_required)
				{
					var objItem = BLItemTemplate.Instance.GetTemplateDetail<TowerTemplateMaster>(objTower.user_id, EntityType.Tower);
					Utility.MiscHelper.CopyMatchingProperties(objItem, objTower);
				}

			}
			else
			{
				// Get entity detail by Id...
				objTower = objBLMisc.GetEntityDetailById<TowerMaster>(objTower.system_id, EntityType.Tower);
				objTower.lstTowerAssociatedPop = new BLTowerAssociatedPop().GetAssociatedPop(objTower.system_id);
				fillRegionProvAbbr(objTower);


			}
			return objTower;
		}
		public ApiResponse<TowerMaster> AddTower(ReqInput data)
		{
			var response = new ApiResponse<TowerMaster>();
			try
			{
				TowerMaster objRequestIn = ReqHelper.GetRequestData<TowerMaster>(data);
				if (string.IsNullOrWhiteSpace(objRequestIn.geom) && objRequestIn.system_id == 0)
				{

					objRequestIn.geom = GetPointTypeParentGeom(objRequestIn.pSystemId, objRequestIn.pEntityType);
				}
				TowerMaster objTowerMaster = GetTowerDetail(objRequestIn);
				objTowerMaster.pSystemId = objRequestIn.pSystemId;
				objTowerMaster.pEntityType = objRequestIn.pEntityType;
				BLItemTemplate.Instance.BindItemDropdowns(objTowerMaster, EntityType.Tower.ToString());
				fillProjectSpecifications(objTowerMaster);
				BindAntennaDropDown(objTowerMaster);
				response.status = StatusCodes.OK.ToString();
				response.results = objTowerMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddTower()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		public void BindAntennaDropDown(TowerMaster objEntityMaster)
		{
			var objDDL = objBLMisc.GetDropDownList(EntityType.Building.ToString());
			objEntityMaster.lstTenancy = objDDL.Where(x => x.dropdown_type == DropDownType.Tenancy.ToString()).ToList();

			objEntityMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objEntityMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var obj_DDL = new BLMisc().GetDropDownList("");
			objEntityMaster.lstBOMSubCategory = obj_DDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			//  objEntityMaster.lstServedByRing = obj_DDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		public ApiResponse<TowerMaster> SaveTower(ReqInput data)
		{
			ModelState.Clear();
			PageMessage objPM = new PageMessage();
			var response = new ApiResponse<TowerMaster>();
			TowerMaster objTowerMaster = ReqHelper.GetRequestData<TowerMaster>(data);
			try
			{

				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objTowerMaster.geom) && objTowerMaster.system_id == 0)
				{
					objTowerMaster.geom = GetPointTypeParentGeom(objTowerMaster.pSystemId, objTowerMaster.pEntityType);
				}

				if (objTowerMaster.networkIdType == NetworkIdType.A.ToString() && objTowerMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Tower.ToString(), gType = GeometryType.Point.ToString(), eGeom = objTowerMaster.geom, parent_eType = objTowerMaster.pEntityType, parent_sysId = objTowerMaster.pSystemId });
					if (objTowerMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objTowerMaster = GetTowerDetail(objTowerMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objTowerMaster.network_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objTowerMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objTowerMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objTowerMaster.network_id = objNetworkCodeDetail.network_code;
					objTowerMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objTowerMaster.network_name))
				{
					objTowerMaster.network_name = objTowerMaster.network_id;
				}

				this.Validate(objTowerMaster);
				var isNew = objTowerMaster.system_id > 0 ? false : true;

				TowerMaster resultItem = new BLTower().Save(objTowerMaster, objTowerMaster.user_id);

				if (ModelState.IsValid)
				{

					if (String.IsNullOrEmpty(resultItem.objPM.message))
					{


						//Save Reference
						if (objTowerMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objTowerMaster.EntityReference, resultItem.system_id);
						}
						string[] LayerName = { EntityType.Tower.ToString() };
						if (isNew)
						{
							objPM.status = ResponseStatus.OK.ToString();
							objPM.isNewEntity = isNew;
							objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{

							objPM.status = ResponseStatus.OK.ToString();
							objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);

						}
						objTowerMaster.objPM = objPM;
					}
				}
				else
				{

					objPM.status = ResponseStatus.FAILED.ToString();
					objPM.message = new LibraryController().getFirstErrorFromModelState();
					objTowerMaster.objPM = objPM;
					response.error_message = resultItem.objPM.message;
					response.status = ResponseStatus.FAILED.ToString();
				}
				if (objTowerMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objTowerMaster;

				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objTowerMaster, EntityType.Splitter.ToString());
					BindAntennaDropDown(objTowerMaster);
					fillProjectSpecifications(objTowerMaster);
					response.results = objTowerMaster;

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveTower()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion

		//HANDHOLE ENTITY  BY ANTRA

		#region Handhole

		#region Add Handhole
		/// <summary> Add Handhole </summary>
		/// <param name="data">json data</param>
		/// <returns>Handhole Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		///<Created Date>11-Jan-2020</Created Date>
		public ApiResponse<HandholeMaster> AddHandhole(ReqInput data)
		{
			var response = new ApiResponse<HandholeMaster>();
			try
			{
				HandholeMaster objRequestIn = ReqHelper.GetRequestData<HandholeMaster>(data);
				HandholeMaster objHandholeMaster = GetHandholeDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objHandholeMaster, EntityType.Handhole.ToString());
				fillProjectSpecifications(objHandholeMaster);
				BindHandholeDropDown(objHandholeMaster);
				objHandholeMaster.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Handhole.ToString()).ToList();
				// Add additional attribute form
				var layerdetails = new BLLayer().getLayer(EntityType.Handhole.ToString());
				objHandholeMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
				//End additional attributes


				response.status = StatusCodes.OK.ToString();
				response.results = objHandholeMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddHandhole()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Save Handhole
		/// <summary> SaveHandhole </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		public ApiResponse<HandholeMaster> SaveHandhole(ReqInput data)
		{
			var response = new ApiResponse<HandholeMaster>();
			HandholeMaster objHandholeMaster = ReqHelper.GetRequestData<HandholeMaster>(data);
			try
			{
				if (objHandholeMaster.networkIdType == NetworkIdType.A.ToString() && objHandholeMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Handhole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHandholeMaster.geom, parent_eType = objHandholeMaster.pEntityType, parent_sysId = objHandholeMaster.pSystemId });
					if (objHandholeMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objHandholeMaster = GetHandholeDetail(objHandholeMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objHandholeMaster.handhole_name = objNetworkCodeDetail.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//   var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objHandholeMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objHandholeMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objHandholeMaster.network_id = objNetworkCodeDetail.network_code;
					objHandholeMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objHandholeMaster.handhole_name))
				{
					objHandholeMaster.handhole_name = objHandholeMaster.network_id;
				}
				this.Validate(objHandholeMaster);
				if (ModelState.IsValid)
				{
					var isNew = objHandholeMaster.system_id > 0 ? false : true;
					objHandholeMaster.is_new_entity = (isNew && objHandholeMaster.source_ref_id != "0" && objHandholeMaster.source_ref_id != "");
					var resultItem = new BLHandhole().SaveEntityHandhole(objHandholeMaster, objHandholeMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						//Save Reference
						string[] LayerName = { EntityType.Handhole.ToString() };
						if (objHandholeMaster.EntityReference != null && resultItem.system_id > 0)
						{
							SaveReference(objHandholeMaster.EntityReference, resultItem.system_id);
						}
						if (isNew)
						{
							objHandholeMaster.objPM.status = ResponseStatus.OK.ToString();
							objHandholeMaster.objPM.isNewEntity = isNew;
							objHandholeMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							BLLoopMangment.Instance.UpdateLoopDetails(objHandholeMaster.system_id, EntityType.Handhole.ToString(), objHandholeMaster.network_id, objHandholeMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHandholeMaster.longitude + " " + objHandholeMaster.latitude }, objHandholeMaster.user_id);
							objHandholeMaster.objPM.status = ResponseStatus.OK.ToString();
							objHandholeMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						objHandholeMaster.objPM = objHandholeMaster.objPM;
						//save AT Status                        
						if (objHandholeMaster.ATAcceptance != null && objHandholeMaster.system_id > 0)
						{
							SaveATAcceptance(objHandholeMaster.ATAcceptance, objHandholeMaster.system_id, objHandholeMaster.user_id);
						}
					}
					else
					{
						objHandholeMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objHandholeMaster.objPM.message = resultItem.objPM.message;
						response.error_message = resultItem.objPM.message;
						response.status = ResponseStatus.FAILED.ToString();
					}
				}
				else
				{
					objHandholeMaster.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objHandholeMaster.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objHandholeMaster.isDirectSave == true)
				{
					response.results = objHandholeMaster;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objHandholeMaster, EntityType.Handhole.ToString());
					fillProjectSpecifications(objHandholeMaster);
					BindHandholeDropDown(objHandholeMaster);
					var layerdetails = new BLLayer().getLayer(EntityType.Pole.ToString());
					objHandholeMaster.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
					objHandholeMaster.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Handhole.ToString()).ToList();
					response.results = objHandholeMaster;

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveHandhole()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}


		#endregion

		#region Bind Handhole Dropdown
		/// <summary>Bind Handhole DropDown </summary>
		/// <param name="objPoleMaster"></param>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		private void BindHandholeDropDown(HandholeMaster objHandholeMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.Handhole.ToString());
			objHandholeMaster.listConstructionType = objDDL.Where(x => x.dropdown_type == DropDownType.Construction_Type.ToString()).ToList();
			//objHandholeMaster.listOwnership = new BLMisc().GetDropDownList("", DropDownType.Ownership.ToString());
			objHandholeMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objHandholeMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objHandholeMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objHandholeMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#region Get Handhole Details
		/// <summary> Get Handhole </summary>
		/// <returns>Get Handhole Details</returns>
		/// <CreatedBy>Sumit Poonia</CreatedBy>
		///<Created Date>11-Jan-2020</Created Date>
		public HandholeMaster GetHandholeDetail(HandholeMaster objHandhole)
		{
			int id = objHandhole.user_id;
			if (objHandhole.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objHandhole, GeometryType.Point.ToString(), objHandhole.geom);
				//Fill Parent detail...              
				fillParentDetail(objHandhole, new NetworkCodeIn() { eType = EntityType.Handhole.ToString(), gType = GeometryType.Point.ToString(), eGeom = objHandhole.geom }, objHandhole.networkIdType);
				objHandhole.longitude = Convert.ToDouble(objHandhole.geom.Split(' ')[0]);
				objHandhole.latitude = Convert.ToDouble(objHandhole.geom.Split(' ')[1]);
				objHandhole.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<HandholeTemplateMaster>(objHandhole.user_id, EntityType.Handhole);
				// Add additional attribute form
				objHandhole.other_info = null;
				//End additional attributes
				MiscHelper.CopyMatchingProperties(objItem, objHandhole);
			}
			else
			{
				// Get entity detail by Id...
				objHandhole = new BLMisc().GetEntityDetailById<HandholeMaster>(objHandhole.system_id, EntityType.Handhole, objHandhole.user_id);
				// Add additional attribute form
				objHandhole.other_info = new BLHandhole().GetOtherInfoHandhole(objHandhole.system_id);
				//End additional attributes


				fillRegionProvAbbr(objHandhole);
			}
			objHandhole.lstUserModule = new BLLayer().GetUserModuleAbbrList(id, UserType.Web.ToString());
			return objHandhole;
		}
		#endregion

		#endregion

		//PATCHPANEL BY SHAZIA
		#region PatchPanel
		#region Add PatchPanel

		/// <returns>PatchPanel Details</returns>
		/// <CreatedBy>shazia barkat</CreatedBy>

		public ApiResponse<PatchPanelMaster> AddPatchPanel(ReqInput data)
		{
			var response = new ApiResponse<PatchPanelMaster>();
			try
			{
				PatchPanelMaster objPatchPanel = ReqHelper.GetRequestData<PatchPanelMaster>(data);
				if (string.IsNullOrWhiteSpace(objPatchPanel.geom) && objPatchPanel.system_id == 0)
				{
					// get geom by parent id...
					objPatchPanel.geom = GetPointTypeParentGeom(objPatchPanel.pSystemId, objPatchPanel.pEntityType);
				}
				PatchPanelMaster objPatchPanelMaster = GetPatchPanelDetail(objPatchPanel);

				BLItemTemplate.Instance.BindItemDropdowns(objPatchPanelMaster, EntityType.PatchPanel.ToString());
				BindPatchPanelDropDown(objPatchPanelMaster);
				fillProjectSpecifications(objPatchPanelMaster);
				new BLMisc().BindPortDetails(objPatchPanelMaster, EntityType.PatchPanel.ToString(), DropDownType.PatchPanel_Port_Ratio.ToString());
				objPatchPanelMaster.pNetworkId = objPatchPanel.pNetworkId;
				response.status = StatusCodes.OK.ToString();
				response.results = objPatchPanelMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddPatchPanel()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Get PatchPanel Details

		/// <returns>PatchPanel Details</returns>
		/// <CreatedBy>shazia barkat</CreatedBy>
		public PatchPanelMaster GetPatchPanelDetail(PatchPanelMaster objPatchPanel)
		{
			int user_id = objPatchPanel.user_id;
			if (objPatchPanel.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objPatchPanel, GeometryType.Point.ToString(), objPatchPanel.geom);
				//Fill Parent detail...              
				fillParentDetail(objPatchPanel, new NetworkCodeIn() { eType = EntityType.PatchPanel.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPatchPanel.geom, parent_sysId = objPatchPanel.pSystemId, parent_eType = objPatchPanel.pEntityType }, objPatchPanel.networkIdType);
				objPatchPanel.longitude = Convert.ToDouble(objPatchPanel.geom.Split(' ')[0]);
				objPatchPanel.latitude = Convert.ToDouble(objPatchPanel.geom.Split(' ')[1]);
				objPatchPanel.ownership_type = "Own";
				// Item template binding
				var objItem = BLItemTemplate.Instance.GetTemplateDetail<PatchPanelTemplateMaster>(objPatchPanel.user_id, EntityType.PatchPanel);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objPatchPanel);
			}
			else
			{
				// Get entity detail by Id...
				objPatchPanel = new BLMisc().GetEntityDetailById<PatchPanelMaster>(objPatchPanel.system_id, EntityType.PatchPanel, objPatchPanel.user_id);
				fillRegionProvAbbr(objPatchPanel);

			}
			objPatchPanel.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objPatchPanel;
		}

		#endregion

		#region Save PatchPanel

		/// <CreatedBy>shazia barkat</CreatedBy>
		public ApiResponse<PatchPanelMaster> SavePatchPanel(ReqInput data)
		{
			var response = new ApiResponse<PatchPanelMaster>();
			PatchPanelMaster objPatchPanelMaster = ReqHelper.GetRequestData<PatchPanelMaster>(data);
			try
			{
				int pSystemId = objPatchPanelMaster.pSystemId;
				string pEntitytype = objPatchPanelMaster.pEntityType;
				string pNetworkId = objPatchPanelMaster.pNetworkId;
				// get parent geometry 
				if (string.IsNullOrWhiteSpace(objPatchPanelMaster.geom) && objPatchPanelMaster.system_id == 0)
				{
					objPatchPanelMaster.geom = GetPointTypeParentGeom(objPatchPanelMaster.pSystemId, objPatchPanelMaster.pEntityType);
				}

				ModelState.Clear();
				if (objPatchPanelMaster.networkIdType == NetworkIdType.A.ToString() && objPatchPanelMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.PatchPanel.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPatchPanelMaster.geom, parent_sysId = pSystemId, parent_eType = pEntitytype });
					if (objPatchPanelMaster.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISESET REGION PROVINCE DETAILS..
						objPatchPanelMaster = GetPatchPanelDetail(objPatchPanelMaster);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objPatchPanelMaster.patchpanel_name = objNetworkCodeDetail.network_code;
						objPatchPanelMaster.pSystemId = pSystemId;
						objPatchPanelMaster.pEntityType = pEntitytype;
						objPatchPanelMaster.pNetworkId = pNetworkId;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						// var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objPatchPanelMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objPatchPanelMaster.served_by_ring = objSubCatDDL[0].dropdown_value;
					}
					//SET NETWORK CODE
					objPatchPanelMaster.network_id = objNetworkCodeDetail.network_code;
					objPatchPanelMaster.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (objPatchPanelMaster.unitValue != null && objPatchPanelMaster.unitValue.Contains(":"))
				{
					objPatchPanelMaster.no_of_input_port = Convert.ToInt32(objPatchPanelMaster.unitValue.Split(':')[0]);
					objPatchPanelMaster.no_of_output_port = Convert.ToInt32(objPatchPanelMaster.unitValue.Split(':')[1]);
				}
				if (string.IsNullOrEmpty(objPatchPanelMaster.patchpanel_name))
				{
					objPatchPanelMaster.patchpanel_name = objPatchPanelMaster.network_id;
				}
				this.Validate(objPatchPanelMaster);
				if (ModelState.IsValid)
				{
					var isNew = objPatchPanelMaster.system_id > 0 ? false : true;

					var resultItem = new BLPatchPanel().SaveEntityPatchPanel(objPatchPanelMaster, objPatchPanelMaster.user_id);

					var layerDetails = ApplicationSettings.listLayerDetails.Where(m => m.layer_name.ToUpper() == EntityType.PatchPanel.ToString().ToUpper()).FirstOrDefault();
					if (objPatchPanelMaster.split_cable_system_id > 0 && layerDetails.is_split_allowed == true && isNew)
					{
						SplitCable(objPatchPanelMaster.system_id, objPatchPanelMaster.split_cable_system_id, "PatchPanel", objPatchPanelMaster.network_status, objPatchPanelMaster.user_id);
						string[] Layer_Name = { EntityType.Cable.ToString() };
						objPatchPanelMaster.objPM.status = ResponseStatus.OK.ToString();
						objPatchPanelMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
						response.status = ResponseStatus.OK.ToString();
						response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_NET_FRM_177, ApplicationSettings.listLayerDetails, Layer_Name);
					}
					// END //
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.PatchPanel.ToString() };

						if (isNew)
						{
							objPatchPanelMaster.objPM.status = ResponseStatus.OK.ToString();
							objPatchPanelMaster.objPM.isNewEntity = isNew;
							objPatchPanelMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objPatchPanelMaster.objPM.status = ResponseStatus.OK.ToString();
								objPatchPanelMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = ResponseStatus.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
							}
							else
							{
								objPatchPanelMaster.objPM.status = ResponseStatus.OK.ToString();
								objPatchPanelMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
								response.status = ResponseStatus.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							}
						}
					}
					//else
					//{
					//    objPatchPanelMaster.objPM.status = ResponseStatus.FAILED.ToString();
					//    objPatchPanelMaster.objPM.message = objPatchPanelMaster.objPM.message;
					//    response.status = ResponseStatus.FAILED.ToString();
					//    response.error_message = objPatchPanelMaster.objPM.message;
					//}
				}
				else
				{
					objPatchPanelMaster.objPM.status = ResponseStatus.FAILED.ToString();
					objPatchPanelMaster.objPM.message = getFirstErrorFromModelState();
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
				}
				if (objPatchPanelMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = ResponseStatus.OK.ToString();
					response.results = objPatchPanelMaster;

				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objPatchPanelMaster, EntityType.PatchPanel.ToString());
					BindPatchPanelDropDown(objPatchPanelMaster);
					// RETURN PARTIAL VIEW WITH MODEL DATA
					fillProjectSpecifications(objPatchPanelMaster);
					new BLMisc().BindPortDetails(objPatchPanelMaster, EntityType.PatchPanel.ToString(), DropDownType.PatchPanel_Port_Ratio.ToString());
					response.results = objPatchPanelMaster;

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SavePatchPanel()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind PatchPanel Dropdown

		/// <param name="objPatchPanelMaster"></param>
		/// <CreatedBy>shazia barkat</CreatedBy>
		private void BindPatchPanelDropDown(PatchPanelMaster objPatchPanelMaster)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.PatchPanel.ToString());
			objPatchPanelMaster.listPatchPanelType = objDDL.Where(x => x.dropdown_type == DropDownType.PatchPanel_type.ToString()).ToList();
			objPatchPanelMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objPatchPanelMaster.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objPatchPanelMaster.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objPatchPanelMaster.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
		}
		#endregion

		#endregion
		#region ISP PatchPanel

		#region ISP PatchPanel Details

		/// <returns>ISP PatchPanel Details</returns>
		/// <CreatedBy>shazia barkat</CreatedBy>
		public PatchPanelMaster GetISPPatchPanelDetail(PatchPanelMaster objPatchPanel)
		{
			if (objPatchPanel.system_id != 0)
			{
				objPatchPanel = new BLMisc().GetEntityDetailById<PatchPanelMaster>(objPatchPanel.system_id, EntityType.PatchPanel, objPatchPanel.user_id);
				objPatchPanel.no_of_port = objPatchPanel.no_of_port;
				objPatchPanel.geom = objPatchPanel.longitude + " " + objPatchPanel.latitude;
				var ispMapping = new BLISP().getMappingByEntityId(objPatchPanel.system_id, EntityType.PatchPanel.ToString());
				if (ispMapping != null && ispMapping.id > 0)
				{
					objPatchPanel.objIspEntityMap.shaft_id = ispMapping.shaft_id;
					objPatchPanel.objIspEntityMap.floor_id = ispMapping.floor_id;
					objPatchPanel.objIspEntityMap.structure_id = ispMapping.structure_id;
				}
				fillRegionProvAbbr(objPatchPanel);
			}
			else
			{
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objPatchPanel.objIspEntityMap.structure_id, EntityType.Structure);
				objPatchPanel.geom = structureDetails.longitude + " " + structureDetails.latitude;
				//NEW ENTITY->Fill Region and Province Detail..
				fillISPRegionProvinceDetail(objPatchPanel, structureDetails, GeometryType.Point.ToString());
				//Fill Parent detail...              
				fillISPParentDetail(objPatchPanel, new ISPNetworkCodeIn() { parent_sysId = objPatchPanel.pSystemId, parent_eType = objPatchPanel.pEntityType, parent_networkId = objPatchPanel.pNetworkId, eType = EntityType.PatchPanel.ToString(), structureId = objPatchPanel.objIspEntityMap.structure_id }, objPatchPanel.networkIdType, objPatchPanel.pSystemId, objPatchPanel.pEntityType, objPatchPanel.pNetworkId);

				var objItem = BLItemTemplate.Instance.GetTemplateDetail<PatchPanelTemplateMaster>(objPatchPanel.user_id, EntityType.PatchPanel);
				Utility.MiscHelper.CopyMatchingProperties(objItem, objPatchPanel);
				objPatchPanel.no_of_port = objItem.no_of_port;

				objPatchPanel.address = BLStructure.Instance.getBuildingAddress(objPatchPanel.objIspEntityMap.structure_id);
				objPatchPanel.ownership_type = "Own";
			}
			return objPatchPanel;
		}

		#endregion

		#region Add ISP PatchPanel

		/// <returns>PatchPanel Details</returns>
		/// <CreatedBy>shazia barkat</CreatedBy>

		public ApiResponse<PatchPanelMaster> AddISPPatchPanel(ReqInput data)
		{
			var response = new ApiResponse<PatchPanelMaster>();
			try
			{
				PatchPanelMaster objRequestIn = ReqHelper.GetRequestData<PatchPanelMaster>(data);
				PatchPanelMaster objPatchPanelMaster = GetISPPatchPanelDetail(objRequestIn);
				BLItemTemplate.Instance.BindItemDropdowns(objPatchPanelMaster, EntityType.PatchPanel.ToString());
				BindISPPatchPanelDropdown(objPatchPanelMaster);
				fillProjectSpecifications(objPatchPanelMaster);
				new BLMisc().BindPortDetails(objPatchPanelMaster, EntityType.PatchPanel.ToString(), DropDownType.PatchPanel_Port_Ratio.ToString());
				response.status = StatusCodes.OK.ToString();
				response.results = objPatchPanelMaster;

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddISPPatchPanel()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region Bind ISP PatchPanel Dropdown

		/// <returns>ISP PatchPanel Details</returns>
		/// <CreatedBy>shazia barkat</CreatedBy>
		private void BindISPPatchPanelDropdown(PatchPanelMaster objPatchPanel)
		{
			var objDDL = new BLMisc().GetDropDownList(EntityType.PatchPanel.ToString());
			objPatchPanel.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
			objPatchPanel.listOwnVendorId = BLCable.Instance.GetAllVendorType(VendorType.Own.ToString()).ToList();
			objPatchPanel.listPatchPanelType = objDDL.Where(x => x.dropdown_type == DropDownType.PatchPanel_type.ToString()).ToList();
			var _objDDL = new BLMisc().GetDropDownList("");
			objPatchPanel.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
			// objPatchPanel.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();

		}
		#endregion

		#region Save ISP PatchPanel
		/// <summary> SaveISPPatchPanel </summary>
		/// <param name="data">Json ReqInput</param>
		/// <CreatedBy>shazia barkat</CreatedBy>
		public ApiResponse<PatchPanelMaster> SaveISPPatchPanel(ReqInput data)
		{
			var response = new ApiResponse<PatchPanelMaster>();
			PatchPanelMaster objPatchPanelMaster = ReqHelper.GetRequestData<PatchPanelMaster>(data);
			try
			{
				string pNetworkId = objPatchPanelMaster.pNetworkId;
				ModelState.Clear();
				int structure_id = objPatchPanelMaster.objIspEntityMap.structure_id;
				int pSystemId = objPatchPanelMaster.pSystemId;
				string pEntityType = objPatchPanelMaster.pEntityType;
				int floor_id = objPatchPanelMaster.objIspEntityMap.floor_id ?? 0;
				int shaft_id = objPatchPanelMaster.objIspEntityMap.shaft_id ?? 0;
				var structureDetails = new BLMisc().GetEntityDetailById<StructureMaster>(objPatchPanelMaster.structure_id, EntityType.Structure);

				if (objPatchPanelMaster.networkIdType == NetworkIdType.A.ToString() && objPatchPanelMaster.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objISPNetworkCode = new BLMisc().GetISPNetworkCodeDetail(new ISPNetworkCodeIn() { parent_sysId = pSystemId, parent_eType = pEntityType, eType = EntityType.PatchPanel.ToString(), structureId = objPatchPanelMaster.objIspEntityMap.structure_id });
					if (objPatchPanelMaster.isDirectSave == true)
					{
						objPatchPanelMaster = GetISPPatchPanelDetail(objPatchPanelMaster);

						objPatchPanelMaster.patchpanel_name = objISPNetworkCode.network_code;
						var objBOMDDL = new BLMisc().GetDropDownList("", "bom_sub_category");
						//  var objSubCatDDL = new BLMisc().GetDropDownList("", "served_by_ring");
						objPatchPanelMaster.bom_sub_category = objBOMDDL[0].dropdown_value;
						// objPatchPanelMaster.served_by_ring = objSubCatDDL[0].dropdown_value;

					}
					objPatchPanelMaster.network_id = objISPNetworkCode.network_code;
					objPatchPanelMaster.sequence_id = objISPNetworkCode.sequence_id;

				}
				if (structureDetails != null)
				{
					objPatchPanelMaster.region_id = structureDetails.region_id;
					objPatchPanelMaster.province_id = structureDetails.province_id;
					objPatchPanelMaster.region_name = structureDetails.region_name;
					objPatchPanelMaster.province_name = structureDetails.province_name;
					objPatchPanelMaster.latitude = structureDetails.latitude;
					objPatchPanelMaster.longitude = structureDetails.longitude;
				}
				if (objPatchPanelMaster.unitValue != null && objPatchPanelMaster.unitValue.Contains(":"))
				{
					objPatchPanelMaster.no_of_input_port = Convert.ToInt32(objPatchPanelMaster.unitValue.Split(':')[0]);
					objPatchPanelMaster.no_of_output_port = Convert.ToInt32(objPatchPanelMaster.unitValue.Split(':')[1]);
				}
				this.Validate(objPatchPanelMaster);
				if (ModelState.IsValid)
				{
					bool isNew = objPatchPanelMaster.system_id == 0 ? true : false;
					var resultItem = new BLPatchPanel().SaveEntityPatchPanel(objPatchPanelMaster, objPatchPanelMaster.user_id);
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.PatchPanel.ToString() };
						if (isNew)
						{
							objPatchPanelMaster.objPM.status = ResponseStatus.OK.ToString(); ;
							objPatchPanelMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							objPatchPanelMaster.objPM.systemId = resultItem.system_id;
							objPatchPanelMaster.objPM.entityType = EntityType.PatchPanel.ToString();
							objPatchPanelMaster.objPM.NetworkId = resultItem.network_id;
							objPatchPanelMaster.objPM.structureId = objPatchPanelMaster.objIspEntityMap.structure_id;
							objPatchPanelMaster.objPM.shaftId = objPatchPanelMaster.objIspEntityMap.shaft_id ?? 0;
							objPatchPanelMaster.objPM.floorId = objPatchPanelMaster.objIspEntityMap.floor_id ?? 0;
							objPatchPanelMaster.objPM.pSystemId = objPatchPanelMaster.parent_system_id;
							objPatchPanelMaster.objPM.pEntityType = objPatchPanelMaster.parent_entity_type;
							response.status = StatusCodes.OK.ToString();
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.results = objPatchPanelMaster;
						}
						else
						{
							if (resultItem.isPortConnected == true)
							{
								objPatchPanelMaster.objPM.status = ResponseStatus.OK.ToString();
								objPatchPanelMaster.objPM.message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message); //resultItem.message;
								response.status = StatusCodes.OK.ToString();
								response.error_message = BLConvertMLanguage.MultilingualMessageConvert(resultItem.message);
								response.results = objPatchPanelMaster;
							}
							else
							{
								BLLoopMangment.Instance.UpdateLoopDetails(objPatchPanelMaster.system_id, EntityType.PatchPanel.ToString(), objPatchPanelMaster.network_id, objPatchPanelMaster.lstLoopMangment, new NetworkCodeIn() { eType = EntityType.Loop.ToString(), gType = GeometryType.Point.ToString(), eGeom = objPatchPanelMaster.longitude + " " + objPatchPanelMaster.latitude }, objPatchPanelMaster.user_id);
								objPatchPanelMaster.objPM.status = ResponseStatus.OK.ToString(); ;
								objPatchPanelMaster.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "FMS updated successfully.";
								response.status = StatusCodes.OK.ToString();
								response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);// "FMS updated successfully.";
								response.results = objPatchPanelMaster;
							}
						}
					}
					else
					{
						objPatchPanelMaster.objPM.status = ResponseStatus.FAILED.ToString();
						objPatchPanelMaster.objPM.message = getFirstErrorFromModelState();
						response.status = StatusCodes.FAILED.ToString();
						response.error_message = getFirstErrorFromModelState();
						response.results = objPatchPanelMaster;
					}
				}
				else
				{
					objPatchPanelMaster.objPM.status = ResponseStatus.FAILED.ToString();
					objPatchPanelMaster.objPM.message = getFirstErrorFromModelState();
					response.status = StatusCodes.FAILED.ToString();
					response.error_message = getFirstErrorFromModelState();
					response.results = objPatchPanelMaster;
				}
				objPatchPanelMaster.entityType = EntityType.PatchPanel.ToString();
				if (objPatchPanelMaster.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.status = StatusCodes.OK.ToString();
					response.results = objPatchPanelMaster;
				}
				else
				{
					BLItemTemplate.Instance.BindItemDropdowns(objPatchPanelMaster, EntityType.PatchPanel.ToString());
					BindISPPatchPanelDropdown(objPatchPanelMaster);
					fillProjectSpecifications(objPatchPanelMaster);
					new BLMisc().BindPortDetails(objPatchPanelMaster, EntityType.PatchPanel.ToString(), DropDownType.PatchPanel_Port_Ratio.ToString());
					objPatchPanelMaster.pNetworkId = pNetworkId;
					response.results = objPatchPanelMaster;
				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveISPPatchPanel()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion
		#endregion
		// END

		#region Get Additional Attribute
		public vm_dynamic_form GetAdditionalAttributesForm(int layer_id)
		{
			BLDynamicAttributes objBLAdditionalAttributes = new BLDynamicAttributes();
			vm_dynamic_form objDynamicControls = new vm_dynamic_form();
			DynamicFormStyles cssStyle = new DynamicFormStyles();
			DynamicFormStyles labelStyle = new DynamicFormStyles();
			objDynamicControls.lstFormControls = objBLAdditionalAttributes.GetDynanicControlsById(layer_id);
			List<DynamicFormStyles> lstStyles = commonUtil.CreateListFromTable<DynamicFormStyles>(objBLAdditionalAttributes.GetDynamicFormStyle());
			foreach (DynamicControls dc in objDynamicControls.lstFormControls)
			{
				//Enum.TryParse(dc.control_type, out DynamicControlsType controlType);
				DynamicControlsType controlType = (DynamicControlsType)Enum.Parse(typeof(DynamicControlsType), dc.control_type, true);
				switch (controlType)
				{
					case DynamicControlsType.TEXT:
						cssStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.TEXT.ToString()).FirstOrDefault();
						dc.control_css_class = cssStyle.css_class;
						labelStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.LABEL.ToString()).FirstOrDefault();
						dc.label_css_class = labelStyle.css_class;
						break;
					case DynamicControlsType.DROPDOWN:
						cssStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.DROPDOWN.ToString()).FirstOrDefault();
						dc.control_css_class = cssStyle.css_class;
						labelStyle = lstStyles.Where(m => m.control_type == DynamicControlsType.LABEL.ToString()).FirstOrDefault();
						dc.label_css_class = labelStyle.css_class;
						break;
				}

			}

			objDynamicControls.lstFormControlsChunk = objDynamicControls.lstFormControls.ToChunks(2).ToList();
			var controlIds = objDynamicControls.lstFormControls.Where(x => x.control_type == DynamicControlsType.DROPDOWN.ToString()).Select(x => x.id).ToArray();
			objDynamicControls.lstFormDDLValues = objBLAdditionalAttributes.GetDDLByControlsId(controlIds);
			return objDynamicControls;
		}
		#endregion
		private void fillRegionProvAbbr(dynamic objEntityModel)
		{
			List<InRegionProvince> objRegionProvince = new List<InRegionProvince>();
			objRegionProvince = BLBuilding.Instance.GetRegionProvinceById(objEntityModel.region_id, objEntityModel.province_id);
			objEntityModel.region_abbreviation = objRegionProvince[0].region_abbreviation;
			objEntityModel.province_abbreviation = objRegionProvince[0].province_abbreviation;
		}

		#region Group Library
		public ApiResponse<List<GroupLibrary>> SaveGroupLibraryEntitys(ReqInput data)
		{
			var response = new ApiResponse<List<GroupLibrary>>();

			Root objGroupLibrary = ReqHelper.GetRequestData<Root>(data);
			try
			{
				List<Properties> lstProperties = new List<Properties>();
				foreach (var item in objGroupLibrary.features)
				{
					Properties objproberties = new Properties();
					if (item.properties.user_id == 0)
						item.properties.user_id = objGroupLibrary.user_id;
					objproberties.display_name = item.properties.display_name;
					objproberties.network_id = item.properties.network_id;
					objproberties.library_id = item.properties.library_id;
					objproberties.line_id = item.properties.line_id;
					objproberties.network_name = item.properties.network_name;
					objproberties.node_type = item.properties.node_type;
					objproberties.system_id = item.properties.system_id;
					objproberties.termination_type = item.properties.termination_type;
					objproberties.user_id = item.properties.user_id;
					objproberties.type = item.geometry.type;
					objproberties.geom = item.geometry.coordinates[0].ToString();
					objproberties.feature_type = item.type;
                    objproberties.source_ref_id = item.properties.source_ref_id;
                    objproberties.source_ref_type = item.properties.source_ref_type;
                    lstProperties.Add(objproberties);
				}

				List<GroupLibrary> lstGroupLibrary = new BLMisc().SaveGroupLibraryEntity(lstProperties);
				var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == lstGroupLibrary[0].entity_type.ToUpper()).FirstOrDefault().layer_title;
				if (lstGroupLibrary[0].status)
				{
					response.status = ResponseStatus.OK.ToString();
					response.error_message = String.Format("{0} saved successfully.", layer_title);//string.Format(Resources.Resources.SI_OSP_GBL_GBL_FRM_181, layer_title);
				}
				else
				{
					response.status = ResponseStatus.FAILED.ToString();
					response.error_message = "Something went wrong while Processing  Request.";
				}

				response.results = lstGroupLibrary;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveGroupLibraryEntitys()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
		#endregion

		#region to add splitter in FAT & FDB BOX BY ANTRA
		public void AddSplitterInBox(dynamic resultItem)
		{
			SplitterMaster ObjSpl = new SplitterMaster();
			ObjSpl.addSplitterInBox = 1;
			ObjSpl.isDirectSave = true;
			ObjSpl.networkIdType = "A";
			ObjSpl.pSystemId = resultItem.system_id;
			ObjSpl.pEntityType = resultItem.entityType;
			ObjSpl.user_id = resultItem.user_id;
			ObjSpl.system_id = 0;
			ObjSpl.geom = resultItem.geom;
			var objSplSpecf = new BLVendorSpecification().GetSplitterPortRatio();
			if (objSplSpecf.Count > 0)
			{
				var objItemPriSpl = objSplSpecf.Where(x => x.specify_type.ToUpper() == "PRIMARY").FirstOrDefault();
				if (objItemPriSpl != null && ObjSpl.pEntityType == EntityType.BDB.ToString())
				{
					ObjSpl.splitter_type = objItemPriSpl.specify_type;
					ObjSpl.vendor_id = objItemPriSpl.vendor_id;
					ObjSpl.specification = objItemPriSpl.specification;
					ObjSpl.subcategory1 = objItemPriSpl.subcategory_1;
					ObjSpl.subcategory2 = objItemPriSpl.subcategory_2;
					ObjSpl.subcategory3 = objItemPriSpl.subcategory_3;
					ObjSpl.no_of_port = objItemPriSpl.no_of_port;
					ObjSpl.category = objItemPriSpl.category_reference;
					ObjSpl.item_code = objItemPriSpl.code;
					ObjSpl.splitter_ratio = objItemPriSpl.iop_value;
				}
				var objItemSecSpl = objSplSpecf.Where(x => x.specify_type.ToUpper() == "SECONDARY").FirstOrDefault();
				if (objItemSecSpl != null && ObjSpl.pEntityType == EntityType.FDB.ToString())
				{
					ObjSpl.splitter_type = objItemSecSpl.specify_type;
					ObjSpl.vendor_id = objItemSecSpl.vendor_id;
					ObjSpl.specification = objItemSecSpl.specification;
					ObjSpl.subcategory1 = objItemSecSpl.subcategory_1;
					ObjSpl.subcategory2 = objItemSecSpl.subcategory_2;
					ObjSpl.subcategory3 = objItemSecSpl.subcategory_3;
					ObjSpl.no_of_port = objItemSecSpl.no_of_port;
					ObjSpl.category = objItemSecSpl.category_reference;
					ObjSpl.item_code = objItemSecSpl.code;
					ObjSpl.splitter_ratio = objItemSecSpl.iop_value;
				}

			}
			ReqInput obj = new ReqInput();
			obj.data = JsonConvert.SerializeObject(ObjSpl);
			SaveSplitter(obj);
		}
		#endregion

		public ApiResponse<Models.SurveyArea> AddSurveyArea(ReqInput data)
		{
			var response = new ApiResponse<Models.SurveyArea>();
			try
			{
				Models.SurveyArea objRequestIn = ReqHelper.GetRequestData<Models.SurveyArea>(data);
				Models.SurveyArea objSurveyArea = GetSurveyAreaDetail(objRequestIn);
				response.status = StatusCodes.OK.ToString();
				response.results = objSurveyArea;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("AddSurveyArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}

		public Models.SurveyArea GetSurveyAreaDetail(Models.SurveyArea objSurveyArea)
		{
			int user_id = objSurveyArea.user_id;
			if (objSurveyArea.system_id == 0)
			{
				//NEW ENTITY->Fill Region and Province Detail..
				fillRegionProvinceDetail(objSurveyArea, GeometryType.Polygon.ToString(), objSurveyArea.geom);
				//Fill Parent detail...              
				fillParentDetail(objSurveyArea, new NetworkCodeIn() { eType = EntityType.SurveyArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSurveyArea.geom }, objSurveyArea.networkIdType);
			}
			else
			{
				// Get entity detail by Id...
				objSurveyArea = new BLMisc().GetEntityDetailById<Models.SurveyArea>(objSurveyArea.system_id, EntityType.SurveyArea);
			}
			objSurveyArea.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());
			return objSurveyArea;
		}

		public ApiResponse<Models.SurveyArea> SaveSurveyArea(ReqInput data)
		{
			var response = new ApiResponse<Models.SurveyArea>();
			Models.SurveyArea objSurveyArea = ReqHelper.GetRequestData<Models.SurveyArea>(data);
			try
			{
				ModelState.Clear();
				PageMessage objPM = new PageMessage();

				if (objSurveyArea.networkIdType == NetworkIdType.A.ToString() && objSurveyArea.system_id == 0)
				{
					//GET AUTO NETWORK CODE...
					var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.SurveyArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSurveyArea.geom });
					if (objSurveyArea.isDirectSave == true)
					{
						//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISE SET REGION PROVINCE DETAILS..
						objSurveyArea = GetSurveyAreaDetail(objSurveyArea);
						// INITIALIZE DEFAULT VALUE FOR REQUIRED FIELDS
						objSurveyArea.surveyarea_name = objNetworkCodeDetail.network_code;
						objSurveyArea.due_date = DateTime.Now;
					}
					//SET NETWORK CODE
					objSurveyArea.network_id = objNetworkCodeDetail.network_code;
					objSurveyArea.sequence_id = objNetworkCodeDetail.sequence_id;
				}
				if (string.IsNullOrEmpty(objSurveyArea.surveyarea_name))
				{
					objSurveyArea.surveyarea_name = objSurveyArea.network_id;
				}

				this.Validate(objSurveyArea);
				if (ModelState.IsValid)
				{
					var isNew = objSurveyArea.system_id > 0 ? false : true;
					objSurveyArea.is_new_entity = (isNew && objSurveyArea.source_ref_id != "0" && objSurveyArea.source_ref_id != "");

					if (objSurveyArea.network_status == null)
					{
						objSurveyArea.network_status = "P";
					}
					var resultItem = new BLSurveyArea().SaveSurveyArea(objSurveyArea, objSurveyArea.user_id);
					if (objSurveyArea.isSurveyModuleAssigned && isNew)
					{
						new BLSurveyArea().SaveMobileSurveyAreaAssigned(objSurveyArea.user_id, objSurveyArea.system_id);
					}
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.SurveyArea.ToString() };

						if (isNew)
						{
							objSurveyArea.objPM.status = ResponseStatus.OK.ToString();
							objSurveyArea.objPM.isNewEntity = isNew;
							objSurveyArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							objSurveyArea.objPM.status = ResponseStatus.OK.ToString();
							objSurveyArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
					}
					else
					{
						objSurveyArea.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
						objSurveyArea.objPM.message = getFirstErrorFromModelState();
						response.error_message = getFirstErrorFromModelState();
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					}
				}
				else
				{
					objSurveyArea.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objSurveyArea.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objSurveyArea.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objSurveyArea;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					response.results = objSurveyArea;

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("SaveSurveyArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}



		#region Save SurveyArea For Building Survey Module BY ANTRA
		[HttpPost]
		public ApiResponse<Models.SurveyArea> CreateAutoSurveyArea(ReqInput data)
		{
			var response = new ApiResponse<Models.SurveyArea>();
			Models.SurveyArea objSurveyArea = ReqHelper.GetRequestData<Models.SurveyArea>(data);
			try
			{
				ModelState.Clear();
				PageMessage objPM = new PageMessage();
				if (objSurveyArea.latitude == 0)
				{
					response.status = StatusCodes.VALIDATION_FAILED.ToString();
					response.error_message = "Invalid latitude!";
					return response;
				}
				else if (objSurveyArea.longitude == 0)
				{
					response.status = StatusCodes.VALIDATION_FAILED.ToString();
					response.error_message = "Invalid longitude!";
					return response;
				}
				else if (objSurveyArea.due_date == null)
				{
					response.status = StatusCodes.VALIDATION_FAILED.ToString();
					response.error_message = "Invalid longitude!";
					return response;
				}
				else if (string.IsNullOrEmpty(objSurveyArea.surveyarea_name))
				{
					response.status = StatusCodes.VALIDATION_FAILED.ToString();
					response.error_message = "Invalid SurveyArea Name!";
					return response;
				}
				var objLocality = BLBuilding.Instance.GetGeomFromSubLocality(objSurveyArea.longitude, objSurveyArea.latitude);
				if (string.IsNullOrEmpty(objLocality.geom))
				{
					response.status = StatusCodes.FAILED.ToString();
					response.error_message = "No Sub Locality Found!";
					return response;
				}
				objSurveyArea.geom = objLocality.geom;
				objSurveyArea.networkIdType = "A";
				objSurveyArea.isDirectSave = true;
				//GET AUTO NETWORK CODE...
				var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.SurveyArea.ToString(), gType = GeometryType.Polygon.ToString(), eGeom = objSurveyArea.geom });
				//GET ENTITY DETAIL FROM TEMPLATE (IF ANY) OTHER WISE SET REGION PROVINCE DETAILS..
				objSurveyArea = GetSurveyAreaDetail(objSurveyArea);
				//SET NETWORK CODE
				objSurveyArea.network_id = objNetworkCodeDetail.network_code;
				objSurveyArea.sequence_id = objNetworkCodeDetail.sequence_id;
				if (string.IsNullOrEmpty(objSurveyArea.surveyarea_name))
				{
					objSurveyArea.surveyarea_name = objSurveyArea.network_id;
				}
				this.Validate(objSurveyArea);
				if (ModelState.IsValid)
				{
					var isNew = objSurveyArea.system_id > 0 ? false : true;
					if (objSurveyArea.network_status == null)
					{
						objSurveyArea.network_status = "P";
					}
					var resultItem = new BLSurveyArea().SaveSurveyArea(objSurveyArea, objSurveyArea.user_id);
					if (isNew)
					{
						new BLSurveyArea().SaveMobileSurveyAreaAssigned(objSurveyArea.user_id, objSurveyArea.system_id);
					}
					if (string.IsNullOrEmpty(resultItem.objPM.message))
					{
						string[] LayerName = { EntityType.SurveyArea.ToString() };

						if (isNew)
						{
							objSurveyArea.objPM.status = ResponseStatus.OK.ToString();
							objSurveyArea.objPM.isNewEntity = isNew;
							objSurveyArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
						else
						{
							objSurveyArea.objPM.status = ResponseStatus.OK.ToString();
							objSurveyArea.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
							response.status = ResponseStatus.OK.ToString();
						}
					}
					else
					{
						objSurveyArea.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
						objSurveyArea.objPM.message = getFirstErrorFromModelState();
						response.error_message = getFirstErrorFromModelState();
						response.status = ResponseStatus.VALIDATION_FAILED.ToString();
					}
				}
				else
				{
					objSurveyArea.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
					objSurveyArea.objPM.message = getFirstErrorFromModelState();
					response.error_message = getFirstErrorFromModelState();
					response.status = ResponseStatus.VALIDATION_FAILED.ToString();
				}
				if (objSurveyArea.isDirectSave == true)
				{
					//RETURN MESSAGE AS JSON FOR DIRECT SAVE
					response.results = objSurveyArea;
					response.status = ResponseStatus.OK.ToString();
				}
				else
				{
					response.results = objSurveyArea;

				}
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("CreateAutoSurveyArea()", "Library Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = ex.Message.ToString();
			}
			return response;
		}
        #endregion


        #region Add Slack
        public ApiResponse<SlackMaster> AddSlack(ReqInput data)
        {
            var response = new ApiResponse<SlackMaster>();
            try
            {
                SlackMaster objRequestIn = ReqHelper.GetRequestData<SlackMaster>(data);
                SlackMaster objSlack = GetSlackDetail(objRequestIn);
                objSlack.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
                response.status = StatusCodes.OK.ToString();
                response.results = objSlack;

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("AddSlack()", "Library Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        #region Slack Details
        public SlackMaster GetSlackDetail(SlackMaster objSlack)
        {
            if (objSlack.system_id == 0)
            {
                objSlack.longitude = Convert.ToDouble(objSlack.geom.Split(' ')[0]);
                objSlack.latitude = Convert.ToDouble(objSlack.geom.Split(' ')[1]);
                objSlack.lstSlack = BLSlack.Instance.GetSlackDetails(objSlack.longitude, objSlack.latitude, objSlack.associated_system_id, "Duct", objSlack.structure_id);
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objSlack, GeometryType.Point.ToString(), objSlack.geom);
                //Fill Parent detail...              
                fillParentDetail(objSlack, new NetworkCodeIn() { eType = EntityType.Slack.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSlack.geom }, objSlack.networkIdType);
            }
            else
            {
                // Get entity detail by Id...
                objSlack = new BLMisc().GetEntityDetailById<SlackMaster>(objSlack.system_id, EntityType.Slack, objSlack.user_id);
                objSlack.lstSlack = BLSlack.Instance.GetSlackDetails(objSlack.longitude, objSlack.latitude, objSlack.associated_system_id, objSlack.associated_entity_type, objSlack.structure_id);
                DuctMaster obj = new DuctMaster();
                int DuctId = Convert.ToInt32(objSlack.duct_id);
                obj = new BLDuct().GetDuctNameAndLengthForSlack(DuctId);
                objSlack.duct_name = obj.duct_name;
                objSlack.duct_calculated_length = obj.calculated_length;
                objSlack.total_slack_count = obj.total_slack_count;
                objSlack.available_calculated_length = (obj.calculated_length - obj.total_slack_length);
                objSlack.total_slack_length = obj.total_slack_length;
                objSlack.associated_network_id = obj.network_id;
                fillRegionProvAbbr(objSlack);
            }
            return objSlack;
        }
        #endregion

        #region Save Slack
        public ApiResponse<SlackMaster> SaveSlack(ReqInput data)
        {
            var response = new ApiResponse<SlackMaster>();
            SlackMaster objSlack = ReqHelper.GetRequestData<SlackMaster>(data);
            try
            {
                ModelState.Clear();
                if (objSlack.system_id == 0)
                {
                    objSlack.created_by = objSlack.user_id;
                    objSlack.created_on = DateTimeHelper.Now;
                    objSlack.associated_entity_type = "Duct";
                    objSlack.longitude = Convert.ToDouble(objSlack.geom.Split(' ')[0]);
                    objSlack.latitude = Convert.ToDouble(objSlack.geom.Split(' ')[1]);
                    objSlack.associated_system_id = Convert.ToInt32(objSlack.duct_id);
                    objSlack.duct_system_id = Convert.ToInt32(objSlack.duct_id);
					objSlack.associated_network_id = objSlack.duct_name;
                    NetworkCodeIn objIn = new NetworkCodeIn();
                    objIn.eType = "Slack"; objIn.eGeom = objSlack.geom; objIn.gType = "Point";
                    var networkCodeDetail = new BLMisc().GetNetworkCodeDetail(objIn);
                    objSlack.parent_entity_type = networkCodeDetail.parent_entity_type;
                    objSlack.parent_network_id = networkCodeDetail.parent_network_id;
                    objSlack.parent_system_id = networkCodeDetail.parent_system_id;
                    objSlack.network_id = networkCodeDetail.network_code;
                    objSlack.sequence_id = networkCodeDetail.sequence_id;
                }
                else
                {
                    objSlack.modified_by = objSlack.user_id;
                    objSlack.modified_on = DateTimeHelper.Now;
                }
                objSlack.objPM = null;
                objSlack.lstSlack = null;
                this.Validate(objSlack);
                if (ModelState.IsValid)
                {
                    var isNew = objSlack.system_id > 0 ? false : true;
                    var resultItem = new BLSlack().SaveEntitySlack(JsonConvert.SerializeObject(objSlack));
                    objSlack.system_id = resultItem.systemId;
                    objSlack.objPM = new PageMessage();
                    string[] LayerName = { EntityType.Slack.ToString() };
                    if (resultItem.systemId > 0)
                    {
                        if (isNew)
                        {
                            objSlack.objPM.status = ResponseStatus.OK.ToString();
                            objSlack.objPM.isNewEntity = isNew;
                            objSlack.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName); ;
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                            response.status = ResponseStatus.OK.ToString();
                        }
                        else
                        {
                            objSlack.objPM.status = ResponseStatus.OK.ToString();
                            objSlack.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                            response.status = ResponseStatus.OK.ToString();
                        }
                    }
                    else
                    {
                        objSlack.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        objSlack.objPM.message = resultItem.message;
                        response.error_message = resultItem.message;
                        response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    }

                }
                else
                {
                    objSlack.objPM.status = ResponseStatus.FAILED.ToString();
                    objSlack.objPM.message = getFirstErrorFromModelState();
                    response.error_message = getFirstErrorFromModelState();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                }
                if (objSlack.isDirectSave == true)
                {
                    //RETURN MESSAGE AS JSON FOR DIRECT SAVE
                    response.results = objSlack;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    objSlack.lstSlack = BLSlack.Instance.GetSlackDetails(objSlack.longitude, objSlack.latitude, objSlack.associated_system_id, "Duct", objSlack.structure_id);
                    objSlack.formInputSettings = new BLFormInputSettings().getformInputSettings().Where(m => m.form_name == EntityType.Duct.ToString()).ToList();
                    response.results = objSlack;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveSlack()", "Library Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        #endregion

        #region Site
        #region Add Site
        /// <summary> Add Site </summary>
        /// <returns>Site Details</returns>
        /// <CreatedBy>Pawan</CreatedBy>

        public ApiResponse<Site> AddSite(ReqInput data)
        {
            var response = new ApiResponse<Site>();
            try
            {
                Site obj = ReqHelper.GetRequestData<Site>(data);
                Site objSite = GetSiteDetail(obj);
                //BLItemTemplate.Instance.BindItemDropdowns(objSite, EntityType.Site.ToString());
                fillProjectSpecifications(objSite);
                //BindCouplerDropDown(objSite);
                // objSite.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Site.ToString()).ToList();
                //Get the layer details to bind additional attributes Coupler
                var layerdetails = new BLLayer().getLayer(EntityType.Site.ToString());
                // objSite.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                //End for additional attributes Coupler
                response.status = StatusCodes.OK.ToString();
                response.results = objSite;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("AddSite()", "Library Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message;
            }
            return response;
        }
        #endregion

        #region Get Site Details
        /// <summary> GetSiteDetail</summary>
        /// <param >Site Object</param>
        /// <returns>Site Details</returns>
        /// <CreatedBy>Pawan</CreatedBy>
        public Site GetSiteDetail(Site objSite)
        {
            int user_id = objSite.created_by;
            if (objSite.system_id == 0)
            {
                //NEW ENTITY->Fill Region and Province Detail..
                fillRegionProvinceDetail(objSite, GeometryType.Point.ToString(), objSite.geom);

                //Fill Parent detail...              
                fillParentDetail(objSite, new NetworkCodeIn() { eType = EntityType.Site.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSite.geom }, objSite.networkIdType);

                objSite.longitude = Convert.ToDouble(objSite.geom.Split(' ')[0]);
                objSite.latitude = Convert.ToDouble(objSite.geom.Split(' ')[1]);
                //objSite.ownership_type = "Own";
                // Item template binding
                //var objItem = BLItemTemplate.Instance.GetTemplateDetail<CouplerTemplateMaster>(objSite.created_by, EntityType.Site);
                //MiscHelper.CopyMatchingProperties(objItem, objSite);
                //objSite.other_info = null;  //for additional-attributes
            }
            else
            {
                // Get entity detail by Id...
                objSite = new BLMisc().GetEntityDetailById<Site>(objSite.system_id, EntityType.Site, objSite.created_by);
                //for additional-attributes
                //objSite.other_info = new BLCoupler().GetOtherInfoCoupler(objSite.system_id);
                fillRegionProvAbbr(objSite);
            }
            objSite.lstUserModule = new BLLayer().GetUserModuleAbbrList(user_id, UserType.Web.ToString());

            return objSite;
        }

        #endregion

        #region Save Site
        /// <summary> SaveCoupler </summary>
        /// <param name="data">ReqInput</param>
        /// <CreatedBy>Pawan Kr</CreatedBy>
        public ApiResponse<Site> SaveSite(ReqInput data)
        {
            var response = new ApiResponse<Site>();
            Site objSite = ReqHelper.GetRequestData<Site>(data);
            try
            {
                ModelState.Clear();

                if (objSite.networkIdType == NetworkIdType.A.ToString() && objSite.system_id == 0)
                {
                    //GET AUTO NETWORK CODE...
                    var objNetworkCodeDetail = new BLMisc().GetNetworkCodeDetail(new NetworkCodeIn() { eType = EntityType.Site.ToString(), gType = GeometryType.Point.ToString(), eGeom = objSite.geom });

                    //SET NETWORK CODE
                    objSite.network_id = objNetworkCodeDetail.network_code;
                    objSite.sequence_id = objNetworkCodeDetail.sequence_id;

                }
                this.Validate(objSite);
                if (ModelState.IsValid)
                {
                    var isNew = objSite.system_id > 0 ? false : true;
                    var resultItem = new BLSite().Save(objSite, objSite.created_by);
                    if (string.IsNullOrEmpty(resultItem.objPM.message))
                    {

                        string[] LayerName = { EntityType.Site.ToString() };
                        if (isNew)
                        {
                            objSite.objPM.status = ResponseStatus.OK.ToString();
                            objSite.objPM.isNewEntity = isNew;
                            objSite.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_GBL_GBL_GBL_GBL_095, ApplicationSettings.listLayerDetails, LayerName);
                        }
                        else
                        {

                            objSite.objPM.status = ResponseStatus.OK.ToString();
                            objSite.objPM.message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_GBL_GBL_GBL_064, ApplicationSettings.listLayerDetails, LayerName);
                        }
                    }
                    else
                    {
                        objSite.objPM.status = ResponseStatus.FAILED.ToString();
                        objSite.objPM.message = getFirstErrorFromModelState();
                        response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                        response.error_message = getFirstErrorFromModelState();
                        response.results = objSite;
                    }
                }
                else
                {
                    objSite.objPM.status = ResponseStatus.FAILED.ToString();
                    objSite.objPM.message = getFirstErrorFromModelState();
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                }

                {
                    //BLItemTemplate.Instance.BindItemDropdowns(objSite, EntityType.Site.ToString());
                    // RETURN PARTIAL VIEW WITH MODEL DATA
                     fillProjectSpecifications(objSite);
                    //  BindCouplerDropDown(objSite);
                    //Get the layer details to bind additional attributes Coupler
                    //var layerdetails = new BLLayer().getLayer(EntityType.Site.ToString());
                    // objSite.objDynamicControls = GetAdditionalAttributesForm(layerdetails.layer_id);
                    //End for additional attributes Coupler
                    //  objSite.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Site.ToString()).ToList();
                    response.results = objSite;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveSite()", "Library Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message;
            }
            return response;
        }
        #endregion

        #region BindSiteDropDown
        /// <param name="data">objSite</param>
        /// <CreatedBy>Site</CreatedBy>
        //public void BindCouplerDropDown(CouplerMaster objSite)
        //{
        //	var objDDL = new BLMisc().GetDropDownList(EntityType.Coupler.ToString());
        //	objSite.listCouplerType = objDDL.Where(x => x.dropdown_type == DropDownType.Coupler.ToString()).ToList();
        //	objSite.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        //	var _objDDL = new BLMisc().GetDropDownList("");
        //	objSite.lstBOMSubCategory = _objDDL.Where(x => x.dropdown_type == DropDownType.bom_sub_category.ToString()).ToList();
        //	objSite.lstServedByRing = _objDDL.Where(x => x.dropdown_type == DropDownType.served_by_ring.ToString()).ToList();
        //}
        #endregion

        #endregion
    }
}

