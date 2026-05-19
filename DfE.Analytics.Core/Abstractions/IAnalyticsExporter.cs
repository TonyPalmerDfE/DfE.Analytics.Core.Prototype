using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    public interface IAnalyticsExporter
    {
        Task TrackAsync(AnalyticsEventEnvelope evt, CancellationToken cancellationToken = default);
    }
}
