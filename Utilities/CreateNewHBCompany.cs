using CoreLibrary.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace HubSpotDealCreator.Utilities
{
    public static class CreateNewHBCompany
    {
        public static async Task<(Deal deal, bool companyCreated)> CreateNewCompany(Deal deal, IConfiguration config)
        {
            bool companyCreated = false;

            try
            {
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
                        dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);
                        deal.Company.CompanyID = jsonResponse.id;

                        // Log company creation success
                        Log.Information("Company created successfully for the company {Name} with ID: {CompanyId}", deal.Company.Name,deal.Company.CompanyID);
                        companyCreated = true;
                    }
                    else
                    {
                        // Handle the case when the request was not successful
                        Log.Error("Error creating company. Status code: {StatusCode}", responseCompany.StatusCode);
                        string errorResponse = await responseCompany.Content.ReadAsStringAsync();
                        Log.Error("Error response: {Error}", errorResponse);
                        companyCreated = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the process
                Log.Error(ex, "An error occurred while creating the company.");
                throw; // Rethrow the exception to be handled by the caller
            }            

            return (deal, companyCreated);
        }
    }
}
