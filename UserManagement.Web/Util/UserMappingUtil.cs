using UserManagement.Web.DTOs;
using UserManagement.Web.Models;
using UserManagement.Web.ViewModels;
using System.Linq;

namespace UserManagement.Web.Util
{
    public static class UserMappingUtil
    {
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

        public static CurrentUserDto ToCurrentUserDto(AppUser user)
        {
            return new CurrentUserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                RoleName = user.RoleName
            };
        }
    }
}
