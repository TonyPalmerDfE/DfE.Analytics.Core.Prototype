using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Context;
using DfE.Analytics.Core.Events;
using System.Text.Json;

namespace DfE.Analytics.Core.Consumer.Mvc.Analytics
{
    // The initial search -  did they search urn, town, did they get results back
    public record SchoolSearchData(string SearchTerm, int ResultCount) : IAnalyticsEventData;
    // The interaction - are people actually using a filter
    public record FilterAppliedData(string FilterName, string SelectedValue, int ResultsAfterFilter) : IAnalyticsEventData;
    // The drill-down - which schools did they click on
    public record SchoolDetailsViewedData(string Urn, string SchoolName, string Phase) : IAnalyticsEventData;
    // The outcome - did they leave the service or go to a schools website
    public record ExternalWebsiteClickedData(string Urn, string DestinationUrl) : IAnalyticsEventData;


    // ENRICHER
    public class UserEnricher : IAnalyticsEnricher
    {
        public Task EnrichAsync(AnalyticsEventEnvelope evt, AnalyticsContext context)
        {
            context.CorrelationId = context.CorrelationId ?? Guid.NewGuid().ToString();

            evt.CorrelationId = context.CorrelationId;
            evt.WithMetadata("Environment", "Local");

            return Task.CompletedTask;
        }
    }

    // DESTINATION
    public class DebugConsoleDestination : IAnalyticsExporter
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
        };

        public Task TrackAsync(AnalyticsEventEnvelope evt, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("========== ANALYTICS EVENT ==========");
            Console.WriteLine($"EventName: {evt.EventName}");
            Console.WriteLine($"Correlation ID: {evt.CorrelationId}");
            Console.WriteLine($"Timestamp (UTC): {evt.Timestamp:O}");
            Console.WriteLine($"Data: {JsonSerializer.Serialize((object)evt.Data, _jsonOptions)}");
            Console.WriteLine($"Metadata: {JsonSerializer.Serialize(evt.Metadata, _jsonOptions)}");
            Console.WriteLine("=====================================");

            return Task.CompletedTask;
        }
    }


    // Dummy session middleware
    public class SessionCorrelationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionCorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AnalyticsContext correlation)
        {
            const string key = "CorrelationId";

            if (!context.Session.TryGetValue(key, out var _))
            {
                string id = Guid.NewGuid().ToString();
                context.Session.SetString(key, id);
                correlation.CorrelationId = id;
            }
            else
            {
                correlation.CorrelationId = context.Session.GetString(key)!;
            }

            await _next(context);
        }
    }
}