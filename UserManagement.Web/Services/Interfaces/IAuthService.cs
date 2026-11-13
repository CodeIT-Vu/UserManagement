using UserManagement.Web.DTOs;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    /// <summary>
    /// Quy định các nghiệp vụ xác thực và quản lý mật khẩu.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Xác thực thông tin đăng nhập và trả về người dùng an toàn.
        /// </summary>
        CurrentUserDto Authenticate(LoginViewModel model);

        /// <summary>
        /// Lấy thông tin người dùng còn hoạt động để kiểm tra ticket.
        /// </summary>
        CurrentUserDto FindActiveUser(string username);

        /// <summary>
        /// Thu hồi các phiên đăng nhập hiện tại của cùng tài khoản.
        /// </summary>
        void Logout(int userId);

        /// <summary>
        /// Kiểm tra dữ liệu và tạo tài khoản mới.
        /// </summary>
        void Register(RegisterViewModel model);

        /// <summary>
        /// Xác minh mật khẩu hiện tại và lưu mật khẩu mới.
        /// </summary>
        void ChangePassword(ChangePasswordViewModel model, string currentUsername);
    }
}
