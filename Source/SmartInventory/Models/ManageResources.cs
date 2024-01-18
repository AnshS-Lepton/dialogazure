using Models.Admin;
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
    public class ManageResources
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }
        public string key { get; set; }

        [AllowHtml]
        public string value { get; set; }
        //[Required] 
        [Key]
        [Column(Order = 1)]
        public string culture { get; set; }
       
        public string language { get; set; }

        public bool is_visible { get; set; }

        public string description { get; set; }
        public Int32? modified_by { get; set; }
        [NotMapped]
        public string Type { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_jquery_used { get; set; }
        public List<ResourcesList> ResourcesList { get; set; }

        public List<ExportResourceTemplate> ExptResourceTemplate { get; set; }

        public List<ResourceLangugeList> ResourceLangugeList { get; set; }
        [NotMapped]
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        public ManageResources()
        {
            objGridAttributes = new CommonGridAttributes();
            pageMsg = new PageMessage();
            ResourcesList = new List<ResourcesList>();
            ResourceLangugeList = new List<ResourceLangugeList>();
            ExptResourceTemplate = new List<ExportResourceTemplate>();
        }

    }
   
    public class ResourcesList
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }
        public string key { get; set; }

        public string value { get; set; }
        //[Required] 

        public string culture { get; set; }
        public string CurrentValue { get; set; }
        public string language { get; set; }

        public string description { get; set; }
        public int S_No { get; set; }
        public bool? is_jquery_used { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
    }
    

    public class ResourceLangugeList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public string dropdown_type { get; set; }

          
        public string dropdown_value { get; set; }
       
        public string dropdown_key { get; set; }

        

    }

    public class ResourceKeyMaster
    {
        [Required]
        public string Projectkey { get; set; }
        public string Projectvalue { get; set; }
        public string Modulekey { get; set; }
        public string Modulevalue { get; set; }
        public string Sub_Modulekey { get; set; }
        public string Sub_Modulevalue { get; set; }
        public string DotNet_JQuerykey { get; set; }
        public string DotNet_JQueryvalue { get; set; }
        public string Purpose_Typekey { get; set; }
        public string Purpose_Typevalue { get; set; }
        public Boolean is_jquery_used { get; set; }
        public Boolean is_mobile_key { get; set; }

        [Required]
        public string Value { get; set; }
        public string key { get; set; }
        public List<res_dropdown_master> lstProjects { get; set; }
        public List<res_dropdown_master> lstModule { get; set; }
        public List<res_dropdown_master> lstSub_Module { get; set; }
        public List<res_dropdown_master> lstDotNet_JQuery { get; set; }
        public List<res_dropdown_master> lstPurpose_Type { get; set; }
       

       
    }

    public class ExportResourceTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public string dropdown_type { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string Purpose { get; set; }
        public string required_language { get; set; }
        public string required_value { get; set; }
    }

    public class AuditResourcelist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string key { get; set; }

        public string value { get; set; }


    }

    public class ResourcesScriptList
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }
        public string key { get; set; }

        public string value { get; set; }
        //[Required] 

        public string culture { get; set; }
      
        public string language { get; set; }

        public string description { get; set; }
        public bool is_default_lang { get; set; }
        public bool is_visible { get; set; }
        public DateTime created_on { get; set; }

        public int created_by { get; set; }
    }

    public class IsUsedJquery
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }
        public string key { get; set; } 
        public Int32 modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool? is_jquery_used { get; set; }
      

    }

    public class TempResources
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string key { get; set; }

        public string value { get; set; }
        //[Required] 

        public string required_value { get; set; }
        public TempResources()
        {

        }
        public string required_language { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int created_by { get; set; }
        public int uploaded_by { get; set; }
        public DateTimeOffset created_on { get; set; }
        
        
    }

    public class res_dropdown_master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string dropdown_type { get; set; }
        public string dropdown_value { get; set; }
        public int created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime modified_on { get; set; }
        public string dropdown_key { get; set; }
        public bool dropdown_status { get; set; }
        public bool is_used_for_mobile { get; set; }
        public bool is_used_for_web { get; set; }
    }
    public class ResLangKeyListParam
    {
        public string language { get; set; }
        public string purpose { get; set; }
    }
    public class ResLangKeyListInfo
    {
        public List<ResLanguageList> lstLanguage { get; set; }
        public List<ResKeysList> lstResKeys { get; set; }
        public string IsSignUpAllowed { get; set; }
        public string policies { get; set; }
        public string TermsAndConditions { get; set; }
        public int isPatternLockEnable { get; set; }
    }
    public class ResLanguageList
    {
        public int id { get; set; }
        public string language { get; set; }
        public string culture { get; set; }
        public string font { get; set; }
        public bool is_active { get; set; }
        public bool is_active_for_mobile { get; set; }
        public bool is_active_for_web { get; set; }
    }
    public class ResKeysList
    {
        public int id { get; set; }
        public string language { get; set; }
        public string culture { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public bool is_jquery_used { get; set; }
    }

    public class ResourcesKeyStatus
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string key { get; set; }
        public bool? status { get; set; }
        public string source_name { get; set; }

        public ResourcesKeyStatus()
        {

        }
        public int created_by { get; set; }
        public DateTimeOffset created_on { get; set; }


    }

}

