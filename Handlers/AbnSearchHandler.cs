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
    public class AbnSearchHandler : DealHandlerBase
    {
        public override async Task<bool> Handle(Deal deal, IConfiguration config)
        {
            // Implementation for ABN search
            // If search is successful, return true
            // Otherwise, pass to the next handler
            bool isABNFound = await CheckHBABN.SearchABN(deal, config);
            if (isABNFound)
            {
                // Deal creation logic can be invoked here if needed
                return true;
            }
            return await PassToNextHandler(deal, config);
        }
    }
}
