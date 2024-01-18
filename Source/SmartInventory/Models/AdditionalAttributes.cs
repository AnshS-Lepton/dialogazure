using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Models
{
    public class DynamicControls
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        [JsonProperty(PropertyName = "FieldID")]
        public int id { get; set; }
        public int entity_id { get; set; }
        public int round_off { get; set; }
        public string field_label { get; set; }
        public string field_name { get; set; }
        public string control_type { get; set; }
        public string control_value_type { get; set; }

        [Column("is_mandatory")]
        [JsonProperty("IsRequired")]
        public bool is_mandatory { get; set; }
        public bool is_visible { get; set; }
        public int min_length { get; set; }
        public int max_length { get; set; }
        public string format { get; set; }
        public string default_value { get; set; }
        public string placeholder_text { get; set; }
        public string control_css_class { get; set; }
        public int field_order { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string label_css_class { get; set; }
        [NotMapped]
        public string cal_id { get; set; }
        [NotMapped]
        public string required_text { get; set; }
        [NotMapped]
        public string Typeof { get; set; }
        [NotMapped]
        public int total { get; set; }
        [NotMapped]
        public int s_no { get; set; }
        public string control_id { get; set; }
        [NotMapped]
        public List<DynamicControlsDDLMaster> dynamicControlsDDLMasters { get; set; }
        public string other_info { get; set; }

    }
    public class DynamicFormData
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public int EntityID { get; set; }
        public string Typeof { get; set; }
        public bool IsRequired { get; set; }
        public int RangeMax { get; set; }
        public int RangeMin { get; set; }
        public string Format { get; set; }
        public string DefaultVal { get; set; }
        public string PlaceHolderText { get; set; }
        public int Pos { get; set; }
        public bool HasntBeenSaved { get; set; }
        public string RangeType { get; set; }
        public string ClassNames { get; set; }
    }

    public class DynamicControlsDDLMaster
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int control_id { get; set; }
        public string display_text { get; set; }
        public string value_text { get; set; }
        public bool is_default { get; set; }
        public bool isvisible { get; set; }

    }

    public class vm_dynamic_form
    {
        public List<DynamicControls> lstFormControls { get; set; }
        public List<List<DynamicControls>> lstFormControlsChunk { get; set; }
        public List<DynamicControlsDDLMaster> lstFormDDLValues { get; set; }

    }
    [Serializable]
    public class DynamicFormResponse
    {
        public int id { get; set; }
        public string json { get; set; }
        public string html { get; set; }
        public int ColumnId { get; set; }
    }
    //dynamic_form_styles
    public class DynamicFormStyles
    {
        public int id { get; set; }
        public string control_type { get; set; }
        public string css_class { get; set; }
        public bool is_active { get; set; }
    }
}
