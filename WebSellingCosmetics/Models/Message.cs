using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }
        public string? Content { get; set; }
        public bool? IsFromUserId { get; set; }
        public bool? IsToUserId { get; set; }
        public byte? Status { get; set; }

        public virtual Account? FromUser { get; set; }
    }
}
