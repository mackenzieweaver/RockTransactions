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
            var bankAccount = await _context.BankAccount
                .Include(ba => ba.Transactions).ThenInclude(t => t.CategoryItem)
                .Include(ba => ba.Transactions).ThenInclude(t => t.FPUser)
                .FirstOrDefaultAsync(ba => ba.Id == id);
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
            ViewData["BankAccountId"] = new SelectList(_context.BankAccount, "Id", "Id", transaction.BankAccountId);
            ViewData["CategoryItemId"] = new SelectList(_context.CategoryItem, "Id", "Id", transaction.CategoryItemId);
            ViewData["FPUserId"] = new SelectList(_context.Users, "Id", "Id", transaction.FPUserId);
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
