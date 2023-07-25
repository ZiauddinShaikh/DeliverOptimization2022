using System.ComponentModel.DataAnnotations;

namespace DeliverOptimization2022.Models
{
    public class tbl_BillabilityMonth
    {
        [Key]
        public int Id { get; set; }
        public string? BillabilityCurrMonth { get; set; }
        public string? BillabilityPreMonth { get; set; }
    }
}
