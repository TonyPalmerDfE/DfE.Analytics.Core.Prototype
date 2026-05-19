using DfE.Analytics.Core.Abstractions;
using DfE.Analytics.Core.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DfE.Analytics.Core.Tracking
{
    public sealed class ConsoleAnalyticsExporter : IAnalyticsExporter
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public Task TrackAsync(AnalyticsEventEnvelope evt, CancellationToken token = default)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("──────────────────────────────────────────────");
            Console.WriteLine($"Analytics Event: {evt.EventName}");
            Console.WriteLine("──────────────────────────────────────────────");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Timestamp:     {evt.Timestamp:o}");
            Console.WriteLine($"CorrelationId: {evt.CorrelationId}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Data:");
            Console.ResetColor();
            Console.WriteLine(JsonSerializer.Serialize((object)evt.Data, JsonOptions));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Metadata:");
            Console.ResetColor();
            Console.WriteLine(JsonSerializer.Serialize(evt.Metadata, JsonOptions));

            Console.WriteLine();
            return Task.CompletedTask;
        }
    }

}
