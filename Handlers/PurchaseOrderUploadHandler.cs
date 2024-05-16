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
    public class PurchaseOrderUploadHandler : DealHandlerBase
    {
        private readonly List<SystemParameters> systemParameters;
        public PurchaseOrderUploadHandler(List<SystemParameters> sp)
        {
            systemParameters = sp;
        }

        public override async Task<(Deal,bool)> Handle(Deal deal, IConfiguration config)
        {
            //File purchase order upload

            Deal tempDeal = UploadPurchaseOrder.UploadFile(deal, config, systemParameters);               

            return await PassToNextHandler(tempDeal, config);
        }
    }
}
