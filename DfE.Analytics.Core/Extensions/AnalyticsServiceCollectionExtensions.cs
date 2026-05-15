using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Correlation;
using DfE.Analytics.Core.Options;
using DfE.Analytics.Core.Tracking;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.Analytics.Core.Extensions
{
    public static class AnalyticsServiceCollectionExtensions
    {
        public static IServiceCollection AddAnalyticsCore(
            this IServiceCollection services,
            Action<AnalyticsOptions>? configure = null)
        {
            if (configure is not null)
                services.Configure(configure);

            services.AddScoped<AnalyticsCorrelationContext>();
            services.AddScoped<IAnalyticsTracker, AnalyticsTracker>();

            return services;
        }

        public static IServiceCollection AddAnalyticsEnricher<T>(this IServiceCollection services)
            where T : class, IAnalyticsEnricher
        {
            services.AddScoped<IAnalyticsEnricher, T>();
            return services;
        }

        public static IServiceCollection AddAnalyticsDestination<T>(this IServiceCollection services)
            where T : class, IAnalyticsEventDestination
        {
            services.AddScoped<IAnalyticsEventDestination, T>();
            return services;
        }
    }
}
