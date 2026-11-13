using System.Web.Mvc;
using UserManagement.Web.Filters;
using UserManagement.Web.Models;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Controllers
{
    /// <summary>
    /// Tiếp nhận yêu cầu tìm kiếm, xem và sửa người dùng của Admin.
    /// </summary>
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Nhận service quản lý người dùng qua constructor.
        /// </summary>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Tìm kiếm người dùng và hiển thị danh sách theo trang.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ValidateModelStateAttribute]
        public ActionResult Index(UserSearchViewModel model)
        {
            UserListViewModel result = _userService.Search(model);
            return View(result);
        }

        /// <summary>
        /// Lấy thông tin người dùng để hiển thị trang chi tiết.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ValidateModelStateAttribute]
        public ActionResult Details(int id = 0)
        {
            UserDetailsViewModel user = _userService.GetDetails(id);
            return View(user);
        }

        /// <summary>
        /// Lấy dữ liệu người dùng để hiển thị form chỉnh sửa.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ValidateModelStateAttribute]
        public ActionResult Edit(int id = 0)
        {
            UserEditViewModel model = _userService.GetEdit(id);
            return View(model);
        }

        /// <summary>
        /// Cập nhật người dùng và trả URL đến trang chi tiết.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelStateAttribute]
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
