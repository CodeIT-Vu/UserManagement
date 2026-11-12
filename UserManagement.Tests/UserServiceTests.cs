using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UserManagement.Web.Constants;
using UserManagement.Web.Exceptions;
using UserManagement.Web.Models;
using UserManagement.Web.Repositories;
using UserManagement.Web.Services;
using UserManagement.Web.ViewModels;

namespace UserManagement.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public void Search_PageTwo_ReturnsPageTwoAndCleanFilters()
        {
            UserSearchViewModel model = new UserSearchViewModel
            {
                Page = 2,
                Username = "  user01  ",
                FullName = "  Nguyen Van A  "
            };
            UserSearchResultData page = new UserSearchResultData { TotalCount = 20 };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.Search("user01", "Nguyen Van A", null, 2, 10)).Returns(page);
            UserService service = new UserService(repository.Object);

            UserListViewModel result = service.Search(model);

            Assert.AreEqual(2, result.Page);
            Assert.AreEqual("user01", result.Search.Username);
            Assert.AreEqual("Nguyen Van A", result.Search.FullName);
        }

        [TestMethod]
        public void Search_NullModel_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserService service = new UserService(repository.Object);

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Search(null));

            Assert.AreEqual(400, error.HttpStatusCode);
        }

        [TestMethod]
        public void Search_PageTooLarge_ReturnsLastPage()
        {
            UserSearchViewModel model = new UserSearchViewModel { Page = 3 };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserSearchResultData emptyPage = new UserSearchResultData { TotalCount = 20 };
            UserSearchResultData lastPage = new UserSearchResultData { TotalCount = 20 };
            repository.Setup(repo => repo.Search(null, null, null, 3, 10)).Returns(emptyPage);
            repository.Setup(repo => repo.Search(null, null, null, 2, 10)).Returns(lastPage);
            UserService service = new UserService(repository.Object);

            UserListViewModel result = service.Search(model);

            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(lastPage.TotalCount, result.TotalCount);
        }

        [TestMethod]
        public void Search_NoUsers_ReturnsFirstPage()
        {
            UserSearchViewModel model = new UserSearchViewModel { Page = 2 };
            UserSearchResultData emptyPage = new UserSearchResultData();
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.Search(null, null, null, 2, 10)).Returns(emptyPage);
            UserService service = new UserService(repository.Object);

            UserListViewModel result = service.Search(model);

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(0, result.TotalCount);
        }

        [TestMethod]
        public void Update_ValidData_NormalizesInput()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserData user = new UserData { UserId = 2, Username = "user01" };
            repository.Setup(repo => repo.GetById(2)).Returns(user);
            UserService service = new UserService(repository.Object);
            UserEditViewModel model = new UserEditViewModel
            {
                UserId = 2,
                FullName = "  New name  ",
                Email = "",
                IsActive = true
            };
            repository.Setup(repo => repo.Update(2, "New name", null, true)).Returns(true);

            service.Update(model, "USER01");

            Assert.AreEqual("New name", model.FullName);
            Assert.IsNull(model.Email);
        }

        [TestMethod]
        public void Update_DisableSelf_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserData user = new UserData { UserId = 2, Username = "user01" };
            repository.Setup(repo => repo.GetById(2)).Returns(user);
            UserService service = new UserService(repository.Object);
            UserEditViewModel model = new UserEditViewModel { UserId = 2, IsActive = false };

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Update(model, "USER01"));

            Assert.AreEqual(400, error.HttpStatusCode);
            Assert.AreEqual(ErrorMessages.CannotDeactivateOwnAccount, error.UserMessage);
            Assert.AreEqual(ErrorCode.CannotDeactivateOwnAccount, error.ErrorCode);
        }

        [TestMethod]
        public void GetDetails_InvalidId_ThrowsNotFound()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(() => service.GetDetails(0));

            Assert.AreEqual(404, error.HttpStatusCode);
        }

        [TestMethod]
        public void GetDetails_UserNotFound_ThrowsNotFound()
        {
            int userId = 99;
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(userId))
                      .Returns((UserData)null);

            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(() => service.GetDetails(userId));

            Assert.AreEqual(404, error.HttpStatusCode);
        }

        [TestMethod]
        public void GetEdit_ExistingUser_ReturnsEditModel()
        {
            UserData user = new UserData
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
        public void Update_NullModel_ThrowsBadRequest()
        {
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            UserService service = new UserService(repository.Object);

            BadRequestException error = Assert.ThrowsException<BadRequestException>(
                () => service.Update(null, "admin"));

            Assert.AreEqual(400, error.HttpStatusCode);
        }

        [TestMethod]
        public void Update_UserNotFound_ThrowsNotFound()
        {
            UserEditViewModel model = new UserEditViewModel { UserId = 99 };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(model.UserId)).Returns((UserData)null);
            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(
                () => service.Update(model, "admin"));

            Assert.AreEqual(404, error.HttpStatusCode);
        }

        [TestMethod]
        public void Update_DisableOtherUser_Succeeds()
        {
            UserData user = new UserData { UserId = 2, Username = "user01" };
            UserEditViewModel model = new UserEditViewModel { UserId = user.UserId, IsActive = false };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(user.UserId)).Returns(user);
            repository.Setup(repo => repo.Update(model.UserId, model.FullName, model.Email, model.IsActive)).Returns(true);
            UserService service = new UserService(repository.Object);

            service.Update(model, "admin");

            repository.Verify(repo => repo.Update(2, null, null, false), Times.Once());
        }

        [TestMethod]
        public void Update_SaveFailed_ThrowsNotFound()
        {
            UserData user = new UserData { UserId = 2, Username = "user01" };
            UserEditViewModel model = new UserEditViewModel { UserId = user.UserId, IsActive = true };
            Mock<IUserRepository> repository = new Mock<IUserRepository>();
            repository.Setup(repo => repo.GetById(user.UserId)).Returns(user);
            repository.Setup(repo => repo.Update(model.UserId, model.FullName, model.Email, model.IsActive)).Returns(false);
            UserService service = new UserService(repository.Object);

            ApiException error = Assert.ThrowsException<ApiException>(
                () => service.Update(model, "admin"));

            Assert.AreEqual(404, error.HttpStatusCode);
        }
    }
}
