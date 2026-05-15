using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Correlation;
using DfE.Analytics.Core.Events;
using DfE.Analytics.Core.Options;
using Microsoft.Extensions.Options;

namespace DfE.Analytics.Core.Tracking
{
    /// <summary>
    /// Provides functionality to track analytics events by enriching them and forwarding them to one or more
    /// destinations.
    /// </summary>
    /// <remarks>The AnalyticsTracker coordinates the enrichment and delivery of analytics events. It applies
    /// all configured enrichers to each event before sending the event to all registered destinations. This class is
    /// typically used to centralize analytics event processing within an application.</remarks>
    public class AnalyticsTracker : IAnalyticsTracker
    {
        private readonly IEnumerable<IAnalyticsEventDestination> _destinations;
        private readonly IEnumerable<IAnalyticsEnricher> _enrichers;
        private readonly AnalyticsCorrelationContext _context;
        private readonly AnalyticsOptions _options;

        public AnalyticsTracker(
            IEnumerable<IAnalyticsEventDestination> destinations,
            IEnumerable<IAnalyticsEnricher> enrichers,
            AnalyticsCorrelationContext context,
            IOptions<AnalyticsOptions> options)
        {
            _destinations = destinations;
            _enrichers = enrichers;
            _context = context;
            _options = options.Value;
        }

        public async Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
                return;

            foreach (IAnalyticsEnricher enricher in _enrichers)
                enricher.Enrich(evt, _context);

            foreach (IAnalyticsEventDestination dest in _destinations)
                await dest.TrackAsync(evt, cancellationToken);
        }
    }
}
