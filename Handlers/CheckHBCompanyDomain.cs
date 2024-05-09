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
    public class DomainSearchHandler : ICompanySearchHandler
    {
        private ICompanySearchHandler nextHandler;

        public ICompanySearchHandler SetNext(ICompanySearchHandler handler)
        {
            nextHandler = handler;
            return handler;
        }

        public async Task<(Deal, bool)> SearchAsync(Deal deal, IConfiguration config)
        {
            bool isDomainFound = false;
            // Call the domain search method
            var (tempDeal,  domainFound) = await CheckHBCompanyDomain.SearchDomain(deal, config);
            deal = tempDeal;
            deal.DomainFound = domainFound;
            if (domainFound)
            {
                // If domain found, return true
                isDomainFound = true;
            }
            else
            {
                // If domain not found, pass to the next handler
                if (nextHandler != null)
                {
                    (tempDeal, domainFound) = await nextHandler.SearchAsync(deal, config);
                    deal = tempDeal; // Update the deal with the result from the next handler
                }
            }
            return (deal, isDomainFound);
        }
    }

}
