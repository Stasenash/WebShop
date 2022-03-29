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
