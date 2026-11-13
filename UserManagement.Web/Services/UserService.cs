using System;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Repositories;
using UserManagement.Web.Util;
using UserManagement.Web.ViewModels;
using UserManagement.Web.Models.DTOs;

namespace UserManagement.Web.Services
{
    /// <summary>
    /// Xử lý nghiệp vụ quản lý người dùng và chuyển dữ liệu sang ViewModel.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private const int DefaultPageSize = 10;

        /// <summary>
        /// Nhận repository người dùng qua constructor.
        /// </summary>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Chuẩn hóa điều kiện tìm kiếm và điều chỉnh phân trang theo tổng số kết quả.
        /// </summary>
        public UserListViewModel Search(UserSearchViewModel model)
        {
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            model.Username = StringUtil.CleanText(model.Username);
            model.FullName = StringUtil.CleanText(model.FullName);

            int totalCount = _userRepository.Count(model.Username, model.FullName, model.IsActive);
            int totalPages = PaginationUtil.CalculateTotalPages(totalCount, DefaultPageSize);

            // Điều chỉnh trang trước khi truy vấn danh sách.
            if (totalPages > 0 && model.Page > totalPages)
            {
                model.Page = totalPages;
            }

            if (totalCount == 0)
            {
                model.Page = 1;
            }

            UserSearchResultDto data = new UserSearchResultDto
            {
                TotalCount = totalCount,
                Users = _userRepository.Search(model.Username, model.FullName, model.IsActive, model.Page, DefaultPageSize)
            };

            return UserMappingUtil.ToListViewModel(data, model, model.Page, DefaultPageSize, totalPages);
        }

        /// <summary>
        /// Lấy người dùng bắt buộc tồn tại và chuyển sang dữ liệu trang chi tiết.
        /// </summary>
        public UserDetailsViewModel GetDetails(int id)
        {
            return UserMappingUtil.ToDetailsViewModel(GetRequiredUser(id));
        }

        /// <summary>
        /// Kiểm tra ID và báo lỗi 404 nếu người dùng không tồn tại.
        /// </summary>
        private UserDto GetRequiredUser(int id)
        {
            if (id <= 0)
            {
                throw new ApiException(HttpStatusCodes.NotFound, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }

            UserDto user = _userRepository.GetById(id);
            if (user == null)
            {
                throw new ApiException(HttpStatusCodes.NotFound, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }

            return user;
        }

        /// <summary>
        /// Lấy người dùng bắt buộc tồn tại và chuyển sang dữ liệu form chỉnh sửa.
        /// </summary>
        public UserEditViewModel GetEdit(int id)
        {
            return UserMappingUtil.ToEditViewModel(GetRequiredUser(id));
        }

        /// <summary>
        /// Cập nhật hồ sơ và ngăn người dùng tự vô hiệu hóa tài khoản của mình.
        /// </summary>
        public void Update(UserEditViewModel model, string currentUsername)
        {
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            model.FullName = StringUtil.CleanText(model.FullName);
            model.Email = StringUtil.CleanText(model.Email);
            UserDto existing = GetRequiredUser(model.UserId);

            bool isCurrentUser = string.Equals(existing.Username, currentUsername, StringComparison.OrdinalIgnoreCase);
            if (isCurrentUser && !model.IsActive)
            {
                throw new BadRequestException(ErrorCode.CannotDeactivateOwnAccount, ErrorMessages.CannotDeactivateOwnAccount);
            }

            bool updated = _userRepository.Update(model.UserId, model.FullName, model.Email, model.IsActive);
            if (!updated)
            {
                throw new ApiException(HttpStatusCodes.NotFound, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }
        }

    }
}
