namespace DfE.Analytics.Core.Correlation
{
    /// <summary>
    /// Provides a context for correlating analytics events using a unique identifier.
    /// </summary>
    /// <remarks>Use this class to associate related analytics events with a common correlation ID, enabling
    /// end-to-end tracking across distributed systems or multiple operations.</remarks>
    public sealed class AnalyticsCorrelationContext
    {
        public string CorrelationId { get; set; } = default!;
    }
}
