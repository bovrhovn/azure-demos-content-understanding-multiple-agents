using Azure;
using Azure.AI.DocumentIntelligence;
using Azure.Identity;
using Spectre.Console;

namespace DocAI.Console.Helpers;

public class DocAI(string docEndpoint, string modelId)
{
    public async Task AnalyzeAsync( string filePath)
    {
        AnsiConsole.MarkupLine("[blue]Starting with analysis[/]");
        var identity = new DefaultAzureCredential();
        var client = new DocumentIntelligenceClient(new Uri(docEndpoint), identity);

        await using var stream = File.OpenRead(filePath);

        var op = await client.AnalyzeDocumentAsync(
            WaitUntil.Completed,
            modelId,
            await BinaryData.FromStreamAsync(stream));
        
        var result = op.Value;

        AnsiConsole.WriteLine($"Model: {modelId}");
        AnsiConsole.WriteLine($"Pages: {result.Pages.Count}");

        for (var p = 0; p < result.Pages.Count; p++)
        {
            var page = result.Pages[p];
            AnsiConsole.WriteLine($"\n--- Page {p + 1} ({page.Width} x {page.Height}, " +
                                  $"unit: {page.Unit}) ---");

            if (page.Lines == null) continue;
            foreach (var line in page.Lines) AnsiConsole.WriteLine(line.Content);
        }

        // Print tables (if any)
        if (result.Tables is { Count: > 0 })
        {
            AnsiConsole.WriteLine($"\nTables found: {result.Tables.Count}");
            for (var t = 0; t < result.Tables.Count; t++)
            {
                var table = result.Tables[t];
                AnsiConsole.WriteLine($"\n== Table {t + 1}: {table.RowCount} rows x {table.ColumnCount} cols ==");

                foreach (var cell in table.Cells)
                {
                    AnsiConsole.WriteLine($"[{cell.RowIndex},{cell.ColumnIndex}] {cell.Content}");
                }
            }
        }
        else AnsiConsole.WriteLine("\nNo tables detected.");
    }
}