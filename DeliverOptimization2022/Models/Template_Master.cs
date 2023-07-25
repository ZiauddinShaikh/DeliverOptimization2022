using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class Template_Master
    {
		[Key]
		[Required]
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Subject { get; set; }
		public string? From_Address { get; set; }
		public string? Password { get; set; }
	}
}
