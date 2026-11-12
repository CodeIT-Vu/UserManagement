using UserManagement.Web.Models;

namespace UserManagement.Web.Repositories
{
    public interface IUserRepository
    {
        AppUser GetByUsername(string username);

        UserSearchResultData Search(string username, string fullName, bool? isActive, int page, int pageSize);

        UserData GetById(int id);

        bool Update(int userId, string fullName, string email, bool isActive);

        void Create(AppUser user);

        bool ChangePassword(int userId, byte[] newHash, byte[] newSalt, int iterations);

        bool IncreaseAuthVersion(int userId);
    }
}
