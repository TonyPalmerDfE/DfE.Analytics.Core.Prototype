namespace DfE.Analytics.Core.BigQuery;

public class BigQueryAnalyticsOptions
{
    public const string SectionName = "Analytics:BigQuery";
    public bool Enabled { get; set; } = true;

    public string ProjectId { get; set; } = default!;
    public string Dataset { get; set; } = default!;
    public string Table { get; set; } = default!;
    public bool UseBatchInsert { get; set; } = true;
}
