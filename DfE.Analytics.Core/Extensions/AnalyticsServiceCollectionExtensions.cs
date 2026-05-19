using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Context;
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
            services.AddScoped<AnalyticsContext>();
            services.AddScoped<IAnalyticsClient, AnalyticsClient>();

            return services;
        }

        public static IServiceCollection AddDfEAnalyticsDefaultDispatcher(this IServiceCollection services)
        {
            Channel<AnalyticsEventEnvelope> channel = Channel.CreateBounded<AnalyticsEventEnvelope>(
                new BoundedChannelOptions(10000)
                {
                    FullMode = BoundedChannelFullMode.DropOldest
                });

            services.AddSingleton(channel.Writer);
            services.AddSingleton(channel.Reader);

            services.AddSingleton<IAnalyticsDispatcher, ChannelAnalyticsDispatcher>();
            services.AddHostedService<AnalyticsBackgroundWorker>();

            return services;
        }

        public static IServiceCollection AddDfEAnalyticsDefaultExporter(this IServiceCollection services)
        {
            services.AddScoped<IAnalyticsExporter, ConsoleAnalyticsExporter>();

            return services;
        }

        public static IServiceCollection AddAnalyticsEnricher<T>(this IServiceCollection services)
            where T : class, IAnalyticsEnricher
        {
            services.AddScoped<IAnalyticsEnricher, T>();
            return services;
        }

        public static IServiceCollection AddAnalyticsExporter<T>(this IServiceCollection services)
            where T : class, IAnalyticsExporter
        {
            services.AddScoped<IAnalyticsExporter, T>();
            return services;
        }
    }
}
