namespace UserManagement.Web.Util
{
    public static class PaginationUtil
    {
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
