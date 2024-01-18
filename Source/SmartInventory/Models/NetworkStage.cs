using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class NetworkStage
    {
        public int systemid { get; set; }
        public string entity_type { get; set; }
        public string curr_status { get; set; }
        public string old_status { get; set; }
        public int user_id { get; set; }
    }
}
