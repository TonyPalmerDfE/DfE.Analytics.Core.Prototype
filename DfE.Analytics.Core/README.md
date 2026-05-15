# DfE.Analytics.Core
A lightweight, framework‑agnostic library for emitting structured analytics events across DfE services.

## What this library does
Gives you a consistent way to:
- Emit structured analytics events
- Add metadata automatically via enrichers
- Correlate events across layers and services
- Send events to any destination you choose

It does not dictate:
- What events you track
- Where events are sent
- What framework you use
- How your domain is structured

## Install
```
dotnet add package DfE.Analytics.Core
```

Register the core:
```
builder.Services.AddAnalyticsCore();
```

## Define an event
Create a data record:
```
public record EstablishmentRetrievedData(int Id, string Name)
    : IAnalyticsEventData;
```
Create the event:
```
public record EstablishmentRetrievedEvent(EstablishmentRetrievedData Data)
    : AnalyticsEvent<EstablishmentRetrievedData>("establishment_retrieved", Data);
```

## Track an event
Inject `IAnalyticsTracker` and call:
```
await tracker.TrackAsync(
    new EstablishmentRetrievedEvent(
        new EstablishmentRetrievedData(est.Id, est.Name)
    )
);
```

Aim is for it work in:
- APIs
- MVC
- Domain services
- Background workers

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

Register it:
```
builder.Services.AddHttpContextAccessor();
builder.Services.AddAnalyticsEnricher<HttpEnricher>();
```

## Add destinations (optional)
Destinations decide where events go.

Example: Console destination
```
public class ConsoleAnalyticsDestination : IAnalyticsEventDestination
{
    public Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Event: {evt.EventName}");
        return Task.CompletedTask;
    }
}
```

Register it:
```
builder.Services.AddAnalyticsDestination<ConsoleAnalyticsDestination>();
```

## Correlation (optional)
Add middleware to set correlation IDs:
```
app.UseMiddleware<HttpCorrelationMiddleware>();
```