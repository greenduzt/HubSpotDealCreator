using CoreLibrary;
using CoreLibrary.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;

namespace HubSpotDealCreator.Handlers
{
    public class AbnSearchHandler : DealHandlerBase
    {
        public override async Task<(Deal,bool)> Handle(Deal deal, IConfiguration config)
        {
            // Implementation for ABN search
            // If search is successful, return true
            // Otherwise, pass to the next handler
            var (dealUpdated,isABNFound) = await CheckHBABN.SearchABN(deal, config);
            if (isABNFound)
            {
                // Deal creation logic can be invoked here if needed
                return (dealUpdated, isABNFound);
            }
            return await PassToNextHandler(deal, config);
        }
    }
}
