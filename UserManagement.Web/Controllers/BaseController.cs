using System.Web.Mvc;
using System.Web.Security;
using UserManagement.Web.Models;

namespace UserManagement.Web.Controllers
{
    public class BaseController : Controller
    {
        protected void SetLoginSessionInfo(AppUser user)
        {
            FormsAuthentication.SetAuthCookie(user.Username, false);
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
