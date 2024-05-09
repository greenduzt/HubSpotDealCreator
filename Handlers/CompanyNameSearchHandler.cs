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
    public class CompanyNameSearchHandler : ICompanySearchHandler
    {
        private ICompanySearchHandler _nextHandler;

        public ICompanySearchHandler SetNext(ICompanySearchHandler handler)
        {
            _nextHandler = handler;
            return handler;
        }

        public async Task<(Deal, bool)> SearchAsync(Deal deal, IConfiguration config)
        {
            // Search company by name
            if (!string.IsNullOrWhiteSpace(deal.Company.Name))
            {
                var (tempDeal, isCompanyFound) = await CheckHBCompanyName.SearchCompanyName(deal, config);
                deal = tempDeal;
                deal.CompanyFound = isCompanyFound;
                if (isCompanyFound)
                {
                    return (deal, true);
                }
            }

            return _nextHandler != null ? await _nextHandler.SearchAsync(deal, config) : (deal, false);
        }
    }
}
