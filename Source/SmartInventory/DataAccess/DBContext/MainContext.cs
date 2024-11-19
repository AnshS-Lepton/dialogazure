using iTextSharp.text.pdf.qrcode;
using Models;
using Models.Admin;
using Models.Feasibility;
using Models.ISP;
using Models.TempUpload;
//using Models.ISP;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Models.WFM;
using System.Configuration;
using System;
using Models.Dashboard;
using Utility;

namespace DataAccess.DBContext
{
    public class MainContext : DbContext
    {
        public DbSet<wfm_tt_rc> wfm_tt_rc { get; set; }
        public DbSet<wfm_tt_rca> wfm_tt_rca { get; set; }
        public DbSet<wfm_email_sms_log> wfm_email_sms_log { get; set; }
        public DbSet<wfm_notification_template> wfm_notification_template { get; set; }
        public DbSet<wfm_notification> wfm_notification { get; set; }
        public DbSet<SlotRequest> SlotRequest { get; set; }
        public DbSet<User_TimeSheet> user_timesheet { get; set; }
        public DbSet<Models.WFM.Task> task { get; set; }
        public DbSet<Route_Issue> Route_Issues { get; set; }
        public DbSet<Models.WFM.HPSM_Ticket_Master_History> HPSM_Ticket_Master_History { get; set; }
        public DbSet<Task_Tracking> Task_Tracking { get; set; }
        public DbSet<Vw_Hpsm_Ticket_Status> vw_Hpsm_Ticket_Status { get; set; }
        public DbSet<User_Master> user_Masters { get; set; }
        public DbSet<nmsticket> objNmsTicket { get; set; }
        public DbSet<VW_USER_MANAGER_RELATION> vw_user_manager_relation { get; set; }
        public DbSet<NOTIFICATION_ALERTS_HISTORY> Notification_Alerts_History { get; set; }

        public DbSet<VSF_TICKET_HISTORY> Vsf_Ticket_History { get; set; }
        public DbSet<Issue_Resolution_Type_Master> issue_resolution_type_master { get; set; }
        public DbSet<Circle_Master> circle_master { get; set; }
        public DbSet<Issue_Type_Master> Issue_Type_Master { get; set; }

        public DbSet<AdditionalMaterialMaster> AdditionalMaterialMaster { get; set; }

        public DbSet<AdditionalMaterial> AdditionalMaterial { get; set; }

        public DbSet<JoCategoryRoleMapping> JoCategoryRoleMapping { get; set; }

        public DbSet<ConnectedDevice> ConnectedDevice { get; set; }
        public DbSet<ConnectedDeviceRequest> ConnectedDeviceRequest { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<WfmRca> WfmRca { get; set; }

        public DbSet<wfm_jo_type_master> wfm_jo_type_master { get; set; }
        public DbSet<wfm_service_facility_master> wfm_service_facility_master { get; set; }
        //public DbSet<wfm_service_facility_master> ServiceFacilityMaster { get; set; }
        public DbSet<JoCategoryMaster> JoCategoryMaster { get; set; }
        //public DbSet<JoTypeMaster> JoTypeMaster { get; set; }
        public DbSet<TT_Customer_Category> TT_Customer_Category { get; set; }
        public DbSet<TT_Customer_Segment> TT_Customer_Segment { get; set; }
        public DbSet<TT_category> TT_category { get; set; }
        public DbSet<TT_Type> TT_Type { get; set; }
        public DbSet<PortManager> portmanagers { get; set; }
        public DbSet<UpdateNotificationLog> notificationLogs { get; set; }
        public DbSet<FatProcessSummary> FatProcessSummaryDetails { get; set; }
        public DbSet<Payment_Details> Payment_Details { get; set; }

        public DbSet<EmailEventTemplate> EmailEventTemplateList { get; set; }
        public DbSet<CorePlannerLogs> CorePlannerLogs { get; set; }


        //public DbSet<ExportReportLog> ExportReportLog { get; set; }
        public MainContext() : base("constr")
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]) == true)
                this.Database.Connection.ConnectionString = MiscHelper.Decrypt(ConfigurationManager.AppSettings["constr"].Trim());
        }

        public MainContext(string connectionStrring) : base("constr")
        {
            if(!string.IsNullOrEmpty(connectionStrring))
            { this.Database.Connection.ConnectionString = MiscHelper.Decrypt(connectionStrring); }
            else if (Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]))
                this.Database.Connection.ConnectionString = MiscHelper.Decrypt(ConfigurationManager.AppSettings["constr"].Trim());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            string dbschema = System.Configuration.ConfigurationManager.AppSettings["dbschema"];

            //dbschema = DbProvider.Name.Equals(Provider.Oracle) ? dbschema.ToUpper() : dbschema.ToLower();

            Database.SetInitializer<MainContext>(null);
            modelBuilder.Entity<CorePlannerLogs>().ToTable("core_planner_logs", dbschema);
            modelBuilder.Entity<ConstructionOhLogicMaster>().ToTable("construction_oh_logic_master", dbschema);
            modelBuilder.Entity<SchedulerLog>().ToTable("oh_logic_scheduler_log", dbschema);
            modelBuilder.Entity<User_Master>().ToTable("vw_user_details", dbschema);
            modelBuilder.Entity<User>().ToTable("user_master", dbschema);
            modelBuilder.Entity<UserModuleMapping>().ToTable("user_module_mapping", dbschema);
            modelBuilder.Entity<UserReportMapping>().ToTable("user_report_mapping", dbschema);
            modelBuilder.Entity<RoleModuleMapping>().ToTable("role_module_mapping", dbschema);
            modelBuilder.Entity<LayerActionDetails>().ToTable("layer_action_mapping", dbschema);
            modelBuilder.Entity<UserModule>().ToTable("module_master", dbschema);

            modelBuilder.Entity<GlobalSetting>().ToTable("global_settings", dbschema);
            modelBuilder.Entity<UserLogin>().ToTable("user_login", dbschema);
            modelBuilder.Entity<WorkSpaceRegionProvince>().ToTable("workspace_region_province", dbschema);
            modelBuilder.Entity<WorkSpaceMaster>().ToTable("workspace_master", dbschema);
            modelBuilder.Entity<WorkSpaceLayers>().ToTable("workspace_layers", dbschema);
            modelBuilder.Entity<LandbaseWorkSpaceLayers>().ToTable("landbase_workspace_layers", dbschema);
            modelBuilder.Entity<BuildingMaster>().ToTable("att_details_building", dbschema);
            modelBuilder.Entity<ADBItemMaster>().ToTable("item_template_adb", dbschema);
            modelBuilder.Entity<CDBItemMaster>().ToTable("item_template_cdb", dbschema);
            modelBuilder.Entity<BDBItemMaster>().ToTable("item_template_bdb", dbschema);
            modelBuilder.Entity<PODItemMaster>().ToTable("item_template_pod", dbschema);
            modelBuilder.Entity<TreeItemMaster>().ToTable("item_template_tree", dbschema);
            modelBuilder.Entity<ManholeItemMaster>().ToTable("item_template_manhole", dbschema);
            modelBuilder.Entity<HandholeItemMaster>().ToTable("item_template_handhole", dbschema);
            modelBuilder.Entity<CouplerItemMaster>().ToTable("item_template_coupler", dbschema);
            modelBuilder.Entity<SCItemMaster>().ToTable("item_template_spliceclosure", dbschema);
            modelBuilder.Entity<SplitterItemMaster>().ToTable("item_template_splitter", dbschema);
            modelBuilder.Entity<CableItemMaster>().ToTable("item_template_cable", dbschema);
            modelBuilder.Entity<MicroductItemMaster>().ToTable("item_template_microduct", dbschema);
            modelBuilder.Entity<TowerItemMaster>().ToTable("item_template_tower", dbschema);
            modelBuilder.Entity<ConduitItemMaster>().ToTable("item_template_conduit", dbschema);
            modelBuilder.Entity<SectorItemMaster>().ToTable("item_template_sector", dbschema);
            modelBuilder.Entity<AntennaItemMaster>().ToTable("item_template_antenna", dbschema);
            modelBuilder.Entity<MicrowaveLinkItemMaster>().ToTable("item_template_microwavelink", dbschema);
            modelBuilder.Entity<StructureMaster>().ToTable("att_details_bld_structure", dbschema);
            modelBuilder.Entity<Area>().ToTable("att_details_area", dbschema);
            modelBuilder.Entity<DSA>().ToTable("att_details_dsa", dbschema);
            modelBuilder.Entity<CSA>().ToTable("att_details_csa", dbschema);
            modelBuilder.Entity<SubArea>().ToTable("att_details_subarea", dbschema);
            modelBuilder.Entity<SurveyArea>().ToTable("att_details_surveyarea", dbschema);
            modelBuilder.Entity<ErrorLog>().ToTable("error_log", dbschema);
            modelBuilder.Entity<PrintExportLog>().ToTable("print_export_log", dbschema);
            modelBuilder.Entity<ApiErrorLog>().ToTable("api_error_log", dbschema);
            modelBuilder.Entity<ADBMaster>().ToTable("att_details_adb", dbschema);
            modelBuilder.Entity<PODMaster>().ToTable("att_details_pod", dbschema);
            modelBuilder.Entity<TreeMaster>().ToTable("att_details_tree", dbschema);
            modelBuilder.Entity<PoleItemMaster>().ToTable("item_template_pole", dbschema);
            modelBuilder.Entity<PoleMaster>().ToTable("att_details_pole", dbschema);
            modelBuilder.Entity<SplitterMaster>().ToTable("att_details_splitter", dbschema);
            modelBuilder.Entity<SurveyAreaAssigned>().ToTable("att_details_surveyarea_assigned", dbschema);
            modelBuilder.Entity<ManholeMaster>().ToTable("att_details_manhole", dbschema);
            modelBuilder.Entity<HandholeMaster>().ToTable("att_details_handhole", dbschema);
            modelBuilder.Entity<CouplerMaster>().ToTable("att_details_coupler", dbschema);
            modelBuilder.Entity<CDBMaster>().ToTable("att_details_cdb", dbschema);
            modelBuilder.Entity<BDBMaster>().ToTable("att_details_bdb", dbschema);
            modelBuilder.Entity<SCMaster>().ToTable("att_details_spliceclosure", dbschema);
            modelBuilder.Entity<MPODItemMaster>().ToTable("item_template_mpod", dbschema);
            modelBuilder.Entity<MPODMaster>().ToTable("att_details_mpod", dbschema);
            modelBuilder.Entity<CableMaster>().ToTable("att_details_cable", dbschema);
            modelBuilder.Entity<ONTItemMaster>().ToTable("item_template_ont", dbschema);
            modelBuilder.Entity<ONTMaster>().ToTable("att_details_ont", dbschema);
            modelBuilder.Entity<Customer>().ToTable("att_details_customer", dbschema);
            // modelBuilder.Entity<AssociateCustomerElements>().ToTable("att_details_customer", dbschema); 
            modelBuilder.Entity<TicketTypeMaster>().ToTable("ticket_type_master", dbschema);
            modelBuilder.Entity<TrenchItemMaster>().ToTable("item_template_trench", dbschema);
            modelBuilder.Entity<DuctItemMaster>().ToTable("item_template_duct", dbschema);
            modelBuilder.Entity<DuctMaster>().ToTable("att_details_duct", dbschema);
            modelBuilder.Entity<GipipeItemMaster>().ToTable("item_template_gipipe", dbschema);
            modelBuilder.Entity<GipipeMaster>().ToTable("att_details_gipipe", dbschema);
            modelBuilder.Entity<TrenchMaster>().ToTable("att_details_trench", dbschema);
            modelBuilder.Entity<CabinetItemMaster>().ToTable("item_template_cabinet", dbschema);
            modelBuilder.Entity<CabinetMaster>().ToTable("att_details_cabinet", dbschema);
            modelBuilder.Entity<ConduitMaster>().ToTable("att_details_conduit", dbschema);
            modelBuilder.Entity<BusinessLayer>().ToTable("business_layer", dbschema);

            modelBuilder.Entity<layerDetail>().ToTable("layer_details", dbschema);
            modelBuilder.Entity<MapScaleSettings>().ToTable("map_scale_settings", dbschema);

            modelBuilder.Entity<InfoSetting>().ToTable("info_settings", dbschema);

            modelBuilder.Entity<SearchSetting>().ToTable("search_settings", dbschema);

            modelBuilder.Entity<ImportRegionProvince>().ToTable("region_boundary", dbschema);

            modelBuilder.Entity<ViewRegionProvinces>().ToTable("vw_att_details_provice_region", dbschema);
            modelBuilder.Entity<WallMountMaster>().ToTable("att_details_wallmount", dbschema);
            modelBuilder.Entity<WallMountItemMaster>().ToTable("item_template_wallmount", dbschema);
            modelBuilder.Entity<ispShaftRange>().ToTable("isp_shaft_range", dbschema);
            modelBuilder.Entity<AssociateEntityMaster>().ToTable("associate_entity_master", dbschema);

            //modelBuilder.Entity<LastMonthActiveUsers>().ToTable("vw_last_month_active_users", dbschema); 

            modelBuilder.Entity<ProjectMaster>().ToTable("att_details_projectspecificnation", dbschema);
            modelBuilder.Entity<CreateVendor>().ToTable("vendor_master", dbschema);

            modelBuilder.Entity<VendorSpecificationMaster>().ToTable("item_template_master", dbschema);
            modelBuilder.Entity<LayerGroupMaster>().ToTable("layer_group_master", dbschema);
            modelBuilder.Entity<LayerGroupMapping>().ToTable("layer_group_mapping", dbschema);
            modelBuilder.Entity<RestrictedArea>().ToTable("att_details_restricted_area", dbschema);
            modelBuilder.Entity<LayerStyleMaster>().ToTable("layer_style_master", dbschema);



            modelBuilder.Entity<ShaftInfo>().ToTable("isp_shaft_info", dbschema);
            modelBuilder.Entity<FloorInfo>().ToTable("isp_floor_info", dbschema);
            modelBuilder.Entity<RoomInfo>().ToTable("isp_room_info", dbschema);
            modelBuilder.Entity<RoomTemplate>().ToTable("isp_template_room", dbschema);
            modelBuilder.Entity<HTBTemplate>().ToTable("isp_template_htb", dbschema);
            modelBuilder.Entity<HTBInfo>().ToTable("isp_htb_info", dbschema);
            modelBuilder.Entity<FDBTemplate>().ToTable("isp_template_fdb", dbschema);
            modelBuilder.Entity<FDBInfo>().ToTable("isp_fdb_info", dbschema);
            modelBuilder.Entity<IspLineMaster>().ToTable("isp_line_master", dbschema);
            modelBuilder.Entity<OpticalRepeaterTemplate>().ToTable("isp_template_opticalrepeater", dbschema);
            modelBuilder.Entity<OpticalRepeaterInfo>().ToTable("isp_opticalrepeater_info", dbschema);

            modelBuilder.Entity<CurrentActiveUsers>().ToTable("vw_active_users", dbschema);
            modelBuilder.Entity<LastMonthActiveUsers>().ToTable("vw_last_month_active_users", dbschema);
            modelBuilder.Entity<UserLocationTracking>().ToTable("location_tracking", dbschema);

            modelBuilder.Entity<ConnectionInfoMaster>().ToTable("connection_info", dbschema);
            modelBuilder.Entity<TempConnectionInfoMaster>().ToTable("temp_connection_info", dbschema);

            #region rahul

            modelBuilder.Entity<EntityNotifications>().ToTable("entity_notifications", dbschema);
            modelBuilder.Entity<EntityNotificationComments>().ToTable("entity_notifications_comments", dbschema);
            modelBuilder.Entity<AddNewEntityUtilization>().ToTable("entity_utilization_settings", dbschema);
            // modelBuilder.Entity<RoleMaster>().ToTable("role_master", dbschema);
            modelBuilder.Entity<ProjectMaster>().ToTable("att_details_projectspecificnation", dbschema);
            modelBuilder.Entity<CreateVendor>().ToTable("vendor_master", dbschema);
            modelBuilder.Entity<VendorSpecificationMaster>().ToTable("item_template_master", dbschema);
            // modelBuilder.Entity<RoleMaster>().ToTable("role_master", dbschema);
            modelBuilder.Entity<ProjectCodeMaster>().ToTable("att_details_project_master", dbschema);
            modelBuilder.Entity<PlanningCodeMaster>().ToTable("att_details_planning_master", dbschema);
            modelBuilder.Entity<WorkorderCodeMaster>().ToTable("att_details_workorder_master", dbschema);
            modelBuilder.Entity<PurposeCodeMaster>().ToTable("att_details_purpose_master", dbschema);
            modelBuilder.Entity<EquipmentTypeMaster>().ToTable("isp_type_master", dbschema);
            modelBuilder.Entity<BrandTypeMaster>().ToTable("isp_brand_master", dbschema);
            modelBuilder.Entity<ModelTypeMaster>().ToTable("isp_base_model", dbschema);
            modelBuilder.Entity<PortInfo>().ToTable("isp_port_master", dbschema);
            modelBuilder.Entity<LabelSetting>().ToTable("label_settings", dbschema);
            modelBuilder.Entity<dropdown_master>().ToTable("dropdown_master", dbschema);
            modelBuilder.Entity<DropdownMasterMapping>().ToTable("dropdown_master_mapping", dbschema);
            modelBuilder.Entity<IspPortInfo>().ToTable("isp_port_info", dbschema);
            modelBuilder.Entity<NELoopDetails>().ToTable("att_details_loop", dbschema);
            modelBuilder.Entity<SplitterLossMaster>().ToTable("splitter_loss_master", dbschema);
            modelBuilder.Entity<LinkBudgetMaster>().ToTable("link_budget_master", dbschema);
            #endregion
            modelBuilder.Entity<IspEntityMapping>().ToTable("isp_entity_mapping", dbschema);


            #region sapna

            modelBuilder.Entity<BuildingRfSStatus>().ToTable("att_details_building_status", dbschema);

            modelBuilder.Entity<ProvinceBoundary>().ToTable("province_boundary", dbschema);

            modelBuilder.Entity<TempBuildingMaster>().ToTable("temp_att_details_building", dbschema);

            #endregion
            modelBuilder.Entity<CableColorSettings>().ToTable("cable_color_master", dbschema);
            modelBuilder.Entity<CableMapColorSettings>().ToTable("cable_color_settings", dbschema);
            modelBuilder.Entity<ConnectionLabelSettings>().ToTable("display_name_settings", dbschema);
            #region FileUploader
            modelBuilder.Entity<ExternalDataUploader>().ToTable("external_data_files", dbschema);
            #endregion
            modelBuilder.Entity<AssociateEntity>().ToTable("associate_entity_info", dbschema);
            modelBuilder.Entity<LibraryAttachment>().ToTable("library_attachments", dbschema);
            modelBuilder.Entity<GeoTaggingImages>().ToTable("geotagging_images", dbschema);
            modelBuilder.Entity<EntityMaintainenceCharges>().ToTable("entity_maintainence_charges", dbschema);
            // modelBuilder.Entity<RoleLayersMapping>().ToTable("role_layers_mapping", dbschema);
            modelBuilder.Entity<EmailSettingsModel>().ToTable("mail_settings", dbschema);
            modelBuilder.Entity<JobPackMaster>().ToTable("att_details_jobpack", dbschema);
            modelBuilder.Entity<layerColumnSettings>().ToTable("layer_columns_settings", dbschema);
            modelBuilder.Entity<VWTerminationPointMaster>().ToTable("vw_termination_point_master", dbschema);
            modelBuilder.Entity<TerminationPointMaster>().ToTable("termination_point_master", dbschema);
            modelBuilder.Entity<FormInputSettings>().ToTable("form_input_settings", dbschema);
            modelBuilder.Entity<Reference>().ToTable("att_entity_reference", dbschema);
            modelBuilder.Entity<entityAtAcceptance>().ToTable("att_entity_at_acceptance", dbschema);
            modelBuilder.Entity<FMSItemMaster>().ToTable("item_template_fms", dbschema);
            modelBuilder.Entity<FMSMaster>().ToTable("att_details_fms", dbschema);
            modelBuilder.Entity<portStatusMaster>().ToTable("port_status_master", dbschema);
            modelBuilder.Entity<ProjectArea>().ToTable("att_details_projectarea", dbschema);
            modelBuilder.Entity<ROWItemMaster>().ToTable("item_template_row", dbschema);
            modelBuilder.Entity<TicketMaster>().ToTable("ticket_master", dbschema);
            modelBuilder.Entity<TicketStepsMaster>().ToTable("ticket_steps_master", dbschema);
            modelBuilder.Entity<PatchCordItemMaster>().ToTable("item_template_patchcord", dbschema);
            modelBuilder.Entity<PatchCordMaster>().ToTable("att_details_patchcord", dbschema);
            modelBuilder.Entity<IntegrationSuperSet>().ToTable("integration_superset", dbschema);
            modelBuilder.Entity<SpliceTrayInfo>().ToTable("att_details_splice_tray", dbschema);


            modelBuilder.Entity<TempTicketMaster>().ToTable("temp_ticket_master", dbschema);
            modelBuilder.Entity<TempFiberLink>().ToTable("temp_fiber_link", dbschema);


            modelBuilder.Entity<ROWMaster>().ToTable("att_details_row", dbschema);
            modelBuilder.Entity<rowApplyStage>().ToTable("att_details_row_apply", dbschema);
            modelBuilder.Entity<rowApproveRejectStage>().ToTable("att_details_row_approve_reject", dbschema);


            #region ISP Equipment
            modelBuilder.Entity<ISPModelInfo>().ToTable("isp_model_info", dbschema);
            modelBuilder.Entity<ISPModelRule>().ToTable("isp_model_rules", dbschema);
            modelBuilder.Entity<ISPModelMaster>().ToTable("isp_model_master", dbschema);
            modelBuilder.Entity<ISPModelMapping>().ToTable("isp_model_mapping", dbschema);
            modelBuilder.Entity<RackInfo>().ToTable("att_details_rack", dbschema);
            modelBuilder.Entity<EquipmentInfo>().ToTable("att_details_model", dbschema);
            modelBuilder.Entity<ISPModelTypeMaster>().ToTable("isp_model_type_master", dbschema);
            modelBuilder.Entity<ISPModelColorMaster>().ToTable("isp_model_color_master", dbschema);
            modelBuilder.Entity<ISPModelImageMaster>().ToTable("isp_model_image_master", dbschema);
            #endregion
            #region Feasibility
            modelBuilder.Entity<FeasibilityCableType>().ToTable("feasibility_cable_type", dbschema);
            modelBuilder.Entity<FeasibilityDemarcationType>().ToTable("feasibility_demarcation_type", dbschema);
            modelBuilder.Entity<FeasibilityInput>().ToTable("feasibility_input", dbschema);
            modelBuilder.Entity<FeasibilityHistory>().ToTable("feasibility_history", dbschema);
            // modelBuilder.Entity<FeasibiltyGeometry>().ToTable("feasibility_geometry", dbschema);
            modelBuilder.Entity<FeasibiltyGeometry>().ToTable("vw_sf_getfeasibilitygeometry", dbschema);
            modelBuilder.Entity<FeasibilityCableTypeHistory>().ToTable("feasibility_cable_type_history", dbschema);
            modelBuilder.Entity<FeasibilityInsideCable>().ToTable("feasibility_inside_cable", dbschema);
            #endregion
            modelBuilder.Entity<ROWPIT>().ToTable("att_details_row_pit", dbschema);
            modelBuilder.Entity<ROWRemarks>().ToTable("att_details_row_remarks", dbschema);
            modelBuilder.Entity<ROWAuthority>().ToTable("att_details_row_authority", dbschema);
            modelBuilder.Entity<PITTemplateMaster>().ToTable("item_template_pit", dbschema);

            modelBuilder.Entity<SiteInfo>().ToTable("att_details_site_info", dbschema);
            modelBuilder.Entity<SiteCustomer>().ToTable("att_details_site_customer", dbschema);
            modelBuilder.Entity<SiteCircleList>().ToTable("site_circle_master", dbschema);
            modelBuilder.Entity<ROWChargesTemplate>().ToTable("row_other_charges_template", dbschema);
            modelBuilder.Entity<UploadSummary>().ToTable("upload_summary", dbschema);
            modelBuilder.Entity<ParentChildLayerMapping>().ToTable("vw_layer_mapping", dbschema);

            modelBuilder.Entity<ManageResources>().ToTable("res_resources", dbschema);
            modelBuilder.Entity<LMCCableInfo>().ToTable("att_details_lmc_cable_info", dbschema);
            modelBuilder.Entity<BuildingComments>().ToTable("att_details_building_status_history", dbschema);
            modelBuilder.Entity<EntityNotificationStatus>().ToTable("entity_notification_status", dbschema);
            #region Data Uploader
            modelBuilder.Entity<TempPOD>().ToTable("temp_du_pod", dbschema);
            modelBuilder.Entity<TempPole>().ToTable("temp_du_pole", dbschema);
            modelBuilder.Entity<TempManhole>().ToTable("temp_du_manhole", dbschema);
            modelBuilder.Entity<TempSplitter>().ToTable("temp_du_splitter", dbschema);
            modelBuilder.Entity<TempSpliceClosure>().ToTable("temp_du_spliceclosure", dbschema);
            modelBuilder.Entity<TempBuilding>().ToTable("temp_du_building", dbschema);
            modelBuilder.Entity<TempADB>().ToTable("temp_du_adb", dbschema);
            modelBuilder.Entity<TempTree>().ToTable("temp_du_tree", dbschema);
            modelBuilder.Entity<TempWallMount>().ToTable("temp_du_wallmount", dbschema);
            modelBuilder.Entity<TempBDB>().ToTable("temp_du_bdb", dbschema);
            modelBuilder.Entity<TempCDB>().ToTable("temp_du_cdb", dbschema);
            modelBuilder.Entity<TempFMS>().ToTable("temp_du_fms", dbschema);
            modelBuilder.Entity<TempONT>().ToTable("temp_du_ont", dbschema);
            modelBuilder.Entity<TempHTB>().ToTable("temp_du_htb", dbschema);
            modelBuilder.Entity<TempFDB>().ToTable("temp_du_fdb", dbschema);
            modelBuilder.Entity<TempMPOD>().ToTable("temp_du_mpod", dbschema);
            modelBuilder.Entity<TempCoupler>().ToTable("temp_du_coupler", dbschema);
            modelBuilder.Entity<TempStructure>().ToTable("temp_du_structure", dbschema);
            modelBuilder.Entity<TempLoop>().ToTable("temp_du_loop", dbschema);
            modelBuilder.Entity<TempTrench>().ToTable("temp_du_trench", dbschema);
            modelBuilder.Entity<TempDuct>().ToTable("temp_du_duct", dbschema);
            modelBuilder.Entity<TempGipipe>().ToTable("temp_du_gipipe", dbschema);

            modelBuilder.Entity<TempCable>().ToTable("temp_du_cable", dbschema);
            modelBuilder.Entity<TempRow>().ToTable("temp_du_row", dbschema);
            modelBuilder.Entity<TempTower>().ToTable("temp_du_tower", dbschema);
            modelBuilder.Entity<TempRoom>().ToTable("temp_du_room", dbschema);

            modelBuilder.Entity<TempMicroduct>().ToTable("temp_du_microduct", dbschema);


            modelBuilder.Entity<TempSector>().ToTable("temp_du_sector", dbschema);
            modelBuilder.Entity<TempAntenna>().ToTable("temp_du_antenna", dbschema);
            modelBuilder.Entity<TempCabinet>().ToTable("temp_du_cabinet", dbschema);
            modelBuilder.Entity<TempWirelessCustomer>().ToTable("temp_du_customer", dbschema);
            modelBuilder.Entity<TempHandhole>().ToTable("temp_du_handhole", dbschema);
            modelBuilder.Entity<TempOpticalRepeater>().ToTable("temp_du_opticalrepeater", dbschema);
            #endregion
            #region Models for Template and Role Mangement
            modelBuilder.Entity<LayerRightsTemplateMaster>().ToTable("layer_rights_template_master", dbschema);
            modelBuilder.Entity<LayerRightsTemplatePermission>().ToTable("layer_rights_template_permission", dbschema);
            modelBuilder.Entity<RoleMaster>().ToTable("role_master", dbschema);
            modelBuilder.Entity<UserPermissionArea>().ToTable("user_permission_area", dbschema);
            modelBuilder.Entity<RolePermissionEntity>().ToTable("role_permission_entity", dbschema);
            #endregion

            #region Fault Entity
            modelBuilder.Entity<Fault>().ToTable("att_details_fault", dbschema);
            modelBuilder.Entity<FaultStatusHistory>().ToTable("att_details_fault_status_history", dbschema);
            #endregion
            modelBuilder.Entity<UserLoginHistoryInfo>().ToTable("user_login_history", dbschema);

            modelBuilder.Entity<FiberLink>().ToTable("att_details_fiber_link", dbschema);
            modelBuilder.Entity<fiberLinkColumnsMapping>().ToTable("fiber_link_columns_settings", dbschema);
            modelBuilder.Entity<AccessoriesInfoModel>().ToTable("att_details_accessories", dbschema);
            modelBuilder.Entity<Competitor>().ToTable("att_details_competitor", dbschema);
            modelBuilder.Entity<TempResources>().ToTable("res_temp_resource_manager", dbschema);
            modelBuilder.Entity<res_dropdown_master>().ToTable("res_dropdown_master", dbschema);
            modelBuilder.Entity<ResourcesKeyStatus>().ToTable("res_resource_junk_key", dbschema);

            modelBuilder.Entity<APIConsumerMaster>().ToTable("api_consumer_master", dbschema);
            modelBuilder.Entity<APIRequestLog>().ToTable("api_request_log", dbschema);
            modelBuilder.Entity<GisApiLog>().ToTable("gis_api_logs", dbschema);
            modelBuilder.Entity<WebRequestLog>().ToTable("web_request_log", dbschema);
            modelBuilder.Entity<TowerMaster>().ToTable("att_details_tower", dbschema);
            modelBuilder.Entity<AntennaMaster>().ToTable("att_details_antenna", dbschema);
            modelBuilder.Entity<SectorMaster>().ToTable("att_details_sector", dbschema);
            modelBuilder.Entity<InstallationInfo>().ToTable("installation_info", dbschema);
            modelBuilder.Entity<TowerAssociatedPop>().ToTable("att_details_tower_pop", dbschema);
            modelBuilder.Entity<MicrowaveLinkMaster>().ToTable("att_details_microwavelink", dbschema);
            modelBuilder.Entity<MicrowavelinkFeederSystem>().ToTable("att_details_microwavelink_feeder", dbschema);
            modelBuilder.Entity<MicrowavelinkPower>().ToTable("att_details_microwavelink_power", dbschema);
            modelBuilder.Entity<MicrowavelinkAntenna>().ToTable("att_details_microwavelink_antenna", dbschema);
            modelBuilder.Entity<MicroductMaster>().ToTable("att_details_microduct", dbschema);
            modelBuilder.Entity<ReportingRoleMapping>().ToTable("reporting_role_mapping", dbschema);
            modelBuilder.Entity<TicketTypeRoleMapping>().ToTable("ticket_type_role_mapping", dbschema);

            #region Help & FAQ's
            modelBuilder.Entity<FAQMaster>().ToTable("faq_master", dbschema);
            modelBuilder.Entity<FAQ_UserManual>().ToTable("faq_usermanual_master", dbschema);
            #endregion
            modelBuilder.Entity<NetworkTicket>().ToTable("att_details_networktickets", dbschema);
            modelBuilder.Entity<ColumnMappingTemplate>().ToTable("data_uploader_column_mapping_template", dbschema);
            modelBuilder.Entity<ColumnMapping>().ToTable("data_uploader_column_mapping", dbschema);
            modelBuilder.Entity<AdditionalAttributes>().ToTable("entity_additional_attributes", dbschema);
            modelBuilder.Entity<TemplateColumn>().ToTable("data_uploader_template", dbschema);
            //Redline
            modelBuilder.Entity<RedlineMaster>().ToTable("redline", dbschema);
            modelBuilder.Entity<RedlineAssignedUsers>().ToTable("redline_assigned_user", dbschema);
            modelBuilder.Entity<RedlineStatusHistory>().ToTable("redline_status_history", dbschema);

            #region LANDBASE LAYERS 
            modelBuilder.Entity<LandBaseLayer>().ToTable("att_details_landbase", dbschema);
            modelBuilder.Entity<LandBaseMaster>().ToTable("landbase_layer_master", dbschema);
            modelBuilder.Entity<ManageDropdownValues>().ToTable("landbase_dropdown_master", dbschema);
            modelBuilder.Entity<TempLandBase>().ToTable("temp_du_landbase", dbschema);
            modelBuilder.Entity<SearchLandBaseLayerSetting>().ToTable("landbase_search_settings", dbschema);
            modelBuilder.Entity<Landbase_label_Settings>().ToTable("landbase_label_settings", dbschema);
            #endregion

            #region Network Planning
            modelBuilder.Entity<NetworkPlanning>().ToTable("att_details_network_plan", dbschema);
            modelBuilder.Entity<temp_auto_network_plan>().ToTable("temp_auto_network_plan", dbschema);
            #endregion

            #region DXF Source List
            modelBuilder.Entity<SourceIdList>().ToTable("tbl_srid_list", dbschema);
            #endregion

            #region Print map Saved Template

            ////// Print map Saved Template
            modelBuilder.Entity<PrintSavedTemplate>().ToTable("print_saved_template", dbschema);

            modelBuilder.Entity<BomBoqAdAttribute>().ToTable("tbl_bomboq_ad_attribute", dbschema);
            modelBuilder.Entity<ConstructionBomDetails>().ToTable("construction_bom_details", dbschema);

            #endregion

            #region Long Running Queries
            modelBuilder.Entity<LongRunningQueries>().ToTable("vw_long_running_queries", dbschema);
            #endregion

            #region vault
            modelBuilder.Entity<VaultItemMaster>().ToTable("item_template_vault", dbschema);
            modelBuilder.Entity<VaultMaster>().ToTable("att_details_vault", dbschema);
            modelBuilder.Entity<TempVault>().ToTable("temp_du_vault", dbschema);

            #endregion

            # region Accessories 
            modelBuilder.Entity<AccessoriesMaster>().ToTable("accessories_master", dbschema);
            modelBuilder.Entity<AccessoriesMapping>().ToTable("accessories_layer_mapping", dbschema);
            #endregion

            #region(FTTH FEASIBILITY)
            modelBuilder.Entity<FTTHFeasibilityDetailModel>().ToTable("feasibility_input_ftth", dbschema);
            #endregion
            # region Errorlog 
            modelBuilder.Entity<ErrorLog>().ToTable("error_log", dbschema);

            #endregion

            modelBuilder.Entity<TempVendorSpecificationMaster>().ToTable("temp_item_template_master", dbschema);
            modelBuilder.Entity<ChangeConfigurationSetting>().ToTable("audit_configuration_setting", dbschema);
            modelBuilder.Entity<UploadVendorSpecificationImageDoc>().ToTable("specification_attachments", dbschema);
            modelBuilder.Entity<ViewGroupLibrary>().ToTable("entity_group_library", dbschema);

            # region Ortho Image 
            modelBuilder.Entity<OrthoImageMasterModel>().ToTable("ortho_image_master", dbschema);
            #endregion
            modelBuilder.Entity<TempPatchPanel>().ToTable("temp_du_patchpanel", dbschema);
            modelBuilder.Entity<PatchPanelItemMaster>().ToTable("item_template_patchpanel", dbschema);
            modelBuilder.Entity<PatchPanelMaster>().ToTable("att_details_patchpanel", dbschema);
            modelBuilder.Entity<VSATDetails>().ToTable("att_details_vsat_hub", dbschema);
            modelBuilder.Entity<VSATAntenna>().ToTable("att_details_vsat_antenna", dbschema);
            modelBuilder.Entity<LayerIconMapping>().ToTable("layer_icon_master", dbschema);
            modelBuilder.Entity<Issue_Type_Master>().ToTable("tbl_wfm_ordertype", dbschema);
            #region Additional Attribute tables
            modelBuilder.Entity<DynamicControls>().ToTable("dynamic_controls", dbschema);
            modelBuilder.Entity<DynamicControlsDDLMaster>().ToTable("dynamic_controls_dropdown_master", dbschema);
            #endregion


            #region WFM Tables
            modelBuilder.Entity<SlotRequest>().ToTable("tbl_wfm_slot_request", dbschema);
            modelBuilder.Entity<ServiceType>().ToTable("tbl_wfm_servicetype", dbschema);
            modelBuilder.Entity<Slot>().ToTable("tbl_wfm_slot", dbschema);
            modelBuilder.Entity<SlotConfirmation>().ToTable("tbl_wfm_slot_confirmation", dbschema);
            modelBuilder.Entity<User_TimeSheet>().ToTable("user_timesheet", dbschema);

            modelBuilder.Entity<Models.WFM.Task>().ToTable("hpsm_ticket_master", dbschema);
            modelBuilder.Entity<HPSM_Ticket_Master_History>().ToTable("hpsm_ticket_master_history", dbschema);
            modelBuilder.Entity<Route_Issue>().ToTable("route_issues", dbschema);
            modelBuilder.Entity<Task_Tracking>().ToTable("task_tracking", dbschema);
            modelBuilder.Entity<Vw_Hpsm_Ticket_Status>().ToTable("vw_hpsm_ticket_status", dbschema);
            modelBuilder.Entity<JobOrderStatus>().ToTable("tbl_wfm_job_order_status", dbschema);
            modelBuilder.Entity<TicketSteps>().ToTable("tbl_wfm_ticket_steps", dbschema);
            modelBuilder.Entity<Brands>().ToTable("tbl_wfm_brand", dbschema);
            //modelBuilder.Entity<BrandModel>().ToTable("tbl_wfm_model", dbschema); 
            modelBuilder.Entity<hpsm_ticket_attachments>().ToTable("hpsm_ticket_attachments", dbschema);
            modelBuilder.Entity<VW_USER_MANAGER_RELATION>().ToTable("VW_USER_MANAGER_RELATION", dbschema);
            modelBuilder.Entity<NOTIFICATION_ALERTS_HISTORY>().ToTable("NOTIFICATION_ALERTS_HISTORY", dbschema);
            modelBuilder.Entity<Issue_Resolution_Type_Master>().ToTable("issue_resolution_type_master", dbschema);
            modelBuilder.Entity<tbl_wfm_jobstatus>().ToTable("tbl_wfm_jobstatus", dbschema);

            modelBuilder.Entity<AdditionalMaterialMaster>().ToTable("tbl_additional_material_master", dbschema);
            modelBuilder.Entity<AdditionalMaterial>().ToTable("tbl_additional_material", dbschema);
            modelBuilder.Entity<vw_hpsm_ticket_attachments>().ToTable("vw_hpsm_ticket_attachments", dbschema);


            modelBuilder.Entity<JoCategoryRoleMapping>().ToTable("wfm_jo_category_role_mapping", dbschema);


            modelBuilder.Entity<RoleSeviceFacilityMapping>().ToTable("wfm_role_service_facility_mapping", dbschema);
            modelBuilder.Entity<RoleJoTypeMapping>().ToTable("wfm_role_jo_type_mapping", dbschema);
            modelBuilder.Entity<UserSeviceFacilityMapping>().ToTable("wfm_user_service_facility_mapping", dbschema);
            modelBuilder.Entity<UserJoTypeMapping>().ToTable("wfm_user_jo_type_mapping", dbschema);




            modelBuilder.Entity<ConnectedDevice>().ToTable("tbl_wfm_connected_device", dbschema);
            modelBuilder.Entity<ConnectedDeviceRequest>().ToTable("tbl_wfm_connected_device_request", dbschema);

            modelBuilder.Entity<UserManagerMapping>().ToTable("user_manager_mapping", dbschema);
            modelBuilder.Entity<userFeToolMapping>().ToTable("user_tools_mapping", dbschema);

            modelBuilder.Entity<UserWarehouseCodeMapping>().ToTable("user_warehousecode_mapping", dbschema);

            modelBuilder.Entity<UserJoCategoryMapping>().ToTable("wfm_user_jo_category_mapping", dbschema);
            modelBuilder.Entity<RoleJoCategoryMapping>().ToTable("wfm_role_jo_category_mapping", dbschema);
            modelBuilder.Entity<WfmRca>().ToTable("tbl_wfm_rca", dbschema);

            modelBuilder.Entity<wfm_jo_type_master>().ToTable("wfm_jo_type_master", dbschema);
            modelBuilder.Entity<wfm_service_facility_master>().ToTable("wfm_service_facility_master", dbschema);
            modelBuilder.Entity<tbl_wfm_slot_duration>().ToTable("tbl_wfm_slot_duration", dbschema);
            modelBuilder.Entity<VMSlotConfirmation>().ToTable("vw_wfm_slot_confirmation", dbschema);
            //modelBuilder.Entity<ServiceFacilityMaster>().ToTable("wfm_service_facility_master", dbschema);
            modelBuilder.Entity<JoCategoryMaster>().ToTable("wfm_jo_category_master", dbschema);
            //modelBuilder.Entity<JoTypeMaster>().ToTable("wfm_jo_type_master", dbschema);
            modelBuilder.Entity<wfm_notification>().ToTable("wfm_notification", dbschema);
            modelBuilder.Entity<wfm_notification_template>().ToTable("wfm_notification_template", dbschema);
            modelBuilder.Entity<wfm_email_sms_log>().ToTable("wfm_email_sms_log", dbschema);
            modelBuilder.Entity<wfm_tt_rc>().ToTable("wfm_tt_rc", dbschema);
            modelBuilder.Entity<wfm_tt_rca>().ToTable("wfm_tt_rca", dbschema);
            modelBuilder.Entity<TT_Customer_Category>().ToTable("wfm_tt_customer_category", dbschema);

            modelBuilder.Entity<TT_Customer_Category>().ToTable("wfm_tt_customer_category", dbschema);
            modelBuilder.Entity<TT_Customer_Segment>().ToTable("wfm_tt_customer_segment", dbschema);
            modelBuilder.Entity<TT_category>().ToTable("wfm_tt_category", dbschema);
            modelBuilder.Entity<TT_Type>().ToTable("wfm_tbl_tt_type", dbschema);
            modelBuilder.Entity<PortManager>().ToTable("tbl_port_manager", dbschema);
            modelBuilder.Entity<UpdateNotificationLog>().ToTable("tbl_notification_log", dbschema);
            modelBuilder.Entity<Payment_Details>().ToTable("tbl_wfm_payment_details", dbschema);

            modelBuilder.Entity<EmailEventTemplate>().ToTable("email_event_trigger_template", dbschema);
            

        
            #endregion

            #region Process Summary
            modelBuilder.Entity<ProcessSummary>().ToTable("process_summary", dbschema);

            #endregion
            #region theme 
            modelBuilder.Entity<DynamicTheme>().ToTable("dynamic_theme_master", dbschema);

            #endregion

            modelBuilder.Entity<BomBoqInfoSummary>().ToTable("bom_boq_revision_info", dbschema);
            modelBuilder.Entity<UserOTPDetails>().ToTable("user_otp_details", dbschema);
            modelBuilder.Entity<OTPAuthenticationSettings>().ToTable("otp_authentication_configuration", dbschema);
            modelBuilder.Entity<ADOIDAuthentication>().ToTable("adoid_authentication", dbschema);

            #region Voice Command Settings
            modelBuilder.Entity<VoiceCommandMaster>().ToTable("voice_command_master", dbschema);
            modelBuilder.Entity<SaveVoiceCommandMaster>().ToTable("voice_command_mapping", dbschema);
            modelBuilder.Entity<TempSaveVoiceCommandMaster>().ToTable("temp_voice_command_mapping", dbschema);
            #endregion
            #region BulkUserUpload
            modelBuilder.Entity<BulkUserUploadSummary>().ToTable("bulk_user_upload_summary", dbschema);
            modelBuilder.Entity<BulkUserUpload>().ToTable("bulk_user_upload_detail", dbschema);
            modelBuilder.Entity<BulkUserUploadModuleMapping>().ToTable("bulk_user_upload_module_mapping", dbschema);
            modelBuilder.Entity<BulkUserUploadJoTypeMapping>().ToTable("bulk_user_upload_jo_type_mapping", dbschema);
            modelBuilder.Entity<BulkUserUploadJoCategoryMapping>().ToTable("bulk_user_upload_jo_category_mapping", dbschema);
            modelBuilder.Entity<BulkUserUploadServiceFacilityMapping>().ToTable("bulk_user_upload_service_facility_mapping", dbschema);
            modelBuilder.Entity<BulkUserUploadWorkAreaDetail>().ToTable("bulk_user_upload_work_area_detail", dbschema);
            modelBuilder.Entity<BulkUserUploadManagerMapping>().ToTable("bulk_user_upload_manager_mapping", dbschema);
            #endregion

            #region Ticket Form
            //modelBuilder.Entity<TicketForm>().ToTable("ticket_master", dbschema);
            modelBuilder.Entity<TicketAttachments>().ToTable("ticket_attachments", dbschema);
            #endregion

            #region UserActivityLog
            modelBuilder.Entity<UserActivityLogSettings>().ToTable("user_activity_log_settings", dbschema);
            modelBuilder.Entity<UserActivityLog>().ToTable("user_activity_log", dbschema);
            //modelBuilder.Entity<UserActivityLogSettingsPage>().ToTable("user_activity_log_settings", dbschema);
            #endregion

            #region AutoCodificationLog
            modelBuilder.Entity<AutoCodificationLog>().ToTable("auto_codification_logs", dbschema);
            #endregion

            #region EntityDeleteLog
            modelBuilder.Entity<EntityDeleteLog>().ToTable("entity_delete_log", dbschema);
            #endregion
            #region BulkAssociationLog
            modelBuilder.Entity<BulkAssociationRequestLog>().ToTable("bulk_assocation_request_logs", dbschema);
            #endregion
            #region Export Report
            modelBuilder.Entity<ExportReportLog>().ToTable("export_report_log", dbschema);
            # endregion

            #region Association Report
            modelBuilder.Entity<AssociationReportLog>().ToTable("association_report_log", dbschema);
            # endregion

            #region SI_Dashboard //Added by Diksha Gupta
            modelBuilder.Entity<HierarchyMaster>().ToTable("vw_db_hierarchy", dbschema);
            modelBuilder.Entity<Kpi_Master>().ToTable("kpi_master", dbschema);
            modelBuilder.Entity<Kpi_Category>().ToTable("kpi_category", dbschema);
            modelBuilder.Entity<Kpi_Master_View>().ToTable("vw_kpi_master", dbschema);
            modelBuilder.Entity<kpi_template>().ToTable("kpi_template", dbschema);
            //modelBuilder.Entity<HierarchyMaster>().HasNoKey()
            #endregion
            modelBuilder.Entity<TrenchCustomerDetails>().ToTable("att_details_trench_customerdetails", dbschema);
            modelBuilder.Entity<FatProcessSummary>().ToTable("fat_process_summary", dbschema);

            #region WorkAreaMarking BY ANTRA
            modelBuilder.Entity<WorkAreaMarking>().ToTable("workarea_marking", dbschema);
            #endregion

            modelBuilder.Entity<ViewSpecificationServiceList>().ToTable("item_template_service_master", dbschema);
            modelBuilder.Entity<CDBAttribute>().ToTable("att_details_cable_cdb", dbschema);
            modelBuilder.Entity<TempCDBAttributes>().ToTable("temp_du_att_details_cable_cdb", dbschema);
            //modelBuilder.Entity<FE_Tools>().ToTable("user_fe_tool_mapping1", dbschema);
            modelBuilder.Entity<FETOOLS_Attachment>().ToTable("user_tools_attachment", dbschema);

            modelBuilder.Entity<TrenchExecution>().ToTable("att_entity_execution_method", dbschema);
            modelBuilder.Entity<Site>().ToTable("att_details_site", dbschema);
            

        }

    }
    #region UserActivityLog


    #endregion

}



public class RoutingContext : DbContext
{
    public RoutingContext() : base("constr_routing_DB")
    {
        var adapter = (IObjectContextAdapter)this;
        var objectContext = adapter.ObjectContext;
        objectContext.CommandTimeout = 60 * 60;
    }
}






