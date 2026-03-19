using DocAI.Models;
using DocAI.Services.General;
using Microsoft.Extensions.Logging;

namespace DocAI.Services.Agents;

public class OrchestratorAgent(
    PdfService pdfService,
    PdfReaderAgent pdfReaderAgent,
    AnalyzerAgent analyzerAgent,
    ValidatorAgent validatorAgent,
    IChatClient chatClient,
    ILogger<OrchestratorAgent> logger)
{
    public async Task<(ExtractedData Data, ValidationResult Validation)> ProcessPdfAsync(string filePath)
    {
        logger.LogInformation("Orchestrator: Starting PDF extraction for '{FileName}'", Path.GetFileName(filePath));

        // Step 1: Extract raw text from PDF
        logger.LogInformation("Step 1: Extracting text from PDF...");
        var extractedData = pdfService.ExtractText(filePath);
        logger.LogInformation("Extracted {CharCount} characters from {PageCount} pages", 
            extractedData.RawText.Length, extractedData.Metadata["PageCount"]);

        // Step 2: Analyze text structure
        logger.LogInformation("Step 2: Analyzing document structure...");
        var structure = await pdfReaderAgent.AnalyzeTextStructure(extractedData.RawText);
        logger.LogInformation("Structure analysis: {Structure}", structure);

        // Step 3: Extract structured data
        logger.LogInformation("Step 3: Extracting structured data (persons, emails, tables)...");
        extractedData = await analyzerAgent.ExtractStructuredData(extractedData);
        logger.LogInformation("Found {PersonCount} persons, {EmailCount} emails, {TableCount} tables", 
            extractedData.Persons.Count, extractedData.Emails.Count, extractedData.Tables.Count);

        // Step 4: Validate extracted data
        logger.LogInformation("Step 4: Validating extracted data...");
        var validation = await validatorAgent.ValidateExtractedData(extractedData);
        logger.LogInformation("Validation complete: {ValidCount} valid, {InvalidCount} invalid, {UnconfirmedCount} unconfirmed",
            validation.ValidItems.Count, validation.InvalidItems.Count, validation.UnconfirmedItems.Count);

        // Step 5: Generate summary
        logger.LogInformation("Step 5: Generating summary...");
        await GenerateSummary(extractedData, validation);

        return (extractedData, validation);
    }

    private async Task GenerateSummary(ExtractedData data, ValidationResult validation)
    {
        var prompt = $"""
            Provide a brief executive summary of the PDF data extraction:
            - Document: {data.FileName}
            - Pages: {data.Metadata.GetValueOrDefault("PageCount", "Unknown")}
            - Persons found: {data.Persons.Count}
            - Emails found: {data.Emails.Count}
            - Tables found: {data.Tables.Count}
            - Valid items: {validation.ValidItems.Count}
            - Invalid items: {validation.InvalidItems.Count}
            - Unconfirmed items: {validation.UnconfirmedItems.Count}
            
            Provide a 2-3 sentence summary of the extraction quality and key findings.
            """;

        var response = await chatClient.CompleteAsync([new ChatMessage(ChatRole.User, prompt)]);
        logger.LogInformation("Summary: {Summary}", response.Message.Content);
    }
}
