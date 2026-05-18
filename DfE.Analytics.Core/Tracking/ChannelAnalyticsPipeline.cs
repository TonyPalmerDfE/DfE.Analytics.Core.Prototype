using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Events;
using System.Threading.Channels;

namespace DfE.Analytics.Core.Tracking
{
    public class ChannelAnalyticsPipeline : IAnalyticsPipeline
    {
        private readonly ChannelWriter<AnalyticsEvent> _writer;

        public Task ProcessAsync(AnalyticsEvent evt, CancellationToken token)
        {
            _writer.TryWrite(evt);
            return Task.CompletedTask;
        }
    }
}
