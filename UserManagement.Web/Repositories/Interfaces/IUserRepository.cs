using System.Collections.Generic;
using UserManagement.Web.Models.Entities;
using UserManagement.Web.Models.DTOs;

namespace UserManagement.Web.Repositories
{
    /// <summary>
    /// Quy định các thao tác đọc và ghi dữ liệu người dùng.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Lấy tài khoản theo tên đăng nhập để xác thực.
        /// </summary>
        AppUser GetByUsername(string username);

        /// <summary>
        /// Đếm người dùng phù hợp với bộ lọc tìm kiếm.
        /// </summary>
        int Count(string username, string fullName, bool? isActive);

        /// <summary>
        /// Lấy danh sách hồ sơ theo bộ lọc và phân trang.
        /// </summary>
        List<UserDto> Search(string username, string fullName, bool? isActive, int page, int pageSize);

        /// <summary>
        /// Lấy hồ sơ người dùng theo ID.
        /// </summary>
        UserDto GetById(int id);

        /// <summary>
        /// Cập nhật hồ sơ và cho biết có đúng một dòng được cập nhật hay không.
        /// </summary>
        bool Update(int userId, string fullName, string email, bool isActive);

        /// <summary>
        /// Lưu tài khoản mới vào database.
        /// </summary>
        void Create(AppUser user);

        /// <summary>
        /// Cập nhật mật khẩu và tăng phiên bản xác thực của tài khoản.
        /// </summary>
        bool ChangePassword(int userId, byte[] newHash, byte[] newSalt, int iterations);

        /// <summary>
        /// Tăng phiên bản xác thực và cho biết cập nhật có thành công hay không.
        /// </summary>
        bool IncreaseAuthVersion(int userId);
    }
}
