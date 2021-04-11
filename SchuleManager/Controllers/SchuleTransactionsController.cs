using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;
using System.Linq.Dynamic;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SchuleManager.Controllers
{
    public class SchuleTransactionsController : Controller
    {
        private static CreateErrorLogs _clf;
        private static readonly string ErrorLogPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogs/"));

        private Db _dblayer;

        #region Bill Students

        [HttpGet]
        public ActionResult BillStudents()
        {
            GetFeesProducts();

            ViewBag.ProductItems = new SelectList(new List<ProductItems>(), "ItemCode", "Description");

            GetAcademicTerms();

            ViewBag.SystemDate = Session["LoginDate"].ToString();

            return View();
        }

        public ActionResult LoadStudentsToBill(int academicId, string productCode, string feeCode)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var studentBillData = ShowStudentsToBill(sortColumn, sortColumnDir, searchValue, academicId, productCode, feeCode);
            var recordsTotal = studentBillData.Count();
            var data = studentBillData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<StudentsToBill> ShowStudentsToBill(string sortColumn, string sortColumnDir, string search, int academicId, string productCode, string feeCode)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var studentsToBill = _dblayer.GetStudentsToBill(academicId, productCode, feeCode);

            var studentsToBillData = (from row in studentsToBill.AsEnumerable()
                select new StudentsToBill
                {
                    AccountID = row.Field<string>("AccountID"),
                    StudentName = row.Field<string>("StudentName"),
                    SchSectionID = row.Field<string>("SchSectionID"),
                    FormSectionID = row.Field<string>("FormSectionID"),
                    FormCode = row.Field<string>("FormCode"),
                    SpecialFees = row.Field<string>("SpecialFees"),
                    ExpectedBill = row.Field<decimal>("ExpectedBill"),
                    NetBillAmt = row.Field<decimal>("NetBillAmt"),
                    AmountToBill = row.Field<decimal>("AmountToBill"),
                    Flag = row.Field<string>("Flag")
                }).AsQueryable();

            var queryableBillList = studentsToBillData;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableBillList = queryableBillList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableBillList =
                    queryableBillList.Where(m => m.AccountID.Contains(search.ToUpper()) ||
                                                 m.StudentName.Contains(search.ToUpper()));
            }

            return queryableBillList;
        }

        [NonAction]
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
                    ProductTypeCode = row.Field<string>("ProductTypeCode"),
                }).Distinct().Where(p => p.ProductTypeCode == "F");

            var lstFeesProducts = productCodes.ToList();

            ViewBag.FeesProducts = new SelectList(lstFeesProducts, "ProductCode", "ProductName");

        }

        [NonAction]
        public void GetAcademicTerms()
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtAcademicTerms = _dblayer.GetAcademicTerms();
            var dtViewAcademicTerms = new DataView(dtAcademicTerms);
            var dstAcademicTerms = dtViewAcademicTerms.ToTable(true, "AcademicID", "AcademicTerm");

            var academicTerms = (from row in dstAcademicTerms.AsEnumerable()
                select new AcademicTerms()
                {
                    AcademicID = row.Field<int>("AcademicID"),
                    AcademicTerm = row.Field<string>("AcademicTerm")
                }).Distinct();

            var lstAcademicTerms = academicTerms.ToList();

            ViewBag.AcademicTerms = new SelectList(lstAcademicTerms, "AcademicID", "AcademicTerm");
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

            var lstProductItems = productItems.ToList();

            return Json(lstProductItems, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductContraGl(string productCode, string feeCode)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtProductContraGl = _dblayer.GetProductContraGl(productCode, feeCode);
            var dtViewProductContraGl = new DataView(dtProductContraGl);
            var dstProductContraGl = dtViewProductContraGl.ToTable(true, "AccountID", "AccountName", "ClearBalance");

            var productContraGl = (from row in dstProductContraGl.AsEnumerable()
                select new GlAccounts
                {
                    AccountID = row.Field<string>("AccountID"),
                    AccountName = row.Field<string>("AccountName"),
                    ClearBalance = row.Field<decimal>("ClearBalance")
                }).Distinct();

            return Json(productContraGl, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Fees Cash Payments

        [HttpGet]
        public ActionResult FeesCashPayments()
        {

            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            //ViewBag.SystemDate = Convert.ToDateTime(DateTime.ParseExact("28-06-2019", "dd-MM-yyyy", CultureInfo.CurrentCulture));

            var model = new FeesCashPaymentsViewModel
            {
                CashierGl = _dblayer.GetCashierGl(Session["Username"].ToString()),
                StudentAccount = new Account()
                {
                    ClearBalance = (decimal)0.00
                },
                Transactions = new Transactions()
                {
                    TranCode = "001",
                    TranCodeDescription = "CASH CREDIT",
                    TranDate = Convert.ToDateTime(Session["LoginDate"]),
                    TranAmount = (decimal)0.00
                }
            };

            ViewBag.SystemDate = Session["LoginDate"].ToString();

            return View(model);
        }

        [HttpGet]
        public JsonResult LoadBillDetails(string accountId)
        {
            var data = ShowBillDetails(accountId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        public IQueryable<StudentBills> ShowBillDetails(string accountId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var studentBillsView = _dblayer.GetStudentOsBills(accountId);

            var studentBillDetails = (from row in studentBillsView.AsEnumerable()
                select new StudentBills
                {
                    BillID = row.Field<decimal>("BillID"),
                    BillDate = row.Field<DateTime>("BillDate").ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),
                    FeeCode = row.Field<string>("FeeCode"),
                    TermID = row.Field<int>("TermID"),
                    Description = row.Field<string>("Description"),
                    OSAmount = row.Field<decimal>("OSAmt"),
                    AmountPaid = row.Field<decimal>("AmountPaid")
                }).AsQueryable();

            return studentBillDetails;

        }

        #endregion

        #region Fees Bank Payments

        [HttpGet]
        public ActionResult FeesBankPayments()
        {
            var model = new FeesBankPaymentsViewModel
            {
                StudentAccount = new Account()
                {
                    ClearBalance = (decimal)0.00
                },
                Transactions = new Transactions()
                {
                    TranCode = "003",
                    TranCodeDescription = "TRANSFER CREDIT",
                    TranDate = Convert.ToDateTime(Session["LoginDate"]),
                    TranAmount = (decimal)0.00
                }
            };

            ViewBag.SystemDate = Session["LoginDate"].ToString();
            return View(model);
        }

        #endregion

        #region View Transactions

        [HttpGet]
        public ActionResult ViewTransactions()
        {
            GetTranCategories();

            return View();
        }

        [NonAction]
        public void GetTranCategories()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dtTranCategories = _dblayer.GetTranCategories();
            var dtViewTranCategories = new DataView(dtTranCategories);
            var dstTranCategories = dtViewTranCategories.ToTable(true, "CatCode", "Category");

            var tranCategories = (from row in dstTranCategories.AsEnumerable()
                select new TranCategory
                {
                    CatCode = row.Field<string>("CatCode"),
                    Category = row.Field<string>("Category")
                }).Distinct();

            var lstTranCategories = tranCategories.ToList();

            ViewBag.TranCategories = new SelectList(lstTranCategories, "CatCode", "Category");
        }

        public ActionResult LoadTransactionsView(string tranCat, string status, string postedby = "", char delTran = 'N')
        {
            var data = ShowTransactionsView(tranCat, status, postedby, delTran);

            return Json(new { message = "success", data });
        }

        public IQueryable<DailyTranDetails> ShowTransactionsView(string tranCat, string status, string postedby = "", char delTran = 'N')
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var transactionsView = _dblayer.GetTransactionsView(tranCat, status, postedby);

            if (delTran == 'Y')
            {
                var transactionsViewData = (from row in transactionsView.AsEnumerable()
                    select new DailyTranDetails
                    {
                        TranID = row.Field<decimal>("TranID"),
                        TranSerialNo = row.Field<decimal>("SerialNo"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        TranAmount = row.Field<decimal>("Amount"),
                        PartTranType = row.Field<string>("DRCR"),
                        DeletedBy = row.Field<string>("DeletedBy"),
                        DelFlag = row.Field<string>("DelFlag"),
                        TranCat = row.Field<string>("Cat"),
                        TranType = row.Field<string>("Type"),
                        DelVerifiedBy = row.Field<string>("DelVerifiedBy")
                    }).AsQueryable();

                return transactionsViewData;
            }
            else
            {
                var transactionsViewData = (from row in transactionsView.AsEnumerable()
                    select new DailyTranDetails
                    {
                        TranID = row.Field<decimal>("TranID"),
                        CreatedBy = row.Field<string>("CreatedBy"),
                        TotalDebit = row.Field<decimal>("TotalDebit"),
                        TotalCredit = row.Field<decimal>("TotalCredit"),
                        Flag = row.Field<string>("Flag"),
                        TranCat = row.Field<string>("Cat"),
                        TranType = row.Field<string>("Type")
                    }).AsQueryable();

                return transactionsViewData;
            }
        }

        public ActionResult LoadTransactionDetails(int tranId, DateTime? opDate = null)
        {
            var data = ShowTransactionDetails(tranId, opDate);

            return Json(new { message = "success", data });
        }

        public IQueryable<Transactions> ShowTransactionDetails(int tranId, DateTime? opDate = null)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var transactionDetailsView = _dblayer.GetTransactionDetails(tranId, opDate);

            var transactionDetails = (from row in transactionDetailsView.AsEnumerable()
                select new Transactions
                {
                    PartTranType = row.Field<string>("D/C") == "D" ? "DEBIT" : "CREDIT",
                    TranType = row.Field<string>("TranType"),
                    AccountID = row.Field<string>("AccountID"),
                    AccountType = row.Field<string>("AcctType"),
                    AccountName = row.Field<string>("AccountName"),
                    ProductCode = row.Field<string>("ProdID"),
                    TranAmount = row.Field<decimal>("Amount"),
                    TranSerialNo = row.Field<decimal>("SerialNo"),
                    TranParticulars = row.Field<string>("Particulars"),
                    TranRemarks = row.Field<string>("TranRemarks"),
                }).AsQueryable();

            return transactionDetails;

        }

        [HttpPost]
        public ActionResult VerifyTransactions(string tranIDs)
        {
            var tranIdList = tranIDs.Split(',');

            var isEmpty = !tranIdList.Any();

            var result = 0;

            if (!isEmpty)
            {
                _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

                foreach (var tranId in tranIdList)
                {
                    result += _dblayer.SuperviseTransactions(Convert.ToDateTime(Session["LoginDate"].ToString()), Convert.ToDecimal(tranId), Session["Username"].ToString());
                }
            }

            if (result > 0)
            {
                return Json(new { message = "Transaction Verified Successfully" }, JsonRequestBehavior.AllowGet);
            }
            switch (result)
            {
                case -10:
                    return Json(new { message = "Working Date is Greater than Transaction Date" }, JsonRequestBehavior.AllowGet);
                case -20:
                    return Json(new { message = "You cannot supervise a transaction you created/modified" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Transaction Verification has failed" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Cash Transactions

        public ActionResult CashTransactions()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var model = new CashTransactionsViewModel
            {
                CashierAccount = _dblayer.GetCashierGl(Session["Username"].ToString()),
                CashTransaction = new Transactions
                {
                    TranCode = "001",
                    TranCodeDescription = "CASH CREDIT",
                    ValueDate = Convert.ToDateTime(Session["LoginDate"]),
                    TranAmount = (decimal)0.00
                }
            };

            ViewBag.SystemDate = Session["LoginDate"].ToString();
            //Debug.WriteLine(Session["LoginDate"].ToString().Substring(0,9));

            return View(model);
        }

        #endregion

        #region Cash Register

        public ActionResult CashRegister()
        {
            var model = new CashRegister()
            {
                TranDate = Session["LoginDate"].ToString().Substring(0, 10)
            };

            return View(model);
        }

        public ActionResult LoadCashRegister(DateTime fromDate, DateTime toDate, string partTranType, string operatorId, string cashiergl)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var cashRegisterData = ShowCashRegister(sortColumn, sortColumnDir, fromDate, toDate, partTranType, operatorId, cashiergl);
            var recordsTotal = cashRegisterData.Count();
            var data = cashRegisterData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });

            //var data = ShowCashRegister(fromDate, toDate, partTranType, operatorId, cashiergl);

            //return Json(new { message = "success", data });
        }

        public IQueryable<CashRegister> ShowCashRegister(string sortColumn, string sortColumnDir, DateTime fromDate, DateTime toDate, string partTranType, string operatorId, string cashiergl)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var cashRegister = _dblayer.GetCashRegister(fromDate, toDate, partTranType, operatorId, cashiergl);

            var cashRegisterDetails = (from row in cashRegister.AsEnumerable()
                select new CashRegister
                {
                    TranDate = row.Field<string>("TranDate"),
                    Tran_SrNo = row.Field<string>("Tran/SrNo"),
                    ProductCode = row.Field<string>("ProductCode"),
                    AccountID = row.Field<string>("AccountID"),
                    AccountName = row.Field<string>("AccountName"),
                    Amount = row.Field<decimal>("Amount"),
                    PartTranType = row.Field<string>("Trx"),
                    OperatorID = row.Field<string>("OperatorID"),
                    SupervisorID = row.Field<string>("SupervisorID"),
                    CashierGL = row.Field<string>("CashierGL"),
                    Narration = row.Field<string>("Narration"),
                }).AsQueryable();

            var queryableRegisterDetails = cashRegisterDetails;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableRegisterDetails = queryableRegisterDetails.OrderBy(sortColumn + " " + sortColumnDir);
            }

            return queryableRegisterDetails;
        }

        //public void ExportCashRegisterToExcel(DateTime fromDate, DateTime toDate, char partTranType, string operatorId = "", string cashiergl = "")
        public void ExportCashRegisterToExcel(FormCollection formCollection)
        {
            //var fromDate = Convert.ToDateTime("22/03/2020");
            //var toDate = Convert.ToDateTime("22/03/2020");
            //const char partTranType = 'B';
            //const string operatorId = "";
            //const string cashiergl = "";

            var fromDate = Convert.ToDateTime(formCollection["fromDate"]);
            var toDate = Convert.ToDateTime(formCollection["toDate"]);
            var partTranType = formCollection["TranType"];
            var operatorId = formCollection["OperatorID"];
            var cashiergl = formCollection["CashierGL"];

            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var cashRegister = _dblayer.GetCashRegister(fromDate, toDate, partTranType, operatorId, cashiergl);

            var cashRegisterDetails = (from dataRow in cashRegister.AsEnumerable()
                select new CashRegister
                {
                    TranDate = dataRow.Field<string>("TranDate"),
                    Tran_SrNo = dataRow.Field<string>("Tran/SrNo"),
                    ProductCode = dataRow.Field<string>("ProductCode"),
                    AccountID = dataRow.Field<string>("AccountID"),
                    AccountName = dataRow.Field<string>("AccountName"),
                    Amount = dataRow.Field<decimal>("Amount"),
                    PartTranType = dataRow.Field<string>("Trx"),
                    OperatorID = dataRow.Field<string>("OperatorID"),
                    SupervisorID = dataRow.Field<string>("SupervisorID"),
                    CashierGL = dataRow.Field<string>("CashierGL"),
                    Narration = dataRow.Field<string>("Narration"),
                }).AsQueryable();

            try
            {
                var ep = new ExcelPackage();
                var sheet = ep.Workbook.Worksheets.Add("CashRegister");
                sheet.Cells["A1"].Value = "Transaction Date";
                sheet.Cells["B1"].Value = "Transaction No.";
                sheet.Cells["C1"].Value = "Product";
                sheet.Cells["D1"].Value = "Account ID";
                sheet.Cells["E1"].Value = "Account Name";
                sheet.Cells["F1"].Value = "Amount";
                sheet.Cells["G1"].Value = "Trx";
                sheet.Cells["H1"].Value = "Operator ID";
                sheet.Cells["I1"].Value = "Supervisor ID";
                sheet.Cells["J1"].Value = "Cashier GL";
                sheet.Cells["K1"].Value = "Narration";

                var row = 2;

                foreach (var item in cashRegisterDetails)
                {
                    sheet.Cells[$"A{row}"].Value = item.TranDate;
                    sheet.Cells[$"B{row}"].Value = item.Tran_SrNo;
                    sheet.Cells[$"C{row}"].Value = item.ProductCode;
                    sheet.Cells[$"D{row}"].Value = item.AccountID;
                    sheet.Cells[$"E{row}"].Value = item.AccountName;
                    sheet.Cells[$"F{row}"].Value = item.Amount;
                    sheet.Cells[$"G{row}"].Value = item.PartTranType;
                    sheet.Cells[$"H{row}"].Value = item.OperatorID;
                    sheet.Cells[$"I{row}"].Value = item.SupervisorID;
                    sheet.Cells[$"J{row}"].Value = item.CashierGL;
                    sheet.Cells[$"K{row}"].Value = item.Narration;
                    row++;
                }


                sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "CashRegister.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();

            }
            catch (Exception ex)
            {
                _clf = new CreateErrorLogs(Session["Username"] + "_error_log_");
                _clf.CreateErrorLog(ErrorLogPath, "Error at ExportCashRegisterToExcel method: " + ex);
            }
        }

        public void ExportCashRegisterToPdf(FormCollection formCollection)
        {
            var fromDate = Convert.ToDateTime(formCollection["fromDate"]);
            var toDate = Convert.ToDateTime(formCollection["toDate"]);
            var partTranType = formCollection["TranType"];
            var operatorId = formCollection["OperatorID"];
            var cashiergl = formCollection["CashierGL"];

            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var cashRegister = _dblayer.GetCashRegister(fromDate, toDate, partTranType, operatorId, cashiergl);

            try
            {
                var document = new Document(new Rectangle(PageSize.A4.Rotate())
                {
                    BackgroundColor = BaseColor.WHITE
                });

                PdfWriter.GetInstance(document, System.Web.HttpContext.Current.Response.OutputStream);

                var transactionTableLayout = new PdfPTable(11);

                var paragraph1 = new Paragraph("SCHULE MANAGER")
                {
                    Alignment = 1,
                    Font = new Font(Font.FontFamily.HELVETICA, 16f)
                };
                var paragraph2 = paragraph1;
                var paragraph3 = new Paragraph("CASH REGISTER")
                {
                    Alignment = 1,
                    Font = new Font(Font.FontFamily.HELVETICA, 11f)
                };
                var paragraph4 = paragraph3;
                var paragraph5 = new Paragraph("Cashier GL.: " + cashiergl)
                {
                    Alignment = 0,
                    Font = new Font(Font.FontFamily.COURIER, 9f)
                };
                var paragraph6 = paragraph5;
                var paragraph7 = new Paragraph("Account Currency: " + operatorId)
                {
                    Alignment = 0,
                    Font = new Font(Font.FontFamily.COURIER, 9f)
                };
                var paragraph8 = paragraph7;
                var paragraph9 = new Paragraph(" ");

                document.Open();
                document.Add(paragraph9);
                document.Add(paragraph2);
                document.Add(paragraph4);
                document.Add(paragraph9);
                document.Add(paragraph9);
                document.Add(paragraph9);
                document.Add(paragraph6);
                document.Add(paragraph8);
                document.Add(paragraph9);
                document.Add(AddTransactionContent(transactionTableLayout, cashRegister));
                document.Close();

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename= CashRegister.pdf");
                System.Web.HttpContext.Current.Response.Write(document);
                Response.Flush();
                Response.End();

            }
            catch (Exception ex)
            {
                _clf = new CreateErrorLogs(Session["Username"] + "_error_log_");
                _clf.CreateErrorLog(ErrorLogPath, "Error at ExportCashRegisterToPdf method: " + ex);
            }

        }

        #region Export Cash Register PDF Methods
        private static PdfPTable AddTransactionContent(PdfPTable transactionTableLayout, DataTable dt)
        {
            var relativeWidths = new[]
            {
                35f,
                25f,
                35f,
                40f,
                50f,
                35f,
                25f,
                35f,
                35f,
                35f,
                70f
            };
            transactionTableLayout.SetWidths(relativeWidths);
            transactionTableLayout.DefaultCell.FixedHeight = 50f;
            transactionTableLayout.WidthPercentage = 100f;
            transactionTableLayout.HorizontalAlignment = 2;
            AddCellToHeader(transactionTableLayout, "Transaction Date");
            AddCellToHeader(transactionTableLayout, "Tran/SrNo");
            AddCellToHeader(transactionTableLayout, "Product");
            AddCellToHeader(transactionTableLayout, "Account ID");
            AddCellToHeader(transactionTableLayout, "Account Name");
            AddCellToHeader(transactionTableLayout, "Amount");
            AddCellToHeader(transactionTableLayout, "Trx");
            AddCellToHeader(transactionTableLayout, "Operator ID");
            AddCellToHeader(transactionTableLayout, "Supervisor ID");
            AddCellToHeader(transactionTableLayout, "Cashier GL");
            AddCellToHeader(transactionTableLayout, "Narration");
            foreach (var dataRow in dt.Rows.Cast<DataRow>().Where(row => dt.Rows.Count > 0))
            {
                AddTextToBody(transactionTableLayout, dataRow["TranDate"].ToString());
                AddTextToBody(transactionTableLayout, dataRow["Tran/SrNo"].ToString());
                AddTextToBody(transactionTableLayout, dataRow["ProductCode"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["AccountID"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["AccountName"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["Amount"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["Trx"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["OperatorID"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["SupervisorID"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["CashierGL"].ToString());
                AddNumbersToBody(transactionTableLayout, dataRow["Narration"].ToString());
            }
            return transactionTableLayout;
        }

        private static void AddCellToHeader(PdfPTable transactionTableLayout, string cellText)
        {
            var pdfPtable = transactionTableLayout;
            var pdfPcell = new PdfPCell(new Phrase(cellText,
                new Font(Font.FontFamily.HELVETICA, 8f, 1, BaseColor.WHITE)))
            {
                HorizontalAlignment = 0,
                Padding = 5f,
                BackgroundColor = new BaseColor(1, 33, 197)
            };
            var cell = pdfPcell;
            pdfPtable.AddCell(cell);
        }

        private static void AddTextToBody(PdfPTable transactionTableLayout, string cellText)
        {
            var pdfPtable = transactionTableLayout;
            var pdfPcell = new PdfPCell(new Phrase(cellText,
                new Font(Font.FontFamily.HELVETICA, 7f, 1, BaseColor.BLACK)))
            {
                HorizontalAlignment = 0,
                Padding = 5f,
                BackgroundColor = BaseColor.WHITE
            };
            var cell = pdfPcell;
            pdfPtable.AddCell(cell);
        }

        private static void AddNumbersToBody(PdfPTable transactionTableLayout, string cellText)
        {
            var pdfPtable = transactionTableLayout;
            var pdfPcell = new PdfPCell(new Phrase(cellText,
                new Font(Font.FontFamily.HELVETICA, 8f, 1, BaseColor.BLACK)))
            {
                HorizontalAlignment = 2,
                Padding = 5f,
                BackgroundColor = BaseColor.WHITE
            };
            var cell = pdfPcell;
            pdfPtable.AddCell(cell);
        }


        #endregion

        #endregion

        #region Internal Transfers

        public ActionResult InternalTransfers()
        {
            var model = new InternalTransfersViewModel
            {
                InternalTransfer = new Transactions
                {
                    TranCode = "003",
                    TranCodeDescription = "TRANSFER CREDIT",
                    ValueDate = Convert.ToDateTime(Session["LoginDate"]),
                    TranAmount = (decimal)0.00
                }
            };

            ViewBag.SystemDate = Session["LoginDate"].ToString();
            return View(model);
        }


        #endregion


        [HttpPost]
        public ActionResult AddEditTransactions(char tranType, char tranCat, List<NewTrans> trans, char action, string glSubHead = "", string feeCode = "", bool isSysTran = false, bool isFaTrans = false, string faProdCode = "", int tranId = 0)
        {
            var transDatatable = ConvertDataSet.ListToDataTable(trans);

            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditTransactions(Convert.ToDateTime(Session["LoginDate"].ToString()), tranType, tranCat, transDatatable, Session["Username"].ToString(), action, glSubHead, feeCode, isSysTran, isFaTrans, faProdCode, tranId);

            if (result > 0)
            {
                return Json(new { success = true, message = "Transaction Successful, Tran ID: " + result }, JsonRequestBehavior.AllowGet);
            }
            if (result != -10)
            {
                if (result != -20)
                {
                    if (result != -30)
                    {
                        if (result != -40)
                        {
                            if (result != -50)
                            {
                                if (result != -60)
                                {
                                    if (result != -70)
                                    {
                                        if (result != -80 && result != -90)
                                        {
                                            if (result != -100)
                                            {
                                                if (result != -110)
                                                {
                                                    if (result == -120)
                                                    {
                                                        return Json(
                                                            new
                                                            {
                                                                success = false,
                                                                message =
                                                                "You cannot edit a system generated Transaction."
                                                            },
                                                            JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                                else
                                                {
                                                    return Json(
                                                        new
                                                        {
                                                            success = false,
                                                            message =
                                                            "Only the User who Created this transaction can Edit it."
                                                        },
                                                        JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                            else
                                            {
                                                return Json(new { success = true, message = "Unbalanced Transaction." },
                                                    JsonRequestBehavior.AllowGet);
                                            }
                                        }
                                        else
                                        {
                                            return Json(
                                                new
                                                {
                                                    success = false,
                                                    message =
                                                    "You Do Not Have Sufficient Funds To Perform the Transaction. Transaction Halted"
                                                }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        return Json(
                                            new
                                            {
                                                success = false,
                                                message =
                                                "The Account Is Not Allowed to go into DEBIT. Transaction Halted"
                                            },
                                            JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    return Json(
                                        new
                                        {
                                            success = false,
                                            message =
                                            "Cash Transactions Not Allowed on Loan Accounts. Transaction Halted"
                                        }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(
                                    new
                                    {
                                        success = false,
                                        message =
                                        "You cannot perform Transaction on Your GL Account. Transaction Halted"
                                    }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Cashier's GL Not Found. Transaction Halted" },
                                JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(
                            new { success = false, message = "Working Date is Greater than Transaction Date -  INVALID." },
                            JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Day Closed, CANNOT CONTINUE" },
                        JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, message = "End of Day is Running, CANNOT CONTINUE" },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Transation Failed, Contact System Administrator" }, JsonRequestBehavior.AllowGet);
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
                        DebitTurnOver = row.Field<decimal>("DebitTurnOver")

                    }).AsQueryable();

                return Json(glAccountDetails, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var studentAccountDetails = (from row in accountDetails.AsEnumerable()
                    select new Account
                    {
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
                        DebitTurnOver = row.Field<decimal>("DebitTurnOver")

                    }).AsQueryable();

                return Json(studentAccountDetails, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAccountBalance(string accountId, char accountType, bool isVerified, bool isCashierGl)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            //var accountBalance = _dblayer.GetAccountBalance(accountId, accountType, isVerified, isCashierGl);
            var data = new
            {
                balance = _dblayer.GetAccountBalance(accountId, accountType, isVerified, isCashierGl)
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUnsupervisedDrcr(string accountId, char accountType, char drcr)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            //var amount = _dblayer.GetUnsupervisedDrcr(accountId, accountType, drcr);
            var data = new
            {
                amount = _dblayer.GetUnsupervisedDrcr(accountId, accountType, drcr)
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }       

        [HttpGet]
        public JsonResult GetTotalCreditsDebits(string accountId, char partTranType)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            //var totalDebitsCredits = _dblayer.GetTotalCreditsDebits(accountId, partTranType);
            var data = new
            {
                amount = _dblayer.GetTotalCreditsDebits(accountId, partTranType)
            };

            return Json(data, JsonRequestBehavior.AllowGet);
        }


    }
}