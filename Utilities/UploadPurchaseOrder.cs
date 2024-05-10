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

namespace HubSpotDealCreator.Utilities
{
    public static class UploadPurchaseOrder
    {
        public static (Deal,string,bool) UploadFile(Deal deal, IConfiguration config, List<SystemParameters> systemParameters)
        {
            bool fileCreated = false;

            var filePath = systemParameters.FirstOrDefault(x=>x.Type.Equals("po_location"));

            string fileName = Path.GetFileName(filePath.AttchmentLocation + @"\" + deal.FileName);

            string constructedFile = config["HubSpot-API:File-Upload-Location"];

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
            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(fileOptions);
            request.AddParameter("options", jsonData);

            var response = client.Execute(request);
            if (response.IsSuccessStatusCode)
            {
                StringBuilder sb = new StringBuilder();

                JObject jsonResponse = JObject.Parse(response.Content);
                string uploadedFileName = jsonResponse["objects"][0]["name"].ToString();
                //constructedFile = constructedFile + uploadedFileName.Replace(" ", "%20");

                sb.Append(constructedFile);
                sb.Append(uploadedFileName.Replace(" ", "%20"));
                sb.Append(".pdf");
                constructedFile = sb.ToString();

                deal.FileName = constructedFile;

                Console.WriteLine("File uploaded successfully.");
                Console.WriteLine("Response: " + response);
                fileCreated = true;
            }
            else
            {
                Console.WriteLine("Error uploading file. Status code: " + response.StatusCode);
            }

            return (deal, constructedFile, fileCreated);
        }
    }
}
