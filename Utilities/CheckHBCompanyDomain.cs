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
    public static class CheckHBCompanyDomain
    {
        public static async Task<(Deal deal, bool isDomainFound)> SearchDomain(Deal deal, IConfiguration config)
        {

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug() 
               .WriteTo.File(config["Logging:Path"], 
                   rollingInterval: RollingInterval.Day, 
                   restrictedToMinimumLevel: LogEventLevel.Debug) 
               .CreateLogger();

            bool isDomainFound = false;

            try 
            { 
                using (HttpClient client = new HttpClient())
                {
                    string json = @"{
                        ""filterGroups"": [
                            {
                                ""filters"": [
                                    {
                                        ""operator"": ""EQ"",
                                        ""propertyName"": ""domain"",
                                        ""value"": """ + deal.Company.Domain + @"""
                                    }
                                ]
                            }
                        ],
                        ""properties"": [""abn"",""name"",""domain"",""customer_type""]
                    }";

                    // Set the authorization header with the API key
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                    var contentByDomain = new StringContent(json, Encoding.UTF8, "application/json");
                    // Make the POST request to search for the company by name
                    HttpResponseMessage responseByDomain = await client.PostAsync("https://api.hubapi.com/crm/v3/objects/companies/search", contentByDomain);
                    if (responseByDomain.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string responseDomainBody = await responseByDomain.Content.ReadAsStringAsync();

                        // Deserialize the JSON response
                        dynamic jsonDomainResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseDomainBody);
                        // Extract the results
                        int count = jsonDomainResponse.total;
                        dynamic[] results = jsonDomainResponse.results.ToObject<dynamic[]>();

                        Log.Information($"Total companies found for domain {deal.Company.Domain}: {count}");

                        if (count > 0)
                        {
                            dynamic result = jsonDomainResponse.results[0];
                            deal.Company.CompanyID = result.id;
                            deal.Company.Name = result.properties.name;
                            deal.Company.Domain = result.properties.domain;
                            deal.Company.ABN = result.properties.abn;
                            deal.Company.CustomerType = result.properties.customer_type;

                            isDomainFound = true;

                            Log.Debug($"Company details: ID: {deal.Company.CompanyID}, Name: {deal.Company.Name}, Domain: {deal.Company.Domain}, ABN: {deal.Company.ABN}, CustomerType: {deal.Company.CustomerType}");

                        }
                        else
                        {
                            isDomainFound = false;
                        }
                    }
                    else
                    {
                        Log.Error($"Error searching by domain. Status code: {responseByDomain.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                Log.Error(ex, "An error occurred while searching by domain");
                throw; // Rethrow the exception to be handled by the caller
            }
            finally
            {
                // Close and flush the Serilog logger
                Log.CloseAndFlush();
            }
            return (deal,  isDomainFound);
        }
    }
}
