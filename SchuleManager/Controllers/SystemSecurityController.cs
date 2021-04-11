using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using SchuleManager.Filters;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    [ValidateUserSession]
    public class SystemSecurityController : Controller
    {
        // GET: SystemSecurity/UserMaintenance
        private Db _dblayer;

        [HttpGet]
        public ActionResult Users()
        {
            ViewBag.CurrentUser = Session["Username"].ToString();
                
            return View();
        }

        [HttpGet]
        public JsonResult GetUserDetails(string username)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var userDetailsDataTable = _dblayer.GetUserDetails(username);

            var userDetails = (from row in userDetailsDataTable.AsEnumerable()
                select new SystemUsers
                {
                    RoleCode = row.Field<string>("RoleCode"),
                    Description = row.Field<string>("Description"),
                    UserPassword = EncryptionLibrary.DecryptText(row.Field<string>("UserPassword")),
                    SurName = row.Field<string>("SurName"),
                    OtherNames = row.Field<string>("OtherNames"),
                    EmailId = row.Field<string>("EmailId"),
                    PhoneNo1 = row.Field<string>("PhoneNo1"),
                    IsCashier = row.Field<bool>("IsCashier"),
                    CashierGl = row.Field<string>("CashierGl"),
                    CashierGLName = row.Field<string>("CashierGLName"),
                    PasswordNeverExpires = row.Field<bool>("PasswordNeverExpires"),
                    MustChangePassword = row.Field<bool>("MustChangePassword"),
                    IsDisabled = row.Field<bool>("IsDisabled"),
                    IsVerified = row.Field<bool>("IsVerified"),
                    CreatedBy = row.Field<string>("CreatedBy")
                }).AsQueryable();

            return Json(userDetails, JsonRequestBehavior.AllowGet);
          
        }

        [HttpGet]
        public JsonResult GetRoleDetails(string roleCode)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var roleDetailsDataTable = _dblayer.GetRoleDetails(roleCode);

            var roleDetails = (from row in roleDetailsDataTable.AsEnumerable()
                select new SystemRoles
                {
                    Description = row.Field<string>("Description"),
                    Deleted = row.Field<bool>("Deleted")
                }).AsQueryable();

            return Json(roleDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGlAccountDetails(string accountId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var accountDetails = _dblayer.GetAccountDetails(accountId, 'G');

            var glAccountDetails = (from row in accountDetails.AsEnumerable()
                select new GlAccounts
                {
                    AccountID = row.Field<string>("AccountID"),
                    AccountName = row.Field<string>("AccountName"),
                    AccountType = row.Field<string>("AccountType"),
                    AccountSubType = row.Field<string>("AccountSubType"),
                    Balance = row.Field<decimal>("Balance"),
                    ShadowBalance = row.Field<decimal>("ShadowBalance"),
                    ClearBalance = row.Field<decimal>("ClearBalance"),
                    UnSupervisedCredit = row.Field<decimal>("UnSupervisedCredit"),
                    UnSupervisedDebit = row.Field<decimal>("UnSupervisedDebit"),
                    CreditTurnOver = row.Field<decimal>("CreditTurnOver"),
                    DebitTurnOver = row.Field<decimal>("DebitTurnOver"),
                    IsVerified = row.Field<bool>("IsVerified")

                }).AsQueryable();

            return Json(glAccountDetails, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult SuperviseSystemUser(string username)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.SuperviseSystemUsers(username, Session["Username"].ToString());

            if (result > 0)
            {
                return Json(new { success = true, message = "User has been supervised Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "User Supervision failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddEditSystemUsers(SystemUsers user, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditSystemUsers(user, action, Session["Username"].ToString());

            return result > 0 ? Json(action != 'A' ? new { success = true, message = "User Updated Successfully" } : new { success = true, message = "New User Added Successfully" }, JsonRequestBehavior.AllowGet) : Json(result == -10 ? new { success = false, message = "Cashier GL Already assigned to another user" } : new { success = false, message = "Add or Update Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Roles()
        {
            return View();
        }

        public ActionResult LoadUserRoles()
        {
            var data = ShowAllRoles();

            return Json(new { data });
        }

        public IQueryable<SystemRoles> ShowAllRoles()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var systemRoles = _dblayer.GetRoles();

            var roleData = (from row in systemRoles.AsEnumerable()
                select new SystemRoles
                {
                    RoleCode = row.Field<string>("RoleCode"),
                    Description = row.Field<string>("Description"),
                    Deleted = row.Field<bool>("Deleted")
                }).AsQueryable();

            return roleData;
        }

        public ActionResult LoadRoleMembers(string roleCode)
        {
            var data = ShowRoleMembers(roleCode);

            return Json(new { data });
        }

        public IQueryable<SystemUsers> ShowRoleMembers(string roleCode)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var systemRoles = _dblayer.GetRoleMembers(roleCode);

            var roleMembersData = (from row in systemRoles.AsEnumerable()
                select new SystemUsers
                {
                    UserName = row.Field<string>("UserName"),
                    SurName = row.Field<string>("SurName"),
                    OtherNames = row.Field<string>("OtherNames")
                }).AsQueryable();

            return roleMembersData;
        }

        [HttpPost]
        public ActionResult AddEditRoles(SystemRoles role, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditRoles(role, action, Session["Username"].ToString());

            if (result > 0)
            {
                return Json(action == 'A' ? new { success = true, message = "Role Added Successfully" } : new { success = true, message = "Role Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result == -10 ? new { success = false, message = "Role already exists" } : new { success = false, message = "Addition or Updation Failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AccessRights()
        {
            //var model = GetModuleTreeData();

            //var jsonModel = new JavaScriptSerializer().Serialize(model);

            //return View(jsonModel);

            //return View("AccessRights", "_Layout", jsonModel);

            GetModules();

            ViewBag.SubModules = new SelectList(new List<SystemModules>(), "SubModuleId", "SubModule");

            return View();
        }

        private void GetModules()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());
            var dt = _dblayer.GetModules();
            var view = new DataView(dt);

            var dst = view.ToTable(true, "ModuleId", "ModuleName");

            var modules = (from row in dst.AsEnumerable()
                select new SystemModules
                {
                    ModuleId = row.Field<int>("ModuleId"),
                    ModuleName = row.Field<string>("ModuleName")
                }).Distinct();

            var lstModules = modules.ToList();

            ViewBag.Modules = new SelectList(lstModules, "ModuleId", "ModuleName");          
            
        }

        public JsonResult GetSubModules(int moduleId)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtModules = _dblayer.GetModules();
            var dtViewModules = new DataView(dtModules);
            var dstSubModules = dtViewModules.ToTable(true, "ModuleId", "SubModuleId", "SubModule");

            var subModules = (from row in dstSubModules.AsEnumerable()
                select new SystemModules
                {
                    ModuleId = row.Field<int>("ModuleId"),
                    SubModuleId = row.Field<int>("SubModuleId"),
                    SubModule = row.Field<string>("SubModule")
                }).Distinct().Where(m => m.ModuleId.Equals(moduleId));

            return Json(subModules.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRoleRights(string roleCode, int subModuleId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dt = _dblayer.GetRoleRights(roleCode, subModuleId);

            var view = new DataView(dt);
            var dst = view.ToTable(true, "MenuId", "MenuDescription", "HasAccess");

            var rights = (from row in dst.AsEnumerable()
                select new RoleRights
                {
                    MenuId = row.Field<int>("MenuId"),
                    MenuDescription = row.Field<string>("MenuDescription"),
                    HasAccess = row.Field<bool>("HasAccess")
                }).Distinct();

            return PartialView("_RoleRights", rights);
        }

        public ActionResult GetDataEntryRights(string roleCode, int menuId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dt = _dblayer.GetDataEntryRights(roleCode, menuId);

            var view = new DataView(dt);
            var dst = view.ToTable(true, "RoleCode","MenuId", "DataDescription", "AskAdd", "AskEdit", "AskDelete", "AskPostingLimit", "SupervisionDescription", "AskSupervision", "AskSupervisionLimit", "CanAdd", "CanEdit", "CanDelete", "PostingLimit", "CanSupervise", "SupervisionLimit");

            var rights = (from row in dst.AsEnumerable()
                select new DataEntryRightsModel
                {
                    RoleCode = row.Field<string>("RoleCode"),
                    MenuId = row.Field<int>("MenuId"),
                    DataDescription = row.Field<string>("DataDescription"),
                    AskAdd = row.Field<bool>("AskAdd"),
                    AskEdit = row.Field<bool>("AskEdit"),
                    AskDelete = row.Field<bool>("AskDelete"),
                    AskPostingLimit = row.Field<bool>("AskPostingLimit"),
                    SupervisionDescription = row.Field<string>("SupervisionDescription"),
                    AskSupervision = row.Field<bool>("AskSupervision"),
                    AskSupervisionLimit = row.Field<bool>("AskSupervisionLimit"),
                    CanAdd = row.Field<bool>("CanAdd"),
                    CanEdit = row.Field<bool>("CanEdit"),
                    CanDelete = row.Field<bool>("CanDelete"),
                    PostingLimit = row.Field<decimal>("PostingLimit"),
                    CanSupervise = row.Field<bool>("CanSupervise"),
                    SupervisionLimit = row.Field<decimal>("SupervisionLimit")
                }).Distinct();

            return PartialView("_DataEntryRights", rights);

        }


        private List<JsTreeModel> GetModuleTreeData(string roleCode = "")
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var systemModules = _dblayer.GetModuleTreeData(roleCode);
            var view = new DataView(systemModules);

            var dstModules = view.ToTable(true, "id", "parent", "text", "children", "treeLevel", "selected");
            var results = (from row in dstModules.AsEnumerable()
                select new JsTree
                {
                    id = row.Field<string>("id"),
                    parent = row.Field<string>("parent"),
                    text = row.Field<string>("text"),
                    children = row.Field<bool>("children"),
                    treeLevel = row.Field<int>("treeLevel"),
                    selected = row.Field<bool>("selected")
                }).Distinct();

            var modules = results.ToList().Where(x => x.treeLevel == 1);
            var submodules = results.ToList().Where(x => x.treeLevel == 2);
            var menus = results.ToList().Where(x => x.treeLevel == 3);


            var moduleTree = new List<JsTreeModel>();
            var subModuleTree = new List<JsTreeModel>();
            var menuTree = new List<JsTreeModel>();


            foreach (var menu in menus)
            {
                menuTree.Add(new JsTreeModel
                {
                    data = menu.text,
                    parent = menu.parent,
                    attr = new JsTreeAttribute { id = menu.id, selected = menu.selected }
                });
            }

            foreach (var submodule in submodules)
            {
                subModuleTree.Add(new JsTreeModel
                {
                    data = submodule.text,
                    parent = submodule.parent,
                    attr = new JsTreeAttribute { id = submodule.id },
                    children = menuTree.Where(x => x.parent == submodule.id).ToList()
                });
            }

            foreach (var module in modules)
            {
                moduleTree.Add(new JsTreeModel
                {
                    data = module.text,
                    attr = new JsTreeAttribute { id = module.id },
                    children = subModuleTree.Where(x => x.parent == module.id).ToList()
                });
            }

            return moduleTree;
        }

        [HttpPost]
        public ActionResult AccessRights(FormCollection formData)
        {
            var roleCode = Request.Form["RoleCode"];

            var roleRightsData = formData.Keys.Cast<string>().ToDictionary(key => key, key => formData[key]);
            var dt = GenerateRoleRightsData(roleCode, roleRightsData);

            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.UpdateRoleRights(roleCode, dt);

            return Json(result > 0 ? new {success = true, message = "Role Rights Update Successfully"} : new {success = false, message = "Access Rights Update Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        public DataTable GenerateRoleRightsData(string roleCode, Dictionary<string, string> list)
        {
            var roleRights = new DataTable();

            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            roleRights.Columns.Add(new DataColumn("RoleCode", typeof(string)));
            roleRights.Columns.Add(new DataColumn("ModuleId", typeof(int)));
            roleRights.Columns.Add(new DataColumn("SubModuleId", typeof(int)));
            roleRights.Columns.Add(new DataColumn("MenuId", typeof(int)));

            foreach (var entry in list)
            {
                if (RegexHelper.Like(entry.Key, "check_3%"))
                {
                    var row = roleRights.NewRow();
                    var id = Convert.ToInt32(entry.Value);

                    //if (id < 3000) continue;
                    var pid = _dblayer.GetModuleParentId(id);
                    var ppid = _dblayer.GetModuleParentId(pid);

                    row["RoleCode"] = roleCode;
                    row["ModuleId"] = ppid;
                    row["SubModuleId"] = pid;
                    row["MenuId"] = id;

                    roleRights.Rows.Add(row);
                }
                
            }

            return roleRights;
        }

        //public ActionResult GetRoleRightsTree(string roleCode = "")
        //{
        //    _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

        //    var systemModules = _dblayer.GetModuleTreeData(roleCode);
        //    var view = new DataView(systemModules);

        //    var dstModules = view.ToTable(true, "id", "parent", "text", "children", "treeLevel", "selected");
        //    var results = (from row in dstModules.AsEnumerable()
        //        select new JsTree()
        //        {
        //            id = row.Field<string>("id"),
        //            parent = row.Field<string>("parent"),
        //            text = row.Field<string>("text"),
        //            children = row.Field<bool>("children"),
        //            treeLevel = row.Field<int>("treeLevel"),
        //            selected = row.Field<bool>("selected")
        //        }).Distinct();

        //    var modules = results.ToList().Where(x => x.treeLevel == 1);
        //    var submodules = results.ToList().Where(x => x.treeLevel == 2);
        //    var menus = results.ToList().Where(x => x.treeLevel == 3);


        //    var moduleTree = new List<JsTreeModel>();
        //    var subModuleTree = new List<JsTreeModel>();
        //    var menuTree = new List<JsTreeModel>();


        //    foreach (var menu in menus)
        //    {
        //        menuTree.Add(new JsTreeModel
        //        {
        //            data = menu.text,
        //            parent = menu.parent,
        //            attr = new JsTreeAttribute { id = menu.id, selected = menu.selected}
        //        });
        //    }

        //    foreach (var submodule in submodules)
        //    {
        //        subModuleTree.Add(new JsTreeModel
        //        {
        //            data = submodule.text,
        //            parent = submodule.parent,
        //            attr = new JsTreeAttribute { id = submodule.id },
        //            children = menuTree.Where(x => x.parent == submodule.id).ToList()
        //        });
        //    }

        //    foreach (var module in modules)
        //    {
        //        moduleTree.Add(new JsTreeModel
        //        {
        //            data = module.text,
        //            attr = new JsTreeAttribute { id = module.id },
        //            children = subModuleTree.Where(x => x.parent == module.id).ToList()
        //        });
        //    }

        //    var jsonModel = new JavaScriptSerializer().Serialize(moduleTree);

        //    return PartialView("_RoleRightsTree",  jsonModel);
        //}
        
        

        public void SaveDataEntryRights(DataEntryRights der)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            _dblayer.UpdateDataEntryRights(der);
        }

        
    }
}