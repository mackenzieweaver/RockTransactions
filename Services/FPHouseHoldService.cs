using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Data.Enums;
using RockTransactions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Services
{
    public class FPHouseHoldService : IFPHouseHoldService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FPUser> _userManager;

        public FPHouseHoldService(ApplicationDbContext context, UserManager<FPUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<FPUser>> ListHouseHoldMembersAsync(int? houseHoldId)
        {
            var houseHold = await _context.HouseHold.Include(hh => hh.FPUsers).FirstOrDefaultAsync(hh => hh.Id == houseHoldId);
            var members = new List<FPUser>();
            foreach (var member in houseHold.FPUsers)
            {
                var role = (await _userManager.GetRolesAsync(member))[0];
                if (role == Roles.Member.ToString())
                {
                    members.Add(member);
                }
            }
            return members;
        }

        public List<Transaction> ListTransactions(HouseHold houseHold)
        {
            var transactions = new List<ICollection<Transaction>>();
            foreach(var bankAccount in houseHold.BankAccounts)
            {
                transactions.Add(bankAccount.Transactions);
            }
            return transactions.SelectMany(t => t).ToList();
        }

        public async Task<string> GetRoleAsync(FPUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles[0];
        }

        public async Task<string> GetGreetingAsync(FPUser user)
        {
            var houseHold = await _context.HouseHold.FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);
            if(houseHold == null)
            {
                return "Hello";
            }
            return houseHold.Greeting;
        }
    }
}
