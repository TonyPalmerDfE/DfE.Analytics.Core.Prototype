using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    /// <summary>
    /// Defines a destination for analytics events that can asynchronously process or forward events for tracking
    /// purposes.
    /// </summary>
    /// <remarks>Implementations of this interface represent endpoints or services that receive analytics
    /// events, such as logging systems, telemetry platforms, or external analytics providers. Implementers should
    /// ensure thread safety if the destination is intended to be used concurrently.</remarks>
    public interface IAnalyticsEventDestination
    {
        Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default);
    }
}
