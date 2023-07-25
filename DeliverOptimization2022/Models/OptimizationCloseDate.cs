using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
	public class OptimizationCloseDate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required(ErrorMessage ="Please enter date")]
		public DateTime CloseDate { get; set; }

		[Required(ErrorMessage ="Please select a file to upload")]
		
		public string? UploadFileName { get; set; }
		public bool Status { get; set; }
		public string? Region { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		[DefaultValue("getutcdate()")]
		public DateTime CreatedDate { get; set; }
	}
}
