using System.Collections.Generic;

namespace UserManagement.Web.Exceptions
{
    public class ValidationException : BadRequestException
    {
        public Dictionary<string, string[]> Errors { get; private set; }

        public ValidationException(string fieldName, string message)
            : base(Constants.ErrorCode.InvalidRequest, Constants.ErrorMessages.InvalidRequest)
        {
            Errors = new Dictionary<string, string[]>();
            Errors.Add(fieldName, new[] { message });
        }

        public ValidationException(string userMessage, Dictionary<string, string[]> errors)
            : base(Constants.ErrorCode.InvalidRequest, userMessage)
        {
            Errors = errors;
        }
    }
}
