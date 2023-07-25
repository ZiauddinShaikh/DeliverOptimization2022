using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class tbl_EmployeeOptimizationList_New
    {
		
		[Key]
		public int ID { get; set; }
		public string? Eid { get; set; }
		public string? EmployeeName { get; set; }
		public string? EmployeeStatus { get; set; }
		public string? HireDate { get; set; }
		public string? DateofLeaving { get; set; }
		public string? EmployeeType { get; set; }
		public string? PositionType { get; set; }
		public string? TimeType { get; set; }
		public string? IsMGR { get; set; }
		public string? JobLevel { get; set; }
		public string? MgmtLevel { get; set; }
		public string? CompanyCode { get; set; }
		public string? AcquisitionName { get; set; }
		public string? Country { get; set; }
		public string? RevenueRegion { get; set; }
		public string? WorkLocationRegion { get; set; }
		public string? ServiceLine { get; set; }
		public string? BSDHIL3 { get; set; }
		public string? DXCMHL5 { get; set; }
		public string? GidcFlag { get; set; }
		public string? PeopleMgrName { get; set; }
		public string? PeopleMgrEmail { get; set; }
		public string? PeopleMgrL3Chief { get; set; }
		public string? PeopleMgrL4Chief { get; set; }
		public string? PeopleMgrL5Chief { get; set; }
		public string? PeopleMgrL6Chief { get; set; }
		public string? PeopleMgrL7Chief { get; set; }
		public string? PeopleMgrL8Chief { get; set; }
		public string? NextlevelMgrName { get; set; }
		public string? NextlevelMgrEmail { get; set; }
		public string? Source { get; set; }
		public string? BillabilityCurrentMth { get; set; }
		public string? BillabilityPrvMth { get; set; }
		public string? Last6MonthAvgBill { get; set; }
		public string? TTClientID { get; set; }
		public string? TTClient { get; set; }
		public string? PpmcAllocStatus { get; set; }
		public string? SystemAppear { get; set; }
		public string? Category { get; set; }
		public string? Exception { get; set; }
		public string? Month { get; set; }
		public string? Level1DD { get; set; }
		public string? Level2DD { get; set; }
		public string? Action { get; set; }
		public string? ActionStatus { get; set; }
		public string? ActionDueDate { get; set; }
		public string? Comment { get; set; }
		public string? ModifiedBy { get; set; }
		public string? ModifiedOn { get; set; }
        public string? Skills { get; set; }
        public string? Technical_Competencies { get; set; }
        public string? Capability { get; set; }
        public string? Mgr_BSDHIL1 { get; set; }
        public string? Next_Mgr_BSDHIL1 { get; set; }
        public string? GIDCCountry { get; set; }
    }
}
