using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Correlation;
using DfE.Analytics.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

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
        private readonly IAnalyticsPipeline _pipeline;
        private readonly IEnumerable<IAnalyticsEnricher> _enrichers;
        private readonly AnalyticsCorrelationContext _context;

        public AnalyticsTracker(
            IAnalyticsPipeline pipeline,
            IEnumerable<IAnalyticsEnricher> enrichers,
            AnalyticsCorrelationContext context)
        {
            _pipeline = pipeline;
            _enrichers = enrichers;
            _context = context;
        }

        public async Task TrackAsync(AnalyticsEvent evt, CancellationToken cancellationToken = default)
        {
            foreach (IAnalyticsEnricher enricher in _enrichers)
                enricher.Enrich(evt, _context);

            await _pipeline.ProcessAsync(evt, cancellationToken);
        }
    }

    public class AnalyticsQueueProcessor : BackgroundService
    {
        private readonly ChannelReader<AnalyticsEvent> _reader;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AnalyticsQueueProcessor> _logger;

        public AnalyticsQueueProcessor(
            ChannelReader<AnalyticsEvent> reader,
            IServiceScopeFactory scopeFactory,
            ILogger<AnalyticsQueueProcessor> logger)
        {
            _reader = reader;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (AnalyticsEvent evt in _reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                IEnumerable<IAnalyticsEventDestination> destinations = scope.ServiceProvider.GetServices<IAnalyticsEventDestination>();

                foreach (IAnalyticsEventDestination destination in destinations)
                {
                    try
                    {
                        await destination.TrackAsync(evt, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Failed to track analytics event '{EventName}' to destination '{DestinationType}'",
                            evt.EventName, destination.GetType().Name);
                    }
                }
            }
        }
    }
}
