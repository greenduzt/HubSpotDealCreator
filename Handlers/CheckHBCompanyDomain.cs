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
    public class DomainSearchHandler : AbstractCompanyHandler
    {
        public override async Task<(Deal, bool)> HandleAsync(Deal deal, IConfiguration config)
        {
            // If Domain is null, pass to the next handler
            if (string.IsNullOrWhiteSpace(deal.Company.Domain) && _nextHandler != null)
            {
                return await _nextHandler.HandleAsync(deal, config);
            }

            // Call the domain search method
            var (tempDeal, domainFound) = await CheckHBCompanyDomain.SearchDomain(deal, config);
            deal = tempDeal;
            deal.DomainFound = domainFound;

            return _nextHandler != null ? await _nextHandler.HandleAsync(deal, config) : (deal, false);
        }
    }

}
