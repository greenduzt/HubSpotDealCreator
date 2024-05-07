﻿using HubSpotDealCreator.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Services
{
    public static class POUpload
    {
        public static string UploadFile(string fn, IConfiguration config, List<SystemParameters> sp)
        {
            var locStr = sp.FirstOrDefault(x => x.Type.Equals("po_location"));

            string fileWithPath = locStr.AttchmentLocation + "/"+fn;
            string fileName = Path.GetFileName(fn);

            string constructedFile = config["HubSpot-API:File-Upload-Location"];

            var file = File.ReadAllBytes(fileWithPath);
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
               
                sb.Append(constructedFile);
                sb.Append(uploadedFileName.Replace(" ", "%20"));
                sb.Append(".pdf");
                constructedFile = sb.ToString();

                Console.WriteLine("File uploaded successfully.");
                Console.WriteLine("Response: " + response);
            }
            else
            {
                Console.WriteLine("Error uploading file. Status code: " + response.StatusCode);
            }

            return constructedFile;
        }
    }
}