using System;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using UserManagement.Web.Constants;
using UserManagement.Web.Models;
using UserManagement.Web.Services;
using UserManagement.Web.Util;

namespace UserManagement.Web.Filters
{
    public class CurrentUserFilter : IAuthenticationFilter
    {
        private readonly IAuthService _authService;

        public CurrentUserFilter(IAuthService authService)
        {
            _authService = authService;
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            IPrincipal principal = filterContext.Principal;
            if (principal == null)
            {
                return;
            }

            if (!principal.Identity.IsAuthenticated)
            {
                return;
            }

            string username = principal.Identity.Name;
            AppUser user = _authService.FindActiveUser(username);

            if (user == null)
            {
                FormsAuthentication.SignOut();
                GenericIdentity anonymousIdentity = new GenericIdentity("");
                filterContext.Principal = new GenericPrincipal(anonymousIdentity, new string[0]);
                return;
            }

            string[] roles = new[] { user.RoleName };
            filterContext.Principal = new GenericPrincipal(principal.Identity, roles);

            filterContext.HttpContext.Items["CurrentUser"] = user;
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            HttpUnauthorizedResult unauthorizedResult = filterContext.Result as HttpUnauthorizedResult;
            if (unauthorizedResult == null)
            {
                return;
            }

            bool isAuthenticated = filterContext.HttpContext.User.Identity.IsAuthenticated;
            bool isJsonRequest = HttpRequestUtil.IsJsonRequest(filterContext.HttpContext.Request);

            if (!isAuthenticated && !isJsonRequest)
            {
                return;
            }

            int statusCode = 401;
            string errorCode = ErrorCode.AuthenticationRequired;
            string message = ErrorMessages.AuthenticationRequired;

            if (isAuthenticated)
            {
                statusCode = 403;
                errorCode = ErrorCode.AccessDenied;
                message = ErrorMessages.AccessDenied;
            }

            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

            if (isJsonRequest)
            {
                ApiErrorResponse response = new ApiErrorResponse();
                response.Code = errorCode;
                response.Message = message;
                response.TraceId = Guid.NewGuid().ToString("N");

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
        }
    }
}
