using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class ValidateEntityGeom
    {
        public string geomType { get; set; }
        public string entityType { get; set; }
        public string txtGeom { get; set; }
        public int system_id { get; set; }
        public bool isTemplate { get; set; }
        public string subEntityType { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public int user_id { get; set; }
        public int ticket_id { get; set; }
    }
}
