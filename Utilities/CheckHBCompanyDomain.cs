using CoreLibrary.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;

namespace HubSpotDealCreator.Utilities
{
    public static class CheckHBCompanyDomain
    {
        public static async Task<(Deal,bool)> SearchDomain(Deal deal, IConfiguration config)
        {
            bool isDomainFound = false;

            if(string.IsNullOrWhiteSpace(deal.Company.Domain))
            {
                return (deal, isDomainFound);
            }

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
                        ""properties"": [""abn"",""name"",""domain"",""customer_type"",""dopricer_discount_cat_acoustic_solutions"",""dopricer_discount_cat_animal_matting"",""dopricer_discount_cat_commercial_flooring"",""dopricer_discount_cat_impact_tiles"",""dopricer_discount_cat_industrial_and_general_matting"",""dopricer_discount_cat_olympact_and_ramp_edges"",""dopricer_discount_cat_recreational_systems"",""dopricer_discount_cat_sealants_and_adhesives"",""dopricer_discount_cat_sports_surfacing_systems_and_synthetic_grass""]
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
                            deal.Company.Acoustic = result.properties.dopricer_discount_cat_acoustic_solutions;
                            deal.Company.Animals = result.properties.dopricer_discount_cat_animal_matting;
                            deal.Company.Commercial = result.properties.dopricer_discount_cat_commercial_flooring;
                            deal.Company.Impact = result.properties.dopricer_discount_cat_impact_tiles;
                            deal.Company.Industrial = result.properties.dopricer_discount_cat_industrial_and_general_matting;
                            deal.Company.Olympact = result.properties.dopricer_discount_cat_olympact_and_ramp_edges;
                            deal.Company.Recreational = result.properties.dopricer_discount_cat_recreational_systems;
                            deal.Company.Sealants = result.properties.dopricer_discount_cat_sealants_and_adhesives;
                            deal.Company.Sports = result.properties.dopricer_discount_cat_sports_surfacing_systems_and_synthetic_grass;

                            isDomainFound = true;

                            Log.Information($"Company details: ID: {deal.Company.CompanyID}, Name: {deal.Company.Name}, Domain: {deal.Company.Domain}, ABN: {deal.Company.ABN}, " +
                                $"CustomerType: {deal.Company.CustomerType}, Acoustic: {deal.Company.Acoustic}, Animal: {deal.Company.Animals}, Commercial: {deal.Company.Commercial}," +
                                $" Impact: {deal.Company.Impact}");

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
            }
           
            return (deal,isDomainFound);
        }
    }
}
