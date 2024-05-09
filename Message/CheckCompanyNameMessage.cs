using HubSpotDealCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Message
{   
    public class CheckCompanyNameMessage : Message
    {
        public Deal Deal { get; }
        public bool IsCompanyFound { get; set; }

        public CheckCompanyNameMessage(Deal d, bool cf)
        {
            Deal = d;
            IsCompanyFound = cf;
        }
    }

}
