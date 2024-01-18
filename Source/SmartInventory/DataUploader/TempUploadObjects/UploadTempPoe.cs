using BusinessLogics;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Data;
using Utility;
namespace DataUploader
{
    public class UploadTempPoe : UploadExcel
    {
        public UploadTempPoe()
        {
            //iuploadStatus = UploadStatus.Instance;
        }
        public override ErrorMessage UploadData(DataTable dataTable, UploadSummary summary)
        {
            ErrorMessage status = new ErrorMessage();
            dataTable = CommonUtility.CheckDataTableForBlankRecords(dataTable);
            try
            {
                if (dataTable.Rows.Count > 0)
                {
                    //get maximum building upload count allowed at a time...
                    if (dataTable.Rows.Count <= 50000)
                    {

                        string strMappingFilePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\Templates\\Bulk\\PoeTemplate.xml");
                        Dictionary<string, string> dicColumnMapping = CommonUtility.GetBulkUploadColumnMapping(strMappingFilePath);


                        string ErrorMsg = string.Empty;


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
                        //List<Customer> lstCustomer = new List<Customer>();
                        List<TempPoe> lstTempPoe = new List<TempPoe>();
                        int tempNetworkId = 0;
                        foreach (DataRow dr in dataTable.Rows)
                        {

                            #region ForLoop
                            TempPoe objTempPoe = new TempPoe();

                            //string strErrorMsg = ValidatePOPData(dr, ref objTempPop, dicColumnMapping);

                            objTempPoe.network_id = tempNetworkId.ToString();
                            //objTempPop.created_by = summary.user_id;
                            objTempPoe.poe_name = dr[dicColumnMapping["poe_name"]].ToString();
                            objTempPoe.latitude = commonUtil.ToDouble(dr[dicColumnMapping["latitude"]].ToString());
                            objTempPoe.longitude = commonUtil.ToDouble(dr[dicColumnMapping["longitude"]].ToString());
                            objTempPoe.poe_chamber_type = dr[dicColumnMapping["poe_chamber_type"]].ToString();

                            objTempPoe.type_of_road = dr[dicColumnMapping["type_of_road"]].ToString();
                            objTempPoe.road_name = dr[dicColumnMapping["road_name"]].ToString();
                            objTempPoe.land_mark = dr[dicColumnMapping["land_mark"]].ToString();

                            objTempPoe.referance_point_1 = dr[dicColumnMapping["referance_point_1"]].ToString();
                            objTempPoe.latitude_1 = commonUtil.ToDouble(dr[dicColumnMapping["latitude_1"]].ToString());
                            objTempPoe.longitude_1 = commonUtil.ToDouble(dr[dicColumnMapping["longitude_1"]].ToString()); ;
                            objTempPoe.distance_in_mtrs_1 = commonUtil.ToDouble(dr[dicColumnMapping["distance_in_mtrs_1"]].ToString());

                            objTempPoe.referance_point_2 = dr[dicColumnMapping["referance_point_2"]].ToString();
                            objTempPoe.latitude_2 = commonUtil.ToDouble(dr[dicColumnMapping["latitude_2"]].ToString());
                            objTempPoe.longitude_2 = commonUtil.ToDouble(dr[dicColumnMapping["longitude_2"]].ToString()); ;
                            objTempPoe.distance_in_mtrs_2 = commonUtil.ToDouble(dr[dicColumnMapping["distance_in_mtrs_2"]].ToString());

                            objTempPoe.referance_point_3 = dr[dicColumnMapping["referance_point_3"]].ToString();
                            objTempPoe.latitude_3 = commonUtil.ToDouble(dr[dicColumnMapping["latitude_3"]].ToString());
                            objTempPoe.longitude_3 = commonUtil.ToDouble(dr[dicColumnMapping["longitude_3"]].ToString()); ;
                            objTempPoe.distance_in_mtrs_3 = commonUtil.ToDouble(dr[dicColumnMapping["distance_in_mtrs_3"]].ToString());

                            objTempPoe.remark_1 = dr[dicColumnMapping["remark_1"]].ToString();
                            objTempPoe.remark_2 = dr[dicColumnMapping["remark_2"]].ToString();
                            objTempPoe.remark_3 = dr[dicColumnMapping["remark_3"]].ToString();
                            objTempPoe.network_status = dr[dicColumnMapping["network_status"]].ToString();
                            objTempPoe.parent_network_id = dr[dicColumnMapping["parent_network_id"]].ToString();
                            objTempPoe.parent_entity_type = dr[dicColumnMapping["parent_entity_type"]].ToString();
                            objTempPoe.upload_id = summary.id;
                            objTempPoe.created_by = summary.user_id;

                            tempNetworkId++;
                            lstTempPoe.Add(objTempPoe);
                            #endregion
                        }

                        BLTempPoe blTempPoe = new BLTempPoe();
                        blTempPoe.Save(lstTempPoe);
                        //blTempPoe.InsertPOEIntoMainTable(summary);

                        status.status = StatusCodes.OK.ToString();
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                return null;
                //return tempToMainTable.UpdateStatusAndGetError("UploadTempPoe", "Upload Data", ex, summary);
            }
        }
    }
}
