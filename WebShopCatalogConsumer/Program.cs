using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Configuration;
using WebShopCatalogConsumer;
using WebShopCatalogConsumer.Db;

public class Program
{
    public static void Main()
    {
        ServiceProvider serviceProvider = RegisterServices();
        SetupMasstransit(serviceProvider);

        ILogger logger = serviceProvider.GetService<ILogger<Program>>();
        logger.LogInformation("Consumer started");

        serviceProvider.Dispose();
    }

    private static ServiceProvider RegisterServices()
    {
        IConfiguration configuration = SetupConfiguration();
        var services = new ServiceCollection();

        services.AddLogging(cfg => cfg.AddConsole());

        services.AddSingleton(configuration);
        services.Configure<CatalogDatabaseSettings>(configuration.GetSection(nameof(CatalogDatabaseSettings)));
        services.AddSingleton<ICatalogDatabaseSettings>(sp => sp.GetRequiredService<IOptions<CatalogDatabaseSettings>>().Value);

        services.AddSingleton<CategoryService>();
        services.AddSingleton<ItemService>();

        services.AddSingleton<CatalogConsumer>();

        return services.BuildServiceProvider();
    }

    private static IConfiguration SetupConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }

    private static void SetupMasstransit(IServiceProvider serviceProvider)
    {
        var consumer = serviceProvider.GetRequiredService(typeof(CatalogConsumer));

        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        var port = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
        var password = Environment.GetEnvironmentVariable("RABBITMQ_PASS");

        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(new Uri(host), credentials => 
            {
                credentials.Username(userName);
                credentials.Password(password);
            });

            cfg.ReceiveEndpoint("Catalog_Queue", e =>
            {
                e.Instance(consumer);
            });
        });
    }
}