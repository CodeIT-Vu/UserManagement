namespace UserManagement.Web.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string errorCode, string userMessage)
            : base(400, errorCode, userMessage)
        {
        }
    }
}
