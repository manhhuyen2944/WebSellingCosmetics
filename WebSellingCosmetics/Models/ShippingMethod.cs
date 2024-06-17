using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class ShippingMethod
    {
        public ShippingMethod()
        {
            Shipments = new HashSet<Shipment>();
        }

        public int ShippingMethodsId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? ShippingFee { get; set; }

        public virtual ICollection<Shipment> Shipments { get; set; }
    }
}
