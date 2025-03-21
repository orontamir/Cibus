using CibusServer.DAL.SQL.Entities;

namespace CibusServer.Models.Extensions
{
    public static class MessageExtension
    {
        public static MessageModel ToMessageModel(this MessageEntity s)
        {

            return new MessageModel
            {
                Id = s.Id,
                Message = s.Message,
                UserId = s.UserId,
                Vote = s.Vote
            };


        }

        public static bool IsComper(this MessageModel s, MessageModel model) 
        {
            return s.Message.Equals(model.Message) && s.UserId == model.UserId;
        }

        public static MessageEntity ToEntity(this MessageModel s)
        {
            if (s == null)
                return new MessageEntity();
            return new MessageEntity
            {
                Id = s.Id,
                Message = s.Message,
                UserId = s.UserId,
                Vote = s.Vote
            };
        }
    }
}
