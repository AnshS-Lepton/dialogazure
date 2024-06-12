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
    public class ROWMaster: IProjectSpecification,IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
       
        public string row_name { get; set; }
        public string row_stage { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string remarks { get; set; }
        public int sequence_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        public string geom_type { get; set; }
        [Required]
        public string row_type { get; set; }
        [NotMapped]
        public string centerLineGeom { get; set; }
        [NotMapped]
        public decimal buffer_width { get; set; }
        [NotMapped]
        public bool isPITApplied { get; set; }
        [NotMapped]
        public bool isROWApplied { get; set; }
        [NotMapped]
        public bool is_approve { get; set; }
        [NotMapped]
        public double pit_default_radius { get; set; }
        [NotMapped]
        public EntityMaintainenceChargesList MaintainenceCharges { get; set; }
        [NotMapped]
        public bool isassociated { get; set; }

        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }
        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
        public string status_remark { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        [NotMapped]
        public List<RouteInfo> lstRouteInfo { get; set; }
        public ROWMaster()
        {
            project_id = 0;
            planning_id = 0;
            workorder_id = 0;
            purpose_id = 0;
               objPM = new PageMessage();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            lstUserModule = new List<string>();
            lstRouteInfo = new List<RouteInfo>();
        }

    }
    public class rowApplyStage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int row_system_id { get; set; }
        [Required]
        public string road_name { get; set; }
        public DateTime apply_date { get; set; }
        public double route_length { get; set; }
        [Required]
        public int no_of_hh { get; set; }
        [Required]
        public double supervision { get; set; }
        [Required]
        public double ri_rate { get; set; }
        [Required]
        public double ri_amount { get; set; }
        public double refundable_amount { get; set; }
        public double non_refundable_amount { get; set; }
        [Required]
        public double local_management { get; set; }
        [Required]
        public double dit_liaisoning { get; set; }
        [Required]
        public double row_liaisoning { get; set; }
        [Required]
        public double otm_liasoning { get; set; }
        [Required]
        public double autocad_drawing { get; set; }
        public double cl_rly_fly_brg_laisioning { get; set; }
        public double cl_ft_brg_laisioning { get; set; }
        [Required]
        public double total_amount { get; set; }
        public string start_point { get; set; }
        public string end_point { get; set; }
        [Required]
        public string mcgm_ward { get; set; }

        public double applied_pit { get; set; }
        [Required]
        public double applied_length { get; set; }
        [Required]
        public double access_charges { get; set; }
        [Required]
        public double considered_dlp_factor { get; set; }
        [Required]
        public string starta_type { get; set; }
        [Required]
        public double budget_material_cost { get; set; }
        [Required]
        public double budget_contractor_cost { get; set; }
        [Required]
        public int no_of_pulled_cables { get; set; }
        public double pit_charges { get; set; }
        public double radius { get; set; }
        public double row_area { get; set; }
        [Required]
        public double cable_pulling_rate { get; set; }
        [Required]
        public double bdotm_otm_trenching_ducting_rate { get; set; }
        [Required]
        public double bdotm_otm_trenching_ducting_length { get; set; }
        [Required]
        public double bdotm_otm_ri_rate { get; set; }
        [Required]
        public double bdotm_otm_ri_length { get; set; }
        [Required]
        public double bdotm_otm_liasioning_rate { get; set; }
        [Required]
        public double bdotm_otm_liasioning_length { get; set; }
        [Required]
        public double bdotl_otl_trenching_ducting_rate { get; set; }
        [Required]
        public double bdotl_otl_trenching_ducting_length { get; set; }
        [Required]
        public double bdotl_otl_ri_rate { get; set; }
        [Required]
        public double bdotl_otl_ri_length { get; set; }

        public double cl_rly_fly_brg_length { get; set; }

        public double cl_ft_brg_length { get; set; }
        [Required]
        public double handhole_rate { get; set; }

        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public string geomType { get; set; }
        [NotMapped]
        [Required]
        public string applyDate { get; set; }
        [NotMapped]
        public string rowType { get; set; }
        [NotMapped]
        public List<ROWAttachments> fileList { get; set; }
        [NotMapped]
        public List<ROWRemarks> remarksList { get; set; }
        [NotMapped]
        public List<attachment_type_master> listAttachmentType { get; set; }
        [NotMapped]
        public List<DropDownMaster> listRowAuthority { get; set; }
        [NotMapped]
        public List<ROWChargesTemplate> ChargesTemplates { get; set; }
        [NotMapped]
        public string ChargeTemplate { get; set; }
        public rowApplyStage()
        {
            objPM = new PageMessage();
            fileList = new List<ROWAttachments>();
            remarksList = new List<ROWRemarks>();
            listAttachmentType = new List<attachment_type_master>();
            listRowAuthority = new List<DropDownMaster>();
            ChargesTemplates = new List<ROWChargesTemplate>();
        }
    }
    public class rowApproveRejectStage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int row_system_id { get; set; }
        [Required]
        public int demand_note { get; set; }
        [Required]
        public string row_status { get; set; }
        [Required]
        public double actual_row_route { get; set; }
        [Required]
        public double dlp_factor { get; set; }
        [Required]
        public double actual_rate { get; set; }
        [Required]
        public double actual_deposit_amount { get; set; }
        [Required]
        public double actual_ri_amount { get; set; }
        [Required]
        public double actual_access_charge_amount { get; set; }
        [Required]
        public double actual_refundable_amount { get; set; }

        public double penalty_amount { get; set; }

        public double total_paid_amount { get; set; }

        public double total_amount { get; set; }

        public DateTime? payment_date { get; set; }

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }
        [Required]
        public int row_window { get; set; }

        public DateTime? access_charges_start_date { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }
        public double total_recurring_charge { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listRowStatus { get; set; }
        [NotMapped]
        public IList<DropDownMaster> listRowStartaType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public string geomType { get; set; }
        [NotMapped]
        public string rowType { get; set; }
        [NotMapped]
        public List<ROWAttachments> fileList { get; set; }
        [NotMapped]
        public List<ROWRemarks> remarksList { get; set; }
        [NotMapped]
        [Required]
        public string startDate { get; set; }
        [NotMapped]
        [Required]
        public string endDate { get; set; }
        [NotMapped]
        [Required]
        public string accessChargeStartDate { get; set; }
        [NotMapped]
        [Required]
        public string paymentDate { get; set; }
        [NotMapped]
        public bool isROWApplied { get; set; }
        [NotMapped]
        public double route_length { get; set; }
        [NotMapped]
        public List<attachment_type_master> listAttachmentType { get; set; }
        public rowApproveRejectStage()
        {
            objPM = new PageMessage();
            fileList = new List<ROWAttachments>();
            remarksList = new List<ROWRemarks>();
            startDate = null;
            endDate = null;
            accessChargeStartDate = null;
            paymentDate = null;
            payment_date = null;
            start_date = null;
            end_date = null;
            access_charges_start_date = null;
            listAttachmentType = new List<attachment_type_master>();

        }
    }
    public class ROWPIT:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string geomType { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        public string network_id { get; set; }
        public int sequence_id { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public ROWPIT()
        {
            objPM = new PageMessage();
        }
    }
    public class ROWDetails
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
    }
    public class ROWChargesTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string template_name { get; set; }
        public double supervision { get; set; }
        public double local_management { get; set; }
        public double dit_liaisoning { get; set; }
        public double row_liaisoning { get; set; }
        public double otm_liasoning { get; set; }
        public double autocad_drawing { get; set; }
        public double cl_rly_fly_brg_laisioning { get; set; }
        public double cl_ft_brg_laisioning { get; set; }
        public double budget_material_cost { get; set; }
        public double budget_contractor_cost { get; set; }
        public double cable_pulling_rate { get; set; }
        public double bdotm_otm_trenching_ducting_rate { get; set; }
        public double bdotm_otm_ri_rate { get; set; }
        public double bdotm_otm_liasioning_rate { get; set; }
        public double bdotl_otl_trenching_ducting_rate { get; set; }
        public double bdotl_otl_ri_rate { get; set; }
        public double handhole_rate { get; set; }
        public double pit_charges { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
    }
    public class ROWArea
    {
        public double pit_area { get; set; }
        public double row_length { get; set; }
        public int total_pit { get; set; }
    }
    public class ROWRemarks
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string row_stage { get; set; }
        public int row_system_id { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string remarks { get; set; }
    }
    public class ROWAssociation
    {
        public int parent_system_id { get; set; }
        public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
        public List<ROWAssociateEntityList> entityList { get; set; }
        public PageMessage pageMsg { get; set; }
        public bool isExportEnabled { get; set; }
        public ROWAssociation()
        {
            entityList = new List<ROWAssociateEntityList>();
            pageMsg = new PageMessage();
        }
    }
    public class ROWAssociateEntityList
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string entity_type { get; set; }
        public string geom_type { get; set; }
        public string geom { get; set; }
        public string associated_on { get; set; }
        public string associated_by { get; set; }
        public bool is_associated { get; set; }
    }


    public class ROWStageRecordCount
    {
        public int TotalAppliedRecords { get; set; }
        public int TotalApprovedRecords { get; set; }
        public int TotalRejectedRecords { get; set; }
        public int TotalAssociatedRecords { get; set; }
        public int TotalNewRecords { get; set; }
    }
    public class ROWAuthority
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string authority { get; set; }
        public bool is_active { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
    }
}
