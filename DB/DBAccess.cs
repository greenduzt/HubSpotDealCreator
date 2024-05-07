using HubSpotDealCreator.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.DB
{
    public static class DBAccess
    {
        public static List<HubSpotProduct> LoadHubSpotProducts()
        {
            List<HubSpotProduct> hubSpotProductList = new List<HubSpotProduct>();
            
            // Create a SqlConnection object
            using (SqlConnection conn = new SqlConnection(DBConfiguration.GetConnectionString()))
            {
                try
                {
                    // Open the connection
                    conn.Open();

                    // Perform database operations here
                    Console.WriteLine("Connected to SQL Server!");

                    // Example: Execute a simple query
                    string sqlQuery = "SELECT * FROM HubSpotProducts";
                    using (SqlCommand command = new SqlCommand(sqlQuery, conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HubSpotProduct hsp = new HubSpotProduct();
                                hsp.RecordID = reader["record_id"].ToString();
                                hsp.Name = reader["name"].ToString();
                                hsp.SKU = reader["sku"].ToString();
                                hsp.ProductDescription = reader["product_description"].ToString();
                                hsp.Unit = reader["unit"].ToString();
                                hsp.Price = Convert.ToDouble(reader["price"]);
                                hsp.ProductCategory = reader["product_category"].ToString();
                                hubSpotProductList.Add(hsp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return hubSpotProductList;
        }

        public static List<SystemParameters> GetSystemParameters()
        {
            List<SystemParameters> systemParameters = new List<SystemParameters>();

            // Create a SqlConnection object
            using (SqlConnection conn = new SqlConnection(DBConfiguration.GetConnectionString()))
            {
                try
                {
                    conn.Open();

                    using (SqlCommand command = new SqlCommand("SELECT * FROM SystemParameters", conn))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SystemParameters sp = new SystemParameters();
                                sp.Type = reader["type"].ToString();
                                sp.AttchmentLocation = reader["attachment_location"].ToString();
                                systemParameters.Add(sp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return systemParameters;
        }

        public static async Task<List<SystemParameters>> GetSystemParametersAsync()
        {
            List<SystemParameters> systemParameters = new List<SystemParameters>();

            // Create a SqlConnection object
            using (SqlConnection conn = new SqlConnection(DBConfiguration.GetConnectionString()))
            {
                try
                {
                    await conn.OpenAsync(); // Asynchronously open the connection

                    using (SqlCommand command = new SqlCommand("SELECT * FROM SystemParameters", conn))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync()) // Asynchronously execute the query
                        {
                            while (await reader.ReadAsync()) // Asynchronously read each row
                            {
                                SystemParameters sp = new SystemParameters();
                                sp.Type = reader["type"].ToString();
                                sp.AttchmentLocation = reader["attachment_location"].ToString();
                                systemParameters.Add(sp);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return systemParameters;
        }

    }
}
