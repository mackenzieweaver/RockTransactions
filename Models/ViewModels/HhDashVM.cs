using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Models.ViewModels
{
    public class HhDashVM
    {
        public IEnumerable<FPUser> Occupants { get; set; }
        public IEnumerable<BankAccount> Accounts { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}
