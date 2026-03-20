namespace DocAI.Services.Options;

public class DocumentProcessingOptions
{
    public const string SectionName = "AzureAI";
    public required string FoundryEndpointMain { get; set; }
    public required string FoundryEndpointMini { get; set; }
    public required string MainModelName { get; set; }
    public required string MiniModelName { get; set; }
}