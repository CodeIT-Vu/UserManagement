using System.ComponentModel.DataAnnotations;
using UserManagement.Web.Constants;

namespace UserManagement.Web.ViewModels
{
    public class RegisterViewModel
    {
        private const int UsernameMinLength = 3;

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(UserValidationLimits.UsernameMaxLength, MinimumLength = UsernameMinLength, ErrorMessage = "Tên đăng nhập phải có 3–50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ Latin, số, dấu gạch dưới, gạch ngang và dấu chấm.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(UserValidationLimits.FullNameMaxLength, ErrorMessage = "Họ tên tối đa 150 ký tự.")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(UserValidationLimits.EmailMaxLength, ErrorMessage = "Email tối đa 254 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(UserValidationLimits.PasswordMaxLength, MinimumLength = UserValidationLimits.PasswordMinLength, ErrorMessage = "Mật khẩu phải có 8–100 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}
