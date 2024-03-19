using BusinessLogics;
using SmartInventory.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Data;
using Utility;
using System.IO;
using NPOI.SS.UserModel;
using SmartInventory.Helper;
using SmartInventory.Settings;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Models.Admin;
using System.Threading;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.XSSF.UserModel;
using System.Dynamic;
using static Mono.Security.X509.X520;
using NPOI.SS.Formula.Functions;
using System.Reflection.Emit;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace SmartInventory.Controllers
{
    [Authorize]
    [SessionExpire]
    [HandleException]
    public class SplicingController : Controller
    {
        List<UserModule> lstUserModule = new BLMisc().GetUserModuleMasterList();
        //
        // GET: /Splicing/       
        public ActionResult Index(double latitude, double longitude, double bufferRadius)
        {
            User objUser = (User)(Session["userDetail"]);
            var splicingEntity = new BLOSPSplicing().getEntityForSplicing(latitude, longitude, bufferRadius, objUser.role_id);
            return PartialView("_Splicing", splicingEntity);
        }
        public ActionResult CableToCable(connectionInput objSplicingIn)
        {
            var lstUserModule = (List<string>)Session["ApplicableModuleList"];
            SplicingViewModel splicingEntity = new SplicingViewModel();
            splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(objSplicingIn, JsonConvert.SerializeObject(objSplicingIn.listConnector));
            var connector = objSplicingIn.listConnector.FirstOrDefault();
            if (splicingEntity.splicingConnections.Count > 0)
            {
                splicingEntity.sourceCable = splicingEntity.splicingConnections.Where(m => m.system_id == objSplicingIn.source_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == objSplicingIn.is_source_start_point).FirstOrDefault();
                splicingEntity.destinationCable = splicingEntity.splicingConnections.Where(m => m.system_id == objSplicingIn.destination_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == objSplicingIn.is_destination_start_point).FirstOrDefault();
                splicingEntity.connector = splicingEntity.splicingConnections.Where(m => m.system_id == Convert.ToInt32(connector.system_id) && m.entity_type == connector.entity_type).FirstOrDefault();
            }
            var isFSALocked = new BLOSPSplicing().CheckSplicingPermission(connector.system_id, connector.entity_type);

            var availablePorts = new BLOSPSplicing().getAvailablePorts(Convert.ToInt32(connector.system_id), connector.entity_type);
            splicingEntity.total_ports = availablePorts.total_ports;
            splicingEntity.available_ports = availablePorts.available_ports;
            splicingEntity.userId = Convert.ToInt32(Session["user_id"]);
            splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
            splicingEntity.isEditAllowed = lstUserModule.Contains("EDS");
            if (splicingEntity.isEditAllowed == false)
            {
                splicingEntity.unauthorisedmessage = "You are not authorised!";
            }
            else
            {
                if (isFSALocked == false)
                {
                    splicingEntity.isEditAllowed = false;
                    splicingEntity.unauthorisedmessage = "Changes in splicing has been restricted due to Design BOM is submitted!";
                }
            }

            splicingEntity.lstUserModule = lstUserModule;
            splicingEntity.lstSpliceTray = BLSpliceTray.Instance.GetSpliceTrayInfo(connector.system_id, connector.entity_type);
            splicingEntity.connector_entity_type = objSplicingIn.connector_entity_type;
            splicingEntity.is_middleware_entity = objSplicingIn.is_middleware_entity;
            splicingEntity.is_virtual = connector.is_virtual;
            splicingEntity.is_virtual_entity = connector.is_virtual_entity;
            return PartialView("_CableToCable", splicingEntity);
        }
        public ActionResult CPEToCustomer(connectionInput objSplicingIn)
        {
            var lstUserModule = (List<string>)Session["ApplicableModuleList"];
            SplicingViewModel splicingEntity = new SplicingViewModel();
            List<SplicingConectionInfo> listConnections = new List<SplicingConectionInfo>();
            //var connectorIds = objSplicingIn.connector_system_id.Split(',');
            //var connectorTypes = objSplicingIn.connector_entity_type.Split(',');
            //if (connectorIds.Count() == 2)
            //{
            //    //for (int i = 0; i < connectorIds.Length; i++)
            //    //{
            //    objSplicingIn.connector_system_id = connectorIds[0];
            //    objSplicingIn.connector_entity_type = connectorTypes[0];
            //    var connectionsList = new BLOSPSplicing().getSplicingInfo(objSplicingIn);
            //    var filterRecord = connectionsList.Where(m => m.destination_system_id == Convert.ToInt32(connectorIds[1]) && m.entity_type == connectorTypes[1]).ToList();
            //   // if (filterRecord.Count() > 0)
            //    //{
            //        connectionsList.ForEach(m => m.is_connected_to_other = false);
            //    //}
            //    listConnections.AddRange(connectionsList);

            //    objSplicingIn.connector_system_id = connectorIds[1];
            //    objSplicingIn.connector_entity_type = connectorTypes[1];
            //    var connectionsList2 = new BLOSPSplicing().getSplicingInfo(objSplicingIn);
            //    filterRecord = connectionsList2.Where(m => m.destination_system_id == Convert.ToInt32(connectorIds[0]) && m.entity_type == connectorTypes[0]).ToList();
            //    //if (connectionsList2.Count > 0)
            //    //{
            //        connectionsList2.ForEach(m => m.is_connected_to_other = false);
            //    //}
            //    listConnections.AddRange(connectionsList2);
            //    //} 
            //}

            splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(objSplicingIn, JsonConvert.SerializeObject(objSplicingIn.listConnector));
            splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
            splicingEntity.isEditAllowed = lstUserModule.Contains("EDS");
            splicingEntity.connector_entity_type = objSplicingIn.connector_entity_type;
            splicingEntity.is_middleware_entity = objSplicingIn.is_middleware_entity;
            return PartialView("_CPEToCustomer", splicingEntity);
        }
        public ActionResult ODFToCable(connectionInput objSplicingIn)
        {
            var lstUserModule = (List<string>)Session["ApplicableModuleList"];
            EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), objSplicingIn.listConnector.First().entity_type);
            SplicingViewModel splicingEntity = new SplicingViewModel();
            splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(objSplicingIn, JsonConvert.SerializeObject(objSplicingIn.listConnector));
            splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
            if (splicingEntity.splicingConnections.Count > 0)
            {
                splicingEntity.destinationCable = splicingEntity.splicingConnections.Where(m => m.system_id == objSplicingIn.destination_system_id && m.entity_type == EntityType.Cable.ToString()).FirstOrDefault();
            }
            splicingEntity.middlePortList = new BLOSPSplicing().middleWarePorts(objSplicingIn.listConnector[0].system_id, enType.ToString());
            var isFSALocked = new BLOSPSplicing().CheckSplicingPermission((objSplicingIn.listConnector.FirstOrDefault()).system_id, (objSplicingIn.listConnector.FirstOrDefault()).entity_type);

            splicingEntity.isEditAllowed = lstUserModule.Contains("EDS");
            if (splicingEntity.isEditAllowed == false)
            {
                splicingEntity.unauthorisedmessage = "You are not authorised!";
            }
            else
            {
                if (isFSALocked == false)
                {
                    splicingEntity.isEditAllowed = false;
                    splicingEntity.unauthorisedmessage = "Changes in splicing has been restricted due to Design BOM is submitted!";
                }
            }
            splicingEntity.lstUserModule = lstUserModule;
            splicingEntity.connector_entity_type = objSplicingIn.connector_entity_type;
            splicingEntity.is_middleware_entity = objSplicingIn.is_middleware_entity;
            //splicingEntity.entity_name = enType.ToString();
            return PartialView("_ODFToCable", splicingEntity);
        }

        public JsonResult SaveConnectionInfo(List<ConnectionInfoMaster> objConnectionInfo)
        {
            var userdetatils = (User)Session["userDetail"];
            objConnectionInfo.ForEach(p => p.created_by = Convert.ToInt32(Session["user_id"]));
            objConnectionInfo.ForEach(p => p.created_on = DateTimeHelper.Now);
            var objConnection = new BLOSPSplicing().SaveConnectionInfo(JsonConvert.SerializeObject(objConnectionInfo));
            var objConection = objConnectionInfo.FirstOrDefault();
            var module = lstUserModule.Where(x => x.module_abbr.ToUpper() == "NTF").FirstOrDefault();
            if (module != null)
            {

                if (objConection != null)
                {

                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        DbMessage objDbMessage = new BLOSPSplicing().SaveUtilizationNotification(objConection);
                        SmartInventoryHub smartInventoryhub = SmartInventoryHub.Instance;
                        var UnreadNotificationCount = new BLMisc().GetUnreadNotificationCount(userdetatils.user_id, userdetatils.role_id);
                        NotificationOutPut objNotification = new NotificationOutPut();
                        objNotification.info = Convert.ToString(UnreadNotificationCount);
                        objNotification.sendToAllUser = false;
                        objNotification.notificationType = notificationType.Utilization.ToString();
                        smartInventoryhub.BroadCastInfo(objNotification);
                    }).ContinueWith(tsk =>
                    {
                        tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("Splicing", "SaveConnectionInfo", ex); return true; });
                    }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                    SendUtilizationEmail(objConnectionInfo);
                }
            }
            return Json(objConnection, JsonRequestBehavior.AllowGet);
        }

        public void SendUtilizationEmail(List<ConnectionInfoMaster> objConnectionInfo)
        {
            try
            {
                var objConection = objConnectionInfo.FirstOrDefault();
                List<NotificationUtlization> objNotificationUtlization = new BLOSPSplicing().GetNotificationUtlization(objConection);

                StringBuilder objUtilizationText = new StringBuilder();
                bool isOverHead = false;

                DataTable dtUtlizationTable = new DataTable();
                dtUtlizationTable.Columns.Add("EntityName");
                dtUtlizationTable.Columns.Add("NetworkId");
                dtUtlizationTable.Columns.Add("TotalPorts");
                dtUtlizationTable.Columns.Add("UsedPorts");
                dtUtlizationTable.Columns.Add("AvailablePorts");
                dtUtlizationTable.Columns.Add("UtilizationPercetage");
                dtUtlizationTable.Columns.Add("UtilizationText");

                if (objNotificationUtlization != null)
                {
                    foreach (NotificationUtlization obj in objNotificationUtlization)
                    {
                        if (obj.utilization_abbr.ToUpper() == "O")
                        {
                            isOverHead = true;
                            int available_ports = obj.total_ports - obj.used_ports;
                            int percentComplete = (int)Math.Round((double)(100 * obj.used_ports) / obj.total_ports);

                            DataRow dtRow = dtUtlizationTable.NewRow();
                            dtRow["EntityName"] = obj.entityname;
                            dtRow["NetworkId"] = obj.network_id;
                            dtRow["TotalPorts"] = obj.total_ports;
                            dtRow["UsedPorts"] = obj.used_ports;
                            dtRow["AvailablePorts"] = available_ports;
                            dtRow["UtilizationPercetage"] = percentComplete;
                            dtRow["UtilizationText"] = obj.utilized_text;
                            dtUtlizationTable.Rows.Add(dtRow);

                        }
                    }

                }

                if (isOverHead == true)
                {
                    #region email sending start
                    List<string> filePath = new List<string>();
                    var filename = "UtilizationReport" + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "_" + DateTimeHelper.Now.ToString("HHmmss") + ".xlsx";
                    string filePaths = Server.MapPath("~/uploads/temp/");
                    filePath.Add(EmailAttachmentFilePath(dtUtlizationTable, filePaths + filename));
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                    objDict.Add("EntityList", objUtilizationText.ToString());
                    List<HttpPostedFileBase> objHttpPostedFileBase = null;
                    BLUser objBLuser = new BLUser();
                    List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.PercentUtilization70.ToString());
                    System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, objHttpPostedFileBase, EmailSettings.AllEmailSettings, filePath,"", EmailEventList.PercentUtilization70.ToString()));
                    //commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, objHttpPostedFileBase, filePath);
                    #endregion
                }

            }
            catch (Exception ex)
            {

            }
        }

        public string EmailAttachmentFilePath(DataTable dt, string filepath)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {

                    dt.TableName = "UtilizationReport";
                    string file = Helper.NPOIExcelHelper.DatatableToExcelFile("xlsx", dt, filepath);
                    byte[] fileBytes = System.IO.File.ReadAllBytes(file);

                    // return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                }
            }
            catch (Exception ex)
            {

            }
            return filepath;
        }

        public JsonResult GetAvailabePorts(int systemId, string entityType)
        {
            var availablePorts = new BLOSPSplicing().getAvailablePorts(systemId, entityType);
            return Json(availablePorts, JsonRequestBehavior.AllowGet);
        }
        public JsonResult deleteConnection(List<deleteConeectionInput> objConnectionInfo)
        {

            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new BLOSPSplicing().deleteConnection(JsonConvert.SerializeObject(objConnectionInfo));
            var module = lstUserModule.Where(x => x.module_abbr.ToUpper() == "NTF").ToString();
            if (module == "NTF")
            {
                new Thread(() =>
                {
                    new BLOSPSplicing().utilizationReset(JsonConvert.SerializeObject(objConnectionInfo));
                }).Start();
            }
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);//response.message;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = BLConvertMLanguage.MultilingualMessageConvert(response.message);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult viewlossdetails(int wavelength_id = 0)
        {

            LinkBudgetMaster objLinkBudget = wavelength_id != 0 ? new BusinessLogics.Admin.BLLinkBudget().GetLinkBudgetDetailByID(wavelength_id) : new LinkBudgetMaster();
            objLinkBudget.lstSplitterLoss = new BusinessLogics.Admin.BLLinkBudget().GetSplitterLossByWaveLength(wavelength_id);
            return PartialView("_viewlossdetails", objLinkBudget);
        }

        public ActionResult ConnectionPathFinder(ViewOSPCPFModel objOSP, int page = 0, string sort = "", string sortdir = "")
        {
            var userdetails = (User)Session["userDetail"];
            objOSP.objFilterAttributes.pageSize = ApplicationSettings.ConnectionPathFinderGridPaging;
            objOSP.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objOSP.objFilterAttributes.sort = sort;
            objOSP.objFilterAttributes.orderBy = sortdir;
            List<ConnectionInfo> listConnections = new List<ConnectionInfo>();
            connectionInfoPath objConnectionPath = new connectionInfoPath();
            if (objOSP.objFilterAttributes.entityid > 0)
            {
                objConnectionPath = new BLOSPSplicing().GetConnectionInfoPath(objOSP.objFilterAttributes);
                var lstPort = new BLOSPSplicing().GetEquipmentPort(objOSP.objFilterAttributes.entityid, objOSP.objFilterAttributes.entity_type);
                BindEquipementCoreDropDown(objOSP);
                if (objConnectionPath.lstConnectionInfo != null)
                {
                    listConnections = objConnectionPath.lstConnectionInfo; //new BLOSPSplicing().GetConnectionInfo(objOSP.objFilterAttributes); //objConnectionPath.lstConnectionInfo;
                                                                           //listConnections = listConnections.Where(m => m.source_network_id != m.destination_network_id).ToList();                    
                    objOSP.lstConnectionInfo = listConnections.Where(x => (x.source_entity_type.ToUpper() + x.source_system_id.ToString()) != (x.destination_entity_type.ToUpper() + x.destination_system_id.ToString())).ToList();
                    objOSP.lstConnectionInfo = objOSP.lstConnectionInfo.Where(x => (x.source_network_id.ToUpper()) != (x.destination_network_id.ToUpper())).ToList();
                    objOSP.objFilterAttributes.totalRecord = objOSP.lstConnectionInfo.Count;
                    Session["viewConnectionPathFinderFilter"] = objOSP.objFilterAttributes;
                    objOSP.lstWave_length = new BusinessLogics.Admin.BLLinkBudget().GetWaveLength();
                }
            }


            List<ConnectionInfo> ChildList = new List<ConnectionInfo>();
            List<ConnectionInfo> downChildList = new List<ConnectionInfo>();
            List<string> lstProcessedElements = new List<string>();
            List<string> downlstProcessedElements = new List<string>();

            //if (listConnections.Count > 0)
            //{
            //    // get first child 
            //    //var firstChildInfo = listConnections.FirstOrDefault();
            //    //if (firstChildInfo != null)
            //    //{
            //    //    ChildList.Add(firstChildInfo);
            //    //    //List <EndToEndSchematic> schematicList = BindChild(listConnections, ChildList, objOSP.objFilterAttributes, lstProcessedElements).FirstOrDefault();
            //    //    objOSP.schematicJsonString = JsonConvert.SerializeObject(BindChild(listConnections, ChildList, objOSP.objFilterAttributes, lstProcessedElements).FirstOrDefault());
            //    //}

            //    var upStreamList = listConnections.Where(m => m.is_backward_path == true).ToList();
            //    var downStreamList = listConnections.Where(m => m.is_backward_path == false).ToList();
            //    var upStreamFirstChild = upStreamList.Where(x => x.source_system_id == objOSP.objFilterAttributes.entityid & x.source_port_no == objOSP.objFilterAttributes.port_no & x.source_entity_type.ToUpper() == objOSP.objFilterAttributes.entity_type.ToUpper()).ToList();
            //    var downStreamFirstChild = downStreamList.Where(x => x.source_system_id == objOSP.objFilterAttributes.entityid & x.source_port_no == objOSP.objFilterAttributes.port_no & x.source_entity_type.ToUpper() == objOSP.objFilterAttributes.entity_type.ToUpper()).ToList();
            //    if (upStreamFirstChild != null && upStreamFirstChild.Count > 0)
            //    {
            //        ChildList.AddRange(upStreamFirstChild);

            //        if (ChildList.Count > 1)
            //        {
            //            var elementInfo = ChildList.FirstOrDefault();
            //            if (elementInfo != null)
            //            {
            //                objOSP.upStreamPath = JsonConvert.SerializeObject(BindMultipleChild(upStreamList, ChildList, elementInfo, objOSP.objFilterAttributes, lstProcessedElements).FirstOrDefault());
            //            }

            //        }
            //        else
            //        {
            //            objOSP.upStreamPath = JsonConvert.SerializeObject(BindChild(upStreamList, ChildList, objOSP.objFilterAttributes, lstProcessedElements).FirstOrDefault());

            //        }


            //    }
            //    if (downStreamFirstChild != null && downStreamFirstChild.Count > 0)
            //    {
            //        downChildList.AddRange(downStreamFirstChild);
            //        //public List<EndToEndSchematic> BindMultipleChild(List<ConnectionInfo> lstConnectionInfo, List<ConnectionInfo> lstChildConnection, ConnectionInfo splitterInfo, ConnectionInfoFilter objConnectionFilters, List<string> lstProcessedElements)
            //        if (downChildList.Count > 1)
            //        {
            //            var elementInfo = downChildList.FirstOrDefault();
            //            if (elementInfo != null)
            //            {
            //                objOSP.downStreamPath = JsonConvert.SerializeObject(BindMultipleChild(downStreamList, downChildList, elementInfo, objOSP.objFilterAttributes, downlstProcessedElements).FirstOrDefault());
            //            }

            //        }
            //        else
            //        {
            //            objOSP.downStreamPath = JsonConvert.SerializeObject(BindChild(downStreamList, downChildList, objOSP.objFilterAttributes, downlstProcessedElements).FirstOrDefault());
            //        }
            //    }
            //}
            var lstUserModule = (List<string>)Session["ApplicableModuleList"];
            objOSP.isOLBEnabled = lstUserModule.Contains("OLB");
            objOSP.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return PartialView("_ConnectionPathFinder", objOSP);
        }

        public ActionResult SchematicView(string val)
        {
            string[] value = val.Split(',');
            var eId = MiscHelper.Decrypt(value[0]);
            var etype = MiscHelper.Decrypt(value[1]);
            var pNo = MiscHelper.Decrypt(value[2]);
            SchematicView objSchematicView = GetSchematicData(eId, etype, pNo, false, true);
            return View(objSchematicView);
        }

        public ActionResult GetStreamWiseSchematic(string val, bool isUpstream, bool isConnectedPort)
        {
            string[] value = val.Split(',');
            var eId = MiscHelper.Decrypt(value[0]);
            var etype = MiscHelper.Decrypt(value[1]);
            var pNo = MiscHelper.Decrypt(value[2]);
            SchematicView objSchematicView = GetSchematicData(eId, etype, pNo, isUpstream, isConnectedPort);
            //if (!string.IsNullOrEmpty(objSchematicView.edges))
            //    objSchematicView.edges = objSchematicView.edges.Replace("\\n <b>", "").Replace("\\", "");
            return Json(objSchematicView, JsonRequestBehavior.AllowGet);
        }

        private static SchematicView GetSchematicData(string eId, string etype, string pNo, bool isUpstream, bool isConnectedPort)
        {
            ViewOSPCPFModel objOSP = new ViewOSPCPFModel();
            objOSP.objFilterAttributes.pageSize = ApplicationSettings.ConnectionPathFinderGridPaging;
            objOSP.objFilterAttributes.currentPage = 0;
            objOSP.objFilterAttributes.sort = "";
            objOSP.objFilterAttributes.orderBy = "";
            objOSP.objFilterAttributes.entity_type = etype;
            objOSP.objFilterAttributes.entityid = Convert.ToInt32(eId);
            objOSP.objFilterAttributes.port_no = Convert.ToInt32(pNo);
            SchematicView objSchematicView = new SchematicView();
            if (objOSP.objFilterAttributes.entityid > 0)
            {
                objSchematicView = new BLOSPSplicing().GetSchematicView(objOSP.objFilterAttributes, isUpstream, isConnectedPort);
                if (!string.IsNullOrEmpty(objSchematicView.legends))
                {
                    objSchematicView.lstlegend = JsonConvert.DeserializeObject<List<legend>>(objSchematicView.legends);
                }
                if (!string.IsNullOrEmpty(objSchematicView.cables))
                {
                    objSchematicView.lstCableLegend = JsonConvert.DeserializeObject<List<CableLegend>>(objSchematicView.cables);
                }
                if (!string.IsNullOrEmpty(objSchematicView.downstreamLegends))
                {
                    objSchematicView.lstdownStremaLegend = JsonConvert.DeserializeObject<List<legend>>(objSchematicView.downstreamLegends);
                }
                if (!string.IsNullOrEmpty(objSchematicView.downstreamCables))
                {
                    objSchematicView.lstDownStreamCableLegend = JsonConvert.DeserializeObject<List<CableLegend>>(objSchematicView.downstreamCables);
                }
            }

            return objSchematicView;
        }

        public ActionResult GetEquipmentSearchResult(string SearchText)
        {
            BLOSPSplicing objSplicing = new BLOSPSplicing();
            List<EquipementSearchResult> lstEquipment = new List<EquipementSearchResult>();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                lstEquipment = objSplicing.GetSearchEquipmentResult(SearchText, Convert.ToInt32(Session["user_id"]));
            }

            return Json(lstEquipment, JsonRequestBehavior.AllowGet);
        }
        private void BindEquipementCoreDropDown(ViewOSPCPFModel obj)
        {
            var objddl = new BLOSPSplicing().GetEquipmentPort(obj.objFilterAttributes.entityid, obj.objFilterAttributes.entity_type);
            obj.lstEquipementCore = objddl.Select(i => new EquipementPort() { port_value = i.port_value, port_text = i.port_text.ToString() }).ToList();
        }

        public ActionResult GetEquipmentPortInfo(int entity_id, string entity_type)
        {
            JsonResponse<List<EquipementPort>> objResp = new JsonResponse<List<EquipementPort>>();
            try
            {
                objResp.result = new BLOSPSplicing().GetEquipmentPort(entity_id, entity_type);
                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("GetEquipmentPortInfo()", "Splicing", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error while fetching port info!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public void DownloadConnectionPathFinderReport()
        {
            if (Session["viewConnectionPathFinderFilter"] != null)
            {

                ConnectionInfoFilter objViewFilter = (ConnectionInfoFilter)Session["viewConnectionPathFinderFilter"];
                List<ConnectionInfo> lstconnectionInfo = new List<ConnectionInfo>();
                List<ConnectionInfoReport> lstconnectionInfoReport = new List<ConnectionInfoReport>();
                var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == EntityType.CDB.ToString().ToUpper()).FirstOrDefault() : null;
                objViewFilter.currentPage = 0;
                objViewFilter.pageSize = 0;
                lstconnectionInfo = new BLOSPSplicing().GetConnectionInfoPath(objViewFilter).lstConnectionInfo.Where(x => (x.source_entity_type.ToUpper() + x.source_system_id.ToString()) != (x.destination_entity_type.ToUpper() + x.destination_system_id.ToString())).ToList();
                //x.source_port_no.ToString().Substring(0, 1)

                lstconnectionInfo.ForEach(m =>
                {
                    ConnectionInfoReport conInfoReport = new ConnectionInfoReport();
                    conInfoReport.approved_by = m.approved_by;
                    conInfoReport.approved_on = m.approved_on;
                    conInfoReport.connection_id = m.connection_id;
                    conInfoReport.created_on = m.created_on;
                    conInfoReport.destination_entity_type = m.destination_entity_title;
                    conInfoReport.destination_tray_display_name = m.destination_tray_display_name;
                    conInfoReport.destination_port_display_name = !m.is_destination_virtual ? m.destination_port_display_name : null;
                    conInfoReport.destination_display_name = m.destination_display_name;
                    conInfoReport.destination_system_id = m.destination_system_id;
                    conInfoReport.id = m.id;
                    conInfoReport.is_backward_path = m.is_backward_path;
                    conInfoReport.is_customer_connected = m.is_customer_connected;
                    conInfoReport.source_entity_type = m.source_entity_title;
                    conInfoReport.source_tray_display_name = m.source_tray_display_name;
                    conInfoReport.source_port_display_name = !m.is_source_virtual ? m.source_port_display_name : null;
                    conInfoReport.source_display_name = m.source_display_name;
                    conInfoReport.source_system_id = m.source_system_id;
                    conInfoReport.totalRecords = m.totalRecords;
                    conInfoReport.created_by = m.created_by;
                    lstconnectionInfoReport.Add(conInfoReport);
                });

                DataTable dtReport = new DataTable();

                dtReport = MiscHelper.ListToDataTable<ConnectionInfoReport>(lstconnectionInfoReport);

                dtReport.Columns.Add("SPLICED_ON", typeof(System.String));
                dtReport.Columns.Add("IS_DOWN_STREAM", typeof(System.String));
                foreach (DataRow dr in dtReport.Rows)
                {
                    dr["SPLICED_ON"] = MiscHelper.FormatDateTime((dr["created_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                    dr["IS_DOWN_STREAM"] = Convert.ToBoolean(dr["IS_BACKWARD_PATH"]) == false ? "Yes" : "No";
                }

                dtReport.Columns.Remove("IS_BACKWARD_PATH");
                dtReport.Columns.Remove("created_on");
                dtReport.Columns.Remove("connection_id");
                dtReport.Columns.Remove("id");
                dtReport.Columns.Remove("source_system_id");
                dtReport.Columns.Remove("destination_system_id");
                dtReport.Columns.Remove("is_customer_connected");
                dtReport.Columns.Remove("created_by");
                dtReport.Columns.Remove("approved_by");
                dtReport.Columns.Remove("approved_on");
                dtReport.Columns.Remove("childordering");
                dtReport.Columns.Remove("totalrecords");
                dtReport.Columns.Remove("SOURCE_PORT_NO");
                dtReport.Columns.Remove("DESTINATION_PORT_NO");
                //dtReport.Columns.Remove("SOURCE_NETWORK_ID");
                //dtReport.Columns.Remove("DESTINATION_NETWORK_ID");
                dtReport.Columns["source_display_name"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_199;
                //dtReport.Columns["SOURCE_NETWORK_ID"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_199;
                dtReport.Columns["SOURCE_ENTITY_TYPE"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_120;
                dtReport.Columns["SOURCE_TRAY_DISPLAY_NAME"].ColumnName = "Source Tray Name";
                dtReport.Columns["SOURCE_PORT_DISPLAY_NAME"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_019;
                dtReport.Columns["destination_display_name"].ColumnName = Resources.Resources.SI_GBL_GBL_GBL_FRM_019; ;
                //dtReport.Columns["DESTINATION_NETWORK_ID"].ColumnName = Resources.Resources.SI_GBL_GBL_GBL_FRM_019;
                dtReport.Columns["DESTINATION_ENTITY_TYPE"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_121;
                dtReport.Columns["DESTINATION_TRAY_DISPLAY_NAME"].ColumnName = "Destination Tray Name";
                dtReport.Columns["DESTINATION_PORT_DISPLAY_NAME"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_022;
                dtReport.Columns["SPLICED_ON"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_023;
                dtReport.Columns["IS_DOWN_STREAM"].ColumnName = "Is DownStream";
                if (layerDetail != null && !layerDetail.is_trayinfo_enabled)
                {
                    dtReport.Columns.Remove("Source Tray Name");
                    dtReport.Columns.Remove("Destination Tray Name");
                }
                var filename = "ConnectionPathFinder";
                ExportCPFData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));


            }
        }


        [HttpGet]
        public void DownloadOpticalLinkBudgetReport()
        {

            if (Session["LinkBudgetFilters"] != null)
            {

                LinkBudgetFilter objLinkBudgetFilter = (LinkBudgetFilter)Session["LinkBudgetFilters"];
                var lstViewLinkBudgetDataDetail = new BLOSPSplicing().GetLinkBudgetDetails(objLinkBudgetFilter);
                DataTable dtReport = new DataTable();
                dtReport = MiscHelper.ListToDataTable<ViewLinkBudgetDataDetail>(lstViewLinkBudgetDataDetail);

                dtReport.Columns.Add("source_port_no_new", typeof(string));
                dtReport.Columns.Add("destination_port_no_new", typeof(string));


                //set column orders..

                dtReport.Columns["RECEIVING_POWER"].SetOrdinal(0);
                dtReport.Columns["source_display_name"].SetOrdinal(1);
                //dtReport.Columns["source_network_id"].SetOrdinal(1);
                dtReport.Columns["source_entity_type"].SetOrdinal(2);
                dtReport.Columns["source_port_no_new"].SetOrdinal(3);
                dtReport.Columns["destination_display_name"].SetOrdinal(4);
                //dtReport.Columns["destination_network_id"].SetOrdinal(4);
                dtReport.Columns["destination_entity_type"].SetOrdinal(5);
                dtReport.Columns["destination_port_no_new"].SetOrdinal(6);
                dtReport.Columns["loss_type"].SetOrdinal(7);
                dtReport.Columns["loss_value"].SetOrdinal(8);
                dtReport.Columns["transmit_power"].SetOrdinal(9);

                // set input output value and text...

                foreach (DataRow dr in dtReport.Rows)
                {
                    dr["source_port_no_new"] = dr["destination_port_no"].ToString().Substring(0, 1) == "-" ? dr["destination_port_no"].ToString().Replace("-", "") + " IN" : dr["destination_port_no"].ToString() + " OUT";
                    dr["destination_port_no_new"] = dr["source_port_no"].ToString().Substring(0, 1) == "-" ? dr["source_port_no"].ToString().Replace("-", "") + " IN" : dr["source_port_no"].ToString() + " OUT";
                }

                dtReport.Columns["RECEIVING_POWER"].ColumnName = String.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_190, ApplicationSettings.dBPowerUnit);
                //dtReport.Columns["source_network_id"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_018;
                dtReport.Columns["source_display_name"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_018;
                dtReport.Columns["source_entity_type"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_120;
                dtReport.Columns["source_port_no_new"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_019;
                //dtReport.Columns["destination_network_id"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_020;
                dtReport.Columns["destination_display_name"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_020;
                dtReport.Columns["destination_entity_type"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_RPT_121;
                dtReport.Columns["destination_port_no_new"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_022;
                dtReport.Columns["loss_type"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_366;
                //dtReport.Columns["loss_value"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_367;
                dtReport.Columns["loss_value"].ColumnName = String.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_367, ApplicationSettings.dBLossUnit);
                dtReport.Columns["transmit_power"].ColumnName = String.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_191, ApplicationSettings.dBPowerUnit);

                //remove extra columns..
                dtReport.Columns.Remove("ID");
                dtReport.Columns.Remove("CONNECTION_ID");
                dtReport.Columns.Remove("SOURCE_SYSTEM_ID");
                dtReport.Columns.Remove("DESTINATION_SYSTEM_ID");
                dtReport.Columns.Remove("IS_CUSTOMER_CONNECTED");
                dtReport.Columns.Remove("CREATED_ON");
                dtReport.Columns.Remove("CREATED_BY");
                dtReport.Columns.Remove("APPROVED_ON");
                dtReport.Columns.Remove("APPROVED_BY");
                dtReport.Columns.Remove("CABLE_CALCULATED_LENGTH");
                dtReport.Columns.Remove("CABLE_MEASURED_LENGTH");
                dtReport.Columns.Remove("SPLITTER_RATIO");
                dtReport.Columns.Remove("source_port_no");
                dtReport.Columns.Remove("destination_port_no");
                dtReport.Columns.Remove("IS_SOURCE_VIRTUAL");
                dtReport.Columns.Remove("IS_DESTINATION_VIRTUAL");



                var filename = "Optical_Link_Budget";
                ExportCPFData(dtReport, filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
            }
        }



        public ActionResult GetCPFelementPath()
        {
            JsonResponse<List<CPFElements>> objResp = new JsonResponse<List<CPFElements>>();
            ConnectionInfoFilter objViewFilter = (ConnectionInfoFilter)Session["viewConnectionPathFinderFilter"];
            try
            {
                objResp.result = new BLOSPSplicing().GetCPFElement(objViewFilter);


                objResp.status = ResponseStatus.OK.ToString();
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("GetCPFelementPath()", "Splicing", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error while CPF element fetching!";
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }


        private void ExportCPFData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        //[OutputCache(CacheProfile = "CacheForOneDay")]
        //public ActionResult SchematicView()
        //{
        //    return PartialView("_SchematicView", new ViewOSPCPFModel());
        //}

        public JsonResult GetEquipmentDetails(string network_id, string entity_type, int system_id, int port_no)
        {
            JsonResponse<EndToEndSchematic> objResp = new JsonResponse<EndToEndSchematic>();
            EndToEndSchematic obj = new EndToEndSchematic();
            ViewOSPCPFModel objOSP = new ViewOSPCPFModel();
            List<ConnectionInfo> ChildList = new List<ConnectionInfo>();
            List<string> lstProcessedElements = new List<string>();
            objOSP.objFilterAttributes.entity_type = entity_type;
            objOSP.objFilterAttributes.entityid = system_id;
            objOSP.objFilterAttributes.port_no = port_no;
            objOSP.objFilterAttributes.currentPage = 1;
            try
            {
                objOSP.lstConnectionInfo = new BLOSPSplicing().GetConnectionInfo(objOSP.objFilterAttributes);
                if (objOSP.lstConnectionInfo != null)
                {
                    // get first child 
                    var firstChildInfo = objOSP.lstConnectionInfo.FirstOrDefault();

                    if (firstChildInfo != null)
                    {
                        ChildList.Add(firstChildInfo);
                        obj = BindChild(objOSP.lstConnectionInfo, ChildList, objOSP.objFilterAttributes, lstProcessedElements).FirstOrDefault();
                    }

                    objResp.status = ResponseStatus.OK.ToString();
                    objResp.result = obj;
                }
                else
                {
                    objResp.status = ResponseStatus.ZERO_RESULTS.ToString();
                    objResp.message = "<p>" + Resources.Resources.SI_OSP_GBL_GBL_RPT_001 + "</p>";// "<p>No Record found.</p>";
                }
            }

            catch (Exception ex)
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = "<p>" + Resources.Resources.SI_GBL_GBL_NET_FRM_035 + "</p>";//Error while fetching Schematic detail!
                objResp.result = new EndToEndSchematic();
                ErrorLogHelper.WriteErrorLog("GetEquipmentDetails()", "Splicing", ex);
                //log exception..

                return Json(objResp, JsonRequestBehavior.AllowGet);
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }

        public bool CheckElementAlreadyExist(List<EndToEndSchematic> lstEndToEndSchematic, string network_id)
        {
            var isfound = false;
            foreach (var objElement in lstEndToEndSchematic)
            {
                if (!objElement.entity_name.ToUpper().Contains(network_id.ToUpper()))
                {
                    isfound = CheckElementAlreadyExist(objElement.children, network_id);
                }
                else
                {
                    isfound = true;
                    break;
                }
            }
            return isfound;
        }

        //Commented by deepk to replace the recursion
        public List<EndToEndSchematic> BindChild(List<ConnectionInfo> lstConnectionInfo, List<ConnectionInfo> lstChildConnections, ConnectionInfoFilter objConnectionFilters, List<string> lstProcessedElements)
        {

            List<EndToEndSchematic> lstEndToEndSchematic = new List<EndToEndSchematic>();
            foreach (var item in lstChildConnections)
            {
                if (lstProcessedElements.Where(m => m.Contains(item.source_system_id + item.source_network_id + item.source_port_no.ToString())).Count() == 0)
                {
                    lstProcessedElements.Add(item.source_system_id + item.source_network_id + item.source_port_no.ToString());
                    //if (!CheckElementAlreadyExist(lstEndToEndSchematic, item.source_network_id))
                    // {
                    EndToEndSchematic objEndToEndSchematic = new EndToEndSchematic();
                    List<ConnectionInfo> lstSubChilds = new List<ConnectionInfo>();
                    objEndToEndSchematic.id = item.connection_id.ToString();
                    objEndToEndSchematic.entity_name = item.is_source_virtual ? item.source_display_name : item.source_display_name + "(" + (item.source_entity_type.ToUpper() != "CABLE" && item.source_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(item.source_port_no) : item.source_port_no.ToString()) + ")";
                    //objEndToEndSchematic.entity_name = item.is_source_virtual ? item.source_network_id : item.source_network_id + "(" + (item.source_entity_type.ToUpper() != "CABLE" && item.source_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(item.source_port_no) : item.source_port_no.ToString()) + ")";
                    objEndToEndSchematic.entity_type = (objConnectionFilters.entityid == item.source_system_id && objConnectionFilters.entity_type == item.source_entity_type && objConnectionFilters.port_no == item.source_port_no) ? item.source_entity_type + "(Source)" : item.source_entity_type;

                    //FETCH CHILD INFORMATION..
                    lstSubChilds = lstConnectionInfo.Where(x => x.source_system_id == item.destination_system_id & x.source_port_no == item.destination_port_no & x.source_entity_type == item.destination_entity_type).ToList();
                    if (lstSubChilds != null && lstSubChilds.Count > 0)
                    {
                        if (lstSubChilds.Count > 1)
                        {
                            var elementInfo = lstConnectionInfo.Where(x => x.source_system_id == item.destination_system_id & x.source_port_no == item.destination_port_no & x.source_entity_type.ToUpper() == item.destination_entity_type.ToUpper()).FirstOrDefault();
                            if (elementInfo != null)
                            {
                                objEndToEndSchematic.children = BindMultipleChild(lstConnectionInfo, lstSubChilds, elementInfo, objConnectionFilters, lstProcessedElements);
                            }

                        }
                        else if (lstSubChilds.Count == 1)
                        {
                            if (item.source_system_id == lstSubChilds[0].source_system_id && item.source_entity_type.ToUpper() == lstSubChilds[0].source_entity_type.ToUpper())
                            {
                                // SAME CHILD WITH DIFFERENT PORT LIKE SC , SPL, ADB CONNECTED DIRECTLY WITH OUTPUT PORT
                                //NEED TO SHOW THE NODES IN COMBINED FORMAT LIKE GGN-JCL000135(1,3)
                                var objSameChild = lstSubChilds[0];
                                objEndToEndSchematic.entity_name = item.source_display_name + "(" + Math.Abs(item.source_port_no) + "," + Math.Abs(objSameChild.source_port_no) + ")";
                                //objEndToEndSchematic.entity_name = item.source_network_id + "(" + Math.Abs(item.source_port_no) + "," + Math.Abs(objSameChild.source_port_no) + ")";
                                objEndToEndSchematic.entity_type = (objConnectionFilters.entityid == item.source_system_id && objConnectionFilters.entity_type == item.source_entity_type && objConnectionFilters.port_no == Math.Abs(item.source_port_no)) ? item.source_entity_type + "(Source)" : item.source_entity_type;
                                //SKIP SAME CHILD AND FETCH NEXT CHILD DETAILS..
                                var lstSubSubChilds = lstConnectionInfo.Where(x => x.source_system_id == objSameChild.destination_system_id & x.source_port_no == objSameChild.destination_port_no & x.source_entity_type.ToUpper() == objSameChild.destination_entity_type.ToUpper()).ToList();
                                if (lstSubSubChilds != null && lstSubSubChilds.Count > 0)
                                {

                                    if (lstSubSubChilds.Count > 1)
                                    {
                                        var elementInfo = lstConnectionInfo.Where(x => x.source_system_id == objSameChild.destination_system_id & x.source_port_no == objSameChild.destination_port_no & x.source_entity_type.ToUpper() == objSameChild.destination_entity_type.ToUpper()).FirstOrDefault();
                                        if (elementInfo != null)
                                        {
                                            objEndToEndSchematic.children = BindMultipleChild(lstConnectionInfo, lstSubSubChilds, elementInfo, objConnectionFilters, lstProcessedElements);
                                        }

                                    }
                                    else
                                    {

                                        objEndToEndSchematic.children = BindChild(lstConnectionInfo, lstSubSubChilds, objConnectionFilters, lstProcessedElements);
                                    }
                                }
                                else
                                {
                                    EndToEndSchematic objLastEndToEndSchematic = new EndToEndSchematic();
                                    objLastEndToEndSchematic.id = objSameChild.connection_id.ToString() + "123";
                                    objLastEndToEndSchematic.entity_name = objSameChild.is_destination_virtual ? objSameChild.destination_display_name : objSameChild.destination_display_name + "(" + (objSameChild.destination_entity_type.ToUpper() != "CABLE" && objSameChild.destination_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(objSameChild.destination_port_no) : objSameChild.destination_port_no.ToString()) + ")";
                                    //objLastEndToEndSchematic.entity_name = objSameChild.is_destination_virtual ? objSameChild.destination_network_id : objSameChild.destination_network_id + "(" + (objSameChild.destination_entity_type.ToUpper() != "CABLE" && objSameChild.destination_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(objSameChild.destination_port_no) : objSameChild.destination_port_no.ToString()) + ")";
                                    objLastEndToEndSchematic.entity_type = (objConnectionFilters.entityid == objSameChild.destination_system_id && objConnectionFilters.entity_type == objSameChild.destination_entity_type && objConnectionFilters.port_no == objSameChild.destination_port_no) ? objSameChild.destination_entity_type + "(Source)" : objSameChild.destination_entity_type;
                                    List<EndToEndSchematic> lstLastEndToEndSchematic = new List<EndToEndSchematic>();
                                    lstLastEndToEndSchematic.Add(objLastEndToEndSchematic);
                                    objEndToEndSchematic.children = lstLastEndToEndSchematic;
                                }

                            }
                            else
                            {
                                objEndToEndSchematic.children = BindChild(lstConnectionInfo, lstSubChilds, objConnectionFilters, lstProcessedElements);
                            }
                        }
                        else
                        {

                            objEndToEndSchematic.children = BindChild(lstConnectionInfo, lstSubChilds, objConnectionFilters, lstProcessedElements);
                        }
                    }
                    else
                    {
                        EndToEndSchematic objLastEndToEndSchematic = new EndToEndSchematic();
                        objLastEndToEndSchematic.id = item.connection_id.ToString() + "123";
                        objLastEndToEndSchematic.entity_name = item.is_destination_virtual ? item.destination_display_name : item.destination_display_name + "(" + (item.destination_entity_type.ToUpper() != "CABLE" && item.destination_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(item.destination_port_no) : item.destination_port_no.ToString()) + ")";
                        //objLastEndToEndSchematic.entity_name = item.is_destination_virtual ? item.destination_network_id : item.destination_network_id + "(" + (item.destination_entity_type.ToUpper() != "CABLE" && item.destination_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(item.destination_port_no) : item.destination_port_no.ToString()) + ")";
                        objLastEndToEndSchematic.entity_type = (objConnectionFilters.entityid == item.destination_system_id && objConnectionFilters.entity_type == item.destination_entity_type && objConnectionFilters.port_no == item.destination_port_no) ? item.destination_entity_type + "(Source)" : item.destination_entity_type;
                        List<EndToEndSchematic> lstLastEndToEndSchematic = new List<EndToEndSchematic>();
                        lstLastEndToEndSchematic.Add(objLastEndToEndSchematic);
                        objEndToEndSchematic.children = lstLastEndToEndSchematic;
                    }
                    lstEndToEndSchematic.Add(objEndToEndSchematic);
                    // }
                }
            }

            return lstEndToEndSchematic;
        }

        public List<EndToEndSchematic> BindMultipleChild(List<ConnectionInfo> lstConnectionInfo, List<ConnectionInfo> lstChildConnection, ConnectionInfo splitterInfo, ConnectionInfoFilter objConnectionFilters, List<string> lstProcessedElements)
        {
            EndToEndSchematic endToEndSchematic = new Models.EndToEndSchematic();
            List<EndToEndSchematic> endToEndSchematicList = new List<EndToEndSchematic>();
            List<ConnectionInfo> customFilterList = new List<ConnectionInfo>();
            lstProcessedElements.Add(splitterInfo.source_system_id + splitterInfo.source_network_id + splitterInfo.source_port_no.ToString());
            endToEndSchematic.id = splitterInfo.id.ToString();
            endToEndSchematic.entity_type = splitterInfo.source_entity_type;
            endToEndSchematic.entity_type = (objConnectionFilters.entityid == splitterInfo.source_system_id && objConnectionFilters.entity_type == splitterInfo.source_entity_type && objConnectionFilters.port_no == splitterInfo.source_port_no) ? splitterInfo.source_entity_type + "(Source)" : splitterInfo.source_entity_type;
            endToEndSchematic.entity_name = splitterInfo.is_source_virtual ? splitterInfo.source_display_name : splitterInfo.source_display_name + "(" + (splitterInfo.source_entity_type.ToUpper() != "CABLE" && splitterInfo.source_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(splitterInfo.source_port_no) : splitterInfo.source_port_no.ToString()) + ")";
            //endToEndSchematic.entity_name = splitterInfo.is_source_virtual ? splitterInfo.source_network_id : splitterInfo.source_network_id + "(" + (splitterInfo.source_entity_type.ToUpper() != "CABLE" && splitterInfo.source_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(splitterInfo.source_port_no) : splitterInfo.source_port_no.ToString()) + ")";
            foreach (var item in lstChildConnection)
            {
                var splitterOutPutPort = lstConnectionInfo.Where(x => x.source_system_id == item.destination_system_id & x.source_port_no == item.destination_port_no & x.source_entity_type.ToUpper() == item.destination_entity_type.ToUpper()).FirstOrDefault();
                if (splitterOutPutPort != null)
                {
                    customFilterList.Add(splitterOutPutPort);
                }
            }
            //only one child..
            if (customFilterList.Count == 1)
            {
                //only one element is connected..
                //NEED TO SHOW THE NODES IN COMBINED FORMAT LIKE GGN-ARAXHS_001_ADB01_SPL01(1,3)
                var objChild = customFilterList[0];
                endToEndSchematic.entity_name = splitterInfo.source_display_name + "(" + Math.Abs(splitterInfo.source_port_no) + "," + Math.Abs(objChild.source_port_no) + ")";
                //endToEndSchematic.entity_name = splitterInfo.source_network_id + "(" + Math.Abs(splitterInfo.source_port_no) + "," + Math.Abs(objChild.source_port_no) + ")";
                //SKIP SAME CHILD AND FETCH NEXT CHILD DETAILS..
                var objSubChilds = lstConnectionInfo.Where(x => x.source_system_id == objChild.destination_system_id & x.source_port_no == objChild.destination_port_no & x.source_entity_type.ToUpper() == objChild.destination_entity_type.ToUpper()).FirstOrDefault();
                customFilterList = new List<ConnectionInfo>();
                if (objSubChilds != null)
                {
                    customFilterList.Add(objSubChilds);
                }
                else
                {
                    EndToEndSchematic objLastEndToEndSchematic = new EndToEndSchematic();
                    objLastEndToEndSchematic.id = objChild.connection_id.ToString() + "123";
                    objLastEndToEndSchematic.entity_name = objChild.is_destination_virtual ? objChild.destination_display_name : objChild.destination_display_name + "(" + (objChild.destination_entity_type.ToUpper() != "CABLE" && objChild.destination_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(objChild.destination_port_no) : objChild.destination_port_no.ToString()) + ")";
                    //objLastEndToEndSchematic.entity_name = objChild.is_destination_virtual ? objChild.destination_network_id : objChild.destination_network_id + "(" + (objChild.destination_entity_type.ToUpper() != "CABLE" && objChild.destination_entity_type.ToUpper() != "CUSTOMER" ? MiscHelper.getPortName(objChild.destination_port_no) : objChild.destination_port_no.ToString()) + ")";
                    objLastEndToEndSchematic.entity_type = (objConnectionFilters.entityid == objChild.destination_system_id && objConnectionFilters.entity_type == objChild.destination_entity_type && objConnectionFilters.port_no == objChild.destination_port_no) ? objChild.destination_entity_type + "(Source)" : objChild.destination_entity_type;
                    List<EndToEndSchematic> lstLastEndToEndSchematic = new List<EndToEndSchematic>();
                    lstLastEndToEndSchematic.Add(objLastEndToEndSchematic);
                    endToEndSchematic.children = lstLastEndToEndSchematic;
                }
            }
            if (customFilterList.Count > 0)
            {

                endToEndSchematic.children = BindChild(lstConnectionInfo, customFilterList, objConnectionFilters, lstProcessedElements);
            }
            endToEndSchematicList.Add(endToEndSchematic);
            return endToEndSchematicList;
        }



        //public List<EndToEndSchematic> BindChild(List<ConnectionInfo> lstConnectionInfo, List<ConnectionInfo> lstChildConnections, ConnectionInfoFilter objConnectionFilters)
        //{

        //    List<EndToEndSchematic> lstEndToEndSchematic = new List<EndToEndSchematic>();

        //    foreach (var item in lstChildConnections)
        //    {
        //        EndToEndSchematic objEndToEndSchematic = new EndToEndSchematic();
        //        List<ConnectionInfo> lstSubChilds = new List<ConnectionInfo>();
        //        objEndToEndSchematic.id = item.connection_id.ToString();

        //        objEndToEndSchematic.entity_name = item.is_source_virtual? item.source_network_id: item.source_network_id + "(" + (item.source_entity_type.ToUpper() != "CABLE" ? MiscHelper.getPortName(item.source_port_no) : item.source_port_no.ToString()) + ")";

        //        objEndToEndSchematic.entity_type = (objConnectionFilters.entityid == item.source_system_id && objConnectionFilters.entity_type == item.source_entity_type && objConnectionFilters.port_no == item.source_port_no) ? item.source_entity_type + "(Source)" : item.source_entity_type;

        //        //FETCH CHILD INFORMATION..
        //        lstSubChilds = lstConnectionInfo.Where(x => x.source_system_id == item.destination_system_id & x.source_port_no == item.destination_port_no & x.source_entity_type == item.destination_entity_type).ToList();
        //        if (lstSubChilds != null && lstSubChilds.Count == 1)
        //        {
        //            if (item.source_system_id == lstSubChilds[0].source_system_id && item.source_entity_type.ToUpper() == lstSubChilds[0].source_entity_type.ToUpper())
        //            {
        //                // SAME CHILD WITH DIFFERENT PORT LIKE SC , SPL, ADB CONNECTED DIRECTLY WITH OUTPUT PORT
        //                //NEED TO SHOW THE NODES IN COMBINED FORMAT LIKE GGN-JCL000135(1,3)
        //                var objSameChild = lstSubChilds[0];
        //                objEndToEndSchematic.entity_name = item.source_network_id + "(" + Math.Abs(item.source_port_no) + "," + Math.Abs(objSameChild.source_port_no) + ")";

        //                //SKIP SAME CHILD AND FETCH NEXT CHILD DETAILS..
        //                var lstSubSubChilds = lstConnectionInfo.Where(x => x.source_system_id == objSameChild.destination_system_id & x.source_port_no == objSameChild.destination_port_no & x.source_entity_type.ToUpper() == objSameChild.destination_entity_type.ToUpper()).ToList();
        //                if (lstSubSubChilds != null && lstSubSubChilds.Count > 0)
        //                {
        //                    objEndToEndSchematic.children = BindChild(lstConnectionInfo, lstSubSubChilds, objConnectionFilters);
        //                }
        //                else
        //                {
        //                    EndToEndSchematic objLastEndToEndSchematic = new EndToEndSchematic();
        //                    objLastEndToEndSchematic.id = objSameChild.connection_id.ToString();
        //                    objLastEndToEndSchematic.entity_name = objSameChild.is_destination_virtual ? objSameChild.destination_network_id : objSameChild.destination_network_id + "(" + (objSameChild.destination_entity_type.ToUpper() != "CABLE" ? MiscHelper.getPortName(objSameChild.destination_port_no) : objSameChild.destination_port_no.ToString()) + ")";
        //                    objLastEndToEndSchematic.entity_type = (objConnectionFilters.entityid == objSameChild.destination_system_id && objConnectionFilters.entity_type == objSameChild.destination_entity_type && objConnectionFilters.port_no == objSameChild.destination_port_no) ? objSameChild.destination_entity_type + "(Source)" : objSameChild.destination_entity_type;
        //                    List<EndToEndSchematic> lstLastEndToEndSchematic = new List<EndToEndSchematic>();
        //                    lstLastEndToEndSchematic.Add(objLastEndToEndSchematic);
        //                    objEndToEndSchematic.children = lstLastEndToEndSchematic;
        //                }
        //            }
        //            else
        //            {
        //                objEndToEndSchematic.children = BindChild(lstConnectionInfo, lstSubChilds, objConnectionFilters);
        //            }
        //        }

        //        else if (lstSubChilds.Count > 1)
        //        {
        //            var elementInfo = lstConnectionInfo.Where(x => x.source_system_id == item.destination_system_id & x.source_port_no == item.destination_port_no & x.source_entity_type.ToUpper() == item.destination_entity_type.ToUpper()).FirstOrDefault();
        //            if (elementInfo != null)
        //            {
        //                objEndToEndSchematic.children = BindMultipleChild(lstConnectionInfo, lstSubChilds, elementInfo, objConnectionFilters);
        //            }
        //            else
        //            {

        //            }
        //        }
        //        else
        //        {
        //            EndToEndSchematic objLastEndToEndSchematic = new EndToEndSchematic();
        //            objLastEndToEndSchematic.id = item.connection_id.ToString()+"123";
        //            objLastEndToEndSchematic.entity_name = item.is_destination_virtual ? item.destination_network_id : item.destination_network_id + "(" + (item.destination_entity_type.ToUpper() != "CABLE" ? MiscHelper.getPortName(item.destination_port_no) : item.destination_port_no.ToString()) + ")";
        //            objLastEndToEndSchematic.entity_type = (objConnectionFilters.entityid == item.destination_system_id && objConnectionFilters.entity_type == item.destination_entity_type && objConnectionFilters.port_no == item.destination_port_no) ? item.destination_entity_type + "(Source)" : item.destination_entity_type;
        //            List<EndToEndSchematic> lstLastEndToEndSchematic = new List<EndToEndSchematic>();
        //            lstLastEndToEndSchematic.Add(objLastEndToEndSchematic);
        //            objEndToEndSchematic.children = lstLastEndToEndSchematic;
        //        }


        //        lstEndToEndSchematic.Add(objEndToEndSchematic);
        //    }

        //    return lstEndToEndSchematic;
        //}

        //public List<EndToEndSchematic> BindMultipleChild(List<ConnectionInfo> lstConnectionInfo, List<ConnectionInfo> lstChildConnection, ConnectionInfo splitterInfo, ConnectionInfoFilter objConnectionFilters)
        //{
        //    EndToEndSchematic endToEndSchematic = new Models.EndToEndSchematic();
        //    List<EndToEndSchematic> endToEndSchematicList = new List<EndToEndSchematic>();
        //    List<ConnectionInfo> customFilterList = new List<ConnectionInfo>();
        //    endToEndSchematic.id = splitterInfo.id.ToString();
        //    endToEndSchematic.entity_type = splitterInfo.source_entity_type;
        //    endToEndSchematic.entity_name = splitterInfo.is_source_virtual?splitterInfo.source_network_id: splitterInfo.source_network_id + "(" + (splitterInfo.source_entity_type.ToUpper() != "CABLE" ? MiscHelper.getPortName(splitterInfo.source_port_no) : splitterInfo.source_port_no.ToString()) + ")";
        //    foreach (var item in lstChildConnection)
        //    {
        //        var splitterOutPutPort = lstConnectionInfo.Where(x => x.source_system_id == item.destination_system_id & x.source_port_no == item.destination_port_no & x.source_entity_type.ToUpper() == item.destination_entity_type.ToUpper()).FirstOrDefault();
        //        if (splitterOutPutPort != null)
        //        {
        //            customFilterList.Add(splitterOutPutPort);
        //        }
        //    }
        //    endToEndSchematic.children = BindChild(lstConnectionInfo, customFilterList, objConnectionFilters);
        //    endToEndSchematicList.Add(endToEndSchematic);
        //    return endToEndSchematicList;
        //}

        [HttpGet]
        public void ExportConnections(int source_system_id, string source_type, bool is_source_connected, string connecting_entity, int destination_system_id, string destination_type, bool is_destination_connected, string image_path, string exportType, string exportKey, string customer_Ids)
        {
            connectionInput objSplicingIn = new connectionInput();
            objSplicingIn.source_system_id = source_system_id;
            objSplicingIn.source_entity_type = source_type;
            objSplicingIn.is_source_start_point = is_source_connected;
            objSplicingIn.listConnector = JsonConvert.DeserializeObject<List<connectors>>(connecting_entity);
            string connector = objSplicingIn.listConnector.First().entity_type.ToUpper();
            objSplicingIn.destination_system_id = destination_system_id;
            objSplicingIn.destination_entity_type = destination_type;
            objSplicingIn.is_destination_start_point = is_destination_connected;
            objSplicingIn.customer_ids = customer_Ids;

            SplicingViewModel splicingEntity = new SplicingViewModel();
            //List<SplicingConectionInfo> listConnections = new List<SplicingConectionInfo>();
            splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfoReport(objSplicingIn, connecting_entity);
            DataTable dtlogs1 = Utility.MiscHelper.ListToDataTable(splicingEntity.splicingConnections);
            List<SplicingConectionInfo> _listConnections = splicingEntity.splicingConnections.Where(m => m.destination_system_id > 0).ToList();
            List<ExportSplicingInfo> lstSplicingInfo = new List<ExportSplicingInfo>();
            if (exportKey == "CPE2C")
            {
                lstSplicingInfo = GetConnectionInfo(_listConnections, objSplicingIn.listConnector.Where(x => x.entity_type == objSplicingIn.listConnector[0].entity_type).FirstOrDefault().system_id, objSplicingIn.listConnector.Where(x => x.entity_type == objSplicingIn.listConnector[0].entity_type).FirstOrDefault().entity_type);
            }
            else if (exportKey == "FMS2C")
            {
                lstSplicingInfo = GetConnectionInfo(_listConnections, objSplicingIn.listConnector[0].system_id, objSplicingIn.listConnector[0].entity_type);
            }
            else
            {
                lstSplicingInfo = GetConnectionInfo(_listConnections, objSplicingIn.source_system_id, connector);
            }
            DataSet ds = new DataSet();
            DataTable dtlogs = Utility.MiscHelper.ListToDataTable(lstSplicingInfo);
            dtlogs.TableName = "Splicing Report";
            if (dtlogs.Rows.Count > 0)
            {
                dtlogs.Columns.Remove("SOURCE_PORT");
                dtlogs.Columns.Remove("VIA_ENTITY_PORT");
                dtlogs.Columns.Remove("USING_ENTITY_INPUT");
                dtlogs.Columns.Remove("USING_ENTITY_OUTPUT");
                dtlogs.Columns.Remove("DESTINATION_PORT_NO");
                dtlogs.Columns.Remove("SOURCE_TRAY_DISPLAY_NAME");
                dtlogs.Columns.Remove("DESTINATION_TRAY_DISPLAY_NAME");
                dtlogs.Columns.Remove("IS_THROUGH_CONNECTION");
                dtlogs.Columns["SOURCE_NETWORK_ID"].SetOrdinal(0);
                dtlogs.Columns["SOURCE_TYPE"].SetOrdinal(1);
                dtlogs.Columns["SOURCE_TUBE_NO"].SetOrdinal(2);
                dtlogs.Columns["SOURCE_PORT_NO"].SetOrdinal(3);
                dtlogs.Columns["VIA_ENTITY_ID"].SetOrdinal(4);
                dtlogs.Columns["VIA_ENTITY"].SetOrdinal(5);
                dtlogs.Columns["VIA_PORT"].SetOrdinal(6);
                dtlogs.Columns["tray_name"].SetOrdinal(7);
                dtlogs.Columns["USING_ENTITY_ID"].SetOrdinal(8);
                dtlogs.Columns["USING_ENTITY"].SetOrdinal(9);
                dtlogs.Columns["USING_INPUT"].SetOrdinal(10);
                dtlogs.Columns["USING_OUTPUT"].SetOrdinal(11);
                dtlogs.Columns["DESTINATION_NETWORK_ID"].SetOrdinal(12);
                dtlogs.Columns["DESTINATION_TYPE"].SetOrdinal(13);
                dtlogs.Columns["DESTINATION_TUBE_NO"].SetOrdinal(14);
                dtlogs.Columns["DESTINATION_PORT"].SetOrdinal(15);

                if (dtlogs.Columns.Contains("SOURCE_NETWORK_ID")) { dtlogs.Columns["SOURCE_NETWORK_ID"].ColumnName = "Source"; }
                if (dtlogs.Columns.Contains("SOURCE_TYPE")) { dtlogs.Columns["SOURCE_TYPE"].ColumnName = "Source Entity Type"; }
                if (dtlogs.Columns.Contains("SOURCE_TUBE_NO")) { dtlogs.Columns["SOURCE_TUBE_NO"].ColumnName = "Source Tube No."; }
                if (dtlogs.Columns.Contains("SOURCE_PORT_NO")) { dtlogs.Columns["SOURCE_PORT_NO"].ColumnName = "Source Core/Port No."; }
                if (dtlogs.Columns.Contains("VIA_ENTITY_ID")) { dtlogs.Columns["VIA_ENTITY_ID"].ColumnName = "Via Entity"; }
                if (dtlogs.Columns.Contains("VIA_ENTITY")) { dtlogs.Columns["VIA_ENTITY"].ColumnName = "Via Entity Type"; }
                if (dtlogs.Columns.Contains("VIA_PORT")) { dtlogs.Columns["VIA_PORT"].ColumnName = "Via Entity Port No."; }
                if (dtlogs.Columns.Contains("tray_name")) { dtlogs.Columns["tray_name"].ColumnName = "Via Tray Name"; }
                if (dtlogs.Columns.Contains("USING_ENTITY_ID")) { dtlogs.Columns["USING_ENTITY_ID"].ColumnName = "Using Entity"; }
                if (dtlogs.Columns.Contains("USING_ENTITY")) { dtlogs.Columns["USING_ENTITY"].ColumnName = "Using Entity Type"; }
                if (dtlogs.Columns.Contains("USING_INPUT")) { dtlogs.Columns["USING_INPUT"].ColumnName = "Using Entity Port In"; }
                if (dtlogs.Columns.Contains("USING_OUTPUT")) { dtlogs.Columns["USING_OUTPUT"].ColumnName = "Using Entity Port Out"; }
                if (dtlogs.Columns.Contains("DESTINATION_NETWORK_ID")) { dtlogs.Columns["DESTINATION_NETWORK_ID"].ColumnName = "Destination"; }
                if (dtlogs.Columns.Contains("DESTINATION_TYPE")) { dtlogs.Columns["DESTINATION_TYPE"].ColumnName = "Destination Entity Type"; }
                if (dtlogs.Columns.Contains("DESTINATION_TUBE_NO")) { dtlogs.Columns["DESTINATION_TUBE_NO"].ColumnName = "Destination Tube No."; }
                if (dtlogs.Columns.Contains("DESTINATION_PORT")) { dtlogs.Columns["DESTINATION_PORT"].ColumnName = "Destination Core/Port No."; }
                if (dtlogs.Columns.Contains("CONNECTIVITY_TYPE")) { dtlogs.Columns["CONNECTIVITY_TYPE"].ColumnName = "Connectivity Type"; }


                ds.Tables.Add(dtlogs);
            }
            Dictionary<string, string> dicImages = new Dictionary<string, string>();
            // dicImages.Add("Splicing Image", @"D:\Pawan_Kumar\SVN\Spactra\branches\NT_Wireless_1\SmartInventory\SmartInventory\Uploads\temp_LOS\LOS_54cfc91d-707d-45cd-ae5b-ee75e0ef1c9e.png");
            dicImages.Add("Splicing Image", image_path);

            if (exportType == "EXCEL")
            {
                if (dtlogs.Rows.Count > 0)
                    ExportData(dtlogs, "Splicing_Connections", dicImages);
                else
                    ExportDataImg("Splicing_Connections", dicImages);
            }

            else
            {
                PDFHelper.GenerateToPDF(ds, "Splicing_Details", "Connection Details", image_path);
            }

        }

        private List<ExportSplicingInfo> GetConnectionInfo(List<SplicingConectionInfo> lstSplicingInfo, int sourceSystemId, string connectingElement)
        {
            List<ExportSplicingInfo> _lstExportSplicingInfo = new List<ExportSplicingInfo>();

            switch (connectingElement)
            {
                case "SPLICECLOSURE":
                    return GetSpliceClosureConnectionInfo(lstSplicingInfo, sourceSystemId);

                case "FDB":
                case "ADB":
                case "CDB":
                case "BDB":
                    return GetParentConnectionInfo(lstSplicingInfo, sourceSystemId, connectingElement);
                case "HTB":
                    return GetHTBConnectionInfo(lstSplicingInfo, sourceSystemId);
                case "ONT":
                    return GetONTConnectionInfo(lstSplicingInfo, sourceSystemId);
                case "FMS":
                    return GetODFConnectionInfo(lstSplicingInfo, sourceSystemId);
                default:
                    break;
            }
            return _lstExportSplicingInfo;
        }
        private List<ExportSplicingInfo> GetSpliceClosureConnectionInfo(List<SplicingConectionInfo> lstSplicingInfo, int sourceSystemId)
        {
            List<ExportSplicingInfo> _lstExportSplicingInfo = new List<ExportSplicingInfo>();
            foreach (SplicingConectionInfo connectionInfo in lstSplicingInfo.Where(m => m.system_id == sourceSystemId).ToList())
            {
                ExportSplicingInfo _objExportSplicing = new ExportSplicingInfo();

                _objExportSplicing.source_network_id = connectionInfo.display_name;
                _objExportSplicing.source_type = connectionInfo.entity_type_title;
                _objExportSplicing.source_tube_no = connectionInfo.tube_number;
                _objExportSplicing.source_port_no = connectionInfo.core_port_number;
                _objExportSplicing.source_port = connectionInfo.source_port;

                _objExportSplicing.via_entity = connectionInfo.via_entity_type_title;
                _objExportSplicing.via_entity_id = connectionInfo.via_display_name;
                _objExportSplicing.via_entity_port = connectionInfo.via_port_no;
                _objExportSplicing.via_port = connectionInfo.via_port;

                _objExportSplicing.destination_network_id = connectionInfo.destination_display_name;
                _objExportSplicing.destination_type = connectionInfo.destination_entity_type_title;
                _objExportSplicing.destination_port_no = connectionInfo.destination_port_no;
                _objExportSplicing.destination_port = connectionInfo.destination_port;
                _objExportSplicing.destination_tube_no = connectionInfo.destination_tube_number;

                _objExportSplicing.source_tray_display_name = connectionInfo.source_tray_display_name;
                _objExportSplicing.destination_tray_display_name = connectionInfo.destination_tray_display_name;
                _objExportSplicing.tray_name = connectionInfo.tray_name;
                if (connectionInfo.is_through_connection == true)
                    _objExportSplicing.Connectivity_Type = "Through";
                else
                    _objExportSplicing.Connectivity_Type = "Splice";
                _lstExportSplicingInfo.Add(_objExportSplicing);
            }

            //foreach (SplicingConectionInfo connectionInfo in lstSplicingInfo.Where(m => m.system_id != sourceSystemId).ToList())
            //{
            //    //t.via_entity=r.via_entity_type and t.via_entity_id=r.via_network_id and t.via_entity_port=r.via_port_no and t.source_network_id<>r.network_id;
            //    ExportSplicingInfo _objExportSplicing = _lstExportSplicingInfo.Where(m => m.via_entity == connectionInfo.via_entity_type && m.via_entity_id == connectionInfo.via_network_id && m.via_entity_port == connectionInfo.via_port_no).FirstOrDefault();

            //}

            //foreach (ExportSplicingInfo exportSplicing in _lstExportSplicingInfo)
            //{
            //    //t.via_entity=r.via_entity_type and t.via_entity_id=r.via_network_id and t.via_entity_port=r.via_port_no and t.source_network_id<>r.network_id;
            //    SplicingConectionInfo _objExportSplicing = lstSplicingInfo.Where(m => m.via_entity_type == exportSplicing.via_entity
            //    && m.via_network_id == exportSplicing.via_entity_id && m.via_port_no == exportSplicing.via_entity_port).FirstOrDefault();
            //    if (_objExportSplicing != null)
            //    {
            //        exportSplicing.destination_network_id = _objExportSplicing.network_id;
            //        exportSplicing.destination_type = _objExportSplicing.entity_type;
            //        exportSplicing.destination_port_no = _objExportSplicing.core_port_number;
            //        exportSplicing.destination_tube_no = _objExportSplicing.no_of_tube;
            //    }
            //}
            return _lstExportSplicingInfo;

        }

        private List<ExportSplicingInfo> GetParentConnectionInfo(List<SplicingConectionInfo> lstSplicingInfo, int sourceSystemId, string connectingElement)
        {
            List<ExportSplicingInfo> _lstExportSplicingInfo = new List<ExportSplicingInfo>();
            List<SplicingConectionInfo> lvlItems = lstSplicingInfo.ToList().FindAll(x => x.system_id == sourceSystemId && x.core_port_status == "Connected").OrderBy(x => x.core_port_number).ToList();
            bool isVirtualPortAllowed = false;
            switch (connectingElement)
            {
                case "FDB":
                    isVirtualPortAllowed = new BLLayer().getLayer(EntityType.FDB.ToString()).is_virtual_port_allowed;
                    break;
                case "ADB":
                    isVirtualPortAllowed = new BLLayer().getLayer(EntityType.ADB.ToString()).is_virtual_port_allowed;
                    break;
                case "CDB":
                    isVirtualPortAllowed = new BLLayer().getLayer(EntityType.CDB.ToString()).is_virtual_port_allowed;
                    break;
                case "BDB":
                    isVirtualPortAllowed = new BLLayer().getLayer(EntityType.BDB.ToString()).is_virtual_port_allowed;
                    break;
                default:
                    break;
            }
            foreach (SplicingConectionInfo splicing in lvlItems)
            {

                ExportSplicingInfo exportSplicingInfo = new ExportSplicingInfo();
                exportSplicingInfo.tray_name = splicing.source_tray_display_name;
                if (string.IsNullOrEmpty(splicing.source_tray_display_name))
                {
                    exportSplicingInfo.tray_name = splicing.destination_tray_display_name;
                }
                exportSplicingInfo.source_network_id = splicing.display_name;
                exportSplicingInfo.source_port_no = splicing.core_port_number;
                exportSplicingInfo.source_port = splicing.source_port;
                exportSplicingInfo.source_type = splicing.entity_type_title;
                exportSplicingInfo.source_tube_no = splicing.tube_number;
                exportSplicingInfo.via_entity = splicing.via_entity_type_title;
                exportSplicingInfo.via_entity_id = splicing.via_display_name;
                exportSplicingInfo.via_entity_port = isVirtualPortAllowed == false ? null : splicing.via_port_no;
                exportSplicingInfo.via_port = splicing.via_port;
                exportSplicingInfo.source_tray_display_name = splicing.source_tray_display_name;
                exportSplicingInfo.destination_tray_display_name = splicing.destination_tray_display_name;
                exportSplicingInfo.destination_tube_no = splicing.destination_tube_number;
                if (splicing.is_through_connection == true)
                    exportSplicingInfo.Connectivity_Type = "Through";
                else
                    exportSplicingInfo.Connectivity_Type = "Splice";

                if (splicing.destination_entity_type.ToUpper() == EntityType.Splitter.ToString().ToUpper() || splicing.destination_entity_type.ToUpper() == EntityType.FMS.ToString().ToUpper())
                {
                    exportSplicingInfo.using_entity = splicing.destination_entity_type_title;
                    exportSplicingInfo.using_entity_id = splicing.destination_display_name;
                    exportSplicingInfo.using_entity_input = splicing.destination_port_no;
                    exportSplicingInfo.using_input = splicing.destination_port;

                    var _lstSplitterSplicingInfo = AddSplitterConnection(splicing.destination_network_id, splicing.destination_system_id.Value, lstSplicingInfo, exportSplicingInfo);
                    _lstExportSplicingInfo.AddRange(_lstSplitterSplicingInfo);
                }
                else
                {
                    exportSplicingInfo.using_entity = String.Empty;
                    exportSplicingInfo.using_entity_id = String.Empty;
                    exportSplicingInfo.using_entity_input = null;
                    exportSplicingInfo.using_entity_output = null;
                    exportSplicingInfo.destination_network_id = splicing.destination_display_name;
                    exportSplicingInfo.destination_port_no = splicing.destination_port_no;
                    exportSplicingInfo.destination_port = splicing.destination_port;
                    exportSplicingInfo.destination_type = splicing.destination_entity_type_title;
                    _lstExportSplicingInfo.Add(exportSplicingInfo);
                }

            }
            return _lstExportSplicingInfo;

        }

        public List<ExportSplicingInfo> AddSplitterConnection(string _source_network_id, int _source_system_id, IList<SplicingConectionInfo> element, ExportSplicingInfo elementsSplitterInfo)
        {
            if (element == null || elementsSplitterInfo == null) return null;
            List<ExportSplicingInfo> lstExportSplitterSplicingInfo = new List<ExportSplicingInfo>();
            List<SplicingConectionInfo> lvlSplitterItems = element.ToList().FindAll(x => x.system_id == _source_system_id && x.network_id == _source_network_id && x.input_output == "O").OrderBy(x => x.core_port_status).ToList();
            foreach (SplicingConectionInfo splicingInfo in lvlSplitterItems)
            {

                ExportSplicingInfo exportSplitterSplicingInfo = new ExportSplicingInfo();
                string sourceNetworkId = "";
                int sourceSystemId = 0;

                exportSplitterSplicingInfo.source_network_id = elementsSplitterInfo.source_network_id;
                exportSplitterSplicingInfo.source_port_no = elementsSplitterInfo.source_port_no;
                exportSplitterSplicingInfo.source_type = elementsSplitterInfo.source_type;
                exportSplitterSplicingInfo.source_port = elementsSplitterInfo.source_port;
                exportSplitterSplicingInfo.source_tube_no = elementsSplitterInfo.source_tube_no;
                exportSplitterSplicingInfo.via_entity = elementsSplitterInfo.via_entity;
                exportSplitterSplicingInfo.via_entity_id = elementsSplitterInfo.via_entity_id;
                exportSplitterSplicingInfo.via_entity_port = elementsSplitterInfo.via_entity_port;
                exportSplitterSplicingInfo.via_port = elementsSplitterInfo.via_port;
                exportSplitterSplicingInfo.destination_network_id = splicingInfo.destination_display_name;
                exportSplitterSplicingInfo.destination_port_no = splicingInfo.destination_port_no;
                exportSplitterSplicingInfo.destination_port = splicingInfo.destination_port;
                exportSplitterSplicingInfo.destination_type = splicingInfo.destination_entity_type_title;

                exportSplitterSplicingInfo.destination_tube_no = splicingInfo.destination_tube_number;
                exportSplitterSplicingInfo.using_entity = elementsSplitterInfo.using_entity;
                exportSplitterSplicingInfo.using_entity_id = elementsSplitterInfo.using_entity_id;
                exportSplitterSplicingInfo.using_entity_input = elementsSplitterInfo.using_entity_input;
                exportSplitterSplicingInfo.using_input = elementsSplitterInfo.using_input;
                exportSplitterSplicingInfo.using_entity_output = splicingInfo.core_port_number;
                exportSplitterSplicingInfo.using_output = splicingInfo.source_port;
                lstExportSplitterSplicingInfo.Add(exportSplitterSplicingInfo);
                if (splicingInfo.destination_entity_type.ToUpper() == EntityType.Splitter.ToString().ToUpper() || splicingInfo.destination_entity_type.ToUpper() == EntityType.FMS.ToString().ToUpper())
                {
                    ExportSplicingInfo exportSplitterInfo = new ExportSplicingInfo();
                    exportSplitterInfo.source_network_id = elementsSplitterInfo.using_entity_id;
                    exportSplitterInfo.source_port_no = splicingInfo.core_port_number;
                    exportSplitterInfo.source_port = splicingInfo.source_port;
                    exportSplitterInfo.source_type = elementsSplitterInfo.using_entity;


                    exportSplitterInfo.via_entity = elementsSplitterInfo.via_entity;
                    exportSplitterInfo.via_entity_id = elementsSplitterInfo.via_entity_id;
                    exportSplitterInfo.via_entity_port = elementsSplitterInfo.via_entity_port;
                    exportSplitterInfo.via_port = elementsSplitterInfo.via_port;
                    exportSplitterInfo.destination_network_id = splicingInfo.destination_display_name;
                    exportSplitterInfo.destination_port_no = splicingInfo.destination_port_no;
                    exportSplitterInfo.destination_port = splicingInfo.destination_port;
                    exportSplitterInfo.destination_type = splicingInfo.destination_entity_type_title;

                    exportSplitterInfo.destination_tube_no = splicingInfo.destination_tube_number;
                    exportSplitterInfo.using_entity = splicingInfo.destination_entity_type_title;
                    exportSplitterInfo.using_entity_id = splicingInfo.destination_display_name;
                    exportSplitterInfo.using_entity_input = splicingInfo.destination_port_no;
                    exportSplitterInfo.using_input = splicingInfo.destination_port;
                    exportSplitterInfo.using_entity_output = splicingInfo.core_port_number;
                    exportSplitterInfo.using_output = splicingInfo.source_port;

                    sourceNetworkId = splicingInfo.destination_network_id;
                    sourceSystemId = splicingInfo.destination_port_no.Value;

                    var _lstSplitterSplicingInfo = AddSplitterConnection(splicingInfo.destination_network_id, splicingInfo.destination_system_id.Value, element, exportSplitterInfo);
                    lstExportSplitterSplicingInfo.AddRange(_lstSplitterSplicingInfo);
                }

            }
            return lstExportSplitterSplicingInfo;
        }

        private List<ExportSplicingInfo> GetHTBConnectionInfo(List<SplicingConectionInfo> lstSplicingInfo, int sourceSystemId)
        {
            List<ExportSplicingInfo> _lstExportSplicingInfo = new List<ExportSplicingInfo>();
            List<SplicingConectionInfo> lvlItems = lstSplicingInfo.ToList().FindAll(x => x.system_id == sourceSystemId && x.core_port_status == "Connected").OrderBy(x => x.core_port_number).ToList();
            var isVirtualPortAllowed = new BLLayer().getLayer(EntityType.HTB.ToString()).is_virtual_port_allowed;
            foreach (SplicingConectionInfo splicing in lvlItems)
            {

                ExportSplicingInfo exportSplicingInfo = new ExportSplicingInfo();
                exportSplicingInfo.source_network_id = splicing.display_name;
                exportSplicingInfo.source_port_no = splicing.core_port_number;
                exportSplicingInfo.source_port = splicing.source_port;
                exportSplicingInfo.source_type = splicing.entity_type_title;
                exportSplicingInfo.source_tube_no = splicing.tube_number;
                exportSplicingInfo.via_entity = splicing.via_entity_type_title;
                exportSplicingInfo.via_entity_id = splicing.via_display_name;
                exportSplicingInfo.via_entity_port = isVirtualPortAllowed == false ? null : splicing.via_port_no;
                exportSplicingInfo.via_entity = splicing.via_port;


                exportSplicingInfo.destination_tube_no = splicing.destination_tube_number;
                if (splicing.is_through_connection == true)
                    exportSplicingInfo.Connectivity_Type = "Through";
                else
                    exportSplicingInfo.Connectivity_Type = "Splice";
                if (splicing.destination_entity_type.ToUpper() == EntityType.HTB.ToString().ToUpper())
                {
                    exportSplicingInfo.using_entity = splicing.destination_entity_type_title;
                    exportSplicingInfo.using_entity_id = splicing.destination_network_id;
                    exportSplicingInfo.using_entity_input = splicing.destination_port_no;
                    exportSplicingInfo.using_input = splicing.destination_port;

                    var _lstSplitterSplicingInfo = AddHTBConnection(splicing.destination_network_id, splicing.destination_system_id.Value, lstSplicingInfo, exportSplicingInfo);
                    if (_lstSplitterSplicingInfo.Count > 0)
                    {
                        _lstExportSplicingInfo.AddRange(_lstSplitterSplicingInfo);
                    }
                    else
                    {
                        _lstExportSplicingInfo.Add(exportSplicingInfo);
                    }
                }
                else
                {
                    exportSplicingInfo.using_entity = String.Empty;
                    exportSplicingInfo.using_entity_id = String.Empty;
                    exportSplicingInfo.using_entity_input = null;
                    exportSplicingInfo.using_entity_output = null;
                    exportSplicingInfo.destination_network_id = splicing.destination_display_name;
                    exportSplicingInfo.destination_port_no = splicing.destination_port_no;
                    exportSplicingInfo.destination_port = splicing.destination_port;
                    exportSplicingInfo.destination_type = splicing.destination_entity_type_title;
                    _lstExportSplicingInfo.Add(exportSplicingInfo);
                }

            }
            return _lstExportSplicingInfo;

        }
        public List<ExportSplicingInfo> AddHTBConnection(string _source_network_id, int _source_system_id, IList<SplicingConectionInfo> element, ExportSplicingInfo elementsHTBInfo)
        {
            if (element == null || elementsHTBInfo == null) return null;
            List<ExportSplicingInfo> lstExportHTBSplicingInfo = new List<ExportSplicingInfo>();
            List<SplicingConectionInfo> lvlHTBItems = element.ToList().FindAll(x => x.system_id == _source_system_id && x.network_id == _source_network_id && x.input_output == "O").OrderBy(x => x.core_port_status).ToList();
            foreach (SplicingConectionInfo splicingInfo in lvlHTBItems)
            {

                ExportSplicingInfo exportSplitterSplicingInfo = new ExportSplicingInfo();
                string sourceNetworkId = "";
                int sourceSystemId = 0;

                exportSplitterSplicingInfo.source_network_id = elementsHTBInfo.source_network_id;
                exportSplitterSplicingInfo.source_port_no = elementsHTBInfo.source_port_no;
                exportSplitterSplicingInfo.source_port = elementsHTBInfo.source_port;
                exportSplitterSplicingInfo.source_type = elementsHTBInfo.source_type;
                exportSplitterSplicingInfo.source_tube_no = elementsHTBInfo.source_tube_no;
                exportSplitterSplicingInfo.via_entity = elementsHTBInfo.via_entity;
                exportSplitterSplicingInfo.via_entity_id = elementsHTBInfo.via_entity_id;
                exportSplitterSplicingInfo.via_entity_port = elementsHTBInfo.via_entity_port;
                exportSplitterSplicingInfo.via_port = elementsHTBInfo.via_port;
                exportSplitterSplicingInfo.destination_network_id = splicingInfo.destination_network_id;
                exportSplitterSplicingInfo.destination_port_no = splicingInfo.destination_port_no;
                exportSplitterSplicingInfo.destination_port = splicingInfo.destination_port;
                exportSplitterSplicingInfo.destination_type = splicingInfo.destination_entity_type_title;

                exportSplitterSplicingInfo.destination_tube_no = splicingInfo.destination_tube_number;
                exportSplitterSplicingInfo.using_entity = elementsHTBInfo.using_entity;
                exportSplitterSplicingInfo.using_entity_id = elementsHTBInfo.using_entity_id;
                exportSplitterSplicingInfo.using_entity_input = elementsHTBInfo.using_entity_input;
                exportSplitterSplicingInfo.using_input = elementsHTBInfo.using_input;
                exportSplitterSplicingInfo.using_entity_output = splicingInfo.core_port_number;
                exportSplitterSplicingInfo.using_output = splicingInfo.source_port;
                lstExportHTBSplicingInfo.Add(exportSplitterSplicingInfo);
                if (splicingInfo.destination_entity_type.ToUpper() == EntityType.HTB.ToString().ToUpper())
                {
                    ExportSplicingInfo exportSplitterInfo = new ExportSplicingInfo();
                    exportSplitterInfo.source_network_id = elementsHTBInfo.using_entity_id;
                    exportSplitterInfo.source_port_no = splicingInfo.core_port_number;
                    exportSplitterInfo.source_port = splicingInfo.source_port;
                    exportSplitterInfo.source_type = elementsHTBInfo.using_entity;


                    exportSplitterInfo.via_entity = elementsHTBInfo.via_entity;
                    exportSplitterInfo.via_entity_id = elementsHTBInfo.via_entity_id;
                    exportSplitterInfo.via_entity_port = elementsHTBInfo.via_entity_port;
                    exportSplitterInfo.via_port = elementsHTBInfo.via_port;
                    exportSplitterInfo.destination_network_id = splicingInfo.destination_network_id;
                    exportSplitterInfo.destination_port_no = splicingInfo.destination_port_no;
                    exportSplitterInfo.destination_port = splicingInfo.destination_port;
                    exportSplitterInfo.destination_type = splicingInfo.destination_entity_type_title;

                    exportSplitterInfo.destination_tube_no = splicingInfo.destination_tube_number;
                    exportSplitterInfo.using_entity = splicingInfo.destination_entity_type;
                    exportSplitterInfo.using_entity_id = splicingInfo.destination_network_id;
                    exportSplitterInfo.using_entity_input = splicingInfo.destination_port_no;
                    exportSplitterInfo.using_input = splicingInfo.destination_port;
                    exportSplitterInfo.using_entity_output = splicingInfo.core_port_number;
                    exportSplitterInfo.using_output = splicingInfo.source_port;

                    sourceNetworkId = splicingInfo.destination_network_id;
                    sourceSystemId = splicingInfo.destination_port_no.Value;

                    var _lstHTBSplicingInfo = AddHTBConnection(splicingInfo.destination_network_id, splicingInfo.destination_system_id.Value, element, exportSplitterInfo);
                    lstExportHTBSplicingInfo.AddRange(_lstHTBSplicingInfo);
                }


            }
            return lstExportHTBSplicingInfo;
        }

        private List<ExportSplicingInfo> GetONTConnectionInfo(List<SplicingConectionInfo> lstSplicingInfo, int sourceSystemId)
        {
            List<ExportSplicingInfo> _lstExportSplicingInfo = new List<ExportSplicingInfo>();
            List<SplicingConectionInfo> lvlItems = lstSplicingInfo.ToList().FindAll(x => x.system_id == sourceSystemId && x.core_port_status == "Connected" && x.input_output == "O").OrderBy(x => x.core_port_number).ToList();
            var isVirtualPortAllowed = new BLLayer().getLayer(EntityType.ONT.ToString()).is_virtual_port_allowed;
            foreach (SplicingConectionInfo splicing in lvlItems)
            {

                ExportSplicingInfo exportSplicingInfo = new ExportSplicingInfo();
                exportSplicingInfo.source_network_id = splicing.display_name;
                exportSplicingInfo.source_port_no = splicing.core_port_number;
                exportSplicingInfo.source_port = splicing.source_port;
                exportSplicingInfo.source_type = splicing.entity_type_title;
                exportSplicingInfo.source_tube_no = splicing.tube_number;
                exportSplicingInfo.via_entity = splicing.via_entity_type_title;
                exportSplicingInfo.via_entity_id = splicing.via_display_name;
                exportSplicingInfo.via_entity_port = isVirtualPortAllowed == false ? null : splicing.via_port_no;
                exportSplicingInfo.via_port = splicing.via_port;

                exportSplicingInfo.destination_tube_no = splicing.destination_tube_number;
                if (splicing.is_through_connection == true)
                    exportSplicingInfo.Connectivity_Type = "Through";
                else
                    exportSplicingInfo.Connectivity_Type = "Splice";
                if (splicing.destination_entity_type.ToUpper() == EntityType.ONT.ToString().ToUpper())
                {
                    exportSplicingInfo.using_entity = splicing.destination_entity_type_title;
                    exportSplicingInfo.using_entity_id = splicing.destination_display_name;
                    exportSplicingInfo.using_entity_input = splicing.destination_port_no;
                    exportSplicingInfo.using_input = splicing.destination_port;

                    var _lstSplitterSplicingInfo = AddONTConnection(splicing.destination_network_id, splicing.destination_system_id.Value, lstSplicingInfo, exportSplicingInfo);
                    if (_lstSplitterSplicingInfo.Count > 0)
                    {
                        _lstExportSplicingInfo.AddRange(_lstSplitterSplicingInfo);
                    }
                    else
                    {
                        _lstExportSplicingInfo.Add(exportSplicingInfo);
                    }
                }
                else
                {
                    exportSplicingInfo.using_entity = String.Empty;
                    exportSplicingInfo.using_entity_id = String.Empty;
                    exportSplicingInfo.using_entity_input = null;
                    exportSplicingInfo.using_entity_output = null;
                    exportSplicingInfo.destination_network_id = splicing.destination_network_id;
                    exportSplicingInfo.destination_port_no = splicing.destination_port_no;
                    exportSplicingInfo.destination_port = splicing.destination_port;
                    exportSplicingInfo.destination_type = splicing.destination_entity_type_title;
                    _lstExportSplicingInfo.Add(exportSplicingInfo);
                }

            }
            return _lstExportSplicingInfo;

        }

        public List<ExportSplicingInfo> AddONTConnection(string _source_network_id, int _source_system_id, IList<SplicingConectionInfo> element, ExportSplicingInfo elementsONTInfo)
        {
            if (element == null || elementsONTInfo == null) return null;
            List<ExportSplicingInfo> lstExportONTSplicingInfo = new List<ExportSplicingInfo>();
            List<SplicingConectionInfo> lvlONTItems = element.ToList().FindAll(x => x.system_id == _source_system_id && x.network_id == _source_network_id && x.input_output == "O").OrderBy(x => x.core_port_status).ToList();
            foreach (SplicingConectionInfo splicingInfo in lvlONTItems)
            {

                ExportSplicingInfo exportSplitterSplicingInfo = new ExportSplicingInfo();
                string sourceNetworkId = "";
                int sourceSystemId = 0;

                exportSplitterSplicingInfo.source_network_id = elementsONTInfo.source_network_id;
                exportSplitterSplicingInfo.source_port_no = elementsONTInfo.source_port_no;
                exportSplitterSplicingInfo.source_port = elementsONTInfo.source_port;
                exportSplitterSplicingInfo.source_type = elementsONTInfo.source_type;
                exportSplitterSplicingInfo.source_tube_no = elementsONTInfo.source_tube_no;
                exportSplitterSplicingInfo.via_entity = elementsONTInfo.via_entity;
                exportSplitterSplicingInfo.via_entity_id = elementsONTInfo.via_entity_id;
                exportSplitterSplicingInfo.via_entity_port = elementsONTInfo.via_entity_port;
                exportSplitterSplicingInfo.via_port = elementsONTInfo.via_port;
                exportSplitterSplicingInfo.destination_network_id = splicingInfo.destination_network_id;
                exportSplitterSplicingInfo.destination_port_no = splicingInfo.destination_port_no;
                exportSplitterSplicingInfo.destination_port = splicingInfo.destination_port;
                exportSplitterSplicingInfo.destination_type = splicingInfo.destination_entity_type_title;

                exportSplitterSplicingInfo.destination_tube_no = splicingInfo.destination_tube_number;
                exportSplitterSplicingInfo.using_entity = elementsONTInfo.using_entity;
                exportSplitterSplicingInfo.using_entity_id = elementsONTInfo.using_entity_id;
                exportSplitterSplicingInfo.using_entity_input = elementsONTInfo.using_entity_input;
                exportSplitterSplicingInfo.using_input = elementsONTInfo.using_input;
                exportSplitterSplicingInfo.using_entity_output = splicingInfo.core_port_number;
                exportSplitterSplicingInfo.using_output = splicingInfo.source_port;
                lstExportONTSplicingInfo.Add(exportSplitterSplicingInfo);
                if (splicingInfo.destination_entity_type.ToUpper() == EntityType.HTB.ToString().ToUpper())
                {
                    ExportSplicingInfo exportSplitterInfo = new ExportSplicingInfo();
                    exportSplitterInfo.source_network_id = elementsONTInfo.using_entity_id;
                    exportSplitterInfo.source_port_no = splicingInfo.core_port_number;
                    exportSplitterInfo.source_port = splicingInfo.source_port;
                    exportSplitterInfo.source_type = elementsONTInfo.using_entity;


                    exportSplitterInfo.via_entity = elementsONTInfo.via_entity;
                    exportSplitterInfo.via_entity_id = elementsONTInfo.via_entity_id;
                    exportSplitterInfo.via_entity_port = elementsONTInfo.via_entity_port;
                    exportSplitterInfo.via_port = elementsONTInfo.via_port;
                    exportSplitterInfo.destination_network_id = splicingInfo.destination_network_id;
                    exportSplitterInfo.destination_port_no = splicingInfo.destination_port_no;
                    exportSplitterInfo.destination_port = splicingInfo.destination_port;
                    exportSplitterInfo.destination_type = splicingInfo.destination_entity_type_title;

                    exportSplitterInfo.destination_tube_no = splicingInfo.destination_tube_number;
                    exportSplitterInfo.using_entity = splicingInfo.destination_entity_type;
                    exportSplitterInfo.using_entity_id = splicingInfo.destination_network_id;
                    exportSplitterInfo.using_entity_input = splicingInfo.destination_port_no;
                    exportSplitterInfo.using_input = splicingInfo.destination_port;
                    exportSplitterInfo.using_entity_output = splicingInfo.core_port_number;
                    exportSplitterInfo.using_output = splicingInfo.source_port;

                    sourceNetworkId = splicingInfo.destination_network_id;
                    sourceSystemId = splicingInfo.destination_port_no.Value;

                    var _lstONTSplicingInfo = AddONTConnection(splicingInfo.destination_network_id, splicingInfo.destination_system_id.Value, element, exportSplitterInfo);
                    lstExportONTSplicingInfo.AddRange(_lstONTSplicingInfo);
                }


            }
            return lstExportONTSplicingInfo;
        }

        private List<ExportSplicingInfo> GetODFConnectionInfo(List<SplicingConectionInfo> lstSplicingInfo, int sourceSystemId)
        {
            List<ExportSplicingInfo> _lstExportSplicingInfo = new List<ExportSplicingInfo>();
            List<SplicingConectionInfo> lvlItems = lstSplicingInfo.ToList().FindAll(x => x.system_id == sourceSystemId && x.core_port_status == "Connected" && x.input_output == "O").OrderBy(x => x.core_port_number).ToList();
            var isVirtualPortAllowed = new BLLayer().getLayer(EntityType.FMS.ToString()).is_virtual_port_allowed;
            foreach (SplicingConectionInfo splicing in lvlItems)
            {

                ExportSplicingInfo exportSplicingInfo = new ExportSplicingInfo();
                exportSplicingInfo.source_network_id = splicing.display_name;
                exportSplicingInfo.source_port_no = splicing.core_port_number;
                exportSplicingInfo.source_port = splicing.source_port;
                exportSplicingInfo.source_type = splicing.entity_type_title;
                exportSplicingInfo.source_tube_no = splicing.tube_number;
                exportSplicingInfo.via_entity = splicing.via_entity_type_title;
                exportSplicingInfo.via_entity_id = splicing.via_display_name;
                exportSplicingInfo.via_entity_port = isVirtualPortAllowed == false ? null : splicing.via_port_no;
                exportSplicingInfo.via_port = splicing.via_port;

                exportSplicingInfo.destination_tube_no = splicing.destination_tube_number;
                exportSplicingInfo.using_entity = String.Empty;
                exportSplicingInfo.using_entity_id = String.Empty;
                exportSplicingInfo.using_entity_input = null;
                exportSplicingInfo.using_entity_output = null;
                exportSplicingInfo.destination_network_id = splicing.destination_display_name;
                exportSplicingInfo.destination_port_no = splicing.destination_port_no;
                exportSplicingInfo.destination_port = splicing.destination_port;
                exportSplicingInfo.destination_type = splicing.destination_entity_type_title;
                if (splicing.is_through_connection == true)
                    exportSplicingInfo.Connectivity_Type = "Through";
                else
                    exportSplicingInfo.Connectivity_Type = "Splice";
                _lstExportSplicingInfo.Add(exportSplicingInfo);

            }
            return _lstExportSplicingInfo;

        }

        int count;
        public List<ExportSplicingInfo> SetOrder(string _source_network_id, int _source_port_no, IList<SplicingConectionInfo> element)
        {
            if (element == null) return null;
            List<ExportSplicingInfo> lstExportSplicingInfo = new List<ExportSplicingInfo>();
            IList<SplicingConectionInfo> lvlItems = element.ToList().FindAll(x => x.network_id == _source_network_id && x.core_port_number == _source_port_no);
            if (lvlItems.Count == 0) count++;
            foreach (SplicingConectionInfo splicingInfo in lvlItems)
            {

                ExportSplicingInfo exportSplicingInfo = new ExportSplicingInfo();
                string source = "";
                int port = 0;

                exportSplicingInfo.source_network_id = splicingInfo.network_id;
                exportSplicingInfo.source_port_no = splicingInfo.core_port_number;
                exportSplicingInfo.source_type = splicingInfo.entity_type;
                exportSplicingInfo.destination_network_id = splicingInfo.destination_network_id;
                exportSplicingInfo.destination_port_no = splicingInfo.destination_port_no;
                exportSplicingInfo.destination_type = splicingInfo.destination_entity_type;

                if (splicingInfo.destination_entity_type == "Cable")
                {
                    exportSplicingInfo.via_entity = splicingInfo.via_entity_type;
                    exportSplicingInfo.via_entity_id = splicingInfo.via_network_id;
                    exportSplicingInfo.via_entity_port = splicingInfo.via_port_no;
                    source = splicingInfo.destination_network_id;
                    port = splicingInfo.destination_port_no.Value;
                }
                if (splicingInfo.destination_entity_type == "Splitter" && (splicingInfo.input_output == "I" || splicingInfo.input_output == null))
                {
                    exportSplicingInfo.using_entity = splicingInfo.destination_entity_type;
                    exportSplicingInfo.using_entity_id = splicingInfo.destination_network_id;
                    exportSplicingInfo.using_entity_input = splicingInfo.core_port_number;
                    exportSplicingInfo.using_entity_output = splicingInfo.destination_port_no;
                    exportSplicingInfo.via_entity = splicingInfo.via_entity_type;
                    exportSplicingInfo.via_entity_id = splicingInfo.via_network_id;
                    exportSplicingInfo.via_entity_port = splicingInfo.via_port_no;

                    source = splicingInfo.destination_network_id;
                    port = splicingInfo.destination_port_no.Value;
                }


                //splicingInfo.iterated = true;

                //exportSplicingInfo.order = count.ToString();
                lstExportSplicingInfo.Add(exportSplicingInfo);
                //exportSplicingInfo.level = level;
                //if (splicingInfo.iterated == false)
                //{
                SetOrder(source, port, element);
                //}
            }
            return lstExportSplicingInfo;
        }

        public JsonResult SaveCaptureImage(string imgdata)
        {
            string path = string.Empty;
            try
            {
                byte[] uploadedImage = Convert.FromBase64String(imgdata);
                //saving the image on the server
                var reqUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Uploads/temp_LOS/";
                string fileName = "LOS_" + Guid.NewGuid() + ".png";
                path = Server.MapPath(@"~/Uploads/temp_LOS/" + fileName);
                System.IO.File.WriteAllBytes(path, uploadedImage);
            }
            catch (Exception ex)
            {
                throw;
            }

            if (string.IsNullOrEmpty(path))
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = true, file = path }, JsonRequestBehavior.AllowGet);
        }
        private void ExportData(DataTable dtReport, string fileName, Dictionary<string, string> dicImages = null)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null && dtReport.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    if (dicImages != null)
                    {
                        workbook = NPOIExcelHelper.AddImageExcel(workbook, dicImages);

                    }
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        private void ExportDataImg(string fileName, Dictionary<string, string> dicImages = null)
        {
            using (var exportData = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                Response.Clear();
                if (dicImages != null)
                {
                    //ISheet imgsheet = workbook.CreateSheet(fileName);
                    workbook = NPOIExcelHelper.AddImageExcel(workbook, dicImages);
                }
                workbook.Write(exportData);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                Response.BinaryWrite(exportData.ToArray());
                Response.End();
            }
        }

        public ActionResult UploadConnection()
        {
            return PartialView("_UploadConnection");
        }
        [HttpPost]
        public ActionResult UploadConnectionData(string PerformAction)
        {
            string strReturn = "";
            string msg = "";
            List<TempConnectionInfoMaster> ConnectionLogs = new List<TempConnectionInfoMaster>();
            //table for data
            DataTable dtExcelData = new DataTable();
            try
            {
                if (Request != null)
                {
                    int userId = Convert.ToInt32(Session["user_id"]);
                    var objfile = Request.Files[0];
                    var fileName = AppendTimeStamp(Request.Files[0].FileName);
                    var filepath = Path.Combine(Server.MapPath("~\\Content\\UploadedFiles\\Splicing\\"), fileName);
                    objfile.SaveAs(filepath);

                    //read uploaded excel files..
                    DataTable dataTable = NPOIExcelHelper.ExcelToTable(filepath, "");

                    // Remove blank rows...
                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        dtExcelData = dataTable.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();
                    }

                    if (dtExcelData.Rows.Count > 0)
                    {
                        //get maximum building upload count allowed at a time...
                        if (dtExcelData.Rows.Count <= ApplicationSettings.BulkBuildingUploadMaxCount)
                        {
                            string ErrorMsg = "";
                            // get branch column mapping...
                            string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\" + ApplicationSettings.SplicingConnectionTemplateColumns);
                            Dictionary<string, string> dicColumnMapping = GetBulkUploadColumnMapping(strMappingFilePath);

                            // validate uploaded excel column with template mapping...
                            ErrorMsg = validateTemplateColumn(dicColumnMapping, dtExcelData);
                            if (ErrorMsg != "")
                                return Json(new { strReturn = ErrorMsg, msg = "error" }, JsonRequestBehavior.AllowGet);
                            if (ErrorMsg == "")
                            {
                                //ADD COLUMN TO DTEXCEL.. (UPLOADED_BY)
                                DataColumn dcUploadedBy = new DataColumn("UPLOADED_BY", typeof(int));
                                dcUploadedBy.DefaultValue = userId;
                                dtExcelData.Columns.Add(dcUploadedBy);

                                //ADD COLUMN TO DTEXCEL.. (IS_VALID)
                                DataColumn dcIsValid = new DataColumn("IS_VALID", typeof(int));
                                dtExcelData.Columns.Add(dcIsValid);

                                //ADD COLUMN TO DTEXCEL.. (ERROR_MSG)
                                DataColumn dcErrorMsg = new DataColumn("ERROR_MSG", typeof(string));
                                dcErrorMsg.MaxLength = 200;
                                dtExcelData.Columns.Add(dcErrorMsg);

                            }

                            //delete DATA FROM TEMP TABLE ON THE BASIS OF UPLOADED_BY ID
                            new BLOSPSplicing().DeleteTempConnectiongData(userId);


                            List<TempConnectionInfoMaster> lstTempConnections = new List<TempConnectionInfoMaster>();





                            foreach (DataRow dr in dtExcelData.Rows)
                            {


                                TempConnectionInfoMaster objTempConnection = new TempConnectionInfoMaster();

                                string strErrorMsg = ValidateConnectionData(dr, ref objTempConnection, dicColumnMapping, PerformAction);

                                objTempConnection.created_by = Convert.ToString(userId);
                                objTempConnection.created_on = DateTimeHelper.Now;

                                objTempConnection.source_network_id = Convert.ToString(dr[dicColumnMapping["source_network_id"]]);
                                objTempConnection.source_entity_type = Convert.ToString(dr[dicColumnMapping["source_entity_type"]]);
                                objTempConnection.source_port_no = Convert.ToString(dr[dicColumnMapping["source_port_no"]]);
                                objTempConnection.source_port_type = Convert.ToString(dr[dicColumnMapping["source_port_type"]]);

                                objTempConnection.destination_network_id = Convert.ToString(dr[dicColumnMapping["destination_network_id"]]);
                                objTempConnection.destination_entity_type = Convert.ToString(dr[dicColumnMapping["destination_entity_type"]]);
                                objTempConnection.destination_port_no = Convert.ToString(dr[dicColumnMapping["destination_port_no"]]);
                                objTempConnection.destination_port_type = Convert.ToString(dr[dicColumnMapping["destination_port_type"]]);

                                objTempConnection.uploaded_by = userId;
                                lstTempConnections.Add(objTempConnection);

                            }
                            if (lstTempConnections.Count > 0)
                            {

                                dynamic result = "";
                                if (PerformAction == "Insert")
                                {
                                    //SAVE DATA INTO TEMP CONNECTION TABLE
                                    new BLOSPSplicing().BulkUploadTempConnection(lstTempConnections.Where(m => m.is_valid == false).ToList());

                                    var validRecords = lstTempConnections.Where(m => m.is_valid == true).ToList();
                                    if (validRecords.Count() > 0)
                                    {
                                        string connections = JsonConvert.SerializeObject(validRecords);
                                        result = new BLOSPSplicing().uploadBulkConnections(connections, userId);
                                    }
                                    else
                                    {
                                        return Json(new { strReturn = Resources.Resources.SI_GBL_GBL_NET_FRM_049, msg = msg == "" ? "success" : msg }, JsonRequestBehavior.AllowGet);
                                    }

                                }

                                if (!result.status)
                                {
                                    // exit function if failed..//"Error in uploading Buildings! <br> Error:" + result.message
                                    return Json(new { strReturn = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_023, result.message), msg = "error" }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            var getTotalUploadBuildingfailureAndSuccess = new BLOSPSplicing().getTotalUploadConnectionfailureAndSuccess(userId);
                            var GetTotalCountOfSuccesAndFailure = "<table border='1' class='alertgrid'><thead><tr><td><b>Status</b></td><td><b>Count</b></td></tr></thead><tbody><tr><td>Success</td><td>" + getTotalUploadBuildingfailureAndSuccess.Item1 + "</td></tr><tr><td>failure</td><td>" + getTotalUploadBuildingfailureAndSuccess.Item2 + "</td></tr></tbody>";
                            strReturn = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_050, GetTotalCountOfSuccesAndFailure);//"Splicing data processed successfully." + "</br>" + GetTotalCountOfSuccesAndFailure
                        }
                        else
                        {
                            // exit function with max record error...
                            //"Maximum " + ApplicationSettings.BulkBuildingUploadMaxCount + " connections can be uploaded at a time!"
                            return Json(new { strReturn = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_051, ApplicationSettings.BulkBuildingUploadMaxCount), msg = "error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        // exit function with no record...//"No record exists in selected file!"
                        return Json(new { strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_327, msg = "error" }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            catch (NPOI.POIFS.FileSystem.NotOLE2FileException ex)
            {
                msg = "error";
                strReturn = Resources.Resources.SI_OSP_GBL_NET_FRM_328;// "Selected file is either corrupted or invalid excel file!";
                ErrorLogHelper.WriteErrorLog("UploadConnectionData()", "Slicing", ex);

            }
            catch (Exception ex)
            {
                msg = "error";
                strReturn = string.Format(Resources.Resources.SI_GBL_GBL_NET_FRM_052, ex.Message); //"Failed to upload Connections! <br> Error:" + ex.Message;
                ErrorLogHelper.WriteErrorLog("UploadConnectionData()", "Library", ex);
            }
            return Json(new { strReturn = strReturn, msg = msg == "" ? "success" : msg }, JsonRequestBehavior.AllowGet);
        }
        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(
            Path.GetFileNameWithoutExtension(fileName),
            DateTimeHelper.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(fileName)
            );

        }
        public Dictionary<string, string> GetBulkUploadColumnMapping(string filepath)
        {
            Dictionary<string, string> dicMapping = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(filepath);
            return dicMapping = doc.Descendants("Mapping").OrderBy(s => (int)int.Parse(s.Attribute("id").Value))
                .Select(p => new
                {
                    DbColName = p.Element("DbColName").Value,
                    TemplateColName = p.Element("TemplateColName").Value
                })
                .ToDictionary(t => t.DbColName, t => t.TemplateColName);
        }
        public string validateTemplateColumn(Dictionary<string, string> dicColumnMapping, DataTable dt)
        {
            string[] arrColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToArray();
            foreach (var pair in dicColumnMapping)
            {
                // if column not found in template and return error..
                if (!arrColumns.Contains(pair.Value.ToLower()))
                    return string.Format(Resources.Resources.SI_OSP_GBL_NET_FRM_331, pair.Value); //"Selected file does not contain '" + pair.Value + "' column!";
            }
            return "";
        }
        public string ValidateConnectionData(DataRow dr, ref TempConnectionInfoMaster objConnection, Dictionary<string, string> dicColumnMapping, string PerformAction)
        {

            int dPortNo = 0, sPortNo = 0;
            objConnection.is_valid = true;
            Regex nonNumericRegex = new Regex(@"\.");
            try
            {
                if (PerformAction == "Insert")
                {

                    //validate Source entity type
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["source_network_id"]].ToString()))
                    {
                        objConnection.is_valid = false;
                        objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_053;//"Source network id can not be blank!";
                    }

                    //validate source_port_no
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["source_port_no"]].ToString()))
                    {
                        objConnection.is_valid = false;
                        objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_054;// "Destination source port no can not be blank!";
                    }
                    else
                    {
                        if (int.TryParse(dr[dicColumnMapping["source_port_no"]].ToString(), out sPortNo))
                        {

                            if (sPortNo == 0)
                            {
                                objConnection.is_valid = false;
                                objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_055;// "Invalid source port no Value!";
                            }
                        }
                        else
                        {
                            objConnection.is_valid = false;
                            objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_055;// "Invalid source port no Value!";
                        }
                    }

                    //validate destination_entity_type
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["destination_network_id"]].ToString()))
                    {
                        objConnection.is_valid = false;
                        objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_056;// "Destination destination network id can not be blank!";
                    }
                    //validate destination_port_no
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["destination_port_no"]].ToString()))
                    {
                        objConnection.is_valid = false;
                        objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_057;// "Destination system id can not be blank!";
                    }
                    else
                    {
                        if (int.TryParse(dr[dicColumnMapping["destination_port_no"]].ToString(), out dPortNo))
                        {
                            if (dPortNo == 0)
                            {
                                objConnection.is_valid = false;
                                objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_058;// "Invalid destination port no Value!";
                            }
                        }
                        else
                        {
                            objConnection.is_valid = false;
                            objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_058;// "Invalid destination port no Value!";
                        }
                    }

                    //validate Source entity type
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["source_entity_type"]].ToString()))
                    {
                        objConnection.is_valid = false;
                        objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_059;// "Source entity type can not be blank!";
                    }

                    //validate destination_entity_type
                    if (string.IsNullOrEmpty(dr[dicColumnMapping["destination_entity_type"]].ToString()))
                    {
                        objConnection.is_valid = false;
                        objConnection.error_msg = Resources.Resources.SI_GBL_GBL_NET_FRM_060;// "Destination entity type can not be blank!";
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }
        public void DownloadUploadConnectionLogs()
        {
            DataTable dtlogs = new DataTable();
            dtlogs.Columns.Add("source_network_id", typeof(string));
            dtlogs.Columns.Add("source_entity_type", typeof(string));
            dtlogs.Columns.Add("destination_network_id", typeof(string));
            dtlogs.Columns.Add("destination_entity_type", typeof(string));
            dtlogs.Columns.Add("status", typeof(string));
            dtlogs.Columns.Add("error_message", typeof(string));
            dtlogs.TableName = "ConnectionLog";
            int userId = Convert.ToInt32(Session["user_id"]);
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                List<TempConnectionInfoMaster> BulkUploadLogs = new BLOSPSplicing().GetUploadConnectionLogs(userId);

                if (BulkUploadLogs.Count() > 0)
                {
                    foreach (var t in BulkUploadLogs)
                    {
                        dtlogs.Rows.Add(t.source_network_id.ToString(), t.source_entity_type.ToString(), t.destination_network_id.ToString(), t.destination_entity_type.ToString(), t.is_valid == true ? "Success" : "Fail", t.error_msg);
                    }

                    IWorkbook workbook = SmartInventory.Helper.NPOIExcelHelper.DataTableToExcel("xlsx", dtlogs);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", AppendTimeStamp("BulkSplicinglogs.xlsx")));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }
        public FileResult DownloadConnectionTemplate(string FileName)
        {
            var file = "~//Content//Templates//Bulk//" + ApplicationSettings.SplicingConnectionTemplate;
            string contentType = "";
            try
            {
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            catch (Exception ex)
            {
                //DACommon.WriteAdminErrorLogDB("DownloadTemplate", "DownloadTemplate[Maker]", ex);
            }
            return File(file, contentType, FileName + ".xlsx");
        }

        public void exportConnectionPathForMarker(string entity_type, int system_id, int port_no)
        {
            ConnectionInfoFilter objViewFilter = new ConnectionInfoFilter();
            objViewFilter.entityid = system_id;
            objViewFilter.entity_type = entity_type;
            objViewFilter.port_no = port_no;

            List<ConnectionInfo> lstconnectionInfo = new List<ConnectionInfo>();
            List<ConnectionInfoReport> lstconnectionInfoReport = new List<ConnectionInfoReport>();

            objViewFilter.currentPage = 0;
            objViewFilter.pageSize = 0;
            lstconnectionInfo = new BLOSPSplicing().GetConnectionInfo(objViewFilter);
            lstconnectionInfo.ForEach(m =>
            {
                ConnectionInfoReport conInfoReport = new ConnectionInfoReport();
                if (m.is_source_virtual == false)
                {
                    if (m.source_entity_type.ToUpper() != "CABLE" && m.source_entity_type.ToUpper() != "CUSTOMER")
                    {
                        if (m.source_port_no.ToString().Substring(0, 1) == "-")
                        {
                            conInfoReport.source_port_no = m.source_port_no.ToString().Replace("-", "") + " IN";
                        }
                        else
                        {

                            conInfoReport.source_port_no = m.source_port_no + " OUT";

                        }
                    }
                    else
                    {
                        conInfoReport.source_port_no = m.source_port_no.ToString();
                    }
                }
                else
                {
                    conInfoReport.source_port_no = "";
                }
                if (m.is_destination_virtual == false)
                {
                    if (m.destination_entity_type.ToUpper() != "CABLE" && m.destination_entity_type.ToUpper() != "CUSTOMER")
                    {
                        if (m.destination_port_no.ToString().Substring(0, 1) == "-")
                        {
                            conInfoReport.destination_port_no = m.destination_port_no.ToString().Replace("-", "") + " IN";
                        }
                        else
                        {
                            if (m.destination_port_no != 0)
                            {
                                conInfoReport.destination_port_no = m.destination_port_no + " OUT";
                            }
                            else
                            {
                                conInfoReport.destination_port_no = "";
                            }
                        }
                    }
                    else
                    {
                        conInfoReport.destination_port_no = m.destination_port_no.ToString();
                    }
                }
                else
                {
                    conInfoReport.destination_port_no = "";
                }
                conInfoReport.approved_by = m.approved_by;
                conInfoReport.approved_on = m.approved_on;
                conInfoReport.connection_id = m.connection_id;
                conInfoReport.created_on = m.created_on;
                conInfoReport.destination_entity_type = m.destination_entity_type;
                //conInfoReport.destination_network_id = m.destination_network_id;
                conInfoReport.destination_display_name = m.destination_display_name;
                conInfoReport.destination_system_id = m.destination_system_id;
                conInfoReport.id = m.id;
                conInfoReport.is_backward_path = m.is_backward_path;
                conInfoReport.is_customer_connected = m.is_customer_connected;
                conInfoReport.source_entity_type = m.source_entity_type;
                //conInfoReport.source_network_id = m.source_network_id;
                conInfoReport.source_display_name = m.source_display_name;
                conInfoReport.source_system_id = m.source_system_id;
                conInfoReport.totalRecords = m.totalRecords;
                conInfoReport.created_by = m.created_by;

                lstconnectionInfoReport.Add(conInfoReport);
            });
            DataTable dtReport = new DataTable();

            dtReport = MiscHelper.ListToDataTable<ConnectionInfoReport>(lstconnectionInfoReport);

            dtReport.Columns.Add("SPLICED_ON", typeof(System.String));
            dtReport.Columns.Add("IS_DOWN_STREAM", typeof(System.String));
            foreach (DataRow dr in dtReport.Rows)
            {
                dr["SPLICED_ON"] = MiscHelper.FormatDateTime((dr["created_on"].ToString()));//DateTime.Parse((dr["created_on"].ToString())).ToString("dd/MMM/yyyy");
                dr["IS_DOWN_STREAM"] = Convert.ToBoolean(dr["IS_BACKWARD_PATH"]) == false ? "Yes" : "No";
            }
            dtReport.Columns.Remove("IS_BACKWARD_PATH");
            dtReport.Columns.Remove("created_on");
            dtReport.Columns.Remove("connection_id");
            dtReport.Columns.Remove("id");
            dtReport.Columns.Remove("source_system_id");
            dtReport.Columns.Remove("destination_system_id");
            dtReport.Columns.Remove("is_customer_connected");
            dtReport.Columns.Remove("created_by");
            dtReport.Columns.Remove("approved_by");
            dtReport.Columns.Remove("approved_on");
            dtReport.Columns.Remove("childordering");
            dtReport.Columns.Remove("totalrecords");
            dtReport.Columns["source_display_name"].ColumnName = "Source";
            dtReport.Columns["destination_display_name"].ColumnName = "Destination";
            var filename = "ConnectionPathFinder";
            ExportCPFData(dtReport, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
        }
        public JsonResult getCustomer(int systemId, string entityType)
        {
            var customers = new BLCustomer().getCustomers(systemId, entityType);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult GetLinkBudgetDetails(LinkBudgetFilter _objLinkBudgetFilter)
        {
            Session["LinkBudgetFilters"] = _objLinkBudgetFilter;
            var lstViewLinkBudgetDataDetail = new BLOSPSplicing().GetLinkBudgetDetails(_objLinkBudgetFilter);
            return PartialView("_ViewLinkBudgetDetail", lstViewLinkBudgetDataDetail);
        }

        #region Optical Budget Link

        [OutputCache(CacheProfile = "CacheForOneDay")]
        public ActionResult OpticalLinkBudgetSeprate(ViewOSPCPFModel objOSP, int page = 0, string sort = "", string sortdir = "")
        {
            try
            {
                objOSP.objFilterAttributes.pageSize = ApplicationSettings.ConnectionPathFinderGridPaging;
                objOSP.objFilterAttributes.currentPage = page == 0 ? 1 : page;
                objOSP.objFilterAttributes.sort = sort;
                objOSP.objFilterAttributes.orderBy = sortdir;

                if (objOSP.objFilterAttributes.entityid > 0)
                {
                    var lstPort = new BLOSPSplicing().GetEquipmentPort(objOSP.objFilterAttributes.entityid, objOSP.objFilterAttributes.entity_type);
                    BindEquipementCoreDropDown(objOSP);

                    objOSP.lstConnectionInfo = new BLOSPSplicing().GetConnectionInfo(objOSP.objFilterAttributes);
                    //var value = objOSP.lstConnectionInfo.Where(m => m.is_backward_path == false).Select(m => m.destination_network_id);
                    objOSP.objFilterAttributes.totalRecord = objOSP.lstConnectionInfo.Count; //objOSP.lstConnectionInfo != null && objOSP.lstConnectionInfo.Count > 0 ? objOSP.lstConnectionInfo[0].totalRecords : 0;
                    Session["viewConnectionPathFinderFilter"] = objOSP.objFilterAttributes;
                    //objOSP.lstWave_length = new BusinessLogics.Admin.BLOpticalLinkBudget().GetWaveLength();
                    //Session["PORTDATA"] = objOSP.lstConnectionInfo;


                }
                else
                {
                    objOSP.lstConnectionInfo = new List<ConnectionInfo>();
                    objOSP.lstEquipementCore = new List<EquipementPort>();
                    //objOSP.lstWave_length = new List<Models.Admin.WaveLength>();


                    objOSP.objFilterAttributes.totalRecord = 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return PartialView("_OpticalLinkBudget", objOSP);
        }



        #endregion

        public ActionResult EntityLogicalView(int systemId, string entityType, string networkId)
        {
            var userdetails = (User)Session["userDetail"];
            LogicalViewVM logicaldetails = new LogicalViewVM();
            logicaldetails.lstport = new BLOSPSplicing().getEntityLogicalView(systemId, entityType);
            logicaldetails.listPortStatus = new BLPortStatus().getPortStatus();
            logicaldetails.system_id = systemId;
            logicaldetails.entity_type = entityType;
            //logicaldetails.networkId = networkId;
            GeomDetailIn objGeomDetailIn = new GeomDetailIn();
            objGeomDetailIn.systemId = systemId.ToString();
            objGeomDetailIn.entityType = entityType;
            objGeomDetailIn.geomType = "Point";
            var moduleAbbr = "ENT-LGV-LG";
            //  List<ConnectionMaster> con = new BLLayer().GetConnectionString(moduleAbbr);
            ConnectionMaster con = new BLLayer().GetConnectionString(moduleAbbr);
            // foreach (var conn in con)
            // {
            if (con != null)
            {
                objGeomDetailIn.connectionString = con.connection_string;
            }

            var objGeometryDetail = new BLSearch().GetGeometryDetails(objGeomDetailIn);
            logicaldetails.networkId = objGeometryDetail.display_name;
            logicaldetails.lstUserModule = new BLLayer().GetUserModuleAbbrList(userdetails.user_id, UserType.Web.ToString());
            return PartialView("_EntityLogicalView", logicaldetails);
        }
        public void ExportLogicalViewReport(int systemId = 0, string entityType = "")
        {
            try
            {
                DataTable dtReport = new DataTable();
                LogicalViewVM logicaldetails = new LogicalViewVM();
                logicaldetails.lstport = new BLOSPSplicing().getEntityLogicalView(systemId, entityType).OrderBy(m => m.source_network_id).ToList();
                dtReport = MiscHelper.ListToDataTable<LogicalViewPortDetail>(logicaldetails.lstport);

                if (dtReport.Rows.Count > 0)
                {
                    dtReport.TableName = "Logical View Report";

                    dtReport.Columns.Remove("connected_port_number");
                    dtReport.Columns.Remove("source_port_type");
                    dtReport.Columns.Remove("source_port_number");
                    dtReport.Columns.Remove("connected_entity_type");
                    dtReport.Columns.Remove("connected_entity_category");
                    dtReport.Columns.Remove("source_RATIO");
                    //dtReport.Columns.Remove("source_NETWORK_ID");
                    dtReport.Columns.Remove("source_SYSTEM_ID");
                    dtReport.Columns.Remove("source_entity_category");
                    dtReport.Columns.Remove("connected_ratio");
                    dtReport.Columns.Remove("connected_system_id");
                    dtReport.Columns.Remove("CONNECTED_ENTITY_NAME");
                    dtReport.Columns.Remove("connected_network_id");

                    dtReport.Columns["source_network_id"].SetOrdinal(0);
                    dtReport.Columns["source_port_io_text"].SetOrdinal(1);
                    dtReport.Columns["connected_port_status"].SetOrdinal(2);
                    dtReport.Columns["connected_entitytype_text"].SetOrdinal(3);
                    //dtReport.Columns["connected_network_id"].SetOrdinal(3);
                    dtReport.Columns["connected_label"].SetOrdinal(4);
                    dtReport.Columns["connected_port_text"].SetOrdinal(5);
                    dtReport.Columns["connected_on"].SetOrdinal(6);


                    dtReport.Columns["SOURCE_NETWORK_ID"].ColumnName = Resources.Resources.SI_OSP_GBL_GBL_GBL_001;
                    dtReport.Columns["source_port_io_text"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_001;
                    dtReport.Columns["connected_port_status"].ColumnName = Resources.Resources.SI_OSP_GBL_NET_FRM_029;
                    dtReport.Columns["connected_entitytype_text"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_002;
                    //dtReport.Columns["connected_network_id"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_003;
                    dtReport.Columns["connected_label"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_003;
                    dtReport.Columns["connected_port_text"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_RPT_004;
                    dtReport.Columns["connected_on"].ColumnName = Resources.Resources.SI_OSP_ONT_NET_FRM_006;
                    dtReport.Columns["PORT_STATUS_COLOR"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_188;
                    dtReport.Columns["PORT_COMMENT"].ColumnName = Resources.Resources.SI_GBL_GBL_NET_FRM_189;

                    var filename = entityType + " Port Details";
                    ExportData(dtReport, "Export_" + filename + "_" + MiscHelper.getTimeStamp());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult ValidtaeConnections(List<ConnectionInfoMaster> connections)
        {
            var resp = new BLOSPSplicing().ValidtaeConnections(JsonConvert.SerializeObject(connections));
            resp.message = BLConvertMLanguage.MultilingualMessageConvert(resp.message);
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// For CPF to KML File
        /// </summary>
        /// <returns></returns>
        public void DownloadCPFIntoKML()
        {
            JsonResponse<List<CPFElements>> objResp = new JsonResponse<List<CPFElements>>();
            ConnectionInfoFilter objViewFilter = (ConnectionInfoFilter)Session["viewConnectionPathFinderFilter"];
            try
            {
                objResp.result = new BLOSPSplicing().GetCPFElement(objViewFilter);
                DataTable dtReport = new DataTable();
                DataSet ds = new DataSet();
                dtReport = MiscHelper.ListToDataTable<CPFElements>(objResp.result);
                if (dtReport.Rows.Count > 0)
                    ds.Tables.Add(dtReport);
                KMLHelper.GetKmlForSplicedEntities(ds, ApplicationSettings.DownloadTempPath);
            }
            catch (Exception ex)
            {

                ErrorLogHelper.WriteErrorLog("GetCPFelementPath()", "Splicing", ex);
                objResp.status = ResponseStatus.ERROR.ToString();
                objResp.message = "Error while CPF element fetching!";
            }
            // return Json(objResp, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetConnectedCustomerDetails(ViewOSPCPFModel objOSP, int page = 0, string sort = "", string sortdir = "")
        {
            objOSP.objFilterAttributes.pageSize = ApplicationSettings.ConnectionPathFinderGridPaging;
            objOSP.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objOSP.objFilterAttributes.sort = sort;
            objOSP.objFilterAttributes.orderBy = sortdir;
            if (objOSP.objFilterAttributes.entityid > 0)
            {
                objOSP.lstConnectedCustomer = new BLOSPSplicing().GetConnectedCustomerDetails(objOSP.objFilterAttributes);
                // var lstPort = new BLOSPSplicing().GetEquipmentPort(objOSP.objFilterAttributes.entityid, objOSP.objFilterAttributes.entity_type);
                // BindEquipementCoreDropDown(objOSP);
                objOSP.objFilterAttributes.totalRecord = objOSP.lstConnectedCustomer.Count;

                Session["GetConnectedCustomerDetails"] = objOSP.lstConnectedCustomer;
            }

            return PartialView("_ConnectedCustomerDetails", objOSP);
        }

        [HttpGet]
        public void DownloadConnectedCustomerReport()
        {
            if (Session["GetConnectedCustomerDetails"] != null)
            {
                try
                {
                    var customerDetails = Session["GetConnectedCustomerDetails"] as IEnumerable<connectedCusotmer>;

                    DataTable dtTable = new DataTable();
                    dtTable = Utility.MiscHelper.ListToDataTable(customerDetails.ToList());

                    var filename = "ConnectedCustomerDetails";


                    if (dtTable.Rows.Count > 0)
                    {
                        dtTable.Columns.Add("CREATED_ON1", typeof(System.String));
                        if (dtTable.Columns.Contains("CAN_ID")) { dtTable.Columns["CAN_ID"].ColumnName = "Can Id"; }
                        if (dtTable.Columns.Contains("CUSTOMER_NAME")) { dtTable.Columns["CUSTOMER_NAME"].ColumnName = "Customer Name"; }
                        if (dtTable.Columns.Contains("CUSTOMER_TYPE")) { dtTable.Columns["CUSTOMER_TYPE"].ColumnName = "Customer Type"; }
                        if (dtTable.Columns.Contains("LMC_TYPE")) { dtTable.Columns["LMC_TYPE"].ColumnName = "LMC Type"; }
                        if (dtTable.Columns.Contains("ADDRESS")) { dtTable.Columns["ADDRESS"].ColumnName = "Address"; }
                        if (dtTable.Columns.Contains("MOBILE_NO")) { dtTable.Columns["MOBILE_NO"].ColumnName = "Mobile Number"; }
                        if (dtTable.Columns.Contains("PHONE_NO")) { dtTable.Columns["PHONE_NO"].ColumnName = "Phone Number"; }
                        if (dtTable.Columns.Contains("EMAIL_ID")) { dtTable.Columns["EMAIL_ID"].ColumnName = "Email Id"; }
                        if (dtTable.Columns.Contains("REGION_NAME")) { dtTable.Columns["REGION_NAME"].ColumnName = "Region Name"; }
                        if (dtTable.Columns.Contains("PROVINCE_NAME")) { dtTable.Columns["PROVINCE_NAME"].ColumnName = "Province Name"; }
                        if (dtTable.Columns.Contains("CREATED_BY")) { dtTable.Columns["CREATED_BY"].ColumnName = "Created By"; }
                        if (dtTable.Columns.Contains("ENTITY_TYPE")) { dtTable.Columns.Remove("ENTITY_TYPE"); }
                        if (dtTable.Columns.Contains("NETWORK_ID")) { dtTable.Columns.Remove("NETWORK_ID"); }
                        if (dtTable.Columns.Contains("CORE_PORT_NUMBER")) { dtTable.Columns.Remove("CORE_PORT_NUMBER"); }


                        foreach (DataRow dr in dtTable.Rows)
                        {
                            dr["CREATED_ON1"] = MiscHelper.FormatDateTime((dr["CREATED_ON"].ToString()));
                        }
                        if (dtTable.Columns.Contains("CREATED_ON")) { dtTable.Columns.Remove("CREATED_ON"); }
                        if (dtTable.Columns.Contains("CREATED_ON1")) { dtTable.Columns["CREATED_ON1"].ColumnName = "Created On"; }

                        ExportCPFData(dtTable, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        public ActionResult UpdateCorePortStatus(UpdateCorePortStatus obj, int entitySystemId = 0, string corePortNumber = null, string entityType = null, string type = null, bool isGridCalling = false, int PortStatusId = 0, string PortComment = "", string Status = "")
        {
            obj.listPortStatus = new BLPortStatus().getPortStatusFiber();
            obj.pageMsg = new DbMessage();
            obj.pageMsg.status = false; obj.pageMsg.message = "";
            obj.isGridCalling = isGridCalling;
            if (Status != PortStatus.Connected.ToString())
            {
                obj.listPortStatus = obj.listPortStatus.Where(i => i.status != PortStatus.Connected.ToString()).ToList();
            }
            else
            {
                obj.listPortStatus = obj.listPortStatus.Where(i => i.status != PortStatus.Vacant.ToString() && i.status != PortStatus.Reserved.ToString()).ToList();
            }
            if (entitySystemId != 0)
            {
                obj.entity_type = entityType;
                obj.entity_system_id = entitySystemId;
                obj.core_port_number = corePortNumber;
                obj.type = type;
                obj.portStatus = PortStatusId;
                obj.comment = PortComment;
                obj.status = Status;

            }
            return PartialView("_UpdateCorePortStatus", obj);
        }
        public ActionResult ViewPortHistory(int system_id)
        {
            var obj = new BLOSPSplicing().viewPortHistory(system_id);
            return PartialView("_ViewPortHistory", obj);
        }
        public ActionResult SavePortStatus(UpdateCorePortStatus obj)
        {

            obj.listPortStatus = new BLPortStatus().getPortStatusFiber();
            obj.pageMsg = new DbMessage();
            obj.pageMsg.status = false; obj.pageMsg.message = "";
            if (obj.portStatus != 0)
            {
                obj.comment = obj.comment == null ? "" : obj.comment;
                obj.user_id = Convert.ToInt32(Session["user_id"]);
                obj.pageMsg = new BLOSPSplicing().UpdateCorePortStatus(obj);
                obj.pageMsg.message = BLConvertMLanguage.MultilingualMessageConvert(obj.pageMsg.message);
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
            // return PartialView("_UpdateCorePortStatus", obj);
        }
        public ActionResult AddConnection(int systemId)
        {
            AddConnection objAddCon = new AddConnection();
            objAddCon.cardList = BLRack.Instance.getCardList(systemId);
            return PartialView("_AddConnection", objAddCon);
        }
        public JsonResult getPortByEquipment(int systemId)
        {
            var ports = BLRack.Instance.getPortByEquipment(systemId);
            return Json(ports, JsonRequestBehavior.AllowGet);
        }
        #region
        public ActionResult ConnectionEditor()
        {
            PatchingViewModel splicingEntity = new PatchingViewModel();
            splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
            return PartialView("_ConnectionEditor", splicingEntity);
        }


        public ActionResult GetAssociatedlinkInfo(ViewOSPCPFModel objOSP, int page = 0, string sort = "", string sortdir = "")
        {

            if (objOSP.objFilterAttributes.entityid > 0)
            {
                //List<CableFiberDetail> getCableFiberDetail = BLCable.Instance.GetFiberDetailByFiberNumber(objOSP.objFilterAttributes.entityid,1);

                objOSP.lstFiberLinkDetails = new BLFiberLink().getAssociatedLinkInfo(objOSP.objFilterAttributes.entityid, objOSP.objFilterAttributes.port_no);
                if (objOSP.lstFiberLinkDetails != null)
                {

                    if (objOSP.lstFiberLinkDetails.ContainsKey("Handover Date"))
                    {
                        // objOSP.lstFiberLinkDetails.Remove("Handover Date");
                        var keyvaluepair = objOSP.lstFiberLinkDetails.Single(x => x.Key == "Handover Date");
                        if (keyvaluepair.Value != null)
                        {
                            objOSP.lstFiberLinkDetails.Remove("Handover Date");
                            var handoverDateFormated = MiscHelper.FormatDate((keyvaluepair.Value).ToString());

                            objOSP.lstFiberLinkDetails.Add("Handover Date", handoverDateFormated);
                        }

                    }
                    if (objOSP.lstFiberLinkDetails.ContainsKey("HOTO Signoff Date"))
                    {
                        var keyvaluepair = objOSP.lstFiberLinkDetails.Single(x => x.Key == "HOTO Signoff Date");
                        if (keyvaluepair.Value != null)
                        {
                            objOSP.lstFiberLinkDetails.Remove("HOTO Signoff Date");
                            var hotosignoffDateFormated = MiscHelper.FormatDate((keyvaluepair.Value).ToString());
                            objOSP.lstFiberLinkDetails.Add("HOTO Signoff Date", hotosignoffDateFormated);
                        }
                    }

                    if (objOSP.lstFiberLinkDetails.ContainsKey("Any ROW Portion"))
                    {
                        var keyvaluepair = objOSP.lstFiberLinkDetails.Single(x => x.Key == "Any ROW Portion");
                        if (keyvaluepair.Value != null)
                        {
                            objOSP.lstFiberLinkDetails.Remove("Any ROW Portion");
                            var anyrowportionFormat = MiscHelper.FormatBoolean((Convert.ToBoolean(keyvaluepair.Value)));
                            objOSP.lstFiberLinkDetails.Add("Any ROW Portion", anyrowportionFormat);
                        }
                    }

                }

                Session["GetAssociatedLinkInfo"] = objOSP.lstFiberLinkDetails;
            }

            return PartialView("_AssociatedlinkInfo", objOSP);
        }

        public ActionResult ConnectionFilter(int entityId, string entityType)
        {
            ConnectionFilter objConfilter = new ConnectionFilter();
            //objConfilter.listPOD = new BLOSPSplicing().getPODList(entityId);
            objConfilter.listRack = BLRack.Instance.getRackList(entityId, entityType);
            objConfilter.entityId = entityId;
            objConfilter.entity_type = entityType;
            return PartialView("_ConnectionFilter", objConfilter);
        }
        public ActionResult FilterConnection(ConnectionFilter objConfilter)
        {
            PatchingViewModel splicingEntity = new PatchingViewModel();
            if (objConfilter.isInsideConnectivity)
            {
                var details = BLRack.Instance.GetEquipmentById(objConfilter.SourceEquipmentId);
                int portCount = BLRack.Instance.GetPortCountById(objConfilter.SourceEquipmentId);
                objConfilter.SourceEquipmentNWId = details.network_id + "-(" + portCount + ")" + "-(" + details.network_status + ")";
                objConfilter.DestinationEquipmentNWId = details.network_id + "-(" + portCount + ")" + "-(" + details.network_status + ")";
            }
            splicingEntity.splicingConnections = new BLOSPSplicing().getModelSplicingInfo(objConfilter.SourceEquipmentId, EntityType.Equipment.ToString(), objConfilter.DestinationEquipmentId, EntityType.Equipment.ToString(), objConfilter.isInsideConnectivity);
            splicingEntity.userId = Convert.ToInt32(Session["user_id"]);
            splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
            objConfilter.SourceEquipmentList = BLRack.Instance.getCardList(objConfilter.SourceEquipmentId);
            objConfilter.DestinationEquipmentList = BLRack.Instance.getCardList(objConfilter.DestinationEquipmentId);
            splicingEntity.filter = objConfilter;
            return PartialView("_ConnectionEditor", splicingEntity);
        }
        public ActionResult MultipleConnection(viewMultipleConnections objMultiCon)
        {
            var multiConnection = new BLOSPSplicing().getMultipleConnections(objMultiCon.source_system_id, objMultiCon.source_port_no, objMultiCon.portType);
            objMultiCon.connectionsList = multiConnection;
            return PartialView("_MultipleConnection", objMultiCon);
        }
        public JsonResult deleteMultiConnection(int connectionId)
        {

            JsonResponse<string> objResp = new JsonResponse<string>();
            DbMessage response = new BLOSPSplicing().deleteModelConnection(connectionId);
            if (response.status)
            {
                objResp.status = ResponseStatus.OK.ToString();
                objResp.message = response.message;
            }
            else
            {
                objResp.status = ResponseStatus.FAILED.ToString();
                objResp.message = response.message;
            }
            return Json(objResp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ModelConnections(ConnectionFilter objConfilter)
        {
            PatchingViewModel splicingEntity = new PatchingViewModel();
            splicingEntity.splicingConnections = new BLOSPSplicing().getModelSplicingInfo(objConfilter.SourcePortId, EntityType.Equipment.ToString(), objConfilter.DestinationPortId, EntityType.Equipment.ToString(), objConfilter.isInsideConnectivity);
            splicingEntity.userId = Convert.ToInt32(Session["user_id"]);
            splicingEntity.filter = objConfilter;
            return PartialView("_ModelConnections", splicingEntity);
        }
        public ActionResult getOutConnectedPorts(viewMultipleConnections objMultiCon)
        {
            var multiConnection = BLRack.Instance.getOutConnectedPorts(objMultiCon.source_system_id, objMultiCon.source_port_no, objMultiCon.rackId);
            objMultiCon.connectionsList = multiConnection;
            return PartialView("_MultipleConnection", objMultiCon);
        }
        #endregion
        public JsonResult GetTrayUsedPort(int systemId)
        {
            int usedPorts = new BLSpliceTray().GetTrayUsedPort(systemId);
            return Json(usedPorts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSplicingReport(int source_system_id = 0, string source_entity_type = "", int page = 0, string sort = "", string sortdir = "")
        {

            connectionInput objSplicingIn = new connectionInput();
            objSplicingIn.source_system_id = source_system_id;
            objSplicingIn.source_entity_type = source_entity_type;
            objSplicingIn.pageSize = ApplicationSettings.SurveyAssignmentGridPaging;
            objSplicingIn.currentPage = page == 0 ? 1 : page;
            objSplicingIn.sort = sort;
            objSplicingIn.orderBy = sortdir;
            Session["viewSplicingReport"] = objSplicingIn;
            objSplicingIn.listSplicingReport = new BLOSPSplicing().getEntitySplicingReport(objSplicingIn);
            objSplicingIn.totalRecord = objSplicingIn.listSplicingReport.Count > 0 ? Convert.ToInt32(objSplicingIn.listSplicingReport[0].totalRecords) : 0;


            return PartialView("_viewSplicingReport", objSplicingIn);
        }
        public void ExportSplicing()
        {
            if (Session["viewSplicingReport"] != null)
            {
                connectionInput objSplicingIn = (connectionInput)Session["viewSplicingReport"];
                objSplicingIn.pageSize = 0;
                objSplicingIn.currentPage = 0;
                objSplicingIn.listSplicingReport = new BLOSPSplicing().getEntitySplicingReport(objSplicingIn);
                var layerDetail = ApplicationSettings.listLayerDetails.Count > 0 ? ApplicationSettings.listLayerDetails.Where(x => x.layer_name.ToUpper() == objSplicingIn.source_entity_type.ToUpper()).FirstOrDefault() : null;

                DataTable dtTable = Utility.MiscHelper.ListToDataTable(objSplicingIn.listSplicingReport);
                var filename = "EntitySplicingReport";
                dtTable.TableName = layerDetail.layer_title + " Splicing";
                if (dtTable.Rows.Count > 0)
                {
                    dtTable.Columns.Remove("TOTALRECORDS");
                    if (dtTable.Columns.Contains("source")) { dtTable.Columns["source"].ColumnName = "Source"; }
                    if (dtTable.Columns.Contains("source_entity_type")) { dtTable.Columns["source_entity_type"].ColumnName = "Source Entity Type"; }
                    if (dtTable.Columns.Contains("source_tube_no")) { dtTable.Columns["source_tube_no"].ColumnName = "Source Tube No."; }
                    if (dtTable.Columns.Contains("source_port_no")) { dtTable.Columns["source_port_no"].ColumnName = "Source Port No."; }
                    if (dtTable.Columns.Contains("via_entity")) { dtTable.Columns["via_entity"].ColumnName = "Via Entity"; }
                    if (dtTable.Columns.Contains("via_entity_type")) { dtTable.Columns["via_entity_type"].ColumnName = "Via Entity Type"; }
                    if (dtTable.Columns.Contains("via_port_no")) { dtTable.Columns["via_port_no"].ColumnName = "Via Entity Port No."; }
                    if (dtTable.Columns.Contains("tray_name")) { dtTable.Columns["tray_name"].ColumnName = "Via Tray Name"; }
                    if (dtTable.Columns.Contains("using_entity")) { dtTable.Columns["using_entity"].ColumnName = "Using Entity"; }
                    if (dtTable.Columns.Contains("using_entity_type")) { dtTable.Columns["using_entity_type"].ColumnName = "Using Entity Type"; }
                    if (dtTable.Columns.Contains("using_port_in")) { dtTable.Columns["using_port_in"].ColumnName = "Using Entity Port In"; }
                    if (dtTable.Columns.Contains("using_port_out")) { dtTable.Columns["using_port_out"].ColumnName = "Using Entity Port Out"; }
                    if (dtTable.Columns.Contains("destination")) { dtTable.Columns["destination"].ColumnName = "Destination"; }
                    if (dtTable.Columns.Contains("destination_entity_type")) { dtTable.Columns["destination_entity_type"].ColumnName = "Destination Entity Type"; }
                    if (dtTable.Columns.Contains("destination_tube_no")) { dtTable.Columns["destination_tube_no"].ColumnName = "Destination Tube No."; }
                    if (dtTable.Columns.Contains("destination_port_no")) { dtTable.Columns["destination_port_no"].ColumnName = "Destination Port No."; }

                }
                else
                {
                    dtTable.Columns.Add("Source", typeof(System.String));
                    dtTable.Columns.Add("Source Entity Type", typeof(System.String));
                    dtTable.Columns.Add("Source Tube No", typeof(System.String));
                    dtTable.Columns.Add("Source Port No", typeof(System.String));
                    dtTable.Columns.Add("Via Entity", typeof(System.String));
                    dtTable.Columns.Add("Via Entity Type", typeof(System.String));
                    dtTable.Columns.Add("Via Entity Port No", typeof(System.String));
                    dtTable.Columns.Add("Tray Name", typeof(System.String));
                    dtTable.Columns.Add("Using Entity", typeof(System.String));
                    dtTable.Columns.Add("Using Entity Type", typeof(System.String));
                    dtTable.Columns.Add("Using Entity Port In", typeof(System.String));
                    dtTable.Columns.Add("Using Entity Port Out", typeof(System.String));
                    dtTable.Columns.Add("Destination", typeof(System.String));
                    dtTable.Columns.Add("Destination Tube No", typeof(System.String));
                    dtTable.Columns.Add("Destination Port No", typeof(System.String));
                }
                ExportSplicingData(dtTable, "Export_" + filename + "_" + DateTimeHelper.Now.ToString("ddMMyyyy") + "-" + DateTimeHelper.Now.ToString("HHmmss"));

            }
        }
        private void ExportSplicingData(DataTable dtReport, string fileName)
        {
            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                if (dtReport != null)
                {
                    if (string.IsNullOrEmpty(dtReport.TableName))
                        dtReport.TableName = fileName;
                    IWorkbook workbook = NPOIExcelHelper.DataTableToExcel("xlsx", dtReport);
                    workbook.Write(exportData);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName + ".xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                    Response.End();
                }
            }
        }

        //CONNECTED CUSTOMER DETAILS FOR POP,RACK,EQUIPMENT AND CARD
        public ActionResult GetConnectedCustomerDetailsInInfo(ViewOSPCPFModel objOSP, int page = 0, string sort = "", string sortdir = "")
        {
            objOSP.objFilterAttributes.pageSize = ApplicationSettings.ConnectionPathFinderGridPaging;
            objOSP.objFilterAttributes.currentPage = page == 0 ? 1 : page;
            objOSP.objFilterAttributes.sort = sort;
            objOSP.objFilterAttributes.orderBy = sortdir;
            if (objOSP.objFilterAttributes.entityid > 0)
            {
                objOSP.lstConnectedCustomer = new BLOSPSplicing().GetConnectedCustomerDetailsInInfo(objOSP.objFilterAttributes);
                // var lstPort = new BLOSPSplicing().GetEquipmentPort(objOSP.objFilterAttributes.entityid, objOSP.objFilterAttributes.entity_type);
                // BindEquipementCoreDropDown(objOSP);
                objOSP.objFilterAttributes.totalRecord = objOSP.lstConnectedCustomer.Count;

                Session["GetConnectedCustomerDetails"] = objOSP.lstConnectedCustomer;
            }

            return PartialView("_ConnectedCustomerDetails", objOSP);
        }

    }
}





