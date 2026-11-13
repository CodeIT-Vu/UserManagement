namespace UserManagement.Web.Security
{
    /// <summary>
    /// Chứa hash và salt được tạo khi băm mật khẩu.
    /// </summary>
    public class PasswordHashResult
    {
        public byte[] Hash { get; set; }

        public byte[] Salt { get; set; }
    }
}
