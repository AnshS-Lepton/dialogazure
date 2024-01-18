using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Admin
{
    public class ConstructionOhLogicMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //[Required]
        public int id { get; set; }

        public int layer_id { get; set; }
        [Required]
        public string category { get; set; }
        public string label { get; set; }
        public string column_name { get; set; }
        public string default_overhead { get; set; }
        public string oh_display_formula { get; set; }
        public string oh_logic { get; set; }

        public List<string> lstUserModule { get; set; }


        public int execution_sequence { get; set; }
        public bool is_accessoroes { get; set; }
        public bool is_aerial_accessoroes { get; set; }

        public bool is_formula_update_allowed { get; set; }
        public double additional_oh_limit { get; set; }
        public int parent_id { get; set; }
        public string entity_sub_class { get; set; }
        public string calculation_sub_type { get; set; }
        public string item_code { get; set; }

        public bool is_active { get; set; }
        public int subarea_system_id { get; set; }
        public string subarea_design_id { get; set; }
        public bool is_default { get; set; }
        public string description { get; set; }
        public string oh_expression_json { get; set; }
        [NotMapped]
        public List<ConstructionOhLogicMaster> constructionohlogicmasterlist { get; set; }
        public bool is_used_as_variable { get; set; }
        [NotMapped]
        public bool isDefaultOh { get; set; }

        public ConstructionOhLogicMaster()
        {
            constructionohlogicmasterlist = new List<ConstructionOhLogicMaster>();
        }

    }

    public class OverheadLogicsDTO
    {
        public string DisplayFormula{ get; set; }
        public string FSAId { get; set; }
        public string OhLogic { get; set; }
        public string ColumnName { get; set; }
        public string OhExpressionJson { get; set; }
    }



}
