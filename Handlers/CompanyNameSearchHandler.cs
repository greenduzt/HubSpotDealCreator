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
    public class CompanyNameSearchHandler : DealHandlerBase
    {  
        public override async Task<(Deal,bool)> Handle(Deal deal, IConfiguration config)
        {
            // Implementation for company name search
            // If search is successful, return true
            // Otherwise, pass to the next handler
            var (dealUpdated,isCompanyNameFound) = await CheckHBCompanyName.SearchCompanyName(deal, config);           
            if (isCompanyNameFound)
            {
                return (dealUpdated,isCompanyNameFound);
            }
            // Pass to the next handler
            return await PassToNextHandler(deal, config);
        }
    }
}
