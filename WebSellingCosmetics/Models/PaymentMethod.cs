using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Payments = new HashSet<Payment>();
        }

        public int PaymentMethodsId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }
}
