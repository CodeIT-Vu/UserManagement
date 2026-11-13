using System;

namespace UserManagement.Web.Exceptions
{
    /// <summary>
    /// Mang mã HTTP, mã lỗi và thông báo để xử lý lỗi thống nhất.
    /// </summary>
    public class ApiException : Exception
    {
        public int HttpStatusCode { get; private set; }

        public string ErrorCode { get; private set; }

        public string UserMessage { get; private set; }

        /// <summary>
        /// Lưu thông tin phản hồi lỗi và giữ exception gốc nếu có.
        /// </summary>
        public ApiException(
            int httpStatusCode,
            string errorCode,
            string userMessage,
            string technicalMessage = null,
            Exception innerException = null)
            : base(technicalMessage ?? userMessage, innerException)
        {
            HttpStatusCode = httpStatusCode;
            ErrorCode = errorCode;
            UserMessage = userMessage;
        }
    }
}
