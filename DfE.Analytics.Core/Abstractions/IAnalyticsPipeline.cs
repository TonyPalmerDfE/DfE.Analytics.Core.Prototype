using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    public interface IAnalyticsPipeline
    {
        Task ProcessAsync(AnalyticsEvent evt, CancellationToken token);
    }
}
