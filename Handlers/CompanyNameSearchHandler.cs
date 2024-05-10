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
    public class CompanyNameSearchHandler : AbstractCompanyHandler
    {  
        public override async Task<(Deal, bool)> HandleAsync(Deal deal, IConfiguration config)
        {
            // If company name is null, pass to the next handler
            if (string.IsNullOrWhiteSpace(deal.Company.Name) && _nextHandler != null)
            {
                return await _nextHandler.HandleAsync(deal, config);
            }

            var (tempDeal, isCompanyFound) = await CheckHBCompanyName.SearchCompanyName(deal, config);
            deal = tempDeal;
            deal.CompanyFound = isCompanyFound;

            // If company is found, return without passing to the next handler
            if (isCompanyFound)
            {
                return (deal, true);
            }
            // Pass to the next handler
            return _nextHandler != null ? await _nextHandler.HandleAsync(deal, config) : (deal, false);
        }
    }
}
