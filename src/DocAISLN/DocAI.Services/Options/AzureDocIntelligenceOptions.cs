namespace DocAI.Services.Options;

public class AzureDocIntelligenceOptions
{
    public const string SectionName = "AzureDocIntelligence";
    public required string DocumentEndpoint { get; set; }
    public string ModelId { get; set; } = "prebuilt-layout";
}