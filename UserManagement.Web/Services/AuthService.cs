using UserManagement.Web.Exceptions;
using UserManagement.Web.Constants;
using UserManagement.Web.Models;
using UserManagement.Web.Repositories;
using UserManagement.Web.Security;
using UserManagement.Web.Util;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public AppUser Authenticate(LoginViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }
            string username = StringUtil.CleanText(model.Username);
            AppUser user = _userRepository.GetByUsername(username);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException(ErrorCode.InvalidLogin, ErrorMessages.InvalidLogin);
            }

            bool passwordMatches = PasswordHasher.VerifyPassword(
                model.Password,
                user.PasswordHash,
                user.PasswordSalt,
                user.PasswordIterations);

            if (!passwordMatches)
            {
                throw new UnauthorizedException(ErrorCode.InvalidLogin, ErrorMessages.InvalidLogin);
            }

            return user;
        }

        public AppUser FindActiveUser(string username)
        {
            AppUser user = _userRepository.GetByUsername(username);
            return user == null || !user.IsActive ? null : user;
        }

        public void Register(RegisterViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrEmpty(model.Password))
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            model.Username = StringUtil.CleanText(model.Username);
            model.FullName = StringUtil.CleanText(model.FullName);
            model.Email = StringUtil.CleanText(model.Email);

            AppUser existingUser = _userRepository.GetByUsername(model.Username);
            if (existingUser != null)
            {
                throw new BadRequestException(ErrorCode.UsernameAlreadyExists, ErrorMessages.UsernameAlreadyExists);
            }

            PasswordHashResult passwordHash = PasswordHasher.HashPassword(model.Password);

            AppUser user = new AppUser
            {
                Username = model.Username,
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = passwordHash.Hash,
                PasswordSalt = passwordHash.Salt,
                PasswordIterations = PasswordHasher.Iterations
            };

            _userRepository.Create(user);
        }

        public void ChangePassword(ChangePasswordViewModel model, string currentUsername)
        {
            if (model == null || string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }
            AppUser user = FindActiveUser(currentUsername) ?? throw new UnauthorizedException(ErrorCode.AuthenticationRequired, ErrorMessages.AuthenticationRequired);
            bool passwordMatches = PasswordHasher.VerifyPassword(model.CurrentPassword, user.PasswordHash, user.PasswordSalt, user.PasswordIterations);
            if (!passwordMatches)
            {
                throw new BadRequestException(ErrorCode.CurrentPasswordIncorrect, ErrorMessages.CurrentPasswordIncorrect);
            }

            PasswordHashResult passwordHash = PasswordHasher.HashPassword(model.NewPassword);

            bool updated = _userRepository.ChangePassword(
                user.UserId, passwordHash.Hash, passwordHash.Salt, PasswordHasher.Iterations);
            if (!updated)
            {
                throw new BadRequestException(ErrorCode.AccountUnavailable, ErrorMessages.AccountUnavailable);
            }
        }
    }
}
