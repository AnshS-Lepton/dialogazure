using BusinessLogics;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;

namespace SmartInventoryServices.Controllers
{
    // [Authorize]
    [APIExceptionFilter]
    public class LogicalDiagramController : Controller
    {
        // GET: LogicalDiagram
        public ActionResult GetLogicalDiagram(string JsonData)
        {
            BLUser objBLUser = new BLUser();
            try
            {
                string JonString = MiscHelper.DecodeTo64(JsonData).ToString();
                string jsonFormattedString = JonString.Replace("\\\"", "\"");
                string ADFSEndPoint = string.Empty;
                // var tmpObj = JObject.Parse(jsonFormattedString);
                GetLogicalDiagramIn request = new JavaScriptSerializer().Deserialize<GetLogicalDiagramIn>(jsonFormattedString);

                var Password = MiscHelper.EncodeTo64(request.password);
                //var user = objDAUser.ChkUserExist(request.userName);
                ADFSEndPoint = System.Configuration.ConfigurationManager.AppSettings["ADFSEndPoint"].ToString().Trim();

                var user = objBLUser.ValidateUser(request.userName, (String.IsNullOrEmpty(ADFSEndPoint) ? Password : ""), UserType.Mobile.ToString());
                if (user != null)
                {
                    if (user.is_active == true)
                    {
                        LogicalViewVM logicaldetails = new LogicalViewVM();
                        logicaldetails.lstport = new BLOSPSplicing().getEntityLogicalView(request.entityId, request.entityType);
                        logicaldetails.listPortStatus = new BLPortStatus().getPortStatus();
                        logicaldetails.system_id = request.entityId;
                        logicaldetails.entity_type = request.entityType;
                        GeomDetailIn objGeomDetailIn = new GeomDetailIn();
                        objGeomDetailIn.systemId = request.entityId.ToString();
                        objGeomDetailIn.entityType = request.entityType;
                        objGeomDetailIn.geomType = "Point";
                        var objGeometryDetail = new BLSearch().GetGeometryDetails(objGeomDetailIn);
                        logicaldetails.networkId = objGeometryDetail.display_name;
                        return View("_EntityLogicalViewDiagram", logicaldetails);
                    }
                    else
                    {
                        return View("_Unauthorize");
                    }
                }
                else
                {
                    return View("_Unauthorize");
                }

            }
            catch (Exception ex)
            {
                return View("_Error", new HandleErrorInfo(ex, "GetLogicalDiagram", "LogicalDiagram"));
                throw;
            }
        }
    }
}