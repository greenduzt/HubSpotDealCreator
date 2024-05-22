using CoreLibrary.Models;
using HubSpotDealCreator.Utilities;
using Microsoft.Extensions.Configuration;

namespace HubSpotDealCreator.Handlers
{
    public class PurchaseOrderUploadHandler : DealHandlerBase
    {
        public override async Task<(Deal,bool)> Handle(Deal deal, IConfiguration config)
        {
            //File purchase order upload

            Deal tempDeal = UploadPurchaseOrder.UploadFile(deal, config);               

            return await PassToNextHandler(tempDeal, config);
        }
    }
}
