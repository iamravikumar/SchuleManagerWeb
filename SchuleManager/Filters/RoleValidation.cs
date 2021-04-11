//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;
//using SchuleManager.Models;

//namespace SchuleManager.Filters
//{
//    public class RoleValidation : ActionFilterAttribute
//    {

//        private readonly TricoFinWebEntities _db = new TricoFinWebEntities();

//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            var controllerName = filterContext.RouteData.Values["controller"].ToString();
//            var actionName = filterContext.RouteData.Values["action"].ToString();
//            var roleid = HttpContext.Current.Session["RoleID"].ToString();

//            var query = _db.sp_GetRoleRights(roleid, controllerName, actionName);
//            var result = query.FirstOrDefault();
//            if (result != null)
//            {
//                base.OnActionExecuting(filterContext);
//            }
//            else
//            {
//                filterContext.Controller.TempData["ErrorMessage"] = "You dont rights to perform this action, contact your administrator";
//                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Error" }));
//            }

//        }
//    }
//}