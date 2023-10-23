using inforpatissien_api.Filters;
using System.Web;
using System.Web.Mvc;

namespace inforpatissien_api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
