using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models;

namespace RockTransactions.Controllers
{
    public class InvitationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailService;

        public InvitationsController(ApplicationDbContext context, IEmailSender emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Invitations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invitation.Include(i => i.HouseHold);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invitations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitation
                .Include(i => i.HouseHold)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        [Authorize(Roles = "Admin,Head")]
        // GET: Invitations/Create
        public IActionResult Create()
        {
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin,Head")]
        // POST: Invitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HouseHoldId,Created,Expires,Accepted,EmailTo,Subject,Body,Code")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                await _emailService.SendEmailAsync(invitation.EmailTo, invitation.Subject, invitation.Body);
                _context.Add(invitation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard", "HouseHolds");
            }
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name", invitation.HouseHoldId);
            return View(invitation);
        }

        // GET: Invitations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitation.FindAsync(id);
            if (invitation == null)
            {
                return NotFound();
            }
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name", invitation.HouseHoldId);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseHoldId,Created,Expires,Accepted,EmailTo,Subject,Body,Code")] Invitation invitation)
        {
            if (id != invitation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invitation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvitationExists(invitation.Id))
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
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name", invitation.HouseHoldId);
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitation
                .Include(i => i.HouseHold)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invitation = await _context.Invitation.FindAsync(id);
            _context.Invitation.Remove(invitation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvitationExists(int id)
        {
            return _context.Invitation.Any(e => e.Id == id);
        }
    }
}
