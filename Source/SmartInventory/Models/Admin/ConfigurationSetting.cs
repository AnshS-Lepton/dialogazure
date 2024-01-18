using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Admin
{

    public class ConfigurationSetting
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int system_id { get; set; }
        //[Required]
        //public string project_name { get; set; }
        //[Required]
        //public string project_code { get; set; }
        //[Required(ErrorMessage = "Required.")]
        //public string planing_name { get; set; }
        //[Required(ErrorMessage = "Required.")]
        //public string planing_code { get; set; }
        //[Required(ErrorMessage = "Required.")]
        //public string purpose_name { get; set; }
        //[Required(ErrorMessage = "Required.")]
        //public string purpose_code { get; set; }
        //[Required(ErrorMessage = "Required.")]
        //public string workorder_name { get; set; }
        //[Required(ErrorMessage = "Required.")]
        //public string workorder_code { get; set; }

        //public string network_stage { get; set; }

        //public int created_by { get; set; }
        ////public DateTime created_on { get; set; }
        //public int? modified_by { get; set; }
        //public DateTime? modified_on { get; set; }

        public string entity_type { get; set; }
        public string equipment_Type { get; set; }
        public string brand_type { get; set; }
        public string model_type { get; set; }




        [NotMapped]
        public PageMessage pageMsg { get; set; }



        [NotMapped]
        public List<KeyValueDropDown> lstBindIspEntityType { get; set; }

        //[NotMapped]
        //public List<EntityTypeISPMaster> lstBindIspEntityType { get; set; }
        [NotMapped]
        public List<EquipmentTypeMaster> lstBindIspEquipmentType { get; set; }
        [NotMapped]
        public List<BrandTypeMaster> lstBindIspBrand { get; set; }
        [NotMapped]
        public List<ModelTypeMaster> lstBindIspModel { get; set; }
       


        public ConfigurationSetting()
        {
            pageMsg = new PageMessage();
            lstBindIspEntityType = new List<KeyValueDropDown>();
            lstBindIspEquipmentType = new List<EquipmentTypeMaster>();
            lstBindIspBrand = new List<BrandTypeMaster>();
            lstBindIspModel = new List<ModelTypeMaster>();

        }


        //[NotMapped]
        //public ProjectCodeMaster saveProjectCodeDetail { get; set; }

        //[NotMapped]
        //public PlanningCodeMaster savePlanningCodeDetail { get; set; }

        //[NotMapped]
        //public WorkorderCodeMaster saveWorkorderCodeDetail { get; set; }


        //[NotMapped]
        //public PurposeCodeMaster savePurposeCodeDetail { get; set; }

    }


     

    public class EquipmentTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string type { get; set; }
    
        public int? created_by { get; set; }
        
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        public int entity_id { get; set; }

        [NotMapped]
        public string entity_type { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public EquipmentTypeMaster()
        {
            pageMsg = new PageMessage();

        }

    }
  
    public class BrandTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        public string brand { get; set; }

        public int type_id {get; set;}

        public int? created_by { get; set; }

        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }


        [NotMapped]
        public string equipment_type { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public BrandTypeMaster()
        {
            pageMsg = new PageMessage();

        }

    }


    public class ModelTypePortMaster
    {
        public ModelTypeMaster modelTypeMaster { get; set; }
        public List<PortInfo> lstPortInfo { get; set; }
        public List<PortInfo> lstOutputPortInfo { get; set; }
      

        public ModelTypePortMaster()
        {
            modelTypeMaster = new ModelTypeMaster();
            lstPortInfo = new List<PortInfo>();
            lstOutputPortInfo = new List<PortInfo>();
          

        }


    }



    public class ModelTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        public string model { get; set; }

        public int brand_id {get; set;}

        public int? created_by { get; set; }

        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        public int power_supply { get; set; }
        public double device_width { get; set; }
        public double device_length { get; set; }
        public double device_height { get; set; }

        public string inputport { get; set; }
        public string outputport { get; set; }

        [NotMapped]
        public string brand_type { get; set; }

        //[NotMapped]
        //public List<InputPortInfo> lstInputInfo { get; set; }

        //[NotMapped]
        //public List<OuputPortInfo> lstOutputPortInfo { get; set; }

          //[NotMapped]
          //public List<PortInfo> lstPortInfo { get; set; }


        


        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public ModelTypeMaster()
        {
            pageMsg = new PageMessage();
            //lstInputInfo = new List<InputPortInfo>();
            //lstOutputPortInfo = new List<OuputPortInfo>();

            //lstPortInfo = new List<PortInfo>();
          
        }

    }


    public class PortInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

          public int model_id { get; set; }

          public int port_no { get; set; }

         public string port_type { get; set; }
        
        public string input_output { get; set; }
   
        public string  port_name { get; set; }
        public int? created_by { get; set; }

        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

         [NotMapped]
        public List<SelectListItem> lstPort_type { get; set; }

        public PortInfo()
        {
            id = 0;

            lstPort_type = new List<SelectListItem>{
                                new SelectListItem{ Text="GEO", Value = "GEO" },
                                new SelectListItem{ Text="PON", Value = "PON" }};

        }


    }




    public class InputPortInfo
    {
        public int id { get; set; }
        public string InputPort_name { get; set; }
        public string InputPort_type { get; set; }
      
        public double length { get; set; }
        public double width { get; set; }

        
        public List<SelectListItem> lstInputPort_type { get; set; }

        public InputPortInfo()
        {
            id = 0;
            length = 0;
            width = 0;


            lstInputPort_type = new List<SelectListItem>{
                                new SelectListItem{ Text="GEO", Value = "1" },
                                new SelectListItem{ Text="PON", Value = "2" }}; 
            
        }

       
    }

    public class OuputPortInfo
    {
        public int id { get; set; }
        public string OutputPort_name { get; set; }
        public string OutputPort_type { get; set; }
       
        public double length { get; set; }
        public double width { get; set; }


        public List<SelectListItem> lstOutputPort_type { get; set; }

        public OuputPortInfo()
        {
            id = 0;
            length = 0;
            width = 0;

            lstOutputPort_type = new List<SelectListItem>{
                                new SelectListItem{ Text="GEO", Value = "1" },
                                new SelectListItem{ Text="PON", Value = "2" }}; 

        }
    }


 
}
