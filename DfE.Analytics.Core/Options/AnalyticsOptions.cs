namespace DfE.Analytics.Core.Options
{
    public class AnalyticsOptions
    {
        public const string SectionName = "DfEAnalytics";

        /// <summary>
        /// Gets or sets a value indicating whether the feature is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the name of the service associated with this instance.
        /// </summary>
        public string ServiceName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the application environment name.
        /// </summary>
        /// <remarks>Use this property to specify or retrieve the current environment, such as
        /// "Development", "Staging", or "Production". The value may influence configuration, logging, or behavior based
        /// on the environment.</remarks>
        public string Environment { get; set; } = default!;
    }

    /// <summary>
    /// Represents configuration options for an analytics destination.
    /// </summary>
    /// <remarks>This is an abstract base class for specifying options related to analytics destinations.
    /// Derived classes should provide additional properties or behavior specific to a particular analytics destination
    /// type.</remarks>
    public abstract class AnalyticsDestinationOptions
    {
        public bool Enabled { get; set; } = true;
    }
}
