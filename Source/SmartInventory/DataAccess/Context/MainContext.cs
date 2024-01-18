using Models;
using System;
using System.Data.Entity;
namespace DataAccess.Context
{
    public class MainContext:DbContext
    {
        public MainContext():base("constr") {  }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            string dbschema = System.Configuration.ConfigurationManager.AppSettings["dbschema"];
            Database.SetInitializer<MainContext>(null);
            modelBuilder.Entity<User>().ToTable("user_master", dbschema);
            modelBuilder.Entity<GlobalSetting>().ToTable("global_settings", dbschema);
            modelBuilder.Entity<UserLogin>().ToTable("user_login", dbschema);
        }
    }
}
