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
    public class CompanyCreationHandler : DealHandlerBase
    { 
        public override async Task<bool> Handle(Deal deal, IConfiguration config)
        { 
            // Call the method to create a new company
            var (tempDeal,  companyCreated) = await CreateNewHBCompany.CreateNewCompany(deal, config);
                 
            // Return the deal and whether a new company was created
            return await PassToNextHandler(tempDeal, config);
        }
    }
}
