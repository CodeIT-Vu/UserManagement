using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Models;
using UserManagement.Web.Util;

namespace UserManagement.Web.Repositories
{
    /// <summary>
    /// Truy cập dữ liệu người dùng bằng Dapper và stored procedure.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private const int DuplicateKeyErrorNumber = 2601;
        private const int UniqueConstraintViolationErrorNumber = 2627;

        /// <summary>
        /// Lấy dữ liệu tài khoản, gồm hash mật khẩu, để service xác thực.
        /// </summary>
        public AppUser GetByUsername(string username)
        {
            string procedureName = "dbo.usp_AppUser_GetByUsername";
            DynamicParameters parameters = new DynamicParameters(new { Username = username });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                AppUser user = connection.QuerySingleOrDefault<AppUser>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return user;
            }
        }

        /// <summary>
        /// Đọc tổng số kết quả và danh sách người dùng theo trang từ hai tập kết quả của proc.
        /// </summary>
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
                try
                {
                    connection.Execute(
                        procedureName, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (SqlException exception) when (exception.Number == DuplicateKeyErrorNumber || exception.Number == UniqueConstraintViolationErrorNumber)
                {
                    throw new DuplicateUsernameException(exception);
                }
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
                int rowsAffected = connection.ExecuteScalar<int>(
                    procedureName, parameters, commandType: CommandType.StoredProcedure);
                return rowsAffected == 1;
            }
        }

        public bool IncreaseAuthVersion(int userId)
        {
            string procedureName = "dbo.usp_AppUser_IncreaseAuthVersion";
            DynamicParameters parameters = new DynamicParameters(new { UserId = userId });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                int rowsAffected = connection.ExecuteScalar<int>(
                    procedureName, parameters, commandType: CommandType.StoredProcedure);

                return rowsAffected == 1;
            }
        }
    }
}
