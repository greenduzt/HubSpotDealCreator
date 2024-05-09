using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using Type = HubSpotDealCreator.Models.Type;

namespace HubSpotDealCreator.Utilities
{
    public static class CreateLineItemsAndDeal
    {
        public static async Task<(Deal deal,  bool isDealCreated)> CreateNewDeal(Deal deal, IConfiguration config)
        {
            bool isDealCreated = false;
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://api.hubapi.com/crm/v3/objects/deals";
                string lineItemsApiUrl = "https://api.hubapi.com/crm/v3/objects/line_items/batch/create";

                // Set the authorization header with the API key
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                // Create a StringBuilder to construct the JSON string
                StringBuilder jsonBuilder = new StringBuilder();
                jsonBuilder.Append("{ \"inputs\": [");

                // Iterate through deal.LineItems and add each line item to the JSON
                foreach (var lineItem in deal.LineItems)
                {
                    jsonBuilder.Append("{");
                    jsonBuilder.Append("\"properties\": {");
                    jsonBuilder.Append($"\"name\": \"{lineItem.Name}\",");
                    jsonBuilder.Append($"\"description\": \"{lineItem.Name}\",");
                    jsonBuilder.Append($"\"hs_sku\": \"{lineItem.SKU}\",");
                    jsonBuilder.Append($"\"price\": \"{lineItem.UnitPrice}\",");
                    jsonBuilder.Append($"\"quantity\": \"{lineItem.Quantity}\"");
                    jsonBuilder.Append("}");
                    jsonBuilder.Append("},");
                }

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

                    //Add the company
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

                    // Step 2: Create the deal and associate it with the line item


                    DealRequest dealRequest = new DealRequest
                    {
                        properties = new DealProperties
                        {
                            dealname = deal.DealName,
                            amount = deal.Total,
                            dealstage = "209611454",//"209611454-deal created 209611456-deal won",
                            hubspot_owner_id = "1040498410",
                            deal_shipping_address = deal.DeliveryAddress.StreetAddress,
                            shipping_city = deal.DeliveryAddress.Suburb,
                            shipping_post_code = deal.DeliveryAddress.PostCode,
                            shipping_state = deal.DeliveryAddress.State,
                            shipping_country = deal.DeliveryAddress.Country,
                            order_notes = deal.OrderNotes,
                            is_the_required_date_ = "Specific",
                            site_contact_name = "NA",
                            site_contact_phone = "00000",
                            date_required = "2024-04-03",
                            transport_company = "AJM",
                            unload_type = "Tailgate",
                            freight_type = "General",
                            order_truck = "Customer",
                            pipeline = "93256906",
                            dealtype = "autogenerated",
                            po_file_access_link = deal.FileName
                        },
                        associations = lineItemAssociations.ToArray(),

                    };


                    // Set the API key in the request headers
                    client.DefaultRequestHeaders.Clear(); // Clear previous headers
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["HubSpot-API:Key"]}");

                    // Log deal request for debugging
                    string dealData = JsonConvert.SerializeObject(dealRequest);
                    Console.WriteLine($"Deal Request: {dealData}");

                    // Create the HttpContent with the JSON data and set the content type
                    HttpContent dealContent = new StringContent(dealData, Encoding.UTF8, "application/json");

                    // Make the POST request to create the deal
                    HttpResponseMessage dealResponse = await client.PostAsync(apiUrl, dealContent);

                    if (dealResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Deal created successfully!");
                    }
                    else
                    {
                        Console.WriteLine($"Error creating deal. Status code: {dealResponse.StatusCode}");
                        string errorResponse = await dealResponse.Content.ReadAsStringAsync();
                        Console.WriteLine(errorResponse);
                    }
                }
                else
                {
                    Console.WriteLine($"Error creating line item. Status code: {lineItemResponse.StatusCode}");
                    string errorResponse = await lineItemResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(errorResponse);
                }
            }

            return (deal, isDealCreated);
        }
    }
}
