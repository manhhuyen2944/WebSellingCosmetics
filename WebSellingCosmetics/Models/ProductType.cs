using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class ProductType
    {
        public ProductType()
        {
            Products = new HashSet<Product>();
        }

        public int ProductTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte? Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
