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
                var houseHold = await _context.HouseHold.FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);
                _context.HouseHold.Remove(houseHold);
            }
            user.HouseHoldId = null;
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, Roles.New.ToString());
            await _context.SaveChangesAsync();

            // sign out / sign in
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Home");
        }

        // GET: HouseHolds/Details/5
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
                return RedirectToAction(nameof(Dashboard));
            }
            return View(houseHold);
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new HhDashVM
            {
                Occupants = await _context.Users.Where(u => u.HouseHoldId == user.HouseHoldId).ToListAsync()
            };
            return View(model);
        }

        // GET: HouseHolds/Edit/5
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
