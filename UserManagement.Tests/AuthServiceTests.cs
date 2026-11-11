using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;
using UserManagement.Web.Models;
using UserManagement.Web.Repositories;
using UserManagement.Web.Security;

namespace UserManagement.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        [TestMethod]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            AppUser user = CreateActiveUser("Password123!");
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);
            LoginViewModel model = new LoginViewModel
            {
                Username = "  user01  ",
                Password = "Password123!"
            };

            AppUser result = service.Authenticate(model);

            Assert.AreSame(user, result);
        }

        [DataTestMethod]
        [DataRow("  user01@example.com  ", "user01@example.com")]
        [DataRow("   ", null)]
        public void Register_WhitespaceInput_NormalizesInput(string email, string expectedEmail)
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            AuthService service = new AuthService(repository.Object);
            RegisterViewModel model = new RegisterViewModel
            {
                Username = "  user01  ",
                FullName = "  Nguyen Van A  ",
                Email = email,
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            service.Register(model);

            Assert.AreEqual("user01", model.Username);
            Assert.AreEqual("Nguyen Van A", model.FullName);
            Assert.AreEqual(expectedEmail, model.Email);
        }

        [DataTestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, false)]
        public void FindActiveUser_AccountStatus_ReturnsOnlyActiveUser(bool userExists, bool isActive)
        {
            AppUser user = userExists ? new AppUser { Username = "user01", IsActive = isActive } : null;
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns(user);
            AuthService service = new AuthService(repository.Object);

            AppUser result = service.FindActiveUser("user01");

            AppUser expectedUser = isActive ? user : null;
            Assert.AreSame(expectedUser, result);
        }

        [TestMethod]
        public void Authenticate_NullModel_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            AuthService service = new AuthService(repository.Object);

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Authenticate(null));

            Assert.AreEqual(400, error.HttpStatusCode);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Authenticate_UnavailableUser_ThrowsUnauthorized(bool userExists)
        {
            AppUser user = userExists ? new AppUser { Username = "user01", IsActive = false } : null;
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
        }

        [TestMethod]
        public void Authenticate_IncorrectPassword_ThrowsUnauthorized()
        {
            AppUser user = CreateActiveUser("Password123!");
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername(user.Username)).Returns(user);
            AuthService service = new AuthService(repository.Object);
            LoginViewModel model = new LoginViewModel
            {
                Username = user.Username,
                Password = "WrongPassword!"
            };

            UnauthorizedException error = Assert.ThrowsException<UnauthorizedException>(
                () => service.Authenticate(model));

            Assert.AreEqual(401, error.HttpStatusCode);
        }

        [TestMethod]
        public void Register_NullModel_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            AuthService service = new AuthService(repository.Object);

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(null));

            Assert.AreEqual(400, error.HttpStatusCode);
        }

        [TestMethod]
        public void Register_DuplicateUsername_ThrowsBadRequest()
        {
            AppUser existingUser = new AppUser { Username = "user01" };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername(existingUser.Username)).Returns(existingUser);
            AuthService service = new AuthService(repository.Object);
            RegisterViewModel model = new RegisterViewModel
            {
                Username = existingUser.Username,
                FullName = "Nguyen Van A",
                Email = "user01@example.com",
                Password = "Password123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Register(model));

            Assert.AreEqual(ErrorCode.UsernameAlreadyExists, error.ErrorCode);
            Assert.AreEqual(ErrorMessages.UsernameAlreadyExists, error.UserMessage);
        }

        [TestMethod]
        public void ChangePassword_NullModel_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            AuthService service = new AuthService(repository.Object);

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.ChangePassword(null, "user01"));

            Assert.AreEqual(400, error.HttpStatusCode);
        }

        [TestMethod]
        public void ChangePassword_UserNotFound_ThrowsUnauthorized()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername("user01")).Returns((AppUser)null);
            AuthService service = new AuthService(repository.Object);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!"
            };

            UnauthorizedException error = Assert.ThrowsException<UnauthorizedException>(
                () => service.ChangePassword(model, "user01"));

            Assert.AreEqual(401, error.HttpStatusCode);
        }

        [TestMethod]
        public void ChangePassword_WrongPassword_ThrowsBadRequest()
        {
            AppUser user = CreateActiveUser("Password123!");
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername(user.Username)).Returns(user);
            AuthService service = new AuthService(repository.Object);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                CurrentPassword = "WrongPassword!",
                NewPassword = "NewPassword123!"
            };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.ChangePassword(model, user.Username));

            Assert.AreEqual(ErrorCode.CurrentPasswordIncorrect, error.ErrorCode);
            Assert.AreEqual(ErrorMessages.CurrentPasswordIncorrect, error.UserMessage);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ChangePassword_SaveResult_HandlesSuccessOrFailure(bool saved)
        {
            AppUser user = CreateActiveUser("Password123!");
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetByUsername(user.Username)).Returns(user);
            repository.Setup(repo => repo.ChangePassword(
                user.UserId, It.IsAny<byte[]>(), It.IsAny<byte[]>(), PasswordHasher.Iterations))
                .Returns(saved);
            AuthService service = new AuthService(repository.Object);
            ChangePasswordViewModel model = new ChangePasswordViewModel
            {
                CurrentPassword = "Password123!",
                NewPassword = "NewPassword123!"
            };

            if (saved)
            {
                service.ChangePassword(model, user.Username);
                return;
            }

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.ChangePassword(model, user.Username));

            Assert.AreEqual(ErrorCode.AccountUnavailable, error.ErrorCode);
            Assert.AreEqual(ErrorMessages.AccountUnavailable, error.UserMessage);
        }

        private static AppUser CreateActiveUser(string password)
        {
            PasswordHashResult passwordHash = PasswordHasher.HashPassword(password);
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
