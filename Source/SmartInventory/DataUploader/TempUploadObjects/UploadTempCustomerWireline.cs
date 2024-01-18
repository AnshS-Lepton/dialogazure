using System;
using System.Collections.Generic;
using System.Data;
using Models;
using BusinessLogics;
using Models.TempUpload;
using Utility;
namespace DataUploader
{

    internal class UploadTempCustomerWireline : UploadExcel
    {
        private readonly ITempToMainTable tempToMainTable;
        //private readonly IUploadStatus iuploadStatus;
        public UploadTempCustomerWireline()
        {
            //iuploadStatus = UploadStatus.Instance;
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
                        //string strMappingFilePath = Server.MapPath("~\\Content\\Templates\\Bulk\\CustomerTemplate.xml");
                        string strMappingFilePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\Templates\\Bulk\\WirelineCustomerTemplate.xml");
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
                        //List<Customer> lstCustomer = new List<Customer>();
                        List<TempWirelineCustomer> lstTempCustomer = new List<TempWirelineCustomer>();
                        int tempEloc = 1;
                        foreach (DataRow dr in dataTable.Rows)
                        {


                            TempWirelineCustomer objCustomer = new TempWirelineCustomer();

                            //string strErrorMsg = ValidateCustomerData(dr, ref objCustomer, dicColumnMapping);
                            objCustomer.network_id = tempEloc.ToString();
                            objCustomer.customer_code = dr[dicColumnMapping["customer_code"]].ToString();
                            objCustomer.customer_name = dr[dicColumnMapping["customer_name"]].ToString();
                            objCustomer.site_id = dr[dicColumnMapping["site_id"]].ToString();
                            objCustomer.site_name = dr[dicColumnMapping["site_name"]].ToString();
                            objCustomer.site_address = dr[dicColumnMapping["site_address"]].ToString();
                            objCustomer.latitude = commonUtil.ToDouble(dr[dicColumnMapping["latitude"]].ToString());
                            objCustomer.longitude = commonUtil.ToDouble(dr[dicColumnMapping["longitude"]].ToString());
                            objCustomer.lms_id = dr[dicColumnMapping["lms_id"]].ToString();
                            objCustomer.customer_link_id = dr[dicColumnMapping["customer_link_id"]].ToString();
                            objCustomer.customer_po_id1 = dr[dicColumnMapping["customer_po_id1"]].ToString();
                            objCustomer.customer_po_id2 = dr[dicColumnMapping["customer_po_id2"]].ToString();
                            objCustomer.owner = dr[dicColumnMapping["owner"]].ToString();
                            objCustomer.building_type1 = dr[dicColumnMapping["building_type1"]].ToString();
                            objCustomer.building_type2 = dr[dicColumnMapping["building_type2"]].ToString();
                            objCustomer.building_height = commonUtil.ToDouble(dr[dicColumnMapping["building_height"]].ToString());
                            objCustomer.building_owner_type = dr[dicColumnMapping["building_owner_type"]].ToString();
                            objCustomer.connected_sw_ip = dr[dicColumnMapping["connected_sw_ip"]].ToString();
                            objCustomer.connected_sw_port = dr[dicColumnMapping["connected_sw_port"]].ToString();
                            objCustomer.customer_subscribed_bw_in_kbps = dr[dicColumnMapping["customer_subscribed_bw_in_kbps"]].ToString();
                            objCustomer.customer_connected_through = dr[dicColumnMapping["customer_connected_through"]].ToString();
                            objCustomer.fiber_length = 0;
                            objCustomer.path_type = dr[dicColumnMapping["path_type"]].ToString();
                            objCustomer.primary_bstn_name = dr[dicColumnMapping["primary_bstn_name"]].ToString();
                            objCustomer.secondary_bstn_name = dr[dicColumnMapping["secondary_bstn_name"]].ToString();
                            objCustomer.commissioning_date = DateTimeHelper.Now;
                            objCustomer.vendor_name = dr[dicColumnMapping["vendor_name"]].ToString();
                            objCustomer.kml_length = 0;
                            objCustomer.remarks = dr[dicColumnMapping["remarks"]].ToString();
                            objCustomer.collector_ring_name = dr[dicColumnMapping["collector_ring_name"]].ToString();
                            objCustomer.physical_status = dr[dicColumnMapping["physical_status"]].ToString();
                            objCustomer.fusion_status = dr[dicColumnMapping["fusion_status"]].ToString();
                            objCustomer.commercial_status = dr[dicColumnMapping["commercial_status"]].ToString();
                            objCustomer.terminate_pe_ip = dr[dicColumnMapping["terminate_pe_ip"]].ToString();
                            objCustomer.pe_interface = dr[dicColumnMapping["pe_interface"]].ToString();
                            objCustomer.bstn_sw_port = dr[dicColumnMapping["bstn_sw_port"]].ToString();
                            objCustomer.bstn_sw_ip = dr[dicColumnMapping["bstn_sw_ip"]].ToString();
                            objCustomer.customer_connecting_port_from_bstn_sw = dr[dicColumnMapping["customer_connecting_port_from_bstn_sw"]].ToString();
                            objCustomer.fms_port_no = dr[dicColumnMapping["fms_port_no"]].ToString();
                            objCustomer.created_by = summary.user_id;
                            objCustomer.network_stage = NetworkStatus.A.ToString();
                            objCustomer.upload_id = summary.id;
                            objCustomer.customer_service_address = dr[dicColumnMapping["customer_service_address"]].ToString();
							objCustomer.primary_site_switch_ip = dr[dicColumnMapping["primary_site_switch_ip"]].ToString();
							objCustomer.primary_switch_port_no = dr[dicColumnMapping["primary_switch_port_no"]].ToString();
							objCustomer.secondary_site_id = dr[dicColumnMapping["secondary_site_id"]].ToString();
							objCustomer.secondary_site_name = dr[dicColumnMapping["secondary_site_name"]].ToString();
							objCustomer.secondary_site_address = dr[dicColumnMapping["secondary_site_address"]].ToString();
							objCustomer.secondary_site_switch_ip = dr[dicColumnMapping["secondary_site_switch_ip"]].ToString();
							objCustomer.secondary_site_switch_port_no = dr[dicColumnMapping["secondary_site_switch_port_no"]].ToString();
                            objCustomer.otdr_length = commonUtil.ToDouble(dr[dicColumnMapping["otdr_length"]].ToString()).ToString();
                            objCustomer.po_length = commonUtil.ToDouble(dr[dicColumnMapping["po_length"]].ToString()).ToString();
							
							tempEloc++;
                            lstTempCustomer.Add(objCustomer);
                        }

                        BLWirelineCustomer objTempCustomer = new BLWirelineCustomer();
                        objTempCustomer.SaveBulkCustomer(lstTempCustomer);
                        status.status = StatusCodes.OK.ToString();
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                return tempToMainTable.UpdateStatusAndGetError("UploadTempCustomer", "Upload Data", ex, summary);
            }
        }
    }
}
