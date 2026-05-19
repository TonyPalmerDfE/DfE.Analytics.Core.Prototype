# DfE Analytics Core
A lightweight, framework‑agnostic library for emitting structured analytics events across DfE services.

## Overview
The system is built around four key concepts:
- <b>Analytics Events</b> — A named event with strongly typed data and optional metadata.
- <b>Enrichers</b> — Components that add contextual information (e.g., correlation IDs) before an event is dispatched.
- <b>Dispatchers</b> — Responsible for sending events to a background channel.
- <b>Exporters</b> — Final destinations that process or forward events (e.g., console, BigQuery, Azure, Storage).

If defaults are enabled, events are written to a channel, then processed by a hosted background worker to avoid blocking application code.

## Install
```
dotnet add package DfE.Analytics.Core
```

Register the core:
```
builder.Services.AddDfEAnalyticsCore();
```

Add the default channel-based dispatcher:
```
builder.Services.AddDfEAnalyticsDefaultDispatcher();
```

Add the default console exporter:
```
builder.Services.AddDfEAnalyticsDefaultExporter();
```

(Optional) Add your own custom enrichers or exporters:
```
services.AddAnalyticsEnricher<IAnalyticsEnricher, MyEnricher>();
services.AddAnalyticsExporter<IAnalyticsExporter, MyExporter>();
```


## Usage

1. Define your event data:
```
public record UserSignedInData(string UserId) : IAnalyticsEventData;
```

2. Create and track an event
```
var evt = new AnalyticsEventEnvelope("user_signed_in", new UserSignedInData("12345"))
    .WithMetadata("Source", "web_application");

await analyticsClient.TrackAsync(evt);
```


## Add enrichers (optional)
Enrichers add metadata automatically.

Example: HTTP enricher
```
public class HttpEnricher : IAnalyticsEnricher
{
    private readonly IHttpContextAccessor _http;

    public HttpEnricher(IHttpContextAccessor http) => _http = http;

    public void Enrich(AnalyticsEvent evt, AnalyticsCorrelationContext context)
    {
        var http = _http.HttpContext;
        if (http == null) return;

        evt.Metadata["http_method"] = http.Request.Method;
        evt.Metadata["request_path"] = http.Request.Path;
    }
}
```

## Add exporters (optional)
Exporters decide where events go.

Example: Console exporter
```
public class ConsoleAnalyticsExporter : IAnalyticsExporter
{
    public Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Event: {evt.EventName}");
        return Task.CompletedTask;
    }
}
```