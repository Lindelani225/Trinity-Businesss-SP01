using System.Web;
using System.Web.Mvc;

namespace BusinesssTrinitySP01
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
