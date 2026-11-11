using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using UserManagement.Web.Models;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString = ConfigurationManager
            .ConnectionStrings["UserManagementDb"].ConnectionString;

        public AppUser GetByUsername(string username)
        {
            string procedureName = "dbo.usp_AppUser_GetByUsername";
            DynamicParameters parameters = new DynamicParameters(new { Username = username });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                AppUser user = connection.QuerySingleOrDefault<AppUser>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return user;
            }
        }

        public UserListViewModel Search(UserSearchViewModel model, int pageSize)
        {
            string procedureName = "dbo.usp_AppUser_Search";

            long pageNumber = model.Page;
            long offset = (pageNumber - 1) * pageSize;

            string username = model.Username;
            if (username == null)
            {
                username = "";
            }

            string fullName = model.FullName;
            if (fullName == null)
            {
                fullName = "";
            }

            DynamicParameters parameters = new DynamicParameters(new
            {
                Username = username,
                FullName = fullName,
                IsActive = model.IsActive,
                Offset = offset,
                PageSize = pageSize
            });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlMapper.GridReader results = connection.QueryMultiple(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    UserListViewModel page = results.ReadSingle<UserListViewModel>();
                    page.Page = model.Page;
                    page.PageSize = pageSize;

                    page.Users = results.Read<UserDetailsViewModel>().ToList();

                    return page;
                }
            }
        }

        public UserDetailsViewModel GetById(int id)
        {
            string procedureName = "dbo.usp_AppUser_GetById";
            DynamicParameters parameters = new DynamicParameters(new { UserId = id });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                UserDetailsViewModel user = connection.QuerySingleOrDefault<UserDetailsViewModel>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return user;
            }
        }

        public bool Update(UserEditViewModel model)
        {
            string procedureName = "dbo.usp_AppUser_Update";
            DynamicParameters parameters = new DynamicParameters(new
            {
                UserId = model.UserId,
                FullName = model.FullName,
                Email = model.Email,
                IsActive = model.IsActive
            });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                int rowsAffected = connection.ExecuteScalar<int>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (rowsAffected == 1)
                {
                    return true;
                }

                return false;
            }
        }

        public void Create(AppUser user)
        {
            string procedureName = "dbo.usp_AppUser_Create";
            DynamicParameters parameters = new DynamicParameters(new
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                PasswordIterations = user.PasswordIterations
            });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                connection.Execute(
                    procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public bool ChangePassword(int userId, byte[] newHash, byte[] newSalt, int iterations)
        {
            string procedureName = "dbo.usp_AppUser_ChangePassword";
            DynamicParameters parameters = new DynamicParameters(new
            {
                UserId = userId,
                NewHash = newHash,
                NewSalt = newSalt,
                PasswordIterations = iterations
            });

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                int rowsAffected = connection.ExecuteScalar<int>(
                    procedureName, parameters, commandType: CommandType.StoredProcedure);
                return rowsAffected == 1;
            }
        }
    }
}
