using System;
using System.Data;
using System.Web;
using System.Web.Mvc;
using SchuleManager.Models;
using CaptchaMvc.HtmlHelpers;
using System.Collections.Generic;
using SchuleManager.DataAccessLayer;
using SchuleManager.Helpers;
using System.Configuration;
using System.Globalization;
using System.IO;

namespace TricoFinUAT.Controllers
{
    public class LoginController : Controller
    {
        private Db _dblayer;
        private string _code;
        private int _daysToExpire;
        private static CreateErrorLogs _clf;
        private static readonly string ErrorLogPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ErrorLogs/"));

        [HttpGet]
        [OverrideActionFilters]
        public ActionResult Login()
        {
            return View();
        } 
        
        [HttpPost]
        [OverrideActionFilters]
        [ValidateAntiForgeryToken]
        // GET: Login
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            try
            {
                var schoolCode = loginViewModel.SchoolCode.Trim().ToLower();

                var connectionString = ConfigurationManager.ConnectionStrings[schoolCode];

                if(connectionString == null)
                {
                    ViewBag.errormessage = "School Code is not Valid";

                    return View(loginViewModel);
                }

                GlobalVariables.Entity = schoolCode;

                if (!this.IsCaptchaValid("Captcha is not Valid"))
                {
                    ViewBag.errormessage = "Error: captcha entered is not Valid";

                    return View(loginViewModel);
                }

                var licenseCheck = CheckLicenseKey();

                if (licenseCheck == 0)
                {
                    ViewBag.errormessage = "Error: You have no license to use this application";

                    return View(loginViewModel);
                }
                else if (licenseCheck == 1)
                {
                    ViewBag.errormessage = "Error: Invalid License Key, Please Contact Developers for a Valid License";

                    return View(loginViewModel);
                }
                else if (licenseCheck == 2)
                {
                    ViewBag.errormessage = "Your License Key Expires Today, Contact Developers for a New License!!";
                }
                else if (licenseCheck == 3)
                {
                    ViewBag.errormessage = "Your License Key Expires Tomorrow, Contact Developers for a New License!!";
                }
                else if (licenseCheck == 4)
                {
                    ViewBag.errormessage = "Your License Key will Expire in " + _daysToExpire +
                                           " days, Consider budgeting for Renewal!!";
                }
                else if (licenseCheck == 5)
                {
                    ViewBag.errormessage = "Your License Key has Expired, Please Contact Developers for a New License.";
                    return View(loginViewModel);
                }


                if (!string.IsNullOrEmpty(loginViewModel.Username) && !string.IsNullOrEmpty(loginViewModel.Password))
                {
                    var username = loginViewModel.Username;
                    var password = EncryptionLibrary.EncryptText(loginViewModel.Password);

                    _dblayer = new Db(GlobalVariables.Entity, username);

                    var ds = _dblayer.GetUserLoginCredentials(username, password);
                    var userCredentials = new List<UserCredentials>();

                    foreach(DataRow dr in ds.Tables[0].Rows)
                    {
                        userCredentials.Add(new UserCredentials
                        {
                            UserId = Convert.ToInt32(dr["UserId"]),
                            UserName = dr["UserName"].ToString(),
                            SurName = dr["SurName"].ToString(),
                            UserPassword = dr["UserPassword"].ToString(),
                            IsDisabled = Convert.ToBoolean(dr["IsDisabled"]),
                            RoleCode = dr["RoleCode"].ToString(),
                            IsVerified = Convert.ToBoolean(dr["IsVerified"]),
                            MustChangePassword = Convert.ToBoolean(dr["MustChangePassword"]),
                            PasswordNeverExpires = Convert.ToBoolean(dr["PasswordNeverExpires"]),
                            HasExpired = Convert.ToBoolean(dr["HasExpired"]),
                            IsClosed = Convert.ToBoolean(dr["IsClosed"]),
                            CloseReason = dr["CloseReason"].ToString(),
                            LoginDate = Convert.ToDateTime(dr["LoginDate"])

                        });
                    }

                    if (userCredentials[0].UserId == 0 || userCredentials[0].UserId < 0)
                    {
                        ViewBag.errormessage = "Invalid Username or Password";
                    }

                    else if (userCredentials[0].IsClosed)
                    {
                        ViewBag.errormessage = "This login account has been closed. " + userCredentials[0].CloseReason;
                        return View(loginViewModel);
                    }
                    else if (userCredentials[0].HasExpired)
                    {
                        return RedirectToAction("ChangePassword", "UserProfile");
                    }
                    else if (userCredentials[0].MustChangePassword)
                    {
                        return RedirectToAction("ChangePassword", "UserProfile");
                    }
                    else if (!userCredentials[0].IsVerified)
                    {
                        ViewBag.errormessage = "Your login account has not been verified. Contact the System Administrator";
                        return View(loginViewModel);
                    }
                    else
                    {
                        //var roleId = userCredentials.RoleID;
                        remove_Anonymous_Cookies();
                        Session["UserId"] = Convert.ToString(userCredentials[0].UserId);
                        Session["UserRole"] = Convert.ToString(userCredentials[0].RoleCode);
                        Session["Username"] = Convert.ToString(userCredentials[0].UserName);
                        Session["SurName"] = Convert.ToString(userCredentials[0].SurName);
                        Session["LoginDate"] = Convert.ToString(userCredentials[0].LoginDate, CultureInfo.CurrentCulture);
                        //Session["LoginDate"] = Convert.ToDateTime(DateTime.ParseExact(userCredentials[0].LoginDate.ToShortDateString(), "dd-MM-yy", CultureInfo.InvariantCulture));

                        return RedirectToAction("Dashboard", "Dashboard");

                    }
                }
            }
            catch (Exception ex)
            {
                _clf = new CreateErrorLogs(loginViewModel.Username);
                _clf.CreateErrorLog(ErrorLogPath, "Error at Login method: " + ex);
            }
            return View();
        }

        [NonAction]
        public void remove_Anonymous_Cookies()
        {
            if (Request.Cookies["WebTime"] == null) return;
            var option = new HttpCookie("WebTime") {Expires = DateTime.Now.AddDays(-1)};
            Response.Cookies.Add(option);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();

            var cookies = new HttpCookie("WebTime")
            {
                Value = "",
                Expires = DateTime.Now.AddHours(-1)
            };
            Response.Cookies.Add(cookies);
            HttpContext.Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Login");        
        }

        public void EndSession()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();

            var cookies = new HttpCookie("WebTime")
            {
                Value = "",
                Expires = DateTime.Now.AddHours(-1)
            };
            Response.Cookies.Add(cookies);
            HttpContext.Session.Clear();
        }

        public int CheckLicenseKey()
        {
            _dblayer = new Db(GlobalVariables.Entity);

            var loginDetails = _dblayer.GetSystemStatusDetails();

            var row = loginDetails.Rows[0];

            //var loginDate = Convert.ToDateTime(row["LoginDate"]);
            //var status = row["Status"].ToString();
            var companyName = row["CompanyName"].ToString();
            var licenseNo = row["LicenseNo"].ToString();
            var licenseDate = Convert.ToDateTime(row["LicenseDate"].ToString());
            //var term = row["Term"].ToString();
            //var period = row["Period"].ToString();
            //var currentTerm = Convert.ToInt32(row["CurrentTerm"]);
            var companyId = Convert.ToInt32(row["CompanyID"]);
            var licensePeriod = Convert.ToInt32(row["LicensePeriod"]);
            

            var licenseKey = new ProductKey();

            if (!string.IsNullOrEmpty(licenseNo))
            {
                licenseKey.BaseDate = licenseDate;
                licenseKey.CompanyID = companyId;
                licenseKey.LicensePeriod = licensePeriod;
                licenseKey.Salt = companyName;
                _code = licenseKey.KeyCode;

                if (_code != licenseNo)
                {
                    return 1;
                }
                else
                {
                    _daysToExpire = licenseDate.Subtract(licenseKey.ExpDate).Days;

                    if (_daysToExpire == 0)
                    {
                        return 2;
                    }
                    else if(_daysToExpire == 1)
                    {
                        return 3;
                    }
                    else if(_daysToExpire <= 60)
                    {
                        return 4;
                    }
                    else if(licenseKey.ExpDate > DateTime.Today)
                    {
                        return 5;
                    }
                }
            }
            return 0;
        }
    }
}