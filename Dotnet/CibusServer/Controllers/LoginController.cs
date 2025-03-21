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
    public class LoginController : ControllerBase
    {
        readonly IUserService _UserService;
        public LoginController(IUserService userService)
        {
            _UserService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenMessageModel>> Login([FromBody] LoginModel model)
        {
            var res = await _UserService.Login(model);
            if (res == null)
            {
                LogService.LogError($"Log in with user : {model.UserName} and password: {model.Password} Failed");
                return Unauthorized(new TokenMessageModel
                {
                    Token = "Login Failed"
                });
            }
               

            return Ok(new TokenMessageModel
            {
                Token = res.Token,
                UserId = res.UserId,
            });
        }

    }
}
