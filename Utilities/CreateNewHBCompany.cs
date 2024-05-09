using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Utilities
{
    public static class CreateNewHBCompany
    {
        public static async Task<(Deal deal, bool companyCreated)> CreateNewCompany( Deal deal, IConfiguration config)
        {
            bool companyCreated = false;

            using (HttpClient client = new HttpClient())
            {
                // Set the authorization header with the API key
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                string jsonCompanyPayload = @"
                                            {
                                                ""properties"": {
                                                    ""name"": """ + deal.Company.Name + @""",
                                                    ""domain"": """ + deal.Company.Domain + @""",
                                                    ""abn"" : """ + deal.Company.ABN + @"""
                                                }
                                            }";

                // Create the content for the POST request
                var content = new StringContent(jsonCompanyPayload, Encoding.UTF8, "application/json");

                // Make the POST request to create the company
                HttpResponseMessage responseCompany = await client.PostAsync("https://api.hubapi.com/crm/v3/objects/companies", content);
                // Check if the request was successful (status code 200) and process the response...
                if (responseCompany.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string responseBody = await responseCompany.Content.ReadAsStringAsync();

                    // Deserialize the JSON response to extract the record ID
                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                    deal.Company.CompanyID = jsonResponse.id;

                    // Process the response (extract relevant data, etc.)
                    Console.WriteLine("Company created successfully with ID: " + deal.Company.CompanyID);
                    companyCreated = true;
                }
                else
                {
                    // Handle the case when the request was not successful
                    Console.WriteLine($"Error creating company. Status code: {responseCompany.StatusCode}");
                    string errorResponse = await responseCompany.Content.ReadAsStringAsync();
                    Console.WriteLine(errorResponse);
                    companyCreated = false;
                }
            }

            return (deal, companyCreated);
        }
    }
}
