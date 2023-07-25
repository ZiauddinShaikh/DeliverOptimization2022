using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DeliverOptimization2022.Session;
using DeliverOptimization2022.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeliverOptimization2022.Controllers
{
    public class DeliverOptimizationNew : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private readonly ILogger<HomeController> logger;
        public DeliverOptimizationNew(IWebHostEnvironment _environment, IConfiguration _configuration,
            ApplicationDbContext dbContext, ILogger<HomeController> logger)
        {
            Environment = _environment;
            Configuration = _configuration;
            this.logger = logger;
            _dbContext = dbContext;
        }
        public IActionResult SessionExpired()
        {
            var session = new UserSession(HttpContext.Session);
            //session.setUser("", "", "", "", "", "");
            //session.setFilter("N", "all;all;all;all;all;all;all;all");
            session.SetMyEmployeeNew(new List<tbl_EmployeeOptimizationList_New>());
            session.SetEmployeeRoleList(new LoggedInEmployeeRole());
            SignOut();
            return View();
        }

		public IActionResult Index()
		{
			try
			{
				//Get the User Email from User Identity Class
				string userShortName = User.Identity.Name;

				//Initialize the session
				var session = new UserSession(HttpContext.Session);
                //string UserRole = session.GetManager();

                //if (string.IsNullOrEmpty(UserRole))
                //{
                //    return RedirectToAction("SessionExpired");
                //}

                //Get the user details based on User Email
                //var adminlist = _dbContext.EmployeeRole.ToList();

                //List<tbl_EmployeeOptimizationList_New> newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y")).ToList();
                //session.SetMyEmployeeNew(newList);



                var adminUser = _dbContext.EmployeeRole.Where(c => c.Email == userShortName).FirstOrDefault();

                //LoggedInEmployeeRole adminUser = new LoggedInEmployeeRole();
                //adminUser = session.GetEmployeeRoleList();
                //if (adminUser.Email == null)
                //{
                //    var adminUser = _dbContext.EmployeeRole.Where(c => c.Email == userShortName).FirstOrDefault();
                //    session.SetEmployeeRoleList(adminUser);
                //}

                
                if (adminUser != null)
                {
                    
                        List<tbl_EmployeeOptimizationList_New> newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y")).ToList();
                        session.SetMyEmployeeNew(newList);
                    if (adminUser.Role == "SAdmin")
                    {
                        session.setUser(adminUser.UserName,adminUser.Email, adminUser.Region, adminUser.ServiceLine, userShortName, "");
                        session.setDOTManagerRole("SAdmin");
                        return RedirectToAction("NewDOT", "DeliverOptimizationNew");
                    }
                    else if (adminUser.Role == "Admin")
                    {
                        session.setUser(adminUser.UserName, adminUser.Email, adminUser.Region, adminUser.ServiceLine, userShortName, "");
                        session.setDOTManagerRole("Admin");
                        return RedirectToAction("NewDOT", "DeliverOptimizationNew");
                    }
                }
                else
                {
                   
                        var DOTUser = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.PeopleMgrEmail == userShortName && c.SystemAppear.StartsWith("Y")).FirstOrDefault();
                        if (DOTUser == null)
                        {
                            DOTUser = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.NextlevelMgrEmail == userShortName && c.SystemAppear.StartsWith("Y")).FirstOrDefault();
                            if (DOTUser != null)
                            {
                                session.setUser(DOTUser.NextlevelMgrName, DOTUser.NextlevelMgrEmail, "", "", userShortName, "");
                                session.setDOTManagerRole("NPM");
                                List<tbl_EmployeeOptimizationList_New> newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.NextlevelMgrEmail == userShortName && c.SystemAppear.StartsWith("Y")).ToList();
                                session.SetMyEmployeeNew(newList);
                                return RedirectToAction("NewDOT", "DeliverOptimizationNew");
                            }
                            else
                            {
                                TempData["NoDataMessage"] = "No EID/s for inputs under your hierarchy";
                            }
                        }
                        else
                        {
                            session.setUser(DOTUser.PeopleMgrName, DOTUser.PeopleMgrEmail, "", "", userShortName, "");
                            session.setDOTManagerRole("PM");
                            List<tbl_EmployeeOptimizationList_New> newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.PeopleMgrEmail == userShortName && c.SystemAppear.StartsWith("Y")).ToList();
                            session.SetMyEmployeeNew(newList);
                            return RedirectToAction("NewDOT", "DeliverOptimizationNew");
                        }
                    
                    
                }
				return View();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
				log.ApplicationName = "DeliverOptimizationTools";
				log.ControllerName = "HomeController";
				log.ActionName = "Index";
				log.ErrorMessage = ex.Message;
				_dbContext.tbl_HCW_ErrorLog.Add(log);
				_dbContext.SaveChanges();
				return View("/Error");
			}
		}

        public IActionResult NewDOT() 
        {
            var session = new UserSession(HttpContext.Session);
            string UserRole = session.GetManager();
            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToAction("SessionExpired");
            }
            return View();
		}
        public IActionResult UpdateEmployeeRecordNew(string EmployeeId, string Level1DD, string Level2DD, string Actionvalue, string ActionStatus, string ActionDueDate, string Comments)
        {
            var session = new UserSession(HttpContext.Session);
            string UserRole = session.GetManager();

            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToAction("SessionExpired");
            }
            //var session = new UserSession(HttpContext.Session);
            string userName = session.GetUsername();
            string userEmail = session.GetUseremail();
            tbl_EmployeeOptimizationList_New emp = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.Eid == EmployeeId).FirstOrDefault();
            emp.Level1DD = Level1DD;
            emp.Level2DD = Level2DD;
            emp.Action = Actionvalue;
            emp.ActionStatus = ActionStatus;
            emp.ActionDueDate = ActionDueDate;
            emp.Comment = Comments;
            emp.ModifiedBy = userName;
            emp.ModifiedOn = Convert.ToString(DateTime.Now.ToString());
            _dbContext.tbl_EmployeeOptimizationList_New.Update(emp);
            _dbContext.SaveChanges();

            //var data = _dbContext.EmployeeOptimizationList.Where(c => c.NextlevelMgrEmail == userEmail).ToList();
            var data = "Record Updated Sucessfully";
            return Json(new { data = data });
        }

        public IActionResult GetDropdown1Value(string region, string serviceLine, string category)
        {
            var session = new UserSession(HttpContext.Session);
            string UserRole = session.GetManager();

            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToAction("SessionExpired");
            }
            var val = _dbContext.tbl_DropdownValue_New.Where(c => c.Category == category && c.Region == region && c.ServiceLine == serviceLine).Select(c => c.Dropdown1).Distinct().ToList();
                //var val = _dbContext.tbl_DropdownValue.Where(c => c.Category == category).Select(c => c.Dropdown1).Distinct().ToList();
            return Json(new { data=val});
        }


        public IActionResult GetDropdown2Value(string region, string serviceLine, string category,string firstDropdownVal)
        {
            var session = new UserSession(HttpContext.Session);
            string UserRole = session.GetManager();

            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToAction("SessionExpired");
            }

            var val = _dbContext.tbl_DropdownValue_New.Where(c => c.Category == category && c.Dropdown1 == firstDropdownVal && c.Region == region && c.ServiceLine == serviceLine).Select(c => c.Dropdown2).Distinct().ToList();
            return Json(new { data = val });
        }

        public IActionResult GetEmployeeRecord()
        {
            try
            {
                var session = new UserSession(HttpContext.Session);
                string UserRole = session.GetManager();

                if (string.IsNullOrEmpty(UserRole))
                {
                    return RedirectToAction("SessionExpired");
                }
                //var session = new UserSession(HttpContext.Session);
                string useremail = session.GetUseremail();
                string filterString = session.GetFilterString();
                //LoggedInEmployeeRole emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();
                //LoggedInEmployeeRole emp = new LoggedInEmployeeRole();
                var emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();
                //emp = session.GetEmployeeRoleList();
                //if (emp.Email == null)
                //{
                //    emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();
                //    session.SetEmployeeRoleList(emp);
                //}

                //string userRole = session.GetManager();
                //string filterStringStatus = session.GetFilterStatus();
                //string region = session.GetRegion();
                //string serviceLine = session.GetServiceLine();
                string userRole = "";
                string region = "";
                string serviceLine = "";
                string toolDescription = "";

                if (emp != null)
                {
                    userRole = emp.Role;
                    region = emp.Region;
                    serviceLine = emp.ServiceLine;
                    toolDescription = emp.ToolsDescription;
                }
                else
                {
                    userRole = session.GetManager();
                }

                //Setting parameter for filter and paging
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;



                List<tbl_EmployeeOptimizationList_New> newList = session.GetEmployeeNew();
                IQueryable<tbl_EmployeeOptimizationList_New> Masterquery = newList.AsQueryable();
                IQueryable<tbl_EmployeeOptimizationList_New> customerData = null;
                
                //session.SetMyEmployeeNew(Masterquery.ToList());
                IQueryable<tbl_EmployeeOptimizationList_New> query = Masterquery.Where(c => c.Category == "Low Billability");



                //Filtering data based on role
                if (userRole == "Admin" || userRole == "SAdmin")
                {

                    Filter filter = new Filter(filterString);
                    //var count = _dbContext.EmployeeOptimizationList.Count();
                    
                    if (!string.IsNullOrEmpty(region))
                    {
                        string[] reg = region.Split(',');
                        if (reg.Count() == 1)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString());
                        }
                        else if (reg.Count() == 2)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString());
                        }
                        else if (reg.Count() == 3)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString() || c.RevenueRegion == reg[2].ToString());
                        }
                        else if (reg.Count() == 4)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString() || c.RevenueRegion == reg[2].ToString() || c.RevenueRegion == reg[3].ToString());
                        }
                        customerData = query;
                    }
                    else
                    {
                        customerData = query;
                    }



                    if (!string.IsNullOrEmpty(serviceLine))
                    {
                        string[] sl = serviceLine.Split(',');
                        if (sl.Count() == 1)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString());
                        }
                        else if (sl.Count() == 2)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString());
                        }
                        else if (sl.Count() == 3)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString());
                        }
                        else if (sl.Count() == 4)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            );
                        }
                        else if (sl.Count() == 5)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            || c.ServiceLine == sl[4].ToString());
                        }
                        else if (sl.Count() == 6)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            || c.ServiceLine == sl[4].ToString() || c.ServiceLine == sl[5].ToString());
                        }
                        else if (sl.Count() == 7)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            || c.ServiceLine == sl[4].ToString() || c.ServiceLine == sl[5].ToString() || c.ServiceLine == sl[6].ToString());
                        }
                        customerData = query;
                    }
                    else
                    {
                        customerData = query;
                    }



                    //if (!string.IsNullOrEmpty(serviceLine))
                    //    query = query.Where(c => c.ServiceLine == serviceLine);
                    //if (!string.IsNullOrEmpty(filterString))
                    //{

                    //    if (filter.HasL3Manager)
                    //        query = query.Where(c => c.PeopleMgrL3Chief == filter.L3Manager);

                    //    if (filter.HasL4Manager)
                    //        query = query.Where(c => c.PeopleMgrL4Chief == filter.L4Manager);

                    //    if (filter.HasL5Manager)
                    //        query = query.Where(c => c.PeopleMgrL5Chief == filter.L5Manager);

                    //    if (filter.HasL6Manager)
                    //        query = query.Where(c => c.PeopleMgrL6Chief == filter.L6Manager);

                    //    //if (toolDescription == "EMEA")
                    //    //{
                    //    //    if (filter.HasInputReceived)
                    //    //        query = query.Where(c => c.BusinessJustification == filter.InputReceived);
                    //    //}
                    //    //else
                    //    //{
                    //    //    if (filter.HasInputReceived)
                    //    //        query = query.Where(c => c.Category == filter.InputReceived);
                    //    //}


                    //    if (filter.HasRevenueRegion)
                    //        query = query.Where(c => c.RevenueRegion == filter.RevenueRegion);

                    //    if (filter.HasServiceLine)
                    //        query = query.Where(c => c.ServiceLine == filter.ServiceLine);

                    //    if (filter.HasGIDC)
                    //        query = query.Where(c => c.GidcFlag == filter.GIDC);



                    //    customerData = query;

                    //}
                    //else
                    //{
                    //    customerData = query;
                    //}
                }

                if (userRole == "PM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.PeopleMgrEmail == useremail
                                    || tempcustomer.NextlevelMgrEmail == useremail
                                    select tempcustomer);

                    customerData = customerData.Where(c => c.SystemAppear.StartsWith("Y"));

                }

                if (userRole == "NPM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.PeopleMgrEmail == useremail
                                    || tempcustomer.NextlevelMgrEmail == useremail
                                    select tempcustomer);

                    customerData = customerData.Where(c => c.SystemAppear.StartsWith("Y"));
                }

                //searching data based on input parameter in the grid search textbox
                if (!string.IsNullOrEmpty(searchValue))
                {
                     customerData = customerData.Where(m => m.EmployeeName.Contains(searchValue)
                                                || m.Country.Contains(searchValue)
                                                || m.Eid.Contains(searchValue)
                                                //|| m.TTClient.Contains(searchValue)
                                                //|| m.BillabilityCurrentMth.Contains(searchValue)
                                                //|| m.BillabilityPrvMth.Contains(searchValue)
                                                //|| m.Last6MonthAvgBill.Contains(searchValue)
                                                //|| m.PeopleMgrName.Contains(searchValue)
                                                //|| m.WorkLocationRegion.Contains(searchValue)
                                                || m.ServiceLine.Contains(searchValue)
                                                || m.RevenueRegion.Contains(searchValue));
                }
                var data = new List<tbl_EmployeeOptimizationList_New>();
                if (customerData != null)
                {
                    
                    recordsTotal = customerData.Count();
                    data = customerData.Skip(skip).Take(pageSize).ToList();
                }
                else
                {
                    recordsTotal = 0;
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "EmployeeController";
                log.ActionName = "GetEmployeeRecord";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }
        }
        public IActionResult GetEmployeeRecordTab2()
        {
            try
            {
                var session = new UserSession(HttpContext.Session);
                string UserRole = session.GetManager();
                if (string.IsNullOrEmpty(UserRole))
                {
                    return RedirectToAction("SessionExpired");
                }
                //var session = new UserSession(HttpContext.Session);
                string useremail = session.GetUseremail();
                string filterString = session.GetFilterString();

                var emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();

                //LoggedInEmployeeRole emp = new LoggedInEmployeeRole();
                //emp = session.GetEmployeeRoleList();
                //if (emp.Email == null)
                //{
                //    emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();
                //    session.SetEmployeeRoleList(emp);
                //}

                //string userRole = session.GetManager();
                //string filterStringStatus = session.GetFilterStatus();
                //string region = session.GetRegion();
                //string serviceLine = session.GetServiceLine();
                string userRole = "";
                string region = "";
                string serviceLine = "";
                string toolDescription = "";

                if (emp != null)
                {
                    userRole = emp.Role;
                    region = emp.Region;
                    serviceLine = emp.ServiceLine;
                    toolDescription = emp.ToolsDescription;
                }
                else
                {
                    userRole = session.GetManager();
                }

                //Setting parameter for filter and paging
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                List<tbl_EmployeeOptimizationList_New> newList = session.GetEmployeeNew();
                IQueryable<tbl_EmployeeOptimizationList_New> Masterquery = newList.AsQueryable();
                IQueryable<tbl_EmployeeOptimizationList_New> customerData = null;
                //IQueryable<tbl_EmployeeOptimizationList_New> Masterquery = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y"));
                session.SetMyEmployeeNew(Masterquery.ToList());
                IQueryable<tbl_EmployeeOptimizationList_New> query = Masterquery.Where(c => c.Category == "Low Billability Outliers");



                //Filtering data based on role
                if (userRole == "Admin" || userRole == "SAdmin")
                {

                    //Filter filter = new Filter(filterString);
                    //var count = _dbContext.EmployeeOptimizationList.Count();

                    if (!string.IsNullOrEmpty(region))
                    {
                        string[] reg = region.Split(',');
                        if (reg.Count() == 1)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString());
                        }
                        else if (reg.Count() == 2)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString());
                        }
                        else if (reg.Count() == 3)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString() || c.RevenueRegion == reg[2].ToString());
                        }
                        else if (reg.Count() == 4)
                        {
                            query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString() || c.RevenueRegion == reg[2].ToString()  || c.RevenueRegion == reg[3].ToString());
                        }
                        customerData = query;
                    }
                    else
                    {
                        customerData = query;
                    }



                    if (!string.IsNullOrEmpty(serviceLine))
                    {
                        string[] sl = serviceLine.Split(',');
                        if (sl.Count() == 1)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString());
                        }
                        else if (sl.Count() == 2)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString());
                        }
                        else if (sl.Count() == 3)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString());
                        }
                        else if (sl.Count() == 4)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            );
                        }
                        else if (sl.Count() == 5)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            || c.ServiceLine == sl[4].ToString());
                        }
                        else if (sl.Count() == 6)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            || c.ServiceLine == sl[4].ToString() || c.ServiceLine == sl[5].ToString());
                        }
                        else if (sl.Count() == 7)
                        {
                            query = query.Where(c => c.ServiceLine == sl[0].ToString() || c.ServiceLine == sl[1].ToString() || c.ServiceLine == sl[2].ToString() || c.ServiceLine == sl[3].ToString()
                            || c.ServiceLine == sl[4].ToString() || c.ServiceLine == sl[5].ToString() || c.ServiceLine == sl[6].ToString());
                        }
                        customerData = query;
                    }
                    else
                    {
                        customerData = query;
                    }



                    //if (!string.IsNullOrEmpty(serviceLine))
                    //    query = query.Where(c => c.ServiceLine == serviceLine);
                    //if (!string.IsNullOrEmpty(filterString))
                    //{

                    //    if (filter.HasL3Manager)
                    //        query = query.Where(c => c.PeopleMgrL3Chief == filter.L3Manager);

                    //    if (filter.HasL4Manager)
                    //        query = query.Where(c => c.PeopleMgrL4Chief == filter.L4Manager);

                    //    if (filter.HasL5Manager)
                    //        query = query.Where(c => c.PeopleMgrL5Chief == filter.L5Manager);

                    //    if (filter.HasL6Manager)
                    //        query = query.Where(c => c.PeopleMgrL6Chief == filter.L6Manager);

                    //    //if (toolDescription == "EMEA")
                    //    //{
                    //    //    if (filter.HasInputReceived)
                    //    //        query = query.Where(c => c.BusinessJustification == filter.InputReceived);
                    //    //}
                    //    //else
                    //    //{
                    //    //    if (filter.HasInputReceived)
                    //    //        query = query.Where(c => c.Category == filter.InputReceived);
                    //    //}


                    //    if (filter.HasRevenueRegion)
                    //        query = query.Where(c => c.RevenueRegion == filter.RevenueRegion);

                    //    if (filter.HasServiceLine)
                    //        query = query.Where(c => c.ServiceLine == filter.ServiceLine);

                    //    if (filter.HasGIDC)
                    //        query = query.Where(c => c.GidcFlag == filter.GIDC);



                    //    customerData = query;

                    //}
                    //else
                    //{
                    //    customerData = query;
                    //}
                }

                if (userRole == "PM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.PeopleMgrEmail == useremail
                                    || tempcustomer.NextlevelMgrEmail == useremail
                                    select tempcustomer);

                    customerData = customerData.Where(c => c.SystemAppear.StartsWith("Y"));

                }

                if (userRole == "NPM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.PeopleMgrEmail == useremail
                                    || tempcustomer.NextlevelMgrEmail == useremail
                                    select tempcustomer);

                    customerData = customerData.Where(c => c.SystemAppear.StartsWith("Y"));
                }

                //searching data based on input parameter in the grid search textbox
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.EmployeeName.Contains(searchValue)
                                                 || m.Country.Contains(searchValue)
                                                 || m.Eid.Contains(searchValue)
                                                 //|| m.TTClient.Contains(searchValue)
                                                 //|| m.BillabilityCurrentMth.Contains(searchValue)
                                                 //|| m.BillabilityPrvMth.Contains(searchValue)
                                                 //|| m.Last6MonthAvgBill.Contains(searchValue)
                                                 //|| m.PeopleMgrName.Contains(searchValue)
                                                 //|| m.WorkLocationRegion.Contains(searchValue)
                                                 || m.ServiceLine.Contains(searchValue)
                                                 || m.RevenueRegion.Contains(searchValue));
                }
                var data = new List<tbl_EmployeeOptimizationList_New>();
                if (customerData != null)
                {

                    recordsTotal = customerData.Count();
                    data = customerData.Skip(skip).Take(pageSize).ToList();
                }
                else
                {
                    recordsTotal = 0;
                }
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "EmployeeController";
                log.ActionName = "GetEmployeeRecord";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }
        }

       

        public ActionResult Report()
        {
            var session = new UserSession(HttpContext.Session);
            string UserRole = session.GetManager();
            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToAction("SessionExpired");
            }
            var data = new DataTable();
            data = GetEmployeeDetails();
            ExportToExcel(data);
            return View("Index");
        }

        //this method is used to generate the excel template;
        private void ExportToExcel(DataTable products)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("EmployeeDetails");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "EID";
                worksheet.Cell(currentRow, 2).Value = "Employee Name";
                worksheet.Cell(currentRow, 3).Value = "Employee Status";
                worksheet.Cell(currentRow, 4).Value = "Hire Date";
                worksheet.Cell(currentRow, 5).Value = "Employee Type";
                worksheet.Cell(currentRow, 6).Value = "Position Type";
                worksheet.Cell(currentRow, 7).Value = "Time Type";
                worksheet.Cell(currentRow, 8).Value = "Is MGR";
                worksheet.Cell(currentRow, 9).Value = "Job Level";
                worksheet.Cell(currentRow, 10).Value = "Mgmt Level";
                worksheet.Cell(currentRow, 11).Value = "Company Code";
                worksheet.Cell(currentRow, 12).Value = "Country";
                worksheet.Cell(currentRow, 13).Value = "Revenue Region";
                worksheet.Cell(currentRow, 14).Value = "Service Line";
                worksheet.Cell(currentRow, 15).Value = "Geo Region";
                worksheet.Cell(currentRow, 16).Value = "BSDHI L3";
                worksheet.Cell(currentRow, 17).Value = "DXC MH L5";
                worksheet.Cell(currentRow, 18).Value = "GIDC";
                worksheet.Cell(currentRow, 19).Value = "People Mgr Name";
                worksheet.Cell(currentRow, 20).Value = "People Mgr Email";
                worksheet.Cell(currentRow, 21).Value = "People Mgr L3 Chief";
                worksheet.Cell(currentRow, 22).Value = "People Mgr L4 Chief";
                worksheet.Cell(currentRow, 23).Value = "People Mgr L5 Chief";
                worksheet.Cell(currentRow, 24).Value = "People Mgr L6 Chief";
                worksheet.Cell(currentRow, 25).Value = "People Mgr L7 Chief";
                worksheet.Cell(currentRow, 26).Value = "People Mgr L8 Chief";
                worksheet.Cell(currentRow, 27).Value = "Next Level Mgr Name";
                worksheet.Cell(currentRow, 28).Value = "Next Level Mgr Email";
                worksheet.Cell(currentRow, 29).Value = "Source";
                worksheet.Cell(currentRow, 30).Value = "Billability Current Mth";
                worksheet.Cell(currentRow, 31).Value = "Billability Previous Mth";
                worksheet.Cell(currentRow, 32).Value = "Billability Avg Three Mth";
                worksheet.Cell(currentRow, 33).Value = "TT Client ID";
                worksheet.Cell(currentRow, 34).Value = "TT Client";
                worksheet.Cell(currentRow, 35).Value = "Exception";
                worksheet.Cell(currentRow, 36).Value = "SystemAppear";
                worksheet.Cell(currentRow, 37).Value = "Category";
                worksheet.Cell(currentRow, 38).Value = "Level 1 DD";
                worksheet.Cell(currentRow, 39).Value = "Level 2 DD";
                worksheet.Cell(currentRow, 40).Value = "Action";
                worksheet.Cell(currentRow, 41).Value = "Action Due Date";
                worksheet.Cell(currentRow, 42).Value = "Action Status";
                worksheet.Cell(currentRow, 43).Value = "Remarks";
                worksheet.Cell(currentRow, 44).Value = "Modified By";
                worksheet.Cell(currentRow, 45).Value = "Modified On";


                for (int i = 0; i < products.Rows.Count; i++)
                {
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = products.Rows[i]["EID"].ToString();
                        worksheet.Cell(currentRow, 2).Value = products.Rows[i]["Employee Name"].ToString();
                        worksheet.Cell(currentRow, 3).Value = products.Rows[i]["Employee Status"].ToString();
                        worksheet.Cell(currentRow, 4).Value = products.Rows[i]["Hire Date"].ToString();
                        worksheet.Cell(currentRow, 5).Value = products.Rows[i]["Employee Type"].ToString();
                        worksheet.Cell(currentRow, 6).Value = products.Rows[i]["Position Type"].ToString();
                        worksheet.Cell(currentRow, 7).Value = products.Rows[i]["Time Type"].ToString();
                        worksheet.Cell(currentRow, 8).Value = products.Rows[i]["Is MGR"].ToString();
                        worksheet.Cell(currentRow, 9).Value = products.Rows[i]["Job Level"].ToString();
                        worksheet.Cell(currentRow, 10).Value = products.Rows[i]["Mgmt Level"].ToString();
                        worksheet.Cell(currentRow, 11).Value = products.Rows[i]["Company Code"].ToString();
                        worksheet.Cell(currentRow, 12).Value = products.Rows[i]["Country"].ToString();
                        worksheet.Cell(currentRow, 13).Value = products.Rows[i]["Revenue Region"].ToString();
                        worksheet.Cell(currentRow, 14).Value = products.Rows[i]["Service Line"].ToString();
                        worksheet.Cell(currentRow, 15).Value = products.Rows[i]["Geo Region"].ToString();
                        worksheet.Cell(currentRow, 16).Value = products.Rows[i]["BSDHI L3"].ToString();
                        worksheet.Cell(currentRow, 17).Value = products.Rows[i]["DXC MH L5"].ToString();
                        worksheet.Cell(currentRow, 18).Value = products.Rows[i]["GIDC"].ToString();
                        worksheet.Cell(currentRow, 19).Value = products.Rows[i]["People Mgr Name"].ToString();
                        worksheet.Cell(currentRow, 20).Value = products.Rows[i]["People Mgr Email"].ToString();
                        worksheet.Cell(currentRow, 21).Value = products.Rows[i]["People Mgr L3 Chief"].ToString();
                        worksheet.Cell(currentRow, 22).Value = products.Rows[i]["People Mgr L4 Chief"].ToString();
                        worksheet.Cell(currentRow, 23).Value = products.Rows[i]["People Mgr L5 Chief"].ToString();
                        worksheet.Cell(currentRow, 24).Value = products.Rows[i]["People Mgr L6 Chief"].ToString();
                        worksheet.Cell(currentRow, 25).Value = products.Rows[i]["People Mgr L7 Chief"].ToString();
                        worksheet.Cell(currentRow, 26).Value = products.Rows[i]["People Mgr L8 Chief"].ToString();
                        worksheet.Cell(currentRow, 27).Value = products.Rows[i]["Next Level Mgr Name"].ToString();
                        worksheet.Cell(currentRow, 28).Value = products.Rows[i]["Next Level Mgr Email"].ToString();
                        worksheet.Cell(currentRow, 29).Value = products.Rows[i]["Source"].ToString();
                        worksheet.Cell(currentRow, 30).Value = products.Rows[i]["Billability Current Mth"].ToString();
                        worksheet.Cell(currentRow, 31).Value = products.Rows[i]["Billability Previous Mth"].ToString();
                        worksheet.Cell(currentRow, 32).Value = products.Rows[i]["Billability Avg Three Mth"].ToString();
                        worksheet.Cell(currentRow, 33).Value = products.Rows[i]["TT Client ID"].ToString();
                        worksheet.Cell(currentRow, 34).Value = products.Rows[i]["TT Client"].ToString();
                        worksheet.Cell(currentRow, 35).Value = products.Rows[i]["Exception"].ToString();
                        worksheet.Cell(currentRow, 36).Value = products.Rows[i]["SystemAppear"].ToString();
                        worksheet.Cell(currentRow, 37).Value = products.Rows[i]["Category"].ToString();
                        worksheet.Cell(currentRow, 38).Value = products.Rows[i]["Level 1 DD"].ToString();
                        worksheet.Cell(currentRow, 39).Value = products.Rows[i]["Level 2 DD"].ToString();
                        worksheet.Cell(currentRow, 40).Value = products.Rows[i]["Action"].ToString();
                        worksheet.Cell(currentRow, 41).Value = products.Rows[i]["Action Due Date"].ToString();
                        worksheet.Cell(currentRow, 42).Value = products.Rows[i]["Action Status"].ToString();
                        worksheet.Cell(currentRow, 43).Value = products.Rows[i]["Remarks"].ToString();
                        worksheet.Cell(currentRow, 44).Value = products.Rows[i]["Modified By"].ToString();
                        worksheet.Cell(currentRow, 45).Value = products.Rows[i]["Modified On"].ToString();

                    }
                }
                using var stream = new MemoryStream();
                worksheet.Range("A1:AU1").Style.Fill.BackgroundColor = XLColor.Purple;
                worksheet.Range("A1:AU1").Style.Font.FontColor = XLColor.White;
                worksheet.Range("A1:AU1").Style.Font.Bold = true;
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=Deliver Optimization Tool - Detailed Report.xlsx");
                Response.ContentType = "application/xlsx";
                Response.Body.WriteAsync(content);
                Response.Body.Flush();
            }
        }

        //This method is used to get the data from databased based on employee email;
        private DataTable GetEmployeeDetails()
        {

            var session = new UserSession(HttpContext.Session);
            string useremail = session.GetUseremail();
            string userRole = session.GetManager();

            var products = GetEmployeeOptimizationTool().ToList();
            DataTable dtProduct = new DataTable("ProductDetails");
            dtProduct.Columns.AddRange(new DataColumn[45] {

            new DataColumn("EID"),
            new DataColumn("Employee Name"),
            new DataColumn("Employee Status"),
            new DataColumn("Hire Date"),
            new DataColumn("Employee Type"),
            new DataColumn("Position Type"),
            new DataColumn("Time Type"),
            new DataColumn("Is MGR"),
            new DataColumn("Job Level"),
            new DataColumn("Mgmt Level"),
            new DataColumn("Company Code"),
            
            new DataColumn("Country"),
            new DataColumn("Revenue Region"),
            new DataColumn("Service Line"),
             new DataColumn("Geo Region"),
            new DataColumn("BSDHI L3"),
            new DataColumn("DXC MH L5"),
            new DataColumn("GIDC"),
            new DataColumn("People Mgr Name"),
            new DataColumn("People Mgr Email"),
            new DataColumn("People Mgr L3 Chief"),
            new DataColumn("People Mgr L4 Chief"),
            new DataColumn("People Mgr L5 Chief"),
            new DataColumn("People Mgr L6 Chief"),
            new DataColumn("People Mgr L7 Chief"),
            new DataColumn("People Mgr L8 Chief"),
            new DataColumn("Next Level Mgr Name"),
            new DataColumn("Next Level Mgr Email"),
            new DataColumn("Source"),
            new DataColumn("Billability Current Mth"),
            new DataColumn("Billability Previous Mth"),
            new DataColumn("Billability Avg Three Mth"),
            new DataColumn("TT Client ID"),
            new DataColumn("TT Client"),
          
            new DataColumn("Exception"),
            new DataColumn("SystemAppear"),
            new DataColumn("Category"),
            new DataColumn("Level 1 DD"),
            new DataColumn("Level 2 DD"),
             new DataColumn("Action"),
            new DataColumn("Action Due Date"),
             new DataColumn("Action Status"),
            new DataColumn("Remarks"),
            new DataColumn("Modified By"),
            new DataColumn("Modified On"),
            
            });

            foreach (var product in products)
            {
                dtProduct.Rows.Add(
                    product.Eid,
                    product.EmployeeName,
                    product.EmployeeStatus,
                    product.HireDate,
                   
                    product.EmployeeType,
                    product.PositionType,
                    product.TimeType,
                    product.IsMGR,
                    product.JobLevel,
                    product.MgmtLevel,
                    product.CompanyCode,
                    
                    product.Country,
                    product.RevenueRegion,
                    product.ServiceLine,
                    product.WorkLocationRegion,
                    product.BSDHIL3,
                    product.DXCMHL5,
                    product.GidcFlag,
                    product.PeopleMgrName,
                    product.PeopleMgrEmail,
                    product.PeopleMgrL3Chief,
                    product.PeopleMgrL4Chief,
                    product.PeopleMgrL5Chief,
                    product.PeopleMgrL6Chief,
                    product.PeopleMgrL7Chief,
                    product.PeopleMgrL8Chief,
                    product.NextlevelMgrName,
                    product.NextlevelMgrEmail,
                    product.Source,
                    product.BillabilityCurrentMth,
                    product.BillabilityPrvMth,
                    product.Last6MonthAvgBill,
                    product.TTClientID,
                    product.TTClient,
                    product.Exception,
                    product.SystemAppear,
                    product.Category,
                    product.Level1DD,
                    product.Level2DD,
                     product.Action,
                    product.ActionDueDate,
                     product.ActionStatus,
                    product.Comment,
                    product.ModifiedBy,
                    product.ModifiedOn
                      
                    );
            }

            return dtProduct;
        }

        private IEnumerable<tbl_EmployeeOptimizationList_New> GetEmployeeOptimizationTool()
        {

            var session = new UserSession(HttpContext.Session);
            string useremail = session.GetUseremail();
            string filterString = session.GetFilterString();


           var  emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();


            //LoggedInEmployeeRole emp = new LoggedInEmployeeRole();
            //emp = session.GetEmployeeRoleList();
            //if (emp.Email == null)
            //{
            //    emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();
            //    session.SetEmployeeRoleList(emp);
            //}

            string userRole = "";
            string region = "";
            string serviceLine = "";

            if (emp != null)
            {
                userRole = emp.Role;
                region = emp.Region;
                serviceLine = emp.ServiceLine;
            }
            else
            {
                userRole = session.GetManager();
            }

            IQueryable<tbl_EmployeeOptimizationList_New> customerData = null;

            //Filtering data based on role
            if (userRole == "Admin" || userRole == "SAdmin")
            {

                IQueryable<tbl_EmployeeOptimizationList_New> query = _dbContext.tbl_EmployeeOptimizationList_New;

                if (!string.IsNullOrEmpty(region))
                {
                    string[] reg = region.Split(',');
                    if (reg.Count() == 1)
                    {
                        query = query.Where(c => c.RevenueRegion == reg[0].ToString());
                    }
                    else if (reg.Count() == 2)
                    {
                        query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString());
                    }
                    else if (reg.Count() == 3)
                    {
                        query = query.Where(c => c.RevenueRegion == reg[0].ToString() || c.RevenueRegion == reg[1].ToString() || c.RevenueRegion == reg[2].ToString());
                    }
                }


                if (!string.IsNullOrEmpty(serviceLine))
                {
                    string[] reg = serviceLine.Split(',');
                    if (reg.Count() == 1)
                    {
                        query = query.Where(c => c.ServiceLine == reg[0].ToString());
                    }
                    else if (reg.Count() == 2)
                    {
                        query = query.Where(c => c.ServiceLine == reg[0].ToString() || c.ServiceLine == reg[1].ToString());
                    }
                    else if (reg.Count() == 3)
                    {
                        query = query.Where(c => c.ServiceLine == reg[0].ToString() || c.ServiceLine == reg[1].ToString() || c.ServiceLine == reg[2].ToString());
                    }
                }


                //if (!string.IsNullOrEmpty(serviceLine))
                //    query = query.Where(c => c.ServiceLine == serviceLine);

                customerData = query;

            }

            if (userRole == "PM")
            {
                customerData = (from tempcustomer in _dbContext.tbl_EmployeeOptimizationList_New
                                where tempcustomer.PeopleMgrEmail == useremail
                                || tempcustomer.NextlevelMgrEmail == useremail
                                select tempcustomer);

                customerData = customerData.Where(c => c.SystemAppear.StartsWith("Y"));

            }

            if (userRole == "NPM")
            {
                customerData = (from tempcustomer in _dbContext.tbl_EmployeeOptimizationList_New
                                where tempcustomer.PeopleMgrEmail == useremail
                                || tempcustomer.NextlevelMgrEmail == useremail
                                select tempcustomer);

                customerData = customerData.Where(c => c.SystemAppear.StartsWith("Y"));
            }


            List<tbl_EmployeeOptimizationList_New> data = customerData.ToList();
            return data;
        }
    }
}
