using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using UserManagement.Web.Models;
using UserManagement.Web.Util;

namespace UserManagement.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        public AppUser GetByUsername(string username)
        {
            string procedureName = "dbo.usp_AppUser_GetByUsername";
            DynamicParameters parameters = new DynamicParameters(new { Username = username });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();

                AppUser user = connection.QuerySingleOrDefault<AppUser>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return user;
            }
        }

        public UserSearchResultData Search(string username, string fullName, bool? isActive, int page, int pageSize)
        {
            string procedureName = "dbo.usp_AppUser_Search";

            long pageNumber = page;
            long offset = (pageNumber - 1) * pageSize;

            DynamicParameters parameters = new DynamicParameters(new
            {
                Username = username ?? "",
                FullName = fullName ?? "",
                IsActive = isActive,
                Offset = offset,
                PageSize = pageSize
            });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();

                using (SqlMapper.GridReader results = connection.QueryMultiple(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    UserSearchResultData result = results.ReadSingle<UserSearchResultData>();
                    result.Users = results.Read<UserData>().ToList();

                    return result;
                }
            }
        }

        public UserData GetById(int id)
        {
            string procedureName = "dbo.usp_AppUser_GetById";
            DynamicParameters parameters = new DynamicParameters(new { UserId = id });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();

                UserData user = connection.QuerySingleOrDefault<UserData>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return user;
            }
        }

        public bool Update(int userId, string fullName, string email, bool isActive)
        {
            string procedureName = "dbo.usp_AppUser_Update";
            DynamicParameters parameters = new DynamicParameters(new
            {
                UserId = userId,
                FullName = fullName,
                Email = email,
                IsActive = isActive
            });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();

                int rowsAffected = connection.ExecuteScalar<int>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return rowsAffected == 1;
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

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
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

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                connection.Open();
                int rowsAffected = connection.ExecuteScalar<int>(
                    procedureName, parameters, commandType: CommandType.StoredProcedure);
                return rowsAffected == 1;
            }
        }
    }
}
