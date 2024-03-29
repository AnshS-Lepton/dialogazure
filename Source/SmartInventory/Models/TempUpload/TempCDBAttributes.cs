using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Models.TempUpload
{
    public class TempCDBAttributes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string cable_id { get; set; }
        public double? execution { get; set; }
        public double? row_availablity { get; set; }
        public double? iru_given_airtel { get; set; }
        public double? iru_given_jio { get; set; }
        public double? iru_given_ttsl_or_ttml { get; set; }
        public double? iru_given_tcl { get; set; }
        public double? iru_given_others { get; set; }
        public double? distance { get; set; }
        public DateTime? row_valid_or_exp { get; set; }
        public int? fiber_pairs_laid { get; set; }
        public int? total_used_pair { get; set; }
        public int? fiber_pairs_used_by_vil { get; set; }
        public int? fiber_pairs_given_to_airtel { get; set; }
        public int? fiber_pairs_given_to_others { get; set; }
        public int? fiber_pairs_free { get; set; }
        public int? faulty_fiber_pairs { get; set; }
        public double? start_latitude { get; set; }
        public double? start_longitude { get; set; }
        public double? end_latitude { get; set; }
        public double? end_longitude { get; set; }
        public string count_non_vil_tenancies_on_route { get; set; }
        public DateTime? route_lit_up_date { get; set; }
        public double? aerial_km { get; set; }
        public double? avg_loss_per_km { get; set; }
        public double? avg_last_six_months_fiber_cut { get; set; }
        public double? row { get; set; }
        public double? material { get; set; }
        public string fiber_type { get; set; }
        public string major_route_name { get; set; }
        public string route_id { get; set; }
        public string section_name { get; set; }
        public string section_id { get; set; }
        public string route_category { get; set; }
        public string network_category { get; set; }
        public string remarks { get; set; }
        public string cable_owner { get; set; }
        public string route_type { get; set; }
        public string operator_type { get; set; }
        public string circle_name { get; set; }
        public int upload_id { get; set; }
        public int? created_by { get; set; }
        public int? batch_id { get; set; }
        public string error_msg { get; set; }
    }
}
