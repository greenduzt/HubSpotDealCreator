using HubSpotDealCreator.DB;
using HubSpotDealCreator.JSON;
using HubSpotDealCreator.Models;
using HubSpotDealCreator.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Builders
{
    public class ProgramBuilder : IProgramBuilder
    {
        private Program program;
        private string connectionString;

        public ProgramBuilder()
        {
            program = new Program();
        }

        public async Task<Program> BuildAsync() // Modify to return Task<Program>
        {
            // Construct Program instance
            Program program = new Program
            {
                systemParameters = await GetSystemParametersAsync()
            };

            // Additional initialization logic here...

            return await Task.FromResult(program);
        }

        private async Task<List<SystemParameters>> GetSystemParametersAsync()
        {
            // Example asynchronous method to fetch system parameters from a database or external service
            // Replace this with your actual implementation
            return await DBAccess.GetSystemParametersAsync();
        }

        public IProgramBuilder SetConnectionString(string connectionString)
        {
            // Set the connection string
            DBConfiguration.Initialize();

            this.connectionString = connectionString;
            return this;
        }

        public IProgramBuilder GetSystemParameters()
        {
            program.systemParameters = DBAccess.GetSystemParameters();
            return this;
        }

        public IProgramBuilder GetHubSpotProducts()
        {
            program.hubSpotProductList = DBAccess.LoadHubSpotProducts();
            return this;
        }

        public IProgramBuilder UploadFile(string fileName, IConfiguration config, List<SystemParameters> sp)
        {
            program.constructedFileName = POUpload.UploadFile(fileName, config, sp);
            return this;
        }
                

        public async Task<IProgramBuilder> CreateNewDeal(string apiKey, string constructedFileName)
        {
           
            return this;
        }

        //Check if company name exists
        public async Task<IProgramBuilder> CheckCompanyExists(Deal deal, IConfiguration config)
        {
            program.companyResult =  CheckHBCompanyName.SearchCompanyName(config["HubSpot-API:Key"], deal);

            return this;
        }


        public Program Build()
        {
            return program;
        }
    }
}
