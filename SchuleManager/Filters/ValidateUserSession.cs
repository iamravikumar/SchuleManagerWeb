using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace SchuleManager.Filters
{
    public class ValidateUserSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["UserId"]))) return;
            filterContext.Controller.TempData["ErrorMessage"] = "Session has been expired please Login";
            filterContext.Result =
                new RedirectToRouteResult(new RouteValueDictionary(new {controller = "Error", action = "Error"}));
                
        }
    }
}