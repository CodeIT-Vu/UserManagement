namespace UserManagement.Web.Models.DTOs
{
    /// <summary>
    /// Chứa thông tin người dùng hiện tại, không chứa dữ liệu mật khẩu.
    /// </summary>
    public class CurrentUserDto
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; }

        public int AuthVersion { get; set; }
    }
}
