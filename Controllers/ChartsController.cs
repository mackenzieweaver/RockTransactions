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
            var list = new List<CategoryItemsBarChartData>();
            var user = await _userManager.GetUserAsync(User);
            var houseHold = await _context.HouseHold
                .Include(hh => hh.Categories).ThenInclude(c => c.CategoryItems)
                .FirstOrDefaultAsync(hh => hh.Id == user.HouseHoldId);

            foreach(var category in houseHold.Categories)
            {
                foreach (var item in category.CategoryItems)
                {
                    list.Add(
                    new CategoryItemsBarChartData
                    {
                        Category = category.Name,
                        Name = item.Name,
                        Goal = item.TargetAmount,
                        Reality = item.ActualAmount
                    });
                }
            }            
            return Json(list);
        }
    }
}
