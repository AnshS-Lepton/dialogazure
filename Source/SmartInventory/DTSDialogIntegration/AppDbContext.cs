using Microsoft.EntityFrameworkCore;
using Models.Admin;
using Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DialogDTSIntegration
{
    public class AppDbContext : DbContext
    {
        // Constructor that accepts DbContextOptions and passes it to the base class
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet representing the ProcessSite table in your database
        public DbSet<ProcessSite> ProcessSites { get; set; }
        public DbSet<ProcessSiteList> ProcessSiteLists { get; set; }
        public DbSet<NetworkCodeDetail> NetworkCodeDetails { get; set; }

        public DbSet<ProcessSiteOutput> processSiteOutputs { get; set; }

        public DbSet<InRegionProvince> inRegionProvinces { get; set; }

        public DbSet<SiteAttributes> SiteDetils { get; set; } // table to store data for DTS

        public DbSet<Site> SiteDetilsMain { get; set; } //main table att_details_site
        // Configuring the model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessSite>()
                .ToTable("site_process_summary")
                .HasKey(ps => ps.process_id); // Set the primary key
            modelBuilder.Entity<ProcessSiteList>()
                .ToTable("process_site_list")
                .HasKey(ps => ps.id);
            modelBuilder.Entity<SiteAttributes>()
                .ToTable("process_site_details")
                .HasKey(ps => ps.id);
            modelBuilder.Entity<Site>()
                .ToTable("att_details_site")
                .HasKey(ps => ps.system_id);
            modelBuilder.Entity<NetworkCodeDetail>().HasNoKey();
            modelBuilder.Entity<InRegionProvince>().HasNoKey();
            modelBuilder.Entity<ProcessSiteOutput>().HasNoKey();
            // Additional configurations can be added here if needed
        }
    }

    public class ProcessSiteOutput
    {
        public int updated_count { get; set; }
        public int inserted_count { get; set; }
    }

    // Define the ProcessSite entity
    public class ProcessSite
    {
        public int process_id { get; set; } // Primary Key
        public DateTime process_start_time { get; set; }
        public DateTime process_end_time { get; set; }
        public string stataus { get; set; } // Make sure to fix the typo to 'status' if needed
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public string remarks { get; set; }
        public string entity_type { get; set; }
    }

    public class ProcessSiteList
    {
        public int id { get; set; } // Primary key
        public int process_id { get; set; } // Foreign key or identifier from Site
        public string site_id { get; set; }
        public string site_name { get; set; }
        public string status { get; set; }
        public string error_message { get; set; }
        public bool is_valid { get; set; } // Optional, to track when the site was added
    }
    
    public class SiteAttributes
    {
        public int id { get; set; }
        public int process_id { get; set; }
        public string site_id { get; set; }
        public string site_name { get; set; }
        public DateTime on_air_date { get; set; }
        public DateTime removed_date { get; set; }
        public string tx_type { get; set; }
        public string tx_technology { get; set; }
        public string tx_segment { get; set; }
        public string tx_ring { get; set; }
        public string address { get; set; }
        public string region { get; set; }
        public string province { get; set; }
        public string district { get; set; }
        public string region_address { get; set; }
        public string depot { get; set; }
        public string ds_division { get; set; }
        public string local_authority { get; set; }
        //[JsonProperty("Lat")]
        public double latitude { get; set; }
        //[JsonProperty("Lng")]
        public double longitude { get; set; }
        //[JsonProperty("Owner")]
        public string owner_name { get; set; }
        public string access_24_7 { get; set; }

        public string tower_type { get; set; }
        public int tower_height { get; set; }
        public string cabinet_type { get; set; }
        public string solution_type { get; set; }

        public int site_rank { get; set; }
       // [JsonProperty("TX_Self_Traffic")]
        public Decimal self_tx_traffic { get; set; }
       // [JsonProperty("TX_Agg_Traffic")]
        public Decimal agg_tx_traffic { get; set; }
        public Decimal metro_ring_utilization { get; set; }
        public int csr_count { get; set; }
        public int dti_circuit { get; set; }
        public string agg_01 { get; set; }
        public string agg_02 { get; set; }
        public int bandwidth { get; set; }
        public string ring_type { get; set; }
        public string link_id { get; set; }
        public string alias_name { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public string network_status { get; set; }
        public string status { get; set; }
        public bool is_new_entity { get; set; }
        public string network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
        public int? parent_system_id { get; set; }
        public int? sequence_id { get; set; }
        public bool is_visible_on_map { get; set; }
        public DateTime? status_updated_on { get; set; }
        public int? status_updated_by { get; }
        public string source_ref_id { get; set; }
        public string source_ref_type { get; set; }
        public string target_ref_id { get; set; }
        public string target_ref_code { get; set; }
        public string target_ref_description { get; set; }
        public string gis_design_id { get; set; }

        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? purpose_id { get; set; }
        public int? workorder_id { get; set; }
        public bool? is_used { get; set; }
        public string tx_agg { get; set; }
        public string bh_status { get; set; }
        public string elevation { get; set; }
        public string segment { get; set; }
        public string ring { get; set; }
        public int? maximum_cost { get; set; }
        public string project_category { get; set; }
        public int? priority { get; set; }
        public int? no_of_cores { get; set; }
        public string fiber_link_type { get; set; }
        public string comment { get; set; }
        public int? plan_cost { get; set; }
        public int? fiber_distance { get; set; }
        public string fiber_link_code { get; set; }
        public string port_type { get; set; }
        public string destination_site_id { get; set; }
        public string destination_port_type { get; set; }
        public decimal? destination_no_of_cores { get; set; }
        public string project_id_dialog { get; set; }
        public bool is_valid { get; set; }
    }

    public class ApiResponse
    {
        public string Status_Code { get; set; }
        public string Status_Description { get; set; }
        public List<SiteAttributes> Response { get; set; }
    }
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

}
