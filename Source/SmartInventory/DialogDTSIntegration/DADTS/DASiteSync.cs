using Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogDTSIntegration.DADTS
{
   
    public class DASiteSync
    {
        public SiteSync Save(SiteSync site)
        {
            var queryDataCreation=string.Empty;
            //string connectionString = ConfigurationManager.ConnectionStrings["SIPostGresConnection"].ConnectionString;

            string connectionString = ConfigurationManager.AppSettings["constr"];
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected to PostgreSQL Server");

                    string checkQuery = "SELECT * FROM SiteSyncDialog WHERE id = @id";
                    using (NpgsqlCommand checkCommand = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCommand.Parameters.AddWithValue("id", site.id);
                        using (NpgsqlDataReader dr = checkCommand.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                dr.Close();
                                // Update the existing site
                                string updateQuery = "UPDATE SiteSyncDialog SET end_datetime = @end_datetime, status = @status, message = @message, lastsuccess_sync = @lastsuccess_sync WHERE id = @id";
                                using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateQuery, conn))
                                {
                                    updateCommand.Parameters.AddWithValue("end_datetime", site.end_datetime);
                                    updateCommand.Parameters.AddWithValue("status", site.status);
                                    updateCommand.Parameters.AddWithValue("message", site.message);
                                    updateCommand.Parameters.AddWithValue("lastsuccess_sync", site.lastsuccess_sync);
                                    updateCommand.Parameters.AddWithValue("id", site.id);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                dr.Close();
                                // Insert new site
                                string insertQuery = "INSERT INTO SiteSyncDialog (id, end_datetime, status, message, lastsuccess_sync) VALUES (@id, @end_datetime, @status, @message, @lastsuccess_sync)";
                                using (NpgsqlCommand insertCommand = new NpgsqlCommand(insertQuery, conn))
                                {
                                    insertCommand.Parameters.AddWithValue("id", site.id);
                                    insertCommand.Parameters.AddWithValue("end_datetime", site.end_datetime);
                                    insertCommand.Parameters.AddWithValue("status", site.status);
                                    insertCommand.Parameters.AddWithValue("message", site.message);
                                    insertCommand.Parameters.AddWithValue("lastsuccess_sync", site.lastsuccess_sync);
                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    return site;
                }
                catch { throw; }
                finally
                {
                    // Ensure connection is closed
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                        Console.WriteLine("PostgreSQL connection closed.");
                    }
                }
            }
        }
        public List<SiteSync> GelAll()
        {
            List<SiteSync> lst = new List<SiteSync>();
            string connectionString = ConfigurationManager.AppSettings["constr"];
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Connected to PostgreSQL Server");

                    string query = "SELECT * FROM SiteSyncDialog WHERE status = 'Success'"; // Adjust the WHERE clause as needed
                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                var site = new SiteSync
                                {
                                    id = dr.GetInt32(dr.GetOrdinal("id")), // Adjust the field name based on your database
                                    end_datetime = dr.GetDateTime(dr.GetOrdinal("end_datetime")),
                                    status = dr.GetString(dr.GetOrdinal("status")),
                                    message = dr.GetString(dr.GetOrdinal("message")),
                                    lastsuccess_sync = dr.GetDateTime(dr.GetOrdinal("lastsuccess_sync")),
                                };
                                lst.Add(site);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
                finally
                {
                    // Ensure connection is closed
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                        Console.WriteLine("PostgreSQL connection closed.");
                    }
                }
            }
            return lst;
        }

        //public List<String> getCablesByLinkIds(string linkids)
        //{
        //    try
        //    {
        //        //return repo.ExecuteProcedure<String>("fn_cable_list_by_linkids", new { v_linkids = linkids }, false);
        //    }
        //    catch { throw; }
        //}
    }
}
