using UserManagement.Web.Exceptions;
using UserManagement.Web.Constants;
using UserManagement.Web.Models;
using UserManagement.Web.Repositories;
using UserManagement.Web.Security;
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
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }
            string username = model.Username.Trim();
            AppUser user = _userRepository.GetByUsername(username);

            if (user == null)
            {
                throw new UnauthorizedException(ErrorCode.InvalidLogin, ErrorMessages.InvalidLogin);
            }

            if (!user.IsActive)
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
            if (user == null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            return user;
        }

        public void Register(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }

            model.Username = model.Username.Trim();
            model.FullName = model.FullName.Trim();
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                model.Email = model.Email.Trim();
            }
            else
            {
                model.Email = null;
            }

            AppUser existingUser = _userRepository.GetByUsername(model.Username);
            if (existingUser != null)
            {
                throw new ValidationException("Username", "Tên đăng nhập đã được sử dụng.");
            }

            PasswordHashResult passwordHash = PasswordHasher.HashPassword(model.Password);

            AppUser user = new AppUser();
            user.Username = model.Username;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PasswordHash = passwordHash.Hash;
            user.PasswordSalt = passwordHash.Salt;
            user.PasswordIterations = PasswordHasher.Iterations;

            _userRepository.Create(user);
        }

        public void ChangePassword(ChangePasswordViewModel model, string currentUsername)
        {
            if (model == null)
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }
            AppUser user = FindActiveUser(currentUsername);
            if (user == null)
            {
                throw new UnauthorizedException(ErrorCode.AuthenticationRequired, ErrorMessages.AuthenticationRequired);
            }

            bool passwordMatches = PasswordHasher.VerifyPassword(model.CurrentPassword, user.PasswordHash, user.PasswordSalt, user.PasswordIterations);
            if (!passwordMatches)
            {
                throw new ValidationException("CurrentPassword", "Mật khẩu hiện tại không đúng.");
            }

            PasswordHashResult passwordHash = PasswordHasher.HashPassword(model.NewPassword);

            bool updated = _userRepository.ChangePassword(
                user.UserId, passwordHash.Hash, passwordHash.Salt, PasswordHasher.Iterations);
            if (!updated)
            {
                throw new ValidationException("CurrentPassword", "Tài khoản không còn tồn tại. Vui lòng đăng nhập lại.");
            }
        }
    }
}
