using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.API
{
    public class ApiResponse<T> where T : class
    {
        public string status { get; set; }
        public string error_message { get; set; }
        public T results { get; set; }
    }

    public class WFMApiResponse<T> where T : class
    {
        public string status { get; set; }
        public string message { get; set; }
        public T results { get; set; }
    }
    public class WFMMobileApiResponse<T> where T : class
    {
        public string status { get; set; }
        public string error_message { get; set; }
        public T results { get; set; }
    }


   public class ActivateCpeRes
    {
        public string statusCode { get; set; }
        public string statusDesc { get; set; }
        public string orderId { get; set; }

    }

    public class ActivateCpeResAxtel
    {
        public STATUS STATUS { get; set; }
    }

    public class STATUS
    {
        public string statusDescription { get; set; }
        public string statusType { get; set; }
        public string statusCode { get; set; }
    }
    public class Response
    {
        public string status { get; set; }
        public string error_message { get; set; }
    }
}
