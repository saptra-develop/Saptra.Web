using System.Web;
using System.Web.Mvc;
using Saptra.Web.Controllers;

namespace Saptra.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AutorizarLogin());
            filters.Add(new LogAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
