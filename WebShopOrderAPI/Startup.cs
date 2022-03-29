using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebShopOrderAPI.Db;

namespace WebShopOrderAPI
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

            services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(_configuration.GetConnectionString("OrderDb")));

            services.AddScoped<OrderService>();
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(x => x.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<OrderDbContext>();
                context.Database.Migrate();
            }
        }
    }
}
