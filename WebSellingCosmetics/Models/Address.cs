﻿using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Address
    {
        public int AddressId { get; set; }
        public string? Street { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? AccountId { get; set; }

        public virtual Account? Account { get; set; }
    }
}
