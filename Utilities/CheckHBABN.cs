using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Utilities
{
    public static class CheckHBABN
    {
        public static async Task<(Deal deal, bool isABNFound)> SearchABN(Deal deal, IConfiguration config)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set minimum log level to Debug
                .WriteTo.File(config["Logging:Path"], // Specify file name
                    rollingInterval: RollingInterval.Day, // Specify rolling interval
                    restrictedToMinimumLevel: LogEventLevel.Debug) // Specify minimum log level
                .CreateLogger();

            bool isABNFound = false;

            try
            {
                string json = @"{
                    ""filterGroups"": [
                        {
                            ""filters"": [
                                {
                                    ""operator"": ""EQ"",
                                    ""propertyName"": ""abn"",
                                    ""value"": """ + deal.Company.ABN + @"""
                                }
                            ]
                        }
                    ],
                    ""properties"": [""abn"",""name"",""domain"",""customer_type""]
                }";

                using (HttpClient client = new HttpClient())
                {
                    // Set the authorization header with the API key
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                    var contentByABN = new StringContent(json, Encoding.UTF8, "application/json");
                    // Make the POST request to search for the company by name
                    HttpResponseMessage responseByABN = await client.PostAsync("https://api.hubapi.com/crm/v3/objects/companies/search", contentByABN);
                    if (responseByABN.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string responseABNBody = await responseByABN.Content.ReadAsStringAsync();

                        // Deserialize the JSON response
                        dynamic jsonABNResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseABNBody);
                        // Extract the results
                        int count = jsonABNResponse.total;
                        dynamic[] results = jsonABNResponse.results.ToObject<dynamic[]>();

                        Log.Information($"Total companies found for ABN {deal.Company.ABN}: {count}");

                        if (count > 0)
                        {
                            dynamic result = jsonABNResponse.results[0];
                            deal.Company.CompanyID = result.id;
                            deal.Company.Name = result.properties.name;
                            deal.Company.Domain = result.properties.domain;
                            deal.Company.ABN = result.properties.abn;
                            deal.Company.CustomerType = result.properties.customer_type;
                            isABNFound = true;

                            Log.Debug($"Company details: ID: {deal.Company.CompanyID}, Name: {deal.Company.Name}, Domain: {deal.Company.Domain}, ABN: {deal.Company.ABN}, CustomerType: {deal.Company.CustomerType}");

                        }
                        else
                        {
                            isABNFound = false;
                        }
                    }
                    else
                    {
                        Log.Error($"Error searching by ABN {deal.Company.ABN}. Status code: {responseByABN.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                Log.Error(ex, "An error occurred while searching by ABN");
            }
            finally
            {
                // Close and flush the Serilog logger
                Log.CloseAndFlush();
            }
            return (deal, isABNFound);
        }
    }
}
