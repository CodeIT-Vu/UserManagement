namespace UserManagement.Web.Util
{
    /// <summary>
    /// Chuẩn hóa chuỗi nhập liệu dùng chung.
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// Cắt khoảng trắng hai đầu và đổi chuỗi rỗng thành null.
        /// </summary>
        public static string CleanText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
