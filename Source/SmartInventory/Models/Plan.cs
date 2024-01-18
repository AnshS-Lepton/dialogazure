using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NearestMahhole
    {
        public int building_id { get; set; }
        public string building_location { get; set; }
        public int manhole_id { get; set; }
        public string mh_location { get; set; }
    }
}
