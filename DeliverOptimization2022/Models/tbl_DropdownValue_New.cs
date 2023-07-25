using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class tbl_DropdownValue_New
    {
        [Key]
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Region { get; set; }
        public string? ServiceLine { get; set; }
        public string? Dropdown1 { get; set; }
        public string? Dropdown2 { get; set; }
    }
}
