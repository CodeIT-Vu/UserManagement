using System.ComponentModel.DataAnnotations;
using UserManagement.Web.Constants;

namespace UserManagement.Web.ViewModels
{
    /// <summary>
    /// Chứa điều kiện tìm kiếm và số trang người dùng yêu cầu.
    /// </summary>
    public class UserSearchViewModel
    {
        [StringLength(UserValidationLimits.UsernameMaxLength, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [StringLength(UserValidationLimits.FullNameMaxLength, ErrorMessage = "Họ tên tối đa 150 ký tự.")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? IsActive { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0.")]
        public int Page { get; set; } = 1;
    }
}
