using CibusServer.DAL.SQL.Entities;

namespace CibusServer.Models.Extensions
{
    public static class UserExtension
    {
        public static UserModel ToUserModel(this UserEntity s)
        {

            return new UserModel
            {
                Id = s.Id,
                UserName = s.UserName,
                Password = s.Password,
            };


        }

    }
}
