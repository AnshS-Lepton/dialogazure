using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models
{
    #region Template
    public class LayerRightsTemplateMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string template_name { get; set; }
        public bool is_active { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }


    }
    public class ViewLayerRightsTempModel
    {
        public List<LayerRightsTemplateMaster> lstLayerRightTemplates { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ViewLayerRightsTempModel()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }

    public class LayerRightsTemplatePermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [NotMapped]
        public int role_id { get; set; }
        public int template_id { get; set; }
        public int layer_id { get; set; }
        public bool add { get; set; }
        public bool edit { get; set; }
        public bool delete { get; set; }
        public bool viewonly { get; set; }
        public string network_status { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int modified_by { get; set; }
       
        [NotMapped]
        public string entity_name { get; set; }
        [NotMapped]
        public bool add_planned { get; set; }
        [NotMapped]
        public bool update_planned { get; set; }
        [NotMapped]
        public bool delete_planned { get; set; }
        [NotMapped]
        public bool view_planned { get; set; }
        [NotMapped]
        public bool add_asbuilt { get; set; }
        [NotMapped]
        public bool update_asbuilt { get; set; }
        [NotMapped]
        public bool delete_asbuilt { get; set; }
        [NotMapped]
        public bool view_asbuilt { get; set; }
        [NotMapped]
        public bool add_dormant { get; set; }
        [NotMapped]
        public bool update_dormant { get; set; }
        [NotMapped]
        public bool delete_dormant { get; set; }
        [NotMapped]
        public bool view_dormant { get; set; }
        
    }

    [Serializable]
    public class TemplateViewModel
    {
        [Required]
        public string template_name { get; set; }
        public int template_Id { get; set; }
        public int? parentTemplateId { get; set; }
        public RoleMaster objRoleMaster { get; set; }
        public List<LayerRightsTemplatePermission> lstTemplatePermission { get; set; }
        public List<SelectListItem> templateList { get; set; }
        public PageMessage pageMsg { get; set; }
        public TemplateViewModel()
        {
            pageMsg = new PageMessage();
            objRoleMaster = new RoleMaster();
            lstTemplatePermission = new List<LayerRightsTemplatePermission>();
            templateList = new List<SelectListItem>();
        }
    }
    #endregion



}
