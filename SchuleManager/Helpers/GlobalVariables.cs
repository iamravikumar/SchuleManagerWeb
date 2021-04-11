using System.Web;

namespace SchuleManager.Helpers
{
    public static class GlobalVariables
    {
        public static string Entity
        {
            get
            {
                return HttpContext.Current.Application["Entity"] as string;
                
            }
            set
            {
                HttpContext.Current.Application["Entity"] = value; 
            }
        }
    }
}