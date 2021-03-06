﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RockTransactions.Services;
using Microsoft.AspNetCore.Identity;
using System.IO;
using RockTransactions.Data.Enums;

namespace RockTransactions.Controllers
{
    [Authorize]
    public class AttachmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FPUser> _userManager;
        private readonly IFPFileService _fileservice;

        public AttachmentsController(ApplicationDbContext context, UserManager<FPUser> userManager, IFPFileService fileservice)
        {
            _context = context;
            _userManager = userManager;
            _fileservice = fileservice;
        }

        // GET: Attachments
        [Authorize(Roles = "Admin,Head")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var applicationDbContext = _context.Attachment.Include(a => a.HouseHold)
                .Where(a => a.HouseHoldId == user.HouseHoldId);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Attachments/Details/5
        [Authorize(Roles = "Admin,Head")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await _context.Attachment
                .Include(a => a.HouseHold)
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.GetUserAsync(User);
            if (attachment.HouseHoldId != user.HouseHoldId)
            {
                return NotFound();
            }

            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        // GET: Attachments/Create
        [Authorize(Roles = "Admin,Head")]
        public IActionResult Create()
        {
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name");
            return View();
        }

        // POST: Attachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Create([Bind("Id,HouseHoldId,FileName,Description,ContentType,FileData")] Attachment attachment, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                attachment.HouseHoldId = (int)user.HouseHoldId;
                attachment.FileName = file.FileName;
                attachment.FileData = await _fileservice.ConvertFileToByteArrayAsync(file);
                attachment.ContentType = $"application/{Path.GetExtension(file.FileName).Replace(".", "")}";
                attachment.Uploaded = DateTime.Now;
                _context.Add(attachment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name", attachment.HouseHoldId);
            return View(attachment);
        }

        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Statement(int id)
        {
            var a = await _context.Attachment.FirstOrDefaultAsync(a => a.Id == id);

            var user = await _userManager.GetUserAsync(User);
            if (a.HouseHoldId != user.HouseHoldId)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", $"inline; filename={a.FileName}");
            return File(a.FileData, a.ContentType);
        }

        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Download(int id)
        {
            var a = await _context.Attachment.FirstOrDefaultAsync(a => a.Id == id);

            var user = await _userManager.GetUserAsync(User);
            if (a.HouseHoldId != user.HouseHoldId)
            {
                return NotFound();
            }

            return File(a.FileData, a.ContentType, $"C:/Users/mackn/Downloads/{a.FileName}");
        }

        // GET: Attachments/Edit/5
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attachment = await _context.Attachment.FindAsync(id);

            var user = await _userManager.GetUserAsync(User);
            if (attachment.HouseHoldId != user.HouseHoldId)
            {
                return NotFound();
            }

            if (attachment == null)
            {
                return NotFound();
            }
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name", attachment.HouseHoldId);
            return View(attachment);
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HouseHoldId,FileName,Description,ContentType,FileData")] Attachment attachment)
        {
            if (id != attachment.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (attachment.HouseHoldId != user.HouseHoldId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attachment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttachmentExists(attachment.Id))
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
            ViewData["HouseHoldId"] = new SelectList(_context.Set<HouseHold>(), "Id", "Name", attachment.HouseHoldId);
            return View(attachment);
        }

        // GET: Attachments/Delete/5
        [Authorize(Roles = "Admin,Head")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (User.IsInRole(Roles.Demo.ToString()))
            {
                TempData["Script"] = "DemoCantDelete()";
                return RedirectToAction(nameof(Index));
            }

            if (id == null)
            {
                return NotFound();
            }

            var attachment = await _context.Attachment
                .Include(a => a.HouseHold)
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.GetUserAsync(User);
            if (attachment.HouseHoldId != user.HouseHoldId)
            {
                return NotFound();
            }

            if (attachment == null)
            {
                return NotFound();
            }

            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Head")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attachment = await _context.Attachment.FindAsync(id);

            var user = await _userManager.GetUserAsync(User);
            if (attachment.HouseHoldId != user.HouseHoldId)
            {
                return NotFound();
            }

            _context.Attachment.Remove(attachment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttachmentExists(int id)
        {
            return _context.Attachment.Any(e => e.Id == id);
        }
    }
}
