using System.ComponentModel.DataAnnotations;

namespace DeliverOptimization2022.Models
{
    public class tbl_EmployeeMigration
    {
        [Key]
        public int Id { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeEmail { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerEmail { get; set; }
        public string? NeedResourceSkill { get; set; }
        public string? MatchingPosition { get; set; }
        public string? Account { get; set; }
        public string? PositionFTE { get; set; }
        public string? PositionEndDate { get; set; }
        public string? BillingType { get; set; }
        public string? WorkOnlyOnsite { get; set; }
        public string? SpecifyCountry { get; set; }
        public string? ContractualLanguage { get; set; }
        public string? FTETracked { get; set; }
        public string? ContractualConstraint { get; set; }
        public string? FullfilmentConstraints { get; set; }
        public string? YourAssessment { get; set; }
        public string? FTE { get; set; }
        public string? MigrateTo { get; set; }
        public string? EarliestReleasedDate { get; set; }
        public string? RiskToDelivery { get; set; }
        public string? ConfirmCurrentAccount { get; set; }
        public string? AccountContact { get; set; }
        public string? Comment { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedOn { get; set; }
        public string? LoadDate { get; set; }
        public string? PositonName { get; set; }

        public string? SystemAppear { get; set; }


    }
}
