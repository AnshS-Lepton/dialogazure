using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Feasibility
{
    public class FeasibilityCableType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]          
        public int cable_type_id { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Enter cores greater than zero")]
        public int cores { get; set; }

        [Required]
        public string display_name { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Enter unit greater than zero")]
        public double material_price_per_unit { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Enter unit greater than zero")]
        public double service_price_per_unit { get; set; }
        public ICollection<FeasibilityInput> FeasibilityInput { get; set; }

        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }

        public bool is_deleted { get; set; }
        public bool is_used { get; set; }

        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }

        [NotMapped]
        public bool IsExists { get; set; }

        [NotMapped]
        public string created_user { get; set; }

        [NotMapped]
        public string modified_user { get; set; }


        [NotMapped]
        public PageMessage pageMsg { get; set; }

        public FeasibilityCableType()
        {
            IsExists = false;
        }
    }

    public class FeasibiltiyCablesearch {

        public int cable_type_id { get; set; }

        [Required]
        public int cores { get; set; }

        [Required]
        public string display_name { get; set; }

        [Required]
        public double material_price_per_unit { get; set; }

        [Required]
        public double service_price_per_unit { get; set; }
    }

    public class FeasibilityCableTypeHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int cable_type_id { get; set; }
        public int cores { get; set; }
        public string display_name { get; set; }
        public double material_price_per_unit { get; set; }
        public double service_price_per_unit { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
    }

    public class FeasibilityDemarcationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int type_id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public ICollection<FeasibiltyGeometry> FeasibiltyGeometry { get; set; }
    }

}
