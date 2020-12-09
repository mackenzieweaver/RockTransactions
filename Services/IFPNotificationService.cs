using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RockTransactions.Models;

namespace RockTransactions.Services
{
    public interface IFPNotificationService
    {
        public Task NotifyOverdraft(string userId, BankAccount bankAccount);
    }
}
