namespace UserManagement.Web.DTOs
{
    public class CurrentUserDto
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string RoleName { get; set; }

        public int AuthVersion { get; set; }
    }
}
