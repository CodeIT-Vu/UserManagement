using System.Collections.Generic;

namespace UserManagement.Web.Models
{
    public class UserSearchResultData
    {
        public int TotalCount { get; set; }
        public List<UserData> Users { get; set; } = new List<UserData>();
    }
}
