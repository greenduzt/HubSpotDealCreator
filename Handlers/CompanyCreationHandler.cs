using HubSpotDealCreator.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Handlers
{
    public class CompanyCreationHandler : AbstractCompanyHandler
    { 
        public override async Task<(Deal, bool)> HandleAsync(Deal deal, IConfiguration config)
        {            
            // No matching company found, create a new company
            Console.WriteLine("No matching company found. Creating a new company...");

            // Call the method to create a new company
            var (tempDeal,  companyCreated) = await CreateNewHBCompany.CreateNewCompany(deal, config);
            deal = tempDeal;
            deal.newCompanyCreated = companyCreated;                           

            // Return the deal and whether a new company was created
            return (deal, companyCreated);
        }
    }
}
