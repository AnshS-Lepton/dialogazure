using BusinessLogics;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Data;
using Utility;

namespace DataUploader
{
    public class UploadTempBTSLayer : UploadExcel
    {
        private readonly ITempToMainTable tempToMainTable;
        private readonly ISpecification itemSpecification;
        //private readonly IUploadStatus iuploadStatus;
        public UploadTempBTSLayer()
        {
            //iuploadStatus = UploadStatus.Instance;
            itemSpecification = new ItemSpecification();
            //tempToMainTable = TempToMainTable.Instance;
        }

        public override ErrorMessage UploadData(DataTable dataTable, UploadSummary summary)
        {
            try
            {
                ErrorMessage status = new ErrorMessage();
                dataTable = CommonUtility.CheckDataTableForBlankRecords(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    //get maximum building upload count allowed at a time...
                    if (dataTable.Rows.Count <= 50000)
                    {

                        string strMappingFilePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\Templates\\Bulk\\TowerTemplate.xml");
                        Dictionary<string, string> dicColumnMapping = CommonUtility.GetBulkUploadColumnMapping(strMappingFilePath);
                        // validate uploaded excel column with template mapping...
                        status = CommonUtility.validateTemplateColumn(dicColumnMapping, dataTable);
                        if (string.IsNullOrWhiteSpace(status.error_msg))
                        {
                            CommonUtility.AddErrorFields(dataTable, summary);
                        }
                        else
                        {
                            //UploadStatus uploadStatus = UploadStatus.Instance;
                            new BLDataUploader().UpdateFailedStatus(summary, status);
                            return status;
                        }
                        List<KeyValueDropDown> keyValueDropDown = itemSpecification.GetSpecification();
                        List<TempBtsLayer> lstTempBTSLayer = new List<TempBtsLayer>();
                        foreach (DataRow dr in dataTable.Rows)
                        {

                            #region ForLoop
                            TempBtsLayer objTempBTSLayer = new TempBtsLayer();
                            objTempBTSLayer.amsl = dr[dicColumnMapping["amsl"]].ToString();
                            objTempBTSLayer.antenna_1_height = dr[dicColumnMapping["antenna_1_height"]].ToString();
                            objTempBTSLayer.antenna_2_height = dr[dicColumnMapping["antenna_2_height"]].ToString();
                            objTempBTSLayer.ap_count = commonUtil.ToInt(dr[dicColumnMapping["ap_count"]].ToString());
                            objTempBTSLayer.colo_provider = dr[dicColumnMapping["colo_provider"]].ToString();
                            objTempBTSLayer.bstn_router_ip = dr[dicColumnMapping["bstn_router_ip"]].ToString();
                            objTempBTSLayer.bstn_sw_ip = dr[dicColumnMapping["bstn_sw_ip"]].ToString();
                            objTempBTSLayer.bst_capacity = commonUtil.ToDouble(dr[dicColumnMapping["bst_capacity"]].ToString());
                            objTempBTSLayer.bst_utilization = commonUtil.ToDouble(dr[dicColumnMapping["bst_utilization"]].ToString());
                            objTempBTSLayer.bts_add = dr[dicColumnMapping["bts_add"]].ToString();
                            objTempBTSLayer.bts_id = dr[dicColumnMapping["bts_id"]].ToString();
                            objTempBTSLayer.bts_name = dr[dicColumnMapping["bts_name"]].ToString();
                            objTempBTSLayer.bts_node_b_id = dr[dicColumnMapping["bts_node_b_id"]].ToString();
                            objTempBTSLayer.bts_node_b_ip_address = dr[dicColumnMapping["bts_node_b_ip_address"]].ToString();

                            objTempBTSLayer.bts_tower_type = dr[dicColumnMapping["bts_tower_type"]].ToString();
                            objTempBTSLayer.building_height = commonUtil.ToDouble(dr[dicColumnMapping["building_height"]].ToString());
                            objTempBTSLayer.category = dr[dicColumnMapping["category"]].ToString();

                            objTempBTSLayer.conn_to_noc = dr[dicColumnMapping["conn_to_noc"]].ToString();
                            objTempBTSLayer.contact_no = dr[dicColumnMapping["contact_no"]].ToString();

                            //objTempBTSLayer.accessibility = ItemSpecification.GetIdByColumnName("accessibility", keyValueDropDown, dr[dicColumnMapping["accessibility"]].ToString());
                            //objTempBTSLayer.construction = ItemSpecification.GetIdByColumnName("construction", keyValueDropDown, dr[dicColumnMapping["construction"]].ToString());
                            //objTempBTSLayer.activation = ItemSpecification.GetIdByColumnName("activation", keyValueDropDown, dr[dicColumnMapping["activation"]].ToString());
                            //objTempBTSLayer.network_status = dr[dicColumnMapping["network_status"]].ToString();

                            objTempBTSLayer.created_on = DateTimeHelper.Now;
                            objTempBTSLayer.du_history_id = summary.id;
                            objTempBTSLayer.latitude = commonUtil.ToDouble(dr[dicColumnMapping["latitude"]].ToString());


                            objTempBTSLayer.layer_type = summary.entity_type == "WirelessSify" ? "Sify" : "Others";
                            objTempBTSLayer.longitude = commonUtil.ToDouble(dr[dicColumnMapping["longitude"]].ToString());
                            objTempBTSLayer.owner_address = dr[dicColumnMapping["owner_address"]].ToString();
                            objTempBTSLayer.owner_name = dr[dicColumnMapping["owner_name"]].ToString();

                            objTempBTSLayer.tower_type = dr[dicColumnMapping["bts_tower_type"]].ToString();
                            objTempBTSLayer.twrht = commonUtil.ToDouble(dr[dicColumnMapping["twrht"]].ToString());

                            objTempBTSLayer.type_of_site = dr[dicColumnMapping["type_of_site"]].ToString();
                            objTempBTSLayer.wsg_group = dr[dicColumnMapping["wsg_group"]].ToString();

                            objTempBTSLayer.bstn_provision_bandwidth = dr[dicColumnMapping["bstn_provision_bandwidth"]].ToString();


                            lstTempBTSLayer.Add(objTempBTSLayer);
                            #endregion
                        }

                        BLTempBTSLayer blTempTower = new BLTempBTSLayer();
                        blTempTower.Save(lstTempBTSLayer);
                        status.status = StatusCodes.OK.ToString();
                        //blTempFMS.InsertFMSIntoMainTable(summary);
                        //objTempPod.SaveTempPOD(lstTempPOD);
                        //blTempFMS.InsertPopIntoMainTable(summary);


                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                return tempToMainTable.UpdateStatusAndGetError(ex, summary);
            }
        }
    }
}
