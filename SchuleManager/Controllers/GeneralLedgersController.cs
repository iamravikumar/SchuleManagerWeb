using System.Data;
using System.Web.Mvc;
using System.Linq;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class GeneralLedgersController : Controller
    {
        private Db _dblayer;

        [HttpGet]
        public ActionResult GlAccountDetails()
        {
            GetGlTypes();

            GetGlSubTypes();

            var model = new GlAccounts
            {
                Balance = 0,
                UnSupervisedCredit = 0,
                CreditTurnOver = 0,
                DebitTurnOver = 0
            };

            return View(model);
        }

        public void GetGlTypes()
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtGlTypes = _dblayer.GetGlTypes();
            var dtViewGlTypes = new DataView(dtGlTypes);
            var dstClientTypes = dtViewGlTypes.ToTable(true, "GLTypeCode", "GLType", "GLShortCode");

            var glTypes = (from row in dstClientTypes.AsEnumerable()
                select new GlTypes
                {
                    GLTypeCode = row.Field<string>("GLTypeCode"),
                    GLType = row.Field<string>("GLType"),
                    GLShortCode = row.Field<string>("GLShortCode")                    
                }).Distinct();

            var lstGlTypes = glTypes.ToList();

            ViewBag.GlType = new SelectList(lstGlTypes, "GLTypeCode", "GLType", "GLShortCode");
        }

        public void GetGlSubTypes()
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtGlSubTypes = _dblayer.GetGlSubTypes();
            var dtViewGlSubTypes = new DataView(dtGlSubTypes);
            var dstGlSubTypes = dtViewGlSubTypes.ToTable(true, "GLSubTypeCode", "GLSubType", "GLTypeCode");

            var glsubTypes = (from row in dstGlSubTypes.AsEnumerable()
                select new GlSubTypes
                {
                    GLSubTypeCode = row.Field<string>("GLSubTypeCode"),
                    GLSubType = row.Field<string>("GLSubType"),
                    GLTypeCode = row.Field<string>("GLTypeCode")
                }).Distinct();

            var lstGlSubTypes = glsubTypes.ToList();

            ViewBag.GlSubType = new SelectList(lstGlSubTypes, "GLSubTypeCode", "GLSubType", "GLTypeCode");
        }

        public JsonResult GetGlSubTypesByGlType(char gltype)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtGlSubTypes = _dblayer.GetGlSubTypesByGlType(gltype);
            var dtViewGlSubTypes = new DataView(dtGlSubTypes);
            var dstGlSubTypes = dtViewGlSubTypes.ToTable(true, "GLSubTypeCode", "GLSubType");

            var glsubTypes = (from row in dstGlSubTypes.AsEnumerable()
                select new GlAccounts()
                {
                    AccountSubTypeCode = row.Field<string>("GLSubTypeCode"),
                    AccountSubType = row.Field<string>("GLSubType")
                }).Distinct();

            var lstGlSubTypes = glsubTypes.ToList();

            return Json(lstGlSubTypes, JsonRequestBehavior.AllowGet);
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
        public ActionResult AddEditGl(GlAccounts account, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditGl(account, action, Session["Username"].ToString());

            if (result > 0)
            {
                if (action == 'A')
                {
                    return Json(new {success = true, message = "New GL Account Added Successfully", newAccountId = result}, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Account Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }                      
            return Json(new { success = false, message = "Add or Update Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult VerifyGlAccount(string accountId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.VerifyAccount(accountId, 'G', Session["Username"].ToString());

            if (result > 0)
            {
                return Json(new { success = true, message = "GL Account has been verified successfully" }, JsonRequestBehavior.AllowGet);               
            }
            return Json(result == -10 ? new { success = false, message = "Same user can not verify the account" } : new { success = false, message = "Account Verification failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CloseGlAccount(string accountId, string closureReason)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.CloseAccount(accountId, closureReason, 'G', Session["Username"].ToString());

            if (result > 0)
            {
                return Json(new { success = true, message = "GL Account has been closed successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Account Closure failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GlSubTypes()
        {
            GetGlTypes();

            return View();
        }

        public ActionResult LoadGlSubTypes(char glType)
        {
            var data = ShowGlSubTypes(glType);

            return Json(new { message = "success", data });
        }

        public IQueryable<GlSubTypes> ShowGlSubTypes(char glType)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var glSubTypes = _dblayer.GetGlSubTypesByGlType(glType);

            var glSubTypesData = (from row in glSubTypes.AsEnumerable()
                select new GlSubTypes
                {
                    GLSubTypeCode = row.Field<string>("GLSubTypeCode"),
                    GLSubType = row.Field<string>("GLSubType"),
                    GLTypeCode = row.Field<string>("GLTypeCode"),
                    Deleted = row.Field<string>("Deleted")
                }).AsQueryable();

            return glSubTypesData;
        }

        [HttpPost]
        public ActionResult AddEditGlSubTypes(GlSubTypes glSubTypes, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditGlSubTypes(glSubTypes, action, Session["Username"].ToString());

            if (result > 0)
            {
                if (action == 'A')
                {
                    return Json(new {success = true, message = "GL Subtype Added Successfully", response = result},
                        JsonRequestBehavior.AllowGet);
                }
                return Json(action == 'E' ? new { success = true, message = "GL Subtype Updated Successfully", response = result } : new { success = true, message = "GL Subtype Deleted Successfully", response = result }, JsonRequestBehavior.AllowGet);
            }           
            return Json(new { success = false, message = "Additon or Update Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GlParameters()
        {
            return View();
        }

        public ActionResult LoadGlParameters()
        {
            var data = ShowGlParameters();

            return Json(new { message = "success", data });
        }

        public IQueryable<GlParameters> ShowGlParameters()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var parameters = _dblayer.GetGlParameters();

            var parameterData = (from row in parameters.AsEnumerable()
                select new GlParameters
                {
                    SerialID = row.Field<int>("SerialID"),
                    Description = row.Field<string>("Description"),
                    AccountID = row.Field<string>("GLAccountID"),
                    AccountType = row.Field<string>("GLTypeCode"),
                    AccountName = row.Field<string>("GLAccountName")
                }).AsQueryable();

            return parameterData;
        }

        [HttpPost]
        public ActionResult EditGlParameters(GlParameters glParameters)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.EditGlParameters(glParameters, Session["Username"].ToString());

            if (result > 0)
            {
                return Json(new { success = true, message = "GL Parameter Updated Successfully", response = result }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Update Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
        }

    }
}