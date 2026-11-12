using System.Configuration;
using System.Data.SqlClient;

namespace UserManagement.Web.Util
{
    public static class DbConnectionFactory
    {
        public static SqlConnection CreateConnection()
        {
            string connectionString = LoadConnectionString();

            SqlConnection connection = new SqlConnection(connectionString);

            return connection;
        }

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
