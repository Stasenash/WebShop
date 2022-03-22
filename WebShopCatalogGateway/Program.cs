using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Events;
using System.Text;
using WebShopCatalogGateway.Db;
using WebShopCatalogGateway;

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
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services
    .AddOcelot()
    .AddDelegatingHandler<FillUserHandler>();

var app = builder.Build();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<StopOcelotMiddleware>();
app.UseMiddleware<JwtMiddleware>();
app.UseOcelot().Wait();

app.Run();

