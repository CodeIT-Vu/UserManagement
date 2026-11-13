namespace UserManagement.Web.Models
{
    /// <summary>
    /// Chứa kết quả thành công và dữ liệu trả về cho trình duyệt.
    /// </summary>
    public class ApiSuccessResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string Data { get; set; }
    }
}
