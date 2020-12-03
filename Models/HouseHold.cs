using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Models
{
    public class HouseHold
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Greeting { get; set; }

        public DateTime Established { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new HashSet<Attachment>();
        public ICollection<Invitation> Invitations { get; set; } = new HashSet<Invitation>();
        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        public ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public ICollection<BankAccount> BankAccounts { get; set; } = new HashSet<BankAccount>();
        public ICollection<FPUser> FPUsers { get; set; } = new HashSet<FPUser>();
    }
}
