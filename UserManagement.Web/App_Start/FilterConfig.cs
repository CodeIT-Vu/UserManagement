using System.Web.Mvc;
using UserManagement.Web.Filters;

namespace UserManagement.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(DependencyResolver.Current.GetService<CurrentUserFilter>());
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
