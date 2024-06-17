using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class ProductsInventory
    {
        public ProductsInventory()
        {
            Products = new HashSet<Product>();
        }

        public int ProductInventoryId { get; set; }
        public int? Quantity { get; set; }
        public int? QuantitySold { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
