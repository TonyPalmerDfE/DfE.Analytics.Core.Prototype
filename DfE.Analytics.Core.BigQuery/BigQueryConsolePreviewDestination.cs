using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Events;
using System.Text.Json;

namespace DfE.Analytics.Core.BigQuery;

public class BigQueryConsolePreviewDestination : IAnalyticsEventDestination
{
    public Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[BigQuery Preview] Event would be inserted:");
        Console.WriteLine(JsonSerializer.Serialize(new
        {
            evt.EventName,
            evt.Timestamp,
            evt.CorrelationId,
            evt.Data,
            evt.Metadata
        }, new JsonSerializerOptions { WriteIndented = true }));

        return Task.CompletedTask;
    }
}
