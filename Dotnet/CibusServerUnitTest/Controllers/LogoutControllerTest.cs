using CibusServer.Controllers;
using CibusServer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CibusServerUnitTest.Controllers
{
    public class LogoutControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly LogoutController _controller;

        public LogoutControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new LogoutController(_mockUserService.Object);
        }

        [Fact]
        public async Task Logout_ReturnsBadRequest_WhenAuthorizationHeaderIsMissing()
        {  
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            var result = await _controller.Logout();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Authorization header not found", badRequestResult.Value);
        }

        [Fact]
        public async Task Logout_ReturnsBadRequest_WhenUserServiceLogoutReturnsFalse()
        {
            var token = "invalid-token";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _mockUserService.Setup(s => s.Logout(token)).ReturnsAsync(false);
            var result = await _controller.Logout();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Exception when removing token", badRequestResult.Value);
        }

        [Fact]
        public async Task Logout_ReturnsOk_WhenUserServiceLogoutReturnsTrue()
        {  
            var token = "valid-token";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext }; 
            _mockUserService.Setup(s => s.Logout(token)).ReturnsAsync(true);
            var result = await _controller.Logout();
            Assert.IsType<OkResult>(result);
        }
    }
}
