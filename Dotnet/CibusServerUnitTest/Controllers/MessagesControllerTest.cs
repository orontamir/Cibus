using CibusServer.Controllers;
using CibusServer.DAL.SQL.Entities;
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
    public class MessagesControllerTest
    {
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly MessagesController _controller;

        public MessagesControllerTest()
        {
            _mockMessageService = new Mock<IMessageService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new MessagesController(_mockMessageService.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task GetMessages_ReturnsList_WhenServiceSucceeds()
        {
            var messages = new List<MessageModel>
        {
            new MessageModel { Message = "Hello", UserId = 1, Vote = 1 },
            new MessageModel { Message = "World", UserId = 2, Vote = 2 }
        };
            _mockMessageService.Setup(s => s.GetAllMessages()).ReturnsAsync(messages);
            var result = await _controller.Messages();
            Assert.Equal(messages, result);
        }

        [Fact]
        public async Task GetMessages_ReturnsNull_WhenServiceThrowsException()
        {
            _mockMessageService.Setup(s => s.GetAllMessages()).ThrowsAsync(new Exception("Error"));
            var result = await _controller.Messages();
            Assert.Null(result);
        }

        [Fact]
        public async Task PostMessage_ReturnsBadRequest_WhenAuthorizationHeaderMissing()
        {
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            string testMessage = "Test Message";
            var result = await _controller.Messages(testMessage);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Authorization header not found", badRequest.Value);
        }

        [Fact]
        public async Task PostMessage_ReturnsBadRequest_WhenUserIdIsNull()
        {
            var token = "some-token";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            string testMessage = "Test Message";

            _mockUserService.Setup(s => s.GetUserId(token)).ReturnsAsync((int?)null);
            var result = await _controller.Messages(testMessage);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User not exist in the Data Base", badRequest.Value);
        }

        [Fact]
        public async Task PostMessage_ReturnsOk_WhenMessageAddedSuccessfully()
        {
            var token = "valid-token";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            string testMessage = "Test Message";
            int userId = 1;

            _mockUserService.Setup(s => s.GetUserId(token)).ReturnsAsync(userId);
            _mockMessageService.Setup(s => s.AddMessage(It.Is<MessageModel>(m =>
                m.Message == testMessage &&
                m.UserId == userId &&
                m.Vote == 1)))
                .ReturnsAsync(true);
            var result = await _controller.Messages(testMessage);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task PostMessage_ReturnsBadRequest_WhenAddMessageFails()
        {
            var token = "valid-token";
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = $"Bearer {token}";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            string testMessage = "Test Message";
            int userId = 1;
            _mockUserService.Setup(s => s.GetUserId(token)).ReturnsAsync(userId);
            _mockMessageService.Setup(s => s.AddMessage(It.IsAny<MessageModel>())).ReturnsAsync(false);
            var result = await _controller.Messages(testMessage);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Exception when Add new message: {testMessage}", badRequest.Value);
        }

        [Fact]
        public async Task Vote_ReturnsBadRequest_WhenMessageDoesNotExist()
        {
            // Arrange
            int messageId = 1;
            int voteValue = 5;
            _mockMessageService.Setup(s => s.GetMessageByMessageId(messageId)).ReturnsAsync((MessageEntity)null);
            var result = await _controller.Vote(messageId, voteValue);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Message id {messageId} not exist", badRequest.Value);
        }

        [Fact]
        public async Task Vote_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            int messageId = 1;
            int voteValue = 5;
            _mockMessageService.Setup(s => s.GetMessageByMessageId(messageId)).ThrowsAsync(new Exception("Error during vote"));
            var result = await _controller.Vote(messageId, voteValue);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error during vote", badRequest.Value);
        }

       

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenMessageDoesNotExist()
        {
            int messageId = 1;
            _mockMessageService.Setup(s => s.GetMessageByMessageId(messageId)).ReturnsAsync((MessageEntity)null);
            var result = await _controller.Delete(messageId);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Message id {messageId} not exist", badRequest.Value);
        }

        [Fact]
        public async Task Delete_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            int messageId = 1;
            _mockMessageService.Setup(s => s.GetMessageByMessageId(messageId)).ThrowsAsync(new Exception("Delete error"));
            var result = await _controller.Delete(messageId);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Delete error", badRequest.Value);
        }
    }
}
