using System.Web.Mvc;
using System.Web.Security;
using UserManagement.Web.DTOs;
using System.Web;

namespace UserManagement.Web.Controllers
{
    /// <summary>
    /// Cung cấp thao tác cookie đăng nhập và chuyển hướng dùng chung.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Tạo cookie đăng nhập chứa phiên bản xác thực hiện tại của tài khoản.
        /// </summary>
        protected void SetLoginSessionInfo(CurrentUserDto user)
        {
            HttpCookie cookie = FormsAuthentication.GetAuthCookie(user.Username, false);
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
            // Giữ thông tin ticket có sẵn và bổ sung AuthVersion vào UserData.
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

        /// <summary>
        /// Xóa cookie đăng nhập bằng Forms Authentication.
        /// </summary>
        protected void LoginSessionClear()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Chỉ cho phép chuyển hướng đến URL nội bộ sau đăng nhập.
        /// </summary>
        protected string GetLoginRedirectUrl(string returnUrl)
        {
            string redirectUrl = Url.IsLocalUrl(returnUrl)
                ? Url.Content(returnUrl)
                : Url.Action("Index", "Home");

            return redirectUrl;
        }
    }
}
