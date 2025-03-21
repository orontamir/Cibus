using CibusServer.DAL.SQL.Entities;
using CibusServer.DAL.SQL;
using CibusServer.Models;
using CibusServer.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CibusServerUnitTest.Services
{
    public class MessageServiceTest
    {
        private readonly Mock<RepositoryBase> _repoMock;
        private readonly JwtService _jwtService;
        private readonly MessageService _messageService;

        public MessageServiceTest()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["DB_CONNECTION_STRING"]).Returns("FakeConnectionString");

            _repoMock = new Mock<RepositoryBase>(configMock.Object);
            _jwtService = new JwtService();
            _messageService = new MessageService(_repoMock.Object, _jwtService);
        }

        [Fact]
        public async Task GetAllMessages_ReturnsMappedMessageModels()
        {
            // Arrange
            var messages = new List<MessageEntity> {
            new MessageEntity { Id = 1, Message = "Hello", UserId = 1, Vote = 0 },
            new MessageEntity { Id = 2, Message = "World", UserId = 2, Vote = 1 }
        };
            _repoMock.Setup(r => r.GetAllMessages()).ReturnsAsync(messages);

            // Act
            var result = await _messageService.GetAllMessages();

            // Assert
            Assert.Equal(messages.Count, result.Count);
            Assert.Equal("Hello", result[0].Message);
            Assert.Equal("World", result[1].Message);
        }

        [Fact]
        public async Task GetAllMessagesByUserId_ReturnsMappedMessageModels()
        {
            // Arrange
            int userId = 1;
            var messages = new List<MessageEntity> {
            new MessageEntity { Id = 1, Message = "User Message", UserId = userId, Vote = 0 }
        };
            _repoMock.Setup(r => r.GetAllMessagesByUserId(userId)).ReturnsAsync(messages);

            // Act
            var result = await _messageService.GetAllMessagesByUserId(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("User Message", result[0].Message);
        }

        [Fact]
        public async Task GetMessageByMessageId_ReturnsCorrectMessageEntity()
        {
            // Arrange
            int messageId = 1;
            var messageEntity = new MessageEntity { Id = messageId, Message = "Test", UserId = 1, Vote = 0 };
            _repoMock.Setup(r => r.GetMessageById(messageId)).ReturnsAsync(messageEntity);

            // Act
            var result = await _messageService.GetMessageByMessageId(messageId);

            // Assert
            Assert.Equal(messageId, result.Id);
            Assert.Equal("Test", result.Message);
        }

        [Fact]
        public async Task AddMessage_ReturnsFalse_WhenMessageAlreadyExists()
        {
            // Arrange
            var messageModel = new MessageModel { Id = 1, Message = "Duplicate", UserId = 1, Vote = 0 };

            // Setup: user exists
            _repoMock.Setup(r => r.GetUserById(messageModel.UserId))
                     .ReturnsAsync(new UserEntity { Id = messageModel.UserId });
            // Setup: GetAllMessages returns a message that is considered a duplicate.
            var existingMessages = new List<MessageEntity> {
            new MessageEntity { Id = 2, Message = "Duplicate", UserId = 1, Vote = 0 }
        };
            _repoMock.Setup(r => r.GetAllMessages()).ReturnsAsync(existingMessages);

            // Act
            var result = await _messageService.AddMessage(messageModel);

            // Assert
            Assert.False(result);
           
        }

        [Fact]
        public async Task AddMessage_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var messageModel = new MessageModel { Id = 1, Message = "New Message", UserId = 99, Vote = 0 };

            // Setup: user does not exist
            _repoMock.Setup(r => r.GetUserById(messageModel.UserId))
                     .ReturnsAsync((UserEntity)null);
            // Setup: no messages exist.
            _repoMock.Setup(r => r.GetAllMessages()).ReturnsAsync(new List<MessageEntity>());

            // Act
            var result = await _messageService.AddMessage(messageModel);

            // Assert
            Assert.False(result);
          
        }

        [Fact]
        public async Task AddMessage_ReturnsTrue_WhenMessageIsAddedSuccessfully()
        {
            // Arrange
            var messageModel = new MessageModel { Id = 1, Message = "Unique Message", UserId = 1, Vote = 0 };

            // Setup: user exists
            _repoMock.Setup(r => r.GetUserById(messageModel.UserId))
                     .ReturnsAsync(new UserEntity { Id = messageModel.UserId });
            // Setup: no duplicate messages exist.
            _repoMock.Setup(r => r.GetAllMessages()).ReturnsAsync(new List<MessageEntity>());
            // Setup: simulate adding a message.
            _repoMock.Setup(r => r.AddMessage(It.IsAny<MessageEntity>()))
                     .ReturnsAsync((MessageEntity me) => { me.Id = 10; return me; });

            // Act
            var result = await _messageService.AddMessage(messageModel);

            // Assert
            Assert.True(result);
           
        }

     

        [Fact]
        public async Task UpdateMessage_ReturnsFalse_WhenUpdateThrowsException()
        {
            // Arrange
            var messageEntity = new MessageEntity { Id = 1, Message = "Update Fail", UserId = 1, Vote = 0 };
            int voteIncrement = 3;
            _repoMock.Setup(r => r.UpdateMessage(messageEntity)).ThrowsAsync(new Exception("Update failed"));

            // Act
            var result = await _messageService.UpdateMessage(messageEntity, voteIncrement);

            // Assert
            Assert.False(result);
        }

        

        [Fact]
        public async Task RemoveMessage_ReturnsFalse_WhenRemovalThrowsException()
        {
            // Arrange
            var messageEntity = new MessageEntity { Id = 1, Message = "Remove Fail", UserId = 1, Vote = 0 };
            _repoMock.Setup(r => r.RemoveMessage(messageEntity)).ThrowsAsync(new Exception("Removal failed"));

            // Act
            var result = await _messageService.RemoveMessage(messageEntity);

            // Assert
            Assert.False(result);
        }
    }
}
