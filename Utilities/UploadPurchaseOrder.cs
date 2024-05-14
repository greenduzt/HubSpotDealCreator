using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace HubSpotDealCreator.Utilities
{
    public static class UploadPurchaseOrder
    {
        public static Deal UploadFile(Deal deal, IConfiguration config, List<SystemParameters> systemParameters)
        {
            bool fileCreated = false;
            string constructedFile = string.Empty;

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.File(config["Logging:Path"],
                   rollingInterval: RollingInterval.Day,
                   restrictedToMinimumLevel: LogEventLevel.Debug, 
                   shared: true)
               .CreateLogger();

            try
            {
                var filePath = systemParameters.FirstOrDefault(x => x.Type.Equals("po_location"));
                string fileName = Path.GetFileName(filePath.AttchmentLocation + @"\" + deal.FileName);

                // Check if deal.FileName is null
                if (string.IsNullOrWhiteSpace(deal.FileName))
                {
                    Log.Information("Deal file name is not available.");
                    return deal;
                }

                var file = File.ReadAllBytes(filePath.AttchmentLocation + @"\" + fileName);
                var client = new RestClient("https://api.hubapi.com/");
                var request = new RestRequest("/filemanager/api/v3/files/upload", Method.Post);

                client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", config["HubSpot-API:Key"]));

                request.AddFile("file", file, fileName, "application/octet-stream");
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
                    fileCreated = true;
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
            finally
            {
                // Close and flush the Serilog logger
                Log.CloseAndFlush();
            }

            return deal;
        }
    }
}
