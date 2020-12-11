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
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryItem> CategoryItem { get; set; }
        public DbSet<HouseHold> HouseHold { get; set; }
        public DbSet<Invitation> Invitation { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<History> History { get; set; }
    }
}
