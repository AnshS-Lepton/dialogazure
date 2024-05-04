using BusinessLogics;
using Models;
using Models.API;
using Newtonsoft.Json;
using SmartInventory.Settings;
using SmartInventoryServices.Filters;
using SmartInventoryServices.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using Utility;

namespace SmartInventoryServices.Controllers
{
    [RoutePrefix("api/Connection")]
    [CustomAuthorization]
    [APIExceptionFilter]
    [CustomAction]
    public class ConnectionController : ApiController
    {
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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

        [HttpPost]
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
        [HttpPost]
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
    }
}
