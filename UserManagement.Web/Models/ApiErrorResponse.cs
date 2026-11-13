using System.Collections.Generic;

namespace UserManagement.Web.Models
{
    /// <summary>
    /// Chứa thông báo lỗi, lỗi từng trường và mã tra cứu khi cần.
    /// </summary>
    public class ApiErrorResponse
    {
        public bool Success
        {
            get { return false; }
        }

        public string Code { get; set; }

        public string Message { get; set; }

        public string TraceId { get; set; }

        public Dictionary<string, string[]> Errors { get; set; }
    }
}
