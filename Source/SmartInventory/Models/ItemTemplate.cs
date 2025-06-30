using Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{

    public class itemMaster
    {
        //ITEM SPECIFICATIONS
        [Required]
        public string specification { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public virtual int no_of_input_port { get; set; }
        public virtual int no_of_output_port { get; set; }
        public virtual int no_of_port { get; set; }
        public virtual string unit { get; set; }
        public virtual string other { get; set; }
        //  public virtual int width { get; set; }
        //   public virtual string rowtype { get; set; }

        [Required]
        public int vendor_id { get; set; }
        [Required]

        public string item_code { get; set; }
        [NotMapped]
        public string unitValue { get; set; }
        //MODEL MAPPING      
        public int type { get; set; }
        public int brand { get; set; }
        public int model { get; set; }
        public int construction { get; set; }
        public int activation { get; set; }
        public int accessibility { get; set; }
        public string hierarchy_type {get; set;}

        [NotMapped]
        public List<KeyValueDropDown> lstSpecification { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstType { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> listType { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstBrand { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstModel { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstAccessibility { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstActivation { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstConstruction { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lsthierarchytype { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstVendor { get; set; }
        [NotMapped]
        public string unit_input_type { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstIOPDetails { get; set; }
        [NotMapped]
        public virtual bool isTemplateFilled { get; set; }
        [NotMapped]
        public bool isPortConnected { get; set; }
        [NotMapped]
        public string message { get; set; }
        [NotMapped]
        public List<FormInputSettings> formInputSettings { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstEntityType { get; set; }
        public int? audit_item_master_id { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstNoOfWays { get; set; }
        //public string specify_type { get; set; }
        public itemMaster()
        {
            specification = "";
            lstSpecification = new List<KeyValueDropDown>();
            lstType = new List<KeyValueDropDown>();
            listType = new List<KeyValueDropDown>();
            lstBrand = new List<KeyValueDropDown>();
            lstModel = new List<KeyValueDropDown>();
            lstAccessibility = new List<KeyValueDropDown>();
            lstActivation = new List<KeyValueDropDown>();
            lstConstruction = new List<KeyValueDropDown>();
            lstVendor = new List<KeyValueDropDown>();
            lstEntityType = new List<DropDownMaster>();
            formInputSettings=new List<FormInputSettings>();

        }
    }

    public class ADBTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR ADB FORM AND TEMPLATE WILL BE THERE.      
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [Required]
        public string entity_category { get; set; }
    }

    public class ADBItemMaster : ADBTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR ADB TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public ADBItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class CDBTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR CDB FORM AND TEMPLATE WILL BE THERE...       
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [Required]
        public string entity_category { get; set; }
    }

    public class CDBItemMaster : CDBTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR CDB TEMPLATE ONLY WILL BE THERE
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }

        [NotMapped]
        public int userId { get; set; }
        public CDBItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class BDBTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR BDB FORM AND TEMPLATE WILL BE THERE...
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [Required]
        public string entity_category { get; set; }
    }

    public class BDBItemMaster : BDBTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR CDB TEMPLATE ONLY WILL BE THERE
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }

        [NotMapped]
        public int userId { get; set; }
        public BDBItemMaster()
        {
            objPM = new PageMessage();
        }
    }


    public class PODTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR POD FORM AND TEMPLATE WILL BE THERE.
        public string pod_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> listPODType { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class PODItemMaster : PODTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR POD TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public PODItemMaster()
        {
            objPM = new PageMessage();
        }
    }


    public class TreeTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR Tree FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class TreeItemMaster : TreeTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR Tree TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public TreeItemMaster()
        {
            objPM = new PageMessage();
        }
    }




    public class itemCategory
    {
        public string category { get; set; }
        public string subCategory_1 { get; set; }
        public string subCategory_2 { get; set; }
        public string subCategory_3 { get; set; }
        public string code { get; set; }
        public int no_of_input_port { get; set; }
        public int no_of_output_port { get; set; }
        public int no_of_port { get; set; }
        public string unit { get; set; }
        public string other { get; set; }
        public int item_template_id { get; set; }
        public int? audit_item_master_id { get; set; }
        public string specify_type { get; set; }
        public int no_of_tube { get; set; }
        public int no_of_core_per_tube { get; set; }

    }

    public class PoleTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR POLE FORM AND TEMPLATE WILL BE THERE...
        [Required]
        public string pole_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstPoleType { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [NotMapped]
        public  string is_specification_allowed { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstImageUpload { get; set; }

    }
    public class PoleItemMaster : PoleTemplateMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public PoleItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class ManholeTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR Manhole FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        public string manhole_types { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstManholeType { get; set; }
        [NotMapped]
        public string is_specification_allowed { get; set; }
    }

    public class ManholeItemMaster : ManholeTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR Manhole TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public ManholeItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class CouplerTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR Manhole FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class CouplerItemMaster : CouplerTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR Manhole TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        public CouplerItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class SCTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SC FORM AND TEMPLATE WILL BE THERE.       
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        public int no_of_ports { get; set; }
    }


    public class FMSTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR FMS FORM AND TEMPLATE WILL BE THERE.       
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class FMSItemMaster : FMSTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR FMS TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        public FMSItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class SCItemMaster : SCTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SC TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public SCItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class SplitterTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR Splitter FORM AND TEMPLATE WILL BE THERE.
        [Required]
        public string splitter_ratio { get; set; }
        [Required]
        public string splitter_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstSplRatio { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [NotMapped]
        public override bool isTemplateFilled { get; set; }
    }

    public class SplitterItemMaster : SplitterTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR Splitter TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        public SplitterItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class MPODTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR MPOD FORM AND TEMPLATE WILL BE THERE.
        public string mpod_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> listMPODType { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class MPODItemMaster : MPODTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR MPOD TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public MPODItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class CableTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SPLT FORM AND TEMPLATE WILL BE THERE.
        [Required]
        public int total_core { get; set; }
        [Required]
        public int no_of_tube { get; set; }
        [Required]
        public int no_of_core_per_tube { get; set; }
        [Required]
        public string cable_type { get; set; }
        public string aerial_location { get; set; }
        public string cable_category { get; set; }
        public string cable_sub_category { get; set; }
        [NotMapped]
        public string fiberCountIn { get; set; }
        [NotMapped]
        public IList<DropDownMaster> fiberCount { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listcableType { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listaerialLocation { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listcableCategory { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listcableSubCategory { get; set; }

    }

    public class CableItemMaster : CableTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public string cblType { get; set; }
        public CableItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class ONTTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR ONT FORM AND TEMPLATE WILL BE THERE. 
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class ONTItemMaster : ONTTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR ONT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }

        [NotMapped]
        public int user_id { get; set; }

        public ONTItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class TrenchTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SPLT FORM AND TEMPLATE WILL BE THERE.
        [Required(ErrorMessage = "Trench width can't be blank")]
        //[Range(1, int.MaxValue, ErrorMessage = "Trench width greater than 0")]
        public double trench_width { get; set; }
        [Required(ErrorMessage = "Trench height can't be blank")]
        //[Range(1, int.MaxValue, ErrorMessage = "Trench height greater than 0")]
        public double trench_height { get; set; }       
        public string trench_type { get; set; }
        [NotMapped]
        public IList<DropDownMaster> trenchTypeIn { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [Required]
        public string trench_serving_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstTrenchServingType { get; set; }
    }

    public class TrenchItemMaster : TrenchTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_Id { get; set; }
        public TrenchItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class DuctTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SPLT FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class DuctItemMaster : DuctTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_Id { get; set; }
        public DuctItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class MicroductTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SPLT FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [NotMapped]
        public string no_of_ways { get; set; }
    }

    public class MicroductItemMaster : MicroductTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_Id { get; set; }
        

        public MicroductItemMaster()
        {
            objPM = new PageMessage();
            lstNoOfWays = new List<DropDownMaster>();
        }
    }
    


    public class ConduitTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SPLT FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class ConduitItemMaster : ConduitTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public ConduitItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class WallMountTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR WallMount FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class WallMountItemMaster : WallMountTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR WallMount TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int UserId { get; set; }
        public WallMountItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class PEPTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR PEP FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class PEPItemMaster : PEPTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR PEP TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int UserId { get; set; }
        public PEPItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    /// <summary>
    /// ================================  ISP TEMPLATES     =============================================================
    /// </summary>
    public class RoomTemplate : RoomTemplateMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string status { get; set; }
        public string template_name { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public RoomTemplate()
        {
            objPM = new PageMessage();
        }
    }

    public class RoomTemplateMaster
    {

        //[Required(ErrorMessage = "Please enter the room type.")]
        public string room_type { get; set; }
        [Required(ErrorMessage = "Please enter the unit height.")]
        [Range(1, int.MaxValue, ErrorMessage = "Height should be greater than 1(m)!")]
        public double room_height { get; set; }
        [Required(ErrorMessage = "Please enter the unit width.")]
        [Range(1, int.MaxValue, ErrorMessage = "Width should be greater than 1(m)!")]
        public double room_width { get; set; }
        [Required(ErrorMessage = "Please enter the unit length.")]
        [Range(1, int.MaxValue, ErrorMessage = "Length should be greater than 1(m)!")]
        public double room_length { get; set; }
        public string unitno { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstUnitType { get; set; }
        [Required(ErrorMessage = "Please select unit type")]
        public string unit_type { get; set; }
        [Required(ErrorMessage = "Please enter the area of unit!")]
        //[MinValue]
        [Range(1, int.MaxValue, ErrorMessage = "Area should be greater than 1(m<sup>2</sup>)!")]
        public double area { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        public RoomTemplateMaster()
        {
            //room_length = 1;
            //room_width = 1;
            //room_height = 1;
        }
    }

    public class HTBTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR HTB FORM AND TEMPLATE WILL BE THERE...
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }

    }
    public class HTBTemplate : HTBTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR HTB TEMPLATE ONLY WILL BE THERE
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string status { get; set; }
        public string template_name { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public HTBTemplate() { objPM = new PageMessage(); }
        [NotMapped]
        public int userId { get; set; }

    }

    public class OpticalRepeaterTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR HTB FORM AND TEMPLATE WILL BE THERE...
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }

    }
    public class OpticalRepeaterTemplate : OpticalRepeaterTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR HTB TEMPLATE ONLY WILL BE THERE
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string status { get; set; }
        public string template_name { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public OpticalRepeaterTemplate() { objPM = new PageMessage(); }
        [NotMapped]
        public int userId { get; set; }

    }

    public class FDBTemplate : FDBTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR HTB TEMPLATE ONLY WILL BE THERE
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string status { get; set; }
        public string template_name { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        public FDBTemplate()
        {
            objPM = new PageMessage();
        }
    }
    public class FDBTemplateMaster : itemMaster
    {
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }


    #region ProjectSpeicficationMapping
    public class ProjectSpecificationMaster : ListDropDownForProjSpecification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string project_name { get; set; }
        [Required]
        public string project_code { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string planning_name { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string planning_code { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string purpose_name { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string purpose_code { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string workorder_name { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string workorder_code { get; set; }

        public string network_stage { get; set; }

        public int created_by { get; set; }
        //public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public ProjectSpecificationMaster()
        {
            pageMsg = new PageMessage();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<KeyValueDropDown>();
            lstBindWorkorderCode = new List<KeyValueDropDown>();
            lstBindPurposeCode = new List<KeyValueDropDown>();

        }

    }

    public class ListDropDownForProjSpecification
    {
        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindPlanningCode { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindWorkorderCode { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindPurposeCode { get; set; }

    }
    #endregion

    #region ROW
    public class ROWItemMaster : ROWTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR ADB TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }

        public ROWItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class ROWTemplateMaster
    {
        [Required(ErrorMessage = "Required.")]
        public string type { get; set; }
        [Required(ErrorMessage = "Required.")]
        public double? width { get; set; }

        [NotMapped]
        [Range(0.1, double.MaxValue, ErrorMessage = "Please enter a value bigger than {0}")]
        public double? customized { get; set; }
        [NotMapped]
        public IList<DropDownMaster> rowtypelist { get; set; }
        [NotMapped]
        public IList<DropDownMaster> rowwidthlist { get; set; }
        [NotMapped]
        public bool isCustomWidthEnable { get; set; }
    }

    #endregion

    public class PatchCordTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SPLT FORM AND TEMPLATE WILL BE THERE.        
        public string patch_cord_category { get; set; }
        public string patch_cord_sub_category { get; set; }
        [NotMapped]
        public IList<DropDownMaster> fiberCount { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listPatchCordCategory { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listPatchCordSubCategory { get; set; }

    }
    public class PatchCordItemMaster : PatchCordTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public PatchCordItemMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class ISPModelTemplateMaster : itemMaster { }

    public class ISPModelTemplate : ISPModelTemplateMaster { }

    public class EquipmentTemplateMaster : itemMaster
    {
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        //[NotMapped]
        //public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }
    public class PITTemplateMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public double radius { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        public PITTemplateMaster()
        {
            objPM = new PageMessage();
        }
    }

    public class AccessoriesTemplateMaster : itemMaster { }

    public class AccessoriesTemplate : AccessoriesTemplateMaster { }


    //cabinet shazia
    public class CabinetTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR Cabinet FORM AND TEMPLATE WILL BE THERE.
        public string cabinet_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> listCabinetType { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class CabinetItemMaster : CabinetTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR Cabinet TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public CabinetItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    //cabinet shazia 

    //vault shazia
    public class VaultTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR Vault FORM AND TEMPLATE WILL BE THERE.
        public string vault_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> listVaultType { get; set; }
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class VaultItemMaster : VaultTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR Vault TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public VaultItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    //Vault shazia 


    //HANDHOLE ENTITY BY ANTRA

    public class HandholeTemplateMaster : itemMaster
    {
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class HandholeItemMaster : HandholeTemplateMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public HandholeItemMaster()
        {
            objPM = new PageMessage();
        }
    }


    public class PatchPanelTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR FMS FORM AND TEMPLATE WILL BE THERE.
        // [NotMapped]
        [NotMapped]
        public List<DropDownMaster> listPatchPanelType { get; set; }
        public override int no_of_input_port { get; set; }
        public override int no_of_output_port { get; set; }
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class PatchPanelItemMaster : PatchPanelTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR FMS TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        public PatchPanelItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class GipipeTemplateMaster : itemMaster
    {
        // ADDITION FIELD WHICH ARE COMMON FOR SPLT FORM AND TEMPLATE WILL BE THERE.
        [NotMapped]
        public override int no_of_input_port { get; set; }
        [NotMapped]
        public override int no_of_output_port { get; set; }
        [NotMapped]
        public override int no_of_port { get; set; }
        [NotMapped]
        public override string unit { get; set; }
        [NotMapped]
        public override string other { get; set; }
    }

    public class GipipeItemMaster : GipipeTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public GipipeItemMaster()
        {
            objPM = new PageMessage();
        }
    }




}
