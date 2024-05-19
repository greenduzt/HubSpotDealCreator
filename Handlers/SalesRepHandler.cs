using CoreLibrary;
using CoreLibrary.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;

namespace HubSpotDealCreator.Handlers
{
    public class SalesRepHandler : DealHandlerBase
    {
        public override async Task<(Deal,bool)> Handle(Deal deal, IConfiguration config)
        {
            var (dealUpdated,isSalesRepFound) = await SalesRepAllocator.AllocateSalesRepToDeal(deal, config);
         
            return await PassToNextHandler(dealUpdated, config);
        }
    }
}
