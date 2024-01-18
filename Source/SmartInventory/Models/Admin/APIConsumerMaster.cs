using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Admin
{
    public class APIConsumerMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int consumer_id { get; set; }
        [Required]
        public string source { get; set; }
        [Required]
        public string user_name { get; set; }
        [Required]
        public string password { get; set; }
        public bool is_active { get; set; }
        public bool is_log_required { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }


    }

    public class APIConsumerMasterVw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int consumer_id { get; set; }
        public string source { get; set; }
        public string user_name { get; set; }
        public int totalRecords { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        public DateTime? created_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string is_active_text { get; set; }
        [NotMapped]
        public string is_log_required_text { get; set; }
        [NotMapped]
        public int total_records { get; set; }
        [NotMapped]
        public int user_id { get; set; }
    }

    public class APIConsumerMasterRequest
    {
        public APIConsumerMaster objAPIConsumerMaster { get; set; }
        public PageMessage pageMsg { get; set; }


        public APIConsumerMasterRequest()
        {
            objAPIConsumerMaster = new APIConsumerMaster();
            pageMsg = new PageMessage();
        }
    }
    public class VwAPIConsumerMaster
    {
        public VwAPIConsumerMasterFilter objAPIConsumerMasterFilter { get; set; }
        public List<APIConsumerMasterVw> lstAPIConsumerMaster { get; set; }

        public VwAPIConsumerMaster()
        {
            objAPIConsumerMasterFilter = new VwAPIConsumerMasterFilter();
            lstAPIConsumerMaster = new List<APIConsumerMasterVw>();
        }
    }
    public class VwAPIConsumerMasterFilter : CommonGridAttributes { }
}
