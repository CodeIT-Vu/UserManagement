using System.Web.Mvc;
using UserManagement.Web.Filters;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;
using UserManagement.Web.Models.DTOs;
using UserManagement.Web.Models.Responses;

namespace UserManagement.Web.Controllers
{
    /// <summary>
    /// Tiếp nhận yêu cầu đăng nhập, đăng ký, đăng xuất và đổi mật khẩu.
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Nhận service xác thực qua constructor.
        /// </summary>
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Hiển thị form đăng nhập và giữ URL cần quay lại.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        /// <summary>
        /// Xác thực tài khoản, tạo cookie và trả URL chuyển hướng.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
        public ActionResult Login(LoginViewModel model)
        {
            CurrentUserDto user = _authService.Authenticate(model);
            SetLoginSessionInfo(user);

            string redirectUrl = GetLoginRedirectUrl(model.ReturnUrl);

            ApiSuccessResponse response = new ApiSuccessResponse();
            response.Success = true;
            response.Data = redirectUrl;

            return Json(response);
        }

        /// <summary>
        /// Thu hồi các phiên của tài khoản trước khi xóa cookie đăng nhập.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            CurrentUserDto user = (CurrentUserDto)HttpContext.Items["CurrentUser"];
            _authService.Logout(user.UserId);
            LoginSessionClear();

            string loginUrl = Url.Action("Login", "Account");

            ApiSuccessResponse response = new ApiSuccessResponse();
            response.Success = true;
            response.Data = loginUrl;

            return Json(response);
        }

        /// <summary>
        /// Hiển thị form đăng ký tài khoản.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        /// <summary>
        /// Đăng ký tài khoản và trả URL đến trang đăng nhập.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
        public ActionResult Register(RegisterViewModel model)
        {
            RegisterDto request = new RegisterDto
            {
                Username = model.Username,
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password
            };
            _authService.Register(request);
            string loginUrl = Url.Action("Login", new { registered = true });

            ApiSuccessResponse response = new ApiSuccessResponse();
            response.Success = true;
            response.Data = loginUrl;
            return Json(response);
        }

        /// <summary>
        /// Hiển thị form đổi mật khẩu.
        /// </summary>
        [HttpGet]
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            return View(model);
        }

        /// <summary>
        /// Đổi mật khẩu, xóa cookie và yêu cầu đăng nhập lại.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            _authService.ChangePassword(model, User.Identity.Name);
            LoginSessionClear();
            string loginUrl = Url.Action("Login", new { passwordChanged = true });

            ApiSuccessResponse response = new ApiSuccessResponse();
            response.Success = true;
            response.Data = loginUrl;
            return Json(response);
        }
    }
}
