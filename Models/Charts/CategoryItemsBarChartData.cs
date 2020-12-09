using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Models.Charts
{
    public class CategoryItemsBarChartData
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public decimal Goal { get; set; }
        public decimal Reality { get; set; }
    }
}
