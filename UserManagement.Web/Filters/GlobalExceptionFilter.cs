using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Models;
using UserManagement.Web.Util;

namespace UserManagement.Web.Filters
{
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null || filterContext.ExceptionHandled)
            {
                return;
            }

            Exception exception = filterContext.Exception;
            ApiException apiException = exception as ApiException;
            ValidationException validationException = exception as ValidationException;
            HttpException httpException = exception as HttpException;

            int statusCode = 500;
            string errorCode = ErrorCode.Unexpected;
            string message = ErrorMessages.Unexpected;
            string traceId = Guid.NewGuid().ToString("N");

            if (apiException != null)
            {
                statusCode = apiException.HttpStatusCode;
                errorCode = apiException.ErrorCode;
                message = apiException.UserMessage;
            }
            else if (exception is HttpAntiForgeryException)
            {
                statusCode = 400;
                errorCode = ErrorCode.InvalidRequest;
                message = ErrorMessages.InvalidRequest;
            }
            else if (httpException != null && httpException.GetHttpCode() == 400)
            {
                statusCode = 400;
                errorCode = ErrorCode.InvalidRequest;
                message = ErrorMessages.InvalidRequest;
            }

            Trace.TraceError("[TraceId:{0}] {1}", traceId, exception);
            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

            bool isJsonRequest = HttpRequestUtil.IsJsonRequest(filterContext.HttpContext.Request);
            if (isJsonRequest)
            {
                ApiErrorResponse response = new ApiErrorResponse();
                response.Code = errorCode;
                response.Message = message;
                response.TraceId = traceId;

                if (validationException != null)
                {
                    response.Errors = validationException.Errors;
                }

                JsonResult result = new JsonResult();
                result.Data = response;
                result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                filterContext.Result = result;
            }
            else
            {
                ViewResult result = new ViewResult();
                result.ViewName = "Error";
                result.ViewData = new ViewDataDictionary();
                result.ViewData["Message"] = message;
                filterContext.Result = result;
            }

            filterContext.ExceptionHandled = true;
        }
    }
}
