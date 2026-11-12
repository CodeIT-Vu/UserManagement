using System.Collections.Generic;

namespace UserManagement.Web.ViewModels
{
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
