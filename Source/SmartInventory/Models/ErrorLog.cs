using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ErrorMessage
    {
        [NotMapped]
        public bool is_valid { get; set; }
        [NotMapped]
        public string error_msg { get; set; }
        [NotMapped]
        public int uploaded_by { get; set; }
        [NotMapped]
        public string error_code { get; set; }
        [NotMapped]
        public string status { get; set; }
        public Exception exMessage { get; set; }

    }
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string client_ip { get; set; }
        public string server_ip { get; set; }
        public string browser_name { get; set; }
        public string browser_version { get; set; }
        public string status_code { get; set; }
        public string controller_name { get; set; }
        public string action_name { get; set; }
        public string err_message { get; set; }
        public string err_label { get; set; }
        public string err_type { get; set; }
        public string err_description { get; set; }
        public string stack_trace { get; set; }
        
        public DateTime? created_on { get; set; }

        [NotMapped]
        public int totalRecords { get; set; }
    }

    public class ErrorLogInput
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
        public object EntityObject { get; set; }
        public Exception exception { get; set; }
        public string ERR_DESCRIPTION { get; set; }
        public string ERR_TYPE { get; set; }
        public string ERR_ON_PAGE { get; set; }
        public string ERR_ON_METHOD { get; set; }
        public ErrorLogInput()
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
    public class ReadMore { public string querytext { get; set; } }
    public class ViewErrorLogFilter: CommonGridAttributes
    {
        public List<ErrorLog> listErrorLog { get; set; }
        public List<ApiErrorLog> listApiErrorLog { get; set; }
        public List<APIRequestLog> listApiRequestLog { get; set; }
        public List<GisApiLog> listGisApiLog { get; set; }
        public ErrorLog objViewMore { get; set; }
        public ApiErrorLog objApiViewMore { get; set; }
        public GisApiLog objGisApiViewMore { get; set; }
        public APIRequestLog objApiRequestViewMore { get; set; }
        public string logtype { get; set; }
        public ViewErrorLogFilter()
        {
            listGisApiLog = new List<GisApiLog>();
            listApiRequestLog = new List<APIRequestLog>();
            listErrorLog = new List<ErrorLog>();
            listApiErrorLog = new List<ApiErrorLog>();
            
        }
    }

    public class PartnerAPILog
    {

        public string request { get; set; }
        public string response { get; set; }
        public string URL { get; set; }
        public string InDateTime { get; set; }
        public string OutDateTime { get; set; }
        public string Transactionid { get; set; }
        public string Latency { get; set; }
        public string UserName { get; set; }


    }
    public class GisApiLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string api_url { get; set; }
        public string request { get; set; }
        public Nullable<DateTime> request_time { get; set; }
        public Nullable<DateTime> response_time { get; set; }
        public string response { get; set; }
        public string api_type { get; set; }
        public int latency { get; set; }
        public string transaction_id { get; set; }
        public int user_id { get; set; }
        public string gdb_version { get; set; }
        public int gis_object_id { get; set; }
        public string entity_type { get; set; }
        public string gis_design_id { get; set; }
        public int system_id { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string process_id { get; set; }


    }
}
