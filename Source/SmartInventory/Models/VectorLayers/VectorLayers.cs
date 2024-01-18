using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VectorLayers
{
    public class VectorDeltaIn
    {
        public string LastFetchTime { get; set; }
        public string PrvinceIds { get; set; }
        public int FSAId { get; set; }
        public string connectionString { get; set; }

    }
    public class VectorDataIn
    {
        public string PrvinceIds { get; set; }
        public string connectionString { get; set; }

    }

    public class VectorProvinceDataIn
    {
        public int UserId { get; set; }
    }
}
