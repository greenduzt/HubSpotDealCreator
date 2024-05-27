using CoreLibrary.Data;
using CoreLibrary.Data.Repositories;
using CoreLibrary.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using Type = CoreLibrary.Models.Type;

namespace HubSpotDealCreator.Utilities
{
    public static class CreateLineItemsAndDeal
    {
        public static HubSpotProduct ValidateLineItem(List<HubSpotProduct> hubSpotProductList, LineItems lineItem)
        {
            if(lineItem == null)
            {
                Log.Error("Lineitem not found!");
                return null;
            }
            
            if (string.IsNullOrWhiteSpace(lineItem.ExpenseRaw))
            {
                Log.Error("Expense row not found!");
                return null;
            }

            string pattern = @"[^a-zA-Z0-9%]+";
            var expenseRawWords = Regex.Replace(lineItem.ExpenseRaw, pattern, " ").Trim().ToLower();
            var wordList = expenseRawWords.Split(' ').ToList();

            foreach (var item in hubSpotProductList)
            { 
                var prodSplitList = item.SKU.Split(' ').ToList();
                if (prodSplitList.Any())
                {
                    int pCount = prodSplitList.Count(), c = 0,prev=0;
                    for (int i = 0; i < prodSplitList.Count; i++)
                    {
                        if (pCount > 0)
                        {
                            c++;

                            int currentIndex = wordList.FindIndex(q => q.ToLower().Equals(prodSplitList[i].ToLower()));
                            if(currentIndex != -1)
                            {
                                if (pCount > 1)
                                {
                                    if ((prev + 1) == currentIndex)
                                    {
                                        if (pCount == c)
                                        {
                                            return item;
                                        }
                                    }
                                }
                                else
                                {
                                    if (pCount == c)
                                    {
                                        return item;
                                    }
                                }

                                // If match found save the index to prev variable
                                prev = currentIndex;
                            }            
                        }
                    }
                }
            }

            return null;
        }

        public static async Task<(Deal deal, bool isDealCreated)> CreateNewDeal(Deal deal, IConfiguration config)
        {
            bool isDealCreated = false;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var services = ConfigureServices(config);
                    using (var serviceProvider = services.BuildServiceProvider())
                    {
                        var hubSpotProductsRepository = serviceProvider.GetRequiredService<IHubSpotProductRepository>();
                        var hubSpotProdRepoRes = await hubSpotProductsRepository.GetAllAsync();
                        string pattern = @"[^a-zA-Z0-9%]+";

                        if (hubSpotProdRepoRes == null || !hubSpotProdRepoRes.Any())
                        {
                            Log.Error($"Could not load HubSpot products");

                            return (deal,false);
                        }
                        
                        string apiUrl = "https://api.hubapi.com/crm/v3/objects/deals";
                        string lineItemsApiUrl = "https://api.hubapi.com/crm/v3/objects/line_items/batch/create";

                        
                        // Set the authorization header with the API key
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                        // Create a StringBuilder to construct the JSON string
                        StringBuilder jsonBuilder = new StringBuilder();
                        jsonBuilder.Append("{ \"inputs\": [");

                        StringBuilder lineItemsInfoBuilder = new StringBuilder();

                        // Iterate through deal.LineItems and add each line item to the JSON
                        foreach (var lineItem in deal.LineItems)
                        {                            
                            // Validate the line items against hubspot line items
                            HubSpotProduct hubSpotProdValidated = ValidateLineItem(hubSpotProdRepoRes.ToList(), lineItem) ?? new HubSpotProduct()
                            {                             
                                Name = "Not Found",
                                SKU = "Not Found",
                                ProductDescription = "Not Found"
                            };                            
                            
                            jsonBuilder.Append("{");
                            jsonBuilder.Append("\"properties\": {");
                            jsonBuilder.Append($"\"name\": \"{hubSpotProdValidated.Name}\",");
                            jsonBuilder.Append($"\"description\": \"{hubSpotProdValidated.ProductDescription}\",");
                            jsonBuilder.Append($"\"hs_sku\": \"{hubSpotProdValidated.SKU}\",");
                            jsonBuilder.Append($"\"price\": \"{lineItem.UnitPrice}\",");
                            jsonBuilder.Append($"\"quantity\": \"{lineItem.Quantity}\"");
                            jsonBuilder.Append("}");
                            jsonBuilder.Append("},");
                            lineItemsInfoBuilder.Append($"[Name : {hubSpotProdValidated.Name}|SKU : {hubSpotProdValidated.SKU}|Price : {lineItem.UnitPrice}|Quantity : {lineItem.Quantity}]");
                        }

                        Log.Information($"Deal Line Items : {lineItemsInfoBuilder.ToString()}");

                        // Remove the trailing comma if there are line items
                        if (deal.LineItems.Any())
                        {
                            jsonBuilder.Length--; // Remove the last character (comma)
                        }

                        // Close the JSON array
                        jsonBuilder.Append("] }");

                        // Convert StringBuilder to string
                        string jsonBody = jsonBuilder.ToString();

                        HttpContent lineItemContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                        HttpResponseMessage lineItemResponse = await client.PostAsync(lineItemsApiUrl, lineItemContent);
                        List<long> lineItemIds = new List<long>();
                        List<Association> lineItemAssociations = new List<Association>();

                        if (lineItemResponse.IsSuccessStatusCode)
                        {
                            string lineItemResponseContent = await lineItemResponse.Content.ReadAsStringAsync();
                            dynamic lineItemObjects = JsonConvert.DeserializeObject(lineItemResponseContent);

                            foreach (var input in lineItemObjects.results)
                            {
                                Association lineItemAssociation = new Association
                                {
                                    to = new To { id = Convert.ToInt64(input.id) },
                                    types = new Type[]
                                    {
                                    new Type
                                    {
                                        associationCategory = "HUBSPOT_DEFINED",
                                        associationTypeId = 19 // 19 is the association type ID for line items
                                    }
                                    }
                                };

                                // Add the Association object to the list
                                lineItemAssociations.Add(lineItemAssociation);
                            }

                            // Add the company
                            lineItemAssociations.Add(new Association
                            {
                                to = new To { id = deal.Company.CompanyID },
                                types = new Type[]
                                {
                                new Type
                                {
                                    associationCategory = "HUBSPOT_DEFINED",
                                    associationTypeId = 5  // 5 is the association type ID for companies
                                }
                                }
                            });

                            // Create the deal request
                            DealRequest dealRequest = new DealRequest
                            {
                                properties = new DealProperties
                                {
                                    dealname = deal.DealName,
                                    amount = deal.Total,
                                    dealstage = "209611454",//"209611454-deal created 209611456-deal won",
                                    hubspot_owner_id = deal.OwnerId,
                                    deal_shipping_address = deal.DeliveryAddress.StreetAddress,
                                    shipping_city = deal.DeliveryAddress.Suburb,
                                    shipping_post_code = deal.DeliveryAddress.PostCode,
                                    shipping_state = deal.DeliveryAddress.State,
                                    shipping_country = deal.DeliveryAddress.Country,
                                    order_notes = deal.OrderNotes,
                                    is_the_required_date_ = "",
                                    site_contact_name = "",
                                    site_contact_phone = "",
                                    date_required = deal.DeliveryDate,
                                    transport_company = "",
                                    unload_type = "",
                                    freight_type = "",
                                    order_truck = "",
                                    pipeline = "93256906",
                                    dealtype = "autogenerated",
                                    po_file_access_link = deal.FileName
                                },
                                associations = lineItemAssociations.ToArray()
                            };

                            // Set the API key in the request headers
                            client.DefaultRequestHeaders.Clear(); // Clear previous headers
                            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                            // Log deal request for debugging
                            string dealData = JsonConvert.SerializeObject(dealRequest);
                            Log.Information($"Deal Name : {deal.DealName}|Amount : {deal.Total} " +
                                $"|HubSpot Owner ID : {dealRequest.properties.hubspot_owner_id} " +
                                $"|Shipping Add : {deal.DeliveryAddress.StreetAddress}" +
                                $"|Shipping City : {deal.DeliveryAddress.Suburb}" +
                                $"|Shipping PostCode : {deal.DeliveryAddress.PostCode}" +
                                $"|Shipping State : {deal.DeliveryAddress.State}" +
                                $"|Shipping Country : {deal.DeliveryAddress.Country}" +
                                $"|Order Notes : {deal.OrderNotes}" +
                                $"|PO URL : {deal.FileName}");

                            // Create the HttpContent with the JSON data and set the content type
                            HttpContent dealContent = new StringContent(dealData, Encoding.UTF8, "application/json");

                            // Make the POST request to create the deal
                            HttpResponseMessage dealResponse = await client.PostAsync(apiUrl, dealContent);

                            if (dealResponse.IsSuccessStatusCode)
                            {
                                isDealCreated = true;
                                Log.Information("Deal created successfully!");
                            }
                            else
                            {
                                Log.Error($"Error creating deal. Status code: {dealResponse.StatusCode}");
                                string errorResponse = await dealResponse.Content.ReadAsStringAsync();
                                Log.Error(errorResponse);
                            }
                        }
                        else
                        {
                            Log.Error($"Error creating line item. Status code: {lineItemResponse.StatusCode}");
                            string errorResponse = await lineItemResponse.Content.ReadAsStringAsync();
                            Log.Error(errorResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating the deal.");
                //throw; // Rethrow the exception to be handled by the caller
            }            

            return (deal, isDealCreated);
        }

        private static IServiceCollection ConfigureServices(IConfiguration config)
        {
            var services = new ServiceCollection();
            services.AddDataAccessServices(config); // Pass IConfiguration
            return services;
        }

        private static string CleanString(string input, string pattern,string type)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Not Found";
            }
            return Regex.Replace(input,pattern, " ").Trim();
        }
    }
}
