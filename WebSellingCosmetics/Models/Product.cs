using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Product
    {
        public Product()
        {
            OderItems = new HashSet<OderItem>();
            ReceiptProducts = new HashSet<ReceiptProduct>();
        }

        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Image3 { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? ProductInventoryId { get; set; }
        public int? ProductTypeId { get; set; }
        public byte? Status { get; set; }

        public virtual ProductsInventory? ProductInventory { get; set; }
        public virtual ProductType? ProductType { get; set; }
        public virtual ICollection<OderItem> OderItems { get; set; }
        public virtual ICollection<ReceiptProduct> ReceiptProducts { get; set; }
    }
}
