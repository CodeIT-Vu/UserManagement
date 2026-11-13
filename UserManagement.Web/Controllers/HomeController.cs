using System.Web.Mvc;
namespace UserManagement.Web.Controllers
{
    /// <summary>
    /// Hiển thị trang chủ cho người dùng đã đăng nhập.
    /// </summary>
    [Authorize]
    public class HomeController : BaseController
    {
        /// <summary>
        /// Hiển thị trang chủ.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}
