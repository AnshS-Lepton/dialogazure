using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Admin
{
    public class CableColorSettings
    {   [Key]

        public int color_id { get; set; }
        public string color_character { get; set; }
        public string color_name { get; set; }
        public string color_code { get; set; }
        public string type { get; set; }
        public int? created_by { get; set; }        
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
    }
    public class ModelCableColorSettings
    {
        public List<CableColorSettings> lstCableColor { get; set; }
        [NotMapped]
        public int total_core { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public string type { get; set; }
        public int userId { get; set; }
        public ModelCableColorSettings()
        {
            pageMsg = new PageMessage();
        }
    }

   
}
