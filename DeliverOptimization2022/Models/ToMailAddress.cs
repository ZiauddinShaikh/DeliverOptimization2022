using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class ToMailAddress
    {
		[Key]
		[Required]
		public int Id { get; set; }
		public string? EmailAddress { get; set; }
		public string? TemplateType { get; set; }


		[Required(ErrorMessage = "Please select a file to upload")]
		[NotMapped]
		public IFormFile? UploadFileName { get; set; }
	}
}
