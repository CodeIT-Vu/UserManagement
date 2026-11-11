using System.Configuration;
using System.Data.SqlClient;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;

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
            var settings = ConfigurationManager.ConnectionStrings["UserManagementDb"];

            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ApiException(500, ErrorCode.DatabaseConnectionStringMissing, ErrorMessages.DatabaseConnectionStringMissing);
            }

            return settings.ConnectionString;
        }
    }
}
