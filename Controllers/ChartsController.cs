using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RockTransactions.Data;
using RockTransactions.Models.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public JsonResult CategoryItemsJsonData()
        {
            var list = new List<CategoryItemsBarChartData>();
            var items = _context.CategoryItem.ToList();
            foreach(var item in items)
            {
                list.Add(
                new CategoryItemsBarChartData
                {
                    Name = item.Name,
                    Goal = item.TargetAmount,
                    Reality = item.ActualAmount
                });
            }
            return Json(list);
        }

        public async Task<JsonResult> BudgetBreakdownJsonData()
        {
            var list = new List<BudgetBreakDownPieChartData>();
            var categories = await _context.Category
                .Include(c => c.CategoryItems)
                .ToListAsync();

            foreach (var category in categories)
            {
                decimal total = 0;
                foreach(var item in category.CategoryItems)
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
    }
}
