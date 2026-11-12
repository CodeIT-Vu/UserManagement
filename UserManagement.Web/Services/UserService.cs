using System;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Models;
using UserManagement.Web.Repositories;
using UserManagement.Web.Util;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    /// <summary>
    /// Xử lý nghiệp vụ quản lý người dùng và chuyển dữ liệu sang ViewModel.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private const int DefaultPageSize = 10;

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

            UserSearchResultData data = _userRepository.Search(model.Username, model.FullName, model.IsActive, model.Page, DefaultPageSize);
            int totalPages = PaginationUtil.CalculateTotalPages(data.TotalCount, DefaultPageSize);

            if (totalPages > 0 && model.Page > totalPages)
            {
                model.Page = totalPages;
                data = _userRepository.Search(model.Username, model.FullName, model.IsActive, model.Page, DefaultPageSize);
                totalPages = PaginationUtil.CalculateTotalPages(data.TotalCount, DefaultPageSize);
            }

            if (data.TotalCount == 0)
            {
                model.Page = 1;
            }

            return UserMappingUtil.ToListViewModel(data, model, model.Page, DefaultPageSize, totalPages);
        }

        public UserDetailsViewModel GetDetails(int id)
        {
            return UserMappingUtil.ToDetailsViewModel(GetRequiredUser(id));
        }

        private UserData GetRequiredUser(int id)
        {
            if (id <= 0)
            {
                throw new ApiException(404, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }

            UserData user = _userRepository.GetById(id);
            if (user == null)
            {
                throw new ApiException(404, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }

            return user;
        }

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
            UserData existing = GetRequiredUser(model.UserId);

            bool isCurrentUser = string.Equals(existing.Username, currentUsername, StringComparison.OrdinalIgnoreCase);
            if (isCurrentUser && !model.IsActive)
            {
                throw new BadRequestException(ErrorCode.CannotDeactivateOwnAccount, ErrorMessages.CannotDeactivateOwnAccount);
            }

            bool updated = _userRepository.Update(model.UserId, model.FullName, model.Email, model.IsActive);
            if (!updated)
            {
                throw new ApiException(404, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }
        }

    }
}
