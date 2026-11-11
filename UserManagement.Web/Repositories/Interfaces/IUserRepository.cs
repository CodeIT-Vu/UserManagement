using UserManagement.Web.Models;

namespace UserManagement.Web.Repositories
{
    using UserManagement.Web.ViewModels;

    public interface IUserRepository
    {
        AppUser GetByUsername(string username);

        UserListViewModel Search(UserSearchViewModel model, int pageSize);

        UserDetailsViewModel GetById(int id);

        bool Update(UserEditViewModel model);

        void Create(AppUser user);

        bool ChangePassword(int userId, byte[] newHash, byte[] newSalt, int iterations);
    }
}
