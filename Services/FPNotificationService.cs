﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Services
{
    public class FPNotificationService : IFPNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailService;

        public FPNotificationService(ApplicationDbContext context, IEmailSender emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task NotifyOverdraft(string userId, BankAccount bankAccount)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var transaction = bankAccount.Transactions.Last();
            string subject = "Overdraft Alert";
            string body= $"Your <b>{bankAccount.Name}</b> account has been overdrafted. Your current balance is <b>{bankAccount.CurrentBalance}</b>. The cause was paying <b>{transaction.Amount}</b> for <b>{transaction.CategoryItem.Name}</b> on <b>{transaction.Created}</b>.";
            await _emailService.SendEmailAsync(user.Email, subject, body);
        }
    }
}
