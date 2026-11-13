namespace UserManagement.Web.Models.DTOs
{
    /// <summary>
    /// Chứa dữ liệu đăng ký tài khoản truyền từ controller sang service.
    /// </summary>
    public class RegisterDto
    {
        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
