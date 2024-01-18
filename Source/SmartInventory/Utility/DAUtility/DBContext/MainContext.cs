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


namespace Utility.DAUtility.DBContext
{
    public class MainContext : DbContext
    {
        
        //before use"constr"
        public MainContext() : base("constr")
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]) == true)
                this.Database.Connection.ConnectionString = Utility.MiscHelper.Decrypt(ConfigurationManager.AppSettings["constr"].Trim());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            string dbschema = System.Configuration.ConfigurationManager.AppSettings["dbschema"];
            Database.SetInitializer<MainContext>(null);
            modelBuilder.Entity<ErrorLog>().ToTable("error_log", dbschema);//k
            modelBuilder.Entity<APIRequestLog>().ToTable("api_request_log", dbschema);//k
            modelBuilder.Entity<ApiErrorLog>().ToTable("api_error_log", dbschema);//k
            modelBuilder.Entity<GisApiLogs>().ToTable("gis_api_logs", dbschema);

        }

    }

    public class RoutingContext : DbContext
    {
        public RoutingContext() : base("constr_DB")
        {
            var adapter = (IObjectContextAdapter)this;
            var objectContext = adapter.ObjectContext;
            objectContext.CommandTimeout = 60 * 60;
        }
    }


}
