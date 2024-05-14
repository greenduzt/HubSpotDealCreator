using Amazon.Runtime.Internal.Util;
using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Utilities
{
    public static class CheckHBCompanyName
    {
        public static async Task<bool> SearchCompanyName(Deal deal, IConfiguration config)
        {  
            using (HttpClient client = new HttpClient())
            {
                string json = @"{
                    ""filterGroups"": [
                        {
                            ""filters"": [
                                {
                                    ""operator"": ""EQ"",
                                    ""propertyName"": ""name"",
                                    ""value"": """ + deal.Company.Name + @"""                                    
                                }
                            ]
                        }
                    ],
                    ""properties"": [""abn"",""name"",""domain"",""customer_type""]
                }";
                bool isCompanyFound = false;

                try
                {

                    // Set the authorization header with the API key
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                    // Create the content for the POST request for searching by name
                    var contentByName = new StringContent(json, Encoding.UTF8, "application/json");

                    // Make the POST request to search for the company by name
                    HttpResponseMessage responseByName = await client.PostAsync("https://api.hubapi.com/crm/v3/objects/companies/search", contentByName);

                    Log.Debug($"SearchCompanyName request made for company name: {deal.Company.Name}");

                    /**********CHECKING COMPANY NAME***********/
                    if (responseByName.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string responseNameBody = await responseByName.Content.ReadAsStringAsync();

                        // Deserialize the JSON response
                        dynamic jsonNameResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseNameBody);

                        // Extract the results
                        int count = jsonNameResponse.total;
                        dynamic[] results = jsonNameResponse.results.ToObject<dynamic[]>();

                        // Display the results
                        Log.Information($"Total companies found for {deal.Company.Name}: {count}");
                        if (count > 0)
                        {
                            dynamic result = jsonNameResponse.results[0];
                            deal.Company.CompanyID = result.id;
                            deal.Company.Name = result.properties.name;
                            deal.Company.Domain = result.properties.domain;
                            deal.Company.ABN = result.properties.abn;
                            deal.Company.CustomerType = result.properties.customer_type;
                            isCompanyFound = true;

                            Log.Information($"Company details: ID: {deal.Company.CompanyID}, Name: {deal.Company.Name}, Domain: {deal.Company.Domain}, ABN: {deal.Company.ABN}, CustomerType: {deal.Company.CustomerType}");
                        }
                        else
                        {
                            isCompanyFound = false;
                        }
                    }
                    else
                    {
                        Log.Error($"Error searching by name for company: {deal.Company.Name}. Status code: {responseByName.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    // Log any exceptions
                    Log.Error(ex, "An error occurred while searching for company name");
                }              

                return isCompanyFound;
            }
        }
    }
}
