using Azure.AI.Inference;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using DocAI.Models;
using DocAI.Services.Agents;
using DocAI.Services.General;
using DocAI.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DocAI.Services.Data;

public class DocumentProcessingService(
    PdfService pdfService,
    IOptions<DocumentProcessingOptions> options,
    ILogger<DocumentProcessingService> logger,
    ILoggerFactory loggerFactory)
    : IDocumentProcessingService
{
    private readonly DocumentProcessingOptions options = options.Value;

    public async Task<(ExtractedData Data, ValidationResult Validation)> ProcessPdfAsync(string filePath)
    {
        logger.LogInformation("Starting document processing for file: {FilePath}", filePath);

        try
        {
            var credential = new DefaultAzureCredential();
            var clientOptions = new AzureAIInferenceClientOptions();
            var tokenPolicy = new BearerTokenAuthenticationPolicy(credential,
                ["https://cognitiveservices.azure.com/.default"]);
            clientOptions.AddPolicy(tokenPolicy, HttpPipelinePosition.PerRetry);
            var mainChatClient =
                new SimpleChatClient(new ChatCompletionsClient(new Uri(options.FoundryEndpointMain), credential, clientOptions), options.MainModelName);
            var miniChatClient = new SimpleChatClient(new ChatCompletionsClient(new Uri(options.FoundryEndpointMini), credential, clientOptions), options.MiniModelName);

            // Create agents
            var pdfReaderAgent = new PdfReaderAgent(miniChatClient);
            var analyzerAgent = new AnalyzerAgent(mainChatClient, loggerFactory.CreateLogger<AnalyzerAgent>());
            var validatorAgent = new ValidatorAgent(loggerFactory.CreateLogger<ValidatorAgent>());
            
            var orchestrator = new OrchestratorAgent(
                pdfService,
                pdfReaderAgent,
                analyzerAgent,
                validatorAgent,
                mainChatClient,
                loggerFactory.CreateLogger<OrchestratorAgent>());

            // Process the PDF
            return await orchestrator.ProcessPdfAsync(filePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing PDF file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<(ExtractedData Data, ValidationResult Validation)> ProcessPdfAsync(Stream fileStream, string fileName)
    {
        logger.LogInformation("Starting document processing for stream: {FileName}", fileName);

        try
        {
            // Save stream to temp file
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}_{fileName}");
            
            await using (var fileStreamWriter = File.Create(tempFilePath))
            {
                await fileStream.CopyToAsync(fileStreamWriter);
            }

            try
            {
                // Process the temp file
                return await ProcessPdfAsync(tempFilePath);
            }
            finally
            {
                // Clean up temp file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing PDF stream: {FileName}", fileName);
            throw;
        }
    }
}
