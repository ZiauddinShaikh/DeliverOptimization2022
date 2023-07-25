using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
    public class TemplateViewModel
    {
		public Template_Master? templateMaster { get; set; }
		public DateTime? date { get; set; }
		public string? Month { get; set; }
		public int Percentage { get; set; }
		public string? HostName { get; set; }
		public string? SmtpServer { get; set; }
		public int SmtpPortNumber { get; set; }
		public string? FromAddress { get; set; }
		public string? FromAddressTitle { get; set; }
		public string? ToAdressTitle { get; set; }
		public IEnumerable<ToMailAddress>? ToMails { get; set; }
	}
}
