using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models;

namespace RockTransactions.Controllers
{
    public class HouseHoldsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HouseHoldsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HouseHolds
        public async Task<IActionResult> Index()
        {
            return View(await _context.HouseHold.ToListAsync());
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
                return RedirectToAction(nameof(Index));
            }
            return View(houseHold);
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
