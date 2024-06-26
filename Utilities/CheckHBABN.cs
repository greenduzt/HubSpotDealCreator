﻿using CoreLibrary.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace HubSpotDealCreator.Utilities
{
    public static class CheckHBABN
    {
        public static async Task<(Deal,bool)> SearchABN(Deal deal, IConfiguration config)
        {
            string pattern = @"[^a-zA-Z0-9]+";
            bool isABNFound = false;

            if(string.IsNullOrWhiteSpace(deal.Company.ABN))
            {
                return (deal, false);
            }

            deal.Company.ABN = Regex.Replace(deal.Company.ABN, pattern, "").Trim();
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
                    ""properties"": [""abn"",""name"",""domain"",""customer_type"",""dopricer_discount_cat_acoustic_solutions"",""dopricer_discount_cat_animal_matting"",""dopricer_discount_cat_commercial_flooring"",""dopricer_discount_cat_impact_tiles"",""dopricer_discount_cat_industrial_and_general_matting"",""dopricer_discount_cat_olympact_and_ramp_edges"",""dopricer_discount_cat_recreational_systems"",""dopricer_discount_cat_sealants_and_adhesives"",""dopricer_discount_cat_sports_surfacing_systems_and_synthetic_grass""]
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
                            deal.Company.Acoustic = result.properties.dopricer_discount_cat_acoustic_solutions;
                            deal.Company.Animals = result.properties.dopricer_discount_cat_animal_matting;
                            deal.Company.Commercial = result.properties.dopricer_discount_cat_commercial_flooring;
                            deal.Company.Impact = result.properties.dopricer_discount_cat_impact_tiles;
                            deal.Company.Industrial = result.properties.dopricer_discount_cat_industrial_and_general_matting;
                            deal.Company.Olympact = result.properties.dopricer_discount_cat_olympact_and_ramp_edges;
                            deal.Company.Recreational = result.properties.dopricer_discount_cat_recreational_systems;
                            deal.Company.Sealants = result.properties.dopricer_discount_cat_sealants_and_adhesives;
                            deal.Company.Sports = result.properties.dopricer_discount_cat_sports_surfacing_systems_and_synthetic_grass;

                            isABNFound = true;

                            Log.Information($"Company details: ID: {deal.Company.CompanyID}, Name: {deal.Company.Name}, Domain: {deal.Company.Domain}, ABN: {deal.Company.ABN}, " +
                                                            $"CustomerType: {deal.Company.CustomerType}, Acoustic: {deal.Company.Acoustic}, Animal: {deal.Company.Animals}, Commercial: {deal.Company.Commercial}," +
                                                            $" Impact: {deal.Company.Impact}");
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
           
            return (deal,isABNFound);
        }
    }
}
