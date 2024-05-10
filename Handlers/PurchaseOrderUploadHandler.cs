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
    public class PurchaseOrderUploadHandler : AbstractCompanyHandler
    {
        private readonly List<SystemParameters> systemParameters;
        public PurchaseOrderUploadHandler(List<SystemParameters> sp)
        {
            systemParameters = sp;
        }

        public override async Task<(Deal,bool)> HandleAsync(Deal deal, IConfiguration config)
        {
            //File purchase order upload

            var (tempDeal,fileName, isFileUploaded) = UploadPurchaseOrder.UploadFile(deal, config, systemParameters);
            deal = tempDeal;
            deal.FileUploaded = isFileUploaded;

            return _nextHandler != null ? await _nextHandler.HandleAsync(deal, config) : (deal, false);
        }
    }
}
