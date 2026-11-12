using System.ComponentModel.DataAnnotations;
using UserManagement.Web.Constants;

namespace UserManagement.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(UserValidationLimits.UsernameMaxLength)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
