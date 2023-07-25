using Microsoft.EntityFrameworkCore;

namespace DeliverOptimization2022.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<OptimizationCloseDate> OptimizationCloseDate { get; set; }
        public DbSet<LoggedInEmployeeRole> EmployeeRole { get; set; }
        public DbSet<tbl_HCW_ErrorLog> tbl_HCW_ErrorLog { get; set; }
        public DbSet<Template_Master> TemplateMaster { get; set; }
        public DbSet<tbl_Update3Month> tbl_Update3Month { get; set; }
        public DbSet<tbl_Update6Month> tbl_Update6Month { get; set; }
        public DbSet<tbl_UpdateBillability> tbl_UpdateBillability { get; set; }
        public DbSet<tbl_Insert3MonthPivot> tbl_Insert3MonthPivot { get; set; }
        public DbSet<tbl_Update3MonthDetails> tbl_Update3MonthDetails { get; set; }
        public DbSet<tblGet3MonthPivot> tblGet3MonthPivot { get; set; }
        public DbSet<tbl_Update3MonthDetails_Final> tbl_Update3MonthDetails_Final { get; set; }
        public DbSet<tbl_BillabilityMonth> tbl_BillabilityMonth { get; set; }
        public DbSet<tbl_EmployeeOptimizationList_New> tbl_EmployeeOptimizationList_New { get; set; }
        public DbSet<tbl_DropdownValue> tbl_DropdownValue { get; set; }
        public DbSet<tbl_DropdownValue_New> tbl_DropdownValue_New { get; set; }
        public DbSet<tbl_PPMC_Position> tbl_PPMC_Position { get; set; }
        public DbSet<tbl_EmployeeRotation> tbl_EmployeeRotation { get; set; }
            public DbSet<tbl_EmployeeMigration> tbl_EmployeeMigration { get; set; }


    }
}
