using DfE.Analytics.Core.Context;
using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Abstractions
{
    public interface IAnalyticsEnricher
    {
        Task EnrichAsync(AnalyticsEventEnvelope evt, AnalyticsContext context);
    }
}
