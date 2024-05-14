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
    public class DealCreationHandler : DealHandlerBase
    {
        public override async Task<bool> Handle(Deal deal,IConfiguration config)
        {
           var ( d,isDealCreated)= await CreateLineItemsAndDeal.CreateNewDeal(deal, config);
           return isDealCreated;    
        }
    }
}
