using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Models.Admin;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Web.UI;
using System.Web.Mvc;

namespace Models.Admin
{
    public class OTPAuthenticationSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        //[NotMapped]
        public string application_name { get; set; }
        public bool is_otp_enabled { get; set; }
        public int resend_otp_timer { get; set; }
        public int temp_lock_attempt { get; set; }
        public int temp_lock_duration { get; set; }
        public int permanent_lock_attempt { get; set; }
        public int permanent_lock_duration { get; set; }
        public int otp_min_value { get; set; }
        public int otp_max_value { get; set; }
        public int otp_expiry_time { get; set; }
        public int otp_resend_limit { get; set; }
        [NotMapped]
        public List<OTPAuthenticationSettings> lstotpdetails { get; set; }

        public int alert_message_timeout { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }

        public OTPAuthenticationSettings()
        {
            lstotpdetails = new List<OTPAuthenticationSettings>();
            pageMsg = new PageMessage();
        }


    }

    public class EmailEventTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int template_id { get; set; }
        public string project_phase { get; set; }
        public string trigger_event { get; set; }
        public string recipient_role { get; set; }
        public string recipient_list { get; set; }
        public string subject { get; set; }
        [AllowHtml]
        public string template { get; set; }
        public bool is_active { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public string remarks { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        public EmailEventTemplate()
        {
            created_by = 1;            
            modified_by = 1;
            
        }
       
    }


    public class EmailTemplate
    {
     public string trigger_event { get; set; }
    }
    
    public class EmailBinder
    {
        
        public EmailEventTemplate objEmailEventTemplate { get; set; }
        public List<EmailTemplate> templatelist { get; set; }
        public string trigger_event { get; set; }
        
       
    }
}
