using System.ComponentModel.DataAnnotations;
using UserManagement.Web.Constants;

namespace UserManagement.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [StringLength(UserValidationLimits.PasswordMaxLength, MinimumLength = UserValidationLimits.PasswordMinLength, ErrorMessage = "Mật khẩu mới phải có 8–100 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu mới không khớp.")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string ConfirmPassword { get; set; }
    }
}
