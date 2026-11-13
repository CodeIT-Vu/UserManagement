using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UserManagement.Web.Util;

namespace UserManagement.Web
{
    /// <summary>
    /// Khởi tạo ứng dụng MVC và xử lý lỗi ngoài controller.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Lấy lỗi chưa được xử lý và ghi phản hồi mà không gọi lại MVC.
        /// </summary>
        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            if (exception == null)
            {
                return;
            }

            ErrorHandlingUtil.WriteApplicationError(new HttpContextWrapper(Context), exception);
            Server.ClearError();
            CompleteRequest();
        }

        /// <summary>
        /// Đăng ký area, DI, filter và route khi ứng dụng khởi động.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.Register();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
