using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Feasibility
{
    public class FeasibilityHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int history_id { get; set; }
        public int feasibility_id { get; set; }
        public FeasibilityInput FeasibilityInput { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string core_level_result { get; set; }
        public string feasibility_result { get; set; }
        public ICollection<FeasibiltyGeometry> FeasibiltyGeometry { get; set; }
        public ICollection<FeasibilityInsideCable> FeasibilityInsideCable { get; set; }

    }

    public class FeasibiltyGeometry
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int history_id { get; set; }
        public FeasibilityHistory FeasibilityHistory { get; set; }
        [NotMapped]
        public string path_type { get; set; }
        public int type_id { get; set; }
        public FeasibilityDemarcationType FeasibilityDemarcationType { get; set; }
        public string cable_geometry { get; set; }
        public double cable_length { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public double material_cost { get; set; }
        public double service_cost { get; set; }
        [NotMapped]
        public bool isSelected { get; set; }
        [NotMapped]
        public int system_id { get; set; }
        [NotMapped]
        public string network_status { get; set; }
        [NotMapped]
        public int available_cores { get; set; }
        public int total_cores { get; set; }
    }

    public class FeasibilityInsideCable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int history_id { get; set; }
        public FeasibilityHistory FeasibilityHistory { get; set; }
        public int system_id { get; set; }
        public string network_status { get; set; }
        public int available_cores { get; set; }
        public int total_cores { get; set; }
        public double cable_length { get; set; }
        public string cable_geometry { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
    }

    #region History

    public class ViewHistoryMaster
    {
        public int systemId { get; set; }

        public FilterHistoryAttr objFilterAttributes { get; set; }

        public string eType { get; set; }

        public List<dynamic> lstData { get; set; }

        public ViewHistoryMaster()
        {
            objFilterAttributes = new FilterHistoryAttr();
            lstData = new List<dynamic>();
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }
    public class FilterHistoryAttr : CommonGridAttributes
    {
        public int systemid { get; set; }
        public string entityType { get; set; }

    }
    #endregion History
}
