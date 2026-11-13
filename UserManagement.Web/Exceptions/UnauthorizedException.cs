namespace UserManagement.Web.Exceptions
{
    /// <summary>
    /// Biểu diễn lỗi xác thực với mã HTTP 401.
    /// </summary>
    public class UnauthorizedException : ApiException
    {
        /// <summary>
        /// Khởi tạo lỗi HTTP 401 với mã lỗi và thông báo xác thực.
        /// </summary>
        public UnauthorizedException(string errorCode, string userMessage)
            : base(401, errorCode, userMessage)
        {
        }
    }
}
