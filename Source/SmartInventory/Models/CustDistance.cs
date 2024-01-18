using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CustDistance
    {
        public string customer_refid { get; set; }
        public double distance_mtr { get; set; }
        public string node_id { get; set; }       
        public string neareast_node { get; set; }
        public string customer_add { get; set; }
        public string utilized_port { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }
    public class CustDistanceEx
    {
        public string splitter_id { get; set; }
        public double distance_mtr { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string customer_add { get; set; }

    }
    public class CustDistanceIN
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string customer_refid { get; set; }
    }
    public class RequestInput
    {
        public string data { get; set; }

    }
}
