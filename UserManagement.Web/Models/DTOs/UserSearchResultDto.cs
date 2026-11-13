using System.Collections.Generic;

namespace UserManagement.Web.Models.DTOs
{
    /// <summary>
    /// Chứa tổng số kết quả và danh sách người dùng của một trang.
    /// </summary>
    public class UserSearchResultDto
    {
        public int TotalCount { get; set; }
        public List<UserDto> Users { get; set; } = new List<UserDto>();
    }
}
