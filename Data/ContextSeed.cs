using Microsoft.AspNetCore.Identity;
using RockTransactions.Data.Enums;
using RockTransactions.Models;
using RockTransactions.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Data
{
    public static class ContextSeed
    {
        public static async Task SeedDataBaseAsync(ApplicationDbContext context, UserManager<FPUser> userManager, RoleManager<IdentityRole> roleManager, IFPFileService fileService)
        {
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager, fileService);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Head.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Member.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.New.ToString()));
        }

        private static async Task SeedUsersAsync(UserManager<FPUser> userManager, IFPFileService fileService)
        {
            #region Admin
            var user = new FPUser();
            user.UserName = "mw@mailinator.com";
            user.Email = "mw@mailinator.com";
            user.FirstName = "Mackenzie";
            user.LastName = "Weaver";
            user.FileData = await fileService.GetDefaultAvatarFileBytesAsync();
            user.FileName = fileService.GetDefaultAvatarFileName();
            user.EmailConfirmed = true;
            try
            {
                var test = await userManager.FindByEmailAsync(user.Email);
                if (test == null)
                {
                    await userManager.CreateAsync(user, "Mweaver1!");
                    await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("========= ERROR ============");
                Console.WriteLine($"Error Seeding {user.Email}");
                Console.WriteLine(ex.Message);
                Console.WriteLine("============================");
                throw;
            }
            #endregion

            #region Head
            user = new FPUser
            {
                UserName = "ar@mailinator.com",
                Email = "ar@mailinator.com",
                FirstName = "Antonio",
                LastName = "Raynor",
                FileData = await fileService.GetDefaultAvatarFileBytesAsync(),
                FileName = fileService.GetDefaultAvatarFileName(),
                EmailConfirmed = true
            };
            try
            {
                var test = await userManager.FindByEmailAsync(user.Email);
                if (test == null)
                {
                    await userManager.CreateAsync(user, "Araynor1!");
                    await userManager.AddToRoleAsync(user, Roles.Head.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("========= ERROR ============");
                Console.WriteLine($"Error Seeding {user.Email}");
                Console.WriteLine(ex.Message);
                Console.WriteLine("============================");
                throw;
            }
            #endregion

            #region Member
            user = new FPUser
            {
                UserName = "jt@mailinator.com",
                Email = "jt@mailinator.com",
                FirstName = "Jason",
                LastName = "Twichell",
                FileData = await fileService.GetDefaultAvatarFileBytesAsync(),
                FileName = fileService.GetDefaultAvatarFileName(),
                EmailConfirmed = true
            };
            try
            {
                var test = await userManager.FindByEmailAsync(user.Email);
                if (test == null)
                {
                    await userManager.CreateAsync(user, "Jtwichell1!");
                    await userManager.AddToRoleAsync(user, Roles.Member.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("========= ERROR ============");
                Console.WriteLine($"Error Seeding {user.Email}");
                Console.WriteLine(ex.Message);
                Console.WriteLine("============================");
                throw;
            }
            #endregion

            #region Member
            user = new FPUser
            {
                UserName = "dr@mailinator.com",
                Email = "dr@mailinator.com",
                FirstName = "Drew",
                LastName = "Russell",
                FileData = await fileService.GetDefaultAvatarFileBytesAsync(),
                FileName = fileService.GetDefaultAvatarFileName(),
                EmailConfirmed = true
            };
            try
            {
                var test = await userManager.FindByEmailAsync(user.Email);
                if (test == null)
                {
                    await userManager.CreateAsync(user, "Drussell1!");
                    await userManager.AddToRoleAsync(user, Roles.New.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("========= ERROR ============");
                Console.WriteLine($"Error Seeding {user.Email}");
                Console.WriteLine(ex.Message);
                Console.WriteLine("============================");
                throw;
            }
            #endregion
        }
    }
}
