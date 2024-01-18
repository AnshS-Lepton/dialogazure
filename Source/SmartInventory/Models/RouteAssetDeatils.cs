using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RouteAssetDeatils
    {
        public string state { get; set; }
        public string urid { get; set; }
        public string depl_section_name { get; set; }
        public float? google_length { get; set; }
        public float? civil_route_length { get; set; }
        public float? section_length { get; set; }
        public float? backbone_civil_length { get; set; }
        public float? last_mile_civil_length { get; set; }
        public float? optical_cable_length { get; set; }
        public float? aerial_route_length { get; set; }
        public int? total_duct { get; set; }
        public string total_row_permission_received { get; set; }
        public Network_Cable_Length[] network_cable_length { get; set; }
        public Duct_Fiber_Details[] duct_fiber_details { get; set; }
        public Row_Status[] row_status { get; set; }
        public Customer_Details[] customer_details { get; set; }


        public class Network_Cable_Length
        {
            public Cable_Length cable_length { get; set; }
        }

        public class Cable_Length
        {
            public string substring { get; set; }
            public string right { get; set; }
            public float? sum { get; set; }
        }

        public class Duct_Fiber_Details
        {
            public Fiber_Details fiber_details { get; set; }
        }

        public class Fiber_Details
        {
            public string duct_name { get; set; }
            public string cable_name { get; set; }
            public string fiber_cable_laid { get; set; }
            public string used_fiber_core { get; set; }
            public string reserved_fiber_core { get; set; }
            public string available_fiber_core { get; set; }
            public string feasibility { get; set; }
            public string total_core { get; set; }
            public string duct_status { get; set; }
        }

        public class Row_Status
        {
            public Row_Details row_details { get; set; }
        }

        public class Row_Details
        {
            public string row_section_name { get; set; }
            public string depl_row_document_no { get; set; }
            public string row_authority { get; set; }
            public string row_permission_length { get; set; }
            public string application_no { get; set; }
            public string application_received_date { get; set; }

        }

        public class Customer_Details
        {
            public Customer_Table customer_table { get; set; }
        }

        public class Customer_Table
        {
            public string no_of_customer { get; set; }
            public string customer_name { get; set; }
            public string unique_po_no { get; set; }
            // public string customer_live_fiber_in_duct { get; set; }
            public string total_hoto_length { get; set; }
            // public string section_hoto_length { get; set; }
            //public string available_optical_cable { get; set; }
            public string link_in_pair { get; set; }
            public string status { get; set; }
            public string section_name { get; set; }
            public string po_end_date { get; set; }
            public string live_core { get; set; }
            public string hoto_date { get; set; }



        }


    }
}
