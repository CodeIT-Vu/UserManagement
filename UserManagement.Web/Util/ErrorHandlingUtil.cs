using System;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Models.Responses;

namespace UserManagement.Web.Util
{
    /// <summary>
    /// Chuẩn hóa phản hồi lỗi và ghi log dùng chung cho MVC và Application_Error.
    /// </summary>
    public static class ErrorHandlingUtil
    {
        /// <summary>
        /// Phân loại exception, tạo phản hồi an toàn và ghi chi tiết lỗi vào log.
        /// </summary>
        public static ApiErrorResponse CreateErrorResponse(Exception exception, out int statusCode)
        {
            Exception error = exception;
            // Bỏ lớp exception bao ngoài của ASP.NET để phân loại đúng lỗi gốc.
            while (error is HttpUnhandledException && error.InnerException != null)
            {
                error = error.InnerException;
            }

            ApiException apiException = error as ApiException;
            HttpException httpException = error as HttpException;
            RequestValidationException validationException = error as RequestValidationException;
            string traceId = Guid.NewGuid().ToString("N");
            statusCode = HttpStatusCodes.InternalServerError;
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
            else if (httpException != null && httpException.GetHttpCode() == HttpStatusCodes.NotFound)
            {
                statusCode = HttpStatusCodes.NotFound;
                response.Code = ErrorCode.PageNotFound;
                response.Message = ErrorMessages.PageNotFound;
            }
            else if (error is HttpAntiForgeryException || (httpException != null && httpException.GetHttpCode() == HttpStatusCodes.BadRequest))
            {
                statusCode = HttpStatusCodes.BadRequest;
                response.Code = ErrorCode.InvalidRequest;
                response.Message = ErrorMessages.InvalidRequest;
            }

            if (validationException != null)
            {
                response.Errors = validationException.Errors;
            }

            // Chỉ trả mã tra cứu cho lỗi hệ thống; lỗi 4xx vẫn được ghi log.
            if (statusCode >= HttpStatusCodes.InternalServerError)
            {
                response.TraceId = traceId;
            }

            Trace.TraceError("[TraceId:{0}] {1}", traceId, exception);
            return response;
        }

        /// <summary>
        /// Đặt mã HTTP, UTF-8 và giữ phản hồi lỗi khỏi bị IIS hoặc Forms Authentication thay thế.
        /// </summary>
        public static void PrepareResponse(HttpResponseBase response, int statusCode)
        {
            response.Clear();
            response.StatusCode = statusCode;
            response.TrySkipIisCustomErrors = true;
            response.SuppressFormsAuthenticationRedirect = true;
            response.ContentEncoding = Encoding.UTF8;
        }

        /// <summary>
        /// Ghi JSON hoặc HTML đơn giản cho lỗi ngoài luồng xử lý MVC.
        /// </summary>
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
                // Ghi HTML trực tiếp và mã hóa nội dung, không gọi lại routing hoặc Razor.
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
