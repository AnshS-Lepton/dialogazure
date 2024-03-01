using System;
using Models;
using BusinessLogics;
using Utility;
using System.Collections.Generic;
using BusinessLogics.BLTemp;

namespace DataUploader
{
    public class TempToMainTable 
    {
        public delegate void NotificationEventHandlerUploadStatus(dynamic data);
        public event NotificationEventHandlerUploadStatus UploadStatusEventTemp;
        BLDataUploader bLDataUploader;
        public TempToMainTable()
        {
            bLDataUploader = new BLDataUploader();
            bLDataUploader.DataUploaderNotifyEventHandler += NotifyUploadStatus;
        }


        public void NotifyUploadStatus(dynamic data)
        {
            if (UploadStatusEventTemp != null)
                UploadStatusEventTemp.Invoke(data);
        }
        public UploadSummary InsertDataInMainTable(EntityType entityType, UploadSummary summary)
        {
            try
            {
            summary.execution_type = ConstantsKeys.EXECUTION;
                //List<ErrorMessage> lstErrorMessages = new List<ErrorMessage>();
                switch (entityType)
                {

                    case EntityType.Pole:
                        var blTemplate = new BLTempPole();
                        blTemplate.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplate.InsertPoleIntoMainTable(summary);
                        break;
                    case EntityType.POD:
                        var blTempPOD = new BLTempPOD();
                        blTempPOD.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempPOD.InsertPopIntoMainTable(summary);
                        break;
                    case EntityType.Building:
                        var blTemplateBuilding = new BusinessLogics.BLTemp.BLTempBuilding();
                        blTemplateBuilding.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateBuilding.InsertBuildingIntoMainTable(summary);
                        break;
                    case EntityType.Manhole:
                        var blTemplateManhole = new BLTempManhole();
                        blTemplateManhole.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateManhole.InsertManholeIntoMainTable(summary);
                        break;
                    case EntityType.Splitter:
                        var blTemplateSplitter = new BLTempSplitter();
                        blTemplateSplitter.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateSplitter.InsertSplitterIntoMainTable(summary);
                        break;
                    case EntityType.SpliceClosure:
                        var blTemplateSpliceClosure = new BLTempSpliceClosure();
                        blTemplateSpliceClosure.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateSpliceClosure.InsertSpliceClosureIntoMainTable(summary);
                        break;
                    case EntityType.ADB:
                        var blTemplateADB = new BLTempADB();
                        blTemplateADB.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateADB.InsertADBIntoMainTable(summary);
                        break;
                    case EntityType.Tree:
                        var blTemplateTree = new BLTempTree();
                        blTemplateTree.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateTree.InsertTreeIntoMainTable(summary);
                        break;
                    case EntityType.WallMount:
                        var blTemplateWallMount = new BLTempWallMount();
                        blTemplateWallMount.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateWallMount.InsertWallMountIntoMainTable(summary);
                        break;
                    case EntityType.BDB:
                        var blTemplateBDB = new BLTempBDB();
                        blTemplateBDB.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateBDB.InsertBDBIntoMainTable(summary);
                        break;
                    case EntityType.CDB:
                        var blTemplateCDB = new BLTempCDB();
                        blTemplateCDB.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateCDB.InsertCDBIntoMainTable(summary);
                        break;
                    case EntityType.FDB:
                        var blTemplateFDB = new BLTempFDB();
                        blTemplateFDB.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateFDB.InsertFDBIntoMainTable(summary);
                        break;
                    case EntityType.ONT:
                        var blTemplateONT = new BLTempONT();
                        blTemplateONT.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateONT.InsertONTIntoMainTable(summary);
                        break;
                    case EntityType.HTB:
                        var blTemplateHTB = new BLTempHTB();
                        blTemplateHTB.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateHTB.InsertHTBIntoMainTable(summary);
                        break;
                    case EntityType.MPOD:
                        var blTemplateMPOD = new BLTempMPOD();
                        blTemplateMPOD.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateMPOD.InsertMPODIntoMainTable(summary);
                        break;
                    case EntityType.Coupler:
                        var blTemplateCoupler = new BLTempCoupler();
                        blTemplateCoupler.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateCoupler.InsertCouplerIntoMainTable(summary);
                        break;
                    case EntityType.Loop:
                        var blTempLoop = new BLTempLoop();
                        blTempLoop.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        blTempLoop.InsertLoopIntoMainTable(summary);
                        break;
                    case EntityType.Structure:
                        var blTempStructure = new BLTempStructure();
                        blTempStructure.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempStructure.InsertStructureIntoMainTable(summary);
                        break;

                    case EntityType.Trench:
                        var blTempTrench = new BLTempTrench();
                        blTempTrench.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempTrench.InsertTrenchIntoMainTable(summary);
                        break;
                    case EntityType.Duct:
                        var blTempDuct = new BLTempDuct();
                        blTempDuct.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempDuct.InsertDuctIntoMainTable(summary);

                        break;

                    case EntityType.Microduct:
                        var blTempMicroduct = new BLTempMicroduct();
                        blTempMicroduct.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempMicroduct.InsertMicroductIntoMainTable(summary);

                        break;
                    case EntityType.Cable:
                        var blTempCable = new BLTempCable();
                        blTempCable.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempCable.InsertCableIntoMainTable(summary);
                        break;


                    case EntityType.FMS:
                        var blTempFMS = new BLTempFMS();
                        blTempFMS.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempFMS.InsertFMSIntoMainTable(summary);
                        break;
                    case EntityType.Tower:
                        var blTempTower = new BLTempTower();
                        blTempTower.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempTower.InsertTowerIntoMainTable(summary);
                        break;
                    case EntityType.Antenna:
                        var blTempAntenna = new BLTempAntenna();
                        blTempAntenna.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempAntenna.InsertIntoMainTable(summary);
                        break;
                    case EntityType.Sector:
                        var blTempSector = new BLTempSector();
                        blTempSector.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTempSector.InsertSectorIntoMainTable(summary);
                        break;
                    case EntityType.Cabinet:
                        var blTemplateCabinet = new BLTempCabinet();
                        blTemplateCabinet.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateCabinet.InsertCabinetIntoMainTable(summary);
                        break;
                    case EntityType.LandBase:
                        var blLandBase = new BusinessLogics.BLTemp.BLTempLandBase();
                        blLandBase.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blLandBase.InsertLandBaseIntoMainTable(summary);
                        break;
                    case EntityType.Vault:
                        var blTemplateVault = new BLTempVault();
                        blTemplateVault.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateVault.InsertVaultIntoMainTable(summary);
                        break;
                    case EntityType.UNIT:
                        var blTemplateRoom = new BusinessLogics.BLTemp.BLTempRoom();
                        blTemplateRoom.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateRoom.InsertRoomIntoMainTable(summary);
                        break;
                    case EntityType.PatchPanel:
                        var blTemplatePatchPanel = new BLTempPatchPanel();
                        blTemplatePatchPanel.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplatePatchPanel.InsertPatchPanelIntoMainTable(summary);
                        break;
                    case EntityType.Customer:
                        var blTemplateCustomerl = new BLWirelessCustomer();
                        blTemplateCustomerl.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateCustomerl.InsertCustomerIntoMainTable(summary);
                        break;
                    case EntityType.Handhole:
                        var blTemplatehandhole = new BLTempHandhole();
                        blTemplatehandhole.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplatehandhole.InsertHandholeIntoMainTable(summary);
                        break;
                    case EntityType.OpticalRepeater:
                        var blTemplateOpticalrepeater = new BLTempOpticalrepeater();
                        blTemplateOpticalrepeater.DataUploaderNotifyEventHandler += NotifyUploadStatus;
                        summary.lstErrorMessage = blTemplateOpticalrepeater.InsertOpticalRepeaterIntoMainTable(summary);
                        break;
                } 
            summary.success_record = bLDataUploader.GetSuccessCount(entityType, summary);
            summary.failed_record = summary.total_record - summary.success_record;
            summary.end_on = DateTimeHelper.Now;
            if (summary.success_record == summary.total_record)
            {
                summary.status_message = ConstantsKeys.UPLOADED_SUCCESSFULLY;
                summary.status = StatusCodes.OK.ToString();
            }
            if (summary.failed_record == summary.total_record)
            {
                summary.status_message = ConstantsKeys.FAILED;
                summary.status = StatusCodes.FAILED.ToString();
            }
            if ((summary.success_record < summary.total_record) && (summary.failed_record < summary.total_record))
            {
                summary.status_message = ConstantsKeys.UPLOADED_SUCCESSFULLY;
                summary.status = StatusCodes.OK.ToString();
            }
            WriteExceptionLog(summary.lstErrorMessage, summary);
            //if (lstErrorMessages.Count == 0)
               // bLDataUploader.DeleteDataFromTempTable(summary);
            bLDataUploader.UpdateStatus(summary);
            return summary;  
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("TempToMainTable", "InsertDataInMainTable", ex);
                throw;
            }
        } 

        public void WriteExceptionLog(List<ErrorMessage> lstErrorMessages, UploadSummary summary)
        {
            if (lstErrorMessages.Count > 0)
            {
                foreach (ErrorMessage error in lstErrorMessages)
                    ErrorLogHelper.WriteErrorLog(summary.entity_type.ToString(), "InsertDataInMainTable", error.exMessage, summary);
            }
        }

        public ErrorMessage UpdateStatusAndGetError(string controllerName, string actionName, Exception ex, UploadSummary summary)
        {
            try
            {
                ErrorMessage error = new ErrorMessage();
                error.error_code = "500";
                error.error_msg = ex.Message;
                error.is_valid = false;
                error.status = StatusCodes.FAILED.ToString();
                error.uploaded_by = summary.user_id;
                ErrorLogHelper.WriteErrorLog(actionName, controllerName, ex, summary);
                summary.status_message = StatusCodes.FAILED.ToString();
                bLDataUploader.UpdateStatus(summary);
                return error;
            }
            catch (Exception e)
            {
                ErrorLogHelper.WriteErrorLog("TempToMainTable", "UpdateStatusAndGetError", e);
                throw;
            } 
        }

        public UploadSummary Validate(EntityType enumEntityType, UploadSummary summary)
        {
            try
            {
                BLLayer objBLLayer = new BLLayer();
                layerDetail networkLayerDetails = objBLLayer.GetLayerDetails(summary.entity_type);
                summary.execution_type = ConstantsKeys.VALIDATION;
                summary = bLDataUploader.Validate(summary, networkLayerDetails.data_upload_table, networkLayerDetails.geom_type);
                return summary;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.WriteErrorLog("TempToMainTable", "Validate", ex);
                throw;
            } 
        }   
    } 
}
