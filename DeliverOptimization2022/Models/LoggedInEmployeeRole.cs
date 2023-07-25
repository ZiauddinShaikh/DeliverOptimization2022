using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
	public class LoggedInEmployeeRole
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string? Email { get; set; }
		[Required]
		public string? Role { get; set; }
		public string? Region { get; set; }
		public string? ServiceLine { get; set; }
		[Required]
		public string? UserName { get; set; }
		public string? ShortName { get; set; }
		public string? ToolsDescription { get; set; }
		public string? Country { get; set; }
	}
}
