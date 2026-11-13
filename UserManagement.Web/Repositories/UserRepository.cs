using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Util;
using UserManagement.Web.Models.Entities;
using UserManagement.Web.Models.DTOs;

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
        /// Đếm người dùng theo bộ lọc trước khi tính trang hợp lệ.
        /// </summary>
        public int Count(string username, string fullName, bool? isActive)
        {
            string procedureName = "dbo.usp_AppUser_Count";
            DynamicParameters parameters = new DynamicParameters(new
            {
                Username = SqlLikeUtil.EscapeWildcards(username) ?? "",
                FullName = SqlLikeUtil.EscapeWildcards(fullName) ?? "",
                IsActive = isActive
            });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                int totalCount = connection.ExecuteScalar<int>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return totalCount;
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng theo bộ lọc và trang đã được điều chỉnh.
        /// </summary>
        public List<UserDto> Search(string username, string fullName, bool? isActive, int page, int pageSize)
        {
            string procedureName = "dbo.usp_AppUser_Search";

            long pageNumber = page;
            long offset = (pageNumber - 1) * pageSize;

            DynamicParameters parameters = new DynamicParameters(new
            {
                Username = SqlLikeUtil.EscapeWildcards(username) ?? "",
                FullName = SqlLikeUtil.EscapeWildcards(fullName) ?? "",
                IsActive = isActive,
                Offset = offset,
                PageSize = pageSize
            });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                List<UserDto> users = connection.Query<UserDto>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure).ToList();

                return users;
            }
        }

        /// <summary>
        /// Lấy hồ sơ người dùng theo ID, không lấy dữ liệu mật khẩu.
        /// </summary>
        public UserDto GetById(int id)
        {
            string procedureName = "dbo.usp_AppUser_GetById";
            DynamicParameters parameters = new DynamicParameters(new { UserId = id });

            using (SqlConnection connection = DbConnectionFactory.CreateConnection())
            {
                UserDto user = connection.QuerySingleOrDefault<UserDto>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return user;
            }
        }

        /// <summary>
        /// Cập nhật hồ sơ và trả kết quả dựa trên số dòng được cập nhật.
        /// </summary>
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

        /// <summary>
        /// Tạo tài khoản và chuyển lỗi trùng khóa SQL thành lỗi trùng tên đăng nhập.
        /// </summary>
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

        /// <summary>
        /// Lưu mật khẩu mới và tăng AuthVersion trong cùng stored procedure.
        /// </summary>
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

        /// <summary>
        /// Tăng phiên bản xác thực của tài khoản và kiểm tra số dòng được cập nhật.
        /// </summary>
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
