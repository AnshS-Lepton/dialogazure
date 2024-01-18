
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SourceIdList
    {
        [Key] 
        public int epsg { get; set; }
        public string dsname { get; set; }
        public string aoi { get; set; }
    }
}
