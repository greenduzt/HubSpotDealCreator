using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubSpotDealCreator.Models
{
    public class DealProperties
    {
        public string dealname { get; set; }
        public decimal amount { get; set; }
        public string dealstage { get; set; }
        public string hubspot_owner_id { get; set; }
        public string deal_shipping_address { get; set; }
        public string shipping_city { get; set; }
        public string shipping_state { get; set; }
        public string shipping_post_code { get; set; }
        public string shipping_country { get; set; }
        public string order_notes { get; set; }
        public string is_the_required_date_ { get; set; }
        public string site_contact_name { get; set; }
        public string site_contact_phone { get; set; }
        public string date_required { get; set; }
        public string transport_company { get; set; }
        public string unload_type { get; set; }
        public string freight_type { get; set; }
        public string order_truck { get; set; }
        public string pipeline { get; set; }
        public string dealtype { get; set; }
        public string po_file_access_link { get; set; }
        public string customer_type { get; set; }
    }
}
