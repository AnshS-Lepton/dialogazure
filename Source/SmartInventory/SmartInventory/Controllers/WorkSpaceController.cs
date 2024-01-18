using BusinessLogics;
using Models;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class WorkSpaceController : Controller
    {

        public PartialViewResult GetWorkSpace()
        {
            var usrId = Convert.ToInt32(Session["user_id"]);
            var workspacelst = new BLWorkSpace().GetworkSpaceDetails(usrId);
            return PartialView("_WorkSpace", workspacelst);
        }

        public JsonResult GetWorkSpaceById(int workSpaceId)
        {
            JsonResponse<WorkSpaceMaster> objResp = new JsonResponse<WorkSpaceMaster>();

            var usrId = Convert.ToInt32(Session["user_id"]);

            BLWorkSpace objBLWorkSpace = new BLWorkSpace();
            var objWS = objBLWorkSpace.GetworkSpaceById(workSpaceId);

            objWS.WSRegionProvince = new BLWorkSpaceRegionProvince().GetWorkSpaceRegionProvince(usrId, workSpaceId);
            objWS.WSLayers = new BLWorkSpaceLayer().GetWorkSpaceLayers(workSpaceId);
            objWS.WSLandbaseLayers = new BLLandbaseWorkSpaceLayer().GetLandbaseWorkSpaceLayers(workSpaceId);

            objResp.status = ResponseStatus.OK.ToString();
            objResp.result = objWS;
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveWorkSpace(WorkSpaceMaster objWS)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();

            try
            {
                objWS.user_id = Convert.ToInt32(Session["user_id"]);
                new BLWorkSpace().SaveWorkSpace(objWS);
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = Resources.Resources.SI_OSP_WSP_NET_FRM_007;
                objResp.result = objWS.workspace_name;
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_WSP_NET_FRM_008;
                // ERROR LOGGING CODE WILL BE THERE...
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteWorkSpace(int workSpaceId)
        {
            JsonResponse<string> objResp = new JsonResponse<string>();
            try
            {
                var output = new BLWorkSpace().DeleteWorkSpace(workSpaceId);
                if (output > 0)
                {
                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.message = Resources.Resources.SI_OSP_WSP_JQ_FRM_003;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.message = Resources.Resources.SI_OSP_WSP_NET_FRM_010;
                }
            }
            catch
            {
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = Resources.Resources.SI_OSP_WSP_NET_FRM_011;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

    }
}