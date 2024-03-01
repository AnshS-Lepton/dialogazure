using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class InputGeom
    {
        public int systemId { get; set; }
        public string geomType { get; set; }
        public string entityType { get; set; }
        public int userId { get; set; }
        public string longLat { get; set; }
        public string commonName { get; set; }
        public string networkStatus { get; set; }
        public string ports { get; set; }
        public string entity_category { get; set; }
        public bool is_virtual{ get; set; }
        public string centerLineGeom { get; set; }
        public decimal buffer_width { get; set; }
        public int? project_id { get; set; }
        public InputGeom()
        {
            networkStatus = "P";
        }
    }
    public class EditGeomIn
    {
        public int systemId { get; set; }
        public string geomType { get; set; }
        public string entityType { get; set; }
        public int userId { get; set; }
        public string longLat { get; set; }
        public bool isExisting { get; set; }
        public int  Bld_Buffer { get; set; }
        public string networkStatus { get; set; } 
        public string centerLineGeom { get; set; }
        public List<EditLineTPIn> tpDetail { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public EditGeomIn()
        {
            networkStatus = "P";
        }
    }
    public class GeomCheck
    {
        public string geomA { get; set; }
        public string geomB { get; set; }
    }

}
