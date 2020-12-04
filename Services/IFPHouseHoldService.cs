﻿using RockTransactions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Services
{
    public interface IFPHouseHoldService
    {
        public Task<List<FPUser>> ListHouseHoldMembersAsync(FPUser user);
        public Task<string> GetRoleAsync (FPUser user);
    }
}