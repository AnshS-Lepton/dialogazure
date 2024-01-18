using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AccessoriesViewModel
    {
        public int parent_systemId { get; set; }

        public FilterAccessoriesAttr objFilterAttributes { get; set; }

        public string parent_eType { get; set; }
        public string parent_network_id { get; set; }
        public string parent_network_status { get; set; }
        public List<AccessoriesInfoModel> lstData { get; set; }
        public List<AccessoriesMaster> listDatas { get; set; }
        public List<AccessoriesMapping> listMappingDatas { get; set; }
        public string geomType { get; set; }
        public AccessoriesViewModel()
        {
            objFilterAttributes = new FilterAccessoriesAttr();
            lstData = new List<AccessoriesInfoModel>();
            listDatas = new List<AccessoriesMaster>();
            listMappingDatas = new List<AccessoriesMapping>();
        }
    }
    public class FilterAccessoriesAttr : CommonGridAttributes
    {
        public int parent_systemId { get; set; }
        public string parent_entityType { get; set; }
        public int status { get; set; }

    }
    public class AccessoriesInfoModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public int accessories_id { get; set; }
        public int quantity { get; set; }
        [Required]
        public string specification { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public string item_code { get; set; }
        [Required]
        public int vendor_id { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string remarks { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        [NotMapped]
        public string parent_network_status { get; set; }

        public string parent_entity_type { get; set; }

        public string status { get; set; }

        public string network_status { get; set; }

        public int created_by { get; set; }

        public DateTime created_on { get; set; }

        public int? modified_by { get; set; }

        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public string entity_type { get; set; }
        [NotMapped]
        public string vendor_name { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public List<AccessoriesMaster> listTypes { get; set; }
        [NotMapped]
        public AccessoriesTemplate accessories_template { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        public int audit_item_master_id { get; set; }
        public AccessoriesInfoModel()
        {
            listTypes = new List<AccessoriesMaster>();
            accessories_template = new AccessoriesTemplate();
            objPM = new PageMessage();
        }
    }
    public class AccessoriesMaster
    {
        public int id { get; set; }
        public string name { get; set; }
        public string display_name { get; set; }

        public int created_by { get; set; }

        public DateTime created_on { get; set; }
       
        public int? modified_by { get; set; }

        public DateTime? modified_on { get; set; }
        public bool is_active { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public int min_quantity { get; set; }
        [NotMapped]
        public int max_quantity { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public int S_No { get; set; }
        [NotMapped]
        public string created_by_user { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public AccessoriesMaster()
        {
            pageMsg = new PageMessage();
        }
    }

    public class AccessoriesReportModel
    {
        public string entity_type { get; set; }
        public int quantity { get; set; }
        public string remarks { get; set; }
        
        public string specification { get; set; }
        public string vendor_name { get; set; }
        public string item_code { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public string parent_network_id { get; set; }

        public string parent_entity_type { get; set; }

        public string status { get; set; }

        public string network_status { get; set; }
        public string created_by_text { get; set; }
        public DateTime created_on { get; set; }
        public string modified_by_text { get; set; }
        public DateTime? modified_on { get; set; }
    }

    public class AccessoriesAuditModel
    {
        [NotMapped]
        public int totalRecords { get; set; }
        public string entity_type { get; set; }
        public int quantity { get; set; }
        public string remarks { get; set; }
        
        public string specification { get; set; }
        public string vendor_name { get; set; }
        public string item_code { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public string parent_network_id { get; set; }

        public string parent_entity_type { get; set; }

        public string status { get; set; }

        public string network_status { get; set; }
        public string created_by_text { get; set; }
        public DateTime created_on { get; set; }
        public string modified_by_text { get; set; }
        public DateTime? modified_on { get; set; }
    }
    public class AccessoriesMapping
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [NotMapped]
        public string name { get; set; }
        public int accessories_id { get; set; }

        public int min_quantity { get; set; }
        public int layer_id { get; set; }

        public int max_quantity { get; set; }
        public bool is_active { get; set; }
        public DateTime created_on { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        public int? modified_by { get; set; }
        [NotMapped]
        public string layer_name { get; set; }
        public DateTime? modified_on { get; set; }
        public int? created_by { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }

        [NotMapped]
        public int S_No { get; set; }
        [NotMapped]
        public List<AccessoriesMaster> lstAccessoriesdropdown { get; set; }
        [NotMapped]
        public List<layerDetail> lstLayers { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSpecifyType { get; set; }
        public string specify_type { get; set; }
        public AccessoriesMapping()
        {
            lstAccessoriesdropdown = new List<AccessoriesMaster>();
            lstLayers = new List<layerDetail>();
            pageMsg = new PageMessage();
            lstSpecifyType = new List<KeyValueDropDown>();
        }
    }
    public class AccessoriesMappingList
    {
        public int id { get; set; }
        public int accessories_id { get; set; }
        public string display_name { get; set; }
    }

}
