using HubSpotDealCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Services
{
    public class CheckHBCompanyName
    {
        public static async Task<Dictionary<Deal, bool>> SearchCompanyName(string apiKey,  Deal deal)
        {
            bool isCompanyFound = false;

            using (HttpClient client = new HttpClient())
            {
                
                // Set the authorization header with the API key
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

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

                // Create the content for the POST request for searching by name
                var contentByName = new StringContent(json, Encoding.UTF8, "application/json");

                // Make the POST request to search for the company by name
                HttpResponseMessage responseByName = await client.PostAsync("https://api.hubapi.com/crm/v3/objects/companies/search", contentByName);

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
                    Console.WriteLine($"Total companies found: {count}");
                    if (count > 0)
                    {
                        dynamic result = jsonNameResponse.results[0];
                        deal.Company.CompanyID = result.id;
                        deal.Company.Name = result.properties.name;
                        deal.Company.Domain = result.properties.domain;
                        deal.Company.ABN = result.properties.abn;
                        deal.Company.CustomerType = result.properties.customer_type;
                        isCompanyFound = true;
                    }
                    else
                    {
                        isCompanyFound = false;
                    }
                }
                else
                {
                    Console.WriteLine($"Error searching by name. Status code: {responseByName.StatusCode}");
                }
            }

            Dictionary<Deal, bool> response = new Dictionary<Deal, bool>(); 
            response.Add(deal, isCompanyFound);

            return response;
        }
    }
}
