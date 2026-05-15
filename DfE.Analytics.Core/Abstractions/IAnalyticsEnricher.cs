using DfE.Analytics.Core.Correlation;
using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    /// <summary>
    /// Defines a contract for enriching analytics events with additional data before they are processed or logged.
    /// </summary>
    /// <remarks>Implementations of this interface can add custom properties or context information to
    /// analytics events. This is typically used to attach application-specific metadata, user information, or
    /// correlation data to events for improved analysis and diagnostics.</remarks>
    public interface IAnalyticsEnricher
    {
        void Enrich(AnalyticsEvent evt, AnalyticsCorrelationContext context);
    }
}
