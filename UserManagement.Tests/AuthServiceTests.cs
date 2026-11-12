using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UserManagement.Web.Constants;
using UserManagement.Web.DTOs;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Models;
using UserManagement.Web.Repositories;
using UserManagement.Web.Security;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;

namespace UserManagement.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
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

        [TestMethod]
        public void FindActiveUser_UserNotFound_ReturnsNull()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("unknown")).Returns((AppUser)null);
            AuthService service = new AuthService(repository.Object);

            CurrentUserDto result = service.FindActiveUser("unknown");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Register_ValidData_CreatesUser()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("newuser")).Returns((AppUser)null);
            AuthService service = new AuthService(repository.Object);
            RegisterViewModel model = new RegisterViewModel
            {
                Username = "  newuser  ",
                FullName = "  Nguyen Van A  ",
                Email = "  user@example.com  ",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            service.Register(model);

            Assert.AreEqual("newuser", model.Username);
            Assert.AreEqual("Nguyen Van A", model.FullName);
            Assert.AreEqual("user@example.com", model.Email);
            repository.Verify(repo => repo.Create(It.IsAny<AppUser>()), Times.Once());
        }

        [TestMethod]
        public void Register_DuplicateUsername_ThrowsBadRequest()
        {
            AppUser user = new AppUser { Username = "user01" };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            RegisterViewModel model = new RegisterViewModel
            {
                Username = "user01",
                FullName = "Nguyen Van A",
                Password = "Password123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(model));

            Assert.AreEqual(ErrorCode.UsernameAlreadyExists, error.ErrorCode);
            repository.Verify(repo => repo.Create(It.IsAny<AppUser>()), Times.Never());
        }

        [TestMethod]
        public void Register_UsernameTakenDuringCreate_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("newuser")).Returns((AppUser)null);
            repository.Setup(repo => repo.Create(It.IsAny<AppUser>()))
                .Throws(new DuplicateUsernameException());
            AuthService service = new AuthService(repository.Object);
            RegisterViewModel model = new RegisterViewModel
            {
                Username = "newuser",
                FullName = "Nguyen Van A",
                Password = "Password123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(model));

            Assert.AreEqual(400, error.HttpStatusCode);
            Assert.AreEqual(ErrorCode.UsernameAlreadyExists, error.ErrorCode);
            Assert.AreEqual(ErrorMessages.UsernameAlreadyExists, error.UserMessage);
            repository.Verify(repo => repo.Create(It.IsAny<AppUser>()), Times.Once());
        }

        [TestMethod]
        public void Register_EmptyUsername_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            AuthService service = new AuthService(repository.Object);
            RegisterViewModel model = new RegisterViewModel
            {
                Username = "",
                FullName = "Nguyen Van A",
                Password = "Password123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(model));

            Assert.AreEqual(400, error.HttpStatusCode);
            repository.Verify(repo => repo.Create(It.IsAny<AppUser>()), Times.Never());
        }

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

        [TestMethod]
        public void Logout_ExistingUser_IncreasesAuthVersion()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.IncreaseAuthVersion(1)).Returns(true);
            AuthService service = new AuthService(repository.Object);

            service.Logout(1);

            repository.Verify(repo => repo.IncreaseAuthVersion(1), Times.Once());
        }

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
