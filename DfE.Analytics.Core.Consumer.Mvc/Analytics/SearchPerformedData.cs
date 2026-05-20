using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Context;
using DfE.Analytics.Core.Events;
using System.Diagnostics;
using System.Text.Json;

namespace DfE.Analytics.Core.Consumer.Mvc.Analytics
{
    public record SchoolSearchData(string SearchTerm, int ResultCount) : IAnalyticsEventData;
    public record FilterAppliedData(string FilterName, string SelectedValue, int ResultsAfterFilter) : IAnalyticsEventData;
    public record SchoolDetailsViewedData(string Urn, string SchoolName, string Phase) : IAnalyticsEventData;
    public record ExternalWebsiteClickedData(string Urn, string DestinationUrl) : IAnalyticsEventData;
    public record WebRequestEventData(string Path, string Method, int StatusCode, long DurationMs) : IAnalyticsEventData;

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

    public class WebRequestEnricher : IAnalyticsEnricher
    {
        private readonly IHttpContextAccessor _http;

        public WebRequestEnricher(IHttpContextAccessor http)
        {
            _http = http;
        }

        public Task EnrichAsync(AnalyticsEventEnvelope evt, AnalyticsContext context)
        {
            HttpContext? httpContext = _http.HttpContext;
            if (httpContext is null)
                return Task.CompletedTask;

            evt.WithMetadata("RequestPath", httpContext.Request.Path);
            evt.WithMetadata("RequestMethod", httpContext.Request.Method);
            evt.WithMetadata("UserAgent", httpContext.Request.Headers["User-Agent"]);
            evt.WithMetadata("CorrelationId", context.CorrelationId); // maybe we don't need?

            return Task.CompletedTask;
        }
    }

    // DESTINATION
    public class DebugConsoleExporter : IAnalyticsExporter
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

    public class AnalyticsRequestMiddleware
    {
        private static readonly string[] IgnoredExtensions =
        {
            ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg",
            ".ico", ".woff", ".woff2", ".ttf", ".map"
        };

        private static readonly string[] IgnoredPaths =
        {
            "/health", "/metrics"
        };

        private readonly RequestDelegate _next;

        public AnalyticsRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAnalyticsClient analytics)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

            // Skip static assets
            if (IgnoredExtensions.Any(ext => path.EndsWith(ext)) ||
                IgnoredPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            AnalyticsEventEnvelope evt = new(
                "web_request",
                new WebRequestEventData(path, context.Request.Method, context.Response.StatusCode, sw.ElapsedMilliseconds)
            );

            await analytics.TrackAsync(evt);
        }

    }

}