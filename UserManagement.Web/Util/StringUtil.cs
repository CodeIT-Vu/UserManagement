namespace UserManagement.Web.Util
{
    public static class StringUtil
    {
        public static string CleanText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
