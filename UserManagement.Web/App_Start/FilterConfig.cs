using System.Web.Mvc;
using UserManagement.Web.Filters;

namespace UserManagement.Web
{
    /// <summary>
    /// Đăng ký các filter dùng chung cho ứng dụng.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Đăng ký filter xác thực người dùng và xử lý exception toàn cục.
        /// </summary>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            FilterProviders.Providers.Add(new CurrentUserFilterProvider());
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
