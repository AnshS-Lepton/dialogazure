using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class EndToEndSchematic
    {
        //public string id { get; set; }
        //public string name { get; set; }
        //public object start { get; set; }
        //public object end { get; set; }
        //public string details { get; set; }
        //public string comment { get; set; }
        //public string SplitterId { get; set; }
        //public string cable { get; set; }

        public string id { get; set; }
        public string entity_name { get; set; }
        public string entity_type { get; set; }
        public string comment { get; set; }
        public List<EndToEndSchematic> children { get; set; }
        public EndToEndSchematic()
        {
            children = new List<EndToEndSchematic>();
            comment = null;
        }
    }
}
