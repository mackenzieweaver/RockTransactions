﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RockTransactions.Data;
using RockTransactions.Extensions;
using RockTransactions.Models;
using RockTransactions.Services;

namespace RockTransactions.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<FPUser> _userManager;
        private readonly SignInManager<FPUser> _signInManager;
        private readonly IFPFileService _fileService;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<FPUser> userManager,
            SignInManager<FPUser> signInManager,
            IFPFileService fileService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [StringLength(50)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [StringLength(50)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Avatar")]
            [NotMapped]
            [DataType(DataType.Upload)]
            [MaxFileSize(2 * 1024 * 1024)]
            [AllowedExtensions(new string[] { ".jpg", ".png" })]
            public IFormFile Avatar { get; set; }
        }

        private async Task LoadAsync(FPUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            // First Name
            if (Input.FirstName != null)
            {
                user.FirstName = Input.FirstName;
                await _context.SaveChangesAsync();
            }
            // Last Name
            if (Input.LastName != null)
            {
                user.LastName = Input.LastName;
                await _context.SaveChangesAsync();
            }
            // Phone Number
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            // Avatar
            if (Input.Avatar != null)
            {
                user.FileData = await _fileService.ConvertFileToByteArrayAsync(Input.Avatar);
                user.FileName = Input.Avatar.FileName;
                await _context.SaveChangesAsync();
            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
