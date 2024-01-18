using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Resources.Abstract;
using Resources.Entities;
using System.Configuration;
using Npgsql;
using Resources.Utility;

namespace Resources.Concrete
{
    public class DbResourceProvider : BaseResourceProvider
    {
        // Database connection string        
        private static string connectionString = null;

        public DbResourceProvider()
        {
            connectionString = Convert.ToBoolean(ConfigurationManager.AppSettings["ISEncryptedConnection"]) == true ? ResourceBuilder.Decrypt(ConfigurationManager.AppSettings["constr"].Trim()) : ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            
        }

        public DbResourceProvider(string connection)
        {
            connectionString = connection;
        }
        /// <summary>
        /// This method can not be convert with entity methods as it will create circular dependency.That's why use NPGSql here.
        /// </summary>
        /// <returns></returns>
        protected override IList<ResourceEntry> ReadResources()
        {
            var resources = new List<ResourceEntry>();

            //const string sql = "select Culture, Name, Value from dbo.Resources;";

            using (var con = new NpgsqlConnection(connectionString))
            {
                //var cmd = new SqlCommand(sql, con);

                con.Open();

                using (var command = new NpgsqlCommand("select culture, key, value from res_resources where is_mobile_key = false", con))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        resources.Add(new ResourceEntry
                        {
                            Name = reader["key"].ToString(),
                            Value = reader["value"].ToString(),
                            Culture = reader["culture"].ToString()
                        });
                    }

                    if (!reader.HasRows) throw new Exception("No resources were found");
                }
            }

            return resources;

        }

        protected override ResourceEntry ReadResource(string name, string culture)
        {
            ResourceEntry resource = null;

            const string sql = "select culture, key, value from dbo.res_resources where culture = @culture and name = @name and is_mobile_key = false;";

            using (var con = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@culture", culture);
                cmd.Parameters.AddWithValue("@name", name);

                con.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        resource = new ResourceEntry
                        {
                            Name = reader["key"].ToString(),
                            Value = reader["value"].ToString(),
                            Culture = reader["culture"].ToString()
                        };
                    }


                    if (!reader.HasRows) throw new Exception(string.Format("Resource {0} for culture {1} was not found", name, culture));
                }
            }

            //resource = new ResourceEntry
            //{
            //    Name = "",
            //    Value = "",
            //    Culture = ""
            //};
            return resource;

        }

        protected override IList<ResourceEntry> ReadResourcesJqueryUsed()
        {
            var resources = new List<ResourceEntry>();

            //const string sql = "select Culture, Name, Value from dbo.Resources;";

            using (var con = new NpgsqlConnection(connectionString))
            {
                //var cmd = new SqlCommand(sql, con);

                con.Open();

                using (var command = new NpgsqlCommand("select culture, key, value from res_resources where is_jquery_used =true and culture='en'", con))
                {
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        resources.Add(new ResourceEntry
                        {
                            Name = reader["key"].ToString(),
                            Value = reader["value"].ToString(),
                            Culture = reader["culture"].ToString()
                        });
                    }

                    if (!reader.HasRows) throw new Exception("No resources were found");
                }
            }

            return resources;

        }




    }
}
