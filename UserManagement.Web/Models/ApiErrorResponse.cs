using System.Collections.Generic;

namespace UserManagement.Web.Models
{
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
