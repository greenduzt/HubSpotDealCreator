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
            bool isNewCompanyCreated = false;

            if (!deal.CompanyFound && !deal.DomainFound && !deal.AbnFound)
            {
                // No matching company found, create a new company
                Console.WriteLine("No matching company found. Creating a new company...");

                // Call the method to create a new company
                var (tempDeal,  companyCreated) = await CreateNewHBCompany.CreateNewCompany(deal, config);
                deal = tempDeal;
                isNewCompanyCreated = companyCreated;
                deal.newCompanyCreated = companyCreated;

                // If a new company was created, set isFound to true
                if (isNewCompanyCreated)
                {
                    return (deal, true);
                }
            }

            // Pass to the next handler if available
            if (!isNewCompanyCreated && _nextHandler != null)
            {
                return await _nextHandler.HandleAsync(deal, config);
            }

            // Return the deal and whether a new company was created
            return (deal, isNewCompanyCreated);
        }
    }
}
