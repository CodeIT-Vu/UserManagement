using System.Collections.Generic;

namespace UserManagement.Web.Exceptions
{
    public class RequestValidationException : BadRequestException
    {
        public Dictionary<string, string[]> Errors { get; private set; }

        public RequestValidationException(string userMessage, Dictionary<string, string[]> errors)
            : base(Constants.ErrorCode.InvalidRequest, userMessage)
        {
            Errors = errors;
        }

    }
}
