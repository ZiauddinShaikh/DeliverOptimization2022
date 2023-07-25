using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class tbl_Update3MonthDetails_Final
    {
        [Key]
        public int Id { get; set; }
        public string? EID { get; set; }
        public string? EmployeeName { get; set; }
        public string? PeopleMgrName { get; set; }
        public string? PeopleMgrEmail { get; set; }
        public string? RevenueRegion { get; set; }
        public string? ServiceLine { get; set; }
        public string? Country { get; set; }
        public string? DXCMHL5 { get; set; }
        public string? PeopleManagerL3Cheif { get; set; }
        public string? PeopleManagerL4Cheif { get; set; }
        public string? PeopleManagerL5Cheif { get; set; }
        public string? BillabilityCurrentMth { get; set; }
        public string? Month { get; set; }
    }
}
