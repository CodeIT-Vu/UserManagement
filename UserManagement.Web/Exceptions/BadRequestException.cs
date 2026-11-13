namespace UserManagement.Web.Exceptions
{
    /// <summary>
    /// Biểu diễn lỗi dữ liệu hoặc nghiệp vụ với mã HTTP 400.
    /// </summary>
    public class BadRequestException : ApiException
    {
        /// <summary>
        /// Khởi tạo lỗi HTTP 400 với mã lỗi và thông báo nghiệp vụ.
        /// </summary>
        public BadRequestException(string errorCode, string userMessage)
            : base(400, errorCode, userMessage)
        {
        }
    }
}
