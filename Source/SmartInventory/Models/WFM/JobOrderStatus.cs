

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WFM
{
    public class JobOrderStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string status { get; set; }
        public bool is_active { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_date { get; set; }
    }
    public class TicketSteps
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string steps { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_date { get; set; }
        public string icon_content { get; set; }
        public string icon_class { get; set; }
        public bool is_processed { get; set; }
    }
    public class Brands
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string brand_name { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_date { get; set; }
    }
    public class BrandModel
    {
        public string model_name { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public DateTime created_datetime { get; set; }
    }

    public class JobOrderDetail
    {
        public string job_id { get; set; }
        public string primary_contact { get; set; }
        public string secondary_contact { get; set; }
        public string email_id { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string address_type { get; set; }
        public string city { get; set; }
        public string pinCode { get; set; }
        public string state_Province { get; set; }
        public string address_id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string cpe_portno { get; set; }
        public string cpe_type { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public int stage { get; set; }
        public int step_order { get; set; }
        

        public string cpe_ref_serial { get; set; }
        public string cpe_item_code { get; set; }
        public string cpe_uom { get; set; }
        public string cpe_wh { get; set; }

        public string set_up_box_serial_number { get; set; }

        public string node { get; set; }
        public string nap_port { get; set; }
    }
    public class customer_detail
    {
        public string job_id { get; set; }
        public string primary_contact { get; set; }
        public string secondary_contact { get; set; }
        public string email_id { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string address_type { get; set; }
        public string city { get; set; }
        public string pinCode { get; set; }
        public string state_Province { get; set; }
        public string address_id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string customer_name { get; set; }
    }
    public class cpe_detail
    {
        public string job_id { get; set; }
        public string cpe_portno { get; set; }
        public string cpe_type { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string cpe_ref_serial { get; set; }
        public string cpe_item_code { get; set; }
        public string cpe_uom { get; set; }
        public string cpe_wh { get; set; }

        public string set_up_box_serial_number { get; set; }// map to device_serial_number1
        public string nap { get; set; }
        public string nap_port { get; set; }
        public string is_cpe_collected { get; set; }    
        public List<BrandModel> lstModels { get; set; }
        public cpe_detail()
        {
            lstModels = new List<BrandModel>();
            cpe_ref_serial = "";
            cpe_item_code = "";
            cpe_uom = "";
            cpe_wh = "";
        }
    }
    public class job_order_status
    {
        public string job_id { get; set; }
        public string action { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string remarks { get; set; }
        public DateTime? date { get; set; }
        public string time { get; set; }
        public string rc { get; set; }
        public string rca { get; set; }
        public string cpe_serialno { get; set; }
        // public string capture_issue { get; set; }
        public string slotid { get; set; }
        public string service_status { get; set; }
        public int step_order { get; set; }
    }

    public class JobRescheduleIn
    {
        public string job_id { get; set; }
        public DateTime date { get; set; }
    }

    public class checkin
    {
        public string job_id { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string user_id { get; set; }
    }
    public class hpsm_ticket_attachments
    {
        public int id { get; set; }
        public string job_id { get; set; }
        [NotMapped]
        public string ImgSrc { get; set; }
        [NotMapped]
        public string ImgSrcThumb { get; set; }
        public string org_file_name { get; set; }
        public string file_name { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public string upload_type { get; set; }
        public int uploaded_by { get; set; }
        public DateTime? uploaded_on { get; set; }
        public int file_size { get; set; }
        public string screen { get; set; }
        // public string remark { get; set; }
        public string doc_type { get; set; }
        [NotMapped]
        public string file_size_converted { get; set; }
        [NotMapped]
        public byte[] attachmentSource { get; set; }
    }

    public class vw_hpsm_ticket_attachments
    {
        public int id { get; set; }
        public string job_id { get; set; }
        [NotMapped]
        public string ImgSrc { get; set; }
        [NotMapped]
        public string ImgSrcThumb { get; set; }
        public string org_file_name { get; set; }
        public string file_name { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public string upload_type { get; set; }
        public int uploaded_by { get; set; }
        public DateTime? uploaded_on { get; set; }
        public int file_size { get; set; }
        public string screen { get; set; }
        //public string remark { get; set; }
        public string doc_type { get; set; }
        [NotMapped]
        public string file_size_converted { get; set; }
        [NotMapped]
        public byte[] attachmentSource { get; set; }
        public string uploaded_by_name { get; set; }
    }
    public class getDetailIn
    {
        public string job_id { get; set; }
        public int step_order { get; set; }
        
    }
    public class ActivateCPEDetail
    {
        public string job_id { get; set; }
        public int step_order { get; set; }
        public string IsCPECollected { get; set; }
    }
    public class getttStatusIn
    {
        public string job_id { get; set; }
        public string main_issue_type { get; set; }

    }
    public class getModelin
    {
        public string brand_name { get; set; }
    }

    public class AdditionalMaterialIn
    {
        public string jobid { get; set; }
        public int step_order { get; set; }
        public List<AdditionalMaterial> additionalmaterial { get; set; }
       //public int reading_from { get; set; }
       //public int reading_to { get; set; }
    }
    public class getCPEDetailIn
    {
        public string serial_no { get; set; }
        public string type { get; set; }
        public int user_id { get; set; }
        public int material_id { get; set; }
        public int quantity { get; set; }
    }

    public class CPEDetail
    {
        public string serial_code { get; set; }
    }




    public class entityconfigurationIn
    {
        public string entity_type { get; set; }
        public string entity_id { get; set; }
        public string serial_no { get; set; }
        public string jobid { get; set; }
    }
    public class reschedule
    {
        public string jobid { get; set; }
        public DateTime? rescheduledate { get; set; }
    }

    public class getStatusDetail
    {
        public string jobid { get; set; }
        public string status { get; set; }
        public string sub_status { get; set; }
        public string remarks { get; set; }
    }

    public class ConnectedDeviceDetail
    {
        public string jobid { get; set; }
        public string serialno { get; set; }
        public string devicetype { get; set; }
        public string requestid { get; set; }
    }
    public class rcaIn
    {
        public string status { get; set; }
    }

    public class ConnectedDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id { get; set; }
        public string ssidname { get; set; }
        public string ip { get; set; }
        public string type { get; set; }
        public string devicename { get; set; }
        public string mac { get; set; }
        public string status { get; set; }
        public string requestid { get; set; }

    }
    public class WfmRca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id { get; set; }
        public string rca { get; set; }
        public string status { get; set; }
        public int ticket_source { get; set; }
    }

    public class Root
    {

        public List<ConnectedDevice> data { get; set; }
        public string requestid { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string statuscode { get; set; }
    }
    public class ConnectedDeviceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id { get; set; }
        public string jobid { get; set; }
        public string requestid { get; set; }
        public string devicetype { get; set; }
        public string serialno { get; set; }
        public string status { get; set; }
        public string message { get; set; }


    }
    
}
