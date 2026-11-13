namespace UserManagement.Web.Constants
{
    /// <summary>
    /// Khai báo mã trạng thái HTTP dùng chung cho ứng dụng.
    /// </summary>
    public static class HttpStatusCodes
    {
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int InternalServerError = 500;
    }
}
