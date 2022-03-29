using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebShopOrderConsumer;
using WebShopOrderConsumer.Db;

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
        services.AddDbContext<OrderDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("OrderDb")));

        services.AddSingleton<OrderService>();
        services.AddSingleton<OrderConsumer>();

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
        var consumer = (OrderConsumer)serviceProvider.GetRequiredService(typeof(OrderConsumer));        

        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
            var port = Environment.GetEnvironmentVariable("RABBITMQ_PORT");
            var userName = Environment.GetEnvironmentVariable("RABBITMQ_USER");
            var password = Environment.GetEnvironmentVariable("RABBITMQ_PASS");

            cfg.Host(new Uri($"rabbitmq://{userName}:{password}@{host}:{port}"));
            cfg.ReceiveEndpoint("Basket-queue", e =>
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