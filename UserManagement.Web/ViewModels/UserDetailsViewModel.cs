namespace UserManagement.Web.ViewModels
{
    /// <summary>
    /// Chứa thông tin hiển thị trên trang chi tiết người dùng.
    /// </summary>
    public class UserDetailsViewModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}
