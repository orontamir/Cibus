using CibusServer.Controllers;
using CibusServer.Interfaces;
using CibusServer.Models;
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
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _userServiceMock = new Mock<IUserService>();
            _messageServiceMock = new Mock<IMessageService>();
            _controller = new UserController(_userServiceMock.Object, _messageServiceMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task userMessages_ReturnsMessages_WhenUserExists()
        {
            string token = "sampletoken";
            int expectedUserId = 1;
            var expectedMessages = new List<MessageModel>
        {
            new MessageModel { Id = 1, Message = "Hello" },
            new MessageModel { Id = 2, Message = "World" }
        };
            _controller.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _userServiceMock.Setup(s => s.GetUserId(token))
                .ReturnsAsync(expectedUserId);
            _messageServiceMock.Setup(s => s.GetAllMessagesByUserId(expectedUserId))
                .ReturnsAsync(expectedMessages);
            var result = await _controller.userMessages();
            Assert.NotNull(result);
            Assert.Equal(expectedMessages, result);
        }

        [Fact]
        public async Task userMessages_ReturnsNull_WhenAuthorizationHeaderMissing()
        {
            var result = await _controller.userMessages();
            Assert.Null(result);
        }

        [Fact]
        public async Task userMessages_ReturnsNull_WhenUserDoesNotExist()
        {
            string token = "sampletoken";
            _controller.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _userServiceMock.Setup(s => s.GetUserId(token))
                .ReturnsAsync((int?)null);

            var result = await _controller.userMessages();

            Assert.Null(result);
        }

        [Fact]
        public async Task userMessages_ReturnsNull_WhenExceptionIsThrown()
        {
            string token = "sampletoken";
            _controller.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _userServiceMock.Setup(s => s.GetUserId(token))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await _controller.userMessages();
            Assert.Null(result);
        }
    }
}
