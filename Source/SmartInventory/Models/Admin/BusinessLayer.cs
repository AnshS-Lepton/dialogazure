using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BusinessLayer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]

        public string layer_name { get; set; }
        public string layer_type { get; set; }
        [Required]
        public string display_layer_name { get; set; }
        [Required]
        public string base_url { get; set; }
        [Required]
        public bool is_active { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        [NotMapped]
        public bool IsSubmit { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public int S_No { get; set; }
        [NotMapped]
        public string hdnlayername { get; set; }
        public string authentication_key { get; set; }
        public string version { get; set; }
        public string style { get; set; }
        public string tilematrixset { get; set; }
        public string url_display_name { get; set; }
        public string used_for { get; set; }
        public string map_file_path { get; set; }
        public bool isbaselayer { get; set; }
        public string imagepath { get; set; }
        public string srs { get; set; }
        public string crs { get; set; }
        public string request { get; set; }
        public string service { get; set; }
        public string format { get; set; }
        public int? reqver { get; set; }
        public bool transparent { get; set; }
        public BusinessLayer()
        {
            pageMsg = new PageMessage();
        }
    }
    public class ViewBusinessLayerModel
    {
        public int Id { get; set; }
        public List<BusinessLayer> lstBusinessLayer { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ViewBusinessLayerModel()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }

    public class BusinessViewModel
    {
        public int id { get; set; }
        public string layer_type { get; set; }
        public string url_display_name { get; set; }
        public string authentication_key { get; set; }
        public string version { get; set; }
        public string base_url { get; set; }
        public string tilematrixset { get; set; }
        public string map_file_path { get; set; }
        public List<layerdetails> LayerDetails { get; set; }

    }
    public class layerdetails
    {
        public string display_layer_name { get; set; }
        public string layer_name { get; set; }
        public string style { get; set; }
        public bool is_active { get; set; }
        public string used_for { get; set; }
        public bool isbaselayer { get; set; }
        public string srs { get; set; }
        public string crs { get; set; }
        public string request { get; set; }
        public string service { get; set; }

        public string format { get; set; }
        public int? reqver { get; set; }
        public bool transparent { get; set; }
        public string imagepath { get; set; }
    }

    public class BusnessLayerforAPI
    {
        public int id { get; set; }
        public string layer_type { get; set; }
        public string url_display_name { get; set; }
        public string base_url { get; set; }
        public string authentication_key { get; set; }
        public string version { get; set; }
        public string tilematrixset { get; set; }
        public string map_file_path { get; set; }

        public string display_layer_name { get; set; }
        public string layer_name { get; set; }
        public string style { get; set; }
        public bool is_active { get; set; }
        public string used_for { get; set; }
    }
}
