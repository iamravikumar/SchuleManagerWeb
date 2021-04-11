using System.Data;
using System.Linq;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class ProductsController : Controller
    {
        private Db _dblayer;

        [HttpGet]
        public ActionResult ProductMaintenance()
        {
            GetClientTypes();

            GetProductTypes();

            return View();
        }

        [NonAction]
        public void GetClientTypes()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dt = _dblayer.GetClientTypes();
            var view = new DataView(dt);
            var dst = view.ToTable(true, "ClientTypeCode", "ClientType");

            var clientTypeData = (from row in dst.AsEnumerable()
                select new ClientTypes
                {
                    ClientTypeCode = row.Field<string>("ClientTypeCode"),
                    ClientType = row.Field<string>("ClientType")
                }).Distinct();

            var lstClientTypes = clientTypeData.ToList();

            ViewBag.ClientType = new SelectList(lstClientTypes, "ClientTypeCode", "ClientType");
        }

        [NonAction]
        public void GetProducts()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dt = _dblayer.GetProducts();
            var view = new DataView(dt);
            var dst = view.ToTable(true, "ProductCode", "ProductName");

            var productData = (from row in dst.AsEnumerable()
                select new Products
                {
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName")
                }).Distinct();

            var lstProducts = productData.ToList();

            ViewBag.Products = new SelectList(lstProducts, "ProductCode", "ProductName");
        }

        [NonAction]
        public void GetProductTypes()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dt = _dblayer.GetProductTypes();
            var view = new DataView(dt);
            var dst = view.ToTable(true, "ProductTypeCode", "ProductType");

            var productTypeData = (from row in dst.AsEnumerable()
                select new ProductTypes
                {
                    ProductTypeCode = row.Field<string>("ProductTypeCode"),
                    ProductType = row.Field<string>("ProductType")
                }).Distinct();

            var lstProductTypes = productTypeData.ToList();

            ViewBag.ProductType = new SelectList(lstProductTypes, "ProductTypeCode", "ProductType");
        }

        [HttpGet]
        public JsonResult GetProductTypesByClientType(char clientType)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dtProductTypes = _dblayer.GetProductTypesByClientType(clientType);
            var dtViewProductTypes = new DataView(dtProductTypes);
            var dstProductTypes = dtViewProductTypes.ToTable(true, "ProductTypeCode", "ProductType");

            var productTypes = (from row in dstProductTypes.AsEnumerable()
                select new Products()
                {
                    ProductTypeCode = row.Field<string>("ProductTypeCode"),
                    ProductType = row.Field<string>("ProductType")
                }).Distinct();

            var lstProductTypes = productTypes.ToList();

            return Json(lstProductTypes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProductDetails(string productCode)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var productDetailsDatatable = _dblayer.GetProductDetails(productCode);

            var productDetails = (from row in productDetailsDatatable.AsEnumerable()
                select new Products
                {
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    ClientTypeCode = row.Field<string>("ClientTypeCode"),
                    ProductTypeCode = row.Field<string>("ProductTypeCode"),
                    AccountPrefix = row.Field<string>("AccountPrefix"),
                    AllowCredits = row.Field<bool>("AllowCredits"),
                    CanGoInCredit = row.Field<bool>("CanGoInCredit"),
                    AllowDebits = row.Field<bool>("AllowDebits"),
                    CanGoInDebit = row.Field<bool>("CanGoInDebit"),
                    ControlAccountGL = row.Field<string>("ControlAccountGL"),
                    ControlAccountGLName = row.Field<string>("ControlAccountGLName"),
                    ControlAccountType = row.Field<string>("ControlAccountType"),
                    ContraAccountGL = row.Field<string>("ContraAccountGL"),
                    ContraAccountGLName = row.Field<string>("ContraAccountGLName"),
                    ContraAccountType = row.Field<string>("ContraAccountType"),
                    PAndLAccountGL = row.Field<string>("PAndLAccountGL"),
                    PAndLAccountGLName = row.Field<string>("PAndLAccountGLName"),
                    PAndLAccountType = row.Field<string>("PAndLAccountType")
                }).AsQueryable();

            return Json(productDetails, JsonRequestBehavior.AllowGet);

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
        public ActionResult AddEditProducts(Products product, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditProducts(product, action, Session["Username"].ToString());

            if (result > 0 && result < 2627)
            {
                if (action == 'A')
                {
                    return Json(new {success = true, message = "Product Added Successfully"},
                        JsonRequestBehavior.AllowGet);
                }
                return Json(action == 'E' ? new {success = true, message = "Product updated Successfully"} : new { success = true, message = "Product deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(result == 2627 ? new { success = false, message = "Product Already Exists" } : new { success = false, message = "Add or Update Failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ProductItems()
        {
            GetProducts();

            return View();
        }

        public ActionResult LoadProductItems(string productCode)
        {
            var data = ShowProductItems(productCode);

            return Json(new { message = "success", data });
        }

        public IQueryable<ProductItems> ShowProductItems(string productCode)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var productItems = _dblayer.GetProductItems(productCode);

            var itemsData = (from row in productItems.AsEnumerable()
                select new ProductItems()
                {
                    ItemCode = row.Field<string>("ItemCode"),
                    Description = row.Field<string>("Description"),
                    PAndLAccountGL = row.Field<string>("PAndLAccountGL"),
                    PAndLAccountGLName = row.Field<string>("PAndLAccountGLName"),
                    PAndLAccountType = row.Field<string>("PAndLAccountType"),
                    ContraAccountGL = row.Field<string>("ContraAccountGL"),
                    ContraAccountGLName = row.Field<string>("ContraAccountGLName"),
                    ContraAccountType = row.Field<string>("ContraAccountType"),
                    ApplyTo = row.Field<string>("ApplyTo")
                }).AsQueryable();

            return itemsData;
        }

        [HttpPost]
        public ActionResult AddEditProductItems(ProductItems item, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditProductItems(item, action, Session["Username"].ToString());

            if (result > 0)
            {
                switch (action)
                {
                    case 'A':
                        return Json(new {success = true, message = "Product Item Added Successfully"},
                            JsonRequestBehavior.AllowGet);
                    case 'E':
                        return Json(new {success = true, message = "Product Item Updated Successfully"},
                            JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Product Item Deleted Successfully" },
                    JsonRequestBehavior.AllowGet);
            }
       
            return Json(new { success = false, message = "Add or Updated Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

  
    }
}