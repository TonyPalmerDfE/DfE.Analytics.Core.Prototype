using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Correlation;
using DfE.Analytics.Core.Events;
using DfE.Analytics.Core.Tracking;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace DfE.Analytics.Core.Extensions
{
    public static class AnalyticsServiceCollectionExtensions
    {
        public static IServiceCollection AddDfEAnalyticsCore(this IServiceCollection services)
        {
            Channel<AnalyticsEvent> channel = Channel.CreateBounded<AnalyticsEvent>(new BoundedChannelOptions(10000)
            {
                FullMode = BoundedChannelFullMode.DropOldest
            });

            services.AddSingleton(channel.Writer);
            services.AddSingleton(channel.Reader);

            services.AddScoped<AnalyticsCorrelationContext>(); 
            services.AddScoped<IAnalyticsTracker, AnalyticsTracker>();
            services.AddHostedService<AnalyticsQueueProcessor>();

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
