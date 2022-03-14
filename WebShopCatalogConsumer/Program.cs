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
    public static async Task Main()
    {
        ServiceProvider serviceProvider = RegisterServices();
        await SetupMasstransit(serviceProvider);

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

    private static async Task SetupMasstransit(IServiceProvider serviceProvider)
    {
        var consumer = (CatalogConsumer)serviceProvider.GetRequiredService(typeof(CatalogConsumer));

        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        var port = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
        var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
        var password = Environment.GetEnvironmentVariable("RABBITMQ_PASS");

        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host(new Uri("rabbitmq://guest:guest@rabbitmq:5672"));
            cfg.ReceiveEndpoint("catalog-queue", e =>
            {
                e.Instance(consumer);
            });
        });

        var source = new CancellationTokenSource();
        await busControl.StartAsync(source.Token);
        try
        {
            Console.WriteLine("Consumer started");
            await Task.Run(() => Console.ReadLine());
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
}