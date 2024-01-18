using BusinessLogics;
using Lepton.Utility;
using Models;
using SmartInventory.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace SmartInventory.Controllers
{
    public class TowerController : Controller
    {
        // GET: Tower
        // GET: Antenna
        private readonly Common objCommon;
        private readonly BLMisc objBLMisc;       
        private readonly BLCommon objBLCommon;
        public TowerController()
        {
            objCommon = new Common();
            objBLMisc = new BLMisc();       
            objBLCommon = new BLCommon();
        }

        public PartialViewResult AddTower(string networkIdType, int systemId = 0, string geom = "", int pSystemId = 0, string pEntityType="" , string pNetworkId = "")
        {
            VaultMaster obj = new VaultMaster();
            obj.networkIdType = networkIdType;
            obj.system_id = systemId;
            obj.geom = geom;
            obj.pSystemId = pSystemId;
            obj.pEntityType = pEntityType;
            obj.pNetworkId = pNetworkId;
            obj.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations ";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TowerMaster>(url, obj, EntityType.Tower.ToString(), EntityAction.Get.ToString());
            TowerMaster _obj = new TowerMaster();
            BLLayer objBLLayer = new BLLayer();
            _obj.lstUserModule = objBLLayer.GetUserModuleAbbrList(obj.user_id, UserType.Web.ToString());
            return PartialView("_AddTower", response.results);
        }
        public TowerMaster GetDetail(int pSystemId, string pEntityType, string networkIdType, int systemId = 0, string geom = "")
        {

            TowerMaster objEntity = new TowerMaster();
            objEntity.geom = geom;
            objEntity.networkIdType = networkIdType;
            if (systemId == 0)
            {
                objEntity.longitude = Convert.ToDouble(geom.Split(' ')[0]);
                objEntity.latitude = Convert.ToDouble(geom.Split(' ')[1]);
                objEntity.ownership_type = "Own";
                //NEW ENTITY->Fill Region and Province Detail..
                objBLCommon.fillRegionProvinceDetail(objEntity, GeometryType.Point.ToString(), geom);
                //Fill Parent detail...              
                objBLCommon.fillParentDetail(objEntity, new NetworkCodeIn() { eType = EntityType.Tower.ToString(), gType = GeometryType.Point.ToString(), eGeom = objEntity.geom, parent_eType = pEntityType, parent_sysId = pSystemId }, networkIdType);
                //Item template binding
                var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_name.ToUpper() == EntityType.Antenna.ToString().ToUpper()).FirstOrDefault();
                if (layerDetails.is_template_required)
                {
                    var objItem = BLItemTemplate.Instance.GetTemplateDetail<TowerTemplateMaster>(Convert.ToInt32(Session["user_id"]), EntityType.Tower);
                    Utility.MiscHelper.CopyMatchingProperties(objItem, objEntity);
                }
            }
            else
            {
                objEntity = objBLMisc.GetEntityDetailById<TowerMaster>(systemId, EntityType.Tower);
                objEntity.lstTowerAssociatedPop = new BLTowerAssociatedPop().GetAssociatedPop(systemId);
            }
            return objEntity;
        }
       
        public ActionResult SaveTower(TowerMaster objEntityMaster, bool isDirectSave = false)
        {

            objEntityMaster.isDirectSave = isDirectSave;
            objEntityMaster.user_id = Convert.ToInt32(Session["user_id"]);
            string url = "api/Library/EntityOperations";
            var response = WebAPIRequest.PostIntegrationAPIRequest<TowerMaster>(url, objEntityMaster, EntityType.Tower.ToString(), EntityAction.Save.ToString());
            if (isDirectSave)
            {
                return Json(response.results.objPM, JsonRequestBehavior.AllowGet);
            }
            TowerMaster _obj = new TowerMaster();
            BLLayer objBLLayer = new BLLayer();
            _obj.lstUserModule = objBLLayer.GetUserModuleAbbrList(objEntityMaster.user_id, UserType.Web.ToString());
            return PartialView("_AddTower", response.results);
        }

        public void BindAntennaDropDown(TowerMaster objEntityMaster)
        {
            var objDDL = objBLMisc.GetDropDownList(EntityType.Building.ToString());
            objEntityMaster.lstTenancy = objDDL.Where(x => x.dropdown_type == DropDownType.Tenancy.ToString()).ToList();

            objEntityMaster.list3rdPartyVendorId = BLCable.Instance.GetAllVendorType(VendorType.ThirdParty.ToString()).ToList();
        }
        public JsonResult GetPopInBuffer(int towerId, int distance)
        {
            var resultItem = new BLTower().GetPopInBuffer(towerId, distance);
            return Json(resultItem, JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveAssociation(int podId, int towerID)
        {
            var error = new BLTowerAssociatedPop().CheckDuplicate(podId, towerID);
            if (error.is_valid)
            {
                var resultItem = new BLTowerAssociatedPop().SaveAssociatedPop(podId, towerID, Convert.ToInt32(Session["user_id"]));
                if (resultItem.system_id > 0)
                {
                    error.status = StatusCodes.OK.ToString();
                }
                else
                {
                    error.status = StatusCodes.EXCEPTION.ToString();
                }
            }
            else
            {
                error.status = StatusCodes.DUPLICATE_EXIST.ToString();
            }
            return Json(error, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssociation(int towerId)
        {
            List<TowerAssociatedPopView> lstTowerAssociatedPop= new BLTowerAssociatedPop().GetAssociatedPop(towerId);
            return PartialView("_PopAssociatedList", lstTowerAssociatedPop);

        }

        public JsonResult DeAssociatePop(int podId , int towerID)
        {
            var resultItem = new BLTowerAssociatedPop().DeAssociatePop(podId, towerID);
            if (resultItem.is_valid) {
                resultItem.status = StatusCodes.OK.ToString();
            }
            else
            {
                resultItem.status = StatusCodes.FAILED.ToString();
            }
            return Json(resultItem, JsonRequestBehavior.AllowGet);
        }
    }
}