using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Models
{
   public class BarCode
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }

        public string network_id { get; set; }

        public string barcode_img_bytes { get; set; }

        public string name { get; set; }

        public string barcode { get; set; }
    }
}
