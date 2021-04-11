using System.Web.Mvc;
using System.Web.Routing;

namespace SchuleManager
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "EditParentDetails",
                "ParentMaintenance/EditParentDetails/{parentId}",
                new { controller = "ParentMaintenance", action = "EditParentDetails" },
                new { parentId = @"\d+" }
            );

            routes.MapRoute(
                "ProductItems",                                              // Route name
                "ProductMaintenance/ProductItems/{productCode}/{productType}",                           // URL with parameters
                new { controller = "ProductMaintenance", action = "ProductItems", productCode = ""}  // Parameter defaults
            );
        }
    }
}
