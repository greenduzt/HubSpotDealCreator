using HubSpotDealCreator.DB;
using HubSpotDealCreator.JSON;
using HubSpotDealCreator.Models;
using HubSpotDealCreator.Services;
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
        public IProgramBuilder GetSystemParameters()
        {
            program.systemParameters = DBAccess.GetSystemParameters();
            return this;
        }

        public IProgramBuilder BuildHubSpotProducts()
        {
            program.hubSpotProductList = DBAccess.LoadHubSpotProducts();
            return this;
        }

        public IProgramBuilder UploadFile(string filePath, string apiKey)
        {
            program.constructedFileName = POUpload.UploadFile(filePath, apiKey);
            return this;
        }
                

        public async Task<IProgramBuilder> CreateNewDeal(string apiKey, string constructedFileName)
        {
            var (tempDeal, tempTransStringBuilder, dealCreated) = await CreateNewHBDeal.CreateNewDeal(program.transStringBuilder, apiKey, program.deal, constructedFileName);
            program.deal = tempDeal;
            program.transStringBuilder = tempTransStringBuilder;
            return this;
        }

        public IProgramBuilder SetConnectionString(string connectionString)
        {
            this.connectionString = connectionString;
            return this;
        }

        public Program Build()
        {
            return program;
        }
    }
}
