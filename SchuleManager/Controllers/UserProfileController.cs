using System;
using System.Web.Mvc;
using SchuleManager.DataAccessLayer;
using SchuleManager.Filters;
using SchuleManager.Helpers;
using SchuleManager.Models;

namespace SchuleManager.Controllers
{
    [ValidateUserSession]
    public class UserProfileController : Controller
    {
        private Db _dblayer;

        [HttpGet]
        // GET: UserProfile
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel changepasswordmodel)
        {
            var password = EncryptionLibrary.EncryptText(changepasswordmodel.OldPassword);

            _dblayer = new Db(GlobalVariables.Entity);

            //var storedPassword = _ILogin.GetPasswordbyUserId(Convert.ToInt32(Session["UserID"]));
            var storedPassword = _dblayer.GetUserPassword(Convert.ToInt32(Session["UserID"]));

            if (storedPassword == password)
            {
                // var result = _ILogin.UpdatePassword(Convert.ToInt32(Session["UserID"]),
                //   EncryptionLibrary.EncryptText(changepasswordmodel.NewPassword));
                var result = _dblayer.UpdateUserPassword(Convert.ToInt32(Session["UserID"]), EncryptionLibrary.EncryptText(changepasswordmodel.NewPassword));
                if (result)
                {
                    ModelState.Clear();
                    ViewBag.message = "Password Changed Successfully";
                    return View(changepasswordmodel);
                }
                ModelState.AddModelError("", @"Some went wrong, please try again after some time");
                return View(changepasswordmodel);
            }
            ModelState.AddModelError("", @"Entered Wrong Old Password");
            return View(changepasswordmodel);
        }
    }
}