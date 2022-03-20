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
            _configuration = configuration;
        }
        public IConfigurationRoot _configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddSingleton(_ => _configuration);

            services.AddDbContext<AdminDbContext>(options => options.UseNpgsql(_configuration.GetConnectionString("AdminDb")));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                    var port = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
                    var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
                    var password = Environment.GetEnvironmentVariable("RABBITMQ_PASS");

                    cfg.Host(new Uri($"rabbitmq://{userName}:{password}@{host}:{port}"));
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
