using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Feasibility
{
    public class FeasibilityInput
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int feasibility_id { get; set; }
        [Required]
        public string feasibility_name { get; set; }
        [Required]
        public string customer_id { get; set; }
        [Required]
        public string customer_name { get; set; }
        [Required]
        public string start_lat_lng { get; set; }
        [Required]
        public string end_lat_lng { get; set; }
        [Required]
        public int cores_required { get; set; }
        [Required]
        public int cable_type_id { get; set; }
        public FeasibilityCableType FeasibilityCableType { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [Required]
        public double buffer_radius_a { get; set; }
        [Required]
        public double buffer_radius_b { get; set; }
        public string start_point_name { get; set; }
        public string end_point_name { get; set; }
        [NotMapped]
        public string core_level_result { get; set; }
        [NotMapped]
        public string feasibility_result { get; set; }
        public ICollection<FeasibilityHistory> FeasibilityHistory { get; set; }

    }
}
