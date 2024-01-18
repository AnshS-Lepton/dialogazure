using BusinessLogics;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Data;
using Utility;
namespace DataUploader
{
    public class UploadTempPOI : UploadExcel
    {
        private readonly ITempToMainTable tempToMainTable;
        //private readonly ISpecification itemSpecification;
        //private readonly IUploadStatus iuploadStatus;
        BLDataUploader bLDataUploader;
        public UploadTempPOI()
        {
            bLDataUploader = new BLDataUploader();
            bLDataUploader.DataUploaderNotifyEventHandler += NotifyUpdateStatus;
            
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
                        string strMappingFilePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\Templates\\Bulk\\POITemplate.xml");
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
                            bLDataUploader.UpdateFailedStatus(summary, status);
                            return status;
                        }
                        //List<KeyValueDropDown> keyValueDropDown = itemSpecification.GetSpecification();
                        List<TempPOI> lstTempFMS = new List<TempPOI>();
                        int tempNetworkId = 0;
                        foreach (DataRow dr in dataTable.Rows)
                        {

                            #region ForLoop
                            TempPOI objTempPOI = new TempPOI();

                            //string strErrorMsg = ValidatePOPData(dr, ref objTempPop, dicColumnMapping);
                            //objTempPOI.accessibility = ItemSpecification.GetIdByColumnName("accessibility", keyValueDropDown, dr[dicColumnMapping["accessibility"]].ToString());
                            //objTempPOI.activation = ItemSpecification.GetIdByColumnName("activation", keyValueDropDown, dr[dicColumnMapping["activation"]].ToString());
                            objTempPOI.bldg_height = commonUtil.ToDouble(dr[dicColumnMapping["bldg_height"]].ToString());
                            objTempPOI.bldg_type = dr[dicColumnMapping["bldg_type"]].ToString();
                            objTempPOI.city_name = dr[dicColumnMapping["city_name"]].ToString();
                            //objTempPOI.construction = ItemSpecification.GetIdByColumnName("construction", keyValueDropDown, dr[dicColumnMapping["construction"]].ToString());
                            objTempPOI.created_on = DateTimeHelper.Now;
                            objTempPOI.data_source = dr[dicColumnMapping["data_source"]].ToString();
                            //objTempPOI.fiber_length = dr[dicColumnMapping["fiber_length"]].ToString();

                            objTempPOI.latitude = commonUtil.ToDouble(dr[dicColumnMapping["latitude"]].ToString());
                            objTempPOI.longitude = commonUtil.ToDouble(dr[dicColumnMapping["longitude"]].ToString());
                            //objTempPOI.network_status = dr[dicColumnMapping["network_status"]].ToString();
                            objTempPOI.poi_id = dr[dicColumnMapping["poi_id"]].ToString();
                            objTempPOI.province_name = dr[dicColumnMapping["province_name"]].ToString();
                            objTempPOI.region_name = dr[dicColumnMapping["region_name"]].ToString();
                            objTempPOI.city_name = dr[dicColumnMapping["city_name"]].ToString();
                            objTempPOI.site_address = dr[dicColumnMapping["site_address"]].ToString();
                            objTempPOI.site_branch_building_name = dr[dicColumnMapping["site_branch_building_name"]].ToString();
                            objTempPOI.total_height = commonUtil.ToDouble(dr[dicColumnMapping["total_height"]].ToString());
                            objTempPOI.tower_height = commonUtil.ToDouble(dr[dicColumnMapping["tower_height"]].ToString());
                            objTempPOI.upload_id = summary.id;
                            objTempPOI.du_history_id = summary.id;
                            tempNetworkId++;
                            lstTempFMS.Add(objTempPOI);
                            #endregion
                        }

                        BLTempPOI blTempPOI = new BLTempPOI();
                        blTempPOI.Save(lstTempFMS);
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
