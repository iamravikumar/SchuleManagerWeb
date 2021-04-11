using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class StudentsController : Controller
    {
        private static CreateErrorLogs _clf;
        private static readonly string ErrorLogPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogs/"));
        private Db _dblayer;

        [HttpGet]
        public ActionResult PersonalInformation()
        {
            GetStaticData();

            return View();
        }

        [HttpPost]
        public ActionResult AddEditStudents(Students student, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var result = _dblayer.AddEditStudents(student, action, Session["Username"].ToString());

            if (result > 0)
            {
                if (action == 'A')
                {
                    return Json(new { success = true, message = "Student Information Added Successfully, Student ID: " + result }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Student Information Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Failed to update information, try again or contact system administrator" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PhotoUpload()
        {
            ViewBag.NewPhotoData = "";
            return View();
        }

        [HttpPost]
        public ActionResult PhotoUpload(ClientPhotos photo)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            try
            {
                // Converting to bytes.  
                var uploadedFile = new byte[photo.FileAttach.InputStream.Length];
                photo.FileAttach.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                // Initialization.  
                var fileContent = Convert.ToBase64String(uploadedFile);
                var fileContentType = photo.FileAttach.ContentType;

                var clientPhotoExists = _dblayer.ClientPhotoExists(photo.EntityID, 'P');

                var action = clientPhotoExists ? 'E' : 'A';

                // Saving photo to database.
                _dblayer.AddEditPhotos(photo.EntityID, fileContent, fileContentType, Session["Username"].ToString(), 'P', action);

            }
            catch (Exception ex)
            {
                _clf = new CreateErrorLogs(Session["Username"].ToString());

                _clf.CreateErrorLog(ErrorLogPath, "Error at PhotoUpload method: " + ex);
            }

            var fileInfo = _dblayer.GetClientPhotoDetails(photo.EntityID, 'P');


            ViewBag.NewPhotoData = fileInfo.PhotoData;

            return View();
        }

        [HttpGet]
        public ActionResult DownloadPhoto(string clientId)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            try
            {
                // Loading File info.  
                var fileInfo = _dblayer.GetClientPhotoDetails(clientId, 'S');

                return Json(new { base64image = fileInfo.PhotoData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _clf = new CreateErrorLogs(Session["Username"].ToString());

                _clf.CreateErrorLog(ErrorLogPath, "Error at DownloadPhoto method: " + ex);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ParentsInformation()
        {
            GetStaticData();

            return View();
        }

        [NonAction]
        public void GetStaticData()
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var dtNationality = _dblayer.GetStaticData('N');
            var dtViewNationality = new DataView(dtNationality);
            var dstNationality = dtViewNationality.ToTable(true, "NationalityCode", "Nationality");

            var nationalities = (from row in dstNationality.AsEnumerable()
                select new Nationalities()
                {
                    NationalityCode = row.Field<string>("NationalityCode"),
                    Nationality = row.Field<string>("Nationality")
                }).Distinct();

            var dtHouses = _dblayer.GetStaticData('H');
            var dtViewHouses = new DataView(dtHouses);
            var dstHouses = dtViewHouses.ToTable(true, "HouseCode", "House");

            var houses = (from row in dstHouses.AsEnumerable()
                select new Houses()
                {
                    HouseCode = row.Field<string>("HouseCode"),
                    House = row.Field<string>("House")
                }).Distinct();

            var dtReligion = _dblayer.GetStaticData('R');
            var dtViewReligion = new DataView(dtReligion);
            var dstReligion = dtViewReligion.ToTable(true, "ReligionCode", "Religion");

            var religion = (from row in dstReligion.AsEnumerable()
                select new Religions()
                {
                    ReligionCode = row.Field<string>("ReligionCode"),
                    Religion = row.Field<string>("Religion")
                }).Distinct();

            var dtClasses = _dblayer.GetStaticData('C');
            var dtViewClasses = new DataView(dtClasses);
            var dstClasses = dtViewClasses.ToTable(true, "ClassCode", "Class");

            var classes = (from row in dstClasses.AsEnumerable()
                select new Classes
                {
                    ClassCode = row.Field<string>("ClassCode"),
                    Class = row.Field<string>("Class")
                }).Distinct();

            var dtRelationships = _dblayer.GetStaticData('P');
            var dtViewRelationships = new DataView(dtRelationships);
            var dstRelationships = dtViewRelationships.ToTable(true, "RshipCode", "Relationship");

            var relationships = (from row in dstRelationships.AsEnumerable()
                select new Relationships()
                {
                    RshipCode = row.Field<string>("RshipCode"),
                    RelationShip = row.Field<string>("RelationShip")
                }).Distinct();

            var dtProductDetails = _dblayer.GetStudentFeesProductDetails();
            var dtViewProductDetails = new DataView(dtProductDetails);
            var dstProductDetails = dtViewProductDetails.ToTable(true, "ProductCode", "ProductName");
            var dstFeeDetails = dtViewProductDetails.ToTable(true, "ItemCode", "Description");

            var productDetails = (from row in dstProductDetails.AsEnumerable()
                select new StudentSpecialFees
                {
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                }).Distinct();

            var feeDetails = (from row in dstFeeDetails.AsEnumerable()
                select new StudentSpecialFees
                {
                    FeeCode = row.Field<string>("ItemCode"),
                    FeeDescription = row.Field<string>("Description"),
                }).Distinct();

            ViewData["Nationality"] = nationalities.ToList();
            ViewData["Houses"] = houses.ToList();
            ViewData["Religions"] = religion.ToList();
            ViewData["Classes"] = classes.ToList();
            ViewData["Relationships"] = relationships.ToList();
            ViewData["StudentProducts"] = productDetails.ToList();
            ViewData["StudentFees"] = feeDetails.ToList();
        }

        [HttpGet]
        public JsonResult GetStudentDetails(string studentId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var studentDetailsData = _dblayer.GetStudentDetails(studentId);

            var studentDetails = (from row in studentDetailsData.AsEnumerable()
                select new Students
                {
                    IndexNo = row.Field<string>("IndexNo"),
                    UNEBNo = row.Field<string>("UNEBNo"),
                    OtherID1 = row.Field<string>("OtherID1"),
                    OtherID2 = row.Field<string>("OtherID2"),
                    SurName = row.Field<string>("SurName"),
                    OtherNames = row.Field<string>("OtherNames"),
                    RAddress = row.Field<string>("RAddress"),
                    Dateofbirth = row.Field<DateTime>("Dateofbirth"),
                    GenderID = row.Field<string>("GenderID"),
                    NationalityCode = row.Field<string>("NationalityCode"),
                    RegDate = row.Field<DateTime>("RegDate"),
                    ClassCode = row.Field<string>("ClassCode"),
                    HouseCode = row.Field<string>("HouseCode"),
                    FormerSchool = row.Field<string>("FormerSchool"),
                    SchSectionID = row.Field<string>("SchSectionID"),
                    Status = row.Field<string>("Status"),
                    FeesBalance = row.Field<decimal>("FeesBal"),
                    FormSection = row.Field<string>("FormSection"),
                    Registered = row.Field<bool>("Registered")
                }).AsQueryable();

            return Json(studentDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetParentDetails(string parentId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var parentDetailsData = _dblayer.GetParentDetails(parentId);

            var parentDetails = (from row in parentDetailsData.AsEnumerable()
                select new Parents()
                {
                    ParentID = row.Field<string>("ParentID"),
                    TitleCode = row.Field<string>("TitleCode"),
                    SurName = row.Field<string>("SurName"),
                    OtherNames = row.Field<string>("OtherNames"),
                    Dateofbirth = row.Field<DateTime>("Dateofbirth"),
                    Gender = row.Field<string>("GenderID"),
                    NationalityCode = row.Field<string>("NationalityCode"),
                    EducationCode = row.Field<string>("EducationCode"),
                    Occupation = row.Field<string>("Occupation"),
                    RAddress = row.Field<string>("RAddress"),
                    RPhone1 = row.Field<string>("RPhone1")

                }).AsQueryable();

            return Json(parentDetails, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStudentParents(string studentId)
        {
            var data = ShowAllStudentParents(studentId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public IQueryable<StudentParent> ShowAllStudentParents(string studentId)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var studentParentsDatatable = _dblayer.GetStudentParents(studentId);

            var studentParents = (from row in studentParentsDatatable.AsEnumerable()
                select new StudentParent
                {
                    ParentID = row.Field<string>("ParentID"),
                    TitleCode = row.Field<string>("TitleCode"),
                    ParentName = row.Field<string>("ParentName"),
                    Relationship = row.Field<string>("Relationship"),
                    RshipCode = row.Field<string>("RshipCode"),
                    _LiveswithStudent = row.Field<string>("LivesWithStudent"),
                    _PaysFees = row.Field<string>("PaysFees"),
                    MoreInfo = row.Field<string>("MoreInfo")
                }).AsQueryable();

            return studentParents;
        }

        [HttpPost]
        public ActionResult AddEditStudentParents(StudentParent studentParents, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity, Session["Username"].ToString());

            var result = _dblayer.AddEditStudentParents(studentParents, action, Session["Username"].ToString());

            if (result > 0)
            {
                if (action == 'A')
                {
                    return Json(new { success = true, message = "Student's Parent Added Successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Student's Parents Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Failed to update information, try again or contact system administrator" }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult AddEditStudentSpecialFees(StudentSpecialFees specialFees)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            if (!specialFees.StudentID.IsNullOrWhiteSpace() && specialFees.Action == "A")
            {
                var result = _dblayer.AddEditStudentSpecialFees(specialFees, 'A', Session["Username"].ToString());
                if (result > 0)
                    return Json(new { success = true, message = "Student's Special Fees Updated Successfully", response = result },
                        JsonRequestBehavior.AllowGet);
            }
            else if (Convert.ToInt32(specialFees.SFID) > 0 && specialFees.Action == "D")
            {
                var result = _dblayer.AddEditStudentSpecialFees(specialFees, 'D', Session["Username"].ToString());
                if (result > 0)
                    return Json(new { success = true, message = "Special Fees Deleted Successfully", response = result },
                        JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Update Failed" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStudentData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var pageSize = length != null ? Convert.ToInt32(length) : 0;
            var skip = start != null ? Convert.ToInt32(start) : 0;

            var parentData = ShowAllStudents(sortColumn, sortColumnDir, searchValue);
            var recordsTotal = parentData.Count();
            var data = parentData.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

        public IQueryable<Students> ShowAllStudents(string sortColumn, string sortColumnDir, string search)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var students = _dblayer.GetClients('S');

            var studentData = (from row in students.AsEnumerable()
                select new Students
                {
                    StudentID = row.Field<string>("StudentID"),
                    SurName = row.Field<string>("SurName"),
                    OtherNames = row.Field<string>("OtherNames"),
                    ClassCode = row.Field<string>("ClassCode")
                }).AsQueryable();

            var queryableStudentList = studentData;

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                queryableStudentList = queryableStudentList.OrderBy(sortColumn + " " + sortColumnDir);
            }
            if (!string.IsNullOrEmpty(search))
            {
                queryableStudentList = queryableStudentList.Where(m => m.SurName.Contains(search.ToUpper()) || m.OtherNames.Contains(search.ToUpper()));
            }

            return queryableStudentList;
        }

        public ActionResult LoadStudentSpecialFees(string studentId)
        {

            var data = ShowStudentSpecialFees(studentId);

            return Json(new { message = "success", data });
        }

        public IQueryable<StudentSpecialFees> ShowStudentSpecialFees(string studentId)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var studentSpecialFees = _dblayer.GetStudentSpecialFees(studentId);

            var studentSpecialFeesDataAsQueryable = (from row in studentSpecialFees.AsEnumerable()
                select new StudentSpecialFees
                {
                    SFID = row.Field<int>("SFID"),
                    StudentID = row.Field<string>("StudentID"),
                    StudentName = row.Field<string>("StudentName"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("Product"),
                    FeeCode = row.Field<string>("FeeCode"),
                    FeeDescription = row.Field<string>("Fee"),
                    PercentageOrAmount = row.Field<string>("PercentageOrAmount"),
                    Value = row.Field<decimal>("Value"),
                    Remarks = row.Field<string>("Remarks"),
                    AcademicYear = row.Field<int>("AcademicYear"),
                    TermID = row.Field<int>("TermID")
                }).AsQueryable();

            var queryableDataList = studentSpecialFeesDataAsQueryable;

            return queryableDataList;
        }



    }
}