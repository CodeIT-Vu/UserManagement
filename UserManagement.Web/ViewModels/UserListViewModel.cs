using System.Collections.Generic;

namespace UserManagement.Web.ViewModels
{
    /// <summary>
    /// Chứa danh sách người dùng, bộ lọc và thông tin phân trang.
    /// </summary>
    public class UserListViewModel
    {
        public UserSearchViewModel Search { get; set; }

        public List<UserDetailsViewModel> Users { get; set; } = new List<UserDetailsViewModel>();

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }
    }
}
