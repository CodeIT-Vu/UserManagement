using System.Web.Mvc;
using UserManagement.Web.Filters;
using UserManagement.Web.DTOs;
using UserManagement.Web.Models;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = returnUrl;
            return View(model);
        }

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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
        public ActionResult Register(RegisterViewModel model)
        {
            _authService.Register(model);
            string loginUrl = Url.Action("Login", new { registered = true });

            ApiSuccessResponse response = new ApiSuccessResponse();
            response.Success = true;
            response.Data = loginUrl;
            return Json(response);
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            return View(model);
        }

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
