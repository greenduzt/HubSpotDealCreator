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
    public class CompanyCreationHandler : ICompanySearchHandler
    {
        private ICompanySearchHandler nextHandler;

        public ICompanySearchHandler SetNext(ICompanySearchHandler handler)
        {
            nextHandler = handler;
            return handler;
        }

        public async Task<(Deal, bool)> SearchAsync(Deal deal, IConfiguration config)
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
            }


            // Pass to the next handler if available
            if (!isNewCompanyCreated && nextHandler != null)
            {
                return await nextHandler.SearchAsync(deal, config);
            }

            // Return the deal and whether a new company was created
            return (deal, isNewCompanyCreated);
        }
    }
}
