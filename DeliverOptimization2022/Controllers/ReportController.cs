using ClosedXML.Excel;
using DeliverOptimization2022.Models;
using DeliverOptimization2022.Session;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DeliverOptimization2022.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private IConfiguration Configuration;
        public ReportController(ApplicationDbContext dbContext, IConfiguration _configuration)
        {
            Configuration = _configuration;
            _dbContext = dbContext;
        }
        public IActionResult ReportIndex()
        {
            return View();
        }


        [HttpPost]
        public JsonResult DonwloadExcel(string ddlVal)
        {
            var session = new UserSession(HttpContext.Session);
            string useremail = session.GetUseremail();
            string userRole = session.GetManager();

            if (ddlVal == "KARAT")
            {
                var emp = _dbContext.EmployeeRole.Where(c => c.Email == useremail).FirstOrDefault();

                userRole = "";
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

                    }
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

                DataTable dt = new DataTable("KARAT");
                dt.Columns.AddRange(new DataColumn[49] {
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
            new DataColumn("Modified On")
            });
                foreach (var product in data)
                {
                    dt.Rows.Add(
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

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return Json(Convert.ToBase64String(stream.ToArray(), 0, stream.ToArray().Length));
                    }
                }

            }
            else if (ddlVal == "Rotate High Tenure")
            {
                List<tbl_EmployeeRotation> newList = session.GetEmployeeRotation();
                //List<tbl_EmployeeRotation> newList = _dbContext.tbl_EmployeeRotation.ToList();
                if (userRole == "SAdmin" || userRole == "Admin")
                {
                    newList = newList.ToList();
                }
                else
                {
                    newList = newList.Where(c => c.ManagerEmail == useremail).ToList();
                }
                DataTable dt = new DataTable("KARAT");
                dt.Columns.AddRange(new DataColumn[22] {
                new DataColumn("EmployeeID"),
                new DataColumn("Employee Name"),
                new DataColumn("ManagerName"),
                new DataColumn("ManagerEmail"),
                new DataColumn("NeedForRespons"),
                new DataColumn("Position"),
                new DataColumn("PositionName"),
                new DataColumn("Account"),
                new DataColumn("PositionEndDate"),
                new DataColumn("MonthsInPosition"),
                new DataColumn("CanEmpReleasedIn6Month"),
                new DataColumn("EarliestReleasedDate"),
                new DataColumn("BackfillPlan"),
                new DataColumn("SpecifySuccessor"),
                new DataColumn("ExpectedKTDuration"),
                new DataColumn("RiskOfServiceDelivery"),
                new DataColumn("ConfirmCurrentAccount"),
                new DataColumn("AccountContactForInterlock"),
                new DataColumn("EmployeeFlightRisk"),
                new DataColumn("Comment"),
                new DataColumn("ModifiedBy"),
                new DataColumn("ModifiedOn")

            });
                foreach (var product in newList)
                {
                    dt.Rows.Add(
                        product.EmployeeID,
                        product.EmployeeName,
                        product.ManagerName,
                        product.ManagerEmail,
                        product.NeedForRespons,
                        product.Position,
                        product.PositionName,
                        product.Account,
                        product.PositionEndDate,
                        product.MonthsInPosition,
                        product.CanEmpReleasedIn6Month,
                        product.EarliestReleasedDate,
                        product.BackfillPlan,
                        product.SpecifySuccessor,
                        product.ExpectedKTDuration,
                        product.RiskOfServiceDelivery,
                        product.ConfirmCurrentAccount,
                         product.AccountContactForInterlock,
                        product.EmployeeFlightRisk,
                        product.Comment,
                        product.ModifiedBy,
                        product.ModifiedOn

                );
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return Json(Convert.ToBase64String(stream.ToArray(), 0, stream.ToArray().Length));
                    }
                }

            }
            //else if (ddlVal == "Migration")
            //{
            //    List<tbl_EmployeeMigration> newList = session.GetEmployeeMigration();

            //    if (userRole == "SAdmin" || userRole == "Admin")
            //    {
            //        newList = newList.ToList();
            //    }
            //    else
            //    {
            //        newList = newList.Where(c => c.ManagerEmail == useremail).ToList();
            //    }
            //    DataTable dt = new DataTable("KARAT");
            //    dt.Columns.AddRange(new DataColumn[27] {
            //    new DataColumn("Employee ID"),
            //    new DataColumn("Employee Name"),
            //     new DataColumn("Manager Name"),
            //      new DataColumn("Manager Email"),
            //    new DataColumn("Need For resources With Skills Of this Employee"),
            //    new DataColumn("Position ID"),
            //    new DataColumn("Position Name"),
            //    new DataColumn("Account"),
            //    new DataColumn("Position End Date"),
            //    new DataColumn("Offering [BSDHI L3]"),
            //    new DataColumn("Country"),
            //    new DataColumn("ClientID"),
            //    new DataColumn("PPMC Location Alignment"),
            //    new DataColumn("PPMC Spoken Language"),
            //    new DataColumn("Billing Type PPMC"),
            //    new DataColumn("Can Work Be Migrated To GDN"),
            //    new DataColumn("Constraint"),
            //    new DataColumn("Sub Constraint"),
            //    new DataColumn("Earliest Release dDate"),
            //    new DataColumn("FTE"),
            //    new DataColumn("Migrate To"),
            //    new DataColumn("Risk To Delivery"),
            //      new DataColumn("Confirm Current Account"),
            //        new DataColumn("Account Contact"),
            //    new DataColumn("Comment"),
            //       new DataColumn("Modified By"),
            //    new DataColumn("Modified On")

            //});
            //    foreach (var product in newList)
            //    {
            //        dt.Rows.Add(
            //            product.EmployeeID,
            //            product.EmployeeName,
            //            product.ManagerName,
            //            product.ManagerEmail,
            //            product.NeedForresourcesWithSkillsOfthisEmployee,
            //            product.Position,
            //            product.PositioinName,
            //            product.Account,
            //            product.PositionEndDate,
            //            product.OfferingBSDHIL3,
            //            product.Country,
            //            product.ClientID,
            //            product.ReviewOnSitePPMC,
            //            product.ReviewLanguagePPMC,
            //            product.BillingTypePPMC,
            //            product.CanWorkBeMigratedToGDN,
            //            product.CanWorkBeMigratedContraint,
            //             product.SubConstraint,
            //            product.EarliestReleasedDate,
            //              product.FTE,
            //            product.MigrateTo,
            //            product.RiskToDelivery,
            //               product.ConfirmCurrentAccount,
            //                product.AccountContact,
            //            product.Comment,
            //            product.ModifiedBy,
            //            product.ModifiedOn

            //    );
            //    }
            //    using (XLWorkbook wb = new XLWorkbook())
            //    {
            //        wb.Worksheets.Add(dt);
            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            wb.SaveAs(stream);
            //            return Json(Convert.ToBase64String(stream.ToArray(), 0, stream.ToArray().Length));
            //        }
            //    }
            //}
            return Json("No Content");


        }




        //public IActionResult GetReportData(string reporttype)
        //{
        //    try
        //    {
        //        var session = new UserSession(HttpContext.Session);
        //        string currentMonthYear = "";
        //        string currentMonthYear1 = "";
        //        string currentMonthYear2 = "";
        //        string parm = "";
        //        int day = (int)DateTime.Now.Day;
        //        if (day > 20)
        //        {
        //            DateTime curMon = DateTime.Now.AddMonths(-1);
        //            string month = curMon.ToString("MMM");
        //            string year = curMon.ToString("yy");
        //            currentMonthYear = month + "-" + year;

        //            DateTime curMonth1 = DateTime.Now.AddMonths(-2);
        //            string month1 = curMonth1.ToString("MMM");
        //            string year1 = curMon.ToString("yy");
        //            currentMonthYear1 = month1 + "-" + year1;

        //            DateTime curMonth2 = DateTime.Now.AddMonths(-3);
        //            string month2 = curMonth2.ToString("MMM");
        //            string year2 = curMon.ToString("yy");
        //            currentMonthYear2 = month2 + "-" + year2;

        //            session.setMonth(currentMonthYear, currentMonthYear1, currentMonthYear2, "", "", "");
        //            parm = currentMonthYear2 + "," + currentMonthYear1 + "," + currentMonthYear;
        //        }
        //        else
        //        {
        //            DateTime curMon = DateTime.Now.AddMonths(-2);
        //            string month = curMon.ToString("MMM");
        //            string year = curMon.ToString("yy");
        //            currentMonthYear = month + "-" + year;

        //            DateTime curMonth1 = DateTime.Now.AddMonths(-3);
        //            string month1 = curMonth1.ToString("MMM");
        //            string year1 = curMon.ToString("yy");
        //            currentMonthYear1 = month1 + "-" + year1;

        //            DateTime curMonth2 = DateTime.Now.AddMonths(-4);
        //            string month2 = curMonth2.ToString("MMM");
        //            string year2 = curMon.ToString("yy");
        //            currentMonthYear2 = month2 + "-" + year2;

        //            session.setMonth(currentMonthYear, currentMonthYear1, currentMonthYear2, "", "", "");
        //            parm = currentMonthYear2 + "," + currentMonthYear1 + "," + currentMonthYear;
        //        }

        //        string Role = session.GetManager();
        //        string Region = session.GetRegion();
        //        string ServiceLine = session.GetServiceLine();
        //        if (string.IsNullOrEmpty(Region))
        //            Region = "";
        //        if (string.IsNullOrEmpty(ServiceLine))
        //            ServiceLine = "";

        //        string SqlconString = this.Configuration.GetConnectionString("DefaultConnection");
        //        using (SqlConnection con = new SqlConnection(SqlconString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("sp_insert_InitialData", con))
        //            {
        //                cmd.CommandTimeout = 0;
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = parm;
        //                cmd.Parameters.Add("@Region", SqlDbType.VarChar).Value = Region;
        //                cmd.Parameters.Add("@ServiceLine", SqlDbType.VarChar).Value = ServiceLine;
        //                con.Open();
        //                cmd.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }
        //        using (SqlConnection con = new SqlConnection(SqlconString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("sp_tbl3MonthPivotFinalInsert", con))
        //            {
        //                cmd.CommandTimeout = 0;
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                //cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = parm;
        //                con.Open();
        //                cmd.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }

        //        var report = _dbContext.tbl_Insert3MonthPivot.ToList();
        //        List<tblViewUpdate3Month> view3Month1 = new List<tblViewUpdate3Month>();
        //        foreach (var v in report)
        //        {
        //            tblViewUpdate3Month r = new tblViewUpdate3Month();
        //            if (Convert.ToInt32(v.CurrentMonthMinus1) > 0 && Convert.ToInt32(v.CurrentMonthMinus2) > 0 && Convert.ToInt32(v.CurrentMonthMinus3) > 0)
        //            {
        //                r.PeopleManagerEmail = v.PeopleManagerEmail;
        //                r.PeopleManagerName = v.PeopleManagerName;
        //                r.CurrentMonthMinus2 = v.CurrentMonthMinus3;
        //                r.CurrentMonthMinus1 = v.CurrentMonthMinus2;
        //                r.CurrentMonth = v.CurrentMonthMinus1;
        //                view3Month1.Add(r);
        //            }

        //        }
        //        DownloadReport(view3Month1);
        //        //using (SqlConnection con = new SqlConnection(SqlconString))
        //        //{
        //        //    using (SqlCommand cmd = new SqlCommand("sp_Initial_Insert", con))
        //        //    {
        //        //        cmd.CommandTimeout = 0;
        //        //        cmd.CommandType = CommandType.StoredProcedure;
        //        //        cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = parm;
        //        //        con.Open();
        //        //        cmd.ExecuteNonQuery();
        //        //        con.Close();
        //        //    }
        //        //}
        //        //List<tblViewUpdate3Month> view3Month = new List<tblViewUpdate3Month>();
        //        //List<tbl_Update3Month> tblUpdate3Month = _dbContext.tbl_Update3Month.ToList();

        //        //if (Role == "Admin")
        //        //{
        //        //    if (!string.IsNullOrEmpty(Region))
        //        //    {
        //        //        tblUpdate3Month = tblUpdate3Month.Where(c => c.RevenueRegion == Region).ToList();
        //        //        //var dd = tblUpdate3Month.Where(c => c.PeopleManager == "Jim Naumovski").ToList();
        //        //    }
        //        //    if (!string.IsNullOrEmpty(ServiceLine))
        //        //    {
        //        //        tblUpdate3Month = tblUpdate3Month.Where(c => c.ServiceLine == ServiceLine).ToList();
        //        //    }
        //        //}

        //        //var pplMgrName = tblUpdate3Month.Select(c => c.PeopleManager).Distinct().ToList();
        //        //foreach (var v in pplMgrName)
        //        //{
        //        //    tblViewUpdate3Month tbl = new tblViewUpdate3Month();
        //        //    tbl.PeopleManagerName = v;
        //        //    tbl.PeopleManagerEmail = tblUpdate3Month.Where(c => c.PeopleManager == v).Select(c => c.PeopleManagerEmail).FirstOrDefault();
        //        //    string curMMinus2 = tblUpdate3Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear2).Sum(c => c.FullCount).ToString();
        //        //    tbl.CurrentMonthMinus2 = curMMinus2;
        //        //    string curMMinus1 = tblUpdate3Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear1).Sum(c => c.FullCount).ToString();
        //        //    tbl.CurrentMonthMinus1 = curMMinus1;
        //        //    string curMonth = tblUpdate3Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear).Sum(c => c.FullCount).ToString();
        //        //    tbl.CurrentMonth = curMonth;

        //        //    string updCount2 = tblUpdate3Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear2).Select(c => c.UpdatedCount).FirstOrDefault();
        //        //    string updCount1 = tblUpdate3Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear1).Select(c => c.UpdatedCount).FirstOrDefault();
        //        //    string updCount = tblUpdate3Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear).Select(c => c.UpdatedCount).FirstOrDefault();

        //        //    int updatedCount = Convert.ToInt32(updCount2) + Convert.ToInt32(updCount1) + Convert.ToInt32(updCount);
        //        //    int cmonth = Convert.ToInt32(curMonth);
        //        //    int cmonth1 = Convert.ToInt32(curMMinus1);
        //        //    int cmonth2 = Convert.ToInt32(curMMinus2);
        //        //    tbl.UpdatedCount = Convert.ToString(updatedCount);
        //        //    //if (updatedCount == 0 && cmonth > 0 && cmonth1 > 0 && cmonth2 > 0)
        //        //    //{
        //        //    //    tbl.UpdatedCount = Convert.ToString(updatedCount);
        //        //    //    view3Month.Add(tbl);
        //        //    //}

        //        //    if (cmonth > 0 && cmonth1 > 0 && cmonth2 > 0)
        //        //    {
        //        //        tbl.UpdatedCount = Convert.ToString(updatedCount);
        //        //        view3Month.Add(tbl);
        //        //    }
        //        //}
        //        //Insert3MonthData(view3Month);

        //        //List<tbl_Update3MonthDetails> tblUpdate3MonthDetails = _dbContext.tbl_Update3MonthDetails.ToList();
        //        //if (Role == "Admin")
        //        //{
        //        //    if (!string.IsNullOrEmpty(Region))
        //        //    {
        //        //        tblUpdate3MonthDetails = tblUpdate3MonthDetails.Where(c => c.RevenueRegion == Region).ToList();
        //        //    }
        //        //    if (!string.IsNullOrEmpty(ServiceLine))
        //        //    {
        //        //        tblUpdate3MonthDetails = tblUpdate3MonthDetails.Where(c => c.ServiceLine == ServiceLine).ToList();
        //        //    }
        //        //}
        //        //List<tbl_Update3MonthDetails_Final> varRepo = new List<tbl_Update3MonthDetails_Final>();

        //        //foreach (var v in tblUpdate3MonthDetails)
        //        //{
        //        //    int count = tblUpdate3MonthDetails.Where(c => c.EmployeeName == v.EmployeeName && c.Month==currentMonthYear).Count();
        //        //    int count1 = tblUpdate3MonthDetails.Where(c => c.EmployeeName == v.EmployeeName && c.Month == currentMonthYear1).Count();
        //        //    int count2 = tblUpdate3MonthDetails.Where(c => c.EmployeeName == v.EmployeeName && c.Month == currentMonthYear2).Count();
        //        //    int Newcount = count + count1 + count2;
        //        //    if (Newcount>2)
        //        //    {
        //        //        tbl_Update3MonthDetails_Final tbl = new tbl_Update3MonthDetails_Final();
        //        //        tbl.EID = v.EID;
        //        //        tbl.EmployeeName = v.EmployeeName;
        //        //        tbl.PeopleMgrName = v.PeopleMgrName;
        //        //        tbl.PeopleMgrEmail = v.PeopleMgrEmail;
        //        //        tbl.RevenueRegion = v.RevenueRegion;
        //        //        tbl.ServiceLine = v.ServiceLine;
        //        //        tbl.Country = v.Country;
        //        //        tbl.DXCMHL5 = v.DXCMHL5;
        //        //        tbl.PeopleManagerL3Cheif = v.PeopleManagerL3Cheif;
        //        //        tbl.PeopleManagerL4Cheif = v.PeopleManagerL4Cheif;
        //        //        tbl.PeopleManagerL5Cheif = v.PeopleManagerL5Cheif;
        //        //        tbl.BillabilityCurrentMth = v.BillabilityCurrentMth;
        //        //        tbl.Month = v.Month;
        //        //        _dbContext.tbl_Update3MonthDetails_Final.Add(tbl);
        //        //    }
        //        //}
        //        //_dbContext.SaveChanges();

        //        //Upate_tbl_Update3MonthDetails(varRepo);



        //        return View(view3Month1);
        //    }
        //    catch 
        //    {
        //        return View();
        //    }

        //}



        //private void Insert3MonthData(List<tblViewUpdate3Month> view3Month)
        //{
        //    foreach (var v in view3Month)
        //    {
        //        tbl_Insert3MonthPivot tbl = new tbl_Insert3MonthPivot();
        //        tbl.PeopleManagerName = v.PeopleManagerName;
        //        tbl.PeopleManagerEmail = v.PeopleManagerEmail;
        //        tbl.CurrentMonthMinus3 = v.CurrentMonthMinus2;
        //        tbl.CurrentMonthMinus2 = v.CurrentMonthMinus1;
        //        tbl.CurrentMonthMinus1 = v.CurrentMonth;
        //        _dbContext.tbl_Insert3MonthPivot.Add(tbl);
        //    }
        //    _dbContext.SaveChanges();
        //}
        public IActionResult Get3MonthDetaiReport()
        {
            var session = new UserSession(HttpContext.Session);
            string currentMonthYear = "";
            string currentMonthYear1 = "";
            string currentMonthYear2 = "";
            string parm = "";
            int day = (int)DateTime.Now.Day;
            if (day > 20)
            {
                DateTime curMon = DateTime.Now.AddMonths(-1);
                string month = curMon.ToString("MMM");
                string year = curMon.ToString("yy");
                currentMonthYear = month + "-" + year;

                DateTime curMonth1 = DateTime.Now.AddMonths(-2);
                string month1 = curMonth1.ToString("MMM");
                string year1 = curMon.ToString("yy");
                currentMonthYear1 = month1 + "-" + year1;

                DateTime curMonth2 = DateTime.Now.AddMonths(-3);
                string month2 = curMonth2.ToString("MMM");
                string year2 = curMon.ToString("yy");
                currentMonthYear2 = month2 + "-" + year2;


                //session.setMonth(currentMonthYear, currentMonthYear1, currentMonthYear2, "", "", "");
                parm = currentMonthYear2 + "," + currentMonthYear1 + "," + currentMonthYear;
            }
            else
            {
                DateTime curMon = DateTime.Now.AddMonths(-2);
                string month = curMon.ToString("MMM");
                string year = curMon.ToString("yy");
                currentMonthYear = month + "-" + year;

                DateTime curMonth1 = DateTime.Now.AddMonths(-3);
                string month1 = curMonth1.ToString("MMM");
                string year1 = curMon.ToString("yy");
                currentMonthYear1 = month1 + "-" + year1;

                DateTime curMonth2 = DateTime.Now.AddMonths(-4);
                string month2 = curMonth2.ToString("MMM");
                string year2 = curMon.ToString("yy");
                currentMonthYear2 = month2 + "-" + year2;


                //session.setMonth(currentMonthYear, currentMonthYear1, currentMonthYear2, "", "", "");
                parm = currentMonthYear2 + "," + currentMonthYear1 + "," + currentMonthYear;
            }


            //string SqlconString = this.Configuration.GetConnectionString("DefaultConnection");
            //using (SqlConnection con = new SqlConnection(SqlconString))
            //{
            //    using (SqlCommand cmd = new SqlCommand("spUpdate3MonthDetails", con))
            //    {
            //        cmd.CommandTimeout = 0;
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = parm;
            //        con.Open();
            //        cmd.ExecuteNonQuery();
            //        con.Close();
            //    }
            //}
            string Role = session.GetManager();
            string Region = session.GetRegion();
            string ServiceLine = session.GetServiceLine();

            //List<tbl_Update3MonthDetails> tblUpdate3Month = _dbContext.tbl_Update3MonthDetails.ToList();
            //if (Role == "Admin")
            //{
            //    if (!string.IsNullOrEmpty(Region))
            //    {
            //        tblUpdate3Month = tblUpdate3Month.Where(c => c.RevenueRegion == Region).ToList();
            //    }
            //    if (!string.IsNullOrEmpty(ServiceLine))
            //    {
            //        tblUpdate3Month = tblUpdate3Month.Where(c => c.ServiceLine == ServiceLine).ToList();
            //    }
            //}
            //List<tbl_Update3MonthDetails> tbl = new List<tbl_Update3MonthDetails>();
            //foreach(var v in tblUpdate3Month)
            //{
            //    int count = tblUpdate3Month.Where(c => c.EmployeeName == v.EmployeeName).Count();
            //    if (count > 2)
            //    {
            //        tbl.Add(v);
            //    }
            //}
            List<tbl_Update3MonthDetails> tblUpdate3Month = _dbContext.tbl_Update3MonthDetails.ToList();

            if (Role == "Admin")
            {
                if (!string.IsNullOrEmpty(Region))
                {
                    tblUpdate3Month = tblUpdate3Month.Where(c => c.RevenueRegion == Region).ToList();
                }
                if (!string.IsNullOrEmpty(ServiceLine))
                {
                    tblUpdate3Month = tblUpdate3Month.Where(c => c.ServiceLine == ServiceLine).ToList();
                }
            }
            List<tbl_Update3MonthDetails> rpt = new List<tbl_Update3MonthDetails>();
            foreach (var v in tblUpdate3Month)
            {
                int count = tblUpdate3Month.Where(c => c.PeopleMgrEmail == v.PeopleMgrEmail && c.Month == currentMonthYear).Count();
                int count1 = tblUpdate3Month.Where(c => c.PeopleMgrEmail == v.PeopleMgrEmail && c.Month == currentMonthYear1).Count();
                int count2 = tblUpdate3Month.Where(c => c.PeopleMgrEmail == v.PeopleMgrEmail && c.Month == currentMonthYear2).Count();
                if (count > 0 && count1 > 0 && count2 > 0)
                {
                    tbl_Update3MonthDetails tbl = new tbl_Update3MonthDetails();
                    tbl.EID = v.EID;
                    tbl.EmployeeName = v.EmployeeName;
                    tbl.PeopleMgrName = v.PeopleMgrName;
                    tbl.PeopleMgrEmail = v.PeopleMgrEmail;
                    tbl.RevenueRegion = v.RevenueRegion;
                    tbl.ServiceLine = v.ServiceLine;
                    tbl.Country = v.Country;
                    tbl.DXCMHL5 = v.DXCMHL5;
                    tbl.PeopleManagerL3Cheif = v.PeopleManagerL3Cheif;
                    tbl.PeopleManagerL4Cheif = v.PeopleManagerL4Cheif;
                    tbl.PeopleManagerL5Cheif = v.PeopleManagerL5Cheif;
                    tbl.BillabilityCurrentMth = v.BillabilityCurrentMth;
                    tbl.Month = v.Month;
                    rpt.Add(tbl);
                }
            }

            DownLoad3MonthDetails(rpt);
            return RedirectToAction("ReportIndex");
        }

        private void DownLoad3MonthDetails(List<tbl_Update3MonthDetails> tbl)
        {
            var data = new DataTable();
            data = Get3MonthDetails(tbl);
            ExportToExceltbl(data);
        }

        private void ExportToExceltbl(DataTable data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("EmployeeDetails");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "EID";
                worksheet.Cell(currentRow, 2).Value = "Employee Name";
                worksheet.Cell(currentRow, 3).Value = "People Manager Name";
                worksheet.Cell(currentRow, 4).Value = "People Manager Email";
                worksheet.Cell(currentRow, 5).Value = "Revenue Region";
                worksheet.Cell(currentRow, 6).Value = "Service Line";
                worksheet.Cell(currentRow, 7).Value = "Country";
                worksheet.Cell(currentRow, 8).Value = "DXCMHL5";
                worksheet.Cell(currentRow, 9).Value = "People Manager L3 Cheif";
                worksheet.Cell(currentRow, 10).Value = "People Manager L4 Cheif";
                worksheet.Cell(currentRow, 11).Value = "People Manager L5 Cheif";
                worksheet.Cell(currentRow, 12).Value = "Billability Current Month";
                worksheet.Cell(currentRow, 13).Value = "Month";

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = (XLCellValue)data.Rows[i]["EID"];
                    worksheet.Cell(currentRow, 2).Value = (XLCellValue)data.Rows[i]["Employee Name"];
                    worksheet.Cell(currentRow, 3).Value = (XLCellValue)data.Rows[i]["People Manager Name"];
                    worksheet.Cell(currentRow, 4).Value = (XLCellValue)data.Rows[i]["People Manager Email"];
                    worksheet.Cell(currentRow, 5).Value = (XLCellValue)data.Rows[i]["Revenue Region"];
                    worksheet.Cell(currentRow, 6).Value = (XLCellValue)data.Rows[i]["Service Line"];
                    worksheet.Cell(currentRow, 7).Value = (XLCellValue)data.Rows[i]["Country"];
                    worksheet.Cell(currentRow, 8).Value = (XLCellValue)data.Rows[i]["DXCMHL5"];
                    worksheet.Cell(currentRow, 9).Value = (XLCellValue)data.Rows[i]["People Manager L3 Cheif"];
                    worksheet.Cell(currentRow, 10).Value = (XLCellValue)data.Rows[i]["People Manager L4 Cheif"];
                    worksheet.Cell(currentRow, 11).Value = (XLCellValue)data.Rows[i]["People Manager L5 Cheif"];
                    worksheet.Cell(currentRow, 12).Value = (XLCellValue)data.Rows[i]["Billability Current Month"];
                    worksheet.Cell(currentRow, 13).Value = (XLCellValue)data.Rows[i]["Month"];
                }
                using var stream = new MemoryStream();
                worksheet.Range("A1:M1").Style.Fill.BackgroundColor = XLColor.Purple;
                worksheet.Range("A1:M1").Style.Font.FontColor = XLColor.White;
                worksheet.Range("A1:M1").Style.Font.Bold = true;
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=ThreeMonthDetailReport.xlsx");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                Response.Body.Flush();
            }
        }
        private DataTable Get3MonthDetails(List<tbl_Update3MonthDetails> tbl)
        {
            DataTable dtProduct = new DataTable("Report");
            dtProduct.Columns.AddRange(new DataColumn[13] {

            new DataColumn("EID"),
            new DataColumn("Employee Name"),
             new DataColumn("People Manager Name"),
              new DataColumn("People Manager Email"),
             new DataColumn("Revenue Region"),
              new DataColumn("Service Line"),
             new DataColumn("Country"),
              new DataColumn("DXCMHL5"),
             new DataColumn("People Manager L3 Cheif"),
             new DataColumn("People Manager L4 Cheif"),
             new DataColumn("People Manager L5 Cheif"),
             new DataColumn("Billability Current Month"),
            new DataColumn("Month"),
            });
            foreach (var v in tbl)
            {
                dtProduct.Rows.Add(
                    v.EID,
                    v.EmployeeName,
                    v.PeopleMgrName,
                    v.PeopleMgrEmail,
                    v.RevenueRegion,
                    v.ServiceLine,
                    v.Country,
                    v.DXCMHL5,
                    v.PeopleManagerL3Cheif,
                    v.PeopleManagerL4Cheif,
                     v.PeopleManagerL5Cheif,
                    v.BillabilityCurrentMth,
                    "'" + v.Month
                    );
            }
            return dtProduct;
        }

        //public IActionResult GetReportData6Month(string reporttype)
        //{
        //    try
        //    {
        //        DateTime curMon = DateTime.Now.AddMonths(-1);
        //        string month = curMon.ToString("MMM");
        //        string year = curMon.ToString("yy");
        //        string currentMonthYear = month + "-" + year;

        //        DateTime curMonth1 = DateTime.Now.AddMonths(-2);
        //        string month1 = curMonth1.ToString("MMM");
        //        string year1 = curMonth1.ToString("yy");
        //        string currentMonthYear1 = month1 + "-" + year1;

        //        DateTime curMonth2 = DateTime.Now.AddMonths(-3);
        //        string month2 = curMonth2.ToString("MMM");
        //        string year2 = curMonth2.ToString("yy");
        //        string currentMonthYear2 = month2 + "-" + year2;

        //        DateTime curMonth3 = DateTime.Now.AddMonths(-4);
        //        string month3 = curMonth3.ToString("MMM");
        //        string year3 = curMonth3.ToString("yy");
        //        string currentMonthYear3 = month3 + "-" + year3;

        //        DateTime curMonth4 = DateTime.Now.AddMonths(-5);
        //        string month4 = curMonth4.ToString("MMM");
        //        string year4 = curMonth4.ToString("yy");
        //        string currentMonthYear4 = month4 + "-" + year4;

        //        DateTime curMonth5 = DateTime.Now.AddMonths(-6);
        //        string month5 = curMonth5.ToString("MMM");
        //        string year5 = curMonth5.ToString("yy");
        //        string currentMonthYear5 = month5 + "-" + year5;



        //        var session = new UserSession(HttpContext.Session);
        //        session.setMonth(currentMonthYear, currentMonthYear1, currentMonthYear2, currentMonthYear3, currentMonthYear4, currentMonthYear5);
        //        string parm = currentMonthYear5 + "," + currentMonthYear4 + "," + currentMonthYear3 + "," + currentMonthYear2 + "," + currentMonthYear1 + "," + currentMonthYear;

        //        string SqlconString = this.Configuration.GetConnectionString("DefaultConnection");
        //        using (SqlConnection con = new SqlConnection(SqlconString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("spGetUpdate6Month", con))
        //            {
        //                cmd.CommandTimeout = 0;
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = parm;
        //                con.Open();
        //                cmd.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }
        //        List<tblViewUpdate6Month> view6Month = new List<tblViewUpdate6Month>();
        //        List<tbl_Update6Month> tblUpdate6Month = _dbContext.tbl_Update6Month.ToList();
        //        var pplMgrName = tblUpdate6Month.Select(c => c.PeopleManager).Distinct().ToList();
        //        foreach (var v in pplMgrName)
        //        {
        //            tblViewUpdate6Month tbl = new tblViewUpdate6Month();
        //            tbl.PeopleManagerName = v;

        //            string curMMinus5 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear5).Select(c => c.FullCount).FirstOrDefault();
        //            tbl.CurrentMonthMinus5 = curMMinus5;

        //            string curMMinus4 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear4).Select(c => c.FullCount).FirstOrDefault();
        //            tbl.CurrentMonthMinus4 = curMMinus4;

        //            string curMMinus3 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear3).Select(c => c.FullCount).FirstOrDefault();
        //            tbl.CurrentMonthMinus3 = curMMinus3;

        //            string curMMinus2 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear2).Select(c => c.FullCount).FirstOrDefault();
        //            tbl.CurrentMonthMinus2 = curMMinus2;

        //            string curMMinus1 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear1).Select(c => c.FullCount).FirstOrDefault();
        //            tbl.CurrentMonthMinus1 = curMMinus1;

        //            string curMonth = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear).Select(c => c.FullCount).FirstOrDefault();
        //            tbl.CurrentMonth = curMonth;

        //            string updCount5 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear5).Select(c => c.UpdatedCount).FirstOrDefault();
        //            string updCount4 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear4).Select(c => c.UpdatedCount).FirstOrDefault();
        //            string updCount3 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear3).Select(c => c.UpdatedCount).FirstOrDefault();

        //            string updCount2 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear2).Select(c => c.UpdatedCount).FirstOrDefault();
        //            string updCount1 = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear1).Select(c => c.UpdatedCount).FirstOrDefault();
        //            string updCount = tblUpdate6Month.Where(c => c.PeopleManager == v && c.Month == currentMonthYear).Select(c => c.UpdatedCount).FirstOrDefault();

        //            int updatedCount = Convert.ToInt32(updCount5) + Convert.ToInt32(updCount4) + Convert.ToInt32(updCount3) + Convert.ToInt32(updCount2)
        //                + Convert.ToInt32(updCount1) + Convert.ToInt32(updCount);
        //            int cmonth = Convert.ToInt32(curMonth);
        //            int cmonth1 = Convert.ToInt32(curMMinus1);
        //            int cmonth2 = Convert.ToInt32(curMMinus2);
        //            int cmonth3 = Convert.ToInt32(curMMinus3);
        //            int cmonth4 = Convert.ToInt32(curMMinus4);
        //            int cmonth5 = Convert.ToInt32(curMMinus5);
        //            tbl.UpdatedCount = Convert.ToString(updatedCount);
        //            if (updatedCount == 0 && cmonth > 0 && cmonth1 > 0 && cmonth2 > 0 && cmonth3 > 0 && cmonth4 > 0 && cmonth5 > 0)
        //            {
        //                tbl.UpdatedCount = Convert.ToString(updatedCount);
        //                view6Month.Add(tbl);
        //            }
        //        }
        //        Download6MonthReport(view6Month);
        //        return RedirectToAction("ReportIndex");
        //    }
        //    catch 
        //    {
        //        return View();
        //    }

        //}

        //private void Download6MonthReport(List<tblViewUpdate6Month> view6Month)
        //{
        //    var data = new DataTable();
        //    data = Get6MonthEmployeeDetails(view6Month);
        //    ExportToExcel6MonthReport(data);
        //}

        //private void ExportToExcel6MonthReport(DataTable data)
        //{
        //    var session = new UserSession(HttpContext.Session);
        //    string currentMonth = session.GetCurrentMonth();
        //    string currentMonth1 = session.GetCurrentMonth1();
        //    string currentMonth2 = session.GetCurrentMonth2();
        //    string currentMonth3 = session.GetCurrentMonth3();
        //    string currentMonth4 = session.GetCurrentMonth4();
        //    string currentMonth5 = session.GetCurrentMonth5();

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("EmployeeDetails");
        //        var currentRow = 1;
        //        worksheet.Cell(currentRow, 1).Value = "People Manager Name";
        //        worksheet.Cell(currentRow, 2).Value = Convert.ToString(currentMonth5);
        //        worksheet.Cell(currentRow, 3).Value = Convert.ToString(currentMonth4);
        //        worksheet.Cell(currentRow, 4).Value = Convert.ToString(currentMonth3);
        //        worksheet.Cell(currentRow, 5).Value = Convert.ToString(currentMonth2);
        //        worksheet.Cell(currentRow, 6).Value = Convert.ToString(currentMonth1);
        //        worksheet.Cell(currentRow, 7).Value = Convert.ToString(currentMonth);
        //        worksheet.Cell(currentRow, 8).Value = "Update Count";
        //        for (int i = 0; i < data.Rows.Count; i++)
        //        {
        //            currentRow++;

        //            worksheet.Cell(currentRow, 1).Value = (XLCellValue)data.Rows[i]["People Manager Name"];
        //            worksheet.Cell(currentRow, 2).Value = (XLCellValue)data.Rows[i][Convert.ToString(currentMonth5)];
        //            worksheet.Cell(currentRow, 3).Value = (XLCellValue)data.Rows[i][Convert.ToString(currentMonth4)];
        //            worksheet.Cell(currentRow, 4).Value = (XLCellValue)data.Rows[i][Convert.ToString(currentMonth3)];
        //            worksheet.Cell(currentRow, 5).Value = (XLCellValue)data.Rows[i][Convert.ToString(currentMonth2)];
        //            worksheet.Cell(currentRow, 6).Value = (XLCellValue)data.Rows[i][Convert.ToString(currentMonth1)];
        //            worksheet.Cell(currentRow, 7).Value = (XLCellValue)data.Rows[i][Convert.ToString(currentMonth)];
        //            worksheet.Cell(currentRow, 8).Value = (XLCellValue)data.Rows[i]["Update Count"];
        //        }
        //        using var stream = new MemoryStream();
        //        worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.Purple;
        //        worksheet.Range("A1:H1").Style.Font.FontColor = XLColor.White;
        //        worksheet.Range("A1:H1").Style.Font.Bold = true;
        //        workbook.SaveAs(stream);
        //        var content = stream.ToArray();
        //        Response.Clear();
        //        Response.Headers.Add("content-disposition", "attachment;filename=SixMonthUpdate.xlsx");
        //        Response.ContentType = "application/xls";
        //        Response.Body.WriteAsync(content);
        //        Response.Body.Flush();
        //    }
        //}

        //private DataTable Get6MonthEmployeeDetails(List<tblViewUpdate6Month> view6Month)
        //{
        //    var session = new UserSession(HttpContext.Session);
        //    string currentMonth = session.GetCurrentMonth();
        //    string currentMonth1 = session.GetCurrentMonth1();
        //    string currentMonth2 = session.GetCurrentMonth2();
        //    string currentMonth3 = session.GetCurrentMonth3();
        //    string currentMonth4 = session.GetCurrentMonth4();
        //    string currentMonth5 = session.GetCurrentMonth5();

        //    DataTable dtProduct = new DataTable("Report");
        //    dtProduct.Columns.AddRange(new DataColumn[8] {

        //    new DataColumn("People Manager Name"),
        //    new DataColumn(Convert.ToString(currentMonth5)),
        //    new DataColumn(Convert.ToString(currentMonth4)),
        //    new DataColumn(Convert.ToString(currentMonth3)),
        //    new DataColumn(Convert.ToString(currentMonth2)),
        //    new DataColumn(Convert.ToString(currentMonth1)),
        //    new DataColumn(Convert.ToString(currentMonth)),
        //    new DataColumn("Update Count"),
        //    });
        //    foreach (var v in view6Month)
        //    {
        //        dtProduct.Rows.Add(
        //            v.PeopleManagerName,
        //            "'" + v.CurrentMonthMinus5,
        //            "'" + v.CurrentMonthMinus4,
        //            "'" + v.CurrentMonthMinus3,
        //            "'" + v.CurrentMonthMinus2,
        //            "'" + v.CurrentMonthMinus1,
        //            "'" + v.CurrentMonth,
        //            v.UpdatedCount
        //            );
        //    }
        //    return dtProduct;
        //}

        //public IActionResult GetReportDataBillability()
        //{
        //    try
        //    {
        //        var session = new UserSession(HttpContext.Session);
        //        string currentMonthYear = "";
        //        string currentMonthYear1 = "";
        //        string currentMonthYear2 = "";
        //        string parm = "";
        //        int day = (int)DateTime.Now.Day;

        //        if (day > 20)
        //        {
        //            DateTime curMon = DateTime.Now.AddMonths(-1);
        //            string month = curMon.ToString("MMM");
        //            string year = curMon.ToString("yy");
        //            currentMonthYear = month + "-" + year;

        //            DateTime curMonth1 = DateTime.Now.AddMonths(-2);
        //            string month1 = curMonth1.ToString("MMM");
        //            string year1 = curMon.ToString("yy");
        //            currentMonthYear1 = month1 + "-" + year1;

        //            DateTime curMonth2 = DateTime.Now.AddMonths(-3);
        //            string month2 = curMonth2.ToString("MMM");
        //            string year2 = curMon.ToString("yy");
        //            currentMonthYear2 = month2 + "-" + year2;


        //            session.setMonth(currentMonthYear, currentMonthYear1, currentMonthYear2, "", "", "");
        //            parm = currentMonthYear2 + "," + currentMonthYear1 + "," + currentMonthYear;
        //        }
        //        else
        //        {
        //            DateTime curMon = DateTime.Now.AddMonths(-2);
        //            string month = curMon.ToString("MMM");
        //            string year = curMon.ToString("yy");
        //            currentMonthYear = month + "-" + year;

        //            DateTime curMonth1 = DateTime.Now.AddMonths(-3);
        //            string month1 = curMonth1.ToString("MMM");
        //            string year1 = curMon.ToString("yy");
        //            currentMonthYear1 = month1 + "-" + year1;

        //            DateTime curMonth2 = DateTime.Now.AddMonths(-4);
        //            string month2 = curMonth2.ToString("MMM");
        //            string year2 = curMon.ToString("yy");
        //            currentMonthYear2 = month2 + "-" + year2;


        //            session.setMonth(currentMonthYear, currentMonthYear1, currentMonthYear2, "", "", "");
        //            parm = currentMonthYear2 + "," + currentMonthYear1 + "," + currentMonthYear;
        //        }


        //        string SqlconString = this.Configuration.GetConnectionString("DefaultConnection");
        //        using (SqlConnection con = new SqlConnection(SqlconString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("spGetUpdateBillability", con))
        //            {
        //                cmd.CommandTimeout = 0;
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.Add("@Month", SqlDbType.VarChar).Value = parm;
        //                con.Open();
        //                cmd.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }

        //        List<tbl_UpdateBillability> bill = _dbContext.tbl_UpdateBillability.ToList();

        //        string Role = session.GetManager();
        //        string Region = session.GetRegion();
        //        string ServiceLine = session.GetServiceLine();

        //        if (Role == "Admin")
        //        {
        //            if (!string.IsNullOrEmpty(Region))
        //            {
        //                bill = bill.Where(c => c.RevenueRegion == Region).ToList();
        //            }
        //            if (!string.IsNullOrEmpty(ServiceLine))
        //            {
        //                bill = bill.Where(c => c.ServiceLine == ServiceLine).ToList();
        //            }
        //        }



        //        DownLoadBillabilityReport(bill);
        //        return RedirectToAction("ReportIndex");
        //    }
        //    catch
        //    {
        //        return RedirectToAction("ReportIndex");
        //    }

        //}

        private void DownLoadBillabilityReport(List<tbl_UpdateBillability> bill)
        {
            var data = new DataTable();
            data = GetBillableDetails(bill);
            ExportBillabilityReport(data);
        }

        private void ExportBillabilityReport(DataTable data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("BillabilityDetails");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Eid";
                worksheet.Cell(currentRow, 2).Value = "Employee Name";
                worksheet.Cell(currentRow, 3).Value = "People Manager Name";
                worksheet.Cell(currentRow, 4).Value = "People Manager Email";
                worksheet.Cell(currentRow, 5).Value = "Revenue Region";
                worksheet.Cell(currentRow, 6).Value = "Service Line";
                worksheet.Cell(currentRow, 7).Value = "Country";
                worksheet.Cell(currentRow, 8).Value = "DXCMHL5";
                worksheet.Cell(currentRow, 9).Value = "People Manager L3 Cheif";
                worksheet.Cell(currentRow, 10).Value = "People Manager L4 Cheif";
                worksheet.Cell(currentRow, 11).Value = "People Manager L5 Cheif";
                worksheet.Cell(currentRow, 12).Value = "Billability Current Month";
                worksheet.Cell(currentRow, 13).Value = "Month".ToString();
                worksheet.Cell(currentRow, 14).Value = "Tentative Billing Date";
                worksheet.Cell(currentRow, 15).Value = "Category";
                worksheet.Cell(currentRow, 16).Value = "Business Justification";
                worksheet.Cell(currentRow, 17).Value = "Comment";
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = (XLCellValue)data.Rows[i]["Eid"];
                    worksheet.Cell(currentRow, 2).Value = (XLCellValue)data.Rows[i]["Employee Name"];
                    worksheet.Cell(currentRow, 3).Value = (XLCellValue)data.Rows[i]["People Manager Name"];
                    worksheet.Cell(currentRow, 4).Value = (XLCellValue)data.Rows[i]["People Manager Email"];
                    worksheet.Cell(currentRow, 5).Value = (XLCellValue)data.Rows[i]["Revenue Region"];
                    worksheet.Cell(currentRow, 6).Value = (XLCellValue)data.Rows[i]["Service Line"];
                    worksheet.Cell(currentRow, 7).Value = (XLCellValue)data.Rows[i]["Country"];
                    worksheet.Cell(currentRow, 8).Value = (XLCellValue)data.Rows[i]["DXCMHL5"];
                    worksheet.Cell(currentRow, 9).Value = (XLCellValue)data.Rows[i]["People Manager L3 Cheif"];
                    worksheet.Cell(currentRow, 10).Value = (XLCellValue)data.Rows[i]["People Manager L4 Cheif"];
                    worksheet.Cell(currentRow, 11).Value = (XLCellValue)data.Rows[i]["People Manager L5 Cheif"];
                    worksheet.Cell(currentRow, 12).Value = (XLCellValue)data.Rows[i]["Billability Current Month"];
                    worksheet.Cell(currentRow, 13).Value = (XLCellValue)data.Rows[i]["Month"].ToString();
                    worksheet.Cell(currentRow, 14).Value = (XLCellValue)data.Rows[i]["Tentative Billing Date"];
                    worksheet.Cell(currentRow, 15).Value = (XLCellValue)data.Rows[i]["Category"];
                    worksheet.Cell(currentRow, 16).Value = (XLCellValue)data.Rows[i]["Business Justification"];
                    worksheet.Cell(currentRow, 17).Value = (XLCellValue)data.Rows[i]["Comment"];
                }
                using var stream = new MemoryStream();
                worksheet.Range("A1:Q1").Style.Fill.BackgroundColor = XLColor.Purple;
                worksheet.Range("A1:Q1").Style.Font.FontColor = XLColor.White;
                worksheet.Range("A1:Q1").Style.Font.Bold = true;
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=Billability Report.xlsx");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                Response.Body.Flush();
            }
        }

        private DataTable GetBillableDetails(List<tbl_UpdateBillability> bill)
        {
            DataTable dtProduct = new DataTable("Report");
            dtProduct.Columns.AddRange(new DataColumn[17] {

            new DataColumn("Eid"),
            new DataColumn("Employee Name"),
            new DataColumn("People Manager Name"),
            new DataColumn("People Manager Email"),
            new DataColumn("Revenue Region"),
            new DataColumn("Service Line"),
            new DataColumn("Country"),
            new DataColumn("DXCMHL5"),
            new DataColumn("People Manager L3 Cheif"),
            new DataColumn("People Manager L4 Cheif"),
            new DataColumn("People Manager L5 Cheif"),
            new DataColumn("Billability Current Month"),
            new DataColumn("Month"),
             new DataColumn("Tentative Billing Date"),
            new DataColumn("Category"),
            new DataColumn("Business Justification"),
            new DataColumn("Comment"),
            });
            foreach (var v in bill)
            {
                dtProduct.Rows.Add(
                    v.Eid,
                    v.EmployeeName,
                    v.PeopleMgrName,
                    v.PeopleMgrEmail,
                    v.RevenueRegion,
                    v.ServiceLine,
                    v.Country,
                    v.DXCMHL5,
                    v.L3Cheif,
                    v.L4Cheif,
                    v.L5Cheif,
                    v.BillablePer,
                    "'" + v.Month,
                    v.TentitiveBillingDate,
                    v.category,
                    v.BusinessJustification,
                    v.Comments
                    );
            }
            return dtProduct;
        }

        private void DownloadReport(List<tblViewUpdate3Month> view3Month)
        {
            var data = new DataTable();
            //data = GetEmployeeDetails(view3Month);
            //ExportToExcel(data);
        }

        //private void ExportToExcel(DataTable data)
        //{
        //    var session = new UserSession(HttpContext.Session);
        //    string currentMonth = "'" + session.GetCurrentMonth();
        //    string currentMonth1 = "'" + session.GetCurrentMonth1();
        //    string currentMonth2 = "'" + session.GetCurrentMonth2();

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("EmployeeDetails");
        //        var currentRow = 1;
        //        worksheet.Cell(currentRow, 1).Value = "People Manager Name";
        //        worksheet.Cell(currentRow, 2).Value = "People Manager Email";
        //        worksheet.Cell(currentRow, 3).Value = currentMonth2.ToString();
        //        worksheet.Cell(currentRow, 4).Value = currentMonth1.ToString();
        //        worksheet.Cell(currentRow, 5).Value = currentMonth.ToString();
        //        //worksheet.Cell(currentRow, 6).Value = "Update Count";
        //        for (int i = 0; i < data.Rows.Count; i++)
        //        {
        //            currentRow++;

        //            worksheet.Cell(currentRow, 1).Value = (XLCellValue)data.Rows[i]["People Manager Name"];
        //            worksheet.Cell(currentRow, 2).Value = (XLCellValue)data.Rows[i]["People Manager Email"];
        //            worksheet.Cell(currentRow, 3).Value = data.Rows[i][currentMonth2].ToString();
        //            worksheet.Cell(currentRow, 4).Value = data.Rows[i][currentMonth1].ToString();
        //            worksheet.Cell(currentRow, 5).Value = data.Rows[i][currentMonth].ToString();
        //            //worksheet.Cell(currentRow, 6).Value = data.Rows[i]["Update Count"];
        //        }
        //        using var stream = new MemoryStream();
        //        worksheet.Range("A1:E1").Style.Fill.BackgroundColor = XLColor.Purple;
        //        worksheet.Range("A1:E1").Style.Font.FontColor = XLColor.White;
        //        worksheet.Range("A1:E1").Style.Font.Bold = true;
        //        workbook.SaveAs(stream);
        //        var content = stream.ToArray();
        //        Response.Clear();
        //        Response.Headers.Add("content-disposition", "attachment;filename=ThreeMonthUpdate-DOT.xlsx");
        //        Response.ContentType = "application/xls";
        //        Response.Body.WriteAsync(content);
        //        Response.Body.Flush();
        //    }
        //}

        //private DataTable GetEmployeeDetails(List<tblViewUpdate3Month> view3Month)
        //{
        //    var session = new UserSession(HttpContext.Session);
        //    string currentMonth = session.GetCurrentMonth();
        //    string currentMonth1 = session.GetCurrentMonth1();
        //    string currentMonth2 = session.GetCurrentMonth2();

        //    DataTable dtProduct = new DataTable("Report");
        //    dtProduct.Columns.AddRange(new DataColumn[5] {

        //    new DataColumn("People Manager Name"),
        //    new DataColumn("People Manager Email"),
        //    new DataColumn("'"+currentMonth2),
        //    new DataColumn("'"+currentMonth1),
        //    new DataColumn("'"+currentMonth),
        //    //new DataColumn("Update Count"),
        //    });
        //    foreach (var v in view3Month)
        //    {
        //        dtProduct.Rows.Add(
        //            v.PeopleManagerName,
        //            v.PeopleManagerEmail,
        //           v.CurrentMonthMinus2,
        //           v.CurrentMonthMinus1,
        //            v.CurrentMonth
        //            //v.UpdatedCount
        //            );
        //    }
        //    return dtProduct;
        //}
    }
}
