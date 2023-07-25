using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Models
{
	public class Filter
	{
		public string FilterString { get; set; }
		public string L3Manager { get; set; }
		public string L4Manager { get; set; }
		public string L5Manager { get; set; }
		public string L6Manager { get; set; }
		public string InputReceived { get; set; }
		public string RevenueRegion { get; set; }
		public string ServiceLine { get; set; }
		public string GIDC { get; set; }
		

		public Filter(string filterString)
		{
			FilterString = filterString ?? "all;all;all;all;all;all;all;all;all";
			string[] filterValue = FilterString.Split(";");
			L3Manager = filterValue[0];
			L4Manager = filterValue[1];
			L5Manager = filterValue[2];
			L6Manager = filterValue[3];
			InputReceived = filterValue[4];
			RevenueRegion = filterValue[5];
			ServiceLine = filterValue[6];
			GIDC = filterValue[7];
			
		}

		public bool HasL3Manager => L3Manager != "all";
		public bool HasL4Manager => L4Manager != "all";
		public bool HasL5Manager => L5Manager != "all";
		public bool HasL6Manager => L6Manager != "all";
		public bool HasInputReceived => InputReceived != "all";
		public bool HasRevenueRegion => RevenueRegion != "all";
		public bool HasServiceLine => ServiceLine != "all";
		public bool HasGIDC => GIDC != "all";
		

		public Dictionary<string, string> ddlInputReceived =>
			new Dictionary<string, string>
			{
				{"Billable","Billable" },
				{"Internal Chargeable","Internal Chargeable" },
				{"Internal Non-Chargeable","Internal Non-Chargeable" },
				{"Bench/ Not Assigned","Bench/ Not Assigned" },
				{"Solutioning","Solutioning" },
				{"Planned Exit","Planned Exit" },
				{"Talent Opt / Reskilling / Transfer","Talent Opt / Reskilling / Transfer" },
			};

		

		public Dictionary<string, string> ddlInputReceivedEMEA =>
			new Dictionary<string, string>
			{
				{"Bench- Not Assigned","Bench- Not Assigned" },
				{"Forgot to enter timesheet or entered incorrectly","Forgot to enter timesheet or entered incorrectly" },
				{"IWO-Cross-charge issue","IWO-Cross-charge issue" },
				{"Increase Billability (additional work) partially","Increase Billability (additional work) partially" },
				{"Just assigned to new work-new client","Just assigned to new work-new client" },
				{"Manager did not approve timesheet on time","Manager did not approve timesheet on time" },
				{"Personal issue-On Leave","Personal issue-On Leave" },
				{"Planned Re-deploy, Re-assign (move into different role) full time","Planned Re-deploy, Re-assign (move into different role) full time" },
				{"T&M direct billable to client","T&M direct billable to client" },
				{"Time Tracking system, setup or reporting issue","Time Tracking system, setup or reporting issue" },
				{"Under Review - Will Leave Assignment (DXC)","Under Review - Will Leave Assignment (DXC)" },
				{"Update Billability Categorization","Update Billability Categorization" },
				{"Waiting for Client’s BGV clearance","Waiting for Client’s BGV clearance" },
			};


	}
}
