using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Events;
using Google.Cloud.BigQuery.V2;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DfE.Analytics.Core.BigQuery;

public class BigQueryAnalyticsDestination : IAnalyticsEventDestination
{
    private readonly BigQueryClient _client;
    private readonly BigQueryAnalyticsOptions _options;

    public BigQueryAnalyticsDestination(
        BigQueryClient client,
        IOptions<BigQueryAnalyticsOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default)
    {
        var row = new BigQueryInsertRow
        {
            { "event_name", evt.EventName },
            { "timestamp", evt.Timestamp },
            { "correlation_id", evt.CorrelationId },
            { "data", JsonSerializer.Serialize(evt.Data) },
            { "metadata", JsonSerializer.Serialize(evt.Metadata) }
        };

        await _client.InsertRowAsync(
            _options.Dataset,
            _options.Table,
            row,
            cancellationToken: cancellationToken);
    }
}
