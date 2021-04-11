using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class FixedAssetsController : Controller
    {
        private Db _dblayer;

        // GET: FixedAssets
        public ActionResult FixedAssetMaintenance()
        {
            GetSuppliers();

            return View();
        }

        public void GetSuppliers()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dtSuppliers = _dblayer.GetStaticData('L');
            var dtViewSuppliers = new DataView(dtSuppliers);
            var dstSuppliers = dtViewSuppliers.ToTable(true, "SDCode", "Description");

            var suppliers = (from row in dstSuppliers.AsEnumerable()
                select new StaticData
                {
                    StaticDataCode = row.Field<string>("SDCode"),
                    Description = row.Field<string>("Description")
                }).Distinct();

            var lstSuppliers = suppliers.ToList();

            ViewBag.SupplierID = new SelectList(lstSuppliers, "StaticDataCode", "Description");
        }

        [HttpGet]
        public JsonResult GetFixedAssetDetails(string assetId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var assetDataTable = _dblayer.GetFixedAssetDetails(assetId);

            var assetDetails = (from row in assetDataTable.AsEnumerable()
                select new FixedAssets
                {
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    Description = row.Field<string>("Description"),
                    AssetIDPrefix = row.Field<string>("AssetIDPrefix"),
                    SupplierID = row.Field<string>("SupplierID"),
                    SerialNo = row.Field<string>("SerialNo"),
                    TagNo = row.Field<string>("TagNo"),
                    BrandName = row.Field<string>("BrandName"),
                    Location = row.Field<string>("Location"),
                    DepMthd = row.Field<string>("DepMthd"),
                    DepRate = row.Field<decimal>("DepRate"),
                    CostPrice = row.Field<decimal>("CostPrice"),
                    Terms = row.Field<int>("Terms"),
                    PurchasedOn = row.Field<DateTime>("PurchasedOn").ToShortDateString(),
                    DepFrom = row.Field<DateTime>("DepFrom").ToShortDateString(),
                    IsVerified = row.Field<bool>("IsVerified"),
                    NetBookValue = row.Field<decimal>("NetBookValue"),
                    AccumDep = row.Field<decimal>("AccumDep"),
                    RemainingTerms = row.Field<int>("RemainingTerms")
                }).AsQueryable();

            return Json(assetDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFixedAssetProductDetails(string productCode)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var fixedAssetProductDatatable = _dblayer.GetFixedAssetProductDetails(productCode);

            var fixedAssetProducts = (from row in fixedAssetProductDatatable.AsEnumerable()
                select new FixedAssetsProducts
                {
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    AssetIDPrefix = row.Field<string>("AssetIDPrefix"),
                    AllowCredits = row.Field<bool>("AllowCredits"),
                    CanGoInCredit = row.Field<bool>("CanGoInCredit"),
                    AllowDebits = row.Field<bool>("AllowDebits"),
                    CanGoInDebit = row.Field<bool>("CanGoInDebit"),
                    ControlAccountGL = row.Field<string>("ControlAccountGL"),
                    ControlAccountGLName = row.Field<string>("ControlAccountGLName"),
                    AccumDepGL = row.Field<string>("AccumDepGL"),
                    AccumDepGLName = row.Field<string>("AccumDepGLName"),
                    DepExpenseGL = row.Field<string>("DepExpenseGL"),
                    DepExpenseGLName = row.Field<string>("DepExpenseGLName"),
                    SaleoffLossGL = row.Field<string>("SaleoffLossGL"),
                    SaleoffLossGLName = row.Field<string>("SaleoffLossGLName"),
                    SaleoffProfitGL = row.Field<string>("SaleoffProfitGL"),
                    SaleoffProfitGLName = row.Field<string>("SaleoffProfitGLName")
                }).AsQueryable();

            return Json(fixedAssetProducts, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult AddEditFixedAssets(FixedAssets asset, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditFixedAssets(asset, action, Session["Username"].ToString());

            return result == null ? Json(new {success = false, message = "Add or Update Failed, Contact System Administrator"},JsonRequestBehavior.AllowGet) : Json(action == 'A' ? new { success = true, message = result } : new { success = true, message = "Fixed Asset Details Updated Successfully" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FixedAssetProducts()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEditFixedAssetsProducts(FixedAssetsProducts product, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditFixedAssetsProducts(product, action, Session["Username"].ToString());

            if (result > 0)
            {
                return action == 'A'
                    ? Json(new { success = true, message = "Fixed Asset Product Added Successfully" }, JsonRequestBehavior.AllowGet)
                    : Json(new { success = true, message = "Fixed Asset Product Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Add or Updated Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Disposal()
        {
            return View();
        }
    }
}