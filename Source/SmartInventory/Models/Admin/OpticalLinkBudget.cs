using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Models.Admin
{


    public class LinkBudgetMaster
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wavelength_id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int wavelength_value { get; set; }
        [Required]
        [Range(0.001, Double.MaxValue)]
        public double patch_cord_loss { get; set; }
        [Required]
        [Range(0.001, Double.MaxValue)]
        public double connector_loss { get; set; }
        [Required]
        [Range(0.001, Double.MaxValue)]
        public double splice_loss { get; set; }
        [Required]
        [Range(0.001, Double.MaxValue)]
        public double cable_loss { get; set; }
        public bool is_active { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public List<SplitterLossMaster> lstSplitterLoss { get; set; }
        public LinkBudgetMaster()
        {
            pageMsg = new PageMessage();
            lstSplitterLoss = new List<SplitterLossMaster>();
            is_active = true; // BY DEFAULT SET TO TRUE... FUNCTIONALITY IS YET TO IMPLEMENT..
        }
            
        [Range(0.001, Double.MaxValue)]
        public double misc_db_loss { get; set; }
        [Range(0.000, Double.MaxValue)]
        public double mechanical_connector { get; set; }
        public bool is_mechanical_connector_included { get; set; }
    }
    public class SplitterLossMaster
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int wavelength_id { get; set; }
        [Required]
        public string splitter_ratio { get; set; }
        [Required]
        [Range(0.001, Double.MaxValue)]
        public double? splitter_loss { get; set; }
        //public bool is_active { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
    }


    public class ViewLinkBudgetDetails
    {
        public List<LinkBudgetMaster> lstLinkBudgetDetail { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ViewLinkBudgetDetails()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }

    


}
