using System.Web.Mvc;
using UserManagement.Web.Models;
using UserManagement.Web.Util;

namespace UserManagement.Web.Filters
{
    /// <summary>
    /// Xử lý exception trong MVC thành JSON hoặc trang lỗi.
    /// </summary>
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// Chuyển exception chưa xử lý thành JSON hoặc trang lỗi theo request.
        /// </summary>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.ExceptionHandled)
            {
                return;
            }

            int statusCode;
            ApiErrorResponse response = ErrorHandlingUtil.CreateErrorResponse(filterContext.Exception, out statusCode);
            ErrorHandlingUtil.PrepareResponse(filterContext.HttpContext.Response, statusCode);

            if (HttpRequestUtil.IsJsonRequest(filterContext.HttpContext.Request))
            {
                filterContext.Result = new JsonResult
                {
                    Data = response,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                ViewResult result = new ViewResult();
                result.ViewName = "~/Views/Shared/Error.cshtml";
                result.ViewData["Message"] = response.Message;
                result.ViewData["TraceId"] = response.TraceId;
                filterContext.Result = result;
            }

            filterContext.ExceptionHandled = true;
        }
    }
}
