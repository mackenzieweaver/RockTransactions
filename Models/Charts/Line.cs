using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Models.Charts
{
    public class Line
    {
        public string Name { get; set; }
        public List<string> Xcords { get; set; }  // dates
        public List<decimal> Ycords { get; set; } // balances
    }
}
