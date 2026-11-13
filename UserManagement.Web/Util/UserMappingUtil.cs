using UserManagement.Web.DTOs;
using UserManagement.Web.Models;
using UserManagement.Web.ViewModels;
using System.Linq;

namespace UserManagement.Web.Util
{
    /// <summary>
    /// Chuyển dữ liệu người dùng sang DTO và ViewModel.
    /// </summary>
    public static class UserMappingUtil
    {
        /// <summary>
        /// Chuyển dữ liệu hồ sơ sang thông tin hiển thị chi tiết.
        /// </summary>
        public static UserDetailsViewModel ToDetailsViewModel(UserData user)
        {
            return new UserDetailsViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        /// <summary>
        /// Chuyển dữ liệu hồ sơ sang các trường được phép chỉnh sửa.
        /// </summary>
        public static UserEditViewModel ToEditViewModel(UserData user)
        {
            return new UserEditViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive
            };
        }

        /// <summary>
        /// Ghép danh sách người dùng với điều kiện tìm kiếm và thông tin phân trang.
        /// </summary>
        public static UserListViewModel ToListViewModel(
            UserSearchResultData result, UserSearchViewModel search, int page, int pageSize, int totalPages)
        {
            return new UserListViewModel
            {
                Users = result.Users.Select(ToDetailsViewModel).ToList(),
                TotalCount = result.TotalCount,
                Search = search,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Chỉ lấy thông tin cần cho xác thực, không đưa hash và salt ra DTO.
        /// </summary>
        public static CurrentUserDto ToCurrentUserDto(AppUser user)
        {
            return new CurrentUserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                RoleName = user.RoleName,
                AuthVersion = user.AuthVersion
            };
        }

    }
}
