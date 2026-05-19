using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    public interface IAnalyticsClient
    {
        Task TrackAsync(AnalyticsEventEnvelope evt, CancellationToken cancellationToken = default);
    }
}
