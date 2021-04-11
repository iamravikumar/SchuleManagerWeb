using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class AccountsController : Controller
    {
        private Db _dblayer;

        [HttpGet]
        public ActionResult AccountDetails()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerifyAccount(string accountId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.VerifyAccount(accountId, 'C', Session["Username"].ToString());

            return result > 0 ? Json(new { success = true, message = "The account has been verified successfully" }, JsonRequestBehavior.AllowGet) : Json(result == -10 ? new { success = false, message = "Same User can not create and verify account" } : new { success = false, message = "Account Verification Failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAccountDetails(string accountId, char accountType, bool isCashierGl = false)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var accountDetails = _dblayer.GetAccountDetails(accountId, accountType, isCashierGl);

            if (accountType == 'G')
            {
                var glAccountDetails = (from row in accountDetails.AsEnumerable()
                    select new GlAccounts
                    {
                        AccountID = row.Field<string>("AccountID"),
                        AccountName = row.Field<string>("AccountName"),
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
            var studentAccountDetails = (from row in accountDetails.AsEnumerable()
                select new Account
                {
                    ClientType = row.Field<string>("ClientType"),
                    ClientID = row.Field<string>("ClientID"),
                    AccountID = row.Field<string>("AccountID"),
                    AccountName = row.Field<string>("AccountName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    ClearBalance = row.Field<decimal>("ClearBalance"),
                    LienAmount = row.Field<decimal>("LienAmount"),
                    FrozenAmount = row.Field<decimal>("FrozenAmount"),
                    UnSupervisedCredit = row.Field<decimal>("UnSupervisedCredit"),
                    UnSupervisedDebit = row.Field<decimal>("UnSupervisedDebit"),
                    CreditTurnOver = row.Field<decimal>("CreditTurnOver"),
                    DebitTurnOver = row.Field<decimal>("DebitTurnOver"),
                    IsVerified =  row.Field<bool>("IsVerified"),
                    ClosedBy = row.Field<string>("ClosedBy")

                }).AsQueryable();

            return Json(studentAccountDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CloseAccount(string accountId, string closeReason)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.CloseAccount(accountId, closeReason, 'C', Session["Username"].ToString());

            if (result > 0)
            {
                return Json(new { message = "The accounts have been closed successfully" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "Account closure failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult BatchVerification()
        {
            return View();
        }

        public ActionResult LoadUnverifiedAccounts(string productCode)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var accountData = ShowUnVerifedAccounts(sortColumn, sortColumnDir, searchValue, productCode);
            var recordsTotal = accountData.Count();
            var data = accountData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<Account> ShowUnVerifedAccounts(string sortColumn, string sortColumnDir, string search, string productCode)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var account = _dblayer.GetUnVerifiedAccounts(Session["Username"].ToString(), 'C', productCode);

            var accountData = (from row in account.AsEnumerable()
                select new Account
                {
                    AccountID = row.Field<string>("AccountID"),
                    AccountName = row.Field<string>("AccountName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    CreatedBy = row.Field<string>("CreatedBy")
                }).AsQueryable();

            var queryableAccountList = accountData;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableAccountList = queryableAccountList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableAccountList = queryableAccountList.Where(m => m.AccountID.Contains(search.ToUpper()) || m.AccountName.Contains(search.ToUpper()));
            }

            return queryableAccountList;
        }

        [HttpPost]
        public ActionResult BatchVerification(string accounts)
        {
            var accountList = accounts.Split(',');

            //var accountList = new List<string>(accounts.Values);

            var isEmpty = !accountList.Any();

            var result = 0;

            if (!isEmpty)
            {
                _dblayer = new Db(GlobalVariables.Entity);

                foreach (var account in accountList)
                {
                    result += _dblayer.VerifyAccount(account, 'C', Session["Username"].ToString());
                }
            }

            if (result > 0)
            {
                return Json(new { message = "The accounts have been verified successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Account verification failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GenerateAccounts()
        {
            ViewBag.SystemDate = Session["LoginDate"].ToString();

            return View();
        }

        public ActionResult LoadClientWithoutAccounts(char clientType, string productCode)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var clientData = ShowClientWithoutAccounts(sortColumn, sortColumnDir, searchValue, clientType, productCode);
            var recordsTotal = clientData.Count();
            var data = clientData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<object> ShowClientWithoutAccounts(string sortColumn, string sortColumnDir, string search, char clientType, string productCode)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var clientsDataTable = _dblayer.GetClientsWithoutAccounts(clientType, productCode);

            if(clientType == 'S')
            {
                var clientData = (from row in clientsDataTable.AsEnumerable()
                    select new Students()
                    {
                        StudentID = row.Field<string>("StudentID"),
                        SurName = row.Field<string>("SurName"),
                        OtherNames = row.Field<string>("OtherNames"),
                        ClassCode = row.Field<string>("ClassCode")
                    }).AsQueryable();

                var queryableClientList = clientData;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableClientList = queryableClientList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableClientList = queryableClientList.Where(m => m.SurName.Contains(search.ToUpper()) || m.OtherNames.Contains(search.ToUpper()) || m.StudentID.Contains(search.ToUpper()));
                }

                return queryableClientList;
            }

            return null;
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
                    ContraAccountGL = row.Field<string>("ContraAccountGL"),
                    ContraAccountGLName = row.Field<string>("ContraAccountGLName"),
                    PAndLAccountGL = row.Field<string>("PAndLAccountGL"),
                    PAndLAccountGLName = row.Field<string>("PAndLAccountGLName")
                }).AsQueryable();

            return Json(productDetails, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GenerateAccounts(string clients, string productCode, string openDate)
        {
            var clientList = clients.Split(',');

            var accountOpenDate = Convert.ToDateTime(openDate);

            var isEmpty = !clientList.Any();

            var result = 0;

            if (!isEmpty)
            {
                _dblayer = new Db(GlobalVariables.Entity);

                foreach (var client in clientList)
                {
                    result += _dblayer.GenerateAccount(client, productCode, accountOpenDate, Session["Username"].ToString());
                }
            }

            if (result > 0)
            {
                return Json(new { message = "The accounts have been generated successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Account generation failed" }, JsonRequestBehavior.AllowGet);
        }


    }
}