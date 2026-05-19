using DfE.Analytics.Core.Abstractions;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.Analytics.Core.BigQuery;

public static class BigQueryServiceCollectionExtensions
{
    public static IServiceCollection AddBigQueryAnalyticsDestination(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection(BigQueryAnalyticsOptions.SectionName);
        BigQueryAnalyticsOptions options = new();
        section.Bind(options);

        if (!options.Enabled)
            return services;

        bool missingConfig =
            string.IsNullOrWhiteSpace(options.ProjectId) ||
            string.IsNullOrWhiteSpace(options.Dataset) ||
            string.IsNullOrWhiteSpace(options.Table);

        if (missingConfig)
        {
            throw new InvalidOperationException(
                "BigQuery analytics is enabled, but required configuration values are missing or empty.");
        }

        services.Configure<BigQueryAnalyticsOptions>(opt =>
            configuration.GetSection(BigQueryAnalyticsOptions.SectionName).Bind(opt));

        services.AddSingleton(provider =>
            BigQueryClient.Create(options.ProjectId));

        services.AddScoped<IAnalyticsExporter, BigQueryAnalyticsDestination>();

        return services;
    }
}

