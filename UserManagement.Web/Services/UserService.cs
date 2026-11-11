using System;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Repositories;
using UserManagement.Web.Util;
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

            model.Username = StringUtil.CleanText(model.Username);
            model.FullName = StringUtil.CleanText(model.FullName);

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
            UserEditViewModel model = new UserEditViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return model;
        }

        public void Update(UserEditViewModel model, string currentUsername)
        {
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            model.FullName = StringUtil.CleanText(model.FullName);
            model.Email = StringUtil.CleanText(model.Email);
            UserDetailsViewModel existing = GetDetails(model.UserId);

            bool isCurrentUser = string.Equals(existing.Username, currentUsername, StringComparison.OrdinalIgnoreCase);
            if (isCurrentUser && !model.IsActive)
            {
                throw new BadRequestException(ErrorCode.CannotDeactivateOwnAccount, ErrorMessages.CannotDeactivateOwnAccount);
            }

            bool updated = _userRepository.Update(model);
            if (!updated)
            {
                throw new ApiException(404, ErrorCode.UserNotFound, ErrorMessages.UserNotFound);
            }
        }

    }
}
