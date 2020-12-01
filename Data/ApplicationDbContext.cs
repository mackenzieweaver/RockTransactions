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
    }
}
