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
    public class DealCreationHandler : AbstractCompanyHandler
    {
        public override async Task<(Deal,bool)> HandleAsync(Deal deal,IConfiguration config)
        {
            //Check if any of the previous handlers succeeded
            bool anyPreviousHandlersSucceeded = deal.CompanyFound || deal.DomainFound || deal.AbnFound || deal.newCompanyCreated;

            if (anyPreviousHandlersSucceeded)
            {
                await CreateLineItemsAndDeal.CreateNewDeal(deal, config);
                return (deal, true);
            }

            // If none of the previous handlers succeeded, return false
            return (deal, false);
        }
    }
}
