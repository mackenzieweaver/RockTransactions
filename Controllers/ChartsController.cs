using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models;
using RockTransactions.Models.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Controllers
{
    [Authorize]
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<FPUser> _userManager;

        public ChartsController(ApplicationDbContext context, UserManager<FPUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<JsonResult> Categories()
        {
            var list = new List<BudgetBreakDownPieChartData>();
            var user = await _userManager.GetUserAsync(User);
            var houseHold = await _context.HouseHold
                .Include(hh => hh.Categories).ThenInclude(c => c.CategoryItems)
                .FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);

            foreach (var category in houseHold.Categories)
            {
                decimal total = 0;
                foreach (var item in category.CategoryItems)
                {
                    total += item.ActualAmount;
                }

                list.Add(
                new BudgetBreakDownPieChartData
                {
                    Name = category.Name,
                    Total = total
                });
            }
            return Json(list);
        }

        [Authorize(Roles = "Admin,Head,Member")]
        public async Task<JsonResult> Items()
        {
            var user = await _userManager.GetUserAsync(User);
            var houseHold = await _context.HouseHold.Include(hh => hh.Categories).ThenInclude(c => c.CategoryItems).FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);
            var items = houseHold.Categories.SelectMany(c => c.CategoryItems).ToList();
            var list = new List<CategoryItemsBarChartData>();
            items.ForEach(item => list.Add(
                new CategoryItemsBarChartData
                {
                    Category = item.Category.Name,
                    Name = item.Name,
                    Goal = item.TargetAmount,
                    Reality = item.ActualAmount
                }
            ));
            return Json(list);
        }

        //[Authorize(Roles = "Admin,Head,Member")]
        public async Task<JsonResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            var houseHold = await _context.HouseHold
                .Include(hh => hh.BankAccounts)
                .ThenInclude(ba => ba.Histories)
                .FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);

            var lines = new List<Line>();
            var oldest = DateTime.Now;
            var newest = DateTime.Now;

            foreach (var account in houseHold.BankAccounts)
            {
                var line = new Line
                {
                    Name = account.Name,
                    Xcords = new List<string>(),
                    Ycords = new List<decimal>()
                };

                var histories = account.Histories.OrderBy(h => h.Date).ToList(); // oldest to newest
                foreach (var history in histories)
                {
                    oldest = UpdateOldestDate(history.Date, oldest);
                    newest = UpdateNewestDate(history.Date, newest);
                    var date = history.Date.ToString("MM/dd/yyyy");
                    line.Xcords.Add(date);
                    line.Ycords.Add(history.Balance);
                }

                lines.Add(line);
            }

            List<string> dates = PopulateDates(oldest, newest);
            var chart = new Chart
            {
                Dates = dates,
                Lines = lines
            };
            return Json(chart);
        }

        // start date
        private DateTime UpdateOldestDate(DateTime date, DateTime oldest)
        {
            if (date < oldest)
            {
                return date;
            }
            return oldest;
        }

        // end date
        private DateTime UpdateNewestDate(DateTime date, DateTime newest)
        {
            if (date > newest)
            {
                return date;
            }
            return newest;
        }

        // list from start to end
        private List<string> PopulateDates(DateTime oldest, DateTime newest)
        {
            var dates = new List<string>();
            while (oldest < newest)
            {
                dates.Add(oldest.ToString("MM/dd/yyyy"));
                oldest = oldest.AddDays(1);
            }
            dates.Add(newest.ToString("MM/dd/yyyy"));
            return dates.Distinct().ToList();
        }
    }
}
