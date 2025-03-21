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
    public class UserController : ControllerBase
    {
        readonly IUserService _UserService;
        readonly IMessageService _MessageService;
        public UserController(IUserService userService, IMessageService messageService)
        {
            _UserService = userService;
            _MessageService = messageService;
        }

        [HttpGet("Messages")]
        public async Task<List<MessageModel>?> userMessages()
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    // Remove "Bearer " prefix to extract the token
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    int? userid = await _UserService.GetUserId(token);
                    if (userid != null)
                    {

                        return await _MessageService.GetAllMessagesByUserId(userid.Value);
                    }
                    else
                    {
                        LogService.LogError($"User not exist in the Data Base");
                        return null;
                    }
                }
                else
                {
                    LogService.LogError($"Authorization header not found");
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
