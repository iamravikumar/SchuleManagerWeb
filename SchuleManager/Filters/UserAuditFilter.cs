using System;
using System.Web;
using System.Web.Mvc;
using SchuleManager.Controllers;
using SchuleManager.Helpers;
using SchuleManager.Models;


namespace SchuleManager.Filters
{
    public class UserAuditFilter : ActionFilterAttribute
    { 

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var dblayer = new DataAccessLayer.Db(GlobalVariables.Entity); 

            var objAudit = new Audit();
            var actionName = filterContext.ActionDescriptor.ActionName; // Getting the Action name
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName; //Getting the Controller name
            var request = filterContext.HttpContext.Request;

            objAudit.UserName = HttpContext.Current.Session["UserName"] != null ? Convert.ToString(HttpContext.Current.Session["UserName"]) : "";

            objAudit.SessionId = HttpContext.Current.Session.SessionID; // Application Session ID
            objAudit.IpAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress; // User IP Address
            objAudit.PageAccessed = Convert.ToString(filterContext.HttpContext.Request.Url); // URL Requested
            objAudit.AccessedTime = DateTime.Now; // Time of request

            if (actionName == "LogOff")
            {
                objAudit.LoggedOutAt = DateTime.Now; //Time user logged out
            }

            objAudit.LoginStatus = "A";
            objAudit.ControllerName = controllerName;
            objAudit.ActionName = actionName;

            var myReferrer = request.UrlReferrer;

            if (myReferrer == null) return;
            if (request.UrlReferrer != null) objAudit.UrlReferrer = request.UrlReferrer.AbsolutePath;

            if (controllerName == "DefaultCaptcha" || controllerName == "Login")return;

            dblayer.AddAuditData(objAudit);         
        }
    }
}