using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebShopBasketAPI.Db;

namespace WebShopBasketAPI
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

            services.Configure<BasketDatabaseSettings>(_configuration.GetSection(nameof(BasketDatabaseSettings)));
            services.AddSingleton<IBasketDatabaseSettings>(sp => sp.GetRequiredService<IOptions<BasketDatabaseSettings>>().Value);

            services.AddSingleton<BasketService>();
            services.AddSingleton<DataService>();
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
