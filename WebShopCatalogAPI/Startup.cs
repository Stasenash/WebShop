using Microsoft.EntityFrameworkCore;
using WebShopAdminAPI.Db;
using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace WebShopCatalogAPI
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

            services.Configure<CatalogDatabaseSettings>(configuration.GetSection(nameof(CatalogDatabaseSettings)));
            services.AddSingleton<ICatalogDatabaseSettings>(sp => sp.GetRequiredService<IOptions<CatalogDatabaseSettings>>().Value);

            services.AddSingleton<CategoryService>();
            services.AddSingleton<ItemService>();

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
