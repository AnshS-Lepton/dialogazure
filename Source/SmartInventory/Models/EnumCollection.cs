using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public enum fileExtensions
    {
        pdf,
        zip,
        png,
        jpg,
        jpeg
    }
    public enum StatusCodes
    {
        OK,
        PARTIAL_SUCCESS,
        ZERO_RESULTS,
        OVER_QUERY_LIMIT,
        REQUEST_DENIED,
        INVALID_REQUEST,
        UNKNOWN_ERROR,
        INVALID_INPUTS,
        SESSION_EXPIRED,
        DUPLICATE_EXIST,
        VALIDATION_FAILED,
        INVALID_FILE,
        FAILED,
        EXCEPTION,
        ACCEPTED,
        REJECTED,
        REQUIRED 
    }
    public enum EntityType
    {
        ADB,
        CDB,
        BDB,
        Building,
        Area,
        SubArea,
        SurveyArea,
        RestrictedArea,
        Structure,
        POD,
        Pole,
        Tree,
        Splitter,
        Manhole,
        SpliceClosure,
        Cable,
        MPOD,
        Customer,
        ONT,
        Trench,
        Duct,
        WallMount,
        //Room,
        HTB,
        FDB,
        UNIT,
        FMS,
        ProjectArea,
        Coupler,
        ROW,
        PatchCord,
        Equipment,
        DSA,
        CSA,
        Rack,
        Floor,
        SHAFT,
        PIT,
        Loop,
        Fault,
        Competitor,
        Tower,
        Antenna,
        Sector,
        MicrowaveLink,
        Network_Ticket,
        Province,
        Region,
        LandBase,
        Cabinet, //cabinet shazia 		
        SpliceTray,
        Vault,
        Microduct,
        Conduit,
        OpticalRepeater,
        ODF,
        Handhole,
        PatchPanel,
        VSAT,
        Gipipe,
        FAT,
        ATB,
        FDC,
        Slack
    }

    public enum EntityAction
    {
        Get,
        Insert,
        Update,
        Delete,
        Report,
        Save
    }
    public enum EntityCategory
    {
        Primary,
        Secondary
    }

    public enum LandBaseEntity
    {
        Buildings
    }
    public enum LayerSettingType
    {
        History,
        Info,
        Report,
        Search
    }

    public enum DropDownType
    {
        Specification,
        TypeMaster,
        Vendor,
        Activation,
        Construction,
        Accessibility,
        Brand,
        Model,
        Tenancy,
        Category,
        RFS_Status,
        Media,
        Building_Type,
        Pole_Type,
        Splitter_Ratio,
        Tree_Type,
        Fiber_Count,
        Activation_Status,
        Trench_Type,
        BDB_PORT_RATIO,
        Adb_Port_Ratio,
        Cdb_Port_Ratio,
        Htb_Port_Ratio,
        Fdb_Port_Ratio,
        Ont_Port_Ratio,
        SC_Port_Ratio,
        FMS_Port_Ratio,
        Survey_Status,
        SurveyUser,
        Building_Status,
        Equipement_Core,
        Cable_Category,
        Cable_Subcategory,
        Cable_Status,
        Area_RFS,
        RestrictedArea_RFS,
        SubArea_RFS,
        Duct_Type,
        Duct_Color,
        Gipipe_Type,
        Gipipe_Color,
        Pipipe_Color,
        MCGM_Ward,
        Strata_Type,
        Surface_Type,
        Execution_Method,
        Construction_Type,
        Reference_Direction,
        Coupler,
        Cable_Type,
        Ownership,
        POD_Type,
        MPOD_Type,
        ROW_Type,
        ROW_Width,
        PatchCord_Category,
        PatchCord_Subcategory,
        PatchCord_Type,
        Ticket_Type,
        Reference_Type,
        AT_Status,
        type_of_activity_charge,
        charge_category,
        Customer_Type,
        Customer_Service_Type,
        Export_Report,
        LMC_TYPE,
        SITE_TYPE,
        Structure_Type,
        Electrical_Meter_Type,
        Cable_Entry_Point,
        Circle,
        Structure_Size,
        LMC_Cascaded_Standalone,
        RTN_Building_Side_Tapping_Point,
        Termination_Details,
        SubCategory,
        RejectComment,
        Fault_Ticket_Type,
        Fault_Status,
        Entity_Type,
        Business_Type,
        BlockNotification,
        UnBlockNotification,
        Antenna_Type,
        Antenna_Sub_Type,
        Antenna_Operator,
        Use_Pattern,
        Downlink,
        Uplink,
        SectorType,
        Frequency,
        Network_Ticket,
        LinkType,
        Network_Status,
        Microduct_Type,
        conduit_Type,
        Conduit_Color,
        OpticalRepeater_Port_Ratio,
        Amplifier_Type,
        FiberLinkType,
        Rack_Type,
        Cabinet_Type,//cabinet shazia
        Vault_Type,
        no_of_entry_exit_points,
        NWTRejectComment,
        PatchPanel_Port_Ratio,
        PatchPanel_type,
        VSAT_Category_Type,
        VSAT_Service_Type,
        Manhole_types,
        VSAT_Transmission_Type,
        VSAT_Return_Type,
        VSAT_Setellite_Type,
        VSAT_Transpond_Type,
        VSAT_RF_Band_Type,
        Number_of_Ways,
        Internal_Diameter,
        External_Diameter,
        Material_Type,
        JO_assign_status,
        JO_view_status,
        bom_sub_category,
        served_by_ring,
        trench_serving_type,
        QualificationType,
        ddlNetworkStatus,
        UserType,
        Splitter_Type,
        CSA_RFS,
        No_of_Ducts_Created,
        Duct_Count
    }
    public enum VendorType
    {
        Own,
        ThirdParty
    }
    public enum GeometryType
    {
        Point,
        Line,
        Polygon,
        Circle
    }
    public enum EntityActions
    {
        Addition,
        Modification,
        Updation,
        Deletion,
        SplicingManual,
        SplicingAuto
    }
    public enum NetworkIdType
    {
        A,  // auto
        M  // Manual
    }

    public enum BuildingStatus
    {
        New,
        Surveyed,
        Resurveyed,
        Approved
    }
    public enum BuildingAction
    {
        Save,
        Update,
        Reject,
        Approve,

    }

    public enum ResponseStatus
    {
        OK,
        ZERO_RESULTS,
        ERROR,
        VALIDATION_FAILED,
        FAILED
    }

    public enum NetworkStatus
    {
        P, //Planned
        A, //As -built
        D //Dorment
    }

    public enum BuildingTenancy
    {
        Single,
        Multiple,
        None
    }
    public enum PortStatus
    {
        Vacant,
        Connected
    }
    public enum UnitInputType
    {
        port,
        iop,
        iopddl
    }
    public enum InOutPortType
    {
        O,
        I
    }



    public enum formFeatureName
    {
        start_end_reading,
        sub_category,
        duct_type,
        duct_color,
        inner_outer_dimension,
        strata_type,
        mcgm_ward,
        manufacture_year,
        surface_type,
        construction_type,
        execution_method,
        with_riser,
        conduit_type,
        conduit_color,
        no_of_tube,
        no_of_core_per_tube,
        cost_per_unit,
        service_cost_per_unit,
        specification,
        prms_id,
        vendor,
        gipipe_type,
        gipipe_color,
    }
    public enum formFeatureType
    {
        feature,
        field,
        tab,
        field_set
    }
    public enum SplicingType
    {
        ODF2CABLE,
        CABLE2CABLE,
        CPE2CUSTOMER
    }
    public enum CorePortStatus
    {
        Faulty,
        Vacant,
        Connected,
        Reserved
    }
    public enum RFSTypes
    {
        ARFS,
        BRFS,
        CRFS
    }

    public enum CableTypes
    {
        ISP,
        Overhead

    }
    public enum UserType
    {
        Mobile,
        Web,
        Crm,
        Other
    }

    public enum ApplicationUserType
    {
        Mobile,
        Web,
        Both
    }
    public enum BuildingCategory
    {
        Vacant,
        Residential,
        Commercial,
        Both
    }
    public enum ROWStage
    {
        Apply,
        Approved,
        Rejected
    }
    namespace Extension
    {
        public static class ExtensionMethods
        {
            public static string EnumValue(this RFSTypes e)
            {
                switch (e)
                {
                    case RFSTypes.ARFS:
                        return "A-RFS";
                    case RFSTypes.BRFS:
                        return "B-RFS";
                    case RFSTypes.CRFS:
                        return "C-RFS";
                }
                return "Horrible Failure!!";
            }
        }
    }
    public enum notificationType
    {
        Utilization,
        Upload,
        PrintMap
    }

    #region Additional-Attributes
    public enum DynamicControlsType
    {
        TEXT,
        DROPDOWN,
        DATETIME,
        LABEL
    }
    #endregion

    #region OTP RESET TYPE
    public enum OTPResetType
    {
        RESET_RESEND_LIMIT_REACHED,
        RESET_RESEND_OTP_FAILED
    }
    #endregion

    #region Bom_boq_category
    public enum Bom_boq_category
    {
        Proposed,
        Existing
    }
    #endregion
}
