using System.Web;
using System.Web.Mvc;

namespace UserManagement.Web.Util
{
    /// <summary>
    /// Nhận biết yêu cầu cần nhận phản hồi JSON.
    /// </summary>
    public static class HttpRequestUtil
    {
        /// <summary>
        /// Nhận biết yêu cầu AJAX hoặc JSON qua header và content type.
        /// </summary>
        public static bool IsJsonRequest(HttpRequestBase request)
        {
            if (request == null)
            {
                return false;
            }

            string accept = request.Headers["Accept"];
            string contentType = request.ContentType;

            return (!string.IsNullOrEmpty(accept) && accept.Contains("application/json"))
                || request.IsAjaxRequest()
                || (!string.IsNullOrEmpty(contentType) && contentType.Contains("application/json"));
        }
    }
}
