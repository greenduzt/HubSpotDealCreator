using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Models
{
    public class DealRequest
    {
        public DealProperties properties { get; set; }
        public Association[] associations { get; set; }
    }
}
