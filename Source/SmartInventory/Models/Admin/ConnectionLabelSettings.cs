using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Admin
{
    public class ConnectionLabelSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int layer_id { get; set; }
        public string display_column_name { get; set; }
        public string status { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstConnectionLabel { get; set; }
        [NotMapped]
        public string entity_title { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public ConnectionLabelSettings()
        {
            lstConnectionLabel = new List<KeyValueDropDown>();
            pageMsg = new PageMessage();
        }
    }

    public class ConnectionLabelSettingsVw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get;set;}
        public string display_column_name { get; set; }
        public string status { get; set; }
        public int layer_id { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public string entity_title { get; set; }

    }
    public class VwConnectionLabelSettings
    {
        public VwConnectionLabelSettingsFilter objConnectionLabelFilter { get; set; }
        public List<ConnectionLabelSettingsVw> lstViwConnectionLabelSettings { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }

        public VwConnectionLabelSettings()
        {
            objConnectionLabelFilter = new VwConnectionLabelSettingsFilter();
            lstViwConnectionLabelSettings = new List<ConnectionLabelSettingsVw>();
        }
    }
    public class VwConnectionLabelSettingsFilter : CommonGridAttributes {}
}
