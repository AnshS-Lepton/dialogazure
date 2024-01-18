using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Legend
    {
        public string baseUrl { get; set; }
        public List<LegendDetail> legendList { get; set; }
        public string pageSize { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public int userRoleId { get; set; }

        public Legend()
        {
            legendList = new List<LegendDetail>();
        }
    }

    public class LegendDetail 
    {
        public int legend_id { get; set; }
        public int layer_id { get; set; }
        public string group_name { get; set; }
        public string sub_Layer { get; set; }
        public string icon_path { get; set; }
        public string color_code { get; set; }
        public int created_by { get; set; }

        public Int64 entity_count { get; set; }

        public string entity_status { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        public string color_code_hex { get; set; }
        public string outline_color_hex { get; set; }

    } 


}
