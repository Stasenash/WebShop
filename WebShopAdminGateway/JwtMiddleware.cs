using Microsoft.EntityFrameworkCore;
using WebShopAdminAPI.Db;

namespace WebShopAdminGateway
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserDbContext _db;

        public JwtMiddleware(RequestDelegate next, UserDbContext db)
        {
            _next = next;
            _db = db;
        }

        public async Task Invoke(HttpContext context, User userService, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateToken(token);
            if (userId != null)
            {
                // attach user to context on successful jwt validation
                context.Items["User"] = _db.Users.Where(x => x.Id == userId).Include(x => x.Roles).FirstOrDefault();
            }

            await _next(context);
        }
    }
}
