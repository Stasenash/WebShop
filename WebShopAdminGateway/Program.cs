using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Events;
using System.Text;
using WebShopAdminAPI.Db;
using WebShopAdminGateway;
using WebShopAdminGateway.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, log) =>
{
    if (host.HostingEnvironment.IsProduction())
        log.MinimumLevel.Information();
    else
        log.MinimumLevel.Debug();

    log.MinimumLevel.Override("Ocelot", LogEventLevel.Information);
    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
    log.WriteTo.Console();
});

builder.Services.AddSingleton<IConfiguration>(_ => builder.Configuration);
builder.Services.AddDbContext<UserDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb")));
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services
    .AddOcelot()
    .AddDelegatingHandler<FillUserHandler>();

var app = builder.Build();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<StopOcelotMiddleware>();
app.UseMiddleware<JwtMiddleware>();

//var configuration = new OcelotPipelineConfiguration
//{
//    PreErrorResponderMiddleware = async (context, next) =>
//    {
//        string path = context.Request.Path.ToString();
//        context.Response.StatusCode = StatusCodes.Status200OK;
//        await context.Response.WriteAsJsonAsync(new
//        {
//            Message = "Testing"
//        });

//        await next.Invoke();
//    }
//};

app.UseOcelot().Wait();

//app.MapWhen(context => true, app => 
//{
//    int b = 10;

//    app.Run(async context =>
//    {
//        string path = context.Request.Path.ToString();
//        int a = 10;

//        context.Response.StatusCode = StatusCodes.Status200OK;
//        await context.Response.WriteAsJsonAsync(new
//        {
//            Message = "Testing"
//        });
//    });
//});

// Configure the HTTP request pipeline
//app.MapPost("/auth", async (HttpContext context, [FromBody] AuthUserRequest request) =>
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var db = scope.ServiceProvider.GetService<UserDbContext>() as UserDbContext;
//        var user = db?.Users.Where(x => x.UserName == request.UserName).Include(x => x.Roles).FirstOrDefault();
//        if (user == null)
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            await context.Response.WriteAsJsonAsync(new
//            {
//                Message = "User not found"
//            });
//            return;
//        }
//        else if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            await context.Response.WriteAsJsonAsync(new
//            {
//                Message = "Wrong password"
//            });
//            return;
//        }

//        var jwtUtils = scope.ServiceProvider.GetService<IJwtUtils>() as IJwtUtils;
//        if (jwtUtils == null) throw new Exception("jwtUtils are null");

//        context.Response.StatusCode = StatusCodes.Status200OK;
//        await context.Response.WriteAsJsonAsync(new
//        {
//            Token = jwtUtils.GenerateToken(user.Id)
//        });
//    }
//});

//app.MapPost("/register", async (HttpContext context, [FromBody] AuthUserRequest request) =>
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var db = scope.ServiceProvider.GetService<UserDbContext>() as UserDbContext;
//        if (db == null) throw new Exception($"{nameof(UserDbContext)} is null");

//        var existingUser = db.Users.Where(x => x.UserName == request.UserName).Include(x => x.Roles).FirstOrDefault();
//        if (existingUser != null && !BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash))
//        {
//            context.Response.StatusCode = StatusCodes.Status400BadRequest;
//            await context.Response.WriteAsJsonAsync(new
//            {
//                Message = $"User {request.UserName} already exists, but password is wrong, so admin role cannot be added"
//            });
//            return;
//        }

//        if (existingUser != null && existingUser.Roles.Any(x => x.Name == "Admin"))
//        {
//            context.Response.StatusCode = StatusCodes.Status200OK;
//            await context.Response.WriteAsJsonAsync(new
//            {
//                Message = $"User {request.UserName} already has admin rights"
//            });
//            return;
//        }

//        var adminRole = db.Roles.First(x => x.Name == "Admin");
//        if (existingUser != null)
//        {
//            existingUser.Roles.Add(adminRole);
//            await db.SaveChangesAsync();

//            context.Response.StatusCode = StatusCodes.Status200OK;
//            await context.Response.WriteAsJsonAsync(new
//            {
//                Message = $"User {request.UserName} was added admin rights"
//            });
//            return;
//        }

//        var user = new User
//        {
//            UserName = request.UserName,
//            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
//            Roles = new List<Role> { adminRole }
//        };

//        db.Users.Add(user);
//        await db.SaveChangesAsync();

//        context.Response.StatusCode = StatusCodes.Status200OK;
//        await context.Response.WriteAsJsonAsync(new
//        {
//            Message = $"User {request.UserName} registered successfully"
//        });
//    }
//});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<UserDbContext>();
    context.Database.Migrate();
}

app.Run();

