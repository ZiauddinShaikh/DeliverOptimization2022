using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class tbl_UpdateBillability
    {
        [Key]
        public int Id { get; set; }
        public int Eid { get; set; }
        public string? EmployeeName { get; set; }
        public string? PeopleMgrName { get; set; }
        public string? PeopleMgrEmail { get; set; }
        public string? RevenueRegion { get; set; }
        public string? ServiceLine { get; set; }
        public string? DXCMHL5 { get; set; }
        public string? BillablePer { get; set; }
        public string? Month { get; set; }
        public string? category { get; set; }
        public string? BusinessJustification { get; set; }
        public int NoOfOcc { get; set; }
        public string? Comments { get; set; }
        public string? TentitiveBillingDate { get; set; }
        public string? Country { get; set; }
        public string? L3Cheif { get; set; }
        public string? L4Cheif { get; set; }
        public string? L5Cheif { get; set; }
    }
}
