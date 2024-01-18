
using System.ComponentModel.DataAnnotations;


namespace Models.API
{
    public class HeaderAttributes
    {

        [Required(ErrorMessage = "Source is requirred.")]
        public string source { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_description { get; set; }
        [Required(ErrorMessage = "Entity_type is requirred.")]
        public string entity_type { get; set; }
        [Required(ErrorMessage = "Entity_action is requirred.")]
        public string entity_action { get; set; }
        public string language { get; set; }
        public int user_id { get; set; }
        public int structure_id { get; set; }
        public bool is_new_entity { get; set; }
        public string authorization { get; set; }

    }
}
