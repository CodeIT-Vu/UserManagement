using System.Web;
using System.Web.Mvc;

namespace UserManagement.Web.Util
{
    public static class HttpRequestUtil
    {
        public static bool IsJsonRequest(HttpRequestBase request)
        {
            if (request == null)
            {
                return false;
            }

            string accept = request.Headers["Accept"];
            string contentType = request.ContentType;

            // The app returns JSON for AJAX calls or requests that specify JSON.
            return (!string.IsNullOrEmpty(accept) && accept.Contains("application/json"))
                || request.IsAjaxRequest()
                || (!string.IsNullOrEmpty(contentType) && contentType.Contains("application/json"));
        }
    }
}
