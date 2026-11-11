using System;

namespace UserManagement.Web.Exceptions
{
    public class ApiException : Exception
    {
        public int HttpStatusCode { get; private set; }

        public string ErrorCode { get; private set; }

        public string UserMessage { get; private set; }

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
