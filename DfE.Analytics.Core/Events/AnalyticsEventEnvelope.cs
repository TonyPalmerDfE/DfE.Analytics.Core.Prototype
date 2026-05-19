using DfE.Analytics.Core.Abstractions;

namespace DfE.Analytics.Core.Events
{
    public sealed record AnalyticsEventEnvelope
    {
        public string EventName { get; }
        public IAnalyticsEventData Data { get; }

        public string CorrelationId { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public Dictionary<string, string> Metadata { get; init; } = new();

        public AnalyticsEventEnvelope(string eventName, IAnalyticsEventData data)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentException("EventName cannot be null or empty.", nameof(eventName));
            if (data is null)
                throw new ArgumentNullException(nameof(data), "Data cannot be null.");

            EventName = eventName;
            Data = data;
        }

        public AnalyticsEventEnvelope WithMetadata(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                Metadata[key] = value ?? string.Empty;

            return this;
        }
    }
}
