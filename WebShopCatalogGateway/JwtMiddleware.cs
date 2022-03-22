using Microsoft.EntityFrameworkCore;
using WebShopCatalogGateway.Db;

namespace WebShopCatalogGateway
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserDbContext db, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { Message = "Missing authorization header" });
            }
            else
            {
                var userId = jwtUtils.ValidateToken(token);
                if (userId != null)
                {
                    // attach user to context on successful jwt validation
                    context.Items["User"] = db.Users.Where(x => x.Id == userId).Include(x => x.Roles).FirstOrDefault();
                }

                await _next(context);
            }
        }
    }
}
