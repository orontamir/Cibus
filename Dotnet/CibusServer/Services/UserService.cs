using CibusServer.DAL.SQL.Entities;
using CibusServer.DAL.SQL;
using CibusServer.Helpers;
using CibusServer.Interfaces;
using CibusServer.Models;
using CibusServer.Models.Extensions;

namespace CibusServer.Services
{
    public class UserService : DALService, IUserService
    {
        readonly JwtService _jwtService;

        public UserService(RepositoryBase repo, JwtService jwtService) : base(repo)
        {
            _jwtService = jwtService;
        }

        public async Task<TokenMessageModel?> Login(LoginModel model)
        {

            var hashedPassword = HashHelper.CalculateHash(model.Password, model.UserName);

            var userEntity = await Repository.Login(model.UserName, hashedPassword);
            if (userEntity != null)
            {
                var loginResult = new TokenMessageModel
                {
                    //Create new token
                    Token = _jwtService.GenerateSecurityToken_ByName(userEntity.UserName, userEntity.Id),
                    UserId = userEntity.Id.ToString()
                };
                _jwtService.SaveToken(userEntity.Id, loginResult.Token);
                return loginResult;
            }
            else
            {
                return null;
            }
        }

        public Task<bool> Logout(string token)
        {

            try
            {
                _jwtService.RemoveToken(token);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                LogService.LogError($"Exception when LogOut , error message: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task<UserModel> Register(LoginModel model)
        {
            model.Password = HashHelper.CalculateHash(model.Password, model.UserName);
            var entity = await Repository.GetUserByUserName(model.UserName);
            if (entity == null)
            {
                UserEntity userEntity = new UserEntity()
                {
                    UserName = model.UserName,
                    Password = model.Password
                };
                return Repository.AddUser(userEntity).Result.ToUserModel();
            }
            else
            {
                //If user name already exist , throw an exception
                throw new Exception($"user \"{model.UserName}\" already exists.\n Please try with a different user name. ");
            }
        }

        public Task<int?> GetUserId(string token)
        {
            return Task.FromResult(_jwtService.GetUserId(token));
        }
    }
}
