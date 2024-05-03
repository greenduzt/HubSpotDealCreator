using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Models
{
    public class LineItems
    {
        public string Name { get; set; }

        public string SKU { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public int Discount { get; set; }

        public decimal NetPrice { get; set; }
        public string ExpenseRaw { get; set; }
    }
}
