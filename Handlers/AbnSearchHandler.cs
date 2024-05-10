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
    public class AbnSearchHandler : AbstractCompanyHandler
    {
        public override async Task<(Deal, bool)> HandleAsync(Deal deal, IConfiguration config)
        {
            // If ABN is null, pass to the next handler
            if (string.IsNullOrWhiteSpace(deal.Company.ABN) && _nextHandler != null)
            {
                return await _nextHandler.HandleAsync(deal, config);
            }

            // Call the ABN search method
            var (tempDeal, abnFound) = await CheckHBABN.SearchABN(deal, config);
            deal = tempDeal;
            deal.AbnFound = abnFound;

            // If ABN is found, return without passing to the next handler
            if (abnFound)
            {
                return (deal, true);
            }
            // Pass to the next handler
            return _nextHandler != null ? await _nextHandler.HandleAsync(deal, config) : (deal, false);
        }
    }
}
