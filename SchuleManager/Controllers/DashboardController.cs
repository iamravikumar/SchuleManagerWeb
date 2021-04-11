using System.Web.Mvc;
using System;
using System.Linq;
using System.Data;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class DashboardController : Controller
    {
        private Db _dblayer;
        private DataTable _dt;

        //List<SubModules> SubModuleList = new List<SubModules>();

        // GET: Dashboard
        public ActionResult Dashboard()
        {
            _dblayer = new Db(GlobalVariables.Entity);
            _dt = _dblayer.GetAccessRights(Session["UserRole"].ToString());
            var view = new DataView(_dt);
            var dst = view.ToTable(true, "ModuleName");

            var results = (from row in dst.AsEnumerable()
                select new Modules
                {
                    ModuleName = row.Field<string>("ModuleName")
                }).Distinct();

            var modules = results.ToList();
            Session["Modules"] = modules;
            return View();
        }

        [NonAction]
        public void GetSubModules(string module)
        {
            _dblayer = new Db(GlobalVariables.Entity);
            _dt = _dblayer.GetAccessRights(Session["UserRole"].ToString());
            var view = new DataView(_dt);
            var dst = view.ToTable(true, "ModuleName", "SubModule", "Controller");

            var results = (from row in dst.AsEnumerable()
                           //where row.Field<string>("ModuleName") == module
                           select new SubModules
                           {
                               ModuleName = row.Field<string>("ModuleName"),
                               SubModule = row.Field<string>("SubModule"),
                               Controller = row.Field<string>("Controller")
                           }).Distinct();

            //var num = results.Count();
            //System.Diagnostics.Debug.WriteLine(num.ToString());

            //List<SubModules> submodules = results.ToList();
            var submodules = results.ToList();
            // Session["SubModule"] = results;
            Session["SubModule"] = submodules;
        }

        [NonAction]
        public void GetMenus(string submodule)
        {
            _dblayer = new Db(GlobalVariables.Entity);
            _dt = _dblayer.GetAccessRights(Session["UserRole"].ToString());
            //var results = (from row in dt.AsEnumerable()
            //               where row.Field<string>("SubModule") == submodule
            //               select row.Field<string>("MenuDescription")).Distinct();

            //var results = (from row in dt.AsEnumerable()
            //               where row.Field<string>("SubModule") == submodule
            //               select row.Field<string>("MenuDescription")).Distinct();

            //var results = (from menu in _dt.AsEnumerable()
            //                where menu.Field<string>("SubModule") == submodule
            //                select new Menus
            //                {
            //                    MenuDescription = menu.Field<string>("MenuDescription"),
            //                    ActionName = menu.Field<string>("ActionName")
            //                }).Distinct();

            //List<Menus> menus = results.ToList();
            //Session["Menus"] = results;

            //var view = new DataView(_dt);
            //var dst = view.ToTable(true, "ModuleName");

            var results = (from menu in _dt.AsEnumerable()
                           //where menu.Field<string>("SubModule") == submodule
                           select new Menus
                           {
                                SubModule = menu.Field<string>("SubModule"),
                                MenuDescription = menu.Field<string>("MenuDescription"),
                                ActionName = menu.Field<string>("ActionName")
                            }).Distinct();

            var menus = results.ToList();

            Session["Menus"] = menus;
        }

        public struct Modules
        {
            public string ModuleName;
        }
    }
}