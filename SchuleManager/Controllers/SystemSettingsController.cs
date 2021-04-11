using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class SystemSettingsController : Controller    
    {
        private Db _dblayer;

        [HttpGet]
        public ActionResult SchoolInformation()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var model = _dblayer.GetSchoolInfo();

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateSchoolInfo(SystemSettings schoolInfo)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.UpdateSchoolInfo(schoolInfo, Session["Username"].ToString());

            if (result > 0)
            {
                return Json(new { success = true, message = "School Information Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "School Information Update Failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSchoolInfo()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var data = _dblayer.GetSchoolInfo();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SystemHolidays()
        {
            ViewBag.SystemDate = Convert.ToDateTime(Session["LoginDate"]);

            return View();
        }

        public ActionResult LoadSystemHolidays()
        {
            var data = ShowSystemHolidays();

            return Json(new { data });
        }

        public IQueryable<SystemHolidays> ShowSystemHolidays()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var shDatatable = _dblayer.GetSystemHolidays();

            var systemHolidays = (from row in shDatatable.AsEnumerable()
                select new SystemHolidays
                {
                    SHolidayDate = row.Field<DateTime>("HolidayDate").ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),
                    Remarks = row.Field<string>("Remarks")
                }).AsQueryable();

            return systemHolidays;
        }

        public ActionResult AddEditSystemHolidays(SystemHolidays holiday, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditSystemHolidays(holiday, action, Session["Username"].ToString());

            if (result > 0)
            {
                return Json(action != 'A' ? new {success = true, message = "Public Holiday deleted successfully"} : new {success = true, message = "Public Holiday added successfully"}, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Add or Delete failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

    }
}