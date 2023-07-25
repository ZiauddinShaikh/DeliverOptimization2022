
using DeliverOptimization2022.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DeliverOptimization2022.Controllers
{
    //This controller is used for SigIn and SignOut from the application.
    //It is also used to Manage the User Like add new user, update and delete existing user.
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private IConfiguration Configuration;
        public AccountController(ApplicationDbContext dbContext, IConfiguration _configuration)
        {
            _dbContext = dbContext;
            Configuration = _configuration;
        }





        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl });
        }

        [HttpGet]

        public IActionResult SignOut()
        {
            SignOut();
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult UserAccess()
        {
            LoggedInEmployeeRole role = new LoggedInEmployeeRole();
            return View(role);
        }
        public IActionResult GetEmployeeRecord()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //IQueryable<LoggedInEmployeeRole> customerData = null;

                IQueryable<LoggedInEmployeeRole> customerData = _dbContext.EmployeeRole;
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.Role.Contains(searchValue)
                                                || m.Role.Contains(searchValue)
                                                || m.Region.Contains(searchValue)
                                                || m.ServiceLine.Contains(searchValue)
                                                || m.UserName.Contains(searchValue));
                }
                var data = new List<LoggedInEmployeeRole>();
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
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "AccountController";
                log.ActionName = "GetEmployeeRecord";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }
        }

        public ActionResult UpdateEmployee(string uId, string uName, string email, string role, string region, 
            string sLine, string toolDesc,string country)
        {
            try
            {
                int eid = Convert.ToInt32(uId);
                LoggedInEmployeeRole emp = _dbContext.EmployeeRole.Where(c => c.Id == eid).FirstOrDefault();
                emp.UserName = uName;
                emp.Email = email;
                emp.Role = role;
                if (!string.IsNullOrEmpty(region))
                {
                    emp.Region = region.Remove(region.Length - 1, 1);
                }
                else
                {
                    emp.Region = "";
                }
                if (!string.IsNullOrEmpty(sLine))
                {
                    emp.ServiceLine = sLine.Remove(sLine.Length - 1, 1);
                }
                else
                {
                    emp.ServiceLine = "";
                }
                if (!string.IsNullOrEmpty(toolDesc))
                {
                    emp.ToolsDescription = toolDesc;
                }
                else
                {
                    emp.ToolsDescription = "";
                }
                if (!string.IsNullOrEmpty(country))
                {
                    emp.Country = country;
                }
                else
                {
                    emp.Country = "";
                }
                _dbContext.EmployeeRole.Update(emp);
                _dbContext.SaveChanges();
                return Json(new { data = "Record Updated Sucessfully" });
            }
            catch (Exception ex)
            {
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "AccountController";
                log.ActionName = "UpdateEmployee";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }

        }
        public ActionResult DeleteEmployee(string uId)
        {
            try
            {
                int eid = Convert.ToInt32(uId);
                LoggedInEmployeeRole emp = _dbContext.EmployeeRole.Where(c => c.Id == eid).FirstOrDefault();
                _dbContext.EmployeeRole.Remove(emp);
                _dbContext.SaveChanges();
                return Json(new { data = "Record Deleted Sucessfully" });
            }
            catch (Exception ex)
            {
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "AccountController";
                log.ActionName = "DeleteEmployee";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }

        }

        public ActionResult AddEmployee(string uId, string uName, string email, string role, string region, string sLine, string toolDesc,string country)
        {
            try
            {
                LoggedInEmployeeRole emp = new LoggedInEmployeeRole();
                emp.UserName = uName;
                emp.Email = email;
                emp.Role = role;
                if (!string.IsNullOrEmpty(region))
                {
                    emp.Region = region.Remove(region.Length - 1, 1);
                }
                else
                {
                    emp.Region = "";
                }
                if (!string.IsNullOrEmpty(sLine))
                {
                    emp.ServiceLine = sLine.Remove(sLine.Length - 1, 1);
                }
                else
                {
                    emp.ServiceLine = "";
                }
                if (!string.IsNullOrEmpty(toolDesc))
                {
                    emp.ToolsDescription = toolDesc;
                }
                else
                {
                    emp.ToolsDescription = "";
                }
                _dbContext.EmployeeRole.Add(emp);
                _dbContext.SaveChanges();
                return Json(new { data = "game" });
            }
            catch (Exception ex)
            {
                tbl_HCW_ErrorLog log = new tbl_HCW_ErrorLog();
                log.ApplicationName = "DeliverOptimizationTools";
                log.ControllerName = "AccountController";
                log.ActionName = "AddEmployee";
                log.ErrorMessage = ex.Message;
                _dbContext.tbl_HCW_ErrorLog.Add(log);
                _dbContext.SaveChanges();
                return View("/Error");
            }

        }

        public ActionResult Backup()
        {
            string SqlconString = this.Configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(SqlconString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TakeBackup", con))
                {
                    //string closedate = Convert.ToString(formdata.CloseDate);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return Json(new { data = "Record Deleted Sucessfully" });
        }
    }
}
