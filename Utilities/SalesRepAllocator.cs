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

                        // Check if Deal.SalesRepName available, if so get the owner id
                        if (!string.IsNullOrWhiteSpace(deal.SalesRepName))
                        {
                            var owner = ownersArray.FirstOrDefault(x => x["email"]?.ToString().Equals(deal.SalesRepName, StringComparison.OrdinalIgnoreCase) ==true);
                            if (owner != null)
                            {
                                deal.OwnerId = owner["ownerId"]?.ToString();
                                isSalesRepFound = true;

                                return (deal, isSalesRepFound);
                            }
                        }                        
                        // Check if Deal.SalesRepName is not available, check ownersArray against deal.Emails
                        foreach (var owner in ownersArray)
                        {
                            string email = owner["email"]?.ToString();
                            string ownerId = owner["ownerId"]?.ToString();

                            if (deal.Emails.Any(x => x.Equals(email, StringComparison.OrdinalIgnoreCase)))
                            {
                                // Allocating the sales rep to the deal
                                deal.OwnerId = ownerId;
                                isSalesRepFound = true;
                                return (deal, isSalesRepFound);                                
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

            if(!isSalesRepFound)
            {
                deal.OrderNotes += "**************" + System.Environment.NewLine + "Could not locate the sales rep";
            }

            return (deal, isSalesRepFound);
        }
    }
}
