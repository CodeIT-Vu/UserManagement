namespace UserManagement.Web.Constants
{
    public static class ErrorMessages
    {
        public const string PageNotFound = "Không tìm thấy trang.";
        public const string AuthenticationRequired = "Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.";
        public const string InvalidLogin = "Tên đăng nhập hoặc mật khẩu không đúng.";
        public const string UserNotFound = "Không tìm thấy người dùng.";
        public const string InvalidRequest = "Dữ liệu nhập không hợp lệ.";
        public const string AccessDenied = "Bạn không có quyền truy cập.";
        public const string Unexpected = "Có lỗi xảy ra, vui lòng thử lại.";
        public const string DatabaseConnectionStringMissing = "Chuỗi kết nối cơ sở dữ liệu chưa được cấu hình hoặc đang để trống.";
        public const string CannotDeactivateOwnAccount = "Bạn không thể tự vô hiệu hoá tài khoản đang đăng nhập.";
        public const string UsernameAlreadyExists = "Tên đăng nhập đã được sử dụng.";
        public const string CurrentPasswordIncorrect = "Mật khẩu hiện tại không đúng.";
        public const string AccountUnavailable = "Tài khoản không còn tồn tại. Vui lòng đăng nhập lại.";
        public const string PasswordRequired = "Password cannot be empty.";
    }
}
