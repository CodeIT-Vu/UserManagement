using System;

namespace UserManagement.Web.Exceptions
{
    /// <summary>
    /// Biểu diễn lỗi trùng tên đăng nhập khi lưu dữ liệu.
    /// </summary>
    public class DuplicateUsernameException : Exception
    {
        /// <summary>
        /// Khởi tạo lỗi trùng tên đăng nhập và giữ exception gốc nếu có.
        /// </summary>
        public DuplicateUsernameException(Exception innerException = null)
            : base("Username already exists.", innerException)
        {
        }
    }
}
