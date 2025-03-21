using CibusServer.Controllers;
using CibusServer.Interfaces;
using CibusServer.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CibusServerUnitTest.Controllers
{
    public class RegisterControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly RegisterController _controller;

        public RegisterControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new RegisterController(_mockUserService.Object);
        }

        [Fact]
        public async Task Register_ReturnsUserModel_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "testuser", Password = "password" };
            var expectedUserModel = new UserModel { Id = 1, UserName = "testuser" };

            // Setup the IUserService.Register method to return the expected user model.
            _mockUserService
                .Setup(s => s.Register(It.IsAny<LoginModel>()))
                .ReturnsAsync(expectedUserModel);

            // Act
            var result = await _controller.Register(loginModel);

            // Assert
            // The implicit conversion wraps the returned UserModel in an ActionResult<UserModel>
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedUserModel.Id, result.Value.Id);
            Assert.Equal(expectedUserModel.UserName, result.Value.UserName);
        }
    }
}
