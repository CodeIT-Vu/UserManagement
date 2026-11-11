using System.Web.Mvc;
using UserManagement.Web.Filters;
using UserManagement.Web.Models;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ValidateRequest]
        public ActionResult Index(UserSearchViewModel model)
        {
            UserListViewModel result = _userService.Search(model);
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ValidateRequest]
        public ActionResult Details(int id = 0)
        {
            UserDetailsViewModel user = _userService.GetDetails(id);
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ValidateRequest]
        public ActionResult Edit(int id = 0)
        {
            UserEditViewModel model = _userService.GetEdit(id);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateRequest]
        public ActionResult Update(UserEditViewModel model)
        {
            _userService.Update(model, User.Identity.Name);
            string redirectUrl = Url.Action("Details", new { id = model.UserId, saved = true });

            ApiSuccessResponse response = new ApiSuccessResponse();
            response.Success = true;
            response.Data = redirectUrl;

            return Json(response);
        }
    }
}
