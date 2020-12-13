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
using RockTransactions.Models.ViewModels;
using RockTransactions.Services;
using Microsoft.AspNetCore.Authorization;

namespace RockTransactions.Controllers
{
    [Authorize]
    public class HouseHoldsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FPUser> _userManager;
        private readonly SignInManager<FPUser> _signInManager;
        private readonly IFPHouseHoldService _houseHoldService;

        public HouseHoldsController(ApplicationDbContext context, UserManager<FPUser> userManager, SignInManager<FPUser> signInManager, IFPHouseHoldService houseHoldService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _houseHoldService = houseHoldService;
        }

        // GET: HouseHolds
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.HouseHold.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            user.HouseHoldId = id;
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());
            await _context.SaveChangesAsync();

            // sign out / sign in
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave()
        {
            var user = await _userManager.GetUserAsync(User);

            if (User.IsInRole(Roles.Head.ToString()))
            {
                var members = await _houseHoldService.ListHouseHoldMembersAsync(user.HouseHoldId);
                if(members.Count > 0)
                {
                    TempData["Script"] = "CantLeave()";
                    return RedirectToAction("Dashboard");
                }
                // delete household
                var houseHold = await _context.HouseHold.FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);
                _context.HouseHold.Remove(houseHold);
            }

            // not in a house
            user.HouseHoldId = null;

            // reset role to New
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, Roles.New.ToString());
            await _context.SaveChangesAsync();

            // sign out / sign in
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Home");
        }

        // GET: HouseHolds/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseHold = await _context.HouseHold
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseHold == null)
            {
                return NotFound();
            }

            return View(houseHold);
        }

        // GET: HouseHolds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HouseHolds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Greeting,Established")] HouseHold houseHold)
        {
            if (ModelState.IsValid)
            {
                _context.Add(houseHold);
                await _context.SaveChangesAsync();

                var user = await _userManager.GetUserAsync(User);
                user.HouseHoldId = houseHold.Id;

                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, Roles.Head.ToString());                
                await _context.SaveChangesAsync();

                // sign out / sign in
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);

                TempData["Script"] = "Wizard()";
                return RedirectToAction(nameof(Dashboard));
            }
            return View(houseHold);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetUp(string bank, AccountType accountType, decimal startBalance,
            string categoryName, string catDesc, string itemName, string itemDesc,  decimal target)
        {
            var user = await _userManager.GetUserAsync(User);
            var bankAccount = new BankAccount
            {
                HouseHoldId = (int)user.HouseHoldId,
                FPUserId = user.Id,
                Name = bank,
                Type = accountType,
                StartingBalance = startBalance,
                CurrentBalance = startBalance
            };
            await _context.AddAsync(bankAccount);
            await _context.SaveChangesAsync();

            var category = new Category
            {
                HouseHoldId = (int)user.HouseHoldId,
                Name = categoryName,
                Description = catDesc
            };
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();

            var item = new CategoryItem
            {
                CategoryId = category.Id,
                Name = itemName,
                Description = itemDesc,
                TargetAmount = target,
                ActualAmount = 0
            };
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Dashboard(string year, string month)
        {
            var user = await _userManager.GetUserAsync(User);
            var houseHold = await _context.HouseHold
                .Include(hh => hh.BankAccounts).ThenInclude(ba => ba.Transactions).ThenInclude(t => t.BankAccount)
                .Include(hh => hh.BankAccounts).ThenInclude(ba => ba.Transactions).ThenInclude(t => t.CategoryItem)
                .Include(hh => hh.BankAccounts).ThenInclude(ba => ba.Transactions).ThenInclude(t => t.FPUser)
                .FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);

            var transactions = _houseHoldService.ListTransactions(houseHold);
            var allTransactions = transactions;

            // filter transactions per year
            if (year != null)
            {   // DateTime.Parse needs a valid datetime format
                var _year = DateTime.Parse($"Jan 1, {year}").Year;
                transactions = transactions.Where(t => t.Created.Year == _year).ToList();
            }
            // filter transactions per month
            if (month != null)
            {   // DateTime.Parse needs a valid datetime format
                var _month = DateTime.Parse($"{month} 1, 2009").Month;
                transactions = transactions.Where(t => t.Created.Month == _month).ToList();
            }

            var model = new HhDashVM
            {
                Occupants = await _context.Users.Where(u => u.HouseHoldId == user.HouseHoldId).ToListAsync(),
                Accounts = houseHold.BankAccounts,
                Transactions = transactions
            };

            var categories = _context.Category
                .Include(c => c.CategoryItems)
                .Where(c => c.HouseHoldId == houseHold.Id).ToList();
            var items = new List<CategoryItem>();
            foreach(var category in categories)
            {
                foreach(var item in category.CategoryItems)
                {
                    items.Add(item);
                }
            }
            var bankAccounts = _context.BankAccount.Where(ba => ba.HouseHoldId == houseHold.Id).ToList();

            // all years between oldest and newest transaction
            //int oldestYear = int.Parse(_context.Transaction.OrderBy(t => t.Created).First().Created.Year.ToString());
            //int currentYear = int.Parse(DateTime.Now.Year.ToString());
            //var years = new List<string>();
            //while(currentYear >= oldestYear)
            //{
            //    years.Add(currentYear.ToString());
            //    currentYear -= 1;
            //}

            
            var years = new List<string>();
            foreach(var transaction in allTransactions)
            {
                if (!years.Contains(transaction.Created.Year.ToString()))
                {
                    years.Add(transaction.Created.Year.ToString());
                }
            }

            var months = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            ViewData["Years"] = new SelectList(years, year ?? DateTime.Now.Year.ToString());
            ViewData["Months"] = new SelectList(months, month ?? DateTime.Now.Month.ToString());

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            ViewData["BankAccountId"] = new SelectList(bankAccounts, "Id", "Name");
            ViewData["CategoryItemId"] = new SelectList(items, "Id", "Name");

            return View(model);
        }

        // GET: HouseHolds/Edit/5
        [Authorize(Roles = "Admin,Head")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseHold = await _context.HouseHold.FindAsync(id);
            if (houseHold == null)
            {
                return NotFound();
            }
            return View(houseHold);
        }

        // POST: HouseHolds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Head")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Greeting,Established")] HouseHold houseHold)
        {
            if (id != houseHold.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(houseHold);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseHoldExists(houseHold.Id))
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
            return View(houseHold);
        }

        // GET: HouseHolds/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseHold = await _context.HouseHold
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseHold == null)
            {
                return NotFound();
            }

            return View(houseHold);
        }

        // POST: HouseHolds/Delete/5
        [Authorize(Roles = "Admin,Head")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            user.HouseHoldId = null;
            var houseHold = await _context.HouseHold.FindAsync(id);
            _context.HouseHold.Remove(houseHold);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseHoldExists(int id)
        {
            return _context.HouseHold.Any(e => e.Id == id);
        }
    }
}
