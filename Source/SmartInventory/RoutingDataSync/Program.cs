using Npgsql;
using System;
using System.Configuration;

namespace DataTransferApi
{
    class Program
    {
        static void Main(string[] args)
        {
            string hostaddr = "", dbname = "", user = "", password = "";
            string queryDataCreation;

            WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], DateTime.Now.ToString());
            try
            {
                Console.WriteLine("Getting Connection ...");

                string SourceConnString = ConfigurationManager.AppSettings["SourceConnString"];
                string DestinationConnString = ConfigurationManager.AppSettings["DestinationConnString"];
                bool IsServerDifferent = ConfigurationManager.AppSettings["IsServerDifferent"].Equals("1");
                string DestinationRoutingTable = ConfigurationManager.AppSettings["DestinationRoutingTable"];

                NpgsqlConnection SourceConn = new NpgsqlConnection(SourceConnString);
                NpgsqlConnection DestinationConn = new NpgsqlConnection(DestinationConnString);

                if (IsServerDifferent)
                {
                    string[] parts = SourceConnString.Split(';');
                    foreach (string part in parts)
                    {
                        string[] subParts = part.Trim().Split('=');
                        if (subParts[0].Equals("Server", StringComparison.OrdinalIgnoreCase))
                            hostaddr = subParts[1];
                        else if (subParts[0].Equals("Database", StringComparison.OrdinalIgnoreCase))
                            dbname = subParts[1];
                        else if (subParts[0].Equals("User Id", StringComparison.OrdinalIgnoreCase))
                            user = subParts[1];
                        else if (subParts[0].Equals("Password", StringComparison.OrdinalIgnoreCase))
                            password = subParts[1];
                    }

                    queryDataCreation = "select * from fn_fs_create_routingData('" + DestinationRoutingTable + "', '" + hostaddr + "', '" + dbname + "', '" + user + "', '" + password + "')";
                }
                else
                {
                    queryDataCreation = "select * from fn_fs_create_routingData('" + DestinationRoutingTable + "')";
                }

                Console.WriteLine("Connecting to Destination Server ...");

                //open connection
                DestinationConn.Open();
                Console.WriteLine("Connection successful!");

                NpgsqlCommand command = new NpgsqlCommand(queryDataCreation, DestinationConn);
                NpgsqlDataReader dr = command.ExecuteReader();

                if (dr.Read())
                {
                    if ((bool)dr[0])
                    {
                        Console.WriteLine(dr[1]);
                        WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], dr[1].ToString());

                        dr.Close();

                        Console.WriteLine("\nNoded network creation started...");
                        Console.WriteLine("Please wait ...");

                        command = new NpgsqlCommand("select * from fn_fs_create_routing_nodes()", DestinationConn);
                        dr = command.ExecuteReader();

                        if (dr.Read())
                        {
                            if ((bool)dr[0])
                            {
                                Console.WriteLine(dr[1]);
                                WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], dr[1].ToString());
                                dr.Close();
                                DestinationConn.Close();
                            }
                            else
                            {
                                Console.WriteLine("\n FAILED!");
                                Console.WriteLine(dr[1]);
                                WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], "FAILED!");
                                WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], dr[1].ToString());
                            }
                        }   
                    }
                    else
                    {
                        Console.WriteLine("\n FAILED!");
                        Console.WriteLine(dr[1]);
                        WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], "FAILED!");
                        WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], dr[1].ToString());
                    }
                }
                else
                {
                    Console.WriteLine("\n Error occurred while reading the result!");
                    WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], "Error occurred while reading the result!");
                }
            }
            catch (Exception e)
            {
                WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], e.Message);
                Console.WriteLine("Error: " + e.Message);
            }
            WriteLog.WriteLogFile(ConfigurationManager.AppSettings["LogFilePath"], "------------------------------");
        }
    }
}
