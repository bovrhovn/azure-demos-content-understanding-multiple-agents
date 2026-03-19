using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.Identity;
using DocAI.Models;
using DocAI.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DocAI.Services.Data;

public class AzureDocIntelligenceService(
    IOptions<AzureDocIntelligenceOptions> options,
    ILogger<AzureDocIntelligenceService> logger)
    : IAzureDocIntelligenceService
{
    private readonly AzureDocIntelligenceOptions _options = options.Value;

    public async Task<ExtractedData> AnalyzeDocumentAsync(string filePath)
    {
        logger.LogInformation("Starting Azure Document Intelligence analysis for file: {FilePath}", filePath);

        try
        {
            var identity = new DefaultAzureCredential();
            var client = new DocumentIntelligenceClient(new Uri(_options.DocumentEndpoint), identity);

            await using var stream = File.OpenRead(filePath);
            var binaryData = await BinaryData.FromStreamAsync(stream);

            logger.LogInformation("Calling Azure Document Intelligence with model: {ModelId}", _options.ModelId);
            
            var operation = await client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                _options.ModelId,
                binaryData);

            var result = operation.Value;

            // Convert to ExtractedData model
            var extractedData = new ExtractedData
            {
                FileName = Path.GetFileName(filePath),
                RawText = string.Empty,
                Persons = [],
                Emails = [],
                Tables = [],
                Metadata = new Dictionary<string, string>
                {
                    ["PageCount"] = result.Pages.Count.ToString(),
                    ["ModelId"] = _options.ModelId,
                    ["Source"] = "AzureDocumentIntelligence"
                }
            };

            // Extract text from all pages
            var textBuilder = new System.Text.StringBuilder();
            for (var p = 0; p < result.Pages.Count; p++)
            {
                var page = result.Pages[p];
                textBuilder.AppendLine($"\n--- Page {p + 1} ({page.Width} x {page.Height}, unit: {page.Unit}) ---");

                if (page.Lines != null)
                {
                    foreach (var line in page.Lines)
                    {
                        textBuilder.AppendLine(line.Content);
                    }
                }
            }
            extractedData.RawText = textBuilder.ToString();

            // Extract tables
            if (result.Tables is { Count: > 0 })
            {
                logger.LogInformation("Found {TableCount} tables", result.Tables.Count);
                
                for (var t = 0; t < result.Tables.Count; t++)
                {
                    var table = result.Tables[t];
                    var tableData = new TableData
                    {
                        Title = $"Table {t + 1}",
                        Headers = [],
                        Rows = []
                    };

                    // Extract table structure
                    var tableDict = new Dictionary<(int row, int col), string>();
                    int maxRow = 0, maxCol = 0;

                    foreach (var cell in table.Cells)
                    {
                        tableDict[(cell.RowIndex, cell.ColumnIndex)] = cell.Content;
                        maxRow = Math.Max(maxRow, cell.RowIndex);
                        maxCol = Math.Max(maxCol, cell.ColumnIndex);
                    }

                    // First row as headers
                    if (maxRow >= 0)
                    {
                        for (int col = 0; col <= maxCol; col++)
                        {
                            tableData.Headers.Add(tableDict.GetValueOrDefault((0, col), ""));
                        }

                        // Remaining rows as data
                        for (int row = 1; row <= maxRow; row++)
                        {
                            var rowData = new List<string>();
                            for (int col = 0; col <= maxCol; col++)
                            {
                                rowData.Add(tableDict.GetValueOrDefault((row, col), ""));
                            }
                            tableData.Rows.Add(rowData);
                        }
                    }

                    extractedData.Tables.Add(tableData);
                }
            }
            else
            {
                logger.LogInformation("No tables detected");
            }

            logger.LogInformation("Document analysis completed. Pages: {PageCount}, Tables: {TableCount}", 
                extractedData.Metadata["PageCount"], extractedData.Tables.Count);

            return extractedData;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing document with Azure Document Intelligence: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<ExtractedData> AnalyzeDocumentAsync(Stream fileStream, string fileName)
    {
        logger.LogInformation("Starting Azure Document Intelligence analysis for stream: {FileName}", fileName);

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
                return await AnalyzeDocumentAsync(tempFilePath);
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
            logger.LogError(ex, "Error analyzing document stream with Azure Document Intelligence: {FileName}", fileName);
            throw;
        }
    }
}
