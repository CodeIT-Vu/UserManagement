using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Repositories;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;

namespace UserManagement.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public void Search_ValidPage()
        {
            UserSearchViewModel model = new UserSearchViewModel
            {
                Page = 2,
                Username = "  user01  ",
                FullName = "  Nguyen Van A  "
            };
            UserListViewModel page = new UserListViewModel { TotalCount = 20, PageSize = 10 };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.Search(model, 10)).Returns(page);
            UserService service = new UserService(repository.Object);

            UserListViewModel result = service.Search(model);

            Assert.AreEqual(2, result.Page);
            Assert.AreEqual("user01", result.Search.Username);
            Assert.AreEqual("Nguyen Van A", result.Search.FullName);
        }

        [TestMethod]
        public void Search_NullModel()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserService service = new UserService(repository.Object);

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Search(null));

            Assert.AreEqual(400, error.HttpStatusCode);
        }

        [TestMethod]
        public void Search_PageTooLarge()
        {
            UserSearchViewModel model = new UserSearchViewModel { Page = 3 };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserListViewModel emptyPage = new UserListViewModel { TotalCount = 20, PageSize = 10 };
            UserListViewModel lastPage = new UserListViewModel { TotalCount = 20, PageSize = 10 };
            repository.SetupSequence(repo => repo.Search(model, 10))
                      .Returns(emptyPage)
                      .Returns(lastPage);
            UserService service = new UserService(repository.Object);

            UserListViewModel result = service.Search(model);

            Assert.AreEqual(2, result.Page);
            Assert.AreSame(lastPage, result);
        }

        [TestMethod]
        public void Search_NoUsers()
        {
            UserSearchViewModel model = new UserSearchViewModel { Page = 2 };
            UserListViewModel emptyPage = new UserListViewModel { PageSize = 10 };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.Search(model, 10)).Returns(emptyPage);
            UserService service = new UserService(repository.Object);

            UserListViewModel result = service.Search(model);

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(0, result.TotalCount);
        }

        [TestMethod]
        public void Update_ValidData()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserDetailsViewModel user = new UserDetailsViewModel { UserId = 2, Username = "user01" };
            repository.Setup(repo => repo.GetById(2)).Returns(user);
            UserService service = new UserService(repository.Object);
            UserEditViewModel model = new UserEditViewModel
            {
                UserId = 2,
                FullName = "  New name  ",
                Email = "",
                IsActive = true
            };
            repository.Setup(repo => repo.Update(model)).Returns(true);

            service.Update(model, "USER01");

            Assert.AreEqual("New name", model.FullName);
            Assert.IsNull(model.Email);
        }

        [TestMethod]
        public void Update_DisableSelf()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserDetailsViewModel user = new UserDetailsViewModel { UserId = 2, Username = "user01" };
            repository.Setup(repo => repo.GetById(2)).Returns(user);
            UserService service = new UserService(repository.Object);
            UserEditViewModel model = new UserEditViewModel { UserId = 2, IsActive = false };

            ValidationException error = Assert.ThrowsException<ValidationException>(
                () => service.Update(model, "USER01"));

            Assert.IsTrue(error.Errors.ContainsKey("IsActive"));
        }

        [TestMethod]
        public void GetDetails_InvalidId()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(() => service.GetDetails(0));

            Assert.AreEqual(404, error.HttpStatusCode);
        }

        [TestMethod]
        public void GetDetails_UserNotFound()
        {
            int userId = 99;
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(userId))
                      .Returns((UserDetailsViewModel)null);

            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(() => service.GetDetails(userId));

            Assert.AreEqual(404, error.HttpStatusCode);
        }

        [TestMethod]
        public void GetEdit_ValidId()
        {
            UserDetailsViewModel user = new UserDetailsViewModel
            {
                UserId = 2,
                FullName = "Nguyen Van A",
                Email = "user01@example.com",
                IsActive = true
            };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(user.UserId)).Returns(user);
            UserService service = new UserService(repository.Object);

            UserEditViewModel result = service.GetEdit(user.UserId);

            Assert.AreEqual(user.UserId, result.UserId);
            Assert.AreEqual(user.FullName, result.FullName);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.IsActive, result.IsActive);
        }

        [TestMethod]
        public void Update_NullModel()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserService service = new UserService(repository.Object);

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Update(null, "admin"));

            Assert.AreEqual(400, error.HttpStatusCode);
        }

        [TestMethod]
        public void Update_UserNotFound()
        {
            UserEditViewModel model = new UserEditViewModel { UserId = 99 };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(model.UserId)).Returns((UserDetailsViewModel)null);
            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(
                () => service.Update(model, "admin"));

            Assert.AreEqual(404, error.HttpStatusCode);
        }

        [TestMethod]
        public void Update_DisableOtherUser()
        {
            UserDetailsViewModel user = new UserDetailsViewModel { UserId = 2, Username = "user01" };
            UserEditViewModel model = new UserEditViewModel { UserId = user.UserId, IsActive = false };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(user.UserId)).Returns(user);
            repository.Setup(repo => repo.Update(model)).Returns(true);
            UserService service = new UserService(repository.Object);

            service.Update(model, "admin");
        }

        [TestMethod]
        public void Update_SaveFailed()
        {
            UserDetailsViewModel user = new UserDetailsViewModel { UserId = 2, Username = "user01" };
            UserEditViewModel model = new UserEditViewModel { UserId = user.UserId, IsActive = true };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(user.UserId)).Returns(user);
            repository.Setup(repo => repo.Update(model)).Returns(false);
            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(
                () => service.Update(model, "admin"));

            Assert.AreEqual(404, error.HttpStatusCode);
        }
    }
}
