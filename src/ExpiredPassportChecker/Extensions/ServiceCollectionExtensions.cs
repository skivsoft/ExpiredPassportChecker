using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;

namespace ExpiredPassportChecker.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHangfireServices(this IServiceCollection services)
        {
            GlobalConfiguration.Configuration
                .UseMemoryStorage()
                ;

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());

            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 1;
            });
        }
    }
}