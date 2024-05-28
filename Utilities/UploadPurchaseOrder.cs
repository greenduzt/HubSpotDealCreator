using CoreLibrary.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;

namespace HubSpotDealCreator.Utilities
{
    public static class UploadPurchaseOrder
    {
        public static Deal UploadFile(Deal deal, IConfiguration config)
        {
            string constructedFile = string.Empty;          

            try
            {   
                // Check if deal.FileName is null
                if (string.IsNullOrWhiteSpace(deal.FileName))
                {
                    Log.Information("Deal file name is not available.");
                    return deal;
                }

                var file = File.ReadAllBytes(deal.FilePath);
                var client = new RestClient("https://api.hubapi.com/");
                var request = new RestRequest("/filemanager/api/v3/files/upload", Method.Post);

                client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", config["HubSpot-API:Key"]));

                request.AddFile("file", file, deal.FileName, "application/octet-stream");
                request.AddParameter("folderPath", "/PO");

                var fileOptions = new
                {
                    access = @"PUBLIC_NOT_INDEXABLE",
                };
                // Transform it to JSON object
                string jsonData = JsonConvert.SerializeObject(fileOptions);
                request.AddParameter("options", jsonData);

                var response = client.Execute(request);

                if (response.IsSuccessStatusCode)
                {
                    JObject jsonResponse = JObject.Parse(response.Content);
                    string uploadedFileName = jsonResponse["objects"][0]["name"].ToString();

                    // Construct the file path
                    constructedFile = $"{config["HubSpot-API:File-Upload-Location"]}{uploadedFileName.Replace(" ", "%20")}.pdf";
                    deal.FileName = constructedFile;

                    Log.Information("File uploaded successfully.");
                    Log.Information("Constructed File Path: " + constructedFile);
                }
                else
                {
                    Log.Error("Error uploading file. Status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                Log.Error(ex, "An error occurred while uploading the file.");
            }
            
            return deal;
        }
    }
}
