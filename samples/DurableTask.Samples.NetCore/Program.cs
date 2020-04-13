using DurableTask.AzureStorage;
using DurableTask.Core;
using DurableTask.ServiceBus.Tracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DurableTask.Samples.NetCore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices)
                .UseConsoleLifetime();
        }

        private static void ConfigureServices(
            HostBuilderContext hostContext,
            IServiceCollection services)
        {
            services.Configure<AppSettings>(
                hostContext.Configuration.GetSection("Application"));

            services.AddTransient<IOrchestrationServiceInstanceStore>(
                sp =>
                {
                    var options = sp.GetRequiredService<IOptions<AppSettings>>();
                    var settings = options.Value;
                    return new AzureTableInstanceStore(
                        settings.TaskHubName,
                        settings.StorageConnectionString);
                });

            services.AddTransient<AzureStorageOrchestrationServiceSettings>(
                sp =>
                {
                    var options = sp.GetRequiredService<IOptions<AppSettings>>();
                    var settings = options.Value;
                    return new AzureStorageOrchestrationServiceSettings
                    {
                        StorageConnectionString = settings.StorageConnectionString,
                        TaskHubName = settings.TaskHubName,
                    };
                });

            services.AddTransient<AzureStorageOrchestrationService>();
            services.AddTransient<IOrchestrationService>(
                sp => sp.GetRequiredService<AzureStorageOrchestrationService>());
            services.AddTransient<IOrchestrationServiceClient>(
                sp => sp.GetRequiredService<AzureStorageOrchestrationService>());

            services.AddTransient<TaskHubClient>();
            services.AddTransient<TaskHubWorker>();

            // services.AddTransient<ConfigService>();
            services.AddHostedService<DurableTaskService>();
        }
    }
}
