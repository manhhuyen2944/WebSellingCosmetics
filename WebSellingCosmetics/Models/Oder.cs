using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Oder
    {
        public Oder()
        {
            OderItems = new HashSet<OderItem>();
            Payments = new HashSet<Payment>();
            Shipments = new HashSet<Shipment>();
        }

        public int OdersId { get; set; }
        public int? AccountId { get; set; }
        public decimal? Total { get; set; }
        public DateTime? CreateDay { get; set; }
        public int? AddressId { get; set; }
        public int? DiscountId { get; set; }
        public byte? Status { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Discount? Discount { get; set; }
        public virtual ICollection<OderItem> OderItems { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; }
    }
}
