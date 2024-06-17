using System;
using System.Collections.Generic;

namespace WebSellingCosmetics.Models
{
    public partial class AccountType
    {
        public AccountType()
        {
            Accounts = new HashSet<Account>();
        }

        public int AccountTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
