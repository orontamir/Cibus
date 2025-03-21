using CibusServer.DAL.SQL.Entities;
using CibusServer.DAL.SQL;
using CibusServer.Interfaces;
using CibusServer.Models;
using CibusServer.Models.Extensions;

namespace CibusServer.Services
{
    public class MessageService: DALService, IMessageService
    {

        readonly JwtService _jwtService;

        public MessageService(RepositoryBase repo, JwtService jwtService) : base(repo)
        {
            _jwtService = jwtService;
        }

        public Task<List<MessageModel>> GetAllMessages()
        {
            return Task.FromResult(Repository.GetAllMessages().Result.Select(o => o.ToMessageModel()).ToList());

        }

        public virtual async Task<bool> AddMessage(MessageModel model)
        {
            try
            {
                bool isUserExist = await Repository.GetUserById(model.UserId) != null;
                bool isMessageExist = GetAllMessages().Result.Any(o => o.IsComper(model));
                if (isMessageExist)
                {
                    LogService.LogError($"Error Message: {model.Message} allready exist");
                    return false;
                }
                else if (!isUserExist)
                {
                    LogService.LogError($"Error Message: User not exist");
                    return false;
                }
                else
                {
                    MessageEntity messageentity = await Repository.AddMessage(model.ToEntity());
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.LogError($"Exception when add message , error message: {ex.Message}");
                return false;
            }

        }

        public Task<MessageEntity> GetMessageByMessageId(int id)
        {
            return Task.FromResult(Repository.GetMessageById(id).Result);
        }

        public async Task<bool> UpdateMessage(MessageEntity messageentity, int vote)
        {
            try
            {
                messageentity.Vote += vote;
                await Repository.UpdateMessage(messageentity);
                return true;

            }
            catch (Exception ex)
            {
                LogService.LogError($"Exception when update message id {messageentity.Id} with vote: {vote}, Error message {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveMessage(MessageEntity messageentity)
        {
            try
            {
                await Repository.RemoveMessage(messageentity);
                return true;

            }
            catch (Exception ex)
            {
                LogService.LogError($"Exception when remove message id {messageentity.Id} Error message: {ex.Message}");
                return false;
            }
        }

        public Task<List<MessageModel>> GetAllMessagesByUserId(int userid)
        {
            return Task.FromResult(Repository.GetAllMessagesByUserId(userid).Result.Select(o => o.ToMessageModel()).ToList());
        }
    }
}
