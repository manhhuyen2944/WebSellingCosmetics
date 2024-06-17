using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class OderItem
    {
        public int OderId { get; set; }
        public int ProductId { get; set; }
        public int? Quantity { get; set; }

        public virtual Oder Oder { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
