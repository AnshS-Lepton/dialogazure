using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class ChangeConfigurationSetting 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int audit_id { get; set; } = 0; 
        public string key_type { get; set; }
      
        public string key_value { get; set; }
       
        public string section { get; set; }

        [NotMapped]
        public string description { get; set; }

        public int created_by { get; set; }

        public DateTime created_on { get; set; }

        public Nullable<int> modified_by { get; set; }

        public Nullable<DateTime> modified_on { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public int totalRecord { get; set; }
        [NotMapped]
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public List<ChangeConfigurationSetting> listCCS { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        public ChangeConfigurationSetting()
        {
            pageMsg = new PageMessage();
            objGridAttributes = new CommonGridAttributes();
            listCCS = new List<ChangeConfigurationSetting>();
        }
    }
}
