using DocAI.Models;
using DocAI.Services.General;

namespace DocAI.Console.Agents;

public class OrchestratorAgent(
    PdfService pdfService,
    PdfReaderAgent pdfReaderAgent,
    AnalyzerAgent analyzerAgent,
    ValidatorAgent validatorAgent,
    IChatClient chatClient)
{
    public async Task<(ExtractedData Data, ValidationResult Validation)> ProcessPdfAsync(string filePath)
    {
        System.Console.WriteLine($"\n🤖 Orchestrator: Starting PDF extraction for '{Path.GetFileName(filePath)}'...\n");

        // Step 1: Extract raw text from PDF
        System.Console.WriteLine("📄 Step 1: Extracting text from PDF...");
        var extractedData = pdfService.ExtractText(filePath);
        System.Console.WriteLine($"   ✓ Extracted {extractedData.RawText.Length} characters from {extractedData.Metadata["PageCount"]} pages\n");

        // Step 2: Analyze text structure
        System.Console.WriteLine("🔍 Step 2: Analyzing document structure...");
        var structure = await pdfReaderAgent.AnalyzeTextStructure(extractedData.RawText);
        System.Console.WriteLine($"   {structure}\n");

        // Step 3: Extract structured data
        System.Console.WriteLine("📊 Step 3: Extracting structured data (persons, emails, tables)...");
        extractedData = await analyzerAgent.ExtractStructuredData(extractedData);
        System.Console.WriteLine($"   ✓ Found {extractedData.Persons.Count} persons");
        System.Console.WriteLine($"   ✓ Found {extractedData.Emails.Count} emails");
        System.Console.WriteLine($"   ✓ Found {extractedData.Tables.Count} tables\n");

        // Step 4: Validate extracted data
        System.Console.WriteLine("✅ Step 4: Validating extracted data...");
        var validation = await validatorAgent.ValidateExtractedData(extractedData);
        System.Console.WriteLine($"   ✓ Validation complete: {validation.ValidItems.Count} valid, " +
                                 $"{validation.InvalidItems.Count} invalid, {validation.UnconfirmedItems.Count} unconfirmed\n");

        // Step 5: Generate summary
        System.Console.WriteLine("📝 Step 5: Generating summary...");
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
        System.Console.WriteLine($"   {response.Message.Content}\n");
    }
}
