using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Models
{
    public class Association
    {
        public To to { get; set; }
        public Type[] types { get; set; }
    }
}
