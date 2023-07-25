using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
	public class tbl_HCW_ErrorLog
	{
		[Key]
		public int Id { get; set; }
		public string? ApplicationName { get; set; }
		public string? ControllerName { get; set; }
		public string? ActionName { get; set; }
		public string? ErrorMessage { get; set; }
	}
}
