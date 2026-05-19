using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Context;
using DfE.Analytics.Core.Events;

namespace DfE.Analytics.Core.Tracking
{
    public class AnalyticsClient : IAnalyticsClient
    {
        private readonly IAnalyticsDispatcher _dispatcher;
        private readonly IEnumerable<IAnalyticsEnricher> _enrichers;
        private readonly AnalyticsContext _context;

        public AnalyticsClient(
            IAnalyticsDispatcher dispatcher,
            IEnumerable<IAnalyticsEnricher> enrichers,
            AnalyticsContext context)
        {
            _dispatcher = dispatcher;
            _enrichers = enrichers;
            _context = context;
        }

        public async Task TrackAsync(AnalyticsEventEnvelope evt, CancellationToken cancellationToken = default)
        {
            foreach (IAnalyticsEnricher enricher in _enrichers)
                await enricher.EnrichAsync(evt, _context);

            await _dispatcher.DispatchAsync(evt, cancellationToken);
        }
    }

}
