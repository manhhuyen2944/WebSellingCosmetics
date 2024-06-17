using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Oders = new HashSet<Oder>();
        }

        public int DiscountId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountPercent { get; set; }
        public int? Quantity { get; set; }
        public int? UseNumber { get; set; }
        public byte? Status { get; set; }

        public virtual ICollection<Oder> Oders { get; set; }
    }
}
