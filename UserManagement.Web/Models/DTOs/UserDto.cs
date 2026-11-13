namespace UserManagement.Web.Models.DTOs
{
    /// <summary>
    /// Chứa dữ liệu hồ sơ người dùng đọc từ repository.
    /// </summary>
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
