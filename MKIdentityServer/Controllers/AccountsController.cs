using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MKIdentityServer.Helpers;
using MKIdentityServer.Models;

namespace MKIdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthHandler _authHandler;
        public AccountsController(IAuthHandler authHandler)
        {
            _authHandler = authHandler;
        }
        [Route("Token")]
        [HttpPost]
        public async Task<IActionResult> Token([FromBody]AuthDto authDto)
        {          
            var user = await _authHandler.GetUser(authDto);
            if (user == null)
            {
                return Unauthorized("Invalid User");
            }          
            return Ok(user);
        }
    }
}
