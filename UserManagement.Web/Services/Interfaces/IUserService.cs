using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    /// <summary>
    /// Quy định các nghiệp vụ tìm kiếm, xem và cập nhật người dùng.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Chuẩn hóa bộ lọc và trả danh sách người dùng theo trang.
        /// </summary>
        UserListViewModel Search(UserSearchViewModel model);

        /// <summary>
        /// Lấy dữ liệu hiển thị chi tiết của người dùng.
        /// </summary>
        UserDetailsViewModel GetDetails(int id);

        /// <summary>
        /// Lấy dữ liệu để chỉnh sửa hồ sơ người dùng.
        /// </summary>
        UserEditViewModel GetEdit(int id);

        /// <summary>
        /// Cập nhật hồ sơ theo các quy tắc nghiệp vụ.
        /// </summary>
        void Update(UserEditViewModel model, string currentUsername);
    }
}
