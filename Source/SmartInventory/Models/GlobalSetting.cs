using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GlobalSetting
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public bool is_edit_allowed { get; set; }
        public string data_type { get; set; }
        public double min_value { get; set; }
        public double max_value { get; set; }
        public bool is_mobile_key { get; set; }
        public bool is_web_key { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        public GlobalSetting()
        {
            pageMsg = new PageMessage();
        }

    }


    public class VMGlobalSetting
    {
        public List<GlobalSetting> lstGlobalSettings { get; set; }
        public int user_id { get; set; }
        public PageMessage pageMsg { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }//change shazia

        public List<KeyValueDropDown> lstSearchBy { get; set; }

        public VMGlobalSetting()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;

            lstGlobalSettings = new List<GlobalSetting>();
            pageMsg = new PageMessage();
        }
    }

    
}