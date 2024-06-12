using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ColumnMapping
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int template_id { get; set; }
        public string template_db_column_name { get; set; }
        public string template_column_name { get; set; }
        public string imported_column_name { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }

        public bool is_mandatory { get; set; } 

        public bool is_template_column_required { get; set; }

    }
 
    public class ColumnMappingTemplate
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int layer_id { get; set; }
        public string template_name { get; set; }       
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }
        [NotMapped]
        public List<ColumnMapping> listMappedColumns { get; set; }
        [NotMapped]
        public List<string> listImportedColumns { get; set; }
        [NotMapped]
        public List<ColumnMappingTemplate> listTemplates { get; set; } 
        [NotMapped]
        public string layerName { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public bool isFinalMapping { get; set; }
        [NotMapped]
        public int uploadId { get; set; } 
        public bool is_region_uploader_template { get; set; } 
        public bool is_province_uploader_template { get; set; }
        [NotMapped]
        public string boundary_type { get; set; }
        [NotMapped]
        public List<int> lst_UploadId { get; set; }
        public ColumnMappingTemplate()
        {
            listMappedColumns = new List<ColumnMapping>();
            listImportedColumns = new List<string>();
            listTemplates = new List<ColumnMappingTemplate>();
            objPM = new PageMessage();
            lst_UploadId = new List<int>();
        }
    }
    public class NewColumns {
        public string sourceColumn { get; set; }
        public string destinationColumn { get; set; }
        public bool isSourceTemplateColumn { get; set; }
    }
}
