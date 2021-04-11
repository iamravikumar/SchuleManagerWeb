using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    public class ParentsController : Controller
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
        public ActionResult AddEditParents(Parents parent, char action)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var result = _dblayer.AddEditParents(parent, action, Session["Username"].ToString());

            if (result > 0)
            {
                if (action == 'A')
                {
                    return Json(new { success = true, message = "Parent Information Added Successfully, Parent ID: " + result }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = "Parent Information Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Failed to update information, try again or contact system administrator" }, JsonRequestBehavior.AllowGet);
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

            var dtTitles = _dblayer.GetStaticData('T');
            var dtViewTitles = new DataView(dtTitles);
            var dstTitles = dtViewTitles.ToTable(true, "TitleCode", "Title");

            var titles = (from row in dstTitles.AsEnumerable()
                select new Titles()
                {
                    TitleCode = row.Field<string>("TitleCode"),
                    Title = row.Field<string>("Title")
                }).Distinct();

            var dtEducation = _dblayer.GetStaticData('E');
            var dtViewEducation = new DataView(dtEducation);
            var dstEducation = dtViewEducation.ToTable(true, "EducationCode", "Education");

            var education = (from row in dstEducation.AsEnumerable()
                select new Education()
                {
                    EducationCode = row.Field<string>("EducationCode"),
                    EducationLevel = row.Field<string>("Education")
                }).Distinct();

            ViewData["Nationality"] = nationalities.ToList();
            ViewData["Titles"] = titles.ToList();
            ViewData["Education"] = education.ToList();
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
        public JsonResult DownloadPhoto(string clientId)
        {
            _dblayer = new Db(GlobalVariables.Entity);

            try
            {
                // Loading File info.  
                var fileInfo = _dblayer.GetClientPhotoDetails(clientId, 'P');

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

    }
}