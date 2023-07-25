using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class EmployeeUpdate
	{
		public int id { get; set; }
		public string? employeeId { get; set; }
		public string? employeeName { get; set; }
		public string? peopleManagerName { get; set; }
		public string? category { get; set; }
		public string? additionalComment { get; set; }
	}
}
