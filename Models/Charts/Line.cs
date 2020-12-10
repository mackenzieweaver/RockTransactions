using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Models.Charts
{
    public class Chart
    {
        // x
        public List<string> Dates { get; set; } = new List<string>();
        
        // many lines
        public List<Line> Lines { get; set; } = new List<Line>();
    }

    public class Line
    {
        // each line has a name
        public string BankName { get; set; }

        // unique balance
        public List<decimal> Balances { get; set; } = new List<decimal>();
    }
}
