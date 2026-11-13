using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UserManagement.Web
{
    /// <summary>
    /// Cấu hình đường dẫn đến controller và action.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Bỏ qua tài nguyên axd và đăng ký route MVC mặc định.
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
