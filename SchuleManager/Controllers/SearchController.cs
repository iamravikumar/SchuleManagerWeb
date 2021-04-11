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
    public class SearchController : Controller
    {
        private Db _dblayer;

        #region General Search

        public ActionResult HelpGeneralSearch(char searchFlag)
        {
            ViewBag.SearchFlag = searchFlag;

            switch (searchFlag)
            {
                case 'G':
                    return PartialView("_GeneralGLSearch");
                case 'P':
                    return PartialView("_ParentSearch");
                case 'S':
                    return PartialView("_StudentSearch");
                case 'A':
                    return PartialView("_GeneralAccountSearch");
                case 'O':
                    return PartialView("_ProductSearch");
                case 'U':
                    return PartialView("_UserSearch");
                case 'R':
                    return PartialView("_RoleSearch");
                case 'C':
                    return PartialView("_GeneralGLSearch");
                case 'F':
                    return PartialView("_FixedAssetProductSearch");
            }
            return Json(
                new
                {
                    success = false,
                    message = "Failed to load Search Window, Kindly Contact the System Administrator"
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadGeneralSearchData(char flag)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form
                .GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]")
                .FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var searchData = ShowGeneralSearchData(sortColumn, sortColumnDir, searchValue, flag);
            var recordsTotal = searchData.Count();
            var data = searchData.Skip(skip).Take(pageSize).ToList();

            return Json(new {draw, recordsFiltered = recordsTotal, recordsTotal, data});
        }

        public IQueryable<object> ShowGeneralSearchData(string sortColumn, string sortColumnDir, string search, char searchFlag)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var searchDataTable = _dblayer.HelpGeneralSearch(searchFlag);

            IQueryable<object> resultList = null;

            if (searchFlag == 'P')
            {
                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new Parents
                    {
                        ParentID = row.Field<string>("ParentID"),
                        TitleCode = row.Field<string>("TitleCode"),
                        SurName = row.Field<string>("SurName"),
                        OtherNames = row.Field<string>("OtherNames"),
                        Dateofbirth = row.Field<DateTime>("Dateofbirth"),
                        Gender = row.Field<string>("GenderID"),
                        NationalityCode = row.Field<string>("NationalityCode"),
                        EducationCode = row.Field<string>("EducationCode"),
                        RAddress = row.Field<string>("RAddress"),
                        RPhone1 = row.Field<string>("RPhone1")
                    }).AsQueryable();

                var queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.SurName.Contains(search.ToUpper()) ||
                                                     m.OtherNames.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'S')
            {
                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new Students
                    {
                        StudentID = row.Field<string>("StudentID"),
                        SurName = row.Field<string>("SurName"),
                        OtherNames = row.Field<string>("OtherNames"),
                        ClassCode = row.Field<string>("ClassCode")
                    }).AsQueryable();

                var queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.SurName.Contains(search.ToUpper()) ||
                                                     m.OtherNames.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'A')
            {
                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new Account()
                    {
                        AccountID = row.Field<string>("AccountID"),
                        AccountName = row.Field<string>("AccountName"),
                        ClientID = row.Field<string>("ClientID"),
                        ClientType = row.Field<string>("ClientType"),
                        ProductCode = row.Field<string>("ProductCode"),
                        ProductName = row.Field<string>("ProductName"),
                        ClearBalance = row.Field<decimal>("ClearBalance"),
                        LienAmount = row.Field<decimal>("LienAmount"),
                        FrozenAmount = row.Field<decimal>("FrozenAmount"),
                        CreditTurnOver = row.Field<decimal>("CreditTurnOver"),
                        DebitTurnOver = row.Field<decimal>("DebitTurnOver"),
                        UnSupervisedCredit = row.Field<decimal>("UnSupervisedCredit"),
                        UnSupervisedDebit = row.Field<decimal>("UnSupervisedDebit")
                    }).AsQueryable();

                var queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.AccountName.Contains(search.ToUpper()) ||
                                                     m.AccountID.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'G')
            {
                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new GlAccounts
                    {
                        AccountID = row.Field<string>("AccountID"),
                        AccountName = row.Field<string>("AccountName"),
                        AccountType = row.Field<string>("AccountType"),
                        AccountSubType = row.Field<string>("AccountSubType"),
                        Balance = row.Field<decimal>("Balance"),
                        UnSupervisedCredit = row.Field<decimal>("UnSupervisedCredit"),
                        UnSupervisedDebit = row.Field<decimal>("UnSupervisedDebit"),
                        CreditTurnOver = row.Field<decimal>("CreditTurnOver"),
                        DebitTurnOver = row.Field<decimal>("DebitTurnOver"),
                        IsVerified = row.Field<bool>("IsVerified")
                    }).AsQueryable();

                var queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.AccountName.Contains(search.ToUpper()) ||
                                                     m.AccountID.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'C')
            {
                searchDataTable = _dblayer.HelpGeneralSearch('G');

                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new GlAccounts
                    {
                        AccountID = row.Field<string>("AccountID"),
                        AccountName = row.Field<string>("AccountName"),
                        AccountType = row.Field<string>("AccountType"),
                        AccountSubType = row.Field<string>("AccountSubType"),
                        Balance = row.Field<decimal>("Balance"),
                        UnSupervisedCredit = row.Field<decimal>("UnSupervisedCredit"),
                        UnSupervisedDebit = row.Field<decimal>("UnSupervisedDebit"),
                        CreditTurnOver = row.Field<decimal>("CreditTurnOver"),
                        DebitTurnOver = row.Field<decimal>("DebitTurnOver"),
                        IsVerified = row.Field<bool>("IsVerified")
                    }).AsQueryable().Where(m =>m.AccountSubType.Equals("A01"));

                var queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList = queryableDataList.Where(m => m.AccountName.Contains(search.ToUpper()) || m.AccountID.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'O')
            {
                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new Products
                    {
                        ProductCode = row.Field<string>("ProductCode"),
                        ProductName = row.Field<string>("ProductName"),
                        ClientType = row.Field<string>("ClientType"),
                        ProductType = row.Field<string>("ProductType"),
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
                        PAndLAccountType = row.Field<string>("PAndLAccountType"),
                        ClientTypeCode = row.Field<string>("ClientTypeCode"),
                        ProductTypeCode = row.Field<string>("ProductTypeCode"),
                    }).AsQueryable();

                var queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.ProductCode.Contains(search.ToUpper()) ||
                                                     m.ProductName.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'U')
            {
                var searchDataAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new SystemUsers()
                    {
                        UserName = row.Field<string>("UserName"),
                        SurName = row.Field<string>("SurName"),
                        OtherNames = row.Field<string>("OtherNames")
                    }).AsQueryable();

                var queryableDataList = searchDataAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.UserName.Contains(search.ToUpper()) ||
                                                     m.OtherNames.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'R')
            {
                var searchDataAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new SystemRoles
                    {
                        RoleCode = row.Field<string>("RoleCode"),
                        Description = row.Field<string>("Description")
                    }).AsQueryable();

                var queryableDataList = searchDataAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.RoleCode.Contains(search.ToUpper()) ||
                                                     m.Description.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }
            else if (searchFlag == 'F')
            {
                var searchDataAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new FixedAssetsProducts
                    {
                        ProductCode = row.Field<string>("ProductCode"),
                        ProductName = row.Field<string>("ProductName"),
                        AssetIDPrefix = row.Field<string>("AssetIDPrefix")
                    }).AsQueryable();

                var queryableDataList = searchDataAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList =
                        queryableDataList.Where(m => m.ProductCode.Contains(search.ToUpper()) ||
                                                     m.ProductName.Contains(search.ToUpper()));
                }

                resultList = queryableDataList;

            }

            return resultList;
        }
        
        #endregion

        #region Account Search

        public ActionResult HelpAccountSearch(char accountType)
        {
            ViewBag.AccountType = accountType;

            return PartialView("_AccountSearch");
        }

        public ActionResult LoadAccountSearchData(char accountType)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var searchData = ShowAccountSearchData(sortColumn, sortColumnDir, searchValue, accountType);
            var recordsTotal = searchData.Count();
            var data = searchData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<object> ShowAccountSearchData(string sortColumn, string sortColumnDir, string search, char accountType)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var searchDataTable = _dblayer.HelpAccountSearch(accountType);

            var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()

            select new Account
            {
                AccountID = row.Field<string>("AccountID"),
                AccountName = row.Field<string>("AccountName"),
                ProductCode = row.Field<string>("ProductCode"),
                ProductName = row.Field<string>("ProductName"),
                ClientID = row.Field<string>("ClientID"),
                ClientType = row.Field<string>("ClientType"),
                ClearBalance = row.Field<decimal>("ClearBalance"),
                LienAmount = row.Field<decimal>("LienAmount"),
                FrozenAmount = row.Field<decimal>("FrozenAmount"),
                CreditTurnOver = row.Field<decimal>("CreditTurnOver"),
                DebitTurnOver = row.Field<decimal>("DebitTurnOver"),
                UnSupervisedCredit = row.Field<decimal>("UnSupervisedCredit"),
                UnSupervisedDebit = row.Field<decimal>("UnSupervisedDebit")
            }).AsQueryable();

            var queryableDataList = searchDatAsQueryable;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableDataList = queryableDataList.Where(m => m.AccountName.Contains(search.ToUpper()) || m.AccountID.Contains(search.ToUpper()));
            }

            IQueryable<object> resultList = queryableDataList;

            return resultList;
        }

        #endregion

        #region Schule Account Search

        public ActionResult HelpSchuleAccountSearch()
        {

            return PartialView("_SchuleAccountSearch");
        }

        public ActionResult LoadSchuleAccountSearchData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var searchData = ShowSchuleAccountSearchData(sortColumn, sortColumnDir, searchValue);
            var recordsTotal = searchData.Count();
            var data = searchData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<Account> ShowSchuleAccountSearchData(string sortColumn, string sortColumnDir, string search)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var searchDataTable = _dblayer.HelpSchuleAccountSearch();

            var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()

                select new Account
                {
                    ClientType = row.Field<string>("ClientType"),
                    ClientID = row.Field<string>("ClientID"),
                    ProductName = row.Field<string>("ProductName"),
                    AccountID = row.Field<string>("AccountID"),
                    ProductCode = row.Field<string>("ProductCode"),
                    AccountName = row.Field<string>("AccountName"),
                }).AsQueryable();

            var queryableDataList = searchDatAsQueryable;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableDataList = queryableDataList.Where(m => m.AccountName.Contains(search.ToUpper()) || m.AccountID.Contains(search.ToUpper()));
            }

            return queryableDataList;
        }

        #endregion

        #region General Ledger Search

        public ActionResult HelpGeneralLedgerSearch(char searchFlag)
        {
            switch (searchFlag)
            {
                case 'P':
                    return PartialView("_PandlGlSearch");
                case 'C':
                    return PartialView("_ControlGlSearch ");
                case 'A':
                    return PartialView("_ContraGlSearch");
            }
            return Json(
                new
                {
                    success = false,
                    message = "Failed to load Search Window, Kindly Contact the System Administrator"
                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadGlSearchData(char flag, char glTypes)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var searchData = ShowGlSearchData(sortColumn, sortColumnDir, searchValue, flag, glTypes);
            var recordsTotal = searchData.Count();
            var data = searchData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<GlAccounts> ShowGlSearchData(string sortColumn, string sortColumnDir, string search, char searchFlag, char glTypes)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            IQueryable<GlAccounts> queryableDataList;

            var searchDataTable = _dblayer.HelpGeneralSearch(searchFlag);

            if (glTypes == 'P')
            {
                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new GlAccounts()
                    {
                        AccountID = row.Field<string>("AccountID"),
                        AccountName = row.Field<string>("AccountName"),
                        AccountType = row.Field<string>("AccountType")
                    }).AsQueryable().Where(g => g.AccountType.Contains('E') || g.AccountType.Contains('I'));

                queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList = queryableDataList.Where(m => m.AccountID.Contains(search.ToUpper()) || m.AccountName.Contains(search.ToUpper()));
                }
            }
            else
            {
                var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                    select new GlAccounts
                    {
                        AccountID = row.Field<string>("AccountID"),
                        AccountName = row.Field<string>("AccountName"),
                        AccountType = row.Field<string>("AccountType")
                    }).AsQueryable().Where(g => g.AccountType.Contains('L') || g.AccountType.Contains('A'));

                queryableDataList = searchDatAsQueryable;

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    queryableDataList = queryableDataList.Where(m => m.AccountID.Contains(search.ToUpper()) || m.AccountName.Contains(search.ToUpper()));
                }
            }

            return queryableDataList;
        }

        public ActionResult GeneralLedgerSearch(char searchFlag)
        {
            switch (searchFlag)
            {
                case 'P':
                    return PartialView("_PandlGlSearch");
                case 'L':
                    return PartialView("_ContraGlSearch");
            }
            return Json(new { success = false, message = "Failed to load Search Window, Kindly Contact the System Administrator" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Client Type Product Search

        public ActionResult HelpClientTypeProductSearch(char clientType)
        {
            ViewBag.ClientType = clientType;

            return PartialView("_ClientTypeProductSearch");
        }

        public ActionResult LoadClientProductSearchData(char clientType)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var searchData = ShowClientProductSearchData(sortColumn, sortColumnDir, searchValue, clientType);
            var recordsTotal = searchData.Count();
            var data = searchData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<Products> ShowClientProductSearchData(string sortColumn, string sortColumnDir, string search, char clientType)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var clientProductsDatatable = _dblayer.HelpClientTypeProductSearch(clientType);

            var clientProducts = (from row in clientProductsDatatable.AsEnumerable()
                select new Products
                {
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    ProductType = row.Field<string>("ProductType")
                }).AsQueryable();


            var queryableDataList = clientProducts;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableDataList = queryableDataList.Where(m => m.ProductCode.Contains(search.ToUpper()) || m.ProductName.Contains(search.ToUpper()));
            }

            return queryableDataList;
        }

        #endregion

        #region Fixed Asset Search

        public ActionResult HelpFixedAssetSearch()
        {
            return PartialView("_FixedAssetSearch");
        }

        public ActionResult LoadFixedAssetSearchData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var searchData = ShowFixedAssetSearchData(sortColumn, sortColumnDir, searchValue);
            var recordsTotal = searchData.Count();
            var data = searchData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<FixedAssets> ShowFixedAssetSearchData(string sortColumn, string sortColumnDir, string search)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var searchDataTable = _dblayer.HelpFixedAssetSearch();

            var searchDatAsQueryable = (from row in searchDataTable.AsEnumerable()
                select new FixedAssets
                {
                    AssetID = row.Field<string>("AssetID"),
                    Description = row.Field<string>("Description"),
                    ProductCode = row.Field<string>("ProductCode")
                }).AsQueryable().Where(a => a.AssetID.Contains(search.ToUpper()) || a.Description.Contains(search.ToUpper()));

            var queryableDataList = searchDatAsQueryable;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableDataList = queryableDataList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableDataList = queryableDataList.Where(m => m.AssetID.Contains(search.ToUpper()) || m.Description.Contains(search.ToUpper()));
            }

            return queryableDataList;
        }

        #endregion


    }
}