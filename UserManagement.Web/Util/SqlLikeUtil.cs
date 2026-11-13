namespace UserManagement.Web.Util
{
    /// <summary>
    /// Xử lý ký tự đặc biệt trong điều kiện SQL LIKE.
    /// </summary>
    public static class SqlLikeUtil
    {
        /// <summary>
        /// Escape ký tự wildcard và dấu escape để tìm theo nội dung người dùng nhập.
        /// </summary>
        public static string EscapeWildcards(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Escape dấu \ trước để không xử lý lại dấu vừa thêm.
            return input.Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_")
                .Replace("[", @"\[");
        }
    }
}
