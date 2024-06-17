using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Shipment
    {
        public int ShipmentsId { get; set; }
        public int? OdersId { get; set; }
        public string? TrackingNumber { get; set; }
        public int? ShippingMethodsId { get; set; }
        public byte? Status { get; set; }

        public virtual Oder? Oders { get; set; }
        public virtual ShippingMethod? ShippingMethods { get; set; }
    }
}
