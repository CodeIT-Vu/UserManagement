using System;

namespace UserManagement.Web.Exceptions
{
    public class DuplicateUsernameException : Exception
    {
        public DuplicateUsernameException(Exception innerException = null)
            : base("Username already exists.", innerException)
        {
        }
    }
}
