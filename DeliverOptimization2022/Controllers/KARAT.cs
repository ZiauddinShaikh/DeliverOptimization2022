using ClosedXML.Excel;
using DeliverOptimization2022.Models;
using DeliverOptimization2022.Session;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace DeliverOptimization2022.Controllers
{
    public class KARAT : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private IWebHostEnvironment Environment;
        private IConfiguration Configuration;
        private readonly ILogger<HomeController> logger;
        public KARAT(IWebHostEnvironment _environment, IConfiguration _configuration,
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
            session.setUser("", "", "", "", "", "");

            session.SetMyEmployeeNew(new List<tbl_EmployeeOptimizationList_New>());
            session.SetEmployeeRoleList(new LoggedInEmployeeRole());
            session.SetMyEmployeeRotaton(new List<tbl_EmployeeRotation>());
            session.SetMyEmployeeMigration(new List<tbl_EmployeeMigration>());
            SignOut();
            return View();
        }



        List<tbl_EmployeeOptimizationList_New> newList = new List<tbl_EmployeeOptimizationList_New>();

        List<tbl_EmployeeRotation> rotation = new List<tbl_EmployeeRotation>();

        List<tbl_EmployeeMigration> migration = new List<tbl_EmployeeMigration>();

        public IActionResult Index()
        {
            try
            {
                //Get the User Email from User Identity Class
                string userShortName = User.Identity.Name;
                //string userShortName = "mingjie.chen@dxc.com";



                var query = from c in _dbContext.OptimizationCloseDate where c.CloseDate > DateTime.Now.Date && c.Status == true select c;
                //Initialize the session
                var session = new UserSession(HttpContext.Session);

                string EditKarat = session.GetLowBillabilityEdit();
                string EditRotation = session.GetRotationEdit();
                string EditMigration = session.GetMigrationEdit();

                LoggedInEmployeeRole adminUser = new LoggedInEmployeeRole();
                adminUser = session.GetEmployeeRoleList();
                if (adminUser.Email == null)
                {
                    adminUser = _dbContext.EmployeeRole.Where(c => c.Email == userShortName).FirstOrDefault();
                    session.SetEmployeeRoleList(adminUser);
                }



                if (EditKarat == "Y")
                {
                    if (adminUser == null)
                    {
                        newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y") && c.PeopleMgrEmail == userShortName).ToList();
                    }
                    else
                    {
                        newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y")).ToList();
                    }

                }
                else if (string.IsNullOrEmpty(EditKarat))
                {
                    if (adminUser == null)
                    {
                        newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y") && c.PeopleMgrEmail == userShortName).ToList();
                    }
                    else
                    {
                        newList = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y")).ToList();
                    }
                }
                else
                {
                    newList = session.GetEmployeeNew();
                }

                if (EditRotation == "Y")
                {
                    rotation = _dbContext.tbl_EmployeeRotation.ToList();

                }
                else if (string.IsNullOrEmpty(EditRotation))
                {
                    rotation = _dbContext.tbl_EmployeeRotation.ToList();

                }
                else
                {
                    rotation = session.GetEmployeeRotation().ToList();
                }

                if (EditMigration == "Y")
                {
                    migration = _dbContext.tbl_EmployeeMigration.Where(c => c.SystemAppear.StartsWith("Y")).ToList();
                }
                else if (string.IsNullOrEmpty(EditMigration))
                {
                    migration = _dbContext.tbl_EmployeeMigration.Where(c => c.SystemAppear.StartsWith("Y")).ToList();
                }
                else
                {
                    migration = session.GetEmployeeMigration().ToList();
                }


                if (adminUser != null)
                {
                    if (rotation.Count > 0)
                    {
                        session.SetMyEmployeeRotaton(rotation);
                    }
                    if (migration.Count > 0)
                    {
                        session.SetMyEmployeeMigration(migration);
                    }
                    //List<tbl_EmployeeOptimizationList_New> newList = _dbContext.tbl_EmployeeOptimizationList_New.ToList();
                    session.SetMyEmployeeNew(newList);
                    if (adminUser.Role == "SAdmin")
                    {
                        session.setUser(adminUser.UserName, adminUser.Email, adminUser.Region, adminUser.ServiceLine, userShortName, "");
                        session.setDOTManagerRole("SAdmin");
                        if (query.Count() > 0)
                        {
                            return RedirectToAction("NewKARAT", "KARAT");
                        }
                        else
                        {
                            session.setRotationTab("Yes");
                            session.setMigrationTab("Yes");
                            session.setLowBillabilityTab("No");
                            session.setLowBillabilityOutliersTab("No");
                            return RedirectToAction("NewKARAT", "KARAT");
                        }
                    }
                    else if (adminUser.Role == "Admin")
                    {
                        if (adminUser.ToolsDescription == "KARAT")
                        {
                            session.setRotationTab("No");
                            session.setMigrationTab("No");
                            session.setLowBillabilityTab("Yes");
                            session.setLowBillabilityOutliersTab("Yes");
                        }
                        else if (adminUser.ToolsDescription == "RM")
                        {
                            session.setRotationTab("Yes");
                            session.setMigrationTab("Yes");
                            session.setLowBillabilityTab("No");
                            session.setLowBillabilityOutliersTab("No");
                        }
                        else
                        {
                            session.setRotationTab("Yes");
                            session.setMigrationTab("Yes");
                            session.setLowBillabilityTab("Yes");
                            session.setLowBillabilityOutliersTab("Yes");
                        }
                        session.setUser(adminUser.UserName, adminUser.Email, adminUser.Region, adminUser.ServiceLine, userShortName, "");
                        session.setDOTManagerRole("Admin");
                        if (query.Count() > 0)
                        {
                            return RedirectToAction("NewKARAT", "KARAT");
                        }
                        else
                        {
                            session.setRotationTab("Yes");
                            session.setMigrationTab("Yes");
                            session.setLowBillabilityTab("No");
                            session.setLowBillabilityOutliersTab("No");
                            return RedirectToAction("NewKARAT", "KARAT");
                        }
                    }
                }
                else
                {
                    var DOTUser = newList.Where(c => c.PeopleMgrEmail == userShortName && c.SystemAppear.StartsWith("Y")).FirstOrDefault();
                    var DOTNextLevelManagerUser = newList.Where(c => c.NextlevelMgrEmail == userShortName && c.SystemAppear.StartsWith("Y")).FirstOrDefault();
                    var rotationUser = rotation.Where(c => c.ManagerEmail == userShortName).FirstOrDefault();
                    var MigrationUser = migration.Where(c => c.ManagerEmail == userShortName).FirstOrDefault();

                    bool displayPage = false;
                    if (DOTUser != null)
                    {
                        session.setUser(DOTUser.PeopleMgrName, DOTUser.PeopleMgrEmail, "", "", userShortName, "");
                        session.setDOTManagerRole("PM");
                        displayPage = true;
                    }
                    else if (DOTNextLevelManagerUser != null)
                    {
                        session.setUser(DOTNextLevelManagerUser.NextlevelMgrName, DOTNextLevelManagerUser.NextlevelMgrEmail, "", "", userShortName, "");
                        session.setDOTManagerRole("NPM");
                        displayPage = true;
                    }
                    else if (rotationUser != null)
                    {
                        session.setUser(rotationUser.ManagerName, rotationUser.ManagerEmail, "", "", userShortName, "");
                        session.setDOTManagerRole("PM");
                        displayPage = true;
                    }
                    else if (MigrationUser != null)
                    {
                        session.setUser(MigrationUser.ManagerName, MigrationUser.ManagerEmail, "", "", userShortName, "");
                        session.setDOTManagerRole("PM");
                        displayPage = true;
                    }
                    else
                    {
                        displayPage = false;
                    }
                    if (displayPage)
                    {
                        List<tbl_EmployeeOptimizationList_New> newList1 = newList.Where(c => c.PeopleMgrEmail == userShortName || c.NextlevelMgrEmail == userShortName).ToList();
                        session.SetMyEmployeeNew(newList1);
                        rotation = rotation.Where(c => c.ManagerEmail == userShortName).ToList();
                        migration = migration.Where(c => c.ManagerEmail == userShortName).ToList();
                        if (newList1.Where(c => c.Category == "Low Billability").Count() > 0)
                        {
                            session.setLowBillabilityTab("Yes");
                        }
                        else
                        {
                            session.setLowBillabilityTab("No");
                        }

                        if (newList1.Where(c => c.Category == "Low Billability Outliers").Count() > 0)
                        {
                            session.setLowBillabilityOutliersTab("Yes");
                        }
                        else
                        {
                            session.setLowBillabilityOutliersTab("No");
                        }


                        if (rotation.Count > 0)
                        {
                            session.SetMyEmployeeRotaton(rotation);
                            session.setRotationTab("Yes");
                        }
                        else
                        {
                            session.setRotationTab("No");
                        }

                        if (migration.Count > 0)
                        {
                            session.SetMyEmployeeMigration(migration);
                            session.setMigrationTab("Yes");
                        }
                        else
                        {
                            session.setMigrationTab("No");
                        }


                        if (query.Count() > 0)
                        {
                            return RedirectToAction("NewKARAT", "KARAT");
                        }
                        else
                        {
                            session.setLowBillabilityTab("No");
                            session.setLowBillabilityOutliersTab("No");
                            session.setRotationTab("Yes");
                            session.setMigrationTab("Yes");
                            return RedirectToAction("NewKARAT", "KARAT");
                        }
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
        public IActionResult NewKARAT()
        {
            var session = new UserSession(HttpContext.Session);
            string UserRole = session.GetManager();
            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToAction("SessionExpired");
            }
            return View();
        }

        public IActionResult CloseDateView()
        {
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
            tbl_EmployeeOptimizationList_New emp = session.GetEmployeeNew().Where(c => c.Eid == EmployeeId).FirstOrDefault();
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

            session.setLowBillabilityEdit("Y");
            session.setLowBillabilityOutliersEdit("Y");
            session.setRotationEdit("N");
            session.setMigrationEdit("N");

            //var data = _dbContext.EmployeeOptimizationList.Where(c => c.NextlevelMgrEmail == userEmail).ToList();
            var data = "Record Updated Sucessfully";
            return Json(new { data = data });
        }

        public IActionResult UpdateEmployeeRotation(string REID, string EmployeeName, string RotationYN, string RearliestReleasedDate, string RBackfillPlan,
            string RSpecifySuccessor, string RExpectedKTDuration, string REmployeeFlightRisk, string RRiskOfServiceDelivery, string RConfirmCurrentAccount,
            string RAccountContactForInterlock, string RComment, string flightRisk)
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
            tbl_EmployeeRotation emp = session.GetEmployeeRotation().Where(c => c.EmployeeID == REID).FirstOrDefault();
            emp.CanEmpReleasedIn6Month = RotationYN;
            emp.EarliestReleasedDate = RearliestReleasedDate;
            emp.BackfillPlan = RBackfillPlan;
            emp.SpecifySuccessor = RSpecifySuccessor;
            emp.ExpectedKTDuration = RExpectedKTDuration;
            emp.EmployeeFlightRisk = REmployeeFlightRisk;
            emp.RiskOfServiceDelivery = RRiskOfServiceDelivery;
            emp.ConfirmCurrentAccount = RConfirmCurrentAccount;
            emp.AccountContactForInterlock = RAccountContactForInterlock;
            emp.Comment = RComment;
            emp.ModifiedBy = userName;
            emp.ModifiedOn = Convert.ToString(DateTime.Now.ToString());
            _dbContext.tbl_EmployeeRotation.Update(emp);
            _dbContext.SaveChanges();
            session.setLowBillabilityEdit("N");
            session.setLowBillabilityOutliersEdit("N");
            session.setRotationEdit("Y");
            session.setMigrationEdit("N");
            //var data = _dbContext.EmployeeOptimizationList.Where(c => c.NextlevelMgrEmail == userEmail).ToList();
            var data = "Record Updated Sucessfully";
            return Json(new { data = data });
        }

        public IActionResult UpdateEmployeeMigration(string MEID, string txtMPositionFTE, string txtMPositionEndDate, string txtMBillingType, string txtMWorkCanOnlyBePerformedOnSite,
           string txtSpecifycountryduetoclientFacing, string txtContractuallyrequiredLanguage, string ddlContractualConstraint, string ddlFulfilmentconstraints,
           string ddlYourassessment, string ddlMigrateTo, string ddlMigrationRisk, string ppmcChecked, string txtMigrationConfirmcurrentaccount,
           string txtMigrationAccountContactforInterlock, string txtMigrationcomment, string txtMigrationEarliestReleaseDate, string txtMigrationFTE, string txtMPositionName)
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
            tbl_EmployeeMigration emp = session.GetEmployeeMigration().Where(c => c.EmployeeID == MEID).FirstOrDefault();
            if (ppmcChecked == "true")
            {
                emp.PositionFTE = txtMPositionFTE;
                emp.PositionEndDate = txtMPositionEndDate;
                emp.BillingType = txtMBillingType;
                emp.WorkOnlyOnsite = txtMWorkCanOnlyBePerformedOnSite;
                emp.SpecifyCountry = txtSpecifycountryduetoclientFacing;
                emp.ContractualLanguage = txtContractuallyrequiredLanguage;
                emp.PositonName = txtMPositionName;
            }
            emp.ContractualConstraint = ddlContractualConstraint;
            emp.FullfilmentConstraints = ddlFulfilmentconstraints;

            emp.YourAssessment = ddlYourassessment;
            emp.EarliestReleasedDate = txtMigrationEarliestReleaseDate;
            emp.FTE = txtMigrationFTE;
            emp.MigrateTo = ddlMigrateTo;
            emp.RiskToDelivery = ddlMigrationRisk;
            emp.ConfirmCurrentAccount = txtMigrationConfirmcurrentaccount;
            emp.AccountContact = txtMigrationAccountContactforInterlock;
            emp.Comment = txtMigrationcomment;

            emp.ModifiedBy = userName;
            emp.ModifiedOn = Convert.ToString(DateTime.Now.ToString());
            _dbContext.tbl_EmployeeMigration.Update(emp);
            _dbContext.SaveChanges();
            session.setLowBillabilityEdit("N");
            session.setLowBillabilityOutliersEdit("N");
            session.setRotationEdit("N");
            session.setMigrationEdit("Y");
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
            return Json(new { data = val });
        }

        public IActionResult GetPPMCPosition(string EmployeeId)
        {
            var session = new UserSession(HttpContext.Session);
            string UserRole = session.GetManager();

            if (string.IsNullOrEmpty(UserRole))
            {
                return RedirectToAction("SessionExpired");
            }
            var val = _dbContext.tbl_PPMC_Position.Where(c => c.EmployeeId == EmployeeId).Select(c => c.PositionIDName).ToList();
            //var val = _dbContext.tbl_DropdownValue.Where(c => c.Category == category).Select(c => c.Dropdown1).Distinct().ToList();
            return Json(new { data = val });
        }
        public IActionResult GetDropdown2Value(string region, string serviceLine, string category, string firstDropdownVal)
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
                string useremail = session.GetUseremail();
                var emp = session.GetEmployeeRoleList();
                string userRole = "";
                string region = "";
                string serviceLine = "";
                string toolDescription = "";
                string country = "";

                if (emp.Email != null)
                {
                    userRole = emp.Role;
                    region = emp.Region;
                    serviceLine = emp.ServiceLine;
                    toolDescription = emp.ToolsDescription;
                    country = emp.Country;
                }
                else
                {
                    userRole = session.GetManager();
                }

                List<tbl_EmployeeOptimizationList_New> newList = session.GetEmployeeNew();
                IQueryable<tbl_EmployeeOptimizationList_New> Masterquery = newList.AsQueryable();
                IQueryable<tbl_EmployeeOptimizationList_New> customerData = null;
                IQueryable<tbl_EmployeeOptimizationList_New> query = Masterquery.Where(c => c.Category == "Low Billability" && c.SystemAppear.StartsWith("Y"));


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
                    if (!string.IsNullOrEmpty(country))
                    {
                        string[] reg = country.Split(',');
                        if (reg.Count() == 1)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString());
                        }
                        else if (reg.Count() == 2)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString());
                        }
                        else if (reg.Count() == 3)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString() || c.GIDCCountry == reg[2].ToString());
                        }
                        else if (reg.Count() == 4)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString() || c.GIDCCountry == reg[2].ToString() || c.GIDCCountry == reg[3].ToString());
                        }
                        customerData = query;
                    }
                    else
                    {
                        customerData = query;
                    }
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
                string useremail = session.GetUseremail();
                var emp = session.GetEmployeeRoleList();
                string userRole = "";
                string region = "";
                string serviceLine = "";
                string toolDescription = "";
                string country = "";

                if (emp.Email != null)
                {
                    userRole = emp.Role;
                    region = emp.Region;
                    serviceLine = emp.ServiceLine;
                    toolDescription = emp.ToolsDescription;
                    country = emp.Country;
                }
                else
                {
                    userRole = session.GetManager();
                }

                List<tbl_EmployeeOptimizationList_New> newList = session.GetEmployeeNew();
                IQueryable<tbl_EmployeeOptimizationList_New> Masterquery = newList.AsQueryable();
                IQueryable<tbl_EmployeeOptimizationList_New> customerData = null;
                //IQueryable<tbl_EmployeeOptimizationList_New> Masterquery = _dbContext.tbl_EmployeeOptimizationList_New.Where(c => c.SystemAppear.StartsWith("Y"));
                session.SetMyEmployeeNew(Masterquery.ToList());
                IQueryable<tbl_EmployeeOptimizationList_New> query = Masterquery.Where(c => c.Category == "Low Billability Outliers" && c.SystemAppear.StartsWith("Y"));



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




                //Filtering data based on role
                if (userRole == "Admin" || userRole == "SAdmin")
                {
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
                    if (!string.IsNullOrEmpty(country))
                    {
                        string[] reg = country.Split(',');
                        if (reg.Count() == 1)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString());
                        }
                        else if (reg.Count() == 2)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString());
                        }
                        else if (reg.Count() == 3)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString() || c.GIDCCountry == reg[2].ToString());
                        }
                        else if (reg.Count() == 4)
                        {
                            query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString() || c.GIDCCountry == reg[2].ToString() || c.GIDCCountry == reg[3].ToString());
                        }
                        customerData = query;
                    }
                    else
                    {
                        customerData = query;
                    }


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

        public IActionResult GetEmployeeRecordRotation()
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
                //string filterString = session.GetFilterString();

                var emp = session.GetEmployeeRoleList();


                string userRole = "";
                string region = "";
                string serviceLine = "";
                string toolDescription = "";
                string country = "";

                if (emp.Email != null)
                {
                    userRole = emp.Role;
                    region = emp.Region;
                    serviceLine = emp.ServiceLine;
                    toolDescription = emp.ToolsDescription;
                    country = emp.Country;
                }
                else
                {
                    userRole = session.GetManager();
                }

                List<tbl_EmployeeRotation> newRotation = session.GetEmployeeRotation();
                if (userRole == "SAdmin" || userRole == "Admin")
                {
                    newRotation = newRotation.ToList();
                }
                else
                {
                    newRotation = newRotation.Where(c => c.ManagerEmail == useremail).ToList();
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







                IQueryable<tbl_EmployeeRotation> Masterquery = newRotation.AsQueryable();
                IQueryable<tbl_EmployeeRotation> customerData = null;

                IQueryable<tbl_EmployeeRotation> query = Masterquery;



                //Filtering data based on role
                if (userRole == "Admin" || userRole == "SAdmin")
                {
                    customerData = query;
                }

                if (userRole == "PM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.ManagerEmail == useremail
                                    select tempcustomer);



                }

                if (userRole == "NPM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.ManagerEmail == useremail
                                    select tempcustomer);


                }

                //searching data based on input parameter in the grid search textbox
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.EmployeeName.Contains(searchValue)
                                                 || m.EmployeeID.Contains(searchValue)
                                                 || m.Account.Contains(searchValue)
                                                 || m.NeedForRespons.Contains(searchValue)
                                                 || m.Position.Contains(searchValue));
                }
                var data = new List<tbl_EmployeeRotation>();
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

        public IActionResult GetEmployeeMigration()
        {
            try
            {
                var session = new UserSession(HttpContext.Session);
                string UserRole = session.GetManager();
                if (string.IsNullOrEmpty(UserRole))
                {
                    return RedirectToAction("SessionExpired");
                }
                string useremail = session.GetUseremail();
                var emp = session.GetEmployeeRoleList();
                string userRole = "";
                string region = "";
                string serviceLine = "";
                string toolDescription = "";
                string country = "";

                if (emp.Email != null)
                {
                    userRole = emp.Role;
                    region = emp.Region;
                    serviceLine = emp.ServiceLine;
                    toolDescription = emp.ToolsDescription;
                    country = emp.Country;
                }
                else
                {
                    userRole = session.GetManager();
                }


                List<tbl_EmployeeMigration> newMigration = session.GetEmployeeMigration();
                if (userRole == "SAdmin" || userRole == "Admin")
                {
                    newMigration = newMigration.ToList();
                }
                else
                {
                    newMigration = newMigration.Where(c => c.ManagerEmail == useremail).ToList();
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



                IQueryable<tbl_EmployeeMigration> Masterquery = newMigration.AsQueryable();
                IQueryable<tbl_EmployeeMigration> customerData = null;

                IQueryable<tbl_EmployeeMigration> query = Masterquery;



                //Filtering data based on role
                if (userRole == "Admin" || userRole == "SAdmin")
                {
                    customerData = query;
                }

                if (userRole == "PM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.ManagerEmail == useremail
                                    select tempcustomer);



                }

                if (userRole == "NPM")
                {
                    customerData = (from tempcustomer in query
                                    where tempcustomer.ManagerEmail == useremail
                                    select tempcustomer);


                }

                //searching data based on input parameter in the grid search textbox
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.EmployeeName.Contains(searchValue)
                                                 || m.EmployeeID.Contains(searchValue)
                                                 || m.Account.Contains(searchValue)
                                                 //|| m.TTClient.Contains(searchValue)
                                                 //|| m.BillabilityCurrentMth.Contains(searchValue)
                                                 //|| m.BillabilityPrvMth.Contains(searchValue)
                                                 //|| m.Last6MonthAvgBill.Contains(searchValue)
                                                 //|| m.PeopleMgrName.Contains(searchValue)
                                                 //|| m.WorkLocationRegion.Contains(searchValue)
                                                 || m.PositionEndDate.Contains(searchValue)
                                                 || m.MatchingPosition.Contains(searchValue));
                }
                var data = new List<tbl_EmployeeMigration>();
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
        public ActionResult GetChangeDate(string date, string region)
        {
            try
            {
                OptimizationCloseDate data = _dbContext.OptimizationCloseDate.Where(c => c.Status == true).FirstOrDefault();
                return Json(new { closeDate = Convert.ToDateTime(data.CloseDate).ToString("dd-MMM-yyyy"), region = region, createdDate = DateTime.Now.ToString("dd-MMM-yyyy") });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "HomeController";
                log.ActionName = "ChangeDate";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }

        }
        public ActionResult ChangeDate(string date, string region)
        {
            try
            {
                OptimizationCloseDate data = _dbContext.OptimizationCloseDate.Where(c => c.Status == true).FirstOrDefault();
                data.CloseDate = Convert.ToDateTime(date);
                _dbContext.OptimizationCloseDate.Update(data);
                _dbContext.SaveChanges();
                return Json(new { closeDate = Convert.ToDateTime(date).ToString("dd-MMM-yyyy"), region = region, createdDate = DateTime.Now.ToString("dd-MMM-yyyy") });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "HomeController";
                log.ActionName = "ChangeDate";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }

        }
        public IActionResult UploadFile()
        {
            try
            {
                var session = new UserSession(HttpContext.Session);
                string useremail = session.GetUseremail();
                if (string.IsNullOrEmpty(useremail))
                {
                    return RedirectToAction("SessionExpired");
                }
                //Get the last updated record from the Close date table.
                //OptimizationCloseDate query = _dbContext.OptimizationCloseDate.Where(c => c.Status == true).FirstOrDefault();
                //ViewBag.CloseDate = query.CloseDate;
                //ViewBag.UploadFileName = query.UploadFileName;
                //ViewBag.CreatedOn = query.CreatedDate;
                ////string userRole = session.GetManager();
                if (string.IsNullOrEmpty(useremail))
                {
                    //reset the session value to 
                    session.setUser("", "", "", "", "", "");
                    return RedirectToAction("Login", "Account");
                }
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "HomeController";
                log.ActionName = "UploadFile";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }

        }
        [HttpPost]
        public IActionResult UploadFiles(string date, string uploadType)
        {
            try
            {
                IFormFile file = Request.Form.Files[0];
                string folderName = "UploadExcel";
                string webRootPath = Environment.WebRootPath;
                string newPath = System.IO.Path.Combine(webRootPath, folderName);
                StringBuilder sb = new StringBuilder();
                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }
                if (file.Length > 0)
                {
                    string sFileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
                    string fullPath = System.IO.Path.Combine(newPath, file.FileName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                        fileStream.Close();
                    }
                    //Create an excel file to wwwroot folder
                    FileStream stream = System.IO.File.Open(fullPath, FileMode.Open, FileAccess.Read);
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    var result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    //Insert the Data read from the Excel file to Database Table.
                    DataTable dt = result.Tables[0];
                    excelReader.Close();
                    string SqlconString = this.Configuration.GetConnectionString("DefaultConnection");

                    using (SqlConnection con = new SqlConnection(SqlconString))
                    {
                        if (uploadType == "Truncate and Upload")
                        {
                            using (SqlCommand cmd = new SqlCommand("spTruncate-Insert-OptimizationTable_New", con))
                            {
                                //string closedate = Convert.ToString(formdata.CloseDate);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@Filename", SqlDbType.VarChar).Value = file.FileName;
                                //cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = item;

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }

                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("spInsertClosingDate", con))
                            {
                                //string closedate = Convert.ToString(formdata.CloseDate);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@Filename", SqlDbType.VarChar).Value = file.FileName;
                                //cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = item;

                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                        //this sp call is used to update the close date.


                        //Bulk copy method is used to read data from excel and upload into sql server.
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.tbl_EmployeeOptimizationList_New";

                            //[OPTIONAL]: Map the Excel columns with that of the database table.
                            sqlBulkCopy.ColumnMappings.Add("EmpID", "Eid");
                            sqlBulkCopy.ColumnMappings.Add("EE Name", "EmployeeName");
                            sqlBulkCopy.ColumnMappings.Add("Emp Status", "EmployeeStatus");
                            sqlBulkCopy.ColumnMappings.Add("Hire Date", "HireDate");
                            //sqlBulkCopy.ColumnMappings.Add("Date of Leaving", "DateofLeaving");
                            sqlBulkCopy.ColumnMappings.Add("Employee Type", "EmployeeType");
                            sqlBulkCopy.ColumnMappings.Add("Position Type", "PositionType");
                            sqlBulkCopy.ColumnMappings.Add("Time Type", "TimeType");
                            sqlBulkCopy.ColumnMappings.Add("Is Manager DXC", "IsMGR");
                            sqlBulkCopy.ColumnMappings.Add("Job Level", "JobLevel");
                            sqlBulkCopy.ColumnMappings.Add("Mgmt Level", "MgmtLevel");
                            sqlBulkCopy.ColumnMappings.Add("Company Code", "CompanyCode");
                            //sqlBulkCopy.ColumnMappings.Add("Acquisition Name", "AcquisitionName");
                            sqlBulkCopy.ColumnMappings.Add("Location Country", "Country");
                            sqlBulkCopy.ColumnMappings.Add("Revenue Region", "RevenueRegion");
                            sqlBulkCopy.ColumnMappings.Add("Work Location Region", "WorkLocationRegion");
                            sqlBulkCopy.ColumnMappings.Add("Service Line", "ServiceLine");
                            sqlBulkCopy.ColumnMappings.Add("BSDHI L3", "BSDHIL3");
                            sqlBulkCopy.ColumnMappings.Add("DXCMH L5 Name", "DXCMHL5");
                            sqlBulkCopy.ColumnMappings.Add("GIDC Flag", "GidcFlag");
                            sqlBulkCopy.ColumnMappings.Add("DXC Mgr Name", "PeopleMgrName");
                            sqlBulkCopy.ColumnMappings.Add("DXC Mgr Email", "PeopleMgrEmail");
                            sqlBulkCopy.ColumnMappings.Add("DXC L3 Name", "PeopleMgrL3Chief");
                            sqlBulkCopy.ColumnMappings.Add("DXC L4 Name", "PeopleMgrL4Chief");
                            sqlBulkCopy.ColumnMappings.Add("DXC L5 Name", "PeopleMgrL5Chief");
                            sqlBulkCopy.ColumnMappings.Add("DXC L6 Name", "PeopleMgrL6Chief");
                            sqlBulkCopy.ColumnMappings.Add("DXC L7 Name", "PeopleMgrL7Chief");
                            sqlBulkCopy.ColumnMappings.Add("DXC L8 Name", "PeopleMgrL8Chief");
                            sqlBulkCopy.ColumnMappings.Add("NextlevelMgrName", "NextlevelMgrName");
                            sqlBulkCopy.ColumnMappings.Add("NextlevelMgrEmail", "NextlevelMgrEmail");
                            sqlBulkCopy.ColumnMappings.Add("Source", "Source");
                            sqlBulkCopy.ColumnMappings.Add("Billability Curr Mth", "BillabilityCurrentMth");
                            sqlBulkCopy.ColumnMappings.Add("Billability Previous Mth", "BillabilityPrvMth");
                            sqlBulkCopy.ColumnMappings.Add("Billability Avg Three Mth", "Last6MonthAvgBill");
                            sqlBulkCopy.ColumnMappings.Add("TT 1st Client ID", "TTClientID");
                            sqlBulkCopy.ColumnMappings.Add("TT 1st Client Name", "TTClient");
                            sqlBulkCopy.ColumnMappings.Add("PPMC AllocStatus", "PpmcAllocStatus");
                            sqlBulkCopy.ColumnMappings.Add("Category", "Category");
                            sqlBulkCopy.ColumnMappings.Add("SystemAppear", "SystemAppear");
                            sqlBulkCopy.ColumnMappings.Add("I.ReportMonth", "Month");
                            sqlBulkCopy.ColumnMappings.Add("Skills", "Skills");
                            sqlBulkCopy.ColumnMappings.Add("Technical Competencies", "Technical_Competencies");
                            sqlBulkCopy.ColumnMappings.Add("Capability", "Capability");
                            sqlBulkCopy.ColumnMappings.Add("Exception", "Exception");
                            sqlBulkCopy.BulkCopyTimeout = 6000000;
                            sqlBulkCopy.BatchSize = 1000;
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);

                            con.Close();
                        }
                    }
                }
                return this.Content("Record Inserted Sucessfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "HomeController";
                log.ActionName = "UploadFiles";
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
                worksheet.Cell(currentRow, 12).Value = "GIDC Country";
                worksheet.Cell(currentRow, 13).Value = "Country";
                worksheet.Cell(currentRow, 14).Value = "Revenue Region";
                worksheet.Cell(currentRow, 15).Value = "Service Line";
                worksheet.Cell(currentRow, 16).Value = "Geo Region";
                worksheet.Cell(currentRow, 17).Value = "BSDHI L3";
                worksheet.Cell(currentRow, 18).Value = "GIDC";
                worksheet.Cell(currentRow, 19).Value = "Capability";
                worksheet.Cell(currentRow, 20).Value = "Skills";
                worksheet.Cell(currentRow, 21).Value = "Technical Competencies";
                worksheet.Cell(currentRow, 22).Value = "People Mgr Name";
                worksheet.Cell(currentRow, 23).Value = "People Mgr Email";
                worksheet.Cell(currentRow, 24).Value = "DXC MH L5";
                worksheet.Cell(currentRow, 25).Value = "People Mgr L3 Chief";
                worksheet.Cell(currentRow, 26).Value = "People Mgr L4 Chief";
                worksheet.Cell(currentRow, 27).Value = "People Mgr L5 Chief";
                worksheet.Cell(currentRow, 28).Value = "People Mgr L6 Chief";
                worksheet.Cell(currentRow, 29).Value = "People Mgr L7 Chief";
                worksheet.Cell(currentRow, 30).Value = "People Mgr L8 Chief";
                worksheet.Cell(currentRow, 31).Value = "Next Level Mgr Name";
                worksheet.Cell(currentRow, 32).Value = "Next Level Mgr Email";
                worksheet.Cell(currentRow, 33).Value = "Source";
                worksheet.Cell(currentRow, 34).Value = "Billability Current Mth";
                worksheet.Cell(currentRow, 35).Value = "Billability Previous Mth";
                worksheet.Cell(currentRow, 36).Value = "Billability Avg Three Mth";
                worksheet.Cell(currentRow, 37).Value = "TT Client ID";
                worksheet.Cell(currentRow, 38).Value = "TT Client";
                worksheet.Cell(currentRow, 39).Value = "Exception";
                worksheet.Cell(currentRow, 40).Value = "SystemAppear";
                worksheet.Cell(currentRow, 41).Value = "Category";
                worksheet.Cell(currentRow, 42).Value = "Level 1 DD";
                worksheet.Cell(currentRow, 43).Value = "Level 2 DD";
                worksheet.Cell(currentRow, 44).Value = "Action";
                worksheet.Cell(currentRow, 45).Value = "Action Due Date";
                worksheet.Cell(currentRow, 46).Value = "Action Status";
                worksheet.Cell(currentRow, 47).Value = "Remarks";
                worksheet.Cell(currentRow, 48).Value = "Modified By";
                worksheet.Cell(currentRow, 49).Value = "Modified On";


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
                        worksheet.Cell(currentRow, 12).Value = products.Rows[i]["GIDC Country"].ToString();
                        worksheet.Cell(currentRow, 13).Value = products.Rows[i]["Country"].ToString();
                        worksheet.Cell(currentRow, 14).Value = products.Rows[i]["Revenue Region"].ToString();
                        worksheet.Cell(currentRow, 15).Value = products.Rows[i]["Service Line"].ToString();
                        worksheet.Cell(currentRow, 16).Value = products.Rows[i]["Geo Region"].ToString();
                        worksheet.Cell(currentRow, 17).Value = products.Rows[i]["BSDHI L3"].ToString();
                        worksheet.Cell(currentRow, 18).Value = products.Rows[i]["GIDC"].ToString();
                        worksheet.Cell(currentRow, 19).Value = products.Rows[i]["Capability"].ToString();
                        worksheet.Cell(currentRow, 20).Value = products.Rows[i]["Skills"].ToString();
                        worksheet.Cell(currentRow, 21).Value = products.Rows[i]["Technical Competencies"].ToString();
                        worksheet.Cell(currentRow, 22).Value = products.Rows[i]["People Mgr Name"].ToString();
                        worksheet.Cell(currentRow, 23).Value = products.Rows[i]["People Mgr Email"].ToString();
                        worksheet.Cell(currentRow, 24).Value = products.Rows[i]["DXC MH L5"].ToString();
                        worksheet.Cell(currentRow, 25).Value = products.Rows[i]["People Mgr L3 Chief"].ToString();
                        worksheet.Cell(currentRow, 26).Value = products.Rows[i]["People Mgr L4 Chief"].ToString();
                        worksheet.Cell(currentRow, 27).Value = products.Rows[i]["People Mgr L5 Chief"].ToString();
                        worksheet.Cell(currentRow, 28).Value = products.Rows[i]["People Mgr L6 Chief"].ToString();
                        worksheet.Cell(currentRow, 29).Value = products.Rows[i]["People Mgr L7 Chief"].ToString();
                        worksheet.Cell(currentRow, 30).Value = products.Rows[i]["People Mgr L8 Chief"].ToString();
                        worksheet.Cell(currentRow, 31).Value = products.Rows[i]["Next Level Mgr Name"].ToString();
                        worksheet.Cell(currentRow, 32).Value = products.Rows[i]["Next Level Mgr Email"].ToString();
                        worksheet.Cell(currentRow, 33).Value = products.Rows[i]["Source"].ToString();
                        worksheet.Cell(currentRow, 34).Value = products.Rows[i]["Billability Current Mth"].ToString();
                        worksheet.Cell(currentRow, 35).Value = products.Rows[i]["Billability Previous Mth"].ToString();
                        worksheet.Cell(currentRow, 36).Value = products.Rows[i]["Billability Avg Three Mth"].ToString();
                        worksheet.Cell(currentRow, 37).Value = products.Rows[i]["TT Client ID"].ToString();
                        worksheet.Cell(currentRow, 38).Value = products.Rows[i]["TT Client"].ToString();
                        worksheet.Cell(currentRow, 39).Value = products.Rows[i]["Exception"].ToString();
                        worksheet.Cell(currentRow, 40).Value = products.Rows[i]["SystemAppear"].ToString();
                        worksheet.Cell(currentRow, 41).Value = products.Rows[i]["Category"].ToString();
                        worksheet.Cell(currentRow, 42).Value = products.Rows[i]["Level 1 DD"].ToString();
                        worksheet.Cell(currentRow, 43).Value = products.Rows[i]["Level 2 DD"].ToString();
                        worksheet.Cell(currentRow, 44).Value = products.Rows[i]["Action"].ToString();
                        worksheet.Cell(currentRow, 45).Value = products.Rows[i]["Action Due Date"].ToString();
                        worksheet.Cell(currentRow, 46).Value = products.Rows[i]["Action Status"].ToString();
                        worksheet.Cell(currentRow, 47).Value = products.Rows[i]["Remarks"].ToString();
                        worksheet.Cell(currentRow, 48).Value = products.Rows[i]["Modified By"].ToString();
                        worksheet.Cell(currentRow, 49).Value = products.Rows[i]["Modified On"].ToString();

                    }
                }
                using var stream = new MemoryStream();
                worksheet.Range("A1:AV1").Style.Fill.BackgroundColor = XLColor.Purple;
                worksheet.Range("A1:AV1").Style.Font.FontColor = XLColor.White;
                worksheet.Range("A1:AV1").Style.Font.Bold = true;
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=KARAT Tool - Detailed Report.xlsx");
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
            dtProduct.Columns.AddRange(new DataColumn[49] {

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
             new DataColumn("GIDC Country"),
            new DataColumn("Country"),
            new DataColumn("Revenue Region"),
            new DataColumn("Service Line"),
             new DataColumn("Geo Region"),
            new DataColumn("BSDHI L3"),
            new DataColumn("GIDC"),
            new DataColumn("Capability"),
            new DataColumn("Skills"),
            new DataColumn("Technical Competencies"),
            new DataColumn("People Mgr Name"),
            new DataColumn("People Mgr Email"),
            new DataColumn("DXC MH L5"),
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
                    product.GIDCCountry,
                    product.Country,
                    product.RevenueRegion,
                    product.ServiceLine,
                    product.WorkLocationRegion,
                    product.BSDHIL3,
                     product.GidcFlag,
                    product.Capability,
                    product.Skills,
                    product.Technical_Competencies,
                    product.PeopleMgrName,
                    product.PeopleMgrEmail,
                    product.DXCMHL5,
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
            //string filterString = session.GetFilterString();


            var emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();


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
            string country = "";

            if (emp != null)
            {
                userRole = emp.Role;
                region = emp.Region;
                serviceLine = emp.ServiceLine;
                country = emp.Country;
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

                if (!string.IsNullOrEmpty(country))
                {
                    string[] reg = country.Split(',');
                    if (reg.Count() == 1)
                    {
                        query = query.Where(c => c.GIDCCountry == reg[0].ToString());
                    }
                    else if (reg.Count() == 2)
                    {
                        query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString());
                    }
                    else if (reg.Count() == 3)
                    {
                        query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString() || c.GIDCCountry == reg[2].ToString());
                    }
                    else if (reg.Count() == 4)
                    {
                        query = query.Where(c => c.GIDCCountry == reg[0].ToString() || c.GIDCCountry == reg[1].ToString() || c.GIDCCountry == reg[2].ToString() || c.GIDCCountry == reg[3].ToString());
                    }
                    //else if (reg.Count() == 5)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString());
                    //}
                    //else if (reg.Count() == 6)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString());
                    //}
                    //else if (reg.Count() == 7)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[6].ToString());
                    //}
                    //else if (reg.Count() == 8)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString());
                    //}
                    //else if (reg.Count() == 9)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString());
                    //}
                    //else if (reg.Count() == 10)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString());
                    //}
                    //else if (reg.Count() == 11)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString());
                    //}
                    //else if (reg.Count() == 12)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString());
                    //}
                    //else if (reg.Count() == 13)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString());
                    //}
                    //else if (reg.Count() == 14)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString() || c.Country == reg[13].ToString());
                    //}
                    //else if (reg.Count() == 15)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString() || c.Country == reg[13].ToString() || c.Country == reg[14].ToString());
                    //}
                    //else if (reg.Count() == 16)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString() || c.Country == reg[13].ToString() || c.Country == reg[14].ToString()
                    //    || c.Country == reg[15].ToString());
                    //}
                    //else if (reg.Count() == 17)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString() || c.Country == reg[13].ToString() || c.Country == reg[14].ToString()
                    //    || c.Country == reg[15].ToString() || c.Country == reg[16].ToString());
                    //}
                    //else if (reg.Count() == 18)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString() || c.Country == reg[13].ToString() || c.Country == reg[14].ToString()
                    //    || c.Country == reg[15].ToString() || c.Country == reg[16].ToString() || c.Country == reg[17].ToString());
                    //}
                    //else if (reg.Count() == 19)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString() || c.Country == reg[13].ToString() || c.Country == reg[14].ToString()
                    //    || c.Country == reg[15].ToString() || c.Country == reg[16].ToString() || c.Country == reg[17].ToString() || c.Country == reg[18].ToString());
                    //}
                    //else if (reg.Count() == 20)
                    //{
                    //    query = query.Where(c => c.Country == reg[0].ToString() || c.Country == reg[1].ToString() || c.Country == reg[2].ToString() || c.Country == reg[3].ToString()
                    //    || c.Country == reg[4].ToString() || c.Country == reg[5].ToString() || c.Country == reg[7].ToString() || c.Country == reg[8].ToString() || c.Country == reg[9].ToString()
                    //    || c.Country == reg[10].ToString() || c.Country == reg[11].ToString() || c.Country == reg[12].ToString() || c.Country == reg[13].ToString() || c.Country == reg[14].ToString()
                    //    || c.Country == reg[15].ToString() || c.Country == reg[16].ToString() || c.Country == reg[17].ToString() || c.Country == reg[18].ToString() || c.Country == reg[19].ToString());
                    //}
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