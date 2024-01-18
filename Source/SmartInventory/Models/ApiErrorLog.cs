using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ApiErrorLog
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }       
        public string controller_name { get; set; }
        public string action_name { get; set; }
        public string err_message { get; set; }
        public string err_label { get; set; }
        public string err_type { get; set; }
        public string err_description { get; set; }
        public string stack_trace { get; set; }
        public string request_data { get; set; }
        public DateTime err_date = DateTimeHelper.Now;
        [NotMapped]
        public DateTime? created_on { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }

    }

    public class ApiErrorLogInput
    {
        public int userId { get; set; }
        public string UserName { get; set; }
        public string fromPage { get; set; }
        public string fromMethod { get; set; }

        public string clientIp { get; set; }
        public string serverIp { get; set; }
        public string browserName { get; set; }
        public string browserVersion { get; set; }
        public string statusCode { get; set; }

        public string errorMessage { get; set; }
        public string requestData { get; set; }
        public object EntityObject { get; set; }
        public Exception exception { get; set; }
        public string ERR_DESCRIPTION { get; set; }
        public string ERR_TYPE { get; set; }
        public string ERR_ON_PAGE { get; set; }
        public string ERR_ON_METHOD { get; set; }

        public ApiErrorLogInput()
        {
            EntityObject = null;
            exception = null;
            userId = 0;
            UserName = "";
            fromPage = "";
            fromMethod = "";
            clientIp = "";
            serverIp = "";
            browserName = "";
            browserVersion = "";
            statusCode = "";
            errorMessage = "";
            ERR_TYPE = "";
            ERR_ON_PAGE = "";
            ERR_ON_METHOD = "";
            ERR_DESCRIPTION = "";

        }
    }
}
