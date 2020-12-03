using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RockTransactions.Data
{
    public class ApplicationDbContext : IdentityDbContext<FPUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<RockTransactions.Models.Attachment> Attachment { get; set; }
        public DbSet<RockTransactions.Models.BankAccount> BankAccount { get; set; }
        public DbSet<RockTransactions.Models.Category> Category { get; set; }
        public DbSet<RockTransactions.Models.CategoryItem> CategoryItem { get; set; }
        public DbSet<RockTransactions.Models.HouseHold> HouseHold { get; set; }
        public DbSet<RockTransactions.Models.Invitation> Invitation { get; set; }
        public DbSet<RockTransactions.Models.Notification> Notification { get; set; }
        public DbSet<RockTransactions.Models.Transaction> Transaction { get; set; }
    }
}
