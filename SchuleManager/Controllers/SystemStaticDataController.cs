using System.Data;
using System.Linq;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class SystemStaticDataController : Controller
    {
        private Db _dblayer;

        [HttpGet]
        public ActionResult StaticDataMaintenance()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadStaticData(char staticDataType)
        {
            var data = ShowStaticData(staticDataType);

            return Json(new { message = "success", data });
        }

        public IQueryable<StaticData> ShowStaticData(char staticDataType)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var sdDataTable = _dblayer.GetStaticData(staticDataType);

            var staticData = (from row in sdDataTable.AsEnumerable()
                select new StaticData
                {
                    StaticDataCode = row.Field<string>("SDCode"),
                    Description = row.Field<string>("Description"),
                    Deleted = row.Field<bool>("Deleted")
                }).AsQueryable();

            return staticData;
        }

        [HttpPost]
        public ActionResult AddEditStaticData(StaticData staticData, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditStaticData(staticData, action, Session["Username"].ToString());

            if (result > 0 && result < 2627)
            {
                if (action == 'A')
                {
                    return Json(new { success = true, message = "Static Data Code Added Successfully" },
                        JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Static Data Code Updated Successfully" } , JsonRequestBehavior.AllowGet);
            }
            return Json(result == 2627 ? new { success = false, message = "Static Data Code Already Exists" } : new { success = false, message = "Add or Update Failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Classes()
        {
            GetSysForms();

            GetClassStreams();

            return View();
        }

        [HttpPost]
        public ActionResult LoadClassesData()
        {
            var data = ShowClasses();

            return Json(new { message = "success", data });
        }

        public IQueryable<Classes> ShowClasses()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var classesDt = _dblayer.GetClasses();

            var classes = (from row in classesDt.AsEnumerable()
                select new Classes
                {
                    ClassCode = row.Field<string>("ClassCode").Trim(),
                    Class = row.Field<string>("Class"),
                    FormCode = row.Field<string>("FormCode").Trim(),
                    StreamCode = row.Field<string>("StreamCode").Trim()
                }).AsQueryable(); 

            return classes;
        }

        [NonAction]
        public void GetSysForms()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dt = _dblayer.GetSysForms();
            var view = new DataView(dt);
            var dst = view.ToTable(true, "FormCode", "Form", "FormSectionID");

            var results = (from row in dst.AsEnumerable()
                select new SysForms
                {
                    FormCode = row.Field<string>("FormCode").Trim(),
                    Form = row.Field<string>("Form"),
                    FormSectionId = row.Field<string>("FormSectionID")
                }).Distinct();

            var forms = results.ToList();

            ViewBag.Forms = new SelectList(forms, "FormCode", "Form");
        }

        [NonAction]
        public void GetClassStreams()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dt = _dblayer.GetClassStreams();
            var view = new DataView(dt);
            var dst = view.ToTable(true, "StreamCode", "Stream");

            var results = (from row in dst.AsEnumerable()
                select new ClassStreams()
                {
                    StreamCode = row.Field<string>("StreamCode"),
                    Stream = row.Field<string>("Stream")
                }).Distinct().Where(m => m.Deleted.Equals(false));

            var streams = results.ToList();

            ViewBag.Streams = new SelectList(streams, "StreamCode", "Stream");

        }

        public JsonResult CheckStaticDataCodeExists(char staticDataType, string staticCode)
        {
            if (staticCode == null) return Json(false);
            _dblayer = new Db(GlobalVariables.Entity);

            var staticDataCodeExists = _dblayer.StatiDataCodeExists(staticDataType, staticCode);

            return Json(staticDataCodeExists);
        }

    }
}