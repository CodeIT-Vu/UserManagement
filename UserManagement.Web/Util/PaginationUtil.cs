namespace UserManagement.Web.Util
{
    /// <summary>
    /// Tính toán thông tin phân trang dùng chung.
    /// </summary>
    public static class PaginationUtil
    {
        /// <summary>
        /// Tính tổng số trang, cộng thêm một trang nếu còn phần dư.
        /// </summary>
        public static int CalculateTotalPages(int totalCount, int pageSize)
        {
            if (pageSize == 0)
            {
                return 0;
            }

            int fullPages = totalCount / pageSize;
            int extraPage = totalCount % pageSize > 0 ? 1 : 0;
            return fullPages + extraPage;
        }
    }
}
