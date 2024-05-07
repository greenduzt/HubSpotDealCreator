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

       

        public Program Build()
        {
            return program;
        }
    }
}
