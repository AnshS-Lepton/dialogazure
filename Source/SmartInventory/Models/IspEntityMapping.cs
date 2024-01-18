using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Models
{
    public class IspEntityMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int structure_id { get; set; }
        public int? shaft_id { get; set; }
        public int? floor_id { get; set; }
        public string entity_type { get; set; }
        public int entity_id { get; set; }
        public int parent_id { get; set; }
        [NotMapped]
        public string AssoType { get; set; }
        [NotMapped]
        public List<ShaftFloorList> lstShaft { get; set; }
        [NotMapped]
        public List<ShaftFloorList> lstFloor { get; set; }
        [NotMapped]
        public List<StructureList> lstStructure { get; set; }
        [NotMapped]
        public List<ISP.StructureElement> UnitList { get; set; }
        [NotMapped]
        public int unitId { get; set; }
        [NotMapped]
        public int AssociateStructure { get; set; }
        [NotMapped]
        public bool isValidParent { get; set; }
        [NotMapped]
        public bool isShaftElement { get; set; }
        [NotMapped]
        public bool isFloorElement { get; set; }
        [NotMapped]
        public int? template_id { get; set; }
        [NotMapped]
        public string operation { get; set; }
        [NotMapped]
        public string element_type { get; set; }
        public IspEntityMapping()
        {
            shaft_id = 0;
            floor_id = 0;
            AssoType = "";
            lstShaft = new List<ShaftFloorList>();
            lstFloor = new List<ShaftFloorList>();
            UnitList = new List<ISP.StructureElement>();
        }
    }
    public class IspLineMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int entity_id { get; set; }
        public string entity_type { get; set; }
        public string cable_type { get; set; }
        public string line_geom { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int structure_id { get; set; }
        public string a_node_type { get; set; }
        public string b_node_type { get; set; }

        public string geom_source { get; set; }
        public string display_name { get; set; }
    }
    public class OSPISPCable
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }       
        public string line_geom { get; set; }
        public int StructureId { get; set; }
        public string geom_source { get; set; }
        public string a_node_type { get; set; }
        public string b_node_type { get; set; }
    }
}
