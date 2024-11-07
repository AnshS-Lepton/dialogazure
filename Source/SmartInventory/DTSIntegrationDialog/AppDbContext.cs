using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSIntegrationDialog
{
    public class AppDbContext : DbContext
    {
        // Constructor that passes the connection string name to the base class
        public AppDbContext() : base("name=DefaultConnection") { }

        // DbSet representing the ProcessSite table in your database
        public DbSet<ProcessSiteSummary> ProcessSites { get; set; }
        public DbSet<ProcessSiteList> ProcessSiteLists { get; set; }
        public DbSet<NetworkCodeDetail> NetworkCodeDetails { get; set; }
        public DbSet<ProcessSiteOutput> ProcessSiteOutputs { get; set; }
        public DbSet<InRegionProvince> InRegionProvinces { get; set; }
        public DbSet<SiteAttributes> SiteDetails { get; set; }
        //public DbSet<Site> SiteDetailsMain { get; set; }
        public DbSet<PODMaster> PopDetails { get; set; }

        // Configuring the model
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessSiteSummary>().ToTable("site_process_summary", "public");

            modelBuilder.Entity<ProcessSiteList>().ToTable("process_site_list", "public").HasKey(ps => ps.id);

            modelBuilder.Entity<SiteAttributes>().ToTable("process_site_details", "public").HasKey(sa => sa.id);

            //modelBuilder.Entity<Site>().ToTable("att_details_site", "public").HasKey(s => s.system_id);
            modelBuilder.Entity<PODMaster>().ToTable("att_details_pod", "public").HasKey(s => s.system_id);

        }
    }
}
