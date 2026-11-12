using System;
using System.Security.Principal;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using UserManagement.Web.Constants;
using UserManagement.Web.DTOs;
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
            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                return;
            }

            string username = principal.Identity.Name;
            FormsIdentity identity = principal.Identity as FormsIdentity;
            string ticketAuthVersion = identity == null ? null : identity.Ticket.UserData;

            CurrentUserDto user = _authService.FindActiveUser(username);

            if (user == null || user.AuthVersion <= 0 || user.AuthVersion.ToString() != ticketAuthVersion)
            {
                FormsAuthentication.SignOut();
                GenericIdentity anonymousIdentity = new GenericIdentity("");
                filterContext.Principal = new GenericPrincipal(anonymousIdentity, Array.Empty<string>());
                filterContext.HttpContext.Items.Remove("CurrentUser");
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

            int statusCode = isAuthenticated ? 403 : 401;
            string errorCode = isAuthenticated ? ErrorCode.AccessDenied : ErrorCode.AuthenticationRequired;
            string message = isAuthenticated ? ErrorMessages.AccessDenied : ErrorMessages.AuthenticationRequired;

            string traceId = Guid.NewGuid().ToString("N");
            Trace.TraceWarning("[TraceId:{0}] HTTP {1}: {2}", traceId, statusCode, errorCode);

            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

            if (isJsonRequest)
            {
                ApiErrorResponse response = new ApiErrorResponse();
                response.Code = errorCode;
                response.Message = message;

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
