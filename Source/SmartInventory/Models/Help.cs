using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FAQMaster 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string category { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public int created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime created_on { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public List<Help_Category> lstCategories { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public FAQMaster()
        {
            lstCategories = new List<Models.Help_Category>();
            pageMsg = new PageMessage();
        }
    }
    public class FAQ_UserManual 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string category { get; set; }
        public string file_name { get; set; }
        public string display_name { get; set; }
        public int file_size { get; set; }
        public string file_url { get; set; }
        public string file_extension { get; set; }
        public int created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime created_on { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string file_size_converted { get; set; }
        public FAQ_UserManual()
        {
            //lstCategories = new List<Models.Help_Category>();
        }
    }
    public class Help_Category
    {
        public int id { get; set; }
        public string category { get; set; }
    }
    public class vmHelp
    {
        public List<FAQMaster> lstFAQMaster { get; set; }
        public  List<FAQ_UserManual> lstFAQUserManual { get; set; }
        public List<string> lstUserModule { get; set; }
        public vmHelp()
        {
            lstFAQMaster = new List<Models.FAQMaster>();
            lstFAQUserManual = new List<FAQ_UserManual>();
            lstUserModule = new List<string>();
        }
    }
    public class ViewFaqFilter : CommonGridAttributes
    {
        public List<FAQMaster> listFaq { get; set; }
        public List<FAQ_UserManual> listFaqUserManual { get; set; }
        //public List<APIRequestLog> listApiRequestLog { get; set; }
        //public ErrorLog objViewMore { get; set; }
        //public ApiErrorLog objApiViewMore { get; set; }
        //public APIRequestLog objApiRequestViewMore { get; set; }
        public ViewFaqFilter()
        {
            //listApiRequestLog = new List<APIRequestLog>();
            listFaq = new List<FAQMaster>();
            listFaqUserManual = new List<FAQ_UserManual>();

        }
    }
} 