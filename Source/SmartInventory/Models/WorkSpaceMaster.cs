using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class WorkSpaceMaster
    {
       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       public int id { get; set; }
       public int user_id { get; set; }
       [Required]
       public string workspace_name { get; set; }
       public int zoom { get; set; }
       public double lat { get; set; }
       public double lng { get; set; }
       public bool has_label { get; set; }
       public DateTime? modified_on { get; set; }
       public DateTime created_on { get; set; }
       public ICollection<WorkSpaceLayers> WSLayers { get; set; }
        public ICollection<LandbaseWorkSpaceLayers> WSLandbaseLayers { get; set; }
        public ICollection<WorkSpaceRegionProvince> WSRegionProvince { get; set; }

    }

    public class WorkSpaceLayers
    {
         [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
         [ForeignKey("WorkSpaceMaster")]
        public int workspace_id { get; set; }
        public int layer_id { get; set; }
        public bool is_as_built { get; set; }
        public bool is_planned { get; set; }
        public bool is_dormant { get; set; }
        public bool is_labels { get; set; }
        public WorkSpaceMaster WorkSpaceMaster { get; set; }
    }


    public class LandbaseWorkSpaceLayers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [ForeignKey("WorkSpaceMaster")]
        public int workspace_id { get; set; }
        public int landbase_layer_id { get; set; }
        public bool is_labels { get; set; }
        public WorkSpaceMaster WorkSpaceMaster { get; set; }
    }

    public class WorkSpaceRegionProvince
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        [ForeignKey("WorkSpaceMaster")]
        public int workspace_id { get; set; }
        public WorkSpaceMaster WorkSpaceMaster { get; set; }
    }
}
