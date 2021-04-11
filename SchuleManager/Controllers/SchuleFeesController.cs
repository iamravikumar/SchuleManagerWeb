using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class SchuleFeesController : Controller
    {
        private Db _dblayer;

        [HttpGet]
        public ActionResult FeesMaintenance()
        {
            GetSysData();

            GetFeesProducts();

            ViewBag.ProductItems = new SelectList(new List<ProductItems>(), "ItemCode", "Description");

            return View();
        }

        public void GetFeesProducts()
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtProducts = _dblayer.GetProducts();
            var dtViewProducts = new DataView(dtProducts);
            var dstProducts = dtViewProducts.ToTable(true, "ProductCode", "ProductName", "ProductTypeCode");

            var productCodes = (from row in dstProducts.AsEnumerable()
                select new Products
                {
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    ProductTypeCode = row.Field<string>("ProductTypeCode")
                }).Distinct().Where(p => p.ProductTypeCode == "F");

            var lstFeesProducts = productCodes.ToList();

            ViewBag.FeesProducts = new SelectList(lstFeesProducts, "ProductCode", "ProductName");
        }

        public JsonResult GetProductItems(string productCode)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtProductItems = _dblayer.GetProductItems(productCode);
            var dtViewProductItems = new DataView(dtProductItems);
            var dstProductItems = dtViewProductItems.ToTable(true, "ItemCode", "Description");

            var productItems = (from row in dstProductItems.AsEnumerable()
                select new ProductItems
                {
                    ItemCode = row.Field<string>("ItemCode"),
                    Description = row.Field<string>("Description")
                }).Distinct();

            return Json(productItems.ToList(), JsonRequestBehavior.AllowGet);
        }

        public void GetSysData()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dtSysForms = _dblayer.GetSysData('F');
            var dtViewSysForms = new DataView(dtSysForms);
            var dstSysForms = dtViewSysForms.ToTable(true, "FormCode", "Form");

            var sysForms = (from row in dstSysForms.AsEnumerable()
                select new SysForms
                {
                    FormCode = row.Field<string>("FormCode").Trim(),
                    Form = row.Field<string>("Form")
                }).Distinct();

            var lstSysForms = sysForms.ToList();


            var dtSysTerms = _dblayer.GetSysData('T');
            var dtViewSysTerms = new DataView(dtSysTerms);
            var dstSysTerms = dtViewSysTerms.ToTable(true, "TermID", "Term");

            var sysTerms = (from row in dstSysTerms.AsEnumerable()
                select new SysTerms
                {
                    TermID = row.Field<int>("TermID"),
                    Term = row.Field<string>("Term")
                }).Distinct();

            var lstSysTerms = sysTerms.ToList();

            var dtSections = _dblayer.GetSysData('S');
            var dtViewSections = new DataView(dtSections);
            var dstSections = dtViewSections.ToTable(true, "SchSectionID", "SchoolSection");

            var sysSchoolSections = (from row in dstSections.AsEnumerable()
                select new SysSchoolSections
                {
                    SchSectionID = row.Field<string>("SchSectionID"),
                    SchoolSection = row.Field<string>("SchoolSection")
                }).Distinct();

            var lstSysSchSections = sysSchoolSections.ToList();

            ViewBag.SysForms = new SelectList(lstSysForms, "FormCode", "Form");
            ViewBag.SysTerms = new SelectList(lstSysTerms, "TermID", "Term");
            ViewBag.SysSchSections = new SelectList(lstSysSchSections, "SchSectionID", "SchoolSection");

        }

        public ActionResult LoadTermFees(string productCode, string feeCode, string schSectionId, int year, int termId)
        {
            var data = ShowTermFees(productCode, feeCode, schSectionId, year, termId);

            return Json(new { data });
        }

        public IQueryable<TermFees> ShowTermFees(string productCode, string feeCode, string schSectionId, int year, int termId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var termFeesDataTable = _dblayer.GetTermFees(productCode, feeCode, schSectionId, year, termId);

            var termFeesData = (from row in termFeesDataTable.AsEnumerable()
                select new TermFees
                {
                    AcademicID = row.Field<int>("AcademicID"),
                    FormCode = row.Field<string>("FormCode").Trim(),
                    FormDescription = row.Field<string>("Form"),
                    Amount = row.Field<decimal>("Amount")
                }).AsQueryable();

            return termFeesData;
        }

        [HttpPost]
        public ActionResult AddEditTermFees(TermFees fee, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditTermFees(fee, action, Session["Username"].ToString());

            if (result > 0 && result < 2627)
            {
                if (action == 'A')
                {
                    return Json(new { success = true, message = "Term Fees Added Successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Term Fees Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result == 2627 ? new { success = false, message = "Duplicate" } : new { success = false, message = "Add or Update Failed" }, JsonRequestBehavior.AllowGet);
        }

    }
}