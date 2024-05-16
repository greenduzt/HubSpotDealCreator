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
    public class DomainSearchHandler : DealHandlerBase
    {
        public override async Task<(Deal,bool)> Handle(Deal deal, IConfiguration config)
        {
            // Implementation for domain search
            // If search is successful, return true
            // Otherwise, pass to the next handler
            var (dealUpdated,isDomainFound) = await CheckHBCompanyDomain.SearchDomain(deal, config);
            if (isDomainFound)
            {
                return (dealUpdated,isDomainFound);
            }
            // Pass to the next handler
            return await PassToNextHandler(deal, config);
        }
    }

}
