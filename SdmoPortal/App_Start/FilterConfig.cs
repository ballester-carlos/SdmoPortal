using System.Web;
using System.Web.Mvc;

namespace SdmoPortal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //TO DO: add SSL
            //TODO: Un-comment for production
            //filters.Add(new AuthorizeAttribute());
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}
