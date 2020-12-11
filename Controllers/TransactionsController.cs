using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models;
using RockTransactions.Data.Enums;
using RockTransactions.Services;

namespace RockTransactions.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FPUser> _userManager;
        private readonly IFPNotificationService _notificationService;

        public TransactionsController(ApplicationDbContext context, UserManager<FPUser> userManager, IFPNotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET: Transactions
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transaction.Include(t => t.BankAccount).Include(t => t.CategoryItem).Include(t => t.FPUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transactions
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Transactions(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var bankAccount = await _context.BankAccount
                .Include(ba => ba.Transactions).ThenInclude(t => t.CategoryItem)
                .Include(ba => ba.Transactions).ThenInclude(t => t.FPUser)
                .FirstOrDefaultAsync(x => x.HouseHoldId == user.HouseHoldId && x.Id == id);
            if(bankAccount == null)
            {
                return NotFound();
            }

            return View("Index", bankAccount.Transactions);
        }

        // GET: Transactions/Details/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.BankAccount)
                .Include(t => t.CategoryItem)
                .Include(t => t.FPUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        [Authorize(Roles = "Admin,Head,Member")]
        public IActionResult Create()
        {
            ViewData["BankAccountId"] = new SelectList(_context.BankAccount, "Id", "Name");
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Name");
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Create([Bind("Id,CategoryItemId,BankAccountId,FPUserId,Created,Type,Memo,Amount,IsDeleted")] Transaction transaction)
        {
            transaction.FPUserId = _userManager.GetUserId(User);
            var bankAccount = await _context.BankAccount.Include(ba => ba.Transactions).ThenInclude(t => t.CategoryItem).FirstOrDefaultAsync(ba => ba.Id == transaction.BankAccountId);
            var categoryItem = await _context.CategoryItem.FirstOrDefaultAsync(ci => ci.Id == transaction.CategoryItemId);
            if (ModelState.IsValid)
            {
                if(transaction.Type == TransactionType.Deposit)
                {
                    bankAccount.CurrentBalance += transaction.Amount;
                }
                else if(transaction.Type == TransactionType.Withdrawal)
                {
                    bankAccount.CurrentBalance -= transaction.Amount;
                    categoryItem.ActualAmount += transaction.Amount;
                    _context.Update(categoryItem);
                }

                // so that only one history per day
                var history = await _context.History.FirstOrDefaultAsync(h => h.BankAccount == bankAccount && h.Date.Day == transaction.Created.Day);
                if(history == null)
                {
                    History _history = new History
                    {
                        BankAccountId = transaction.BankAccountId,
                        Balance = (decimal)bankAccount.CurrentBalance,
                        Date = transaction.Created
                    };
                    _context.Add(_history);
                }
                else
                {
                    history.BankAccountId = transaction.BankAccountId;
                    history.Balance = (decimal)bankAccount.CurrentBalance;
                    history.Date = transaction.Created;
                    _context.Update(history);
                }

                _context.Add(transaction);
                _context.Update(bankAccount);
                await _context.SaveChangesAsync();

                if(bankAccount.CurrentBalance < 0)
                {
                    await _notificationService.NotifyOverdraft(transaction.FPUserId, bankAccount);
                    TempData["Script"] = "Overdraft()";
                }

                return RedirectToAction("Dashboard", "HouseHolds");
            }

            var user = await _userManager.GetUserAsync(User);
            var houseHold = await _context.HouseHold
                .Include(hh => hh.BankAccounts).ThenInclude(ba => ba.Transactions)
                .Include(hh => hh.Categories)
                .ThenInclude(c => c.CategoryItems)
                .FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);
            var banks = houseHold.BankAccounts;
            var items = houseHold.Categories.SelectMany(c => c.CategoryItems).ToList();

            ViewData["BankAccountId"] = new SelectList(banks, "Id", "Name", transaction.BankAccountId);
            ViewData["CategoryItemId"] = new SelectList(items, "Id", "Name", transaction.CategoryItemId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["BankAccountId"] = new SelectList(_context.BankAccount, "Id", "Id", transaction.BankAccountId);
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Id", transaction.CategoryItemId);
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "Id", transaction.FPUserId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryItemId,BankAccountId,FPUserId,Created,Type,Memo,Amount,IsDeleted")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    transaction.FPUserId = _userManager.GetUserId(User);
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Dashboard", "HouseHolds");
            }
            ViewData["BankAccountId"] = new SelectList(_context.BankAccount, "Id", "Id", transaction.BankAccountId);
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Id", transaction.CategoryItemId);
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "Id", transaction.FPUserId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.BankAccount)
                .Include(t => t.CategoryItem)
                .Include(t => t.FPUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "HouseHolds");
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
