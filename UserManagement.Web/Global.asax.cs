using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using UserManagement.Web.Constants;
using UserManagement.Web.Models;
using UserManagement.Web.Util;

namespace UserManagement.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.Register();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            HttpException httpException = exception as HttpException;
            int statusCode = 500;
            if (httpException != null)
            {
                statusCode = httpException.GetHttpCode();
            }
            string errorCode = ErrorCode.Unexpected;
            string userMessage = ErrorMessages.Unexpected;
            string traceId = Guid.NewGuid().ToString("N");

            if (exception is HttpAntiForgeryException || statusCode == 400)
            {
                statusCode = 400;
                errorCode = ErrorCode.InvalidRequest;
                userMessage = ErrorMessages.InvalidRequest;
            }
            else if (statusCode == 404)
            {
                errorCode = ErrorCode.PageNotFound;
                userMessage = ErrorMessages.PageNotFound;
            }
            else if (statusCode == 403)
            {
                errorCode = ErrorCode.AccessDenied;
                userMessage = ErrorMessages.AccessDenied;
            }
            else
            {
                statusCode = 500;
            }

            Trace.TraceError("[TraceId:{0}] {1}", traceId, exception);
            Response.Clear();
            Response.StatusCode = statusCode;
            Response.TrySkipIisCustomErrors = true;
            Response.SuppressFormsAuthenticationRedirect = true;

            if (HttpRequestUtil.IsJsonRequest(new HttpRequestWrapper(Request)))
            {
                ApiErrorResponse errorResponse = new ApiErrorResponse();
                errorResponse.Code = errorCode;
                errorResponse.Message = userMessage;
                errorResponse.TraceId = traceId;

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(errorResponse);

                Response.ContentType = "application/json";
                Response.Write(json);
            }
            else
            {
                string encodedMessage = Server.HtmlEncode(userMessage);
                string html = "<!doctype html><html lang=\"vi\"><meta charset=\"utf-8\"><title>Lỗi</title><body><h1>Không thể xử lý yêu cầu</h1><p>"
                    + encodedMessage + "</p></body></html>";

                Response.ContentType = "text/html";
                Response.Write(html);
            }

            Server.ClearError();
        }
    }
}
