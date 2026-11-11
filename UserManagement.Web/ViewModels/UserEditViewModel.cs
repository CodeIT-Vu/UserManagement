using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.ViewModels
{
    public class UserEditViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Mã người dùng không hợp lệ.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(150, ErrorMessage = "Họ tên tối đa 150 ký tự.")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(254, ErrorMessage = "Email tối đa 254 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; }
    }
}
