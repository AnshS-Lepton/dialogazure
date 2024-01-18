using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class ProvinceBoundary
    {
       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string province_name{ get; set; }

        public int region_id { get; set; }

        public string province_abbreviation { get; set; }
    }
}
