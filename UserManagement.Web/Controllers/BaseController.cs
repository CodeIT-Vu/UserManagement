using System.Web.Mvc;
using System.Web.Security;
using UserManagement.Web.DTOs;
using System.Web;

namespace UserManagement.Web.Controllers
{
    public class BaseController : Controller
    {
        protected void SetLoginSessionInfo(CurrentUserDto user)
        {
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(user.Username, false);
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            FormsAuthenticationTicket versionedTicket = new FormsAuthenticationTicket(
                ticket.Version,
                ticket.Name,
                ticket.IssueDate,
                ticket.Expiration,
                ticket.IsPersistent,
                user.AuthVersion.ToString(),
                ticket.CookiePath);

            cookie.Value = FormsAuthentication.Encrypt(versionedTicket);
            Response.Cookies.Set(cookie);
        }

        protected void LoginSessionClear()
        {
            FormsAuthentication.SignOut();
        }

        protected string GetLoginRedirectUrl(string returnUrl)
        {
            string redirectUrl = Url.IsLocalUrl(returnUrl)
                ? Url.Content(returnUrl)
                : Url.Action("Index", "Home");

            return redirectUrl;
        }
    }
}
