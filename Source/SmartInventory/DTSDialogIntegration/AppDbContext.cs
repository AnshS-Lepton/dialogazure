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

    //public class ProcessSiteOutput
    //{
    //    public int updated_count { get; set; }
    //    public int inserted_count { get; set; }
    //}

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

    public class ApiResponse
    {
        public string Status_Code { get; set; }
        public string Status_Description { get; set; }
        public List<SiteAttributes> Response { get; set; }
    }
    //public class AccessTokenResponse
    //{
    //    public string access_token { get; set; }
    //    public string scope { get; set; }
    //    public string token_type { get; set; }
    //    public int expires_in { get; set; }
    //}

}
