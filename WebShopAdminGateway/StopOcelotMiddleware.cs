using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using WebShopAdminAPI.Db;
using WebShopAdminGateway;
using WebShopAdminGateway.Db;

public class StopOcelotMiddleware
{
    private readonly RequestDelegate _next;

    public StopOcelotMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, UserDbContext userDb, IJwtUtils jwtUtils)
    {
        string[] routesToIgnore = new string[] { "/auth", "/register" };

        string path = context.Request.Path.ToString();
        if (!routesToIgnore.Contains(context.Request.Path.ToString())) await _next(context);

        var request = await GetRequest(context);
        if (path == "/auth")
            await Auth(context, userDb, request, jwtUtils);
        else if (path == "/register")
            await Register(context, userDb, request, jwtUtils);
    }

    private async Task<AuthUserRequest> GetRequest(HttpContext context)
    {
        AuthUserRequest parsedrequest;
        var request = context.Request;

        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var requestContent = Encoding.UTF8.GetString(buffer);

        parsedrequest = JsonConvert.DeserializeObject<AuthUserRequest>(requestContent);

        request.Body.Position = 0;  //rewinding the stream to 0

        return parsedrequest;
    }

    private async Task Auth(HttpContext context, UserDbContext db, AuthUserRequest request, IJwtUtils jwtUtils)
    {
        var user = db?.Users.Where(x => x.UserName == request.UserName).Include(x => x.Roles).FirstOrDefault();
        if (user == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = "User not found"
            });
            return;
        }
        else if (!user.Roles.Any(x => x.Name == "Admin"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = "You are not admin"
            });
            return;
        }
        else if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = "Wrong password"
            });
            return;
        }

        if (jwtUtils == null) throw new Exception("jwtUtils are null");

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(new
        {
            Token = jwtUtils.GenerateToken(user.Id)
        });
    }

    private async Task Register(HttpContext context, UserDbContext db, AuthUserRequest request, IJwtUtils jwtUtils)
    {
        if (db == null) throw new Exception($"{nameof(UserDbContext)} is null");

        var existingUser = db.Users.Where(x => x.UserName == request.UserName).Include(x => x.Roles).FirstOrDefault();
        if (existingUser != null && !BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = $"User {request.UserName} already exists, but password is wrong, so admin role cannot be added"
            });
            return;
        }

        if (existingUser != null && existingUser.Roles.Any(x => x.Name == "Admin"))
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = $"User {request.UserName} already has admin rights"
            });
            return;
        }

        var adminRole = db.Roles.First(x => x.Name == "Admin");
        if (existingUser != null)
        {
            existingUser.Roles.Add(adminRole);
            await db.SaveChangesAsync();

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = $"User {request.UserName} was added admin rights"
            });
            return;
        }

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Roles = new List<Role> { adminRole }
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(new
        {
            Message = $"User {request.UserName} registered successfully"
        });
    }
}