using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RockTransactions.Models
{
    public class Category
    {
        public int Id { get; set; }
        public int HouseHoldId { get; set; }
        public HouseHold HouseHold { get; set; }

        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Description { get; set; }

        public virtual ICollection<CategoryItem> CategoryItems { get; set; } = new HashSet<CategoryItem>();
    }
}
