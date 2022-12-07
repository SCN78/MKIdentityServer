using Microsoft.Extensions.Options;
using MKIdentityServer.Context;

namespace MKIdentityServer.Identity.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;       

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;           
        }

        public async Task Invoke(HttpContext context, TokenHelper jwtUtils, IDataContext dataContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if(!string.IsNullOrEmpty(token))
            {
                var user = dataContext.GetUserByToken(token);
                if(user != null)
                {                   
                    if (jwtUtils.ValidateCurrentToken(token, user.UserSecret))
                    {
                        // attach user to context on successful jwt validation
                        context.Items["User"] = user;
                    }
                }                
            }
            await _next(context);
        }
    }
}
