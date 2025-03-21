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
    public class LoginControllerTest
    {

        private readonly Mock<IUserService> _mockUserService;
        private readonly LoginController _controller;

        public LoginControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new LoginController(_mockUserService.Object);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserServiceReturnsNull()
        {
            var loginModel = new LoginModel { UserName = "testuser", Password = "password" };
            _mockUserService
                .Setup(s => s.Login(It.IsAny<LoginModel>()))
                .ReturnsAsync((TokenMessageModel)null);
            var result = await _controller.Login(loginModel);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var tokenMessage = Assert.IsType<TokenMessageModel>(unauthorizedResult.Value);
            Assert.Equal("Login Failed", tokenMessage.Token);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenUserServiceReturnsValidResponse()
        {
            var loginModel = new LoginModel { UserName = "testuser", Password = "password" };
            var loginResponse = new TokenMessageModel
            {
                Token = "ValidToken",
                UserId = "1"
            };
            _mockUserService
                .Setup(s => s.Login(It.IsAny<LoginModel>()))
                .ReturnsAsync(loginResponse);
            var result = await _controller.Login(loginModel);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tokenMessage = Assert.IsType<TokenMessageModel>(okResult.Value);
            Assert.Equal("ValidToken", tokenMessage.Token);
            Assert.Equal("1", tokenMessage.UserId);
        }
    }
}
