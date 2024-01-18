using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class DynamicTheme
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int theme_id { get; set; }
        public string theme_name { get; set; }
        public string css_file_content { get; set; }
        public string thumbnail { get; set; }
        public bool is_active { get; set; }
        [NotMapped]
        public List<DynamicTheme> themelist { get; set; }

        public DynamicTheme()
        {
          themelist = new List<DynamicTheme>();
        }

    }
   
}
