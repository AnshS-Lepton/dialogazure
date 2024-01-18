using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class ViewAOIConfiguration
    {
        [Key]
        public int srno { get; set; }
        public string state_name { get; set; }
        public string state_abbre { get; set; }
        public string city_name { get; set; }
        public string city_abbre { get; set; }
        [NotMapped]
        public List<ViewAOIConfiguration> lstAOIAttributeDetails { get; set; }

        public ViewAOIConfiguration()
        {
            lstAOIAttributeDetails = new List<ViewAOIConfiguration>();
        }

    }

   
}
