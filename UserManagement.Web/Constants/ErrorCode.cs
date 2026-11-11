namespace UserManagement.Web.Constants
{
    public static class ErrorCode
    {
        public const string AuthenticationRequired = "AUTH_REQUIRED";
        public const string InvalidLogin = "INVALID_CREDENTIALS";
        public const string PageNotFound = "PAGE_NOT_FOUND";
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string InvalidRequest = "VALIDATION_FAILED";
        public const string AccessDenied = "ACCESS_DENIED";
        public const string Unexpected = "UNEXPECTED_ERROR";
        public const string DatabaseConnectionStringMissing = "DATABASE_CONNECTION_STRING_MISSING";
        public const string CannotDeactivateOwnAccount = "CANNOT_DEACTIVATE_OWN_ACCOUNT";
        public const string UsernameAlreadyExists = "USERNAME_ALREADY_EXISTS";
        public const string CurrentPasswordIncorrect = "CURRENT_PASSWORD_INCORRECT";
        public const string AccountUnavailable = "ACCOUNT_UNAVAILABLE";
    }
}
