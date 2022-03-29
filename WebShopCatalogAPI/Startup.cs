using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebShopCatalogAPI.Db;

namespace WebShopCatalogAPI
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

            services.Configure<CatalogDatabaseSettings>(_configuration.GetSection(nameof(CatalogDatabaseSettings)));
            services.AddSingleton<ICatalogDatabaseSettings>(sp => sp.GetRequiredService<IOptions<CatalogDatabaseSettings>>().Value);

            services.AddSingleton<CategoryService>();

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
        }
    }
}
