using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
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
            bool isABNFound = false;
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
                    // Display the results
                    Console.WriteLine($"Total companies found: {count}");
                    if (count > 0)
                    {
                        dynamic result = jsonABNResponse.results[0];
                        deal.Company.CompanyID = result.id;
                        deal.Company.Name = result.properties.name;
                        deal.Company.Domain = result.properties.domain;
                        deal.Company.ABN = result.properties.abn;
                        deal.Company.CustomerType = result.properties.customer_type;
                        isABNFound = true;
                    }
                    else
                    {
                        isABNFound = false;
                    }
                }
                else
                {
                    Console.WriteLine($"Error searching by ABN. Status code: {responseByABN.StatusCode}");
                }
            }
            return (deal, isABNFound);
        }
    }
}
