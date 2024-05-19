using CoreLibrary.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;

namespace HubSpotDealCreator.Handlers
{
    public class DealCreationHandler : DealHandlerBase
    {
        public override async Task<(Deal,bool)> Handle(Deal deal,IConfiguration config)
        {
           var ( dealUpdated,isDealCreated)= await CreateLineItemsAndDeal.CreateNewDeal(deal, config);
           return (dealUpdated,isDealCreated);    
        }
    }
}
