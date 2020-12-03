using RockTransactions.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public int HouseHoldId { get; set; }
        public HouseHold HouseHold { get; set; }

        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public int FPUserId { get; set; }
        public FPUser FPUser { get; set; }

        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        public AccountType Type { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(6,2)")]
        public decimal StartingBalance { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(6,2)")]
        public decimal CurrentBalance { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
