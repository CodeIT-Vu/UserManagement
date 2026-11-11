using System;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Repositories;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserListViewModel Search(UserSearchViewModel model)
        {
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            model.Username = CleanText(model.Username);
            model.FullName = CleanText(model.FullName);

            int pageSize = 10;
            UserListViewModel result = _userRepository.Search(model, pageSize);
            int totalPages = result.TotalPages;

            if (totalPages > 0 && model.Page > totalPages)
            {
                model.Page = totalPages;
                result = _userRepository.Search(model, pageSize);
            }

            if (result.TotalCount == 0)
            {
                model.Page = 1;
            }

            result.Page = model.Page;
            result.PageSize = pageSize;
            result.Search = model;
            return result;
        }

        public UserDetailsViewModel GetDetails(int id)
        {
            if (id <= 0)
            {
                throw new ApiException(404, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }

            UserDetailsViewModel user = _userRepository.GetById(id);
            if (user == null)
            {
                throw new ApiException(404, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }

            return user;
        }

        public UserEditViewModel GetEdit(int id)
        {
            UserDetailsViewModel user = GetDetails(id);
            UserEditViewModel model = new UserEditViewModel();
            model.UserId = user.UserId;
            model.FullName = user.FullName;
            model.Email = user.Email;
            model.IsActive = user.IsActive;
            return model;
        }

        public void Update(UserEditViewModel model, string currentUsername)
        {
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            model.FullName = CleanText(model.FullName);
            model.Email = CleanText(model.Email);
            UserDetailsViewModel existing = GetDetails(model.UserId);

            bool isCurrentUser = string.Equals(existing.Username, currentUsername, StringComparison.OrdinalIgnoreCase);
            if (isCurrentUser && !model.IsActive)
            {
                throw new ValidationException("IsActive", "Bạn không thể tự vô hiệu hoá tài khoản đang đăng nhập.");
            }

            bool updated = _userRepository.Update(model);
            if (!updated)
            {
                throw new ApiException(404, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }
        }

        private static string CleanText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }
    }
}
