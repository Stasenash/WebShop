using Microsoft.EntityFrameworkCore;
using WebShopAdminAPI.Db;
using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace WebShopAdminAPI
{
    public class Startup
    {
        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }
        public IConfigurationRoot Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSingleton(_ => Configuration);

            services.AddDbContext<AdminDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("AdminDb")));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                    var port = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
                    var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
                    var password = Environment.GetEnvironmentVariable("RABBITMQ_PASS");

                    cfg.Host(new Uri(host), credentials =>
                    {
                        credentials.Username(userName);
                        credentials.Password(password);
                    });
                });
            });
            services.AddMassTransitHostedService();
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(x => x.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AdminDbContext>();
                context.Database.Migrate();
            }
        }
    }
}
