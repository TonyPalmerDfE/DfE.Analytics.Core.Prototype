using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace DfE.Analytics.Core.Tracking
{
    public class AnalyticsBackgroundWorker : BackgroundService
    {
        private readonly ChannelReader<AnalyticsEventEnvelope> _reader;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AnalyticsBackgroundWorker> _logger;

        public AnalyticsBackgroundWorker(
            ChannelReader<AnalyticsEventEnvelope> reader,
            IServiceScopeFactory scopeFactory,
            ILogger<AnalyticsBackgroundWorker> logger)
        {
            _reader = reader;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (AnalyticsEventEnvelope evt in _reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                IEnumerable<IAnalyticsExporter> destinations = scope.ServiceProvider.GetServices<IAnalyticsExporter>();

                foreach (IAnalyticsExporter destination in destinations)
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
