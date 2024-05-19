using CoreLibrary.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;

namespace HubSpotDealCreator.Utilities
{
    public static class SalesRepAllocator
    {
        public static async Task<(Deal,bool)> AllocateSalesRepToDeal(Deal deal, IConfiguration config)
        {
            bool isSalesRepFound = false;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                    HttpResponseMessage response = await client.GetAsync("https://api.hubapi.com/owners/v2/owners");

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JArray ownersArray = JArray.Parse(responseBody);

                        foreach (var owner in ownersArray)
                        {
                            string email = (string)owner["email"];
                            string ownerId = (string)owner["ownerId"];

                            if(deal.Emails.Any(x=>x.Equals(email,StringComparison.OrdinalIgnoreCase)))
                            {
                                // Allocating the sales rep to the deal
                                deal.OwnerId = ownerId;
                                isSalesRepFound = true;
                                break; // Exit the loop once a sales rep is found
                            }
                        }
                    }
                    else
                    {
                        // Handle error
                        Log.Error($"Error retrieving Sales Pro users. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                Log.Error(ex, "An error occurred while retrieving Sales Pro users");
            }



            return (deal,isSalesRepFound);
        }
    }
}
