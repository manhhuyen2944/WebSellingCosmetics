using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class ReceiptProduct
    {
        public int ReceiptProductId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }
        public DateTime? CreateDay { get; set; }
        public byte? Status { get; set; }

        public virtual Product? Product { get; set; }
    }
}
