using Microsoft.AspNetCore.Mvc;
using MKIdentityServer.Helpers;
using MKIdentityServer.Identity.Services;
using MKIdentityServer.Models;

namespace MKIdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthService _authHandler;
        public AccountsController(IAuthService authHandler)
        {
            _authHandler = authHandler;
        }
        [Route("Token")]
        [HttpPost]
        public async Task<IActionResult> Token([FromBody]AuthDto authDto)
        {          
            var user = await _authHandler.GetAuthorizedUser(authDto);
            if (user == null)
            {
                return Unauthorized("Invalid User");
            }          
            return Ok(user);
        }
    }
}
