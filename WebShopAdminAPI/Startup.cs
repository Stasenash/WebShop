using Microsoft.EntityFrameworkCore;
using WebShopAdminAPI.Db;

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
