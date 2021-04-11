using System.Web.Mvc;
using SchuleManager.Helpers;

namespace SchuleManager
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorLoggerAttribute());
        }
    }
}