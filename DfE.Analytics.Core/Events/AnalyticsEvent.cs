using DfE.Analytics.Core.Abstractions;

namespace DfE.Analytics.Core.Events
{
    /// <summary>
    /// Represents an immutable analytics event containing event data, metadata, and contextual information for tracking
    /// and analysis purposes.
    /// </summary>
    /// <remarks>Use this type to encapsulate all relevant information about a single analytics event,
    /// including its name, associated data, correlation identifier, timestamp, and any additional metadata. The event
    /// data is provided via the IAnalyticsEventData interface, allowing for flexible event payloads. The record is
    /// sealed and immutable except for the CorrelationId property, which can be set to associate related events.
    /// Metadata can be extended using the WithMetadata method to add custom key-value pairs for further event
    /// context.</remarks>
    public sealed record AnalyticsEvent
    {
        public string EventName { get; }
        public IAnalyticsEventData Data { get; }

        public string CorrelationId { get; set; } = string.Empty;
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public Dictionary<string, string> Metadata { get; init; } = new();

        public AnalyticsEvent(string eventName, IAnalyticsEventData data)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentException("EventName cannot be null or empty.", nameof(eventName));
            if (data is null)
                throw new ArgumentNullException(nameof(data), "Data cannot be null.");

            EventName = eventName;
            Data = data;
        }

        /// <summary>
        /// Adds or updates a metadata entry for the analytics event and returns the current instance for method
        /// chaining.
        /// </summary>
        /// <remarks>This method enables fluent configuration of analytics event metadata by supporting
        /// method chaining.</remarks>
        /// <param name="key">The key of the metadata entry to add or update. Cannot be null, empty, or consist only of white-space
        /// characters.</param>
        /// <param name="value">The value to associate with the specified metadata key. If null, an empty string is used.</param>
        /// <returns>The current <see cref="AnalyticsEvent"/> instance with the updated metadata.</returns>
        public AnalyticsEvent WithMetadata(string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(key))
                Metadata[key] = value ?? string.Empty;

            return this;
        }
    }
}
