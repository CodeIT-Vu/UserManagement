namespace UserManagement.Web.Security
{
    public class PasswordHashResult
    {
        public byte[] Hash { get; set; }

        public byte[] Salt { get; set; }
    }
}
