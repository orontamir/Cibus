using CibusServer.DAL.SQL.Entities;
using CibusServer.Interfaces;
using CibusServer.Models;
using CibusServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CibusServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        readonly IMessageService _MessageService;
        readonly IUserService _UserService;
        public MessagesController(IMessageService messageService, IUserService userService)
        {
            _MessageService = messageService;
            _UserService = userService;
        }

        [HttpGet]
        public async Task<List<MessageModel>> Messages()
        {
            try
            {
                return await _MessageService.GetAllMessages();
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Messages([FromBody] string message)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                // Remove "Bearer " prefix to extract the token
                var token = authHeader.Substring("Bearer ".Length).Trim();
                int? userid = await _UserService.GetUserId(token);
                if (userid != null)
                {
                    MessageModel messageModel = new MessageModel
                    {
                        Message = message,
                        UserId = userid.Value,
                        Vote = 1
                    };
                    var res = await _MessageService.AddMessage(messageModel);
                    if (res) return Ok();
                    else
                    {
                        LogService.LogError($"Exception when Add new message: {message}");
                        return BadRequest($"Exception when Add new message: {message}");
                    }

                }
                else
                {
                    LogService.LogError($"User not exist in the Data Base");
                    return BadRequest($"User not exist in the Data Base");
                }

            }
            else
            {
                LogService.LogError($"Authorization header not found");
                return BadRequest("Authorization header not found");
            }
        }

        [HttpPost("{message_id}/Vote")]
        public async Task<IActionResult> Vote(int message_id, [FromBody] int value)
        {
            try
            {
                //Check if exist message by message id
                MessageEntity messageEnity = await _MessageService.GetMessageByMessageId(message_id);
                if (messageEnity != null)
                {
                    //If exist 
                    //update the vote from value in message
                    await _MessageService.UpdateMessage(messageEnity, value);
                    return Ok();
                }
                else
                {
                    //if not exist throw new Exception 
                    LogService.LogError($"Message id {message_id} not exist");
                    return BadRequest($"Message id {message_id} not exist");
                }

            }
            catch (Exception ex)
            {
                LogService.LogError($"Exception when update message id {message_id} , error message: {ex.Message}");
                return BadRequest(ex.Message);
            }


        }

        [HttpDelete("{message_id}")]
        public async Task<IActionResult> Delete(int message_id)
        {
            try
            {
                //Get message by message id
                MessageEntity messageEnity = await _MessageService.GetMessageByMessageId(message_id);
                if (messageEnity != null)
                {
                    //If message exist 
                    //Remove message
                    await _MessageService.RemoveMessage(messageEnity);
                    return Ok();
                }
                else
                {
                    //if not exist throw new Exception 
                    LogService.LogError($"Message id {message_id} not exist");
                    return BadRequest($"Message id {message_id} not exist");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError($"Exception when delete message id {message_id} , error message: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
