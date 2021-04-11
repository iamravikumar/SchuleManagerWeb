using System;
using System.Web;
using System.Web.Mvc;

namespace SchuleManager.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error()
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

            return View("Error");
        }
    }
}