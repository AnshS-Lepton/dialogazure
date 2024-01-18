using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.API
{
    public class ADFSDetail
    {
        public string tokenId { get; set; }
        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
        public string errorMsg { get; set; }
    }
}
