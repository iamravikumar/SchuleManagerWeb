using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace SchuleManager.Helpers
{
    public class ErrorLoggerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {

            var strLogText = "";
            var ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;
            var objClass = filterContext;
            strLogText += "Message ---\n{0}" + ex.Message;

            switch (ex.Source)
            {
                case ".Net SqlClient Data Provider":
                    strLogText += Environment.NewLine + "SqlClient Error ---\n{0}" + "Check Sql Error";
                    break;
                case "System.Web.Mvc":
                    strLogText += Environment.NewLine + ".Net Error ---\n{0}" + "Check MVC Code For Error";
                    break;
                default:
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        strLogText += Environment.NewLine + ".Net Error ---\n{0}" + "Check MVC Ajax Code For Error";
                    }
                    break;
            }

            strLogText += Environment.NewLine + "Source ---\n{0}" + ex.Source;
            strLogText += Environment.NewLine + "StackTrace ---\n{0}" + ex.StackTrace;
            strLogText += Environment.NewLine + "TargetSite ---\n{0}" + ex.TargetSite;
            if (ex.InnerException != null)
            {
                strLogText += Environment.NewLine + "Inner Exception is {0}" + ex.InnerException;//error prone
            }
            if (ex.HelpLink != null)
            {
                strLogText += Environment.NewLine + "HelpLink ---\n{0}" + ex.HelpLink;//error prone
            }

            var timestamp = DateTime.Now.ToString("d-MMMM-yyyy", new CultureInfo("en-GB"));

            var errorFolder = ConfigurationManager.AppSettings["ErrorLogPath"];

            if (!Directory.Exists(errorFolder))
            {
                Directory.CreateDirectory(errorFolder);
            }

            var log = !File.Exists($@"{errorFolder}\Log_{timestamp}.txt") ? new StreamWriter(
                $@"{errorFolder}\Log_{timestamp}.txt") : File.AppendText($@"{errorFolder}\Log_{timestamp}.txt");

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            // Write to the file:
            log.WriteLine(Environment.NewLine + DateTime.Now);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            log.WriteLine("Controller Name :- " + controllerName);
            log.WriteLine("Action Method Name :- " + actionName);
            log.WriteLine("------------------------------------------------------------------------------------------------");
            log.WriteLine(objClass);
            log.WriteLine(strLogText);
            log.WriteLine();

            // Close the stream:
            log.Close();
            filterContext.HttpContext.Session.Abandon();
            filterContext.Result = new ViewResult()
            {
                ViewName = "Error"
            };


        }
    }
}