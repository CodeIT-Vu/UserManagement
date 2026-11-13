using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Repositories;
using UserManagement.Web.Security;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;
using UserManagement.Web.Models.Entities;
using UserManagement.Web.Models.DTOs;

namespace UserManagement.Tests
{
    /// <summary>
    /// Kiểm tra nghiệp vụ xác thực, đăng ký, đăng xuất và đổi mật khẩu.
    /// </summary>
    [TestClass]
    public class AuthServiceTests
    {
        /// <summary>
        /// Đăng nhập đúng mật khẩu trả về thông tin người dùng.
        /// </summary>
        [TestMethod]
        public void Authenticate_CorrectPassword_ReturnsUser()
        {
            AppUser user = CreateActiveUser();
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            LoginViewModel model = new LoginViewModel
            {
                Username = "user01",
                Password = "Password123!"
            };

            CurrentUserDto result = service.Authenticate(model);

            Assert.AreEqual(user.UserId, result.UserId);
            Assert.AreEqual(user.Username, result.Username);
        }

        /// <summary>
        /// Tài khoản dùng hash 600000 vòng vẫn đăng nhập được sau khi đổi cấu hình.
        /// </summary>
        [TestMethod]
        public void Authenticate_LegacyIterations_ReturnsUser()
        {
            const int legacyIterations = 600000;
            byte[] salt = new byte[16];
            byte[] hash;
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(
                "Password123!", salt, legacyIterations, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(32);
            }

            AppUser user = new AppUser
            {
                UserId = 1,
                Username = "user01",
                IsActive = true,
                PasswordHash = hash,
                PasswordSalt = salt,
                PasswordIterations = legacyIterations
            };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            LoginViewModel model = new LoginViewModel
            {
                Username = "user01",
                Password = "Password123!"
            };

            CurrentUserDto result = service.Authenticate(model);

            Assert.AreEqual(user.UserId, result.UserId);
            Assert.AreEqual(user.Username, result.Username);
        }

        /// <summary>
        /// Đăng nhập sai mật khẩu báo lỗi xác thực.
        /// </summary>
        [TestMethod]
        public void Authenticate_WrongPassword_ThrowsUnauthorized()
        {
            AppUser user = CreateActiveUser();
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            LoginViewModel model = new LoginViewModel
            {
                Username = "user01",
                Password = "WrongPassword!"
            };

            UnauthorizedException error = Assert.ThrowsException<UnauthorizedException>(
                () => service.Authenticate(model));

            Assert.AreEqual(401, error.HttpStatusCode);
            Assert.AreEqual(ErrorCode.InvalidLogin, error.ErrorCode);
        }

        /// <summary>
        /// Đăng nhập với tài khoản không tồn tại báo lỗi xác thực.
        /// </summary>
        [TestMethod]
        public void Authenticate_UserNotFound_ThrowsUnauthorized()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("unknown")).Returns((AppUser)null);
            AuthService service = new AuthService(repository.Object);
            LoginViewModel model = new LoginViewModel
            {
                Username = "unknown",
                Password = "Password123!"
            };

            UnauthorizedException error = Assert.ThrowsException<UnauthorizedException>(
                () => service.Authenticate(model));

            Assert.AreEqual(401, error.HttpStatusCode);
            Assert.AreEqual(ErrorCode.InvalidLogin, error.ErrorCode);
        }

        /// <summary>
        /// Đăng nhập với tài khoản bị khóa báo lỗi xác thực.
        /// </summary>
        [TestMethod]
        public void Authenticate_InactiveUser_ThrowsUnauthorized()
        {
            AppUser user = new AppUser { Username = "user01", IsActive = false };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            LoginViewModel model = new LoginViewModel
            {
                Username = "user01",
                Password = "Password123!"
            };

            UnauthorizedException error = Assert.ThrowsException<UnauthorizedException>(
                () => service.Authenticate(model));

            Assert.AreEqual(401, error.HttpStatusCode);
            Assert.AreEqual(ErrorCode.InvalidLogin, error.ErrorCode);
        }

        /// <summary>
        /// Tìm tài khoản còn hoạt động trả về thông tin người dùng.
        /// </summary>
        [TestMethod]
        public void FindActiveUser_ActiveUser_ReturnsUser()
        {
            AppUser user = new AppUser { UserId = 1, Username = "user01", IsActive = true };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);

            CurrentUserDto result = service.FindActiveUser("user01");

            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserId, result.UserId);
            Assert.AreEqual(user.Username, result.Username);
        }

        /// <summary>
        /// Tìm tài khoản không tồn tại trả về null.
        /// </summary>
        [TestMethod]
        public void FindActiveUser_UserNotFound_ReturnsNull()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("unknown")).Returns((AppUser)null);
            AuthService service = new AuthService(repository.Object);

            CurrentUserDto result = service.FindActiveUser("unknown");

            Assert.IsNull(result);
        }

        /// <summary>
        /// Đăng ký hợp lệ chuẩn hóa dữ liệu và gọi tạo tài khoản một lần.
        /// </summary>
        [TestMethod]
        public void Register_ValidData_CreatesUser()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("newuser")).Returns((AppUser)null);
            AuthService service = new AuthService(repository.Object);
            RegisterDto request = new RegisterDto
            {
                Username = "  newuser  ",
                FullName = "  Nguyen Van A  ",
                Email = "  user@example.com  ",
                Password = "Password123!"
            };

            service.Register(request);

            Assert.AreEqual("  newuser  ", request.Username);
            Assert.AreEqual("  Nguyen Van A  ", request.FullName);
            Assert.AreEqual("  user@example.com  ", request.Email);
            repository.Verify(repo => repo.Create(It.Is<AppUser>(user =>
                user.Username == "newuser"
                && user.FullName == "Nguyen Van A"
                && user.Email == "user@example.com"
                && user.PasswordIterations == 100000)), Times.Once());
        }

        /// <summary>
        /// Tên đăng nhập đã tồn tại bị từ chối trước khi tạo tài khoản.
        /// </summary>
        [TestMethod]
        public void Register_DuplicateUsername_ThrowsBadRequest()
        {
            AppUser user = new AppUser { Username = "user01" };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            RegisterDto request = new RegisterDto
            {
                Username = "user01",
                FullName = "Nguyen Van A",
                Password = "Password123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(request));

            Assert.AreEqual(ErrorCode.UsernameAlreadyExists, error.ErrorCode);
            repository.Verify(repo => repo.Create(It.IsAny<AppUser>()), Times.Never());
        }

        /// <summary>
        /// Trùng tên ngay lúc lưu được chuyển thành lỗi nghiệp vụ HTTP 400.
        /// </summary>
        [TestMethod]
        public void Register_UsernameTakenDuringCreate_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("newuser")).Returns((AppUser)null);
            // Giả lập tên còn trống khi tra cứu nhưng bị trùng ngay lúc lưu.
            repository.Setup(repo => repo.Create(It.IsAny<AppUser>()))
                .Throws(new DuplicateUsernameException());
            AuthService service = new AuthService(repository.Object);
            RegisterDto request = new RegisterDto
            {
                Username = "newuser",
                FullName = "Nguyen Van A",
                Password = "Password123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(request));

            Assert.AreEqual(400, error.HttpStatusCode);
            Assert.AreEqual(ErrorCode.UsernameAlreadyExists, error.ErrorCode);
            Assert.AreEqual(ErrorMessages.UsernameAlreadyExists, error.UserMessage);
            repository.Verify(repo => repo.Create(It.IsAny<AppUser>()), Times.Once());
        }

        /// <summary>
        /// Đăng ký thiếu tên đăng nhập báo lỗi và không tạo tài khoản.
        /// </summary>
        [TestMethod]
        public void Register_EmptyUsername_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            AuthService service = new AuthService(repository.Object);
            RegisterDto request = new RegisterDto
            {
                Username = "",
                FullName = "Nguyen Van A",
                Password = "Password123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(request));

            Assert.AreEqual(400, error.HttpStatusCode);
            repository.Verify(repo => repo.Create(It.IsAny<AppUser>()), Times.Never());
        }

        /// <summary>
        /// Đúng mật khẩu hiện tại thì gọi cập nhật mật khẩu một lần.
        /// </summary>
        [TestMethod]
        public void ChangePassword_CorrectCurrentPassword_UpdatesPassword()
        {
            AppUser user = CreateActiveUser();
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            repository.Setup(repo => repo.ChangePassword(
                1, It.IsAny<byte[]>(), It.IsAny<byte[]>(), PasswordHasher.Iterations)).Returns(true);
            AuthService service = new AuthService(repository.Object);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!"
            };

            service.ChangePassword(model, "user01");

            repository.Verify(repo => repo.ChangePassword(
                1, It.IsAny<byte[]>(), It.IsAny<byte[]>(), PasswordHasher.Iterations), Times.Once());
        }

        /// <summary>
        /// Sai mật khẩu hiện tại báo lỗi và không cập nhật mật khẩu.
        /// </summary>
        [TestMethod]
        public void ChangePassword_WrongCurrentPassword_ThrowsBadRequest()
        {
            AppUser user = CreateActiveUser();
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                CurrentPassword = "WrongPassword!",
                NewPassword = "NewPassword123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.ChangePassword(model, "user01"));

            Assert.AreEqual(ErrorCode.CurrentPasswordIncorrect, error.ErrorCode);
            repository.Verify(repo => repo.ChangePassword(
                1, It.IsAny<byte[]>(), It.IsAny<byte[]>(), PasswordHasher.Iterations), Times.Never());
        }

        /// <summary>
        /// Đổi mật khẩu cho tài khoản không tồn tại báo lỗi xác thực.
        /// </summary>
        [TestMethod]
        public void ChangePassword_UserNotFound_ThrowsUnauthorized()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("unknown")).Returns((AppUser)null);
            AuthService service = new AuthService(repository.Object);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!"
            };

            UnauthorizedException error = Assert.ThrowsException<UnauthorizedException>(
                () => service.ChangePassword(model, "unknown"));

            Assert.AreEqual(401, error.HttpStatusCode);
        }

        /// <summary>
        /// Đăng xuất gọi tăng phiên bản xác thực của đúng tài khoản.
        /// </summary>
        [TestMethod]
        public void Logout_ExistingUser_IncreasesAuthVersion()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.IncreaseAuthVersion(1)).Returns(true);
            AuthService service = new AuthService(repository.Object);

            service.Logout(1);

            repository.Verify(repo => repo.IncreaseAuthVersion(1), Times.Once());
        }

        /// <summary>
        /// Đăng xuất không cập nhật được tài khoản báo lỗi xác thực.
        /// </summary>
        [TestMethod]
        public void Logout_UserNotFound_ThrowsUnauthorized()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.IncreaseAuthVersion(1)).Returns(false);
            AuthService service = new AuthService(repository.Object);

            UnauthorizedException error = Assert.ThrowsException<UnauthorizedException>(
                () => service.Logout(1));

            Assert.AreEqual(401, error.HttpStatusCode);
            Assert.AreEqual(ErrorCode.AuthenticationRequired, error.ErrorCode);
        }

        /// <summary>
        /// Tạo tài khoản hoạt động với hash mật khẩu thật dùng trong test.
        /// </summary>
        private static AppUser CreateActiveUser()
        {
            PasswordHashResult passwordHash = PasswordHasher.HashPassword("Password123!");
            return new AppUser
            {
                UserId = 1,
                Username = "user01",
                IsActive = true,
                PasswordHash = passwordHash.Hash,
                PasswordSalt = passwordHash.Salt,
                PasswordIterations = PasswordHasher.Iterations
            };
        }
    }
}
