using System.Configuration;
using System.Data.SqlClient;

namespace UserManagement.Web.Util
{
    /// <summary>
    /// Tạo kết nối SQL từ cấu hình của ứng dụng.
    /// </summary>
    public static class DbConnectionFactory
    {
        /// <summary>
        /// Tạo kết nối SQL ở trạng thái đóng để nơi gọi quản lý việc sử dụng.
        /// </summary>
        public static SqlConnection CreateConnection()
        {
            string connectionString = LoadConnectionString();

            SqlConnection connection = new SqlConnection(connectionString);

            return connection;
        }

        /// <summary>
        /// Đọc chuỗi kết nối và báo lỗi cấu hình nếu thiếu hoặc rỗng.
        /// </summary>
        private static string LoadConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["UserManagementDb"];

            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigurationErrorsException("Chuỗi kết nối cơ sở dữ liệu chưa được cấu hình hoặc đang để trống.");
            }

            return settings.ConnectionString;
        }
    }
}
