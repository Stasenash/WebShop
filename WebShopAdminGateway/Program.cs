using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ocelot.DependencyInjection;
using System.Configuration;
using WebShopAdminAPI.Db;
using WebShopAdminGateway;
using WebShopAdminGateway.Db;
using BCrypt.Net;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("routes.json");

builder.Services.AddSingleton<IConfiguration>(_ => builder.Configuration);
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb")));
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<JwtUtils>();
builder.Services
    .AddOcelot()
    .AddDelegatingHandler<FillUserHandler>();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapPost("/auth", async (HttpContext context, [FromBody] AuthUserRequest request) =>
{
    var db = app.Services.GetService<UserDbContext>() as UserDbContext;
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
    else if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            Message = "Wrong password"
        });
        return;
    }

    var jwtUtils = app.Services.GetService<JwtUtils>() as JwtUtils;
    if (jwtUtils == null) throw new Exception("jwtUtils are null");

    context.Response.StatusCode = StatusCodes.Status200OK;
    await context.Response.WriteAsJsonAsync(new
    {
        Token = jwtUtils.GenerateToken(user.Id)
    });
});

app.MapPost("/register", async (HttpContext context, [FromBody] AuthUserRequest request) =>
{
    var db = app.Services.GetService<UserDbContext>() as UserDbContext;
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
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<UserDbContext>();
    context.Database.Migrate();
}

app.Run();

