using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UserManagement.Web.Util;

namespace UserManagement.Web
{
    public class MvcApplication : HttpApplication
    {
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

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.Register();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
