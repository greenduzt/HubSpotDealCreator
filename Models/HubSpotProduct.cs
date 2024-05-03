using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Models
{
    public class HubSpotProduct
    {
        public string RecordID { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string ProductDescription { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public string ProductCategory { get; set; }
    }
}
