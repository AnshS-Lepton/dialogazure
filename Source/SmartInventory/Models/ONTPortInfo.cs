using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ONTPortInfoMaster
    {       
        public int port_no { get; set; }
        public string port_type { get; set; }
        public string port_name { get; set; }
        public string input_output { get; set; }
        public string port_status { get; set; }
        public string customer { get; set; }
       
    }  
}
