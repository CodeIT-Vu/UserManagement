using System.Collections.Generic;

namespace UserManagement.Web.Models
{
    /// <summary>
    /// Chứa tổng số kết quả và danh sách người dùng của một trang.
    /// </summary>
    public class UserSearchResultData
    {
        public int TotalCount { get; set; }
        public List<UserData> Users { get; set; } = new List<UserData>();
    }
}
