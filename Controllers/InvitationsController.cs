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

        // POST: Invitations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Head")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HouseHoldId,Created,Expires,Accepted,EmailTo,Subject,Body,Code")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                // prevent inviting user already in household
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == invitation.EmailTo);
                if(user != null && user.HouseHoldId != null)
                {
                    TempData["Script"] = "CantInvite()";
                    return RedirectToAction("Dashboard", "HouseHolds");
                }

                // create invitation record
                _context.Add(invitation);
                await _context.SaveChangesAsync();

                // construct email
                var callbackUrl = Url.Action("Accept", "Invitations", new { email = invitation.EmailTo, code = invitation.Code }, protocol: Request.Scheme);
                string houseHoldName = (await _context.HouseHold.FirstOrDefaultAsync(hh => hh.Id == invitation.HouseHoldId)).Name;
                var emailBody = $"{invitation.Body} <br/><p><h3>Your invited to join the {houseHoldName} household.</h3><br/><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Click here to accept</a>.";
                
                // send email
                await _emailService.SendEmailAsync(invitation.EmailTo, invitation.Subject, emailBody);

                // sweet alert
                TempData["Script"] = "CanInvite()";
                return RedirectToAction("Dashboard", "HouseHolds");
            }
            ViewData["HouseHoldId"] = new SelectList(_context.HouseHold, "Id", "Name", invitation.HouseHoldId);
            return View(invitation);
        }

        public async Task<IActionResult> Accept(string email, string code)
        {
            var invitation = await _context.Invitation.FirstOrDefaultAsync(i => i.Code.ToString() == code);
            if (invitation == null || invitation.Accepted == true || DateTime.Now > invitation.Expires)
            {
                return NotFound();
            }

            invitation.Accepted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("SpecialRegistration", new { code = invitation.Code });
        }

        public async Task<IActionResult> SpecialRegistration(string code)
        {
            return View();
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
