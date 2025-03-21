using CibusServer.DAL.SQL.Entities;
using CibusServer.DAL.SQL;
using CibusServer.Helpers;
using CibusServer.Models;
using CibusServer.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CibusServer.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CibusServerUnitTest.Services
{
    public class UserServiceTest
    {
        private readonly Mock<RepositoryBase> _repoMock;
        private readonly Mock<JwtService> _jwtServiceMock;
        private readonly UserService _userService;
        public UserServiceTest()
        {
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["DB_CONNECTION_STRING"]).Returns("FakeConnectionString");

            _repoMock = new Mock<RepositoryBase>(configMock.Object);
            _jwtServiceMock = new Mock<JwtService>();
            _userService = new UserService(_repoMock.Object, _jwtServiceMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsTokenMessage_WhenUserExists()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "testUser", Password = "password" };
            string hashed = HashHelper.CalculateHash(loginModel.Password, loginModel.UserName);
            var userEntity = new UserEntity { Id = 1, UserName = "testUser", Password = hashed };

            _repoMock.Setup(r => r.Login(loginModel.UserName, hashed))
                     .ReturnsAsync(userEntity);
            _jwtServiceMock.Setup(j => j.GenerateSecurityToken_ByName(userEntity.UserName, userEntity.Id))
                           .Returns("FakeToken");

            // Act
            var result = await _userService.Login(loginModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("FakeToken", result.Token);
            Assert.Equal("1", result.UserId);
           
        }

        [Fact]
        public async Task Login_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "nonexistent", Password = "password" };
            string hashed = HashHelper.CalculateHash(loginModel.Password, loginModel.UserName);

            _repoMock.Setup(r => r.Login(loginModel.UserName, hashed))
                     .ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userService.Login(loginModel);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Logout_ReturnsTrue_WhenNoException()
        {
            // Arrange
            string token = "sometoken";
            _jwtServiceMock.Setup(j => j.RemoveToken(token));

            // Act
            var result = await _userService.Logout(token);

            // Assert
            Assert.True(result);
            _jwtServiceMock.Verify(j => j.RemoveToken(token), Times.Once);
        }

        [Fact]
        public async Task Logout_ReturnsFalse_WhenExceptionThrown()
        {
            // Arrange
            string token = "sometoken";
            _jwtServiceMock.Setup(j => j.RemoveToken(token)).Throws(new Exception("Test exception"));

            // Act
            var result = await _userService.Logout(token);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Register_ReturnsUserModel_WhenUserDoesNotExist()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "newUser", Password = "password" };
            string hashed = HashHelper.CalculateHash(loginModel.Password, loginModel.UserName);

            // Simulate that the user does not exist.
            _repoMock.Setup(r => r.GetUserByUserName(loginModel.UserName))
                     .ReturnsAsync((UserEntity)null);

            // When adding a user, return a new user entity with an Id.
            _repoMock.Setup(r => r.AddUser(It.Is<UserEntity>(u => u.UserName == loginModel.UserName && u.Password == hashed)))
                     .ReturnsAsync((UserEntity u) => {
                         u.Id = 10;
                         return u;
                     });

            // Act
            var result = await _userService.Register(loginModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("newUser", result.UserName);
        }

        [Fact]
        public async Task Register_ThrowsException_WhenUserAlreadyExists()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "existingUser", Password = "password" };
            string hashed = HashHelper.CalculateHash(loginModel.Password, loginModel.UserName);
            var existingUser = new UserEntity { Id = 5, UserName = "existingUser", Password = hashed };

            _repoMock.Setup(r => r.GetUserByUserName(loginModel.UserName))
                     .ReturnsAsync(existingUser);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _userService.Register(loginModel));
            Assert.Contains("already exists", ex.Message);
        }

        [Fact]
        public async Task GetUserId_ReturnsCorrectId()
        {
            // Arrange
            string token = "validToken";
            _jwtServiceMock.Setup(j => j.GetUserId(token)).Returns(42);

            // Act
            var result = await _userService.GetUserId(token);

            // Assert
            Assert.Equal(42, result);
        }
    }
}
