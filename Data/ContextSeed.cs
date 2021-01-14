using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RockTransactions.Data.Enums;
using RockTransactions.Models;
using RockTransactions.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RockTransactions.Data
{
    public static class ContextSeed
    {
        public static async Task SeedDataBaseAsync(ApplicationDbContext context, UserManager<FPUser> userManager, RoleManager<IdentityRole> roleManager, IFPFileService fileService, IConfiguration configuration)
        {
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager, fileService, configuration, context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Head.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Member.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.New.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Demo.ToString()));
        }

        private static async Task SeedUsersAsync(UserManager<FPUser> userManager, IFPFileService fileService, IConfiguration configuration, ApplicationDbContext context)
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

            #region New
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

            #region Demo
            user = new FPUser();
            user.UserName = configuration["DemoEmail"];
            user.Email = configuration["DemoEmail"];
            user.FirstName = "Rock";
            user.LastName = "Demo";
            user.FileData = await fileService.GetDefaultAvatarFileBytesAsync();
            user.FileName = fileService.GetDefaultAvatarFileName();
            user.EmailConfirmed = true;
            try
            {
                var test = await userManager.FindByEmailAsync(user.Email);
                if (test == null)
                {
                    await userManager.CreateAsync(user, configuration["DemoPassword"]);
                    await userManager.AddToRoleAsync(user, Roles.Head.ToString());
                    await userManager.AddToRoleAsync(user, Roles.Demo.ToString());
                    await SeedAccount(user, context);
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

        private static async Task SeedAccount(FPUser user, ApplicationDbContext context)
        {
            await SeedHouseholdAsync(user, context);
            await SeedAccountsAsync(user, context);
            await SeedCategoriesAsync(user, context);
            await SeedItemsAsync(user, context);
            await SeedTransactionsAsync(user, context);
        }

        private static async Task SeedHouseholdAsync(FPUser user, ApplicationDbContext context)
        {
            var household = new HouseHold
            {
                Name = "Demo",
                Greeting = "Welcome",
                Established = DateTime.Now
            };
            context.Add(household);
            await context.SaveChangesAsync();
            user.HouseHoldId = household.Id;
            await context.SaveChangesAsync();
        }

        private static async Task SeedAccountsAsync(FPUser user, ApplicationDbContext context) 
        {
            //account 1
            var account = new BankAccount
            {
                HouseHoldId = (int)user.HouseHoldId,
                FPUserId = user.Id,
                Name = "Chase",
                Type = AccountType.Checking,
                StartingBalance = 500,
                CurrentBalance = 500
            };
            context.Add(account);
            await context.SaveChangesAsync();
            //account 2
            account = new BankAccount
            {
                HouseHoldId = (int)user.HouseHoldId,
                FPUserId = user.Id,
                Name = "WellsFargo",
                Type = AccountType.Savings,
                StartingBalance = 1000,
                CurrentBalance = 1000
            };
            context.Add(account);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCategoriesAsync(FPUser user, ApplicationDbContext context) 
        {
            var category = new Category
            {
                HouseHoldId = (int)user.HouseHoldId,
                Name = "House",
                Description = "All house related expenses ie: rent, repairs, lightbulbs, etc..."
            };
            context.Add(category);
            await context.SaveChangesAsync();

            category = new Category
            {
                HouseHoldId = (int)user.HouseHoldId,
                Name = "Car",
                Description = "All car related expenses ie: gas, insurance, oil changes, etc..."
            };
            context.Add(category);
            await context.SaveChangesAsync();

            category = new Category
            {
                HouseHoldId = (int)user.HouseHoldId,
                Name = "Food",
                Description = "All food related expenses ie: groceries, retaurants, snacks, etc..."
            };
            context.Add(category);
            await context.SaveChangesAsync();
        }

        private static async Task SeedItemsAsync(FPUser user, ApplicationDbContext context) 
        {
            var item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "House").Id,
                Name = "Rent",
                Description = "Monthly rent / mortgage",
                TargetAmount = 1200,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();
            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "House").Id,
                Name = "Repairs",
                Description = "Supplies for any damages done to house",
                TargetAmount = 100,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();
            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "House").Id,
                Name = "Lightbulbs",
                Description = "Supplies for any damages done to house",
                TargetAmount = 20,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();

            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "Car").Id,
                Name = "Gas",
                Description = "Monthly gasoline needed to drive around town",
                TargetAmount = 100,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();
            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "Car").Id,
                Name = "Insurance",
                Description = "Government requirement",
                TargetAmount = 200,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();
            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "Car").Id,
                Name = "Oil Change",
                Description = "Only needed once every three months",
                TargetAmount = (decimal)(29.99 / 3),
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();

            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "Food").Id,
                Name = "Walmart",
                Description = "General groceries",
                TargetAmount = 400,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();
            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "Food").Id,
                Name = "Restaurants",
                Description = "Eating out",
                TargetAmount = 100,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();
            item = new CategoryItem
            {
                CategoryId = context.Category.FirstOrDefault(c => c.HouseHoldId == user.HouseHoldId && c.Name == "Food").Id,
                Name = "Snacks",
                Description = "Lil extras every now and then",
                TargetAmount = 50,
                ActualAmount = 0
            };
            context.Add(item);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTransactionsAsync(FPUser user, ApplicationDbContext context) 
        {
            // accounts
            var account = await context.BankAccount
                .FirstOrDefaultAsync(ba => ba.HouseHoldId == user.HouseHoldId && ba.Name == "Chase");
            var account2 = await context.BankAccount
                .FirstOrDefaultAsync(ba => ba.HouseHoldId == user.HouseHoldId && ba.Name == "WellsFargo");

            // t1
            var transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-14).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "Biweekly check",
                Amount = 1871.31M,
                IsDeleted = false
            };
            account.CurrentBalance += transaction.Amount;

            // so that only one history per day
            var history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 1
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-14).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // CategoryItems that belong to demo household
            var items = context.HouseHold.Where(hh => hh.Id == user.HouseHoldId)
                .Include(hh => hh.Categories).ThenInclude(c => c.CategoryItems).FirstOrDefault()
                .Categories.SelectMany(c => c.CategoryItems);

            // t2
            var item = items.Where(i => i.Name == "Rent").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-13).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "Monthly rent",
                Amount = 1200,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 2
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-13).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t3
            item = items.Where(i => i.Name == "Repairs").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-12).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "Touch-up paint",
                Amount = 20,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 3
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-12).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t4
            item = items.Where(i => i.Name == "Lightbulbs").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-11).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "white bulbs to replace orange",
                Amount = 20,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 4
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-11).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t5
            item = items.Where(i => i.Name == "Gas").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-10).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "to/from work",
                Amount = 80,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 5
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-10).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t6
            item = items.Where(i => i.Name == "Insurance").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-9).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "government mandate",
                Amount = 200,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 6
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-9).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t7
            item = items.Where(i => i.Name == "Oil Change").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-8).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "5000 miles since last one",
                Amount = 30,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 7
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-8).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t8
            item = items.Where(i => i.Name == "Walmart").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-7).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "groceries",
                Amount = 150,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 8
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-7).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t9
            item = items.Where(i => i.Name == "Restaurants").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-6).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "Arbies",
                Amount = 18.95M,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 9
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-6).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();

            // t10
            item = items.Where(i => i.Name == "Snacks").FirstOrDefault();
            transaction = new Transaction
            {
                Type = TransactionType.Withdrawal,
                CategoryItemId = item.Id,
                CategoryItem = item,
                BankAccountId = account.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-5).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "Chips/pretzels",
                Amount = 5.65M,
                IsDeleted = false
            };
            account.CurrentBalance -= transaction.Amount;
            transaction.CategoryItem.ActualAmount += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account);
            await context.SaveChangesAsync();

            // SAVINGS 10
            transaction = new Transaction
            {
                Type = TransactionType.Deposit,
                BankAccountId = account2.Id,
                FPUserId = user.Id,
                Created = DateTime.Now.AddDays(-5).AddHours(new Random().Next(24)).AddMinutes(new Random().Next(60)).AddSeconds(new Random().Next(60)),
                Memo = "20% of income",
                Amount = 100,
                IsDeleted = false
            };
            account2.CurrentBalance += transaction.Amount;

            // so that only one history per day
            history = await context.History
                .FirstOrDefaultAsync(
                    h => h.BankAccount == account2 &&
                    h.Date.Day == transaction.Created.Day);
            if (history == null)
            {
                History _history = new History
                {
                    BankAccountId = transaction.BankAccountId,
                    Balance = (decimal)account2.CurrentBalance,
                    Date = transaction.Created
                };
                context.Add(_history);
            }
            else
            {
                history.BankAccountId = transaction.BankAccountId;
                history.Balance = (decimal)account2.CurrentBalance;
                history.Date = transaction.Created;
                context.Update(history);
            }

            context.Add(transaction);
            context.Update(account2);
            await context.SaveChangesAsync();
        }
    }
}
