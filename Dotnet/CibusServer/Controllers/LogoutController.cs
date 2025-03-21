using CibusServer.Interfaces;
using CibusServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CibusServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class LogoutController : ControllerBase
    {

        readonly IUserService _UserService;
        public LogoutController(IUserService userService)
        {
            _UserService = userService;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                // Remove "Bearer " prefix to extract the token
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var res = await _UserService.Logout(token);
                if (res) return Ok();
                else
                {
                    LogService.LogError("Exception when removing token");
                    return BadRequest("Exception when removing token");
                }
            }
            else
            {
                LogService.LogError("Authorization header not found");
                return BadRequest("Authorization header not found");
            }


        }


    }
}
