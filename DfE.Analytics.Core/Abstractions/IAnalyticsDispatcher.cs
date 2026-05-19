using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    public interface IAnalyticsDispatcher
    {
        Task DispatchAsync(AnalyticsEventEnvelope evt, CancellationToken token);
    }
}
