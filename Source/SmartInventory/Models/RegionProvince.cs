using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ImportRegionProvince
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string region_name { get; set; }
        public string region_abbreviation { get; set; }
        public bool isvisible { get; set; }
        public string boundary_type { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<UpdateGeomtaryProperties> lstUpdateRegionProvince { get; set; }
        [NotMapped]
        public string uploadDirectoryPath { get; set; }

        [NotMapped]
        public int user_id { get; set; }

        public ImportRegionProvince()
        {
            lstUpdateRegionProvince = new List<UpdateGeomtaryProperties>();
            pageMsg = new PageMessage();
        }
    }
    
    public class UpdateGeomtaryProperties
    {
        public int existing_id { get; set; }
        public int id { get; set; }
        public string region_name { get; set; }
        public string region_abbreviation { get; set; }
        public string country_name { get; set; }
        public string province_name { get; set; }
        public string province_abbreviation { get; set; }
        public string geomtext { get; set; }
        public string boundarytype { get; set; }
        public string filename { get; set; }

        public bool IschkBoundary { get; set; }

        public string shapefilepath { get; set; }
        public int? created_by { get; set; }
        public string entryStatus { get; set; }
       
        public bool is_active { get; set; }
        public latlong southWest { get; set; }
        public latlong northEast { get; set; }
        public UpdateGeomtaryProperties()
        {
            region_name = string.Empty;
            province_name = string.Empty;
        }
    }

    public class UpdateGeomtaryValue
    {
        [NotMapped]
        public List<UpdateGeomtaryProperties> lstUpdateRegionProvince { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public bool is_active { get; set; }
        public UpdateGeomtaryValue()
        {
            lstUpdateRegionProvince = new List<UpdateGeomtaryProperties>();
            pageMsg = new PageMessage();
        }
        [NotMapped]
        public int user_id { get; set; }
    }

    public class Status
    {
        public string status { get; set; }
        public string message { get; set; }

    }


    public class ViewRegionProvinces
    {
        [Key]
        public int srno { get; set; }
        [NotMapped]
        public List<ViewRegionProvinces> lstViewRegionProvinceDetails { get; set; }
        [NotMapped]
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public int id { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string region_abbreviation { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string province_abbreviation { get; set; }
        [NotMapped]
        public string country_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string purpose { get; set; }
        [NotMapped]
        public DateTime? created_on { get; set; }
        [NotMapped]
        public int? created_by { get; set; }
        [NotMapped]
        public int? modified_by { get; set; }
        [NotMapped]
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public bool is_active { get; set; }
        [NotMapped]
        public string geometry_extent { get; set; }
        public ViewRegionProvinces()
        {
            objGridAttributes = new CommonGridAttributes();
            lstViewRegionProvinceDetails = new List<ViewRegionProvinces>();
        }

    }

    public class UpdateRegion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        public string region_name { get; set; }

        public string region_abbreviation { get; set; }
        public string country_name { get; set; }
        public int? created_by { get; set; }
        //public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public int user_id { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public UpdateRegion()
        {
            pageMsg = new PageMessage();

        }

    }
    public class InRegionByProvince
    {
        public int region_id { get; set; }
        public string region_name { get; set; }
        public string region_abbreviation { get; set; }
        public string country_name { get; set; }
    }

    public class RegionProvinceRequest
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public string geom { get; set; }
        public string action { get; set; }
        public string region_name { get; set; }
        public string region_abbreviation { get; set; }
        public string province_name { get; set; }
        public string province_abbreviation { get; set; }
        public string country { get; set; }
        public int user_id { get; set; }
    }
}
