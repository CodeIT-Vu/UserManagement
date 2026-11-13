using UserManagement.Web.Exceptions;
using UserManagement.Web.Constants;
using UserManagement.Web.DTOs;
using UserManagement.Web.Models;
using UserManagement.Web.Repositories;
using UserManagement.Web.Security;
using UserManagement.Web.Util;
using UserManagement.Web.ViewModels;

namespace UserManagement.Web.Services
{
    /// <summary>
    /// Xử lý nghiệp vụ đăng nhập, đăng ký và đổi mật khẩu.
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Bộ hash giả dùng lại khi tài khoản không tồn tại hoặc bị khóa để vẫn thực hiện PBKDF2.
        /// </summary>
        private static readonly PasswordHashResult DummyPasswordHash = PasswordHasher.HashPassword("DummyLoginPassword");
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Nhận repository người dùng qua constructor.
        /// </summary>
        public AuthService(IUserRepository repository)
        {
            _userRepository = repository;
        }

        /// <summary>
        /// Kiểm tra mật khẩu thật hoặc giả trước khi kết luận đăng nhập; trả về thông tin người dùng an toàn.
        /// </summary>
        public CurrentUserDto Authenticate(LoginViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }
            string username = StringUtil.CleanText(model.Username);
            AppUser user = _userRepository.GetByUsername(username);

            // Tài khoản thiếu hoặc bị khóa vẫn kiểm tra hash giả để hạn chế chênh lệch thời gian.
            bool canAuthenticate = user != null && user.IsActive;
            byte[] hash = canAuthenticate ? user.PasswordHash : DummyPasswordHash.Hash;
            byte[] salt = canAuthenticate ? user.PasswordSalt : DummyPasswordHash.Salt;
            int iterations = canAuthenticate ? user.PasswordIterations : PasswordHasher.Iterations;

            bool passwordMatches = PasswordHasher.VerifyPassword(
                model.Password,
                hash,
                salt,
                iterations);

            if (!canAuthenticate || !passwordMatches)
            {
                throw new UnauthorizedException(ErrorCode.InvalidLogin, ErrorMessages.InvalidLogin);
            }

            return UserMappingUtil.ToCurrentUserDto(user);
        }

        /// <summary>
        /// Lấy thông tin xác thực an toàn nếu tài khoản còn hoạt động.
        /// </summary>
        public CurrentUserDto FindActiveUser(string username)
        {
            AppUser user = _userRepository.GetByUsername(username);
            return user == null || !user.IsActive ? null : UserMappingUtil.ToCurrentUserDto(user);
        }

        /// <summary>
        /// Tăng AuthVersion để thu hồi các ticket hiện tại của cùng tài khoản.
        /// </summary>
        public void Logout(int userId)
        {
            bool updated = _userRepository.IncreaseAuthVersion(userId);
            if (!updated)
            {
                throw new UnauthorizedException(ErrorCode.AuthenticationRequired, ErrorMessages.AuthenticationRequired);
            }
        }

        /// <summary>
        /// Kiểm tra tên đăng nhập và băm mật khẩu trước khi tạo tài khoản.
        /// </summary>
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

            try
            {
                _userRepository.Create(user);
            }
            catch (DuplicateUsernameException)
            {
                // Database vẫn chặn trùng nếu tên đăng nhập bị lấy sau bước kiểm tra ban đầu.
                throw new BadRequestException(ErrorCode.UsernameAlreadyExists, ErrorMessages.UsernameAlreadyExists);
            }
        }

        /// <summary>
        /// Xác minh mật khẩu hiện tại trước khi lưu mật khẩu mới.
        /// </summary>
        public void ChangePassword(ChangePasswordViewModel model, string currentUsername)
        {
            if (model == null || string.IsNullOrEmpty(model.CurrentPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                throw new BadRequestException(ErrorCode.InvalidRequest, ErrorMessages.InvalidRequest);
            }
            AppUser user = _userRepository.GetByUsername(currentUsername);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException(ErrorCode.AuthenticationRequired, ErrorMessages.AuthenticationRequired);
            }
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
