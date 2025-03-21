using CibusServer.DAL.SQL.Entities;
using CibusServer.Models;

namespace CibusServer.Interfaces
{
    public interface IMessageService
    {
        Task<List<MessageModel>> GetAllMessages();
        Task<bool> AddMessage(MessageModel model);
        Task<MessageEntity> GetMessageByMessageId(int id);
        Task<bool> UpdateMessage(MessageEntity messageentity, int vote);
        Task<bool> RemoveMessage(MessageEntity messageentity);
        Task<List<MessageModel>> GetAllMessagesByUserId(int userid);
    }
}
