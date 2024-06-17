using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class Account
    {
        public Account()
        {
            Addresses = new HashSet<Address>();
            Messages = new HashSet<Message>();
            Oders = new HashSet<Oder>();
        }

        public int AccountId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Avartar { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public int? Point { get; set; }
        public DateTime? Birthday { get; set; }
        public byte? Gender { get; set; }
        public int? RoleId { get; set; }
        public int? AccountTypeId { get; set; }
        public byte? Status { get; set; }

        public virtual AccountType? AccountType { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Oder> Oders { get; set; }
    }
}
