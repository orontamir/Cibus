using CibusServer.Interfaces;
using CibusServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CibusServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class RegisterController : ControllerBase
    {
        readonly IUserService _UserService;
        public RegisterController(IUserService userService)
        {
            _UserService = userService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserModel>> Register([FromBody] LoginModel model)
        {
            return await _UserService.Register(model);

        }


    }
}
