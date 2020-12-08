using RockTransactions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Services
{
    public interface IFPHouseHoldService
    {
        public Task<List<FPUser>> ListHouseHoldMembersAsync(int? houseHoldId);
        public Task<string> GetRoleAsync (FPUser user);
        public List<Transaction> ListTransactions(HouseHold houseHold);
    }
}
