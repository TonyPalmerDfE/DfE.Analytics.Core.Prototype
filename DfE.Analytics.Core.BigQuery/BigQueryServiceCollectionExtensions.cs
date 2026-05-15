using DfE.Analytics.Core.Abstractions;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DfE.Analytics.Core.BigQuery;

public static class BigQueryServiceCollectionExtensions
{
    public static IServiceCollection AddBigQueryAnalyticsDestination(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var section = configuration.GetSection("Analytics:Destinations:BigQuery");
        var enabled = section.GetValue<bool>("Enabled");

        // 1. If not enabled -> do nothing
        if (!enabled)
            return services;

        // 2. If enabled but missing required config -> fallback to console preview
        var projectId = section.GetValue<string>("ProjectId");
        var dataset = section.GetValue<string>("Dataset");
        var table = section.GetValue<string>("Table");

        var missingConfig = string.IsNullOrWhiteSpace(projectId)
                         || string.IsNullOrWhiteSpace(dataset)
                         || string.IsNullOrWhiteSpace(table);

        if (missingConfig)
        {
            services.AddScoped<IAnalyticsEventDestination, BigQueryConsolePreviewDestination>();
            return services;
        }

        // 3. Fully configured -> register real BigQuery destination
        //services.Configure<BigQueryAnalyticsOptions>(section);

        services.AddSingleton(provider =>
        {
            var opts = provider.GetRequiredService<IOptions<BigQueryAnalyticsOptions>>().Value;
            return BigQueryClient.Create(opts.ProjectId);
        });

        services.AddScoped<IAnalyticsEventDestination, BigQueryAnalyticsDestination>();

        return services;
    }

}
