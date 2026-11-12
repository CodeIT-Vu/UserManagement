using System;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Models;

namespace UserManagement.Web.Util
{
    public static class ErrorHandlingUtil
    {
        public static ApiErrorResponse CreateErrorResponse(Exception exception, out int statusCode)
        {
            Exception error = exception;
            while (error is HttpUnhandledException && error.InnerException != null)
            {
                error = error.InnerException;
            }

            ApiException apiException = error as ApiException;
            HttpException httpException = error as HttpException;
            RequestValidationException validationException = error as RequestValidationException;
            string traceId = Guid.NewGuid().ToString("N");
            statusCode = 500;
            ApiErrorResponse response = new ApiErrorResponse
            {
                Code = ErrorCode.Unexpected,
                Message = ErrorMessages.Unexpected
            };

            if (apiException != null)
            {
                statusCode = apiException.HttpStatusCode;
                response.Code = apiException.ErrorCode;
                response.Message = apiException.UserMessage;
            }
            else if (httpException != null && httpException.GetHttpCode() == 404)
            {
                statusCode = 404;
                response.Code = ErrorCode.PageNotFound;
                response.Message = ErrorMessages.PageNotFound;
            }
            else if (error is HttpAntiForgeryException || (httpException != null && httpException.GetHttpCode() == 400))
            {
                statusCode = 400;
                response.Code = ErrorCode.InvalidRequest;
                response.Message = ErrorMessages.InvalidRequest;
            }

            if (validationException != null)
            {
                response.Errors = validationException.Errors;
            }

            if (statusCode >= 500)
            {
                response.TraceId = traceId;
            }

            Trace.TraceError("[TraceId:{0}] {1}", traceId, exception);
            return response;
        }

        public static void PrepareResponse(HttpResponseBase response, int statusCode)
        {
            response.Clear();
            response.StatusCode = statusCode;
            response.TrySkipIisCustomErrors = true;
            response.SuppressFormsAuthenticationRedirect = true;
            response.ContentEncoding = Encoding.UTF8;
        }

        public static void WriteApplicationError(HttpContextBase context, Exception exception)
        {
            int statusCode;
            ApiErrorResponse error = CreateErrorResponse(exception, out statusCode);
            PrepareResponse(context.Response, statusCode);

            if (HttpRequestUtil.IsJsonRequest(context.Request))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(error);

                context.Response.ContentType = "application/json";
                context.Response.Write(json);
            }
            else
            {
                string message = HttpUtility.HtmlEncode(error.Message);
                string traceIdHtml = "";
                if (error.TraceId != null)
                {
                    traceIdHtml = $"<p>Mã tra cứu: {HttpUtility.HtmlEncode(error.TraceId)}</p>";
                }
                string html = $@"<!DOCTYPE html>
                            <html lang=""vi"">
                            <head>
                                <meta charset=""utf-8"">
                                <title>Có lỗi xảy ra</title>
                            </head>
                            <body>
                                <h1>Không thể xử lý yêu cầu</h1>
                                <p>{message}</p>
                                {traceIdHtml}
                            </body>
                            </html>";

                context.Response.ContentType = "text/html";
                context.Response.Write(html);
            }
        }
    }
}
