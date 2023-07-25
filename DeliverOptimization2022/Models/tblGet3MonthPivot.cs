using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class tblGet3MonthPivot
    {
        public string? PeopleManagerName { get; set; }
        [Key]
        public string? PeopleManagerEmail { get; set; }
        public string? CurrentMonthMinus3 { get; set; }
        public string? CurrentMonthMinus2 { get; set; }
        public string? CurrentMonthMinus1 { get; set; }
    }
}
