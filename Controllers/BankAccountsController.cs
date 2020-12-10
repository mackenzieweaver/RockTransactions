using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Data.Enums;
using RockTransactions.Models;
using Microsoft.AspNetCore.Authorization;

namespace RockTransactions.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FPUser> _userManager;

        public BankAccountsController(ApplicationDbContext context, UserManager<FPUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Head,Member")]
        // GET: BankAccounts
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.BankAccount.Where(x => x.HouseHoldId == user.HouseHoldId).Include(b => b.HouseHold).Include(u => u.FPUser);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin,Head,Member")]
        // GET: BankAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var bankAccount = await _context.BankAccount
                .Include(b => b.HouseHold)
                .FirstOrDefaultAsync(x => x.HouseHoldId == user.HouseHoldId && x.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        [Authorize(Roles = "Admin,Head,Member")]
        // GET: BankAccounts/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["HouseHoldId"] = user.HouseHoldId;
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Head,Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HouseHoldId,FPUserId,Name,Type,StartingBalance,CurrentBalance")] BankAccount bankAccount)
        {
            bankAccount.FPUserId = _userManager.GetUserId(User);
            bankAccount.CurrentBalance = bankAccount.StartingBalance;
            if (ModelState.IsValid)
            {
                _context.Add(bankAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "HouseHolds");
            }
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name", bankAccount.HouseHoldId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var bankAccount = await _context.BankAccount
                .FirstOrDefaultAsync(x => x.HouseHoldId == user.HouseHoldId && x.Id == id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name", bankAccount.HouseHoldId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Head,Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseHoldId,FPUserId,Name,Type,StartingBalance,CurrentBalance")] BankAccount bankAccount)
        {
            if (id != bankAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bankAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankAccountExists(bankAccount.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name", bankAccount.HouseHoldId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var bankAccount = await _context.BankAccount
                .FirstOrDefaultAsync(x => x.HouseHoldId == user.HouseHoldId && x.Id == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [Authorize(Roles = "Admin,Head,Member")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bankAccount = await _context.BankAccount.FindAsync(id);
            _context.BankAccount.Remove(bankAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard", "HouseHolds");
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccount.Any(e => e.Id == id);
        }
    }
}
