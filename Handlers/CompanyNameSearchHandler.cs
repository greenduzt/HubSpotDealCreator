using CoreLibrary.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;

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
