using BusinessLogics;
using BusinessLogics.ISP;
using Models;
using Models.API;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Linq;
using System.Web.Http;
using System.Data;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [RoutePrefix("api/ItemTemplate")]
    [CustomAuthorization]
    [APIExceptionFilter]
    [CustomAction]   
    public class ItemTemplateController : ApiController
    {
        #region Entity Template
        /// <summary>
        /// Entity Template Generic Api
        /// </summary>
        /// <param name="data">Json Data</param>
        /// <returns>Result According TO pass Action</returns>
        /// <Created By>Sumit Poonia</Created By>
        /// <Created Date>01-Jan-2021</Created Date>
        [HttpPost]

        public dynamic EntityTemplate(ReqInput data)
        {
            var response = new ApiResponse<dynamic>();
            HeaderAttributes headerAttribute = ReqHelper.getHeaderValue(Request.Headers.ToList());
            data.data = ReqHelper.MergeHeaderAttributeInJsonObject(data, headerAttribute);
            this.Validate(headerAttribute);
            if (ModelState.IsValid)
            {
                if (headerAttribute.entity_type.ToUpper() == EntityType.Pole.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetPoleTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SavePoleTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Manhole.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetManholeTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveManholeTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Tree.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetTreeTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveTreeTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.WallMount.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetWallmountTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveWallmountTemplate(data);
                    }

                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.POD.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetPodTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SavePODTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.MPOD.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetMPODTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveMPODTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Cable.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetCableTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveCableTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.SpliceClosure.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetSCTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveSpliceClosureTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Splitter.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetSplitterTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveSplitterTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.ADB.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetADBTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveADBTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }

                else if (headerAttribute.entity_type.ToUpper() == EntityType.ONT.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetONTTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveONTTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.FDB.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetFDBTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveFDBTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.BDB.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetBdbTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveBdbTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.CDB.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetCdbTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveCdbTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.FMS.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetFMSTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveFMSTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.HTB.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetHTBTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveHTBTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Coupler.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetCouplerTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveCouplerTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Trench.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetTrenchTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveTrenchTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Duct.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetDuctTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveDuctTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Gipipe.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetGipipeTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveGipipeTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }

                else if (headerAttribute.entity_type.ToUpper() == EntityType.Conduit.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetConduitTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveConduitTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
				else if (headerAttribute.entity_type.ToUpper() == EntityType.Microduct.ToString().ToUpper())
				{
					if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
					{
						return GetMicroductTemplate(data);
					}
					else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
					{
						return SaveMicroductTemplate(data);
					}
					else
					{
						response.status = ResponseStatus.FAILED.ToString();
						response.error_message = "Entity_Action not matched";
						return response;
					}
				}
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Tower.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetTowerTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveTowerTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }

                else if (headerAttribute.entity_type.ToUpper() == EntityType.Sector.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetSectorTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveSectorTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }



                else if (headerAttribute.entity_type.ToUpper() == EntityType.Antenna.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetAntennaTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveAntennaTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                else if (headerAttribute.entity_type.ToUpper() == EntityType.OpticalRepeater.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetOpticalRepeaterTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveOpticalRepeaterTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }

                //cabinet shazia 
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Cabinet.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetCabinetTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveCabinetTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                //vault shazia end

                else if (headerAttribute.entity_type.ToUpper() == EntityType.Vault.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetVaultTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveVaultTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                //Vault shazia end 

                //Handhole By ANTRA
                else if (headerAttribute.entity_type.ToUpper() == EntityType.Handhole.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetHandholeTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SaveHandholeTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }

                //END Handhole
                //PatchPanel by shazia
                else if (headerAttribute.entity_type.ToUpper() == EntityType.PatchPanel.ToString().ToUpper())
                {
                    if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                    {
                        return GetPatchPanelTemplate(data);
                    }
                    else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                    {
                        return SavePatchPanelTemplate(data);
                    }
                    else
                    {
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = "Entity_Action not matched";
                        return response;
                    }
                }
                // end PatchPanel 

            }
            else if (headerAttribute.entity_type.ToUpper() == EntityType.Gipipe.ToString().ToUpper())
            {
                if (headerAttribute.entity_action.ToUpper() == EntityAction.Get.ToString().ToUpper())
                {
                    return GetDuctTemplate(data);
                }
                else if (headerAttribute.entity_action.ToUpper() == EntityAction.Save.ToString().ToUpper())
                {
                    return SaveGipipeTemplate(data);
                }
                else
                {
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = "Entity_Action not matched";
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

        #region Manhole

        #region Get Manhole Template
        /// <summary> GetManholeTemplate </summary>
        /// <returns>Manhole Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>

        public ApiResponse<ManholeItemMaster> GetManholeTemplate(ReqInput data)
        {
            var response = new ApiResponse<ManholeItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                ManholeItemMaster objManholeItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ManholeItemMaster>(objItemTemplateIn.userId, EntityType.Manhole);
                var objDDL = new BLMisc().GetDropDownList(EntityType.Manhole.ToString());
                objManholeItemMaster.lstManholeType = objDDL.Where(x => x.dropdown_type == DropDownType.Manhole_types.ToString()).ToList();
                objManholeItemMaster.is_specification_allowed = new BLLayer().GetSpecificationAllowed(EntityType.Manhole.ToString()).ToString();
                BLItemTemplate.Instance.BindItemDropdowns(objManholeItemMaster, EntityType.Manhole.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objManholeItemMaster;
                
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetManholeTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Manhole Template!";
            }
            return response;
        }
        #endregion

        #region Save Manhole Template
        /// <summary>SaveManholeTemplate </summary>
        /// <returns>Manhole Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<ManholeItemMaster> SaveManholeTemplate(ReqInput data)
        {
            var response = new ApiResponse<ManholeItemMaster>();
            try
            {
                ManholeItemMaster objManholeItem = ReqHelper.GetRequestData<ManholeItemMaster>(data);
                this.Validate(objManholeItem);
                if (ModelState.IsValid)
                {
                    var itemid = objManholeItem.id;
                    var resultItem = new BLManholeItemMaster().SaveManholeItemTemplate(objManholeItem, objManholeItem.userId);
                    if (itemid > 0)  // Update 
                    {
                        objManholeItem.id = resultItem.id;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_MH_NET_FRM_002;
                        objManholeItem.objPM.status = ResponseStatus.OK.ToString();
                        objManholeItem.objPM.message = Resources.Resources.SI_OSP_MH_NET_FRM_002;
                        response.results = objManholeItem;
                    }
                    else
                    {
                        objManholeItem.id = resultItem.id;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_MH_NET_FRM_003;
                        objManholeItem.objPM.status = ResponseStatus.OK.ToString();
                        objManholeItem.objPM.message = Resources.Resources.SI_OSP_MH_NET_FRM_003;
                        response.results = objManholeItem;
                    }
                }
                else
                {
                    objManholeItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    objManholeItem.objPM.message = getFirstErrorFromModelState();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objManholeItem;
                }
                var objDDL = new BLMisc().GetDropDownList(EntityType.Manhole.ToString());
                objManholeItem.lstManholeType = objDDL.Where(x => x.dropdown_type == DropDownType.Manhole_types.ToString()).ToList();
                BLItemTemplate.Instance.BindItemDropdowns(objManholeItem, EntityType.Manhole.ToString());
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveManholeTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        #endregion

        #region Pole
        #region Get Pole Template 
        /// <summary>GetPoleTemplate </summary>
        /// <returns>Pole Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>

        public ApiResponse<PoleItemMaster> GetPoleTemplate(ReqInput data)
        {
            var response = new ApiResponse<PoleItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                PoleItemMaster objPoleItemMaster = BLItemTemplate.Instance.GetTemplateDetail<PoleItemMaster>(objItemTemplateIn.userId, EntityType.Pole);
                var objDDL = new BLMisc().GetDropDownList(EntityType.Pole.ToString());
                objPoleItemMaster.lstPoleType = objDDL.Where(x => x.dropdown_type == DropDownType.Pole_Type.ToString()).ToList();
                BLItemTemplate.Instance.BindItemDropdowns(objPoleItemMaster, EntityType.Pole.ToString());
                objPoleItemMaster.is_specification_allowed = new BLLayer().GetSpecificationAllowed(EntityType.Pole.ToString()).ToString();
                response.status = StatusCodes.OK.ToString();
                response.results = objPoleItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetPoleTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Pole Template!";
            }
            return response;
        }
        #endregion

        #region Save Pole Template 
        /// <summary>SavePoleTemplate </summary>
        /// <returns>Pole Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<PoleItemMaster> SavePoleTemplate(ReqInput data)
        {
            var response = new ApiResponse<PoleItemMaster>();
            try
            {
                PoleItemMaster objPoleItem = ReqHelper.GetRequestData<PoleItemMaster>(data);
                this.Validate(objPoleItem);
                if (ModelState.IsValid)
                {
                    var itemid = objPoleItem.id;
                    var resultItem = new BLPoleItemMaster().SavePoleItemTemplate(objPoleItem, Convert.ToInt32(objPoleItem.userId));
                    if (itemid > 0)  // Update 
                    {
                        objPoleItem.id = resultItem.id;
                        objPoleItem.objPM.status = ResponseStatus.OK.ToString();
                        objPoleItem.objPM.message = Resources.Resources.SI_OSP_POL_NET_FRM_008;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_POL_NET_FRM_008;
                        response.results = objPoleItem;
                    }
                    else
                    {
                        objPoleItem.id = resultItem.id;
                        objPoleItem.objPM.status = ResponseStatus.OK.ToString();
                        objPoleItem.objPM.message = Resources.Resources.SI_OSP_POL_NET_FRM_009;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_POL_NET_FRM_009;
                        response.results = objPoleItem;
                    }
                }
                else
                {
                    objPoleItem.objPM.message = getFirstErrorFromModelState();
                    objPoleItem.objPM.status = ResponseStatus.FAILED.ToString();
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objPoleItem;
                }
                //fill dropdown
                BindpoleDropDown(objPoleItem);
                BLItemTemplate.Instance.BindItemDropdowns(objPoleItem, EntityType.Pole.ToString());
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SavePoleTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #endregion

        #region Tree
        #region Get Tree Template 
        /// <summary>GetTreeTemplate </summary>
        /// <returns>Tree Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<TreeItemMaster> GetTreeTemplate(ReqInput data)
        {
            var response = new ApiResponse<TreeItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                TreeItemMaster objTreeItemMaster = BLItemTemplate.Instance.GetTemplateDetail<TreeItemMaster>(objItemTemplateIn.userId, EntityType.Tree);
                BLItemTemplate.Instance.BindItemDropdowns(objTreeItemMaster, EntityType.Tree.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objTreeItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetTreeTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Tree Template!";
            }
            return response;
        }
        #endregion

        #region Save Tree Template 
        /// <summary>SaveTreeTemplate </summary>
        /// <returns>Tree Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<TreeItemMaster> SaveTreeTemplate(ReqInput data)
        {
            var response = new ApiResponse<TreeItemMaster>();
            try
            {
                TreeItemMaster objTreeItem = ReqHelper.GetRequestData<TreeItemMaster>(data);
                this.Validate(objTreeItem);
                if (ModelState.IsValid)
                {
                    var itemid = objTreeItem.id;
                    var resultItem = new BLTreeItemMaster().SaveTreeItemTemplate(objTreeItem, objTreeItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objTreeItem.objPM.status = ResponseStatus.OK.ToString();
                        objTreeItem.objPM.message = Resources.Resources.SI_OSP_TRE_NET_FRM_016;// "Tree template update successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_TRE_NET_FRM_016;
                    }
                    else
                    {
                        objTreeItem.objPM.status = ResponseStatus.OK.ToString();
                        objTreeItem.objPM.message = Resources.Resources.SI_OSP_TRE_NET_FRM_020;// "Tree template saved successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_TRE_NET_FRM_020;
                        objTreeItem = resultItem;
                    }
                }
                else
                {
                    objTreeItem.objPM.message = getFirstErrorFromModelState();
                    objTreeItem.objPM.status = ResponseStatus.FAILED.ToString();
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objTreeItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objTreeItem, EntityType.Tree.ToString());
                response.results = objTreeItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveTreeTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region Wallmount
        #region Get Wallmount Template 
        /// <summary>GetWallmountTemplate </summary>
        /// <returns>WallMount Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<WallMountItemMaster> GetWallmountTemplate(ReqInput data)
        {
            var response = new ApiResponse<WallMountItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                WallMountItemMaster objWallMountItemMaster = BLItemTemplate.Instance.GetTemplateDetail<WallMountItemMaster>(objItemTemplateIn.userId, EntityType.WallMount);
                BLItemTemplate.Instance.BindItemDropdowns(objWallMountItemMaster, EntityType.WallMount.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objWallMountItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetWallmountTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Wallmount Template!";
            }
            return response;
        }
        #endregion

        #region Save Wallmount Template 
        /// <summary>SaveWallmountTemplate </summary>
        /// <returns>Wallmount Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<WallMountItemMaster> SaveWallmountTemplate(ReqInput data)
        {
            var response = new ApiResponse<WallMountItemMaster>();
            try
            {
                WallMountItemMaster objWallMountItem = ReqHelper.GetRequestData<WallMountItemMaster>(data);
                this.Validate(objWallMountItem);
                if (ModelState.IsValid)
                {
                    var itemid = objWallMountItem.id;
                    var resultItem = new BLWallMountItemMaster().SaveWallMountItemTemplate(objWallMountItem, objWallMountItem.UserId);

                    if (itemid > 0)  // Update 
                    {
                        objWallMountItem.objPM.status = ResponseStatus.OK.ToString();
                        objWallMountItem.objPM.message = Resources.Resources.SI_OSP_WMT_NET_FRM_009;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_WMT_NET_FRM_009;
                    }
                    else
                    {
                        objWallMountItem.objPM.status = ResponseStatus.OK.ToString();
                        objWallMountItem.objPM.message = Resources.Resources.SI_OSP_WMT_NET_FRM_010;
                        objWallMountItem = resultItem;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_WMT_NET_FRM_010;
                    }
                }
                else
                {
                    objWallMountItem.objPM.message = getFirstErrorFromModelState();
                    objWallMountItem.objPM.status = ResponseStatus.FAILED.ToString();
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objWallMountItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objWallMountItem, EntityType.WallMount.ToString());
                response.results = objWallMountItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveWallMountTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region Pod
        #region Get Pod Template 
        /// <summary>GetPodTemplate </summary>
        /// <returns>Pod Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>015-Jan-2021</CreatedDate>
        public ApiResponse<PODItemMaster> GetPodTemplate(ReqInput data)
        {
            var response = new ApiResponse<PODItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                PODItemMaster objPODItemMaster = BLItemTemplate.Instance.GetTemplateDetail<PODItemMaster>(objItemTemplateIn.userId, EntityType.POD);
                BLItemTemplate.Instance.BindItemDropdowns(objPODItemMaster, EntityType.POD.ToString());
                BindPODDropdown(objPODItemMaster);
                response.status = StatusCodes.OK.ToString();
                response.results = objPODItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetPodTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Wallmount Template!";
            }
            return response;
        }
        #endregion

        #region Bind Pod Dropdown
        /// <summary>BindPODDropdown </summary>
        /// <returns>Pod Dropdown Item</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>15-Jan-2021</CreatedDate>
        private void BindPODDropdown(PODItemMaster objPODItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.POD.ToString());
            objPODItemMaster.listPODType = objDDL.Where(x => x.dropdown_type == DropDownType.POD_Type.ToString()).ToList();
        }
        #endregion

        #region Save Pod Template 
        /// <summary>SaveWallmountTemplate </summary>
        /// <returns>Wallmount Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>15-Jan-2021</CreatedDate>
        public ApiResponse<PODItemMaster> SavePODTemplate(ReqInput data)
        {
            var response = new ApiResponse<PODItemMaster>();
            try
            {
                PODItemMaster objPodItem = ReqHelper.GetRequestData<PODItemMaster>(data);
                this.Validate(objPodItem);
                if (ModelState.IsValid)
                {
                    var layer_title = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.POD.ToString().ToUpper()).FirstOrDefault().layer_title;
                    var itemid = objPodItem.id;
                    var resultItem = new BLPODItemMaster().SavePODItemTemplate(objPodItem, objPodItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objPodItem.objPM.status = ResponseStatus.OK.ToString();
                        objPodItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template update successfully.
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);
                    }
                    else
                    {
                        objPodItem.objPM.status = ResponseStatus.OK.ToString();
                        objPodItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//{0} template saved successfully.
                        response.status = StatusCodes.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);
                        objPodItem = resultItem;
                    }
                }
                else
                {
                    objPodItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objPodItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = StatusCodes.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objPodItem, EntityType.POD.ToString());
                BindPODDropdown(objPodItem);
                response.results = objPodItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SavePODTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region MPOD
        #region Get MPOD Template 
        /// <summary>GetMPODTemplate </summary>
        /// <returns>Pod Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>015-Jan-2021</CreatedDate>
        public ApiResponse<MPODItemMaster> GetMPODTemplate(ReqInput data)
        {
            var response = new ApiResponse<MPODItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                MPODItemMaster objMPODItemMaster = BLItemTemplate.Instance.GetTemplateDetail<MPODItemMaster>(objItemTemplateIn.userId, EntityType.MPOD);
                BLItemTemplate.Instance.BindItemDropdowns(objMPODItemMaster, EntityType.MPOD.ToString());
                BindMPODDropdown(objMPODItemMaster);
                response.status = StatusCodes.OK.ToString();
                response.results = objMPODItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetMPODTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching MPOD Template!";
            }
            return response;
        }
        #endregion

        #region Bind MPOD Dropdown
        /// <summary>BindMPODDropdown </summary>
        /// <returns>MPOD Dropdown Item</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>15-Jan-2021</CreatedDate>
        private void BindMPODDropdown(MPODItemMaster objMPODItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.MPOD.ToString());
            objMPODItemMaster.listMPODType = objDDL.Where(x => x.dropdown_type == DropDownType.MPOD_Type.ToString()).ToList();
        }
        #endregion

        #region Save MPOD Template 
        /// <summary>SaveWallmountTemplate </summary>
        /// <returns>MPOD Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>15-Jan-2021</CreatedDate>
        public ApiResponse<MPODItemMaster> SaveMPODTemplate(ReqInput data)
        {
            var response = new ApiResponse<MPODItemMaster>();
            try
            {
                MPODItemMaster objMPODItem = ReqHelper.GetRequestData<MPODItemMaster>(data);
                this.Validate(objMPODItem);
                if (ModelState.IsValid)
                {
                    if (ModelState.IsValid)
                    {
                        var itemid = objMPODItem.id;
                        var resultItem = new BLMPODItemMaster().SaveMPODItemTemplate(objMPODItem, objMPODItem.userId);

                        if (itemid > 0)  // Update 
                        {
                            objMPODItem.objPM.status = ResponseStatus.OK.ToString();
                            objMPODItem.objPM.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_010;
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = Resources.Resources.SI_OSP_MPOD_NET_FRM_010;
                        }
                        else
                        {
                            objMPODItem.objPM.status = ResponseStatus.OK.ToString();
                            objMPODItem.objPM.message = Resources.Resources.SI_OSP_MPOD_NET_FRM_011;
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = Resources.Resources.SI_OSP_MPOD_NET_FRM_011;
                            objMPODItem = resultItem;
                        }
                    }
                    else
                    {
                        objMPODItem.objPM.message = getFirstErrorFromModelState();
                        objMPODItem.objPM.status = ResponseStatus.FAILED.ToString();
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = getFirstErrorFromModelState();
                        response.results = objMPODItem;
                    }
                    //fill dropdown
                    BLItemTemplate.Instance.BindItemDropdowns(objMPODItem, EntityType.MPOD.ToString());
                    BindMPODDropdown(objMPODItem);
                    response.results = objMPODItem;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveMPODTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region Cable
        #region Get Cable Template 
        /// <summary>GetCableTemplate </summary>
        /// <returns>Cable Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>15-Feb-2021</CreatedDate>
        public ApiResponse<CableItemMaster> GetCableTemplate(ReqInput data)
        {
            var response = new ApiResponse<CableItemMaster>();
            try
            {
                CableItemMaster objItemTemplateIn = ReqHelper.GetRequestData<CableItemMaster>(data);
                CableItemMaster objCableItemMaster = new CableItemMaster();
                objCableItemMaster = BLItemTemplate.Instance.GetTemplateDetail<CableItemMaster>(objItemTemplateIn.user_id, EntityType.Cable, objItemTemplateIn.cblType);
                BindCableDropDown(objCableItemMaster);
                objCableItemMaster.fiberCountIn = objCableItemMaster.total_core.ToString();
                BLItemTemplate.Instance.BindItemDropdowns(objCableItemMaster, EntityType.Cable.ToString());
                new BLMisc().BindPortDetails(objCableItemMaster, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
                
                objCableItemMaster.cable_type = (objItemTemplateIn.cblType == "ISP" ? "ISP" : objCableItemMaster.cable_type);
                objCableItemMaster.unitValue = Convert.ToString(objCableItemMaster.total_core);
                objCableItemMaster.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
                response.status = StatusCodes.OK.ToString();
                response.results = objCableItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetCableTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Cable Template!";
            }
            return response;
        }
        #endregion

        #region Bind Cable Dropdown
        /// <summary>BindCableDropdown </summary>
        /// <returns>Cable Dropdown Item</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>15-Feb-2021</CreatedDate>
        private void BindCableDropDown(CableItemMaster objCableItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Cable.ToString());
            objCableItemMaster.fiberCount = objDDL.Where(x => x.dropdown_type == DropDownType.Fiber_Count.ToString()).ToList();
            objCableItemMaster.listcableCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Category.ToString()).ToList();
            objCableItemMaster.listcableSubCategory = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Subcategory.ToString()).ToList();
            objCableItemMaster.listcableType = objDDL.Where(x => x.dropdown_type == DropDownType.Cable_Type.ToString()).ToList();
        }
        #endregion

        #region Save Cable Template 
        /// <summary>SaveCableTemplate </summary>
        /// <returns>Cable Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>15-Feb-2021</CreatedDate>
        public ApiResponse<CableItemMaster> SaveCableTemplate(ReqInput data)
        {
            var response = new ApiResponse<CableItemMaster>();
            try
            {
                CableItemMaster objCableItem = ReqHelper.GetRequestData<CableItemMaster>(data);
                this.Validate(objCableItem);
                if (ModelState.IsValid)
                {
                    if (ModelState.IsValid)
                    {
                        var itemid = objCableItem.id;
                        if (!(string.IsNullOrEmpty(objCableItem.unitValue)))
                        {
                            objCableItem.total_core = Convert.ToInt32(objCableItem.unitValue);
                        }
                        var resultItem = new BLCableItemMaster().SaveCableItemTemplate(objCableItem, objCableItem.user_id);
                        string[] LayerName = { EntityType.Cable.ToString() };
                        if (itemid > 0)  // Update 
                        {
                            objCableItem.objPM.status = ResponseStatus.OK.ToString();
                            objCableItem.objPM.message = objCableItem.cable_type + " " + ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CAB_NET_FRM_063, ApplicationSettings.listLayerDetails, LayerName); //objCableItem.cable_type + " " + Resources.Resources.SI_OSP_CAB_NET_FRM_063;
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = objCableItem.cable_type + " " + ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CAB_NET_FRM_063, ApplicationSettings.listLayerDetails, LayerName); //objCableItem.cable_type + " " + Resources.Resources.SI_OSP_CAB_NET_FRM_063;
                        }
                        else
                        {
                            objCableItem.objPM.status = ResponseStatus.OK.ToString();
                            objCableItem.objPM.message = objCableItem.cable_type + " " + ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CAB_NET_FRM_064, ApplicationSettings.listLayerDetails, LayerName); //objCableItem.cable_type + " " + Resources.Resources.SI_OSP_CAB_NET_FRM_064;
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = objCableItem.cable_type + " " + ConvertMultilingual.GetLayerActionMessage(Resources.Resources.SI_OSP_CAB_NET_FRM_064, ApplicationSettings.listLayerDetails, LayerName); //objCableItem.cable_type + " " + Resources.Resources.SI_OSP_CAB_NET_FRM_064;
                            objCableItem = resultItem;
                        }
                    }
                    else
                    {
                        objCableItem.objPM.message = getFirstErrorFromModelState();
                        objCableItem.objPM.status = ResponseStatus.FAILED.ToString();
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = getFirstErrorFromModelState();
                        response.results = objCableItem;

                    }
                    //fill dropdown
                    BindCableDropDown(objCableItem);
                    new BLMisc().BindPortDetails(objCableItem, EntityType.Cable.ToString(), DropDownType.Fiber_Count.ToString());
                    BLItemTemplate.Instance.BindItemDropdowns(objCableItem, EntityType.Cable.ToString());
                    objCableItem.formInputSettings = ApplicationSettings.formInputSettings.Where(m => m.form_name == EntityType.Cable.ToString()).ToList();
                    response.results = objCableItem;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveCableTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region ADB
        #region Get ADB Template 
        /// <summary>GetADBTemplate </summary>
        /// <returns>ADB Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>

        public ApiResponse<ADBItemMaster> GetADBTemplate(ReqInput data)
        {
            var response = new ApiResponse<ADBItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                ADBItemMaster objADBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ADBItemMaster>(objItemTemplateIn.userId, EntityType.ADB);
                BLItemTemplate.Instance.BindItemDropdowns(objADBItemMaster, EntityType.ADB.ToString());
                new BLMisc().BindPortDetails(objADBItemMaster, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
                var objDDL = new BLMisc().GetDropDownList(EntityType.ADB.ToString(), DropDownType.Entity_Type.ToString());
                objADBItemMaster.lstEntityType = objDDL;
                response.status = StatusCodes.OK.ToString();
                response.results = objADBItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetADBTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching ADB Template!";
            }
            return response;
        }
        #endregion

        #region Save ADB Template 
        /// <summary>SaveADBTemplate </summary>
        /// <returns>ADB Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<ADBItemMaster> SaveADBTemplate(ReqInput data)
        {
            var response = new ApiResponse<ADBItemMaster>();
            try
            {
                ADBItemMaster objADBItem = ReqHelper.GetRequestData<ADBItemMaster>(data);
                this.Validate(objADBItem);
                if (ModelState.IsValid)
                {
                    var itemid = objADBItem.id;
                    if (objADBItem.unitValue != null && objADBItem.unitValue.Contains(":"))
                    {
                        objADBItem.no_of_input_port = Convert.ToInt32(objADBItem.unitValue.Split(':')[0]);
                        objADBItem.no_of_output_port = Convert.ToInt32(objADBItem.unitValue.Split(':')[1]);
                    }
                    var resultItem = new BLADBItemMaster().SaveADBItemTemplate(objADBItem, objADBItem.userId);
                    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.ADB.ToString().ToUpper()).FirstOrDefault().layer_title;
                    if (itemid > 0)  // Update 
                    {
                        objADBItem.objPM.status = ResponseStatus.OK.ToString();
                        objADBItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title); //Resources.Resources.SI_OSP_ADB_NET_FRM_008;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title); //Resources.Resources.SI_OSP_ADB_NET_FRM_008;
                    }
                    else
                    {
                        objADBItem.objPM.status = ResponseStatus.OK.ToString();
                        objADBItem.objPM.message = string.Format(Resources.Resources.SI_OSP_CAB_NET_FRM_064, layer_title); //Resources.Resources.SI_OSP_ADB_NET_FRM_009;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_OSP_CAB_NET_FRM_064, layer_title); //Resources.Resources.SI_OSP_ADB_NET_FRM_009;
                        objADBItem = resultItem;
                    }
                }
                else
                {
                    objADBItem.objPM.message = getFirstErrorFromModelState();
                    objADBItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objADBItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objADBItem, EntityType.ADB.ToString());
                new BLMisc().BindPortDetails(objADBItem, EntityType.ADB.ToString(), DropDownType.Adb_Port_Ratio.ToString());
                response.results = objADBItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveADBTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing ADB Request.";
            }
            return response;
        }
        #endregion

        #endregion

        #region Splitter
        #region Get Splitter Template 
        /// <summary>GetSplitterTemplate </summary>
        /// <returns>Splitter Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>18-feb-2021</CreatedDate>

        public ApiResponse<SplitterItemMaster> GetSplitterTemplate(ReqInput data)
        {
            var response = new ApiResponse<SplitterItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                SplitterItemMaster objSplitterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SplitterItemMaster>(objItemTemplateIn.userId, EntityType.Splitter);
                objSplitterItemMaster.pEntityType = objItemTemplateIn.pEntityType;
                BindSplitterDropDown(objSplitterItemMaster);
                BLItemTemplate.Instance.BindItemDropdowns(objSplitterItemMaster, EntityType.Splitter.ToString());
                if (objSplitterItemMaster.pEntityType == EntityType.FDB.ToString())
                {
                    if (ApplicationSettings.splitterTypeForFat == 2)
                    {
                        objSplitterItemMaster.lstType = objSplitterItemMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Secondary.ToString()).ToList();
                    }
                    else if (ApplicationSettings.splitterTypeForFat == 1)
                    {
                        objSplitterItemMaster.lstType = objSplitterItemMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Primary.ToString()).ToList();
                    }
                    else
                    {
                        objSplitterItemMaster.lstType = objSplitterItemMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString()).ToList();
                    }
                }
                if (objSplitterItemMaster.pEntityType == EntityType.BDB.ToString())
                {
                    if (ApplicationSettings.splitterTypeForFdc == 2)
                    {
                        objSplitterItemMaster.lstType = objSplitterItemMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Secondary.ToString()).ToList();
                    }
                    else if (ApplicationSettings.splitterTypeForFdc == 1)
                    {
                        objSplitterItemMaster.lstType = objSplitterItemMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString() && x.value == EntityCategory.Primary.ToString()).ToList();
                    }
                    else
                    {
                        objSplitterItemMaster.lstType = objSplitterItemMaster.lstType.Where(x => x.ddtype == DropDownType.TypeMaster.ToString()).ToList();
                    }
                }
                objSplitterItemMaster.unitValue = objSplitterItemMaster.splitter_ratio;
                objSplitterItemMaster.formInputSettings = new BLFormInputSettings().getformInputSettings();
                response.status = StatusCodes.OK.ToString();
                response.results = objSplitterItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetSplitterTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Splitter Template!";
            }
            return response;
        }
        #endregion

        #region Bind Splitter DropDown 
        /// <summary>BindSplitterDropDown </summary>
        /// <returns>Splitter Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>18-feb-2021</CreatedDate>
        private void BindSplitterDropDown(SplitterItemMaster objSplitterItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Splitter.ToString());
            //objSplitterItemMaster.lstSplRatio = objDDL.Where(x => x.dropdown_type == DropDownType.Splitter_Ratio.ToString()).ToList();
            new BLMisc().BindPortDetails(objSplitterItemMaster, EntityType.Splitter.ToString(), DropDownType.Splitter_Ratio.ToString());
        }
        #endregion

        #region Save Splitter Template 
        /// <summary>SaveSplitterTemplate </summary>
        /// <returns>Splitter Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>18-feb-2021</CreatedDate>
        public ApiResponse<SplitterItemMaster> SaveSplitterTemplate(ReqInput data)
        {
            var response = new ApiResponse<SplitterItemMaster>();
            try
            {
                SplitterItemMaster objSplitterItem = ReqHelper.GetRequestData<SplitterItemMaster>(data);
                if (!string.IsNullOrEmpty(objSplitterItem.unitValue))
                    objSplitterItem.splitter_ratio = objSplitterItem.unitValue; 

                this.Validate(objSplitterItem);
                if (ModelState.IsValid)
                {
                    var itemid = objSplitterItem.id;
                    var resultItem = new BLSplitterItemMaster().SaveSplitterItemTemplate(objSplitterItem, objSplitterItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objSplitterItem.objPM.status = ResponseStatus.OK.ToString();
                        objSplitterItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_077;// "Splitter template update successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_077;//"Splitter template update successfully.";
                    }
                    else
                    {
                        objSplitterItem.objPM.status = ResponseStatus.OK.ToString();
                        objSplitterItem.objPM.message = Resources.Resources.SI_GBL_GBL_GBL_FRM_042;// "Splitter template saved successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_GBL_GBL_GBL_FRM_042;// "Splitter template saved successfully.";
                        objSplitterItem = resultItem;
                    }
                }
                else
                {
                    objSplitterItem.objPM.message = getFirstErrorFromModelState();
                    objSplitterItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objSplitterItem;
                }
                //fill dropdown
                BindSplitterDropDown(objSplitterItem);
                BLItemTemplate.Instance.BindItemDropdowns(objSplitterItem, EntityType.Splitter.ToString());
                objSplitterItem.formInputSettings = new BLFormInputSettings().getformInputSettings();
                response.results = objSplitterItem;

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveSplitterTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing Splitter Request.";
            }
            return response;
        }
        #endregion

        #endregion

        #region SpliceClosure

        #region GetSCTemplate
        /// <summary> GetSCTemplate </summary>
        /// <returns>SpliceClosure Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>16-feb-2021</CreatedDate>
        public ApiResponse<SCItemMaster> GetSCTemplate(ReqInput data)
        {
            var response = new ApiResponse<SCItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                SCItemMaster objSCItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SCItemMaster>(objItemTemplateIn.userId, EntityType.SpliceClosure);
                objSCItemMaster.no_of_port = objSCItemMaster.no_of_ports;
                BLItemTemplate.Instance.BindItemDropdowns(objSCItemMaster, EntityType.SpliceClosure.ToString());
                new BLMisc().BindPortDetails(objSCItemMaster, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objSCItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetSCTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Splice Closure Template!";
            }
            return response;
        }
        #endregion

        #region Save SpliceClosure Template
        /// <summary>SaveSpliceClosureTemplate </summary>
        /// <returns>SpliceClosure Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>05-Jan-2021</CreatedDate>
        public ApiResponse<SCItemMaster> SaveSpliceClosureTemplate(ReqInput data)
        {
            SCItemMaster objSCItem = ReqHelper.GetRequestData<SCItemMaster>(data);
            var response = new ApiResponse<SCItemMaster>();
            try
            {
                this.Validate(objSCItem);

                objSCItem.no_of_ports = objSCItem.no_of_port;
                if (ModelState.IsValid)
                {
                    if (objSCItem.unitValue != null && objSCItem.unitValue.Contains(":"))
                    {
                        objSCItem.no_of_input_port = Convert.ToInt32(objSCItem.unitValue.Split(':')[0]);
                        objSCItem.no_of_output_port = Convert.ToInt32(objSCItem.unitValue.Split(':')[1]);
                    }
                    var itemid = objSCItem.id;
                    var resultItem = new BLSCItemMaster().SaveSCItemTemplate(objSCItem, objSCItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objSCItem.objPM.status = ResponseStatus.OK.ToString();
                        objSCItem.objPM.message = Resources.Resources.SI_OSP_SC_NET_FRM_016;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_SC_NET_FRM_016;
                    }
                    else
                    {
                        objSCItem.objPM.status = ResponseStatus.OK.ToString();
                        objSCItem.objPM.message = Resources.Resources.SI_OSP_SC_NET_FRM_017;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_SC_NET_FRM_017;
                        objSCItem = resultItem;
                    }
                }
                else
                {
                    objSCItem.objPM.message = getFirstErrorFromModelState();
                    objSCItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objSCItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objSCItem, EntityType.SpliceClosure.ToString());
                new BLMisc().BindPortDetails(objSCItem, EntityType.SpliceClosure.ToString(), DropDownType.SC_Port_Ratio.ToString());
                response.results = objSCItem;

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveSpliceClosureTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        #endregion

        #region ONT
        #region GetONTTemplate
        /// <summary> GetONTTemplate </summary>
        /// <returns>ONT Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>18-feb-2021</CreatedDate>
        public ApiResponse<ONTItemMaster> GetONTTemplate(ReqInput data)
        {
            var response = new ApiResponse<ONTItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                ONTItemMaster objONTItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ONTItemMaster>(objItemTemplateIn.userId, EntityType.ONT);
                BLItemTemplate.Instance.BindItemDropdowns(objONTItemMaster, EntityType.ONT.ToString());
                new BLMisc().BindPortDetails(objONTItemMaster, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objONTItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetONTTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching ONT Template!";
            }
            return response;
        }
        #endregion

        #region Save ONT Template 
        /// <summary>SaveONTTemplate </summary>
        /// <returns>ONT Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>18-Feb-2021</CreatedDate>
        public ApiResponse<ONTItemMaster> SaveONTTemplate(ReqInput data)
        {
            var response = new ApiResponse<ONTItemMaster>();
            try
            {
                ONTItemMaster objONTItem = ReqHelper.GetRequestData<ONTItemMaster>(data);
                this.Validate(objONTItem);
                if (ModelState.IsValid)
                {
                    var itemid = objONTItem.id;
                    if (objONTItem.unitValue != null && objONTItem.unitValue.Contains(":"))
                    {
                        objONTItem.no_of_input_port = Convert.ToInt32(objONTItem.unitValue.Split(':')[0]);
                        objONTItem.no_of_output_port = Convert.ToInt32(objONTItem.unitValue.Split(':')[1]);
                    }
                    var resultItem = new BLONTItemMaster().SaveONTItemTemplate(objONTItem, objONTItem.user_id);
                    var layer_title = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.ONT.ToString().ToUpper()).FirstOrDefault().layer_title;
                    if (itemid > 0)  // Update 
                    {
                        objONTItem.objPM.status = ResponseStatus.OK.ToString();
                        objONTItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//Resources.Resources.SI_OSP_ONT_NET_FRM_014;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title); //Resources.Resources.SI_OSP_ONT_NET_FRM_014;
                    }
                    else
                    {
                        objONTItem.objPM.status = ResponseStatus.OK.ToString();
                        objONTItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//Resources.Resources.SI_OSP_ONT_NET_FRM_015;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//Resources.Resources.SI_OSP_ONT_NET_FRM_015;
                        objONTItem = resultItem;
                    }
                }
                else
                {
                    objONTItem.objPM.message = getFirstErrorFromModelState();
                    objONTItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objONTItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objONTItem, EntityType.ONT.ToString());
                new BLMisc().BindPortDetails(objONTItem, EntityType.ONT.ToString(), DropDownType.Ont_Port_Ratio.ToString());
                response.results = objONTItem;

            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveONTTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region FDB
        #region GetFDBTemplate
        /// <summary> GetFDBTemplate </summary>
        /// <returns>FDB Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>20-feb-2021</CreatedDate>
        public ApiResponse<FDBTemplate> GetFDBTemplate(ReqInput data)
        {
            var response = new ApiResponse<FDBTemplate>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                //var objFDBTemplate = BLISP.Instance.getFDBTemplate(objItemTemplateIn.userId);
				FDBTemplate objFDBTemplate = BLItemTemplate.Instance.GetTemplateDetail<FDBTemplate>(objItemTemplateIn.userId, EntityType.FDB);
				BLItemTemplate.Instance.BindItemDropdowns(objFDBTemplate, EntityType.FDB.ToString());
                new BLMisc().BindPortDetails(objFDBTemplate, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objFDBTemplate;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetFDBTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching FDB Template!";
            }
            return response;
        }
        #endregion

        #region Save FDB Template 
        /// <summary>SaveFDBTemplate </summary>
        /// <returns>FDB Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>20-Feb-2021</CreatedDate>
        public ApiResponse<FDBTemplate> SaveFDBTemplate(ReqInput data)
        {
            var response = new ApiResponse<FDBTemplate>();
            try
            {
                FDBTemplate objFDBTemplate = ReqHelper.GetRequestData<FDBTemplate>(data);
                this.Validate(objFDBTemplate);
                if (ModelState.IsValid)
                {
                    var fdbTemplate = new FDBTemplate();


                    int unitSystemId = objFDBTemplate.system_id;
                    if (objFDBTemplate.unitValue != null && objFDBTemplate.unitValue.Contains(":"))
                    {
                        objFDBTemplate.no_of_input_port = Convert.ToInt32(objFDBTemplate.unitValue.Split(':')[0]);
                        objFDBTemplate.no_of_output_port = Convert.ToInt32(objFDBTemplate.unitValue.Split(':')[1]);
                    }
                    fdbTemplate = new BLISP().saveFDBTemplate(objFDBTemplate,objFDBTemplate.user_id);

                    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.FDB.ToString().ToUpper()).FirstOrDefault().layer_title;
                    if (unitSystemId > 0)
                    {

                        fdbTemplate.objPM.status = ResponseStatus.OK.ToString(); ;
                        fdbTemplate.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template updated successfully.
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template updated successfully.
                    }
                    else
                    {
                        fdbTemplate.objPM.status = ResponseStatus.OK.ToString(); ;
                        fdbTemplate.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title); //{0} template saved successfully."
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//{0} template updated successfully.                    }
                    }
                    //fill dropdown
                    BLItemTemplate.Instance.BindItemDropdowns(fdbTemplate, EntityType.FDB.ToString());
                    new BLMisc().BindPortDetails(fdbTemplate, EntityType.FDB.ToString(), DropDownType.Fdb_Port_Ratio.ToString());
                    response.results = fdbTemplate;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveFDBTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region CDB
        #region Get Cdb Template 
        /// <summary>GetCdbTemplate</summary>
        /// <returns>Cdb Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>24-feb-2021</CreatedDate>

        public ApiResponse<CDBItemMaster> GetCdbTemplate(ReqInput data)
        {
            var response = new ApiResponse<CDBItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                CDBItemMaster objCDBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<CDBItemMaster>(objItemTemplateIn.userId, EntityType.CDB);
                BLItemTemplate.Instance.BindItemDropdowns(objCDBItemMaster, EntityType.CDB.ToString());
                new BLMisc().BindPortDetails(objCDBItemMaster, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
                var objDDL = new BLMisc().GetDropDownList(EntityType.CDB.ToString(), DropDownType.Entity_Type.ToString());
                objCDBItemMaster.lstEntityType = objDDL;
                response.status = StatusCodes.OK.ToString();
                response.results = objCDBItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetCdbTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Pole Template!";
            }
            return response;
        }
        #endregion

        #region Save Cdb Template 
        /// <summary>SaveCdbTemplate </summary>
        /// <returns>Cdb Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>24-feb-2021</CreatedDate>
        public ApiResponse<CDBItemMaster> SaveCdbTemplate(ReqInput data)
        {
            var response = new ApiResponse<CDBItemMaster>();
            try
            {
                CDBItemMaster objCDBItem = ReqHelper.GetRequestData<CDBItemMaster>(data);
                this.Validate(objCDBItem);
                if (ModelState.IsValid)
                {
                    var itemid = objCDBItem.id;
                    if (objCDBItem.unitValue != null && objCDBItem.unitValue.Contains(":"))
                    {
                        objCDBItem.no_of_input_port = Convert.ToInt32(objCDBItem.unitValue.Split(':')[0]);
                        objCDBItem.no_of_output_port = Convert.ToInt32(objCDBItem.unitValue.Split(':')[1]);
                    }
                    var resultItem = new BLCDBItemMaster().SaveCDBItemTemplate(objCDBItem, objCDBItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objCDBItem.objPM.status = ResponseStatus.OK.ToString();
                        objCDBItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_018;// "CDB template update successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_018;// "CDB template update successfully.";
                    }
                    else
                    {
                        objCDBItem.objPM.status = ResponseStatus.OK.ToString();
                        objCDBItem.objPM.message = Resources.Resources.SI_GBL_GBL_GBL_GBL_099;// "CDB template  saved successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_GBL_GBL_GBL_GBL_099;// "CDB template update successfully.";
                        objCDBItem = resultItem;
                    }
                }
                else
                {
                    objCDBItem.objPM.message = getFirstErrorFromModelState();
                    objCDBItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objCDBItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objCDBItem, EntityType.CDB.ToString());
                new BLMisc().BindPortDetails(objCDBItem, EntityType.CDB.ToString(), DropDownType.Cdb_Port_Ratio.ToString());
                response.results = objCDBItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveCdbTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #endregion

        #region BDB
        #region Get Bdb Template 
        /// <summary>GetBdbTemplate</summary>
        /// <returns>Bdb Template Details</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>24-feb-2021</CreatedDate>

        public ApiResponse<BDBItemMaster> GetBdbTemplate(ReqInput data)
        {
            var response = new ApiResponse<BDBItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                BDBItemMaster objBDBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<BDBItemMaster>(objItemTemplateIn.userId, EntityType.BDB);
                BLItemTemplate.Instance.BindItemDropdowns(objBDBItemMaster, EntityType.BDB.ToString());
                new BLMisc().BindPortDetails(objBDBItemMaster, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
                var objDDL = new BLMisc().GetDropDownList(EntityType.BDB.ToString(), DropDownType.Entity_Type.ToString());
                objBDBItemMaster.lstEntityType = objDDL;
                response.status = StatusCodes.OK.ToString();
                response.results = objBDBItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetBdbTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Pole Template!";
            }
            return response;
        }
        #endregion

        #region Save Bdb Template 
        /// <summary>SaveBdbTemplate </summary>
        /// <returns>Bdb Template Status</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        /// <CreatedDate>24-feb-2021</CreatedDate>
        public ApiResponse<BDBItemMaster> SaveBdbTemplate(ReqInput data)
        {
            var response = new ApiResponse<BDBItemMaster>();
            try
            {
                BDBItemMaster objBDBItem = ReqHelper.GetRequestData<BDBItemMaster>(data);
                this.Validate(objBDBItem);
                if (ModelState.IsValid)
                {
                    var layer_title = ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.BDB.ToString().ToUpper()).FirstOrDefault().layer_title;
                    var itemid = objBDBItem.id;
                    if (objBDBItem.unitValue != null && objBDBItem.unitValue.Contains(":"))
                    {
                        objBDBItem.no_of_input_port = Convert.ToInt32(objBDBItem.unitValue.Split(':')[0]);
                        objBDBItem.no_of_output_port = Convert.ToInt32(objBDBItem.unitValue.Split(':')[1]);
                    }
                    var resultItem = new BLBDBItemMaster().SaveBDBItemTemplate(objBDBItem, objBDBItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objBDBItem.objPM.status = ResponseStatus.OK.ToString();
                        objBDBItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template update successfully.
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//{0} template update successfully.
                    }
                    else
                    {
                        objBDBItem.objPM.status = ResponseStatus.OK.ToString();
                        objBDBItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//{0} template saved successfully.
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//{0} template saved successfully.
                        objBDBItem = resultItem;
                    }
                }
                else
                {
                    objBDBItem.objPM.message = getFirstErrorFromModelState();
                    objBDBItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objBDBItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objBDBItem, EntityType.BDB.ToString());
                new BLMisc().BindPortDetails(objBDBItem, EntityType.BDB.ToString(), DropDownType.BDB_PORT_RATIO.ToString());
                response.results = objBDBItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveBdbTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion

        #endregion

        #region FMS
        #region GetFMSTemplate
        /// <summary> GetFMSTemplate </summary>
        /// <returns>FMS Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>24-feb-2021</CreatedDate>
        public ApiResponse<FMSItemMaster> GetFMSTemplate(ReqInput data)
        {
            var response = new ApiResponse<FMSItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                FMSItemMaster objFMSItemMaster = BLItemTemplate.Instance.GetTemplateDetail<FMSItemMaster>(objItemTemplateIn.userId, EntityType.FMS);
                BLItemTemplate.Instance.BindItemDropdowns(objFMSItemMaster, EntityType.FMS.ToString());
                new BLMisc().BindPortDetails(objFMSItemMaster, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objFMSItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetFMSTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching FMS Template!";
            }
            return response;
        }
        #endregion

        #region Save FMS Template 
        /// <summary>SaveFMSTemplate </summary>
        /// <returns>FMS Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>24-Feb-2021</CreatedDate>
        public ApiResponse<FMSItemMaster> SaveFMSTemplate(ReqInput data)
        {
            var response = new ApiResponse<FMSItemMaster>();
            try
            {
                FMSItemMaster objFMSItem = ReqHelper.GetRequestData<FMSItemMaster>(data);
                this.Validate(objFMSItem);
                if (ModelState.IsValid)
                {
                    if (objFMSItem.unitValue != null && objFMSItem.unitValue.Contains(":"))
                    {
                        objFMSItem.no_of_input_port = Convert.ToInt32(objFMSItem.unitValue.Split(':')[0]);
                        objFMSItem.no_of_output_port = Convert.ToInt32(objFMSItem.unitValue.Split(':')[1]);
                    }
                    var itemid = objFMSItem.id;
                    var resultItem = new BLFMSItemMaster().SaveFMSItemTemplate(objFMSItem, objFMSItem.user_id);
                    var layer_title = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.FMS.ToString().ToUpper()).FirstOrDefault().layer_title;
                    if (itemid > 0)  // Update 
                    {
                        objFMSItem.objPM.status = ResponseStatus.OK.ToString();
                        objFMSItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title); //"ODF template update successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//"ODF template update successfully.";
                    }
                    else
                    {
                        objFMSItem.objPM.status = ResponseStatus.OK.ToString();
                        objFMSItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);// "ODF template saved successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//"ODF template saved successfully.";
                        objFMSItem = resultItem;
                    }
                }
                else
                {
                    objFMSItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objFMSItem.objPM.message = "Please fill mandatory field !";
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = "Please fill mandatory field !";
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objFMSItem, EntityType.FMS.ToString());
                new BLMisc().BindPortDetails(objFMSItem, EntityType.FMS.ToString(), DropDownType.FMS_Port_Ratio.ToString());
                response.results = objFMSItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveFDBTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region HTB
        #region GetHTBTemplate
        /// <summary> GetHTBTemplate </summary>
        /// <returns>HTB Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>24-feb-2021</CreatedDate>
        public ApiResponse<HTBTemplate> GetHTBTemplate(ReqInput data)
        {
            var response = new ApiResponse<HTBTemplate>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                // var objHTBTemplate = BLISP.Instance.getHTBTemplate(objItemTemplateIn.userId);
                HTBTemplate objHTBItemMaster = BLItemTemplate.Instance.GetTemplateDetail<HTBTemplate>(objItemTemplateIn.userId, EntityType.HTB);
                objHTBItemMaster.created_by = objItemTemplateIn.userId;
                BLItemTemplate.Instance.BindItemDropdowns(objHTBItemMaster, EntityType.HTB.ToString());
                new BLMisc().BindPortDetails(objHTBItemMaster, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objHTBItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetHTBTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching HTB Template!";
            }
            return response;
        }
        #endregion

        #region Save HTB Template 
        /// <summary>SaveHTBTemplate </summary>
        /// <returns>HTB Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>24-Feb-2021</CreatedDate>
        public ApiResponse<HTBTemplate> SaveHTBTemplate(ReqInput data)
        {
            var response = new ApiResponse<HTBTemplate>();
            try
            {
                HTBTemplate objHTBTemplate = ReqHelper.GetRequestData<HTBTemplate>(data);
                var htbTemplate = new HTBTemplate();
                this.Validate(objHTBTemplate);
                if (ModelState.IsValid)
                {
                    int unitSystemId = objHTBTemplate.system_id;
                    if (objHTBTemplate.unitValue != null && objHTBTemplate.unitValue.Contains(":"))
                    {
                        objHTBTemplate.no_of_input_port = Convert.ToInt32(objHTBTemplate.unitValue.Split(':')[0]);
                        objHTBTemplate.no_of_output_port = Convert.ToInt32(objHTBTemplate.unitValue.Split(':')[1]);
                    }
                    htbTemplate = new BLISP().saveHTBTemplate(objHTBTemplate);
                    if (unitSystemId > 0)
                    {

                        htbTemplate.objPM.status = ResponseStatus.OK.ToString(); ;
                        htbTemplate.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_GBL_147;// "HTB template updated successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_GBL_147;// "HTB template updated successfully.";
                        response.results = htbTemplate;

                    }
                    else
                    {
                        htbTemplate.objPM.status = ResponseStatus.OK.ToString();
                        htbTemplate.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_019;// "HTB template saved successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_019;
                        response.results = htbTemplate;

                    }
                }
                else
                {
                    htbTemplate.objPM.message = getFirstErrorFromModelState();
                    htbTemplate.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = htbTemplate;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(htbTemplate, EntityType.HTB.ToString());
                new BLMisc().BindPortDetails(htbTemplate, EntityType.HTB.ToString(), DropDownType.Htb_Port_Ratio.ToString());
                response.results = htbTemplate;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveHTBTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region OpticalRepeater
        #region GetOpticalRepeaterTemplate
        /// <summary> GetOpticalRepeaterTemplate </summary>
        /// <returns>OpticalRepeater Template Details</returns>
        /// <CreatedBy>Tapish</CreatedBy>
        /// <CreatedDate>04-may-2021</CreatedDate>
        public ApiResponse<OpticalRepeaterTemplate> GetOpticalRepeaterTemplate(ReqInput data)
        {
            var response = new ApiResponse<OpticalRepeaterTemplate>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                // var objHTBTemplate = BLISP.Instance.getHTBTemplate(objItemTemplateIn.userId);
                OpticalRepeaterTemplate objOpticalRepeaterItemMaster = BLItemTemplate.Instance.GetTemplateDetail<OpticalRepeaterTemplate>(objItemTemplateIn.userId, EntityType.OpticalRepeater);
                objOpticalRepeaterItemMaster.created_by = objItemTemplateIn.userId;
                BLItemTemplate.Instance.BindItemDropdowns(objOpticalRepeaterItemMaster, EntityType.OpticalRepeater.ToString());
                new BLMisc().BindPortDetails(objOpticalRepeaterItemMaster, EntityType.OpticalRepeater.ToString(), DropDownType.OpticalRepeater_Port_Ratio.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objOpticalRepeaterItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetOpticalRepeaterTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching OpticalRepeater Template!";
            }
            return response;
        }
        #endregion

        #region Save OpticalRepeater Template 
        /// <summary>SaveOpticalRepeaterTemplate </summary>
        /// <returns>OpticalRepeater Template Status</returns>
        /// <CreatedBy>Tapish</CreatedBy>
        /// <CreatedDate>04-May-2021</CreatedDate>
        public ApiResponse<OpticalRepeaterTemplate> SaveOpticalRepeaterTemplate(ReqInput data)
        {
            var response = new ApiResponse<OpticalRepeaterTemplate>();
            try
            {
                OpticalRepeaterTemplate objOpticalRepeaterTemplate = ReqHelper.GetRequestData<OpticalRepeaterTemplate>(data);
                var OpticalRepeaterTemplate = new OpticalRepeaterTemplate();
                this.Validate(objOpticalRepeaterTemplate);
                if (ModelState.IsValid)
                {
                    int unitSystemId = objOpticalRepeaterTemplate.system_id;
                    if (objOpticalRepeaterTemplate.unitValue != null && objOpticalRepeaterTemplate.unitValue.Contains(":"))
                    {
                        objOpticalRepeaterTemplate.no_of_input_port = Convert.ToInt32(objOpticalRepeaterTemplate.unitValue.Split(':')[0]);
                        objOpticalRepeaterTemplate.no_of_output_port = Convert.ToInt32(objOpticalRepeaterTemplate.unitValue.Split(':')[1]);
                    }
                    OpticalRepeaterTemplate = new BLISP().saveOpticalRepeaterTemplate(objOpticalRepeaterTemplate);
                    if (unitSystemId > 0)
                    {

                        OpticalRepeaterTemplate.objPM.status = ResponseStatus.OK.ToString(); ;
                        OpticalRepeaterTemplate.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_079;// "OpticalRepeater template updated successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_079;// "OpticalRepeater template updated successfully.";
                        response.results = OpticalRepeaterTemplate;

                    }
                    else
                    {
                        OpticalRepeaterTemplate.objPM.status = ResponseStatus.OK.ToString();
                        OpticalRepeaterTemplate.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_078;// "OpticalRepeater template saved successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_078;//"OpticalRepeater template saved successfully.";
                        response.results = OpticalRepeaterTemplate;

                    }
                }
                else
                {
                    OpticalRepeaterTemplate.objPM.message = getFirstErrorFromModelState();
                    OpticalRepeaterTemplate.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = OpticalRepeaterTemplate;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(OpticalRepeaterTemplate, EntityType.OpticalRepeater.ToString());
                new BLMisc().BindPortDetails(OpticalRepeaterTemplate, EntityType.OpticalRepeater.ToString(), DropDownType.OpticalRepeater_Port_Ratio.ToString());
                response.results = OpticalRepeaterTemplate;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveOpticalRepeaterTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region Coupler
        #region GetCouplerTemplate
        /// <summary> GetCouplerTemplate </summary>
        /// <returns>Coupler Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>04-Mar-2021</CreatedDate>
        public ApiResponse<CouplerItemMaster> GetCouplerTemplate(ReqInput data)
        {
            var response = new ApiResponse<CouplerItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                CouplerItemMaster objCouplerItemMaster = BLItemTemplate.Instance.GetTemplateDetail<CouplerItemMaster>(objItemTemplateIn.userId, EntityType.Coupler);
                BLItemTemplate.Instance.BindItemDropdowns(objCouplerItemMaster, EntityType.Coupler.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objCouplerItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetCouplerTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching HTB Template!";
            }
            return response;
        }
        #endregion

        #region Save Coupler Template 
        /// <summary>SaveCouplerTemplate </summary>
        /// <returns>Coupler Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>04-Mar-2021</CreatedDate>
        public ApiResponse<CouplerItemMaster> SaveCouplerTemplate(ReqInput data)
        {
            var response = new ApiResponse<CouplerItemMaster>();
            try
            {
                CouplerItemMaster objCouplerItem = ReqHelper.GetRequestData<CouplerItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objCouplerItem.id;
                    var resultItem = new BLCouplerItemMaster().SaveCouplerItemTemplate(objCouplerItem, objCouplerItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objCouplerItem.objPM.status = ResponseStatus.OK.ToString();
                        objCouplerItem.objPM.message = Resources.Resources.SI_OSP_CPR_NET_FRM_003;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_CPR_NET_FRM_003;
                    }
                    else
                    {
                        objCouplerItem.objPM.status = ResponseStatus.OK.ToString();
                        objCouplerItem.objPM.message = Resources.Resources.SI_OSP_CPR_NET_FRM_004;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_CPR_NET_FRM_004;
                        objCouplerItem = resultItem;
                    }
                }
                else
                {
                    objCouplerItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objCouplerItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objCouplerItem, EntityType.Coupler.ToString());
                response.results = objCouplerItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveCouplerTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region Trench
        #region GetTrenchTemplate
        /// <summary> GetTrenchTemplate </summary>
        /// <returns>Trench Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<TrenchItemMaster> GetTrenchTemplate(ReqInput data)
        {
            var response = new ApiResponse<TrenchItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                TrenchItemMaster objTrenchItemMaster = BLItemTemplate.Instance.GetTemplateDetail<TrenchItemMaster>(objItemTemplateIn.userId, EntityType.Trench);
                BindTrenchDropDown(objTrenchItemMaster);
                BLItemTemplate.Instance.BindItemDropdowns(objTrenchItemMaster, EntityType.Trench.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objTrenchItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetTrenchTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching HTB Template!";
            }
            return response;
        }
        #endregion

        #region Save Trench Template 
        /// <summary>SaveTrenchTemplate </summary>
        /// <returns>Trench Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<TrenchItemMaster> SaveTrenchTemplate(ReqInput data)
        {
            var response = new ApiResponse<TrenchItemMaster>();
            try
            {
                TrenchItemMaster objTrenchItem = ReqHelper.GetRequestData<TrenchItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objTrenchItem.id;
                    var resultItem = new BLTrenchItemMaster().SaveTrenchItemTemplate(objTrenchItem, objTrenchItem.user_Id);

                    if (itemid > 0)  // Update 
                    {
                        objTrenchItem.objPM.status = ResponseStatus.OK.ToString();
                        objTrenchItem.objPM.message = Resources.Resources.SI_OSP_TCH_NET_FRM_018;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_TCH_NET_FRM_018;
                    }
                    else
                    {
                        objTrenchItem.objPM.status = ResponseStatus.OK.ToString();
                        objTrenchItem.objPM.message = Resources.Resources.SI_OSP_TCH_NET_FRM_019;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_TCH_NET_FRM_019;
                        objTrenchItem = resultItem;
                    }
                }
                else
                {
                    objTrenchItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objTrenchItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                }
                ////fill dropdown
                BindTrenchDropDown(objTrenchItem);
                BLItemTemplate.Instance.BindItemDropdowns(objTrenchItem, EntityType.Trench.ToString());
                response.results = objTrenchItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveTrenchTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #region Bind Trench DropDown
        /// <summary> BindTrenchDropDown </summary>
        /// <returns>lst Trench type</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// 
        private void BindTrenchDropDown(TrenchItemMaster objTrenchItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Trench.ToString());
            objTrenchItemMaster.trenchTypeIn = objDDL.Where(x => x.dropdown_type == DropDownType.Trench_Type.ToString()).ToList();
            objTrenchItemMaster.lstTrenchServingType = objDDL.Where(x => x.dropdown_type == DropDownType.trench_serving_type.ToString()).ToList();
        }
        #endregion

        #endregion

        #region Duct
        #region GetDuctTemplate
        /// <summary> GetDuctTemplate </summary>
        /// <returns>Duct Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<DuctItemMaster> GetDuctTemplate(ReqInput data)
        {
            var response = new ApiResponse<DuctItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                DuctItemMaster objDuctItemMaster = BLItemTemplate.Instance.GetTemplateDetail<DuctItemMaster>(objItemTemplateIn.userId, EntityType.Duct);
                BLItemTemplate.Instance.BindItemDropdowns(objDuctItemMaster, EntityType.Duct.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objDuctItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetDuctTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching HTB Template!";
            }
            return response;
        }
        #endregion
        #endregion
        #region Conduit
        #region GetConduitTemplate
        /// <summary> GetConduitTemplate </summary>
        /// <returns>Conduit Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<ConduitItemMaster> GetConduitTemplate(ReqInput data)
        {
            var response = new ApiResponse<ConduitItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                ConduitItemMaster objConduitItemMaster = BLItemTemplate.Instance.GetTemplateDetail<ConduitItemMaster>(objItemTemplateIn.userId, EntityType.Conduit);
                BLItemTemplate.Instance.BindItemDropdowns(objConduitItemMaster, EntityType.Conduit.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objConduitItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetConduitTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching HTB Template!";
            }
            return response;
        }
        #endregion

        #endregion

        #region GetMicroductTemplate
        /// <summary> GetMicroductTemplate </summary>
        /// <returns>Microduct Template Details</returns>
        /// <CreatedBy>Alok Misra</CreatedBy>
        /// <CreatedDate>06-April-2021</CreatedDate>
        public ApiResponse<MicroductItemMaster> GetMicroductTemplate(ReqInput data)
		{
			var response = new ApiResponse<MicroductItemMaster>();
			try
			{
				ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
				MicroductItemMaster objMicroductItemMaster = BLItemTemplate.Instance.GetTemplateDetail<MicroductItemMaster>(objItemTemplateIn.userId, EntityType.Microduct);
				BLItemTemplate.Instance.BindItemDropdowns(objMicroductItemMaster, EntityType.Microduct.ToString());
                objMicroductItemMaster.lstNoOfWays = BLItemTemplate.Instance.GetMicroDuctData().ToList();
                response.status = StatusCodes.OK.ToString();
				response.results = objMicroductItemMaster;
			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("GetMicroductTemplate()", "Item Template Controller", data.data, ex);
				response.status = StatusCodes.UNKNOWN_ERROR.ToString();
				response.error_message = "Error While fetching Microduct Template!";
			}
			return response;
		}
		#endregion


		#region Save Microduct Template 
		/// <summary>SaveDuctTemplate </summary>
		/// <returns>Trench Template Status</returns>
		/// <CreatedBy>Antra Mathur</CreatedBy>
		/// <CreatedDate>11-Mar-2021</CreatedDate>
		public ApiResponse<MicroductItemMaster> SaveMicroductTemplate(ReqInput data)
		{
			var response = new ApiResponse<MicroductItemMaster>();
            try
            {
                MicroductItemMaster objDuctItem = ReqHelper.GetRequestData<MicroductItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objDuctItem.id;
                    var resultItem = new BLMicroductItemMaster().SaveMicroductItemTemplate(objDuctItem, objDuctItem.user_Id);

                    if (itemid > 0)  // Update 
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_NET_FRM_492;// "Microduct Template Updated successfully";
                        response.status = ResponseStatus.OK.ToString();
                        //response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_076;// "There is some issue while updating Microduct";
                    }
                    else
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_GBL_FRM_040;// "Microduct Template Saved successfully";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_GBL_GBL_GBL_FRM_041;// "There is some issue while saving Microduct";
                        objDuctItem = resultItem;
                    }
                }
                else
                {
                    objDuctItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objDuctItem;
                }
                //fill dropdown
                var objDDL = new BLMisc().GetDropDownList(EntityType.Microduct.ToString());
                objDuctItem.lstNoOfWays = objDDL.Where(x => x.dropdown_type == DropDownType.Number_of_Ways.ToString()).ToList();
                BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.Microduct.ToString());
                response.results = objDuctItem;
            }


            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveMicroductTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
			return response;
		}
        #endregion


        #region GetTowerTemplate
        /// <summary> GetTowerTemplate </summary>
        /// <returns>Tower Template Details</returns>
        /// <CreatedBy>Alok Misra</CreatedBy>
        /// <CreatedDate>06-April-2021</CreatedDate>
        public ApiResponse<TowerItemMaster> GetTowerTemplate(ReqInput data)
        {
            var response = new ApiResponse<TowerItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                TowerItemMaster objTowerItemMaster = BLItemTemplate.Instance.GetTemplateDetail<TowerItemMaster>(objItemTemplateIn.userId, EntityType.Tower);
                BLItemTemplate.Instance.BindItemDropdowns(objTowerItemMaster, EntityType.Tower.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objTowerItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetTowerTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Tower Template!";
            }
            return response;
        }
        #endregion

        #region Save Tower Template 
        /// <summary>SaveTowerTemplate </summary>
        /// <returns>Trench Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<TowerItemMaster> SaveTowerTemplate(ReqInput data)
        {
            var response = new ApiResponse<TowerItemMaster>();
            try
            {
                TowerItemMaster objDuctItem = ReqHelper.GetRequestData<TowerItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objDuctItem.id;
                    var resultItem = new BLTowerItemMaster().SaveTowerItemTemplate(objDuctItem, objDuctItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_057;//"Tower Updated successfully";
                        response.status = ResponseStatus.OK.ToString();
                        //response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_058;// "There is some issue while updating Tower";
                    }
                    else
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_059;// "Tower Saved successfully";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_060;//"There is some issue while saving Tower";
                        objDuctItem = resultItem;
                    }
                }
                else
                {
                    objDuctItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objDuctItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.Tower.ToString());
                response.results = objDuctItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuctTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion


        #region GetSectorTemplate
        /// <summary> GetSectorTemplate </summary>
        /// <returns>Sector Template Details</returns>
        /// <CreatedBy>Alok Misra</CreatedBy>
        /// <CreatedDate>06-April-2021</CreatedDate>
        public ApiResponse<SectorItemMaster> GetSectorTemplate(ReqInput data)
        {
            var response = new ApiResponse<SectorItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                SectorItemMaster objSectorItemMaster = BLItemTemplate.Instance.GetTemplateDetail<SectorItemMaster>(objItemTemplateIn.userId, EntityType.Sector);
                BLItemTemplate.Instance.BindItemDropdowns(objSectorItemMaster, EntityType.Sector.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objSectorItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetSectorTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Sector Template!";
            }
            return response;
        }
        #endregion

        #region Save Sector Template 
        /// <summary>SaveSectorTemplate </summary>
        /// <returns>Trench Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<SectorItemMaster> SaveSectorTemplate(ReqInput data)
        {
            var response = new ApiResponse<SectorItemMaster>();
            try
            {
                SectorItemMaster objDuctItem = ReqHelper.GetRequestData<SectorItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objDuctItem.id;
                    var resultItem = new BLSectorItemMaster().SaveSectorItemTemplate(objDuctItem, objDuctItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_061;// "Sector Template Updated successfully";
                        response.status = ResponseStatus.OK.ToString();
                        //response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_062;// "There is some issue while updating Sector";
                    }
                    else
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_063;// "Sector Template Saved successfully";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_064;// "There is some issue while saving Sector";
                        objDuctItem = resultItem;
                    }
                }
                else
                {
                    objDuctItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objDuctItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.Sector.ToString());
                response.results = objDuctItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuctTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion




        #region GetAntennaTemplate
        /// <summary> GetAntennaTemplate </summary>
        /// <returns>Antenna Template Details</returns>
        /// <CreatedBy>Alok Misra</CreatedBy>
        /// <CreatedDate>06-April-2021</CreatedDate>
        public ApiResponse<AntennaItemMaster> GetAntennaTemplate(ReqInput data)
        {
            var response = new ApiResponse<AntennaItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                AntennaItemMaster objAntennaItemMaster = BLItemTemplate.Instance.GetTemplateDetail<AntennaItemMaster>(objItemTemplateIn.userId, EntityType.Antenna);
                BLItemTemplate.Instance.BindItemDropdowns(objAntennaItemMaster, EntityType.Antenna.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objAntennaItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAntennaTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Antenna Template!";
            }
            return response;
        }
        #endregion

        #region Save Antenna Template 
        /// <summary>SaveAntennaTemplate </summary>
        /// <returns>Trench Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<AntennaItemMaster> SaveAntennaTemplate(ReqInput data)
        {
            var response = new ApiResponse<AntennaItemMaster>();
            try
            {
                AntennaItemMaster objDuctItem = ReqHelper.GetRequestData<AntennaItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objDuctItem.id;
                    var resultItem = new BLAntennaItemMaster().SaveAntennaItemTemplate(objDuctItem, objDuctItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_GBL_FRM_039;// "Antenna Template Updated successfully";
                        response.status = ResponseStatus.OK.ToString();
                        //response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_065;// "There is some issue while updating Antenna";
                    }
                    else
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_066;// "Antenna Template Saved successfully";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_067;// "There is some issue while saving Antenna";
                        objDuctItem = resultItem;
                    }
                }
                else
                {
                    objDuctItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objDuctItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.Antenna.ToString());
                response.results = objDuctItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuctTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion



        #region GetMicrowaveLinkTemplate
        /// <summary> GetMicrowaveLinkTemplate </summary>
        /// <returns>MicrowaveLink Template Details</returns>
        /// <CreatedBy>Alok Misra</CreatedBy>
        /// <CreatedDate>06-April-2021</CreatedDate>
        public ApiResponse<MicrowaveLinkItemMaster> GetMicrowaveLinkTemplate(ReqInput data)
        {
            var response = new ApiResponse<MicrowaveLinkItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                MicrowaveLinkItemMaster objMicrowaveLinkItemMaster = BLItemTemplate.Instance.GetTemplateDetail<MicrowaveLinkItemMaster>(objItemTemplateIn.userId, EntityType.MicrowaveLink);
                BLItemTemplate.Instance.BindItemDropdowns(objMicrowaveLinkItemMaster, EntityType.MicrowaveLink.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objMicrowaveLinkItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetMicrowaveLinkTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching MicrowaveLink Template!";
            }
            return response;
        }
        #endregion

        #region Save MicrowaveLink Template 
        /// <summary>SaveMicrowaveLinkTemplate </summary>
        /// <returns>Trench Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<MicrowaveLinkItemMaster> SaveMicrowaveLinkTemplate(ReqInput data)
        {
            var response = new ApiResponse<MicrowaveLinkItemMaster>();
            try
            {
                MicrowaveLinkItemMaster objDuctItem = ReqHelper.GetRequestData<MicrowaveLinkItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objDuctItem.id;
                    var resultItem = new BLMicrowaveLinkItemMaster().SaveMicrowaveLinkItemTemplate(objDuctItem, objDuctItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_068;// "MicrowaveLink Updated successfully";
                        response.status = ResponseStatus.OK.ToString();
                        //response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_069;// "There is some issue while updating MicrowaveLink";
                    }
                    else
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        //objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_070;// "MicrowaveLink Saved successfully";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_071;// "There is some issue while saving MicrowaveLink";
                        objDuctItem = resultItem;
                    }
                }
                else
                {
                    objDuctItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objDuctItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.MicrowaveLink.ToString());
                response.results = objDuctItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuctTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion




        #region Save Duct Template 
        /// <summary>SaveDuctTemplate </summary>
        /// <returns>Trench Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<DuctItemMaster> SaveDuctTemplate(ReqInput data)
        {
            var response = new ApiResponse<DuctItemMaster>();
            try
            {
                DuctItemMaster objDuctItem = ReqHelper.GetRequestData<DuctItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objDuctItem.id;
                    var resultItem = new BLDuctItemMaster().SaveDuctItemTemplate(objDuctItem, objDuctItem.user_Id);

                    if (itemid > 0)  // Update 
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                    }
                    else
                    {
                        objDuctItem.objPM.status = ResponseStatus.OK.ToString();
                        objDuctItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        objDuctItem = resultItem;
                    }
                }
                else
                {
                    objDuctItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objDuctItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objDuctItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objDuctItem, EntityType.Duct.ToString());
                response.results = objDuctItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuctTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion


        #region Save Conduit Template 
        /// <summary>SaveConduitTemplate </summary>
        /// <returns>Trench Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>11-Mar-2021</CreatedDate>
        public ApiResponse<ConduitItemMaster> SaveConduitTemplate(ReqInput data)
        {
            var response = new ApiResponse<ConduitItemMaster>();
            try
            {
                ConduitItemMaster objConduitItem = ReqHelper.GetRequestData<ConduitItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objConduitItem.id;
                    var resultItem = new BLConduitItemMaster().SaveConduitItemTemplate(objConduitItem, objConduitItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objConduitItem.objPM.status = ResponseStatus.OK.ToString();
                        //objConduitItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        objConduitItem.objPM.message= Resources.Resources.SI_OSP_GBL_GBL_FRM_056;
                        response.status = ResponseStatus.OK.ToString();
                        //response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_017;
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_053;
                    }
                    else
                    {
                        objConduitItem.objPM.status = ResponseStatus.OK.ToString();
                        //objConduitItem.objPM.message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        objConduitItem.objPM.message =Resources.Resources.SI_OSP_GBL_GBL_FRM_055;
                        response.status = ResponseStatus.OK.ToString();
                        //response.error_message = Resources.Resources.SI_OSP_DUC_NET_FRM_018;
                        response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_054;
                        objConduitItem = resultItem;
                    }
                }
                else
                {
                    objConduitItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objConduitItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objConduitItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objConduitItem, EntityType.Conduit.ToString());
                response.results = objConduitItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveConduitTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion





       

        #region Bind pole DropDown
        /// <summary> BindpoleDropDown </summary>
        /// <returns>lst pole type</returns>
        /// <CreatedBy>Sumit Poonia</CreatedBy>
        private void BindpoleDropDown(PoleItemMaster objPoleItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Pole.ToString());
            objPoleItemMaster.lstPoleType = objDDL.Where(x => x.dropdown_type == DropDownType.Pole_Type.ToString()).ToList();
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

        #region Cabinet
        #region Get Cabinet Template 
        
        public ApiResponse<CabinetItemMaster> GetCabinetTemplate(ReqInput data)
        {
            var response = new ApiResponse<CabinetItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                CabinetItemMaster objCabinetItemMaster = BLItemTemplate.Instance.GetTemplateDetail<CabinetItemMaster>(objItemTemplateIn.userId, EntityType.Cabinet);
                BLItemTemplate.Instance.BindItemDropdowns(objCabinetItemMaster, EntityType.Cabinet.ToString());
                BindCabinetDropdown(objCabinetItemMaster);
                response.status = StatusCodes.OK.ToString();
                response.results = objCabinetItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetCabinetTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Cabinet Template!";
            }
            return response;
        }
        #endregion

        #region Bind Cabinet Dropdown
        private void BindCabinetDropdown(CabinetItemMaster objCabinetItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Cabinet.ToString());
            objCabinetItemMaster.listCabinetType = objDDL.Where(x => x.dropdown_type == DropDownType.Cabinet_Type.ToString()).ToList();
        }
        #endregion

        #region Save Cabinet Template 
        
        public ApiResponse<CabinetItemMaster> SaveCabinetTemplate(ReqInput data)
        {
            var response = new ApiResponse<CabinetItemMaster>();
            try
            {
                CabinetItemMaster objCabinetItem = ReqHelper.GetRequestData<CabinetItemMaster>(data);
                this.Validate(objCabinetItem);
                if (ModelState.IsValid)
                {
                    if (ModelState.IsValid)
                    {
                        var itemid = objCabinetItem.id;
                        var resultItem = new BLCabinetItemMaster().SaveCabinetItemTemplate(objCabinetItem, objCabinetItem.userId);

                        if (itemid > 0)  // Update 
                        {
                            objCabinetItem.objPM.status = ResponseStatus.OK.ToString();
                            objCabinetItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_072;// "Cabinet template update successfully.";
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_072;// "Cabinet template update successfully.";
                        }
                        else
                        {
                            objCabinetItem.objPM.status = ResponseStatus.OK.ToString();
                            objCabinetItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_073;// "Cabinet template saved successfully";
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_073;//"Cabinet template saved successfully";
                            objCabinetItem = resultItem;
                        }
                    }
                    else
                    {
                        objCabinetItem.objPM.message = getFirstErrorFromModelState();
                        objCabinetItem.objPM.status = ResponseStatus.FAILED.ToString();
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = getFirstErrorFromModelState();
                        response.results = objCabinetItem;
                    }
                    //fill dropdown
                    BLItemTemplate.Instance.BindItemDropdowns(objCabinetItem, EntityType.Cabinet.ToString());
                    BindCabinetDropdown(objCabinetItem);
                    response.results = objCabinetItem;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveCabinetTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region Vault
        #region Get Vault Template 

        public ApiResponse<VaultItemMaster> GetVaultTemplate(ReqInput data)
        {
            var response = new ApiResponse<VaultItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                VaultItemMaster objVaultItemMaster = BLItemTemplate.Instance.GetTemplateDetail<VaultItemMaster>(objItemTemplateIn.userId, EntityType.Vault);
                BLItemTemplate.Instance.BindItemDropdowns(objVaultItemMaster, EntityType.Vault.ToString());
                BindVaultDropdown(objVaultItemMaster);
                response.status = StatusCodes.OK.ToString();
                response.results = objVaultItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetVaultTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Vault Template!";
            }
            return response;
        }
        #endregion

        #region Bind vault Dropdown
        private void BindVaultDropdown(VaultItemMaster objVaultItemMaster)
        {
            var objDDL = new BLMisc().GetDropDownList(EntityType.Vault.ToString());
            objVaultItemMaster.listVaultType = objDDL.Where(x => x.dropdown_type == DropDownType.Vault_Type.ToString()).ToList();
        }
        #endregion

        #region Save Vault Template 

        public ApiResponse<VaultItemMaster> SaveVaultTemplate(ReqInput data)
        {
            var response = new ApiResponse<VaultItemMaster>();
            try
            {
                VaultItemMaster objVaultItem = ReqHelper.GetRequestData<VaultItemMaster>(data);
                this.Validate(objVaultItem);
                if (ModelState.IsValid)
                {
                    if (ModelState.IsValid)
                    {
                        var itemid = objVaultItem.id;
                        var resultItem = new BLVaultItemMaster().SaveVaultItemTemplate(objVaultItem, objVaultItem.userId);

                        if (itemid > 0)  // Update 
                        {
                            objVaultItem.objPM.status = ResponseStatus.OK.ToString();
                            objVaultItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_074;
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_074;
                        }
                        else
                        {
                            objVaultItem.objPM.status = ResponseStatus.OK.ToString();
                            objVaultItem.objPM.message = Resources.Resources.SI_OSP_GBL_GBL_FRM_075;
                            response.status = ResponseStatus.OK.ToString();
                            response.error_message = Resources.Resources.SI_OSP_GBL_GBL_FRM_075;
                            objVaultItem = resultItem;
                        }
                    }
                    else
                    {
                        objVaultItem.objPM.message = getFirstErrorFromModelState();
                        objVaultItem.objPM.status = ResponseStatus.FAILED.ToString();
                        response.status = ResponseStatus.FAILED.ToString();
                        response.error_message = getFirstErrorFromModelState();
                        response.results = objVaultItem;
                    }
                    //fill dropdown
                    BLItemTemplate.Instance.BindItemDropdowns(objVaultItem, EntityType.Vault.ToString());
                    BindVaultDropdown(objVaultItem);
                    response.results = objVaultItem;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveVaultTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion

        #region Handhole

        #region Get Handhole Template
        /// <summary> GetHandholeTemplate </summary>
        /// <returns>Handhole Template Details</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>17-Aug-2021</CreatedDate>

        public ApiResponse<HandholeItemMaster> GetHandholeTemplate(ReqInput data)
        {
            var response = new ApiResponse<HandholeItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                HandholeItemMaster objHandholeItemMaster = BLItemTemplate.Instance.GetTemplateDetail<HandholeItemMaster>(objItemTemplateIn.userId, EntityType.Handhole);
                BLItemTemplate.Instance.BindItemDropdowns(objHandholeItemMaster, EntityType.Handhole.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objHandholeItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetHandholeTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching Handhole Template!";
            }
            return response;
        }
        #endregion

        #region Save Handhole Template
        /// <summary>SaveHandholeTemplate </summary>
        /// <returns>Handhole Template Status</returns>
        /// <CreatedBy>Antra Mathur</CreatedBy>
        /// <CreatedDate>17-Aug-2021</CreatedDate>
        public ApiResponse<HandholeItemMaster> SaveHandholeTemplate(ReqInput data)
        {
            var response = new ApiResponse<HandholeItemMaster>();
            try
            {
                HandholeItemMaster objHandholeItem = ReqHelper.GetRequestData<HandholeItemMaster>(data);
                this.Validate(objHandholeItem);
                if (ModelState.IsValid)
                {
                    var itemid = objHandholeItem.id;
                    var resultItem = new BLHandholeItemMaster().SaveHandholeItemTemplate(objHandholeItem, objHandholeItem.userId);
                    if (itemid > 0)  // Update 
                    {
                        objHandholeItem.id = resultItem.id;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = "Handhole template updated successfully.";
                        objHandholeItem.objPM.status = ResponseStatus.OK.ToString();
                        objHandholeItem.objPM.message = "Handhole template updated successfully.";
                        response.results = objHandholeItem;
                    }
                    else  //save
                    {
                        objHandholeItem.id = resultItem.id;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = "Handhole template saved successfully.";
                        objHandholeItem.objPM.status = ResponseStatus.OK.ToString();
                        objHandholeItem.objPM.message = "Handhole template saved successfully.";
                        response.results = objHandholeItem;
                    }
                }
                else
                {
                    objHandholeItem.objPM.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    objHandholeItem.objPM.message = getFirstErrorFromModelState();
                    response.status = ResponseStatus.VALIDATION_FAILED.ToString();
                    response.error_message = getFirstErrorFromModelState();
                    response.results = objHandholeItem;
                }
                BLItemTemplate.Instance.BindItemDropdowns(objHandholeItem, EntityType.Handhole.ToString());
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveHandholeTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }

            return response;
        }
        #endregion

        #endregion

        #region PatchPanel
        #region GetPatchPanelTemplate
       
        /// <CreatedBy>shazia barkat</CreatedBy>
        /// <CreatedDate>20-10-2021</CreatedDate>
        public ApiResponse<PatchPanelItemMaster> GetPatchPanelTemplate(ReqInput data)
        {
            var response = new ApiResponse<PatchPanelItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                PatchPanelItemMaster objPatchPanelItemMaster = BLItemTemplate.Instance.GetTemplateDetail<PatchPanelItemMaster>(objItemTemplateIn.userId, EntityType.PatchPanel);
                BLItemTemplate.Instance.BindItemDropdowns(objPatchPanelItemMaster, EntityType.PatchPanel.ToString());
                var objDDL = new BLMisc().GetDropDownList(EntityType.PatchPanel.ToString());
                objPatchPanelItemMaster.listPatchPanelType = objDDL.Where(x => x.dropdown_type == DropDownType.PatchPanel_type.ToString()).ToList();
                new BLMisc().BindPortDetails(objPatchPanelItemMaster, EntityType.PatchPanel.ToString(), DropDownType.PatchPanel_Port_Ratio.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objPatchPanelItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetPatchPanelTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching PatchPanel Template!";
            }
            return response;
        }
        #endregion

        #region Save PatchPanel Template 

        /// <CreatedBy>shazia barkat</CreatedBy>
        /// <CreatedDate>20-10-2021</CreatedDate>
        public ApiResponse<PatchPanelItemMaster> SavePatchPanelTemplate(ReqInput data)
        {
            var response = new ApiResponse<PatchPanelItemMaster>();
            try
            {
                PatchPanelItemMaster objPatchPanelItem = ReqHelper.GetRequestData<PatchPanelItemMaster>(data);
                this.Validate(objPatchPanelItem);
                if (ModelState.IsValid)
                {
                    if (objPatchPanelItem.unitValue != null && objPatchPanelItem.unitValue.Contains(":"))
                    {
                        objPatchPanelItem.no_of_input_port = Convert.ToInt32(objPatchPanelItem.unitValue.Split(':')[0]);
                        objPatchPanelItem.no_of_output_port = Convert.ToInt32(objPatchPanelItem.unitValue.Split(':')[1]);
                    }
                    var itemid = objPatchPanelItem.id;
                    var resultItem = new BLPatchPanelItemMaster().SavePatchPanelItemTemplate(objPatchPanelItem, objPatchPanelItem.user_id);
                    var layer_title = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.PatchPanel.ToString().ToUpper()).FirstOrDefault().layer_title;
                    if (itemid > 0)  // Update 
                    {
                        objPatchPanelItem.objPM.status = ResponseStatus.OK.ToString();
                        objPatchPanelItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title); //"ODF template update successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_100, layer_title);//"ODF template update successfully.";
                    }
                    else
                    {
                        objPatchPanelItem.objPM.status = ResponseStatus.OK.ToString();
                        objPatchPanelItem.objPM.message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);// "ODF template saved successfully.";
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = string.Format(Resources.Resources.SI_GBL_GBL_GBL_GBL_101, layer_title);//"ODF template saved successfully.";
                        objPatchPanelItem = resultItem;
                    }
                }
                else
                {
                    objPatchPanelItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objPatchPanelItem.objPM.message = "Please fill mandatory field !";
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = "Please fill mandatory field !";
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objPatchPanelItem, EntityType.PatchPanel.ToString());
                new BLMisc().BindPortDetails(objPatchPanelItem, EntityType.PatchPanel.ToString(), DropDownType.PatchPanel_Port_Ratio.ToString());
                var objDDL = new BLMisc().GetDropDownList(EntityType.PatchPanel.ToString());
                objPatchPanelItem.listPatchPanelType = objDDL.Where(x => x.dropdown_type == DropDownType.PatchPanel_type.ToString()).ToList();
                
                response.results = objPatchPanelItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SavePatchPanelTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        #endregion
        #endregion
        public ApiResponse<GipipeItemMaster> GetGipipeTemplate(ReqInput data)
        {
            var response = new ApiResponse<GipipeItemMaster>();
            try
            {
                ItemTemplateIn objItemTemplateIn = ReqHelper.GetRequestData<ItemTemplateIn>(data);
                GipipeItemMaster objGipipeItemMaster = BLItemTemplate.Instance.GetTemplateDetail<GipipeItemMaster>(objItemTemplateIn.userId, EntityType.Gipipe);
                BLItemTemplate.Instance.BindItemDropdowns(objGipipeItemMaster, EntityType.Gipipe.ToString());
                response.status = StatusCodes.OK.ToString();
                response.results = objGipipeItemMaster;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetGipipeTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While fetching HTB Template!";
            }
            return response;
        }
        public ApiResponse<GipipeItemMaster> SaveGipipeTemplate(ReqInput data)
        {
            var response = new ApiResponse<GipipeItemMaster>();
            try
            {
                GipipeItemMaster objGipipeItem = ReqHelper.GetRequestData<GipipeItemMaster>(data);
                if (ModelState.IsValid)
                {
                    var itemid = objGipipeItem.id;
                    var resultItem = new BLGipipeItemMaster().SaveGipipeItemTemplate(objGipipeItem, objGipipeItem.userId);

                    if (itemid > 0)  // Update 
                    {
                        objGipipeItem.objPM.status = ResponseStatus.OK.ToString();
                        objGipipeItem.objPM.message = Resources.Resources.SI_OSP_GIP_NET_FRM_0017;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GIP_NET_FRM_0017;
                    }
                    else
                    {
                        objGipipeItem.objPM.status = ResponseStatus.OK.ToString();
                        objGipipeItem.objPM.message = Resources.Resources.SI_OSP_GIP_NET_FRM_0018;
                        response.status = ResponseStatus.OK.ToString();
                        response.error_message = Resources.Resources.SI_OSP_GIP_NET_FRM_0018;
                        objGipipeItem = resultItem;
                    }
                }
                else
                {
                    objGipipeItem.objPM.status = ResponseStatus.FAILED.ToString();
                    objGipipeItem.objPM.message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.status = ResponseStatus.FAILED.ToString();
                    response.error_message = Resources.Resources.SI_GBL_GBL_NET_FRM_001;
                    response.results = objGipipeItem;
                }
                //fill dropdown
                BLItemTemplate.Instance.BindItemDropdowns(objGipipeItem, EntityType.Duct.ToString());
                response.results = objGipipeItem;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveDuctTemplate()", "Item Template Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }



    }
}