using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class tbl_Insert3MonthPivot
    {
        public int Id { get; set; }
        public string? PeopleManagerName { get; set; }
        public string? PeopleManagerEmail { get; set; }
        public string? CurrentMonthMinus3 { get; set; }
        public string? CurrentMonthMinus2 { get; set; }
        public string? CurrentMonthMinus1 { get; set; }
    }
}
