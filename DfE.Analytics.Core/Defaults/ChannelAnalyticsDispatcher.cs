using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Events;
using System.Threading.Channels;

namespace DfE.Analytics.Core.Tracking
{
    public class ChannelAnalyticsDispatcher : IAnalyticsDispatcher
    {
        private readonly ChannelWriter<AnalyticsEventEnvelope> _writer;

        public ChannelAnalyticsDispatcher(ChannelWriter<AnalyticsEventEnvelope> writer)
        {
            ArgumentNullException.ThrowIfNull(writer);
            _writer = writer;
        }

        public Task DispatchAsync(AnalyticsEventEnvelope evt, CancellationToken token)
        {
            _writer.TryWrite(evt);
            return Task.CompletedTask;
        }
    }
}
