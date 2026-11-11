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

        public int TotalPages
        {
            get
            {
                if (PageSize == 0)
                {
                    return 0;
                }

                int totalPages = TotalCount / PageSize;

                if (TotalCount % PageSize > 0)
                {
                    totalPages++;
                }

                return totalPages;
            }
        }
    }
}
