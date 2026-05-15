using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    /// <summary>
    /// Defines a contract for tracking analytics events asynchronously.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for recording analytics events, such as
    /// user actions or system occurrences, to an external analytics service or storage. Implementations should be
    /// thread-safe if used in multi-threaded environments.</remarks>
    public interface IAnalyticsTracker
    {
        Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default);
    }
}
