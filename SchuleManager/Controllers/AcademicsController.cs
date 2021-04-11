using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class AcademicsController : Controller
    {
        private Db _dblayer;

        private static CreateErrorLogs _clf;
        private static readonly string ErrorLogPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogs/"));

        // GET: Academics
        public ActionResult AcademicPeriods()
        {
            return View();
        }

        public ActionResult LoadAcademicPeriods(int year)
        {
            var data = ShowAcademicPeriods(year);

            return Json(new { message = true, data });
        }

        public IQueryable<AcademicTerms> ShowAcademicPeriods(int year)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var academicPeriods = _dblayer.GetAcademicPeriods(year);

            var periodsData = (from row in academicPeriods.AsEnumerable()
                select new AcademicTerms()
                {
                    AcademicID = row.Field<int>("AcademicID"),
                    TermID = row.Field<int>("TermID"),
                    SFromDate = row.Field<DateTime>("FromDate").ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),
                    SToDate = row.Field<DateTime>("ToDate").ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),
                    SHFromDate = row.Field<DateTime>("HFromDate").ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),
                    SHToDate = row.Field<DateTime>("HToDate").ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture),
                }).AsQueryable();

            return periodsData;
        }

        [HttpPost]
        public ActionResult AddEditAcademicPeriods(FormCollection formData)
        {
            try
            {
                var academicYear = Convert.ToInt32(formData["Year"]);

                var terms = new List<AcademicPeriods>()
                {
                    new AcademicPeriods
                    {
                        TermID = 1,
                        FromDate = DateTime.ParseExact(formData["T1SPFromDate"], "MM/dd/yyyy", null),
                        ToDate = DateTime.ParseExact(formData["T1SPToDate"], "MM/dd/yyyy", null),
                        HFromDate = DateTime.ParseExact(formData["T1HPFromDate"], "MM/dd/yyyy", null),
                        HToDate = DateTime.ParseExact(formData["T1HPToDate"], "MM/dd/yyyy", null)
                    },
                    new AcademicPeriods
                    {
                        TermID = 2,
                        FromDate = DateTime.ParseExact(formData["T2SPFromDate"], "MM/dd/yyyy", null),
                        ToDate = DateTime.ParseExact(formData["T2SPToDate"], "MM/dd/yyyy", null),
                        HFromDate = DateTime.ParseExact(formData["T2HPFromDate"], "MM/dd/yyyy", null),
                        HToDate = DateTime.ParseExact(formData["T2HPToDate"], "MM/dd/yyyy", null)
                    },
                    new AcademicPeriods
                    {
                        TermID = 3,
                        FromDate = DateTime.ParseExact(formData["T3SPFromDate"], "MM/dd/yyyy", null),
                        ToDate = DateTime.ParseExact(formData["T3SPToDate"], "MM/dd/yyyy", null),
                        HFromDate = DateTime.ParseExact(formData["T3HPFromDate"], "MM/dd/yyyy", null),
                        HToDate = DateTime.ParseExact(formData["T3HPToDate"], "MM/dd/yyyy", null)
                    }
                };

                //var dataTable = ListToDataTable(terms);
                var dataTable = ConvertDataSet.ListToDataTable(terms);

                _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

                var result = _dblayer.AddEditAcademicPeriods(dataTable, academicYear, Session["Username"].ToString());

                if (result > 0)
                    return Json(new {success = true, message = "Academic Periods Added Successfully"},
                        JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _clf = new CreateErrorLogs(Session["Username"].ToString());
                _clf.CreateErrorLog(ErrorLogPath, "Error at AddEditAcademicPeriods method: " + ex);
            }
            
            return Json(new { success = false, message = "Academic Periods Update Failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult StudentRegistration()
        {
            GetAcademicTerms();  
            
            GetStudentClasses();    

            return View();
        }

        [HttpPost]
        public ActionResult StudentRegistration(int academicId, string students, char action)
        {
            var studentList = students.Split(',');

            var isEmpty = !studentList.Any();

            var result = 0;

            if (!isEmpty)
            {
                _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

                foreach (var student in studentList)
                {
                    result += _dblayer.RegisterUnregisterStudents(academicId, student, action, Session["Username"].ToString());
                }
            }

            if (result > 0)
            {
                return Json(new { message = "Student(s) has been processed successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Student Regisration/Unregistration has failed" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStudentRegister(int academicId, char action)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var studentData = ShowStudentRegister(sortColumn, sortColumnDir, searchValue, academicId, action);
            var recordsTotal = studentData.Count();
            var data = studentData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<StudentRegistration> ShowStudentRegister(string sortColumn, string sortColumnDir, string search, int academicId, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var register = _dblayer.GetStudentRegDetails(academicId, action);

            var registrationData = (from row in register.AsEnumerable()
                select new StudentRegistration()
                {
                    StudentID = row.Field<string>("StudentID"),
                    StudentName = row.Field<string>("StudentName"),
                    Gender = row.Field<string>("Gender"),
                    Form = row.Field<string>("Form"),
                    Class = row.Field<string>("Class"),
                    SchoolSection = row.Field<string>("SchoolSection"),
                    StudentStatus = row.Field<string>("StudentStatus"),
                    Registered = row.Field<string>("Registered")
                }).AsQueryable();

            var queryableRegistrationList = registrationData;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableRegistrationList = queryableRegistrationList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableRegistrationList = queryableRegistrationList.Where(m => m.StudentID.Contains(search.ToUpper()) || m.StudentName.Contains(search.ToUpper()));
            }

            return queryableRegistrationList;
        }

        [HttpPost]
        public ActionResult UpdateStudentClassSection(int academicId, string students, string classCode, char schSectionId)
        {
            var studentList = students.Split(',');

            var isEmpty = !studentList.Any();

            var result = 0;

            if (!isEmpty)
            {
                _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

                foreach (var student in studentList)
                {
                    result += _dblayer.UpdateStudentRegistration(academicId, student, classCode, schSectionId, Session["Username"].ToString());
                }
            }

            if (result > 0)
            {
                return Json(new { message = "Student(s) has been updated successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { message = "Student(s) Update has failed" }, JsonRequestBehavior.AllowGet);
        }

        public void GetAcademicTerms()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

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

        public void GetStudentClasses()
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var dtStudentClasses = _dblayer.GetStaticData('C');
            var dtViewStudentClasses = new DataView(dtStudentClasses);
            var dstStudentClasses = dtViewStudentClasses.ToTable(true, "ClassCode", "Class");

            var studentClasses = (from row in dstStudentClasses.AsEnumerable()
                select new Classes
                {
                    ClassCode = row.Field<string>("ClassCode"),
                    Class = row.Field<string>("Class")
                }).Distinct();

            var lstStudentClasses = studentClasses.ToList();

            ViewBag.Classes = new SelectList(lstStudentClasses, "ClassCode", "Class");
        }

       
    }
}