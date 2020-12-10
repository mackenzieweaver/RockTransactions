using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace RockTransactions.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FPUser> _userManager;

        public CategoriesController(ApplicationDbContext context, UserManager<FPUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Categories
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Category
                .Where(x => x.HouseHoldId == user.HouseHoldId)
                .Include(c => c.HouseHold);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Categories/Details/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Category
                .Include(c => c.HouseHold)
                .Where(x => x.HouseHoldId == user.HouseHoldId)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [Authorize(Roles = "Admin,Head,Member")]
        public IActionResult Create()
        {
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Create([Bind("Id,HouseHoldId,Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Script"] = $"NewColor('{category.Name}')";
                return RedirectToAction("Dashboard", "HouseHolds");
            }
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name", category.HouseHoldId);
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Category
                .Where(x => x.HouseHoldId == user.HouseHoldId)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name", category.HouseHoldId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseHoldId,Name,Description")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name", category.HouseHoldId);
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var category = await _context.Category
                .Include(c => c.HouseHold)
                .Where(x => x.HouseHoldId == user.HouseHoldId)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Category.FindAsync(id);
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
