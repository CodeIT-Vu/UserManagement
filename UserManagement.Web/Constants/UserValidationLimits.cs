namespace UserManagement.Web.Constants
{
    /// <summary>
    /// Khai báo giới hạn độ dài dùng chung khi kiểm tra dữ liệu người dùng.
    /// </summary>
    public static class UserValidationLimits
    {
        public const int UsernameMaxLength = 50;
        public const int FullNameMaxLength = 150;
        public const int EmailMaxLength = 254;
        public const int PasswordMinLength = 8;
        public const int PasswordMaxLength = 100;
    }
}
