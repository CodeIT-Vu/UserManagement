using System.Collections.Generic;

namespace UserManagement.Web.Exceptions
{
    /// <summary>
    /// Mang lỗi validation theo từng trường nhập liệu.
    /// </summary>
    public class RequestValidationException : BadRequestException
    {
        public Dictionary<string, string[]> Errors { get; private set; }

        /// <summary>
        /// Lưu danh sách lỗi theo trường để trả về form.
        /// </summary>
        public RequestValidationException(string userMessage, Dictionary<string, string[]> errors)
            : base(Constants.ErrorCode.InvalidRequest, userMessage)
        {
            Errors = errors;
        }

    }
}
