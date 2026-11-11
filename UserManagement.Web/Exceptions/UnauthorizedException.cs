namespace UserManagement.Web.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string errorCode, string userMessage)
            : base(401, errorCode, userMessage)
        {
        }
    }
}
