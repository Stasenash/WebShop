using Serilog;
using Serilog.Events;
using WebShopAdminAPI;

var builder = WebApplication.CreateBuilder(args);
    
builder.Host.UseSerilog((host, log) =>
    {
        if (host.HostingEnvironment.IsProduction())
            log.MinimumLevel.Information();
        else
            log.MinimumLevel.Debug();

        log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        log.MinimumLevel.Override("Quartz", LogEventLevel.Information);
        log.WriteTo.Console();
    });
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app);
app.Run();
