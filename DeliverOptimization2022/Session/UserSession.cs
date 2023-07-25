using DeliverOptimization2022.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverOptimization2022.Session
{
    public class UserSession
    {
        private ISession _session { get; set; }
        public UserSession(ISession session)
        {
            _session = session;
        }
        private const string EmployeeKey = "Employee";
        private const string RotationEmployeeKey = "RotationEmployee";
        private const string MigrationEmployeeKey = "MigrationEmployee";
        //private const string EmployeeLBKey = "EmployeeLB";
        //private const string EmployeeLBOKey = "EmployeeLBO";
        //private const string EmployeeKeyHB = "EmployeeHB";
        private const string EmployeeRoleListKey = "EmployeeRoleList";


        private const string EmployeeReportKey = "EmployeeReport";
        private const string UserNameKey = "UserName";
        private const string UserEmailKey = "UserEmail";
        private const string ManagerKey = "Manager";
        //private const string FilterStringKey = "FilterString";
        //private const string FilterStatusKey = "FilterStatus";
        //private const string FilterIdValueKey = "FilterIdValue";
        private const string FilterDropdownValueKey = "FilterDropdownValue";
        private const string RegionKey = "Region";
        private const string ServiceLineKey = "ServiceLine";
        private const string UserShortNameKey = "UserShortName";
        //public const string EMEAEmployeeKey = "EMEAEmployee";
        public const string ToolDescriptionKey = "ToolDescription";
        //public const string ShowDOTMenuKey = "ShowDOTMenu";
        //public const string ShowEMEAMenuKey = "ShowEMEAMenu";
        //private const string EMEAManagerKey = "EMEAManager";

        //private const string CurrentMonthKey = "CurrentMonth";
        //private const string CurrentMonth1Key = "CurrentMonth1";
        //private const string CurrentMonth2Key = "CurrentMonth2";
        //private const string CurrentMonth3Key = "CurrentMonth3";
        //private const string CurrentMonth4Key = "CurrentMonth4";
        //private const string CurrentMonth5Key = "CurrentMonth5";

        private const string LowBillabilityKey = "LowBillability";
        private const string LowBillabilityOutliersKey = "LowBillabilityOutliers";
        private const string RotationKey = "Rotation";
        private const string MigrationKey = "Migration";

        private const string EditLowBillabilityKey = "EditLowBillability";
        private const string EditLowBillabilityOutliersKey = "EditLowBillabilityOutliers";
        private const string EditRotationKey = "EditRotation";
        private const string EditMigrationKey = "EditMigration";



        public void setLowBillabilityEdit(string eidtStatus)
        {
            _session.SetString(EditLowBillabilityKey, eidtStatus);
        }
        public void setLowBillabilityOutliersEdit(string eidtStatus)
        {
            _session.SetString(EditLowBillabilityOutliersKey, eidtStatus);
        }
        public void setRotationEdit(string eidtStatus)
        {
            _session.SetString(EditRotationKey, eidtStatus);
        }
        public void setMigrationEdit(string eidtStatus)
        {
            _session.SetString(EditMigrationKey, eidtStatus);
        }

        public string GetLowBillabilityEdit()
        {
            return _session.GetString(EditLowBillabilityKey);
        }
        public string GetLowBillabilityOutliersEdit()
        {
            return _session.GetString(EditLowBillabilityOutliersKey);
        }
        public string GetRotationEdit()
        {
            return _session.GetString(EditRotationKey);
        }
        public string GetMigrationEdit()
        {
            return _session.GetString(EditMigrationKey);
        }
        //public void setMonth(string currentMonth, string currentMonth1, string currentMonth2,
        //    string currentMonth3, string currentMonth4, string currentMonth5)
        //{
        //    _session.SetString(CurrentMonthKey, currentMonth);
        //    _session.SetString(CurrentMonth1Key, currentMonth1);
        //    _session.SetString(CurrentMonth2Key, currentMonth2);
        //    _session.SetString(CurrentMonth3Key, currentMonth3);
        //    _session.SetString(CurrentMonth4Key, currentMonth4);
        //    _session.SetString(CurrentMonth5Key, currentMonth5);
        //}

        public string GetLowBillabilityTab()
        {
            return _session.GetString(LowBillabilityKey);
        }
        public string GetLowBillabilityOutliersTab()
        {
            return _session.GetString(LowBillabilityOutliersKey);
        }
        public string GetRotationTab()
        {
            return _session.GetString(RotationKey);
        }
        public string GetMigrationTab()
        {
            return _session.GetString(MigrationKey);
        }
        //public string GetCurrentMonth()
        //{
        //    return _session.GetString(CurrentMonthKey);
        //}
        //public string GetCurrentMonth1()
        //{
        //    return _session.GetString(CurrentMonth1Key);
        //}
        //public string GetCurrentMonth2()
        //{
        //    return _session.GetString(CurrentMonth2Key);
        //}
        //public string GetCurrentMonth3()
        //{
        //    return _session.GetString(CurrentMonth3Key);
        //}
        //public string GetCurrentMonth4()
        //{
        //    return _session.GetString(CurrentMonth4Key);
        //}
        //public string GetCurrentMonth5()
        //{
        //    return _session.GetString(CurrentMonth5Key);
        //}
        public void setUser(string userName, string email,string region, string serviceLine, string userShortName, string toolDescription)
        {
            _session.SetString(UserNameKey, userName);
            _session.SetString(UserEmailKey, email);
            _session.SetString(UserShortNameKey, userShortName);

            if (!string.IsNullOrEmpty(region))
                _session.SetString(RegionKey, region);

            if (!string.IsNullOrEmpty(serviceLine))
                _session.SetString(ServiceLineKey, serviceLine);

            if (!string.IsNullOrEmpty(toolDescription))
                _session.SetString(ToolDescriptionKey, toolDescription);
        }
        public void setDOTManagerRole(string role)
        {
            _session.SetString(ManagerKey, role);
        }
        //public void setEMEAManagerRole(string role)
        //{
        //    _session.SetString(EMEAManagerKey, role);
        //}

        public void setLowBillabilityTab(string tabName)
        {
            _session.SetString(LowBillabilityKey, tabName);
        }
        public void setLowBillabilityOutliersTab(string tabName)
        {
            _session.SetString(LowBillabilityOutliersKey, tabName);
        }
        public void setRotationTab(string tabName)
        {
            _session.SetString(RotationKey, tabName);
        }
        public void setMigrationTab(string tabName)
        {
            _session.SetString(MigrationKey, tabName);
        }

        public void SetMyEmployeeNew(List<tbl_EmployeeOptimizationList_New> list)
        {
            _session.SetObject(EmployeeKey, list);
        }
        public void SetMyEmployeeRotaton(List<tbl_EmployeeRotation> list)
        {
            _session.SetObject(RotationEmployeeKey, list);
        }
        public void SetMyEmployeeMigration(List<tbl_EmployeeMigration> list)
        {
            _session.SetObject(MigrationEmployeeKey, list);
        }

        public void SetEmployeeRoleList(LoggedInEmployeeRole list)
        {
            _session.SetObject(EmployeeRoleListKey, list);
        }


        //public void SetMyEmployeeNewLB(List<tbl_EmployeeOptimizationList_New> list)
        //{
        //    _session.SetObject(EmployeeLBKey, list);
        //}

        //public void SetMyEmployeeNewLBO(List<tbl_EmployeeOptimizationList_New> list)
        //{
        //    _session.SetObject(EmployeeLBOKey, list);
        //}

        //public void SetMyEmployeeNewLHB(List<tbl_EmployeeOptimizationList_New> list)
        //{
        //    _session.SetObject(EmployeeKeyHB, list);
        //}

       
        public List<tbl_EmployeeOptimizationList_New> GetEmployeeNew() =>
          _session.GetObject<List<tbl_EmployeeOptimizationList_New>>(EmployeeKey) ??
              new List<tbl_EmployeeOptimizationList_New>();

        public List<tbl_EmployeeRotation> GetEmployeeRotation() =>
          _session.GetObject<List<tbl_EmployeeRotation>>(RotationEmployeeKey) ??
              new List<tbl_EmployeeRotation>();

        public List<tbl_EmployeeMigration> GetEmployeeMigration() =>
          _session.GetObject<List<tbl_EmployeeMigration>>(MigrationEmployeeKey) ??
              new List<tbl_EmployeeMigration>();

        public LoggedInEmployeeRole GetEmployeeRoleList() =>
            _session.GetObject<LoggedInEmployeeRole>(EmployeeRoleListKey) ??
                new LoggedInEmployeeRole();


    
        //public void setFilterIdValue(string Id)
        //{
        //    _session.SetString(FilterIdValueKey, Id);
           
        //}
        public void setFilterDropdownValue(string Id)
        {
            _session.SetString(FilterDropdownValueKey, Id);

        }
        
        //public void setFilter(string filterStringStatus, string filterStringKey)
        //{
        //    _session.SetString(FilterStatusKey, filterStringStatus);
        //    _session.SetString(FilterStringKey, filterStringKey);
        //}

        //public void setShowDOTMenu(string DOTMenu)
        //{
        //    _session.SetString(ShowDOTMenuKey, DOTMenu);
        //}
        //public string GetShowDOTMenu()
        //{
        //    return _session.GetString(ShowDOTMenuKey);
        //}

        //public void setShowEMEAMenu(string EMEAMenu)
        //{
        //    _session.SetString(ShowEMEAMenuKey, EMEAMenu);
        //}
        //public string GetShowEMEAMenu()
        //{
        //    return _session.GetString(ShowEMEAMenuKey);
        //}

        public string GetUserShortName()
        {
            return _session.GetString(UserShortNameKey);
        }
        public string GetToolDescription()
        {
            return _session.GetString(ToolDescriptionKey);
        }
        public string GetUsername()
        {
            return _session.GetString(UserNameKey);
        }

        public string GetRegion()
        {
            return _session.GetString(RegionKey);
        }

        public string GetServiceLine()
        {
            return _session.GetString(ServiceLineKey);
        }

        //public string GetFilterStatus()
        //{
        //    return _session.GetString(FilterStatusKey);
        //}

        //public string GetFilterString()
        //{
        //    return _session.GetString(FilterStringKey);
        //}

        //public string GetFilterIdValue()
        //{
        //    return _session.GetString(FilterIdValueKey);
        //}
        public string GetFilterDropdownValue()
        {
            return _session.GetString(FilterDropdownValueKey);
        }
        public string GetUseremail()
        {
            return _session.GetString(UserEmailKey);
        }
        public string GetManager()
        {
            return _session.GetString(ManagerKey);
        }
        //public string GetEMEAManager()
        //{
        //    return _session.GetString(EMEAManagerKey);
        //}
    }
}
