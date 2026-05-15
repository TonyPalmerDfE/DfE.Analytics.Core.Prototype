using DfE.Analytics.Core.Abstractions;

namespace DfE.Analytics.Core.Events
{
    /// <summary>
    /// Represents a base type for analytics events, encapsulating event name, associated data, and metadata for
    /// tracking and analysis purposes.
    /// </summary>
    /// <remarks>Inherit from this record to define specific analytics events with custom event data. The
    /// event name and data are required for each event instance. Use the CorrelationId property to associate related
    /// events, and the Metadata dictionary to include additional context or custom attributes relevant to the event.
    /// The Timestamp property records when the event was created, using UTC by default.</remarks>
    public abstract record AnalyticsEvent
    {
        public string EventName { get; init; }
        public IAnalyticsEventData Data { get; init; }

        public string CorrelationId { get; set; } = default!;
        public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
        public Dictionary<string, string> Metadata { get; init; } = new();

        protected AnalyticsEvent(string eventName, IAnalyticsEventData data)
        {
            EventName = eventName;
            Data = data;
        }
    }

    /// <summary>
    /// Represents a strongly typed analytics event with associated event data.
    /// </summary>
    /// <remarks>This abstract record provides a base for analytics events that require strongly typed event
    /// data. Inherit from this type to define specific analytics events with custom data payloads.</remarks>
    /// <typeparam name="TData">The type of data associated with the analytics event. Must implement <see cref="IAnalyticsEventData"/>.</typeparam>
    public abstract record AnalyticsEvent<TData> : AnalyticsEvent
    where TData : IAnalyticsEventData
    {
        public new TData Data { get; }

        protected AnalyticsEvent(string eventName, TData data) : base(eventName, data)
        {
            Data = data;
        }
    }
}
