using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class UploadSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string file_name { get; set; }
        public DateTime start_on { get; set; }
        public DateTime end_on { get; set; }
        public string entity_type { get; set; }
        public string status { get; set; }
        public int total_record { get; set; }
        public int failed_record { get; set; }
        public int success_record { get; set; }
        public int other_record { get; set; }
        public string status_message { get; set; }
        public string err_description { get; set; }
        public int line_number { get; set; }
        public int total { get; set; }
        public string execution_type { get; set; }
        public string plan_id { get; set; }
        public bool is_child_entity { get; set; }
        [NotMapped]
        public List<ErrorMessage> lstErrorMessage { get; set; }
    }

    [NotMapped]
    public class ViewUploadSummary : UploadSummary
    {
        public string uploaded_by { get; set; }
        public int s_no { get; set; }
    }
    public class TemplateColumn 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int layer_id { get; set; }
        [NotMapped]
        public string layer_name { get; set; }
        public string db_column_name { get; set; }
        public string db_column_data_type { get; set; }
        public string template_column_name { get; set; }
        public string description { get; set; }
        public string example_value { get; set; }
        public int max_length { get; set; }
        public int column_sequence { get; set; }
        public string display_column_data_type { get; set; }
        public bool is_dropdown { get; set; }
        public bool is_kml_attribute { get; set; }
        public bool is_nullable { get; set; }
        public bool is_excel_attribute { get; set; }
        public DateTime? created_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        public int? created_by { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool is_mandatory { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
       
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<layerDetail> lstLayerDetails { get; set; }
        [NotMapped]
        public List<TemplateColumnDropdown> lstTemplateColumnDropdown { get; set; }
      
        public TemplateColumn()
        {
            lstLayerDetails = new List<layerDetail>();
            lstTemplateColumnDropdown = new List<TemplateColumnDropdown>();

        }
    }

    public class ViewTemplateColumn : CommonGridAttributes
    {
        public List<TemplateColumn> listTemplateColumn { get; set; }
        public ViewTemplateColumn()
        {
            listTemplateColumn = new List<TemplateColumn>();
        }

    }

    public class TemplateColumnDropdown
    {
        public string db_column_name { get; set; }
        public string data_type { get; set; }
        public int sequence { get; set; }
        
    }
}
