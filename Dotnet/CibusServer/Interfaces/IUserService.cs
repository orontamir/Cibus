using CibusServer.Models;

namespace CibusServer.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> Register(LoginModel model);
        Task<TokenMessageModel?> Login(LoginModel model);
        Task<bool> Logout(string token);
        Task<int?> GetUserId(string token);
    }
}
