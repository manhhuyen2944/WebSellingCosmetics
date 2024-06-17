using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Payment
    {
        public int PaymentsId { get; set; }
        public int? OdersId { get; set; }
        public int? PaymentMethodsId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public byte? Status { get; set; }

        public virtual Oder? Oders { get; set; }
        public virtual PaymentMethod? PaymentMethods { get; set; }
    }
}
