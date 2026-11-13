namespace UserManagement.Web.Models
{
    /// <summary>
    /// Chứa dữ liệu tài khoản và mật khẩu đã băm để xác thực nội bộ.
    /// </summary>
    public class AppUser
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public int PasswordIterations { get; set; }

        public bool IsActive { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int AuthVersion { get; set; }
    }
}
