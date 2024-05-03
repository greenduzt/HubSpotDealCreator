using HubSpotDealCreator.DB;
using HubSpotDealCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Services
{
    public class HubSpotProductService
    {
        public List<HubSpotProduct> GetHubSpotProducts()
        {
            return DBAccess.LoadHubSpotProducts();
        }
    }
}
