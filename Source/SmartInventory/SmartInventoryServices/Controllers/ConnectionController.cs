using BusinessLogics;
using Models;
using Models.API;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Utility;
using static SmartInventoryServices.Filters.CustomAuthorization;

namespace SmartInventoryServices.Controllers
{
    [System.Web.Http.RoutePrefix("api/Connection")]
    [CustomAuthorization]
    [APIExceptionFilter]
    [CustomAction]
    public class ConnectionController : ApiController
    {
        [System.Web.Http.HttpPost]
        public ApiResponse<ViewSplicingEntity> Splicing(ReqInput data)
        {
            var response = new ApiResponse<ViewSplicingEntity>();
            try
            {
                ViewSplicingEntity splicingEntities = new ViewSplicingEntity();
                SplicingInputReq obj = ReqHelper.GetRequestData<SplicingInputReq>(data);
                var splicingEntity = new BLOSPSplicing().getEntityForSplicing(obj.latitude, obj.longitude, obj.bufferRadius, obj.role_id);
                splicingEntities.SplicingLst = splicingEntity;
                if (splicingEntity != null)
                {
                    response.results = splicingEntities;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    response.status = ResponseStatus.ZERO_RESULTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("Splicing()", "Connection Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<SplicingViewModel> CableToCable(ReqInput data)
        {
            var response = new ApiResponse<SplicingViewModel>();
            try
            {
                connectionInput obj = ReqHelper.GetRequestData<connectionInput>(data);
                SplicingViewModel splicingEntity = new SplicingViewModel();
                var lstUserModule = new BLLayer().GetUserModuleAbbrList(obj.user_id, UserType.Mobile.ToString());
                splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(obj, JsonConvert.SerializeObject(obj.listConnector));
                var connector = obj.listConnector.FirstOrDefault();
                if (splicingEntity.splicingConnections.Count > 0)
                {
                    splicingEntity.sourceCable = splicingEntity.splicingConnections.Where(m => m.system_id == obj.source_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == obj.is_source_start_point).FirstOrDefault();
                    splicingEntity.destinationCable = splicingEntity.splicingConnections.Where(m => m.system_id == obj.destination_system_id && m.entity_type == EntityType.Cable.ToString() && m.is_cable_a_end == obj.is_destination_start_point).FirstOrDefault();
                    splicingEntity.connector = splicingEntity.splicingConnections.Where(m => m.system_id == Convert.ToInt32(connector.system_id) && m.entity_type == connector.entity_type).FirstOrDefault();
                }
                var isFSALocked = new BLOSPSplicing().CheckSplicingPermission(connector.system_id, connector.entity_type);
                var availablePorts = new BLOSPSplicing().getAvailablePorts(Convert.ToInt32(connector.system_id), connector.entity_type);
                splicingEntity.total_ports = availablePorts.total_ports;
                splicingEntity.available_ports = availablePorts.available_ports;
                splicingEntity.userId = obj.user_id;
                splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
                splicingEntity.isEditAllowed = lstUserModule.Contains("EDS"); ;

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
                splicingEntity.connector_entity_type = obj.connector_entity_type;
                splicingEntity.is_middleware_entity = obj.is_middleware_entity;
                splicingEntity.is_virtual = connector.is_virtual;
                splicingEntity.is_virtual_entity = connector.is_virtual_entity;
               
                if (splicingEntity != null)
                {
                    response.results = splicingEntity;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    response.status = ResponseStatus.ZERO_RESULTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("CableToCable()", "Connection Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<SplicingViewModel> CPEToCustomer(ReqInput data)
        {
            var response = new ApiResponse<SplicingViewModel>();
            try
            {
                connectionInput obj = ReqHelper.GetRequestData<connectionInput>(data);
                SplicingViewModel splicingEntity = new SplicingViewModel();
                List<SplicingConectionInfo> listConnections = new List<SplicingConectionInfo>();
                splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(obj, JsonConvert.SerializeObject(obj.listConnector));
                splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
                splicingEntity.isEditAllowed = obj.lstUserModule.Contains("EDS");
                splicingEntity.connector_entity_type = obj.connector_entity_type;
                splicingEntity.is_middleware_entity = obj.is_middleware_entity;
                if (splicingEntity != null)
                {
                    response.results = splicingEntity;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    response.status = ResponseStatus.ZERO_RESULTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("CPEToCustomer()", "Connection Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<SplicingViewModel> ODFToCable(ReqInput data)
        {
            var response = new ApiResponse<SplicingViewModel>();
            try
            {
                connectionInput obj = ReqHelper.GetRequestData<connectionInput>(data);
                var lstUserModule = new BLLayer().GetUserModuleAbbrList(obj.user_id, UserType.Mobile.ToString());
                SplicingViewModel splicingEntity = new SplicingViewModel();
                List<SplicingConectionInfo> listConnections = new List<SplicingConectionInfo>();
                EntityType enType = (EntityType)System.Enum.Parse(typeof(EntityType), obj.listConnector.First().entity_type);
                splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfo(obj, JsonConvert.SerializeObject(obj.listConnector));
                splicingEntity.listPortStatus = new BLPortStatus().getPortStatus();
                if (splicingEntity.splicingConnections.Count > 0)
                {
                    splicingEntity.destinationCable = splicingEntity.splicingConnections.Where(m => m.system_id == obj.destination_system_id && m.entity_type == EntityType.Cable.ToString()).FirstOrDefault();
                }
                splicingEntity.middlePortList = new BLOSPSplicing().middleWarePorts(obj.listConnector[0].system_id, enType.ToString());
                var isFSALocked = new BLOSPSplicing().CheckSplicingPermission((obj.listConnector.FirstOrDefault()).system_id, (obj.listConnector.FirstOrDefault()).entity_type);

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
                splicingEntity.connector_entity_type = obj.connector_entity_type;
                splicingEntity.is_middleware_entity = obj.is_middleware_entity;
                if (splicingEntity != null)
                {
                    response.results = splicingEntity;
                    response.status = ResponseStatus.OK.ToString();
                }
                else
                {
                    response.status = ResponseStatus.ZERO_RESULTS.ToString();
                    response.error_message = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ODFToCable()", "Connection Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }

            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<DbMessage> ValidateConnections(ReqInput data)
        {          
            var response = new ApiResponse<DbMessage>();
            try
            {
                List<ConnectionInfoMaster> connections = ReqHelper.GetRequestData<List<ConnectionInfoMaster>>(data);
                var resp = new BLOSPSplicing().ValidtaeConnections(JsonConvert.SerializeObject(connections));
                resp.message = BLConvertMLanguage.MultilingualMessageConvert(resp.message);
                if(resp.message != null)
                {
                    response.results = resp;
                    response.status = ResponseStatus.OK.ToString();
                }
            }
            catch(Exception ex) 
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("ValidtaeConnections()", "Connection Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = ex.Message.ToString();
            }
            return response;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<DbMessage> deleteConnection(ReqInput data)
        {
            var objResp = new ApiResponse<DbMessage>();  
            try
            {
                List<deleteConeectionInput> objConnectionInfo = ReqHelper.GetRequestData<List<deleteConeectionInput>>(data);
                var response = new BLOSPSplicing().deleteConnection(JsonConvert.SerializeObject(objConnectionInfo));
                List<UserModule> lstUserModule = new BLMisc().GetUserModuleMasterList();
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
                    objResp.results = response;//response.message;
                }
                else
                {
                    objResp.status = ResponseStatus.FAILED.ToString();
                    objResp.results = response;
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("deleteConnection()", "Connection Controller", data.data, ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.error_message = ex.Message.ToString();
            }
            return objResp;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<DbMessage> SaveConnectionInfo(ReqInput data)
        {
            var objResp = new ApiResponse<DbMessage>();
            try
            {               
                ConnectionInfoMasterInput objConnectionInfo = ReqHelper.GetRequestData<ConnectionInfoMasterInput>(data);
                var userdetatils = objConnectionInfo.user;
                List<UserModule> lstUserModule = new BLMisc().GetUserModuleMasterList();
                objConnectionInfo.connections.ForEach(p => p.created_by = objConnectionInfo.user.user_id);
                objConnectionInfo.connections.ForEach(p => p.created_on = DateTimeHelper.Now);
                var objConnection = new BLOSPSplicing().SaveConnectionInfo(JsonConvert.SerializeObject(objConnectionInfo.connections));
                var objConection = objConnectionInfo.connections.FirstOrDefault();
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
                        SendUtilizationEmail(objConnectionInfo.connections);
                    }
                }
                objResp.results = objConnection;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveConnectionInfo()", "Connection Controller", data.data, ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.error_message = ex.Message.ToString();
            }
            return objResp;
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
                    string filePaths = HttpContext.Current.Server.MapPath("~/uploads/temp/");
                    filePath.Add(EmailAttachmentFilePath(dtUtlizationTable, filePaths + filename));
                    Dictionary<string, string> objDict = new Dictionary<string, string>();
                    objDict.Add("Date", DateTime.Now.ToString("dddd, MMMM dd, yyyy"));
                    objDict.Add("EntityList", objUtilizationText.ToString());
                    List<HttpPostedFileBase> objHttpPostedFileBase = null;
                    BLUser objBLuser = new BLUser();
                    List<EventEmailTemplateDetail> objEventEmailTemplateDetail = objBLuser.GetEventEmailTemplateDetail(EmailEventList.PercentUtilization70.ToString());
                    System.Threading.Tasks.Task.Run(() => commonUtil.SendEventBasedEmail(objEventEmailTemplateDetail, objDict, objHttpPostedFileBase, EmailSettings.AllEmailSettings, filePath, "", EmailEventList.PercentUtilization70.ToString()));
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

        [System.Web.Http.HttpPost]
        public ApiResponse<AvailablePorts> GetAvailabePorts(ReqInput data)
        {
            var objResp = new ApiResponse<AvailablePorts>();
            try
            {
                AvailabePortReq availabePortReq = ReqHelper.GetRequestData<AvailabePortReq>(data);
                var availablePorts = new BLOSPSplicing().getAvailablePorts(availabePortReq.systemId, availabePortReq.entityType);
                if (availablePorts != null)
                {
                    objResp.results = availablePorts;
                    objResp.status = StatusCodes.OK.ToString();
                }
                else
                {
                    objResp.results = availablePorts;
                    objResp.status = StatusCodes.FAILED.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetAvailabePorts()", "Connection Controller", data.data, ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.error_message = ex.Message.ToString();
            }
            return objResp;
        }
        [System.Web.Http.HttpPost]
        public ApiResponse<dynamic> GetTrayUsedPort(ReqInput data)
        {
            var objResp = new ApiResponse<dynamic>();
            try
            {
                AvailabePortReq availabePortReq = ReqHelper.GetRequestData<AvailabePortReq>(data);
                int usedPorts = new BLSpliceTray().GetTrayUsedPort(availabePortReq.systemId);
                // var availablePorts = new BLOSPSplicing().getAvailablePorts(availabePortReq.systemId, availabePortReq.entityType);
                if (usedPorts != 0)
                {
                    objResp.results = usedPorts;
                    objResp.status = StatusCodes.OK.ToString();
                }
                else
                {
                    //objResp.results = availablePorts;
                    objResp.status = StatusCodes.FAILED.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("GetTrayUsedPort()", "Connection Controller", data.data, ex);
                objResp.status = StatusCodes.UNKNOWN_ERROR.ToString();
                objResp.error_message = ex.Message.ToString();
            }
            return objResp;
        }

        [System.Web.Http.HttpPost]
        public ApiResponse<DbMessage> SaveBulkConnection(ReqInput data)
        {
            var response = new ApiResponse<DbMessage>();
            try
            {
                List<bulkSplicingInput> objInput = ReqHelper.GetRequestData<List<bulkSplicingInput>>(data);
                List<ConnectionInfoMaster> lstConnection = new List<ConnectionInfoMaster>();
                List<ConnectionInfoMaster> lstValidateConnection = new List<ConnectionInfoMaster>();
                bulkSplicingInput objSource = objInput.Where(m => m.connectionType == "Source").FirstOrDefault();
                bulkSplicingInput objDestination = objInput.Where(m => m.connectionType == "Destination").FirstOrDefault();
                bulkSplicingInput objConnector = objInput.Where(m => m.connectionType == "Connector").FirstOrDefault();
                    for (int i = objSource.from; i <= objSource.to; i++)
                    {
                        ConnectionInfoMaster objConnection = new ConnectionInfoMaster();
                        objConnection.source_system_id = objSource.systemId;
                        objConnection.source_entity_type = objSource.entityType;
                        objConnection.source_network_id = objSource.networkId;
                        objConnection.source_port_no = i;
                        objConnection.is_source_cable_a_end = objSource.isCableAend;

                        objConnection.destination_system_id = objDestination.systemId;
                        objConnection.destination_entity_type = objDestination.entityType;
                        objConnection.destination_network_id = objDestination.networkId;
                        objConnection.destination_port_no = objDestination.from;
                        objConnection.is_destination_cable_a_end = objDestination.isCableAend;

                    if (objConnector != null)
                    {
                        objConnection.equipment_system_id = objConnector.systemId;
                        objConnection.equipment_network_id = objConnector.networkId;
                        objConnection.equipment_entity_type = objConnector.entityType;
                    }
                    objConnection.splicing_source = "Mobile Splicing";

                        objConnection.is_through_connection = false;


                        lstValidateConnection.Add(objConnection);
                        var resp = new BLOSPSplicing().ValidtaeConnections(JsonConvert.SerializeObject(lstValidateConnection));
                        if (resp.status) { lstConnection.Add(objConnection); }

                        if (objDestination.from == objDestination.to) { break; }
                        objDestination.from++;
                    }
                SaveConnectionInfo(lstConnection);

                response.status = StatusCodes.OK.ToString();
                response.error_message = "";
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("SaveBulkConnection()", "Splicing Controller", data.data, ex);
                response.status = StatusCodes.UNKNOWN_ERROR.ToString();
                response.error_message = "Error While Processing  Request.";
            }
            return response;
        }
        public void SaveConnectionInfo(List<ConnectionInfoMaster> objConnectionInfo)
        {
            var access_token = Request.Headers.Authorization.Parameter;
            var secureDataFormat = new Microsoft.Owin.Security.DataHandler.TicketDataFormat(new MachineKeyProtector());
            Microsoft.Owin.Security.AuthenticationTicket ticket = secureDataFormat.Unprotect(access_token);
            var user_id = Convert.ToInt32(ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userId").Value);
            var role_id = Convert.ToInt32(ticket.Properties.Dictionary.FirstOrDefault(x => x.Key == "userRoleId").Value);
            List<UserModule> lstUserModule = new BLMisc().GetUserModuleMasterList();
            objConnectionInfo.ForEach(p => p.created_by = user_id);
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
                        var UnreadNotificationCount = new BLMisc().GetUnreadNotificationCount(user_id, role_id);
                        NotificationOutPut objNotification = new NotificationOutPut();
                        objNotification.info = Convert.ToString(UnreadNotificationCount);
                        objNotification.sendToAllUser = false;
                        objNotification.notificationType = notificationType.Utilization.ToString();
                        smartInventoryhub.BroadCastInfo(objNotification);
                    }).ContinueWith(tsk =>
                    {
                        tsk.Exception.Handle(ex => { ErrorLogHelper.WriteErrorLog("Splicing", "SaveConnectionInfo", ex); return true; });
                    }, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
                    //SendUtilizationEmail(objConnectionInfo);
                }
            }
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage GetSplicingReports(ReqInput data)
        {
            HttpResponseMessage result = new HttpResponseMessage();


            splicingReport obj = ReqHelper.GetRequestData<splicingReport>(data);
            if (ModelState.IsValid)
            {
                try
                {

                    connectionInput objSplicingIn = new connectionInput();
                    objSplicingIn.source_system_id = obj.source_system_id;
                    objSplicingIn.source_entity_type = obj.source_type;
                    objSplicingIn.is_source_start_point = obj.is_source_connected;
                    objSplicingIn.listConnector = JsonConvert.DeserializeObject<List<connectors>>(obj.connecting_entity);
                    string connector = objSplicingIn.listConnector.First().entity_type.ToUpper();
                    objSplicingIn.destination_system_id = obj.destination_system_id;
                    objSplicingIn.destination_entity_type = obj.destination_type;
                    objSplicingIn.is_destination_start_point = obj.is_destination_connected;
                    objSplicingIn.customer_ids = obj.customer_Ids;

                    SplicingViewModel splicingEntity = new SplicingViewModel();
                    splicingEntity.splicingConnections = new BLOSPSplicing().getSplicingInfoReport(objSplicingIn, obj.connecting_entity);
                    List<SplicingConectionInfo> _listConnections = splicingEntity.splicingConnections.Where(m => m.destination_system_id > 0).ToList();
                    List<ExportSplicingInfo> lstSplicingInfo = new List<ExportSplicingInfo>();
                    if (obj.exportKey == "CPE2C")
                    {

                        lstSplicingInfo = GetConnectionInfo(_listConnections, objSplicingIn.listConnector.Where(x => x.entity_type == objSplicingIn.listConnector[0].entity_type).FirstOrDefault().system_id, objSplicingIn.listConnector.Where(x => x.entity_type == objSplicingIn.listConnector[0].entity_type).FirstOrDefault().entity_type);
                    }
                    else if (obj.exportKey == "FMS2C")
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
                        var filename = "Splicing Report_" + "status" + "_" + DateTime.Now.ToString("ddMMyyyy") + "_" + DateTime.Now.ToString("HHmmss") + ".xlsx";
                        byte[] fileBytes = DataTableToExcelBytes(dtlogs);
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(fileBytes);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = filename
                        };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    }
                    else
                    {
                        result = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    }


                }
                catch (Exception ex)
                {
                    ErrorLogHelper logHelper = new ErrorLogHelper();
                    logHelper.ApiLogWriter("DownloadExcel()", "Main Controller", data.data, ex);

                }
            }
            return result;
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

        public static void CreateCustomCell(IRow row, int colIndex, string colText, ICellStyle cellStyle, bool isHeader = false)
        {
            ICell cell = row.CreateCell(colIndex);
            cell.SetCellValue(colText);
            cell.CellStyle = cellStyle;

        }
        public static ICellStyle getCellStyle(IWorkbook workbook, string styleType = "")
        {
            ICellStyle cellStyle = (ICellStyle)workbook.CreateCellStyle();

            // create font style
            IFont myFont = (IFont)workbook.CreateFont();
            myFont.FontHeightInPoints = (short)10;
            myFont.FontName = "ARIAL";

            cellStyle.SetFont(myFont);
            cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            cellStyle.Alignment = HorizontalAlignment.Left;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            if (styleType.ToUpper() == "HEADER" || styleType.ToUpper() == "FOOTER")
            {

                cellStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;

            }
            else if (styleType.ToUpper() == "SUB_HEADER")
            {
                cellStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                cellStyle.FillPattern = FillPattern.SolidForeground;
            }
            return cellStyle;
        }
        private byte[] DataTableToExcelBytes(DataTable dt)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Splicing Report");
            ICellStyle headerStyle = getCellStyle(workbook, "HEADER");
            IRow row1 = sheet.CreateRow(0);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                CreateCustomCell(row1, j, dt.Columns[j].ToString(), headerStyle);
            }

            var cellstyle = getCellStyle(workbook);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    CreateCustomCell(row, j, dt.Rows[i][j].ToString(), cellstyle);
                }
            }
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                return ms.ToArray();
            }
        }
    }
}
